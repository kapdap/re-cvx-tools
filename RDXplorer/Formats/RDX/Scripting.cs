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
                OpCode opcode = Settings.OpCodes[data[i].ToString("X2")];

                builder.Append(data[i].ToString("X2"));
                builder.Append(" ");

                foreach (Argument argument in opcode.Arguments)
                {
                    int length = argument.Size / 8;

                    for (int j = 0; j < length; j++)
                        builder.Append(data[++i].ToString("X2"));

                    builder.Append(" ");
                }

                builder.Remove(builder.Length - 1, 1);
                builder.Append(Environment.NewLine);
            }

            return builder.ToString();
        }

        public string Decompile(byte[] data)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                OpCode opcode = Settings.OpCodes[data[i].ToString("X2")];

                builder.Append($"{opcode.Name}(");

                foreach (Argument argument in opcode.Arguments)
                {
                    int length = argument.Size / 8;

                    byte[] bytes = new byte[length];

                    for (int j = 0; j < length; j++)
                        bytes[j] = data[++i];

                    // TODO: Check if we need to determine endianness (PS3, GCN)
                    Array.Reverse(bytes);

                    switch (argument.Type)
                    {
                        case "float":
                            builder.Append(BitConverter.ToHalf(bytes).ToString());
                            break;

                        case "short":
                            builder.Append(BitConverter.ToInt16(bytes).ToString());
                            break;

                        case "ushort":
                            builder.Append(BitConverter.ToUInt16(bytes).ToString());
                            break;

                        case "sbyte":
                            builder.Append(((sbyte)bytes[0]).ToString());
                            break;

                        case "byte":
                            builder.Append(bytes[0].ToString());
                            break;

                        default:
                            builder.Append(BitConverter.ToString(bytes));
                            break;
                    }

                    builder.Append(", ");
                }

                builder.Remove(builder.Length - 2, 2);
                builder.Append($")");
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
