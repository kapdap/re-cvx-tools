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

using ARCVX.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ARCVX.Formats
{
    public class Mes : Base<MesHeader>
    {
        public override int MAGIC { get; } = 0;
        public override int MAGIC_LE { get; } = 0;

        public override bool IsValid
        {
            get
            {
                if (_isValid == null)
                    File.Refresh();

                _isValid ??= File.Exists && File.Extension == ".mes";

                return (bool)_isValid;
            }
        }

        private List<MesEntry> _entries;
        public List<MesEntry> Entries
        {
            get
            {
                _entries ??= GetEntries();
                return _entries;
            }
        }

        public static Dictionary<Region, Dictionary<ushort, string>> Languages = new()
        {
            { Region.JP, Language.JP },
            { Region.US, Language.US },
            { Region.GB, Language.GB },
            { Region.ES, Language.ES },
            { Region.DE, Language.DE },
            { Region.FR, Language.FR },
        };

        public Mes(FileInfo file) : base(file) =>
            LoadLanguage();

        public Mes(FileInfo file, Region table) : base(file) =>
            LoadLanguage(table);

        public Mes(FileInfo file, Stream stream) : base(file, stream) =>
            LoadLanguage();

        public Mes(FileInfo file, Stream stream, Region table) : base(file, stream) =>
            LoadLanguage(table);

        public void LoadLanguage() =>
            LoadLanguage(Region.US);

        public void LoadLanguage(Region table)
        {
            if (Languages.ContainsKey(table))
                DecodeMap = Languages[table];
            else
                DecodeMap = Languages[Region.US];

            EncodeMap = DecodeMap.ToDictionary(x => x.Value, x => x.Key);
        }

        public override int GetMagic()
        {
            OpenReader();
            return 0;
        }

        public override MesHeader GetHeader()
        {
            OpenReader();

            Reader.SetPosition(0);

            return IsValid ? new MesHeader
            {
                Count = Reader.ReadInt32(),
            } : new();
        }

        public Span<byte> GetEntryBytes(MesEntry entry)
        {
            long position = Stream.Position;
            Stream.Position = entry.Offset;

            Span<byte> buffer = new byte[entry.Length];

            Stream.Read(buffer);
            Stream.Position = position;

            return buffer;
        }

        public List<MesEntry> GetEntries()
        {
            OpenReader();

            Reader.SetPosition(4);

            List<MesEntry> list = new();

            for (int i = 0; i < Header.Count; i++)
            {
                MesEntry entry = new();
                entry.Offset = Reader.ReadUInt32();
                entry.Order = (uint)i;
                list.Add(entry);
            }

            List<MesEntry> entries = new();

            for (int i = 0; i < list.Count; i++)
            {
                StringBuilder message = new();

                MesEntry entry = list[i];

                Reader.SetPosition(entry.Offset);

                uint next = (uint)(i < list.Count - 1 ? list[i + 1].Offset : Reader.GetLength());

                entry.Length = next - entry.Offset;

                while (Reader.GetPosition() < next)
                {
                    ushort text = Reader.ReadUInt16();

                    if (text == 0xFF00)
                        message.Append(Environment.NewLine);
                    else if (text == 0xFF02)
                        message.Append($"[WAIT:{Reader.ReadInt16()}]");
                    else if (text == 0xFF03)
                        message.Append($"[ITEM:{Reader.ReadInt16()}]");
                    else if (text == 0xFFFF)
                        break;
                    else
                        message.Append(DecodeCharacter(text));
                }

                entry.Message = message.ToString();

                entries.Add(entry);
            }

            return entries;
        }

        public List<FileInfo> Export() =>
            Export(GetDefaultDirectory());

        public List<FileInfo> Export(DirectoryInfo folder)
        {
            List<FileInfo> list = new();

            foreach (MesEntry entry in Entries)
            {
                FileInfo outputFile = GetEntryFile(entry, folder);

                if (!outputFile.Directory.Exists)
                    outputFile.Directory.Create();

                System.IO.File.WriteAllText(outputFile.FullName, entry.Message);
                list.Add(outputFile);
            }

            return list;
        }

        public MemoryStream CreateHeaderStream(List<MesEntry> entries)
        {
            MemoryStream stream = new();

            stream.Write(Bytes.GetValueBytes(entries.Count, ByteOrder));

            foreach (MesEntry entry in entries)
                stream.Write(Bytes.GetValueBytes(entry.Offset, ByteOrder));

            stream.Position = 0;

            return stream;
        }

        public MemoryStream CreateNewStream(DirectoryInfo folder)
        {
            HashSet<string> newline = new() { "\r", "\n" };

            using MemoryStream entryStream = new();

            List<MesEntry> newEntries = new();

            long length = 4 + (Entries.Count * 4);
            long offset = length;
            foreach (MesEntry entry in Entries)
            {
                MesEntry newEntry = entry;
                FileInfo inputFile = GetEntryFile(entry, folder);

                if (!inputFile.Exists)
                {
                    entryStream.Write(GetEntryBytes(entry));
                    continue;
                }

                string message = System.IO.File.ReadAllText(inputFile.FullName);

                for (int i = 0; i < message.Length; i++)
                {
                    int start = i;
                    char character = message[i];

                    if (newline.Contains(character.ToString()))
                    {
                        character = message[i + 1];
                        if (newline.Contains(character.ToString()))
                            ++i;
                        entryStream.Write(Bytes.GetValueBytes((ushort)0xFF00, ByteOrder));
                        continue;
                    }

                    StringBuilder builder = new();

                    if (character.ToString() == "[")
                    {
                        do
                        {
                            if (i == message.Length)
                                throw new Exception($"Unclosed [ bracket starting at position {start + 1} in {inputFile.FullName}");

                            builder.Append(character);
                            character = message[++i];
                        }
                        while (character.ToString() != "]");

                        builder.Append(character);
                    }
                    else
                        builder.Append(character);

                    string text = builder.ToString();

                    if (text.StartsWith("[0x") && text.Length == 6)
                        entryStream.Write(Bytes.GetValueBytes(byte.Parse(text.Substring(3, 2), NumberStyles.HexNumber)));
                    if (text.StartsWith("[WAIT:") && text.Length > 8)
                    {
                        entryStream.Write(Bytes.GetValueBytes(0xFF02, ByteOrder));
                        entryStream.Write(Bytes.GetValueBytes(ushort.Parse(text.Substring(6, text.Length - 7)), ByteOrder));
                    }
                    if (text.StartsWith("[ITEM:") && text.Length > 8)
                    {
                        entryStream.Write(Bytes.GetValueBytes(0xFF03, ByteOrder));
                        entryStream.Write(Bytes.GetValueBytes(ushort.Parse(text.Substring(6, text.Length - 7)), ByteOrder));
                    }
                    else
                    {
                        try { entryStream.Write(Bytes.GetValueBytes(EncodeCharacter(text), ByteOrder)); }
                        catch { }
                    }
                }

                entryStream.Write(Bytes.GetValueBytes((ushort)0xFFFF));

                newEntry.Offset = (uint)offset;
                newEntry.Length = (uint)(offset - entryStream.Position);

                newEntries.Add(newEntry);

                offset = length + entryStream.Position;
            }

            MemoryStream outputStream = new();

            using (MemoryStream headerStream = CreateHeaderStream(newEntries))
                headerStream.CopyTo(outputStream);

            entryStream.Position = 0;
            entryStream.CopyTo(outputStream);

            outputStream.Position = 0;
            return outputStream;
        }

        public Mes Save() =>
            Save(GetDefaultDirectory(), File);

        public Mes Save(FileInfo file) =>
            Save(GetDefaultDirectory(), file);

        public Mes Save(DirectoryInfo folder) =>
            Save(folder, File);

        public Mes Save(DirectoryInfo folder, FileInfo file)
        {
            FileInfo outputFile = new(Path.Join(File.DirectoryName, "_" + Path.GetRandomFileName()));

            using (FileStream outputStream = outputFile.OpenWrite())
            using (MemoryStream newStream = CreateNewStream(folder))
                newStream.CopyTo(outputStream);

            if (file.FullName == File.FullName)
                CloseReader();

            outputFile.Refresh();
            outputFile.MoveTo(file.FullName, true);

            return new(outputFile);
        }

        public FileInfo GetEntryFile(MesEntry entry, DirectoryInfo folder)
        {
            string fileName = Path.GetFileNameWithoutExtension(File.FullName);
            return new(Path.Join(folder.FullName, $"{fileName}_{entry.Order}.txt"));
        }

        public DirectoryInfo GetDefaultDirectory()
        {
            string fileName = Path.GetFileNameWithoutExtension(File.FullName);
            return new(Path.Join(File.Directory.FullName, fileName));
        }

        public static ushort EncodeCharacter(string value) =>
            EncodeMap.ContainsKey(value) ? EncodeMap[value] : throw new Exception($"Character not supported {value}");

        public static string DecodeCharacter(ushort value) =>
            DecodeMap.ContainsKey(value) ? DecodeMap[value] : $"[0x{value}]";

        public static Dictionary<ushort, string> DecodeMap { get; private set; }
        public static Dictionary<string, ushort> EncodeMap { get; private set; }

        public static class Language
        {
            // TODO: Japanese language table
            public static Dictionary<ushort, string> JP = new()
            {
            };

            public static Dictionary<ushort, string> US = new()
            {
                { 0xFE00, "[0xFE00]"},
                { 0xFE01, "[WHITE]"},
                { 0xFE02, "[BLUE]"},
                { 0xFE03, "[RED]"},
                { 0xFE04, "[GREEN]"},
                { 0xFE05, "[GREY]"},
                { 0xFE08, "[CLEAR]"},
                { 0xFE09, "[SELECT]"},
                { 0xFF00, "[LINE]"},
                { 0xFF01, " "},
                { 0xFF02, "[WAIT]"},
                { 0xFF03, "[ITEM]"},
                { 0xFF04, "»"},
                { 0x0001, "!"},
                { 0x0002, "\""},
                { 0x0003, "[BIOHAZARD]"},
                { 0x0004, "[CROWN]"},
                { 0x0005, "%"},
                { 0x0006, "[HEART]"},
                { 0x0007, "'"},
                { 0x0008, "("},
                { 0x0009, ")"},
                { 0x000A, "[CLUB]"},
                { 0x000B, "+"},
                { 0x000C, ","},
                { 0x000D, "-"},
                { 0x000E, "."},
                { 0x000F, "/"},
                { 0x0010, "0"},
                { 0x0011, "1"},
                { 0x0012, "2"},
                { 0x0013, "3"},
                { 0x0014, "4"},
                { 0x0015, "5"},
                { 0x0016, "6"},
                { 0x0017, "7"},
                { 0x0018, "8"},
                { 0x0019, "9"},
                { 0x001A, ":"},
                { 0x001B, ";"},
                { 0x001C, ">"},
                { 0x001D, "▼"},
                { 0x001E, "[AA]"},
                { 0x001F, "?"},
                { 0x0020, "[O]"},
                { 0x0021, "A"},
                { 0x0022, "B"},
                { 0x0023, "C"},
                { 0x0024, "D"},
                { 0x0025, "E"},
                { 0x0026, "F"},
                { 0x0027, "G"},
                { 0x0028, "H"},
                { 0x0029, "I"},
                { 0x002A, "J"},
                { 0x002B, "K"},
                { 0x002C, "L"},
                { 0x002D, "M"},
                { 0x002E, "N"},
                { 0x002F, "O"},
                { 0x0030, "P"},
                { 0x0031, "Q"},
                { 0x0032, "R"},
                { 0x0033, "S"},
                { 0x0034, "T"},
                { 0x0035, "U"},
                { 0x0036, "V"},
                { 0x0037, "W"},
                { 0x0038, "X"},
                { 0x0039, "Y"},
                { 0x003A, "Z"},
                { 0x003B, "┌"},
                { 0x003C, "×"},
                { 0x003D, "┘"},
                { 0x003E, "·"},
                { 0x003F, "_"},
                { 0x0041, "a"},
                { 0x0042, "b"},
                { 0x0043, "c"},
                { 0x0044, "d"},
                { 0x0045, "e"},
                { 0x0046, "f"},
                { 0x0047, "g"},
                { 0x0048, "h"},
                { 0x0049, "i"},
                { 0x004A, "j"},
                { 0x004B, "k"},
                { 0x004C, "l"},
                { 0x004D, "m"},
                { 0x004E, "n"},
                { 0x004F, "o"},
                { 0x0050, "p"},
                { 0x0051, "q"},
                { 0x0052, "r"},
                { 0x0053, "s"},
                { 0x0054, "t"},
                { 0x0055, "u"},
                { 0x0056, "v"},
                { 0x0057, "w"},
                { 0x0058, "x"},
                { 0x0059, "y"},
                { 0x005A, "z"},
                { 0x005B, "α"},
                { 0x005C, "ε"},
                { 0x005D, "[UMBRELLA]"},
                { 0x005E, "~"},
                { 0x005F, "®"},
                { 0xFFFF, "[END]"}
            };

            // TODO: English language table
            public static Dictionary<ushort, string> GB = new()
            {
            };

            // TODO: Spanish language table
            public static Dictionary<ushort, string> ES = new()
            {
            };

            // TODO: German language table
            public static Dictionary<ushort, string> DE = new()
            {
            };

            // TODO: French language table
            public static Dictionary<ushort, string> FR = new()
            {
            };
        }
    }

    public struct MesHeader
    {
        public int Count;
    }

    public struct MesEntry
    {
        public uint Order;
        public uint Offset;
        public uint Length;
        public string Message;
    }

    public enum Region
    {
        JP,
        US,
        GB,
        ES,
        DE,
        FR,
    }
}