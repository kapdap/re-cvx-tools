using ARCVX.Reader;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections.Generic;
using System.IO;

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
            if (!IsValid)
                return new ARCHeader { };

            OpenReader();

            Reader.SetPosition(0);

            return new ARCHeader
            {
                Magic = Reader.ReadInt32(ByteOrder.LittleEndian),
                Version = Reader.ReadInt16(),
                Count = Reader.ReadInt16(),
            };
        }

        public List<ARCEntry> GetEntries()
        {
            if (!IsValid)
                return null;

            OpenReader();

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

            OpenReader();

            Reader.SetPosition(entry.Offset);

            MemoryStream stream = new();

            Stream.CopyTo(stream);
            stream.SetLength((int)entry.DataSize);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        public string GetEntryExtension(ARCEntry entry) =>
            GetTypeExtension(entry.TypeHash);

        public string GetTypeExtension(int hash) =>
            TypeMap.ContainsKey(hash) ? TypeMap[hash] : hash.ToString("X8");

        public FileInfo ExportEntryData(ARCEntry entry, DirectoryInfo folder)
        {
            FileInfo outputFile = new(Path.Join(folder.FullName, Path.ChangeExtension(entry.Path, GetEntryExtension(entry))));

            if (!outputFile.Directory.Exists)
                outputFile.Directory.Create();

            if (outputFile.Exists)
                outputFile.Delete();

            using MemoryStream entryStream = GetEntryStream(entry);
            using FileStream outputStream = outputFile.OpenWrite();

            int magic = entryStream.ReadByte();

            entryStream.Seek(0, SeekOrigin.Begin);

            try
            {
                if (magic != 0x68 && magic != 0x78)
                    throw new Exception();

                using InflaterInputStream zlibStream = new(entryStream);
                zlibStream.CopyTo(outputStream);
            }
            catch
            {
                entryStream.CopyTo(outputStream);
            }

            return outputFile;
        }

        public IEnumerable<ARCExport> ExportAllEntries() =>
            ExportAllEntries(File.Directory);

        public IEnumerable<ARCExport> ExportAllEntries(DirectoryInfo folder)
        {
            foreach (ARCEntry entry in Entries)
            {
                FileInfo path = ExportEntryData(entry, folder);

                yield return new()
                {
                    Path = path.FullName,
                    Entry = entry,
                };
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

    public struct ARCExport
    {
        public string Path;
        public ARCEntry Entry;
    }
}