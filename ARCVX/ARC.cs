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
            if (!File.Exists && File.Length < 24)
                return false;

            OpenReader();

            long position = Reader.GetPosition();
            Reader.SetPosition(0);

            int magic = Reader.ReadInt32(false);
            Reader.SetPosition(position);

            return magic == MAGIC_HFS;
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
            OpenReader();

            Reader.SetPosition(0);

            return new HFSHeader
            {
                Magic = Reader.ReadInt32(false),
                Unknown0 = Reader.ReadInt16(),
                Unknown1 = Reader.ReadInt16(),
                Size = Reader.ReadInt32(),
                Padding = Reader.ReadInt32(),
            };
        }

        public ARCHeader GetARCHeader()
        {
            OpenReader();

            Reader.SetPosition(16);

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
                entry.FileSize = Reader.ReadUInt32();
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
            TypeMap.ContainsKey(hash) ? TypeMap[hash] : "bin";

        // TODO: Complete Type map list
        public Dictionary<int, string> TypeMap { get; } = new()
        {
            {0, "bin"}
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
        public short Unknown0;
        public short Unknown1;
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
        public uint FileSize;
        public uint DataSize;
        public uint Offset;
    }
}