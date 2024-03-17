using ARCVX.Extensions;
using ARCVX.Reader;
using ARCVX.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using ZLibDotNet;

namespace ARCVX.Formats
{
    public class ARC : Base<ARCHeader>
    {
        public override int MAGIC { get; } = 0x41524300; // "ARC."
        public override int MAGIC_LE { get; } = 0x00435241; // ".CRA"

        private List<ARCEntry> _entries;
        public List<ARCEntry> Entries
        {
            get
            {
                _entries ??= GetEntries();
                return _entries;
            }
        }

        public ARC(FileInfo file) : base(file) { }
        public ARC(FileInfo file, Stream stream) : base(file, stream) { }

        public override ARCHeader GetHeader()
        {
            Reader.SetPosition(0);

            return IsValid ? new ARCHeader
            {
                Magic = Reader.ReadInt32(ByteOrder.LittleEndian),
                Version = Reader.ReadInt16(),
                Count = Reader.ReadInt16(),
            } : new();
        }

        public List<ARCEntry> GetEntries()
        {
            if (!IsValid)
                return null;

            Reader.SetPosition(8);

            List<ARCEntry> entries = [];

            for (int i = 0; i < Header.Count; i++)
            {
                ARCEntry entry = new();

                long position = Reader.GetPosition();

                entry.Path = Reader.ReadNullTerminatedString();

                Reader.SetPosition(position + 64);

                entry.TypeHash = Reader.ReadInt32();
                entry.DataSize = Reader.ReadUInt32();

                uint size = Reader.ReadUInt32();
                entry.FileSize = size & 0x00FFFFFF;
                entry.Flags = (byte)(size >> 24);

                entry.Offset = Reader.ReadUInt32();

                entries.Add(entry);
            }

            return entries;
        }

