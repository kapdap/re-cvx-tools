using ARCVX.Reader;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ARCVX.Formats
{
    public class ARC : IDisposable
    {
        public const int MAGIC_ARC = 0x41524300; // "ARC."
        public const int MAGIC_ARC_LE = 0x00435241; // ".CRA"

        public const int MAGIC_HFS = 0x48465300; // "HFS."
        public const int MAGIC_HFS_LE = 0x00534648; // ".SFH"

        public const int HFS_CHUNK_SIZE = 0x20000;
        public const int HFS_VALID_SIZE = 0x10;

        public FileInfo File { get; }
        public FileStream Stream { get; private set; }
        public EndianReader Reader { get; private set; }

        private bool? _isValid = null;
        public bool IsValid
        {
            get
            {
                if (_isValid == null)
                {
                    _isValid = File.Exists && File.Length > 8;

                    if ((bool)_isValid)
                        _isValid = Magic == MAGIC_HFS || Magic == MAGIC_ARC;
                }

                return (bool)_isValid;
            }
        }

        private bool? _isHFS = null;
        public bool IsHFS
        {
            get
            {
                if (_isHFS == null)
                    _isHFS = IsValid && Magic == MAGIC_HFS;
                return (bool)_isHFS;
            }
        }

        private int? _magic;
        public int Magic
        {
            get
            {
                if (_magic == null)
                    _magic = GetMagic();
                return (int)_magic;
            }
        }

        private FileHeader? _header;
        public FileHeader Header
        {
            get
            {
                if (_header == null && IsValid)
                    _header = new FileHeader
                    {
                        HFS = GetHFSHeader(),
                        ARC = GetARCHeader()
                    };

                return _header ?? new FileHeader();
            }
        }

        private List<ARCEntry> _entries;
        public List<ARCEntry> Entries
        {
            get
            {
                if (_entries == null && IsValid)
                    _entries = GetEntries();
                return _entries;
            }
        }

        public ARC(FileInfo file) =>
            File = file;

        public int GetMagic()
        {
            OpenReader();

            long position = Reader.GetPosition();
            Reader.SetPosition(0);

            int magic = Reader.ReadInt32(false);
            Reader.SetPosition(position);

            if (magic == MAGIC_HFS || magic == MAGIC_ARC)
                Reader.IsBigEndian = true;
            else if (magic == MAGIC_HFS_LE || magic == MAGIC_ARC_LE)
                Reader.IsBigEndian = false;

            return magic;
        }

        public HFSHeader GetHFSHeader()
        {
            if (!IsHFS)
                return new HFSHeader { };

            Reader.SetPosition(0);

            return new HFSHeader
            {
                Magic = Reader.ReadInt32(false),
                Version = Reader.ReadInt16(),
                Type = Reader.ReadInt16(),
                Size = Reader.ReadInt32(),
                Padding = Reader.ReadInt32(),
            };
        }

        public ARCHeader GetARCHeader()
        {
            OpenReader();

            Reader.SetPosition(IsHFS ? 16 : 0);

            return new ARCHeader
            {
                Magic = Reader.ReadInt32(false),
                Version = Reader.ReadInt16(),
                Count = Reader.ReadInt16(),
            };
        }

        public List<ARCEntry> GetEntries()
        {
            OpenReader();

            Reader.SetPosition(IsHFS ? 24 : 8);

            List<ARCEntry> entries = new();

            for (int i = 0; i < Header.ARC.Count; i++)
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

        public long GetEntryOffset(ARCEntry entry) =>
            entry.Offset + (entry.Offset / HFS_CHUNK_SIZE * HFS_VALID_SIZE) + HFS_VALID_SIZE;

        public MemoryStream GetEntryStream(ARCEntry entry)
        {
            OpenReader();

            MemoryStream stream = new();

            Reader.SetPosition(GetEntryOffset(entry));

            for (int i = 0; i < entry.DataSize; i++)
            {
                if (Stream.Position % HFS_CHUNK_SIZE == 0)
                    Reader.AddPosition(HFS_VALID_SIZE);

                stream.WriteByte(Reader.ReadByte());
            }

            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength((int)entry.DataSize);

            return stream;
        }

        public FileInfo ExportEntryData(ARCEntry entry, DirectoryInfo folder)
        {
            FileInfo outputFile = new(Path.Join(folder.FullName, entry.Path + "." + GetExtension(entry.TypeHash)));

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
                if (magic != 0x68)
                    throw new Exception();

                using InflaterInputStream zlibStream = new(entryStream, new Inflater());
                zlibStream.IsStreamOwner = false;
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
            List<ARCEntry> entries = GetEntries();
            foreach (ARCEntry entry in entries)
            {
                FileInfo path = ExportEntryData(entry, folder);

                yield return new()
                {
                    Path = path.FullName,
                    Entry = entry,
                };
            }
        }

        public void OpenReader()
        {
            if (Stream == null)
            {
                Stream = File.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                Reader = new(Stream);
            }
        }

        public void CloseReader()
        {
            if (Stream != null)
            {
                ((IDisposable)Stream).Dispose();
                Stream = null;
            }

            if (Reader != null)
            {
                ((IDisposable)Reader).Dispose();
                Reader = null;
            }
        }

        public void Dispose() =>
            CloseReader();

        public string GetExtension(int hash) =>
            TypeMap.ContainsKey(hash) ? TypeMap[hash] : hash.ToString("X8");

        public Dictionary<int, string> TypeMap { get; } = new()
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

    public struct FileHeader
    {
        public HFSHeader HFS;
        public ARCHeader ARC;
    }

    public struct HFSHeader
    {
        public int Magic;
        public short Version;
        public short Type;
        public int Size;
        public int Padding;
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