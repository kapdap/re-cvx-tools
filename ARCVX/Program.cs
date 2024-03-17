using ARCVX.Formats;
using System;
using System.Collections.Generic;
using System.IO;

namespace ARCVX
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 1 || !Path.Exists(args[0]))
            {
                Console.WriteLine("Path not found.");
                Console.ReadLine();
                return;
            }

            DirectoryInfo folder = new(args[0]);
            List<FileInfo> files = [];

            if (folder.Exists)
                files = [..new DirectoryInfo(args[0]).EnumerateFiles("*.arc",
                    args.Length > 1 && args[1] == "-t" ?
                    SearchOption.TopDirectoryOnly :
                    SearchOption.AllDirectories)];
            else
                files.Add(new(args[0]));

            if (files.Count < 1)
            {
                Console.WriteLine($"No ARC files found in directory.");
                Console.ReadLine();
                return;
            }

            foreach (FileInfo file in files)
            {
                if (file.Extension == ".tex")
                    ConvertTexture(file);
                else
                    ExtractARC(file, folder.Exists ?
                        new($"{folder.FullName}.export") :
                        new(Path.Combine(file.Directory.FullName,
                            Path.ChangeExtension(file.Name, ".export"))));
            }

            Console.WriteLine();
            Console.WriteLine($"ARC extraction complete.");
            Console.ReadLine();
        }

        public static void RebuildARC(FileInfo file, DirectoryInfo folder)
        {
            using HFS hfs = new(file);
            using ARC arc = hfs.IsValid ? new(file, hfs.GetDataStream()) : new(file);

            if (!arc.IsValid)
            {
                Console.WriteLine($"{file.FullName} is not a supported ARC file.");
                return;
            }

            if (hfs.IsValid)
            {
                using MemoryStream stream = arc.CreateNewStream(folder);
                //_ = hfs.Save(stream);
            }
            else
                _ = arc.Save(folder);
        }

        public static void ExtractARC(FileInfo file, DirectoryInfo folder)
        {
            HFS hfs = new(file);
            using ARC arc = hfs.IsValid ? new(file, hfs.GetDataStream()) : new(file);
            hfs.Dispose();

            if (!arc.IsValid)
            {
                Console.WriteLine($"{file.FullName} is not a supported ARC file.");
                return;
            }

            Console.WriteLine($"Extracting {file.FullName}");
            Console.WriteLine();

            foreach (ARCExport export in arc.ExportAllEntries(folder))
            {
                Console.WriteLine($"Extracted {export.File}");

                if (export.Entry.TypeHash == 0x241F5DEB)
                    ConvertTexture(export.File);

                // TODO: Convert more file formats
                /*if (export.Entry.TypeHash == 0x46C78353 ||
                    export.Entry.TypeHash == 0x6A76E771)
                    ConvertTexture(new(export.Path));

                if (export.Entry.TypeHash == 0x375F06DA)
                    ConvertTexture(new(export.Path));*/
            }

            Console.WriteLine("---------------------------------");
        }

        public static void ConvertTexture(FileInfo file)
        {
            HFS hfs = new(file);
            using Tex tex = hfs.IsValid ? new(file, hfs.GetDataStream()) : new(file);
            hfs.Dispose();

            FileInfo output;
            if ((output = tex.Export()) != null)
                Console.WriteLine("Converted " + output.FullName);
        }

        // TODO: Convert script files.
        public static void ConvertScript(FileInfo file)
        {
            using Evt evt = new(file);

            FileInfo output;
            if ((output = evt.Export()) != null)
                Console.WriteLine("Converted " + output.FullName);
        }

        // TODO: Convert message files.
        public static void ConvertMessage(FileInfo file)
        {
            using Mes mes = new(file);

            FileInfo output;
            if ((output = mes.Export()) != null)
                Console.WriteLine("Converted " + output.FullName);
        }
    }
}