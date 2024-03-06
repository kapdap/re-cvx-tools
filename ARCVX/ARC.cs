using ARCVX.Reader;
using ComponentAce.Compression.Libs.zlib;
using System;
using System.Collections.Generic;
using System.IO;

namespace ARCVX
{
    public class ARC : IDisposable
    {
        public const int MAGIC_HFS = 0x48465300; // "HFS."
        public const int MAGIC_ARC = 0x41524300; // "ARC."
        public const int MAGIC_HFS_LE = 0x00534648; // ".SFH"
        public const int MAGIC_ARC_LE = 0x00435241; // ".CRA"
        public const int CHUNK_SIZE = 0x20000;

        public FileInfo File { get; }
        public FileStream Stream { get; private set; }
        public EndianReader Reader { get; private set; }

        public int Magic { get; private set; }

        public FileHeader Header { get; private set; }
        public List<ARCEntry> Entries { get; private set; }

        private bool? _isValid;
        public bool IsValid
        {
            get
            {
                if (_isValid == null)
                {
                    _isValid = File.Exists && File.Length > 8;

                    if ((bool)_isValid)
                    {
                        ReadMagic();
                        _isValid = Magic == MAGIC_HFS || Magic == MAGIC_ARC;
                    }
                }

                return (bool)_isValid;
            }
        }

        private bool? _isHFS;
        public bool IsHFS
        {
            get
            {
                if (_isHFS == null)
                    _isHFS = IsValid && Magic == MAGIC_HFS;
                return (bool)_isHFS;
            }
        }

        public ARC(FileInfo file)
        {
            File = file;
            ReadHeader();
            ReadEntries();
        }

        public void ReadMagic() =>
            Magic = GetMagic();

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

        public void ReadHeader()
        {
            if (IsValid)
                Header = new FileHeader
                {
                    HFS = GetHFSHeader(),
                    ARC = GetARCHeader()
                };
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

        public void ReadEntries()
        {
            if (IsValid)
                Entries = GetEntries();
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

        public byte[] GetEntryData(ARCEntry entry)
        {
            OpenReader();

            Reader.SetPosition(entry.Offset + (entry.Offset / CHUNK_SIZE * 16) + 16);

            byte[] data = new byte[entry.DataSize];
            Stream.Read(data, 0, (int)entry.DataSize);

            return data;
        }

        public void ExportEntryData(ARCEntry entry, DirectoryInfo folder)
        {
            FileInfo entryFile = new(Path.Combine(folder.FullName, entry.Path));

            if (!entryFile.Directory.Exists)
                entryFile.Directory.Create();

            FileInfo outputFile;
            int i = 0;

            do
            {
                outputFile = new(entryFile.FullName + (i != 0 ? $"_{i}" : "") + "." + GetExtension(entry.TypeHash));
                i++;
            }
            while (outputFile.Exists);

            using FileStream outputStream = outputFile.OpenWrite();

            byte[] data = GetEntryData(entry);

            try
            {
                using MemoryStream dataStream = new();

                dataStream.Write(data, 0, data.Length);
                dataStream.Seek(0, SeekOrigin.Begin);

                using ZInputStream zlibStream = new(dataStream);

                int value;
                while ((value = zlibStream.Read()) != -1)
                    outputStream.WriteByte((byte)value);
            }
            catch
            {
                outputStream.Write(data);
            }
        }

        public IEnumerable<ARCEntry> ExportAllEntries() =>
            ExportAllEntries(File.Directory);

        public IEnumerable<ARCEntry> ExportAllEntries(DirectoryInfo folder)
        {
            List<ARCEntry> entries = GetEntries();
            foreach (ARCEntry entry in entries)
            {
                ExportEntryData(entry, folder);
                yield return entry;
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
            {0x0949A1DA, "mdl"},
            {0x09C48A11, "bin"},
            {0x0A736313, "mdl"},
            {0x108F442E, "skin"},
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
            {0x40171000, "bin"},
            {0x42940D09, "fmt"},
            {0x4356673E, "man"},
            {0x46C78353, "mes"},
            {0x4C0DB839, "sdl"},
            {0x5DF3D947, "bin"},
            {0x5FF4BE71, "ene"},
            {0x6505B384, "ddsp"},
            {0x681835FC, "itm"},
            {0x6A76E771, "mes"},
            {0x6B0369B1, "atr"},
            {0x6B571E45, "lgt"},
            {0x6E69693A, "obj"},
            {0x7050198A, "pmb"},
            {0x73850D05, "arc"},
            {0x74AFE18C, "skin"},
            {0x7618CC9A, "mtn"},
            {0x7D9D148B, "eft"},
            {0x7DB518E8, "mdl"},
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
}