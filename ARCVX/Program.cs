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

        List<FileInfo> files = new();

        if (Directory.Exists(args[0]))
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
            ExtractFile(file);

        Console.WriteLine();
        Console.WriteLine($"ARC extraction complete.");
        Console.ReadLine();
    }

    public static void ExtractFile(FileInfo file)
    {
        using ARC arc = new(file);

        if (!arc.IsValid())
        {
            Console.WriteLine($"{file.FullName} is not a supported ARC file.");
            return;
        }

        Console.WriteLine(file.FullName);
        Console.WriteLine();

        foreach (ARCEntry entry in arc.ExportAllEntries())
            Console.WriteLine($"Extracting {entry.Path}");

        Console.WriteLine("---------------------------------");
    }
}
