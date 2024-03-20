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
using ARCVX.Formats;
using ARCVX.Reader;
using ARCVX.Utilities;
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

        private static Config Config { get; set; }

        private static async Task<int> Main(string[] args)
        {
            // Detect path type and set default arguments
            if (args.Length > 0 && Path.Exists(args[0]))
            {
                string ext = Path.GetExtension(args[0]);

                if (ext == EXTRACT)
                {
                    DirectoryInfo folder = new(Path.ChangeExtension(args[0], null));
                    FileInfo file = new(Path.ChangeExtension(args[0], "arc"));

                    if (folder.Exists)
                        args = ["repack", "--path", folder.FullName, "--folder", .. args];
                    else if (file.Exists)
                        args = ["repack", "--path", file.FullName, "--folder", .. args];
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

                        return !Path.Exists(path) ? throw new Exception("Path does not exist") : path;
                    }
                    catch (Exception e)
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

                        return !Path.Exists(path) ? throw new Exception("Path does not exist") : path;
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
                aliases: ["-f", "--folder"],
                description: $"Optional folder to extract contents (<path>{EXTRACT})"
            );

            Option<DirectoryInfo> repackOption = new(
                aliases: ["-f", "--folder"],
                description: $"Optional folder with content to repack (<path>{EXTRACT})"
            );

            Option<bool> overwriteOption = new(
                aliases: ["-o", "--overwrite"],
                description: "Overwrite existing files after repacking",
                getDefaultValue: () => false
            );

            Option<bool> rebuildOption = new(
                aliases: ["-r", "--rebuild"],
                description: "Use with convert command to rebuild content file",
                getDefaultValue: () => false
            );

            Option<Region> languageOption = new(
                aliases: ["-l", "--lang"],
                description: "Language table to use when decoding and encoding messages",
                getDefaultValue: () => Region.US
            );

            Option<FileInfo> languageFileOption = new(
                aliases: ["-g", "--langFile"],
                description: "Path to custom language table file to use when decoding and encoding messages"
            );

            Option<ByteOrder> byteOrderOption = new(
                aliases: ["-b", "--byteorder"],
                description: "Byte order to use when reading and writing files",
                getDefaultValue: () => ByteOrder.BigEndian
            );

            string description = "Extract and repack Resident Evil/Biohazard: Code: Veronica X HD .arc files" + Environment.NewLine + Environment.NewLine;
            description += "To extract drag and drop a .arc file or a folder containing .arc files onto ARCVX.exe" + Environment.NewLine;
            description += "To repack drag and drop the \"<name>.extract\" folder onto ARCVX.exe";

            RootCommand rootCommand = new(description);

            Command extractCommand = new("extract", "Extract .arc container");
            extractCommand.SetHandler((config) =>
            {
                Config = config;
                ExtractCommand();
            }, new ConfigBinder(arcOption, extractOption, overwriteOption, languageOption, languageFileOption, byteOrderOption));
            extractCommand.AddOption(arcOption);
            extractCommand.AddOption(extractOption);
            extractCommand.AddOption(languageOption);
            extractCommand.AddOption(languageFileOption);

            Command repackCommand = new("repack", "Repack .arc container");
            repackCommand.SetHandler((config) =>
            {
                Config = config;
                RepackCommand();
            }, new ConfigBinder(arcOption, repackOption, overwriteOption, languageOption, languageFileOption, byteOrderOption));
            repackCommand.AddOption(arcOption);
            repackCommand.AddOption(repackOption);
            repackCommand.AddOption(overwriteOption);
            repackCommand.AddOption(languageOption);
            repackCommand.AddOption(languageFileOption);

            Command convertCommand = new("convert", "Convert files to readable formats");
            convertCommand.SetHandler((config) =>
            {
                Config = config;
                ConvertCommand();
            }, new ConfigBinder(pathOption, repackOption, rebuildOption, languageOption, languageFileOption, byteOrderOption));
            convertCommand.AddOption(pathOption);
            convertCommand.AddOption(rebuildOption);
            convertCommand.AddOption(languageOption);
            convertCommand.AddOption(languageFileOption);

            Command unpackCommand = new("unpack", "Unpack file from HFS container");
            unpackCommand.SetHandler((config) =>
            {
                Config = config;
                UnpackCommand();
            }, new ConfigBinder(pathOption, repackOption, rebuildOption, languageOption, languageFileOption, byteOrderOption));
            unpackCommand.AddOption(pathOption);
            unpackCommand.AddOption(repackOption);
            unpackCommand.AddOption(rebuildOption);

            rootCommand.AddGlobalOption(byteOrderOption);

            rootCommand.AddCommand(extractCommand);
            rootCommand.AddCommand(repackCommand);
            rootCommand.AddCommand(convertCommand);
            rootCommand.AddCommand(unpackCommand);

            return await rootCommand.InvokeAsync(args);
        }

        public static Task<int> ExtractCommand()
        {
            DirectoryInfo folder = new(Config.Path);
            List<FileInfo> files = [];

            if (folder.Exists)
                files = [.. folder.GetFiles("*.arc", SearchOption.AllDirectories)];
            else
                files.Add(new(Config.Path));

            if (files.Count < 1)
            {
                Console.WriteLine("No .arc files found in directory.");
                Console.ReadLine();
                return Task.FromResult(1);
            }

            Console.WriteLine($"Extracting {files.Count} files...");
            Console.WriteLine();

            foreach (FileInfo file in files)
            {
                try
                {
                    DirectoryInfo output =
                        Config.Folder != null && Config.Folder.Exists ?
                        Config.Folder :

                        folder.Exists ?
                        new($"{folder.FullName}{EXTRACT}") :
                        new(Path.Combine(file.Directory.FullName,
                            Path.ChangeExtension(file.Name, EXTRACT)));

                    ExtractARC(file, output);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed " + file.FullName);
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }

            Console.WriteLine();
            Console.WriteLine("ARC extraction complete.");
            Console.ReadLine();

            return Task.FromResult(0);
        }

        public static Task<int> RepackCommand()
        {
            if (Config.Overwrite && !CLI.Confirm($"Rebuilding will destroy existing .arc files.{Environment.NewLine}" +
                $"Ensure you have backups avaliable.{Environment.NewLine}Are you sure you want to continue?"))
                return Task.FromResult(2);

            DirectoryInfo folder = new(Config.Path);
            List<FileInfo> files = [];

            if (folder.Exists)
                files = [.. folder.GetFiles("*.arc", SearchOption.AllDirectories)];
            else
                files.Add(new(Config.Path));

            if (files.Count < 1)
            {
                Console.WriteLine("No .arc files found in directory.");
                Console.ReadLine();
                return Task.FromResult(1);
            }

            Console.WriteLine($"Repacking {files.Count} files...");
            Console.WriteLine();

            foreach (FileInfo file in files)
            {
                try
                {
                    DirectoryInfo input =
                        Config.Folder != null && Config.Folder.Exists ?
                        Config.Folder :

                        folder.Exists ?
                        new($"{folder.FullName}{EXTRACT}") :
                        new(Path.Combine(file.Directory.FullName,
                            Path.ChangeExtension(file.Name, EXTRACT)));

                    if (input.Exists)
                        RepackARC(file, input);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed " + file.FullName);
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }

            Console.WriteLine();
            Console.WriteLine("ARC repacking complete.");
            Console.ReadLine();

            return Task.FromResult(0);
        }

        public static Task<int> UnpackCommand()
        {
            DirectoryInfo folder = new(Config.Path);
            List<FileInfo> files = [];

            if (folder.Exists)
                files = [.. folder.GetFiles("*.*", SearchOption.AllDirectories)];
            else
                files.Add(new(Config.Path));

            if (files.Count < 1)
            {
                Console.WriteLine("No files found in directory.");
                Console.ReadLine();
                return Task.FromResult(1);
            }

            Console.WriteLine($"Unpacking HFS files...");
            Console.WriteLine();

            DirectoryInfo outputFolder = Config.Folder != null && 
                Config.Folder.Exists ? Config.Folder : 
                Config.Overwrite ? new($"{folder.FullName.Replace(".unhfs", string.Empty)}.rehfs") : new($"{folder.FullName}.unhfs");

            foreach (FileInfo file in files)
            {
                string path = file.Name;

                if (file.FullName.StartsWith(folder.FullName))
                    path = file.FullName.Substring(folder.FullName.Length);

                FileInfo outputFile = new(Path.Join(outputFolder.FullName, path));

                if (!outputFile.Directory.Exists)
                    outputFile.Directory.Create();

                try
                {
                    if (Config.Overwrite)
                    {
                        using (HFS hfs = new(file) { ByteOrder = Config.ByteOrder })
                        {
                            if (hfs.IsValid)
                                continue;

                            using FileStream inputStream = file.OpenReadShared();

                            _ = hfs.SaveStream(inputStream, outputFile);
                        }

                        Console.WriteLine($"Repacked {file.FullName}");
                    }
                    else
                    {
                        using HFS hfs = new(file) { ByteOrder = Config.ByteOrder };

                        if (!hfs.IsValid)
                            continue;

                        using (MemoryStream dataStream = hfs.GetDataStream())
                        {
                            hfs.Dispose();

                            using FileStream outputStream = outputFile.OpenWrite();

                            dataStream.CopyTo(outputStream);
                        }

                        Console.WriteLine($"Unpacked {file.FullName}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed {file.FullName}");
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }

            Console.WriteLine();
            Console.WriteLine("HFS unpack complete.");
            Console.ReadLine();

            return Task.FromResult(0);
        }

        public static Task<int> ConvertCommand()
        {
            DirectoryInfo folder = new(Config.Path);
            List<FileInfo> files = [];

            if (folder.Exists)
                files = [.. folder.GetFiles("*.*", SearchOption.AllDirectories)];
            else
                files.Add(new(Config.Path));

            if (files.Count < 1)
            {
                Console.WriteLine("No convertable files found in directory.");
                Console.ReadLine();

                return Task.FromResult(1);
            }

            Console.WriteLine("Converting files...");
            Console.WriteLine();

            foreach (FileInfo file in files)
            {
                if (file.Extension == ".tex")
                    ConvertTexture(file);

                if (file.Extension == ".mes")
                    ConvertMessage(file);

                /*if (file.Extension == ".evt")
                    ConvertScript(file);*/
            }

            Console.WriteLine();
            Console.WriteLine("File conversion complete.");
            Console.ReadLine();

            return Task.FromResult(0);
        }

        public static void ExtractARC(FileInfo file, DirectoryInfo folder)
        {
            HFS hfs = new(file) { ByteOrder = Config.ByteOrder };
            using ARC arc = hfs.IsValid ? new(file, hfs.GetDataStream()) : new(file);
            hfs.Dispose();

            arc.ByteOrder = Config.ByteOrder;

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

                if (export.File.Extension == ".mes")
                    ConvertMessage(export.File);

                /*if (export.File.Extension == ".evt")
                    ConvertScript(export.File);*/
            }

            Console.WriteLine("---------------------------------");
        }

        public static void RepackARC(FileInfo file, DirectoryInfo folder)
        {
            using HFS hfs = new(file) { ByteOrder = Config.ByteOrder };
            using ARC arc = hfs.IsValid ? new(file, hfs.GetDataStream()) : new(file);

            arc.ByteOrder = Config.ByteOrder;

            if (!arc.IsValid)
            {
                Console.WriteLine($"{file.FullName} is not a supported ARC file.");
                return;
            }

            Console.Write($"Rebuilding {file.FullName}... ");

            arc.Language = Config.Language;
            arc.LanguageFile = Config.LanguageFile;

            try
            {
                if (hfs.IsValid)
                {
                    using MemoryStream stream = arc.CreateNewStream(folder);
                    _ = Config.Overwrite ? hfs.SaveStream(stream) : hfs.SaveStream(stream, new(Path.ChangeExtension(arc.File.FullName, ".tmp")));
                }
                else
                {
                    _ = Config.Overwrite ? arc.Save(folder) : arc.Save(folder, new(Path.ChangeExtension(arc.File.FullName, ".tmp")));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed " + file.FullName);
                Console.WriteLine(e.Message);
                Console.ReadLine();

                return;
            }

            Console.WriteLine("done!");
        }

        public static void ConvertTexture(FileInfo file)
        {
            HFS hfs = new(file) { ByteOrder = Config.ByteOrder };
            using Tex tex = hfs.IsValid ? new(file, hfs.GetDataStream()) : new(file);
            hfs.Dispose();

            tex.ByteOrder = Config.ByteOrder;

            try
            {
                FileInfo output = tex.Export();

                if (output != null)
                    Console.WriteLine("Converted " + output.FullName);
                else
                    Console.WriteLine("Unsupported " + file.FullName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed " + file.FullName);
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        public static void ConvertMessage(FileInfo file)
        {
            using Mes mes = new(file) { ByteOrder = Config.ByteOrder };

            try
            {
                if (Config.LanguageFile != null)
                    mes.LoadLanguage(Config.LanguageFile);
                else
                    mes.LoadLanguage(Config.Language);

                if (Config.Overwrite)
                {
                    _ = mes.Save();
                    Console.WriteLine("Rebuilt " + file.FullName);
                }
                else
                {
                    List<FileInfo> output = mes.Export();

                    if (output.Count > 0)
                        Console.WriteLine("Converted " + file.FullName);
                    else
                        Console.WriteLine("Unsupported " + file.FullName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed " + file.FullName);
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        public static void ConvertScript(FileInfo file)
        {
            // TODO: Convert script files.
            using Evt evt = new(file) { ByteOrder = Config.ByteOrder };

            try
            {
                FileInfo output = evt.Export();

                if (output != null)
                    Console.WriteLine("Converted " + file.FullName);
                else
                    Console.WriteLine("Unsupported " + file.FullName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed " + file.FullName);
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
    }
}