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
using System.IO;
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

        public Mes(FileInfo file) : base(file) { }
        public Mes(FileInfo file, Stream stream) : base(file, stream) { }

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

        public List<MesEntry> GetEntries()
        {
            OpenReader();

            Reader.SetPosition(4);

            List<MesEntry> list = new();

            for (int i = 0; i < Header.Count; i++)
            {
                MesEntry entry = new();
                entry.Offset = Reader.ReadUInt32();
                list.Add(entry);
            }

            List<MesEntry> entries = new();

            for (int i = 0; i < list.Count; i++)
            {
                MesEntry entry = list[i];

                Reader.SetPosition(entry.Offset);

                StringBuilder message = new();

                uint next = (uint)(i < list.Count - 1 ? list[i + 1].Offset : Reader.GetLength());
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

        public List<FileInfo> Export()
        {
            List<FileInfo> list = new();

            foreach (MesEntry entry in Entries)
            {
                string fileName = Path.GetFileNameWithoutExtension(File.FullName);
                FileInfo outputFile = new(Path.Join(File.Directory.FullName, fileName, fileName + $"_{entry.Offset}.txt"));

                if (!outputFile.Directory.Exists)
                    outputFile.Directory.Create();

                System.IO.File.WriteAllText(outputFile.FullName, entry.Message);
                list.Add(outputFile);
            }

            return list;
        }

        public static ushort EncodeCharacter(string character)
        {
            //string key = value.ToString("X4");
            //return DecodeMap.ContainsKey(key) ? DecodeMap[key] : $"[0x{key}]";
            return 0;
        }

        public static string DecodeCharacter(ushort value)
        {
            string key = Bytes.SwapBytes(value).ToString("X4");
            return DecodeMap.ContainsKey(key) ? DecodeMap[key] : $"[0x{key}]";
        }

        private static Dictionary<string, string> _decodeMap = new();
        private static Dictionary<string, string> DecodeMap
        {
            get
            {
                if (_decodeMap.Count == 0)
                {
                    // TODO: Create Tables for JPN and EUR releases
                    FileInfo file = new(Path.Combine(AppContext.BaseDirectory, "Data\\ASCII\\BIOCV_USA.tbl"));

                    if (file.Exists)
                    {
                        string[] lines = System.IO.File.ReadAllLines(file.FullName);

                        foreach (string line in lines)
                        {
                            string[] parts = line.Split("=");

                            if (parts.Length >= 2 && !_decodeMap.ContainsKey(parts[0]))
                                _decodeMap.Add(parts[0], parts[1]);
                        }
                    }
                }

                return _decodeMap;
            }
        }

        private static Dictionary<string, string> _encodeMap = new();
        private static Dictionary<string, string> EncodeMap
        {
            get
            {
                if (_encodeMap.Count == 0)
                {
                    // TODO: Create Tables for JPN and EUR releases
                    FileInfo file = new(Path.Combine(AppContext.BaseDirectory, "Data\\ASCII\\BIOCV_USA.tbl"));

                    if (file.Exists)
                    {
                        string[] lines = System.IO.File.ReadAllLines(file.FullName);

                        foreach (string line in lines)
                        {
                            string[] parts = line.Split("=");

                            if (parts.Length >= 2 && !_encodeMap.ContainsKey(parts[1]))
                                _encodeMap.Add(parts[1], parts[0]);
                        }
                    }
                }

                return _encodeMap;
            }
        }
    }

    public struct MesHeader
    {
        public int Count;
    }

    public struct MesEntry
    {
        public uint Offset;
        public string Message;
    }
}