        public MemoryStream GetEntryStream(ARCEntry entry)
        {
            if (!IsValid)
                return null;

            Reader.SetPosition(entry.Offset);

            MemoryStream stream = new();
            Stream.CopyTo(stream);
            stream.SetLength((int)entry.DataSize);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        public string GetEntryPath(ARCEntry entry) =>
            Path.ChangeExtension(entry.Path, GetEntryExtension(entry));

        public string GetEntryExtension(ARCEntry entry) =>
            GetTypeExtension(entry.TypeHash);

        public string GetTypeExtension(int hash) =>
            TypeMap.ContainsKey(hash) ? TypeMap[hash] : hash.ToString("X8");

        public ARCExport ExportEntryData(ARCEntry entry, DirectoryInfo folder)
        {
            FileInfo outputFile = new(Path.Join(folder.FullName, Path.ChangeExtension(entry.Path, GetEntryExtension(entry))));

            try
            {
                if (!outputFile.Directory.Exists)
                    outputFile.Directory.Create();

                if (outputFile.Exists)
                    outputFile.Delete();
            }
            catch
            {
                return null;
            }

            try
            {
                using MemoryStream entryStream = GetEntryStream(entry);
                using FileStream outputStream = outputFile.OpenWrite();

                ZlibHeader zlibHeader = new(entryStream.ReadByte(), entryStream.ReadByte());

                entryStream.Seek(0, SeekOrigin.Begin);

                if (!zlibHeader.IsValid())
                {
                    using ZLibStream zlibStream = new(entryStream, CompressionMode.Decompress);
                    zlibStream.CopyTo(outputStream);
                }
                else
                    entryStream.CopyTo(outputStream);
            }
            catch
            {
                return null;
            }

            return new()
            {
                File = outputFile,
                Entry = entry,
            };
        }

        public IEnumerable<ARCExport> ExportAllEntries() =>
            ExportAllEntries(File.Directory);

        public IEnumerable<ARCExport> ExportAllEntries(DirectoryInfo folder)
        {
            foreach (ARCEntry entry in Entries)
                yield return ExportEntryData(entry, folder);
        }

        public MemoryStream CreateHeaderStream()
        {
            MemoryStream stream = new();

            stream.Write(Bytes.GetValueBytes(Header.Magic, ByteOrder.LittleEndian));
            stream.Write(Bytes.GetValueBytes(Header.Version));
            stream.Write(Bytes.GetValueBytes(Header.Count));

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public MemoryStream CreateEntriesStream(List<ARCEntry> entries)
        {
            MemoryStream stream = new();

            foreach (ARCEntry entry in entries)
            {
                uint flags = (uint)entry.Flags << 24;
                flags |= entry.FileSize & 0x00FFFFFF;

                stream.Write(Bytes.GetStringBytes(entry.Path, 0x40));
                stream.Write(Bytes.GetValueBytes(entry.TypeHash));
                stream.Write(Bytes.GetValueBytes(entry.DataSize));
                stream.Write(Bytes.GetValueBytes(flags));
                stream.Write(Bytes.GetValueBytes(entry.Offset));
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public MemoryStream CreateNewStream(DirectoryInfo folder)
        {
            using MemoryStream newStream = new();
            List<ARCEntry> newEntries = new();

            long offset = HEADER_SIZE + (Entries.Count * 0x50);
            foreach (ARCEntry entry in Entries)
            {
                using MemoryStream dataStream = new();
                using MemoryStream entryStream = GetEntryStream(entry);

                ARCEntry newEntry = entry;
                FileInfo inputFile = new(Path.Join(folder.FullName, GetEntryPath(entry)));

                if (inputFile.Exists)
                {
                    using FileStream inputStream = inputFile.OpenReadShared();
                    ZlibHeader zlibHeader = new(entryStream.ReadByte(), entryStream.ReadByte());

                    if (zlibHeader.IsValid())
                    {
                        ZLib zlib = new();

                        uint length = zlib.CompressBound((uint)inputFile.Length);

                        Span<byte> compressedData = new byte[length];
                        Span<byte> uncompressedData = new byte[inputFile.Length];

                        inputStream.Read(uncompressedData);

                        ZStream zStream = new()
                        {
                            Input = uncompressedData,
                            Output = compressedData
                        };

                        _ = zlib.DeflateInit(ref zStream, ZLib.Z_DEFAULT_COMPRESSION, ZLib.Z_DEFLATED, zlibHeader.CINFO + 8, 8, ZLib.Z_DEFAULT_STRATEGY);
                        _ = zlib.Deflate(ref zStream, ZLib.Z_FULL_FLUSH);
                        _ = zlib.DeflateEnd(ref zStream);

                        dataStream.Write(compressedData.Slice(0, (int)zStream.TotalOut));
                    }
                    else
                        inputStream.CopyTo(dataStream);

                    newEntry.FileSize = (uint)inputFile.Length;
                    newEntry.DataSize = (uint)dataStream.Length;
                }
                else
                    entryStream.CopyTo(dataStream);

                newEntry.Offset = (uint)offset;

                offset += dataStream.Length;

                newEntries.Add(newEntry);

                dataStream.Seek(0, SeekOrigin.Begin);
                dataStream.CopyTo(newStream);
            }

            MemoryStream outputStream = new();

            using (MemoryStream stream = CreateHeaderStream())
                stream.CopyTo(outputStream);

            using (MemoryStream stream = CreateEntriesStream(newEntries))
                stream.CopyTo(outputStream);

            newStream.Seek(0, SeekOrigin.Begin);
            newStream.CopyTo(outputStream);

            outputStream.Seek(0, SeekOrigin.Begin);
            return outputStream;
        }

        public FileInfo Save(DirectoryInfo folder) =>
            Save(folder, File);

        public FileInfo Save(DirectoryInfo folder, FileInfo file)
        {
            FileInfo tempFile = new(Path.Join(File.DirectoryName, "_" + Path.GetRandomFileName()));

            try
            {
                if (!tempFile.Directory.Exists)
                    tempFile.Directory.Create();

                using (FileStream tempStream = tempFile.OpenWrite())
                    using (MemoryStream newStream = CreateNewStream(folder))
                        newStream.CopyTo(tempStream);

                tempFile.MoveTo(file.FullName, true);

                return file;
            }
            catch
            {
                try { tempFile.Delete(); } catch { }
                return null;
            }
        }

        private Dictionary<int, string> TypeMap { get; } = new()
        {
            {0x02358E1A, "spkg"},
            {0x051BE0EC, "rut"},
            {0x070078B5, "mnt"},
            {0x0949A1DA, "adl"},
            {0x09C48A11, "unk"},
            {0x0A736313, "mdl"}, // wpn mdl
            {0x108F442E, "mdl"}, // ene mdl
            {0x130124FA, "rmh"},
            {0x167DBBFF, "stq"},
            {0x18FF29AB, "cut"},
            {0x1BCC4966, "srq"},
            {0x1BE1DBEB, "dom"},
            {0x232E228C, "rev"},
            {0x241F5DEB, "tex"},
            {0x24339E8C, "evl"},
            {0x2749C8A8, "mrl"},
            {0x28D65BFA, "pos"},
            {0x2ADFA358, "pvl"},
            {0x348C831D, "evc"},
            {0x375F06DA, "evt"},
            {0x40171000, "dat"},
            {0x42940D09, "fmt"},
            {0x4356673E, "man"},
            {0x46C78353, "mes"}, // rmn mes
            {0x4C0DB839, "sdl"},
            {0x5DF3D947, "mry"},
            {0x5FF4BE71, "ene"},
            {0x6505B384, "ddsp"},
            {0x681835FC, "itm"},
            {0x6A76E771, "mes"}, // adv mes
            {0x6B0369B1, "atr"},
            {0x6B571E45, "lgt"},
            {0x6E69693A, "obj"},
            {0x7050198A, "pmb"},
            {0x73850D05, "arcs"},
            {0x74AFE18C, "mdl"}, // ply mdl
            {0x7618CC9A, "mtn"},
            {0x7D9D148B, "eft"},
            {0x7DB518E8, "mdl"}, // obj mdl
            {0x7E33A16C, "spc"},
            {0x7F68C6AF, "mpac"},
        };
    }

    public struct ARCHeader
    {
        public int Magic;
        public short Version;
        public short Count;
    }

    public struct ARCEntry
    {
        public string Path;
        public int TypeHash;
        public uint DataSize;
        public uint FileSize;
        public byte Flags;
        public uint Offset;
    }

    public class ARCExport
    {
        public FileInfo File;
        public ARCEntry Entry;
    }
}