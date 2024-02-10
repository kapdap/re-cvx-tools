using RDXplorer.Extensions;
using RDXplorer.Models.RDX;
using System;
using System.Collections.Generic;
using System.IO;

namespace RDXplorer.Formats.RDX
{
    public static class Export
    {
        public static void Document(DocumentModel document, DirectoryInfo folder)
        {
            Header(document, folder);
            Tables(document, folder);
            Models(document, folder);
            Motions(document, folder);
            Scripts(document, folder);
            Textures(document, folder);
        }

        public static void Header(DocumentModel document, DirectoryInfo folder)
        {
            using FileStream fs = document.PathInfo.OpenReadShared();
            using BinaryReader br = new(fs);

            fs.Seek(0, SeekOrigin.Begin);
            WriteFile(br.ReadBytes(0x46C), new(Path.Combine(folder.FullName, "header.bin")));
        }

        public static void Tables(DocumentModel document, DirectoryInfo folder)
        {
            using FileStream fs = document.PathInfo.OpenReadShared();
            using BinaryReader br = new(fs);

            List<HeaderEntryModel> properties = new()
            {
                document.Header.Camera,
                document.Header.Lighting,
                document.Header.Enemy,
                document.Header.Object,
                document.Header.Item,
                document.Header.Effect,
                document.Header.Boundary,
                document.Header.AOT,
                document.Header.Trigger,
                document.Header.Player,
                document.Header.Event,
                document.Header.Unknown1,
                //document.Header.Unknown2,
                document.Header.Action,
                document.Header.Text,
                document.Header.Sysmes,
            };

            for (int i = 0; i < properties.Count; i++)
            {
                HeaderEntryModel model = properties[i];

                if (model.Value == 0)
                    continue;

                int nextIndex = i;
                int nextOffset = 0;

                while (nextOffset == 0 && nextIndex < properties.Count)
                    nextOffset = nextIndex < properties.Count - 1
                        ? (int)properties[++nextIndex].Value
                        : (int)document.Header.Model.Value;

                fs.Seek(model.Value, SeekOrigin.Begin);

                WriteFile(br.ReadBytes(nextOffset - (int)model.Value),
                    new(Path.Combine(folder.FullName, $"tbl_{model.Name.ToLower().Replace(" ", "_")}.bin")));
            }
        }

        public static void Models(DocumentModel document, DirectoryInfo folder)
        {
            using FileStream fs = document.PathInfo.OpenReadShared();
            using BinaryReader br = new(fs);

            for (int i = 0; i < document.Model.Count; i++)
            {
                ModelTableModel table = document.Model[i];
                fs.Seek(table.Fields.Pointer.Value, SeekOrigin.Begin);
                WriteFile(br.ReadBytes((int)table.Size),
                    new(Path.Combine(folder.FullName, $"mdl_{i}.bin")));
            }
        }

        public static void Motions(DocumentModel document, DirectoryInfo folder)
        {
            using FileStream fs = document.PathInfo.OpenReadShared();
            using BinaryReader br = new(fs);

            for (int i = 0; i < document.Motion.Count; i++)
            {
                MotionTableModel table = document.Motion[i];

                for (int j = 0; j < table.Blocks.Count; j++)
                {
                    MotionBlockModel block = table.Blocks[j];
                    fs.Seek(block.Position, SeekOrigin.Begin);
                    WriteFile(br.ReadBytes((int)block.Fields.Size.Value),
                        new(Path.Combine(folder.FullName, $"mtn_{i}_{j}.bin")));
                }
            }
        }

        public static void Scripts(DocumentModel document, DirectoryInfo folder)
        {
            using FileStream fs = document.PathInfo.OpenReadShared();
            using BinaryReader br = new(fs);

            for (int i = 0; i < document.Script.Count; i++)
            {
                ScriptModel model = document.Script[i];
                Scripting scripting = new();

                fs.Seek(model.Pointer.Value, SeekOrigin.Begin);
                byte[] data = br.ReadBytes((int)model.Size);

                WriteFile(data, new(Path.Combine(folder.FullName, $"scd_{i}.bin")));
                WriteFile(scripting.Decode(data), new(Path.Combine(folder.FullName, $"scd_{i}_hex.txt")));
                WriteFile(scripting.Decompile(data), new(Path.Combine(folder.FullName, $"scd_{i}_code.txt")));
            }
        }

        public static void Textures(DocumentModel document, DirectoryInfo folder)
        {
            using FileStream fs = document.PathInfo.OpenReadShared();
            using BinaryReader br = new(fs);

            for (int i = 0; i < document.Texture.Count; i++)
            {
                TextureTableModel table = document.Texture[i];

                for (int j = 0; j < table.Blocks.Count; j++)
                {
                    TextureBlockModel block = table.Blocks[j];

                    fs.Seek(block.Position + 32, SeekOrigin.Begin);

                    string extension = ".bin";

                    if (block.Fields.Type.Text == "TIM2")
                        extension = ".tm2";
                    else if (block.Fields.Type.Text.Contains("PIL"))
                        extension = ".pil";
                    else if (block.Fields.Type.Text.Contains("PVR"))
                        extension = ".pvr";
                    else if (block.Fields.Type.Text.Contains("PVP"))
                        extension = ".pvp";

                    WriteFile(br.ReadBytes((int)block.Fields.Size.Value),
                        new(Path.Combine(folder.FullName, $"tex_{i}_{j}{extension}")));
                }
            }
        }

        private static void WriteFile(string data, FileInfo file)
        {
            CreateDirectory(file.Directory);
            File.WriteAllText(file.FullName, data);
        }

        private static void WriteFile(Span<byte> data, FileInfo file)
        {
            CreateDirectory(file.Directory);

            using FileStream fs = file.Create();
            fs.Write(data);
        }

        private static void CreateDirectory(DirectoryInfo folder)
        {
            if (!folder.Exists)
            {
                folder.Create();
                folder.Refresh();
            }
        }
    }
}
