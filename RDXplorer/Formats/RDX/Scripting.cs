using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;

namespace RDXplorer.Formats.RDX
{
    public class Scripting
    {
        public Document Settings { get; private set; }
        public FileInfo PathInfo { get; private set; } = new("Data\\scripting.yml");

        public Scripting()
        {
            Settings = ReadSettings();
        }

        public Scripting(FileInfo path)
        {
            PathInfo = path;
            Settings = ReadSettings();
        }

        public string Decode(byte[] data)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                OpCode opcode;
                string key = string.Empty;

                if (i < data.Length - 2)
                    key = data[i].ToString("X2") + data[i + 1].ToString("X2");

                if (!Settings.OpCodes.ContainsKey(key))
                    key = data[i].ToString("X2");

                try { opcode = Settings.OpCodes[key]; }
                catch { throw new Exception($"Unknown OpCode: {key}"); }

                i += (key.Length / 2) - 1;

                builder.Append(key);
                builder.Append(" ");

                foreach (Argument argument in opcode.Arguments)
                {
                    int length = argument.Size / 8;

                    for (int j = 0; j < length; j++)
                        if (i < data.Length - 1)
                            builder.Append(data[++i].ToString("X2"));

                    builder.Append(" ");
                }

                builder.Append(Environment.NewLine);
            }

            return builder.ToString();
        }

        public string Decompile(byte[] data)
        {
            StringBuilder builder = new StringBuilder();

            Stack<int> tabStack = new();
            int tabSize = 0;

            for (int i = 0; i < data.Length; i++)
            {
                StringBuilder args = new StringBuilder();
                OpCode opcode;
                byte[] bytes = null;
                string key = string.Empty;

                if (i < data.Length - 2)
                    key = data[i].ToString("X2") + data[i + 1].ToString("X2");

                if (!Settings.OpCodes.ContainsKey(key))
                    key = data[i].ToString("X2");

                try { opcode = Settings.OpCodes[key]; }
                catch { throw new Exception($"Unknown OpCode: {key}"); }

                i += (key.Length / 2) - 1;

                if (tabStack.Count > 0 && i >= tabStack.Peek())
                {
                    tabStack.Pop();
                    tabSize--;
                }

                for (int j = 0; j < tabSize; j++)
                    builder.Append("\t");

                foreach (Argument argument in opcode.Arguments)
                {
                    bytes = new byte[argument.Size / 8];

                    for (int j = 0; j < bytes.Length; j++)
                        if (i < data.Length - 1)
                            bytes[j] = data[++i];

                    // TODO: Check if we need to determine endianness (PS3, GCN)
                    Array.Reverse(bytes);

                    switch (argument.Type)
                    {
                        case "float":
                            args.Append(BitConverter.ToHalf(bytes).ToString());
                            break;

                        case "short":
                            args.Append(BitConverter.ToInt16(bytes).ToString());
                            break;

                        case "ushort":
                            args.Append(BitConverter.ToUInt16(bytes).ToString());
                            break;

                        case "sbyte":
                            args.Append(((sbyte)bytes[0]).ToString());
                            break;

                        case "byte":
                            args.Append(bytes[0].ToString());
                            break;

                        default:
                            args.Append(BitConverter.ToString(bytes));
                            break;
                    }

                    args.Append(", ");
                }

                if (args.Length > 2)
                    args.Remove(args.Length - 2, 2);

                if ((opcode.Name == "If" || 
                    opcode.Name == "Else" || 
                    opcode.Name == "While" || 
                    opcode.Name == "For") &&
                    bytes != null && bytes.Length > 0)
                {
                    tabStack.Push(i + bytes[0] - 1);
                    tabSize++;
                }

                builder.Append(opcode.Name);
                builder.Append("(");
                builder.Append(args);
                builder.Append(")");
                builder.Append(Environment.NewLine);
            }

            return builder.ToString();
        }

        public Document ReadSettings()
        {
            Deserializer deserializer = (Deserializer)new DeserializerBuilder().Build();
            return deserializer.Deserialize<Document>(File.ReadAllText(PathInfo.FullName));
        }

        public class Document
        {
            public Dictionary<string, OpCode> OpCodes { get; set; } = new();
        }

        public class OpCode
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Type { get; set; }
            public int Size { get; set; }
            public int Indent { get; set; }

            public List<Argument> Arguments { get; set; } = new();
        }

        public class Argument
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Type { get; set; }
            public int Size { get; set; }
        }
    }
}
