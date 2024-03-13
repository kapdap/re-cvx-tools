using ARCVX.Formats;
using System;
using System.Collections.Generic;
using System.IO;

namespace ARCVX;

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
        List<FileInfo> files = new();

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
                ConvertTex(file);
            else
                ExtractFile(file, folder.Exists ?
                    new($"{folder.FullName}.export") :
                    new(Path.Combine(file.Directory.FullName,
                        Path.ChangeExtension(file.Name, ".export"))));
        }

        Console.WriteLine();
        Console.WriteLine($"ARC extraction complete.");
        Console.ReadLine();
    }

    public static void ExtractFile(FileInfo file, DirectoryInfo folder)
    {
        using ARC arc = new(file);

        if (!arc.IsHFS)
        {
            Console.WriteLine($"{file.FullName} is not a supported ARC file.");
            return;
        }

        Console.WriteLine($"Extracting {file.FullName}");
        Console.WriteLine();

        foreach (ARCExport export in arc.ExportAllEntries(folder))
        {
            Console.WriteLine($"Extracted {export.Path}");

            if (export.Entry.TypeHash == 0x241F5DEB)
                ConvertTex(new(export.Path));
        }

        Console.WriteLine("---------------------------------");
    }

    public static void ConvertTex(FileInfo file)
    {
        FileInfo output;
        using Tex tex = new(file);
        if ((output = tex.Export()) != null)
            Console.WriteLine("Converted " + output.FullName);
    }
}