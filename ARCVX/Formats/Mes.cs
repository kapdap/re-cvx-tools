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

using ARCVX.Reader;
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
            { Region.IT, Language.IT },
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
            DecodeMap = Languages.ContainsKey(table) ? Languages[table] : Languages[Region.US];
            EncodeMap = DecodeMap.ToDictionary(x => x.Value, x => x.Key);
        }

        public void LoadLanguage(FileInfo file)
        {
            try
            {
                DecodeMap = new();

                string[] lines = System.IO.File.ReadAllLines(file.FullName);

                foreach (string line in lines)
                {
                    string[] parts = line.Split("=");

                    if (parts.Length >= 2)
                        DecodeMap.Add(ushort.Parse(parts[0], NumberStyles.HexNumber), parts[1]);
                }

                EncodeMap = DecodeMap.ToDictionary(x => x.Value, x => x.Key);
            }
            catch
            {
                LoadLanguage();
            }
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
            OpenReader();

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
            EndianWriter writer = new(stream, ByteOrder);

            writer.Write(entries.Count);

            foreach (MesEntry entry in entries)
                writer.Write(entry.Offset);

            writer.SetPosition(0);

            return stream;
        }

        public MemoryStream CreateNewStream(DirectoryInfo folder)
        {
            List<MesEntry> newEntries = new();

            using MemoryStream entryStream = new();
            using EndianWriter entryWriter = new(entryStream, ByteOrder);

            uint length = (uint)(4 + (Entries.Count * 4));
            uint offset = length;

            foreach (MesEntry entry in Entries)
            {
                FileInfo inputFile = GetEntryFile(entry, folder);

                if (!inputFile.Exists)
                {
                    entryWriter.Write(GetEntryBytes(entry));
                    continue;
                }

                string message = System.IO.File.ReadAllText(inputFile.FullName);

                for (int i = 0; i < message.Length; i++)
                {
                    int start = i;
                    char character = message[i];

                    // Skip next character if Windows newline
                    if (character == '\r' && message[i + 1] == '\n')
                        ++i;

                    if (character == '\r' || character == '\n')
                    {
                        entryWriter.Write((ushort)0xFF00);
                        continue;
                    }

                    StringBuilder builder = new();

                    if (character == '[')
                    {
                        do
                        {
                            if (i == message.Length)
                                throw new Exception($"Unclosed [ bracket starting at position {start + 1} in {inputFile.FullName}");

                            builder.Append(character);
                            character = message[++i];
                        }
                        while (character.ToString() != "]");
                    }

                    builder.Append(character);

                    string text = builder.ToString();

                    if (text.StartsWith("[0x") && text.Length == 8)
                    {
                        entryWriter.Write(ushort.Parse(text.Substring(3, 4), NumberStyles.HexNumber));
                    }
                    else if (text.StartsWith("[WAIT:") && text.Length > 7)
                    {
                        entryWriter.Write((ushort)0xFF02);
                        entryWriter.Write(ushort.Parse(text[6..^1]));
                    }
                    else if (text.StartsWith("[ITEM:") && text.Length > 7)
                    {
                        entryWriter.Write((ushort)0xFF03);
                        entryWriter.Write(ushort.Parse(text[6..^1]));
                    }
                    else
                    {
                        try { entryWriter.Write(EncodeCharacter(text)); }
                        catch { } // Ignore unknown characters
                    }
                }

                entryWriter.Write((ushort)0xFFFF);

                MesEntry newEntry = entry;

                newEntry.Offset = offset;
                newEntry.Length = offset - (uint)entryWriter.GetPosition();

                newEntries.Add(newEntry);

                offset = length + (uint)entryWriter.GetPosition();
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

            if (!outputFile.Directory.Exists)
                outputFile.Directory.Create();

            using (FileStream outputStream = outputFile.OpenWrite())
            using (MemoryStream newStream = CreateNewStream(folder))
                newStream.CopyTo(outputStream);

            if (file.FullName == File.FullName)
                CloseReader();

            if (!file.Directory.Exists)
                file.Directory.Create();

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
            DecodeMap.ContainsKey(value) ? DecodeMap[value] : $"[0x{value:X4}]";

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
                { 0xFFFF, "[END]"}
            };

            public static Dictionary<ushort, string> GB = new()
            {
                { 0x0001, "!" },
                { 0x0002, "\"" },
                { 0x0003, "[BIOHAZARD]" },
                { 0x0004, "[CROWN]" },
                { 0x0005, "%" },
                { 0x0006, "[HEART]" },
                { 0x0007, "'" },
                { 0x0008, "(" },
                { 0x0009, ")" },
                { 0x000A, "[CLUB]" },
                { 0x000B, "+" },
                { 0x000C, "," },
                { 0x000D, "-" },
                { 0x000E, "." },
                { 0x000F, "/" },
                { 0x0010, "0" },
                { 0x0011, "1" },
                { 0x0012, "2" },
                { 0x0013, "3" },
                { 0x0014, "4" },
                { 0x0015, "5" },
                { 0x0016, "6" },
                { 0x0017, "7" },
                { 0x0018, "8" },
                { 0x0019, "9" },
                { 0x001A, ":" },
                { 0x001B, ";" },
                { 0x001C, ">" },
                { 0x001D, "▼" },
                { 0x001E, "[AA]" },
                { 0x001F, "?" },
                { 0x0020, "[O]" },
                { 0x0021, "A" },
                { 0x0022, "B" },
                { 0x0023, "C" },
                { 0x0024, "D" },
                { 0x0025, "E" },
                { 0x0026, "F" },
                { 0x0027, "G" },
                { 0x0028, "H" },
                { 0x0029, "I" },
                { 0x002A, "J" },
                { 0x002B, "K" },
                { 0x002C, "L" },
                { 0x002D, "M" },
                { 0x002E, "N" },
                { 0x002F, "O" },
                { 0x0030, "P" },
                { 0x0031, "Q" },
                { 0x0032, "R" },
                { 0x0033, "S" },
                { 0x0034, "T" },
                { 0x0035, "U" },
                { 0x0036, "V" },
                { 0x0037, "W" },
                { 0x0038, "X" },
                { 0x0039, "Y" },
                { 0x003A, "Z" },
                { 0x003B, "┌" },
                { 0x003C, "×" },
                { 0x003D, "┘" },
                { 0x003E, "·" },
                { 0x003F, "_" },
                { 0x0040, "[EMPTY]" },
                { 0x0041, "a" },
                { 0x0042, "b" },
                { 0x0043, "c" },
                { 0x0044, "d" },
                { 0x0045, "e" },
                { 0x0046, "f" },
                { 0x0047, "g" },
                { 0x0048, "h" },
                { 0x0049, "i" },
                { 0x004A, "j" },
                { 0x004B, "k" },
                { 0x004C, "l" },
                { 0x004D, "m" },
                { 0x004E, "n" },
                { 0x004F, "o" },
                { 0x0050, "p" },
                { 0x0051, "q" },
                { 0x0052, "r" },
                { 0x0053, "s" },
                { 0x0054, "t" },
                { 0x0055, "u" },
                { 0x0056, "v" },
                { 0x0057, "w" },
                { 0x0058, "x" },
                { 0x0059, "y" },
                { 0x005A, "z" },
                { 0x005B, "[α]" },
                { 0x005C, "[ε]" },
                { 0x005D, "[UMBRELLA]" },
                { 0x005E, "~" },
                { 0x005F, "À" },
                { 0x0060, "Á" },
                { 0x0061, "Â" },
                { 0x0062, "Ã" },
                { 0x0063, "Ä" },
                { 0x0064, "Å" },
                { 0x0065, "È" },
                { 0x0066, "É" },
                { 0x0067, "Ê" },
                { 0x0068, "Ë" },
                { 0x0069, "Ì" },
                { 0x006A, "Í" },
                { 0x006B, "Î" },
                { 0x006C, "Ï" },
                { 0x006D, "Ò" },
                { 0x006E, "Ó" },
                { 0x006F, "Ô" },
                { 0x0070, "Ö" },
                { 0x0071, "Ù" },
                { 0x0072, "Ú" },
                { 0x0073, "Û" },
                { 0x0074, "Ü" },
                { 0x0075, "à" },
                { 0x0076, "á" },
                { 0x0077, "â" },
                { 0x0078, "ä" },
                { 0x0079, "ß" },
                { 0x007A, "è" },
                { 0x007B, "é" },
                { 0x007C, "ê" },
                { 0x007D, "ë" },
                { 0x007E, "ì" },
                { 0x007F, "í" },
                { 0x0080, "î" },
                { 0x0081, "ï" },
                { 0x0082, "ò" },
                { 0x0083, "ó" },
                { 0x0084, "ô" },
                { 0x0085, "ö" },
                { 0x0086, "ù" },
                { 0x0087, "ú" },
                { 0x0088, "û" },
                { 0x0089, "ü" },
                { 0x008A, "Ç" },
                { 0x008B, "ç" },
                { 0x008C, "Ñ" },
                { 0x008D, "ñ" },
                { 0x008E, "ª" },
                { 0x008F, "į" },
                { 0x0090, "¿" },
                { 0x0091, "æ" },
                { 0x0092, "Œ" },
                { 0x0093, "œ" },
                { 0x0094, "®" },
                { 0x0095, "™" },
                { 0x0096, "°" },
                { 0xFE00, "[0xFE00]" },
                { 0xFE01, "[WHITE]" },
                { 0xFE02, "[BLUE]" },
                { 0xFE03, "[RED]" },
                { 0xFE04, "[GREEN]" },
                { 0xFE05, "[GREY]" },
                { 0xFE08, "[CLEAR]" },
                { 0xFE09, "[SELECT]" },
                { 0xFF00, "[LINE]" },
                { 0xFF01, " " },
                { 0xFF02, "[WAIT]" },
                { 0xFF03, "[ITEM]" },
                { 0xFF04, "»" },
                { 0xFFFF, "[END]" },
            };

            public static Dictionary<ushort, string> ES = new()
            {
                { 0x0001, "!" },
                { 0x0002, "\"" },
                { 0x0003, "[BIOHAZARD]" },
                { 0x0004, "[KORONA]" },
                { 0x0005, "%" },
                { 0x0006, "[SERCE]" },
                { 0x0007, "'" },
                { 0x0008, "(" },
                { 0x0009, ")" },
                { 0x000A, "[TREFL]" },
                { 0x000B, "+" },
                { 0x000C, "," },
                { 0x000D, "-" },
                { 0x000E, "." },
                { 0x000F, "/" },
                { 0x0010, "0" },
                { 0x0011, "1" },
                { 0x0012, "2" },
                { 0x0013, "3" },
                { 0x0014, "4" },
                { 0x0015, "5" },
                { 0x0016, "6" },
                { 0x0017, "7" },
                { 0x0018, "8" },
                { 0x0019, "9" },
                { 0x001A, ":" },
                { 0x001B, ";" },
                { 0x001C, ">" },
                { 0x001D, "▼" },
                { 0x001E, "[AA]" },
                { 0x001F, "?" },
                { 0x0020, "[O]" },
                { 0x0021, "A" },
                { 0x0022, "B" },
                { 0x0023, "C" },
                { 0x0024, "D" },
                { 0x0025, "E" },
                { 0x0026, "F" },
                { 0x0027, "G" },
                { 0x0028, "H" },
                { 0x0029, "I" },
                { 0x002A, "J" },
                { 0x002B, "K" },
                { 0x002C, "L" },
                { 0x002D, "M" },
                { 0x002E, "N" },
                { 0x002F, "O" },
                { 0x0030, "P" },
                { 0x0031, "Q" },
                { 0x0032, "R" },
                { 0x0033, "S" },
                { 0x0034, "T" },
                { 0x0035, "U" },
                { 0x0036, "V" },
                { 0x0037, "W" },
                { 0x0038, "X" },
                { 0x0039, "Y" },
                { 0x003A, "Z" },
                { 0x003B, "┌" },
                { 0x003C, "×" },
                { 0x003D, "┘" },
                { 0x003E, "·" },
                { 0x003F, "_" },
                { 0x0040, "[PUSTO]" },
                { 0x0041, "a" },
                { 0x0042, "b" },
                { 0x0043, "c" },
                { 0x0044, "d" },
                { 0x0045, "e" },
                { 0x0046, "f" },
                { 0x0047, "g" },
                { 0x0048, "h" },
                { 0x0049, "i" },
                { 0x004A, "j" },
                { 0x004B, "k" },
                { 0x004C, "l" },
                { 0x004D, "m" },
                { 0x004E, "n" },
                { 0x004F, "o" },
                { 0x0050, "p" },
                { 0x0051, "q" },
                { 0x0052, "r" },
                { 0x0053, "s" },
                { 0x0054, "t" },
                { 0x0055, "u" },
                { 0x0056, "v" },
                { 0x0057, "w" },
                { 0x0058, "x" },
                { 0x0059, "y" },
                { 0x005A, "z" },
                { 0x005B, "[α]" },
                { 0x005C, "[ε]" },
                { 0x005D, "[UMBRELLA]" },
                { 0x005E, "~" },
                { 0x005F, "À" },
                { 0x0060, "Á" },
                { 0x0061, "Â" },
                { 0x0062, "Ã" },
                { 0x0063, "Ä" },
                { 0x0064, "Å" },
                { 0x0065, "È" },
                { 0x0066, "É" },
                { 0x0067, "Ê" },
                { 0x0068, "Ë" },
                { 0x0069, "Ì" },
                { 0x006A, "Í" },
                { 0x006B, "Î" },
                { 0x006C, "Ï" },
                { 0x006D, "Ò" },
                { 0x006E, "Ó" },
                { 0x006F, "Ô" },
                { 0x0070, "Ö" },
                { 0x0071, "Ù" },
                { 0x0072, "Ú" },
                { 0x0073, "Û" },
                { 0x0074, "Ü" },
                { 0x0075, "à" },
                { 0x0076, "á" },
                { 0x0077, "â" },
                { 0x0078, "ä" },
                { 0x0079, "ß" },
                { 0x007A, "è" },
                { 0x007B, "é" },
                { 0x007C, "ê" },
                { 0x007D, "ë" },
                { 0x007E, "ì" },
                { 0x007F, "í" },
                { 0x0080, "î" },
                { 0x0081, "ï" },
                { 0x0082, "ò" },
                { 0x0083, "ó" },
                { 0x0084, "ô" },
                { 0x0085, "ö" },
                { 0x0086, "ù" },
                { 0x0087, "ú" },
                { 0x0088, "û" },
                { 0x0089, "ü" },
                { 0x008A, "Ç" },
                { 0x008B, "ç" },
                { 0x008C, "Ñ" },
                { 0x008D, "ñ" },
                { 0x008E, "ª" },
                { 0x008F, "į" },
                { 0x0090, "¿" },
                { 0x0091, "æ" },
                { 0x0092, "Œ" },
                { 0x0093, "œ" },
                { 0x0094, "®" },
                { 0x0095, "™" },
                { 0x0096, "°" },
                { 0xFE00, "[0xFE00]" },
                { 0xFE01, "[WHITE]" },
                { 0xFE02, "[BLUE]" },
                { 0xFE03, "[RED]" },
                { 0xFE04, "[GREEN]" },
                { 0xFE05, "[GREY]" },
                { 0xFE08, "[CLEAR]" },
                { 0xFE09, "[SELECT]" },
                { 0xFF00, "[LINE]" },
                { 0xFF01, " " },
                { 0xFF02, "[WAIT]" },
                { 0xFF03, "[ITEM]" },
                { 0xFF04, "»" },
                { 0xFFFF, "[END]" }
            };

            // TODO: German language table
            public static Dictionary<ushort, string> DE = new()
            {
            };

            // TODO: French language table
            public static Dictionary<ushort, string> FR = new()
            {
            };

            // TODO: Italian language table
            public static Dictionary<ushort, string> IT = new()
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
        IT,
    }
}