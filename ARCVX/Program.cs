using ARCVX.Formats;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ARCVX
{
    internal class Program
    {
        private const string EXTRACT = ".extract";

        private static HashSet<string> Convert { get; } = [".tex", ".mes", ".evt"];

        private static async Task<int> Main(string[] args)
        {
            // Detect path type and set default arguments
            if (args.Length > 0 && Path.Exists(args[0]))
            {
                string ext = Path.GetExtension(args[0]);

                if (ext == EXTRACT)
                {
                    DirectoryInfo folder = new(Path.ChangeExtension(args[0], null));

                    if (folder.Exists)
                        args = ["rebuild", "--path", folder.FullName];
                    else
                        args = ["convert", "--path", .. args];
                }
                else if (Convert.Contains(ext))
                    args = ["convert", "--path", .. args];
                else
                    args = ["extract", "--path", .. args];
            }

            Option<string> arcOption = new(
                aliases: ["-p", "--path"],
                description: "Path to .arc file or folder containing .arc files",
                isDefault: true,
                parseArgument: result =>
                {
                    try
                    {
                        string path = result.Tokens.Single().Value;

                        if (!Path.Exists(path))
                            throw new Exception("Path does not exist");

                        return path;
                    }
                    catch(Exception e)
                    {
                        result.ErrorMessage = e.Message;
                        return null;
                    }
                }
            );
            arcOption.IsRequired = true;

            Option<string> pathOption = new(
                aliases: ["-p", "--path"],
                description: "Path to content file or folder",
                isDefault: true,
                parseArgument: result =>
                {
                    try
                    {
                        string path = result.Tokens.Single().Value;

                        if (!Path.Exists(path))
                            throw new Exception("Path does not exist");

                        return path;
                    }
                    catch (Exception e)
                    {
                        result.ErrorMessage = e.Message;
                        return null;
                    }
                }
            );
            pathOption.IsRequired = true;

            Option<DirectoryInfo> extractOption = new(
                aliases: ["-e", "--extract"],
                description: $"Optional path to extract .arc contents (<path>{EXTRACT})"
            );

            Option<DirectoryInfo> rebuildOption = new(
                aliases: ["-r", "--rebuild"], 
                description: $"Optional path to folder with content to rebuild .arc container (<path>{EXTRACT})"
            );

            RootCommand rootCommand = new("Extract and rebuild Resident Evil/Biohazard: Code: Veronica X HD .arc files");

            Command extractCommand = new("extract", "Extract .arc container") { arcOption, extractOption };
            extractCommand.SetHandler((path, extract) => { ExtractCommand(path!, extract!); }, arcOption, extractOption);
            rootCommand.AddCommand(extractCommand);

            Command rebuildCommand = new("rebuild", "Rebuild .arc container") { arcOption, rebuildOption, };
            rebuildCommand.SetHandler((path, rebuild) => { RebuildCommand(path!, rebuild!); }, arcOption, rebuildOption);
            rootCommand.AddCommand(rebuildCommand);

            Command convertCommand = new("convert", "Convert files to readable formats") { pathOption };
            convertCommand.SetHandler((path) => { ConvertCommand(path!); }, pathOption);
            rootCommand.AddCommand(convertCommand);

            return await rootCommand.InvokeAsync(args);
        }

        public static void ExtractCommand(string path, DirectoryInfo extract = null)
        {
            DirectoryInfo folder = new(path);
            List<FileInfo> files = [];

            if (folder.Exists)
                files = [.. new DirectoryInfo(path).GetFiles("*.arc", SearchOption.AllDirectories)];
            else
                files.Add(new(path));

            if (files.Count < 1)
            {
                Console.WriteLine($"No .arc files found in directory.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Extracting {files.Count} files...");
            Console.WriteLine();

            foreach (FileInfo file in files)
            {
                DirectoryInfo output =
                    extract != null && extract.Exists ?
                    extract :

                    folder.Exists ?
                    new($"{folder.FullName}{EXTRACT}") :
                    new(Path.Combine(file.Directory.FullName,
                        Path.ChangeExtension(file.Name, EXTRACT)));

                ExtractARC(file, output);
            }

            Console.WriteLine();
            Console.WriteLine($"ARC extraction complete.");
            Console.ReadLine();
        }

        public static void RebuildCommand(string path, DirectoryInfo rebuild = null)
        {
            DirectoryInfo folder = new(path);
            List<FileInfo> files = [];

            if (folder.Exists)
                files = [.. new DirectoryInfo(path).GetFiles("*.arc", SearchOption.AllDirectories)];
            else
                files.Add(new(path));

            if (files.Count < 1)
            {
                Console.WriteLine($"No .arc files found in directory.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Rebuilding {files.Count} files...");
            Console.WriteLine();

            foreach (FileInfo file in files)
            {
                DirectoryInfo input =
                    rebuild != null && rebuild.Exists ?
                    rebuild :

                    folder.Exists ?
                    new($"{folder.FullName}{EXTRACT}") :
                    new(Path.Combine(file.Directory.FullName,
                        Path.ChangeExtension(file.Name, EXTRACT)));

                if (input.Exists)
                    RebuildARC(file, input);
            }

            Console.WriteLine();
            Console.WriteLine($"ARC rebuild complete.");
            Console.ReadLine();
        }

        public static void ConvertCommand(string path)
        {
            DirectoryInfo folder = new(path);
            List<FileInfo> files = [];

            if (folder.Exists)
                files = [.. new DirectoryInfo(path).GetFiles("*.*", SearchOption.AllDirectories)];
            else
                files.Add(new(path));

            if (files.Count < 1)
            {
                Console.WriteLine($"No convertable files found in directory.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Converting files...");
            Console.WriteLine();

            foreach (FileInfo file in files)
            {
                if (file.Extension == ".tex")
                    ConvertTexture(file);

                /*if (file.Extension == ".mes")
                    ConvertMessage(file);

                if (file.Extension == ".evt")
                    ConvertScript(file);*/
            }

            Console.WriteLine();
            Console.WriteLine($"File conversion complete.");
            Console.ReadLine();
        }

        public static void ExtractARC(FileInfo file, DirectoryInfo folder)
        {
            using ARC arc = (ARC)HFS.Unpack(file);

            if (!arc.IsValid)
            {
                Console.WriteLine($"{file.FullName} is not a supported ARC file.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Extracting {file.FullName}");
            Console.WriteLine();

            foreach (ARCExport export in arc.ExportAllEntries(folder))
            {
                if (export == null)
                {
                    Console.Error.WriteLine($"Failed {export.File}");
                    Console.ReadLine();
                    continue;
                }

                Console.WriteLine($"Extracted {export.File}");

                if (export.File.Extension == ".tex")
                    ConvertTexture(export.File);

                /*if (export.File.Extension == ".mes")
                    ConvertMessage(export.File);

                if (export.File.Extension == ".evt")
                    ConvertScript(export.File);*/
            }

            Console.WriteLine("---------------------------------");
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

            Console.Write($"Rebuilding {file.FullName}... ");

            if (hfs.IsValid)
            {
                //using MemoryStream stream = arc.CreateNewStream(folder);
                //_ = hfs.Save(stream);
                _ = arc.Save(folder, new(Path.ChangeExtension(arc.File.FullName, ".tmp")));
            }
            else
            {
                //_ = arc.Save(folder);
                _ = arc.Save(folder, new(Path.ChangeExtension(arc.File.FullName, ".tmp")));
            }

            Console.WriteLine("done!");
        }

        public static void ConvertTexture(FileInfo file)
        {
            using Tex tex = (Tex)HFS.Unpack(file);

            try
            {
                FileInfo output = tex.Export();

                if (output != null)
                    Console.WriteLine("Converted " + output.FullName);
                else
                    Console.WriteLine($"Unsupported " + file.FullName);
            }
            catch
            {
                Console.WriteLine("Failed " + file.FullName);
                Console.ReadLine();
            }
        }

        public static void ConvertMessage(FileInfo file)
        {
            // TODO: Convert message files.
            using Mes mes = new(file);

            FileInfo output;
            if ((output = mes.Export()) != null)
                Console.WriteLine("Converted " + output.FullName);
        }

        public static void ConvertScript(FileInfo file)
        {
            // TODO: Convert script files.
            using Evt evt = new(file);

            FileInfo output;
            if ((output = evt.Export()) != null)
                Console.WriteLine("Converted " + output.FullName);
        }
    }
}