// SPDX-FileCopyrightText: 2024 Kapdap <kapdap@pm.me>
//
// SPDX-License-Identifier: MIT
/*  ARCVX
 *  
 *  Copyright 2024 Kapdap <kapdap@pm.me>
 *
 *  Use of this source code is governed by an MIT-style
 *  license that can be found in the LICENSE file or at
 *  https://opensource.org/licenses/MIT.
 */

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

        public Region Language { get; set; }
        public FileInfo LanguageFile { get; set; }

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

        public ReadOnlySpan<byte> GetEntryBytes(ARCEntry entry)
        {
            OpenReader();

            Span<byte> buffer = new byte[entry.DataSize];

            Stream.Position = entry.Offset;
            Stream.Read(buffer);

            return buffer;
        }

        public MemoryStream GetEntryStream(ARCEntry entry)
        {
            MemoryStream stream = new();

            stream.Write(GetEntryBytes(entry));
            stream.Position = 0;

            return stream;
        }

        public ARCExport ExportEntryData(ARCEntry entry, DirectoryInfo folder)
        {
            FileInfo outputFile = new(Path.Join(folder.FullName, Path.ChangeExtension(entry.Path, GetEntryExtension(entry))));

            if (!outputFile.Directory.Exists)
                outputFile.Directory.Create();

            if (outputFile.Exists)
                outputFile.Delete();

            using MemoryStream entryStream = GetEntryStream(entry);
            using FileStream outputStream = outputFile.OpenWrite();

            ZlibHeader zlibHeader = new(entryStream.ReadByte(), entryStream.ReadByte());

            entryStream.Position = 0;

            if (zlibHeader.IsValid())
                using (ZLibStream zlibStream = new(entryStream, CompressionMode.Decompress))
                    zlibStream.CopyTo(outputStream);
            else
                entryStream.CopyTo(outputStream);

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
            EndianWriter writer = new(stream, ByteOrder);

            writer.Write(Header.Magic, ByteOrder.LittleEndian);
            writer.Write(Header.Version);
            writer.Write(Header.Count);

            writer.SetPosition(0);

            return stream;
        }

        public MemoryStream CreateEntriesStream(List<ARCEntry> entries)
        {
            MemoryStream stream = new();
            EndianWriter writer = new(stream, ByteOrder);

            foreach (ARCEntry entry in entries)
            {
                uint flags = (uint)entry.Flags << 24;
                flags |= entry.FileSize & 0x00FFFFFF;

                writer.Write(Bytes.GetStringBytes(entry.Path, 0x40));
                writer.Write(entry.TypeHash);
                writer.Write(entry.DataSize);
                writer.Write(flags);
                writer.Write(entry.Offset);
            }

            writer.SetPosition(0);

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
                    if (inputFile.Extension == ".mes")
                    {
                        using Mes mes = new(inputFile) { ByteOrder = ByteOrder };

                        if (LanguageFile != null)
                            mes.LoadLanguage(LanguageFile);
                        else
                            mes.LoadLanguage(Language);

                        _ = mes.Save();
                    }

                    inputFile.Refresh();

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
                newEntries.Add(newEntry);

                dataStream.Position = 0;
                dataStream.CopyTo(newStream);

                offset += dataStream.Length;
            }

            MemoryStream outputStream = new();

            using (MemoryStream headerStream = CreateHeaderStream())
                headerStream.CopyTo(outputStream);

            using (MemoryStream entriesStream = CreateEntriesStream(newEntries))
                entriesStream.CopyTo(outputStream);

            newStream.Position = 0;
            newStream.CopyTo(outputStream);

            outputStream.Position = 0;

            return outputStream;
        }

        public ARC Save(DirectoryInfo folder) =>
            Save(folder, File);

        public ARC Save(DirectoryInfo folder, FileInfo file)
        {
            FileInfo outputFile = new(Path.Join(File.DirectoryName, "_" + Path.GetRandomFileName()));

            try
            {
                if (!outputFile.Directory.Exists)
                    outputFile.Directory.Create();

                using (FileStream outputStream = outputFile.OpenWrite())
                using (MemoryStream newStream = CreateNewStream(folder))
                    newStream.CopyTo(outputStream);

                if (file.FullName == File.FullName)
                    CloseReader();

                outputFile.Refresh();
                outputFile.MoveTo(file.FullName, true);

                return new(outputFile);
            }
            catch
            {
                try { outputFile.Delete(); } catch { }
                throw;
            }
        }

        public static string GetEntryPath(ARCEntry entry) =>
            Path.ChangeExtension(entry.Path, GetEntryExtension(entry));

        public static string GetEntryExtension(ARCEntry entry) =>
            GetTypeExtension(entry.TypeHash);

        public static string GetTypeExtension(int hash) =>
            TypeMap.ContainsKey(hash) ? TypeMap[hash] : hash.ToString("X8");

        private static Dictionary<int, string> TypeMap { get; } = new()
        {
            {0x02358E1A, "spkg"},
            {0x051BE0EC, "rut"},
            {0x070078B5, "rmt"},
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