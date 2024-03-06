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

        public FileHeader Header { get; private set; }

        public ARC(FileInfo file)
        {
            File = file;
            ReadHeader();
        }

        public bool IsValid()
        {
            if (!File.Exists && File.Length < 8)
                return false;

            int magic = GetMagic();

            return magic == MAGIC_HFS ||
                magic == MAGIC_ARC ||
                magic == MAGIC_HFS_LE ||
                magic == MAGIC_ARC_LE;
        }

        public bool IsHFS() =>
            GetMagic() == MAGIC_HFS;

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
            if (IsValid())
                Header = new FileHeader
                {
                    HFS = GetHFSHeader(),
                    ARC = GetARCHeader()
                };
        }

        public HFSHeader GetHFSHeader()
        {
            if (!IsHFS())
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

            Reader.SetPosition(IsHFS() ? 16 : 0);

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
            FileInfo entryFile = new(Path.Combine(folder.FullName,
                Path.GetFileNameWithoutExtension(File.Name), entry.Path));

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

        // TODO: Complete Type map list
        public Dictionary<int, string> TypeMap { get; } = new()
        {
            {0x02358E1A, "spkg"},
            {0x051BE0EC, "051BE0EC"},
            {0x070078B5, "070078B5"},
            {0x0949A1DA, "0949A1DA"},
            {0x09C48A11, "09C48A11"},
            {0x0A736313, "0A736313"},
            {0x108F442E, "108F442E"},
            {0x130124FA, "130124FA"},
            {0x167DBBFF, "stq"},
            {0x18FF29AB, "18FF29AB"},
            {0x1BCC4966, "srq"},
            {0x1BE1DBEB, "1BE1DBEB"},
            {0x232E228C, "rev"},
            {0x241F5DEB, "tex"},
            {0x24339E8C, "24339E8C"},
            {0x2749C8A8, "mrl"},
            {0x28D65BFA, "28D65BFA"},
            {0x2ADFA358, "2ADFA358"},
            {0x348C831D, "348C831D"},
            {0x375F06DA, "375F06DA"},
            {0x40171000, "40171000"},
            {0x42940D09, "42940D09"},
            {0x4356673E, "4356673E"},
            {0x46C78353, "46C78353"},
            {0x4C0DB839, "sdl"},
            {0x5DF3D947, "5DF3D947"},
            {0x5FF4BE71, "5FF4BE71"},
            {0x6505B384, "6505B384"},
            {0x681835FC, "681835FC"},
            {0x6A76E771, "6A76E771"},
            {0x6B0369B1, "6B0369B1"},
            {0x6B571E45, "6B571E45"},
            {0x6E69693A, "6E69693A"},
            {0x7050198A, "7050198A"},
            {0x73850D05, "arc"},
            {0x74AFE18C, "74AFE18C"},
            {0x7618CC9A, "7618CC9A"},
            {0x7D9D148B, "7D9D148B"},
            {0x7DB518E8, "7DB518E8"},
            {0x7E33A16C, "spc"},
            {0x7F68C6AF, "7F68C6AF"},
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