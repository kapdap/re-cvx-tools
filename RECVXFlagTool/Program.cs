using Microsoft.Win32;
using RECVXFlagTool.Collections;
using RECVXFlagTool.Models;
using RECVXFlagTool.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace RECVXFlagTool
{
    public static class Program
    {
        public static GameMemoryScanner MemoryScanner { get; } = new GameMemoryScanner(GameEmulator.DetectEmulator());

        public static bool IsGameRunning
        {
            get
            {
                if (!MemoryScanner.ProcessRunning)
                    return false;

                MemoryScanner.UpdateVirtualMemoryPointer();
                MemoryScanner.UpdateGameVersion();

                return MemoryScanner.Memory.Game.Supported;
            }
        }

        public static bool IsClosing { get; set; } = false;

        public static void Initialize(MainWindow main)
        {
            Windows.Main = main;

            Models.AppViewModel.PropertyChanged += AppViewModel_PropertyChanged;

            LoadFlagNames();
            ExecuteScanner();
        }

        private static void AppViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FlagNames")
                Models.AppViewModel.Memory.ClearFlags();
        }

        private static async void ExecuteScanner()
        {
            while (true)
            {
                DetectEmulator();
                RefreshMemory();

                // Task.Delay to reduce CPU usage
                await Task.Delay(33);
            }
        }

        private static void DetectEmulator()
        {
            GameEmulator emulator = GameEmulator.DetectEmulator();
            if (emulator != null && !emulator.Process.Equals(MemoryScanner.Emulator?.Process))
                MemoryScanner.Initialize(emulator);
        }

        private static void RefreshMemory()
        {
            bool isGameRunning = IsGameRunning;

            SetStatusBarText(isGameRunning);

            if (!isGameRunning)
                return;

            MemoryScanner.Refresh();
        }

        private static void SetStatusBarText(bool isGameRunning)
        {
            GameModel game = MemoryScanner.Memory.Game;
            GameEmulator emulator = MemoryScanner.Emulator;

            Models.AppViewModel.StatusBar = isGameRunning
                ? $"{game.Name} ({game.Code}) ({game.Country}) ({game.Console}) ({emulator.Process.ProcessName} ({emulator.Process.Id})))"
                : MemoryScanner.ProcessRunning
                ? $"{emulator.Process.ProcessName} ({emulator.Process.Id}) detected"
                : "No emulator process detected";
        }

        public static void LoadFlagNames()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.LastOpenFile))
                LoadFlagNames(new(Properties.Settings.Default.LastOpenFile));
            else
                LoadFlagNames(new("flagnames.json"));
        }

        public static void LoadFlagNames(FileInfo file)
        {
            if (file == null || !file.Exists)
                return;

            try
            {
                Models.AppViewModel.FlagNames = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(file.FullName));
                Models.AppViewModel.FlagFile = file;

                Properties.Settings.Default.LastOpenFile = file.FullName;
                Properties.Settings.Default.Save();
            }
            catch { }
        }

        public static void SaveFlagNames() =>
            SaveFlagNames(Models.AppViewModel.FlagFile ?? SaveFile());

        public static void SaveFlagNames(FileInfo file)
        {
            if (file == null)
                return;

            UpdateFlagNames(MemoryScanner.Memory.DocumentFlags);
            UpdateFlagNames(MemoryScanner.Memory.KilledFlags);
            UpdateFlagNames(MemoryScanner.Memory.EventFlags);
            UpdateFlagNames(MemoryScanner.Memory.ItemFlags);
            UpdateFlagNames(MemoryScanner.Memory.MapFlags);

            try
            {
                File.WriteAllText(file.FullName, JsonSerializer.Serialize(Models.AppViewModel.FlagNames, new JsonSerializerOptions { WriteIndented = true }));

                if (Models.AppViewModel.FlagFile == null)
                {
                    Models.AppViewModel.FlagFile = file;

                    Properties.Settings.Default.LastOpenFile = file.FullName;
                    Properties.Settings.Default.Save();
                }
            }
            catch { }
        }

        public static void ExportFlagsCSV(DirectoryInfo folder)
        {
            if (folder == null)
                return;

            WriteFlagsCSV(new(Path.Combine(folder.FullName, "documents.csv")), MemoryScanner.Memory.DocumentFlags);
            WriteFlagsCSV(new(Path.Combine(folder.FullName, "enemies.csv")), MemoryScanner.Memory.KilledFlags);
            WriteFlagsCSV(new(Path.Combine(folder.FullName, "events.csv")), MemoryScanner.Memory.EventFlags);
            WriteFlagsCSV(new(Path.Combine(folder.FullName, "items.csv")), MemoryScanner.Memory.ItemFlags);
            WriteFlagsCSV(new(Path.Combine(folder.FullName, "maps.csv")), MemoryScanner.Memory.MapFlags);
        }

        public static void WriteFlagsCSV(FileInfo file, FlagCollection flags)
        {
            try
            {
                if (file.Exists)
                    file.Delete();

                using StreamWriter writer = new(file.Open(FileMode.CreateNew, FileAccess.Write, FileShare.None));

                writer.WriteLine("Index,Pointer,Bit,Name");
                foreach (FlagModel model in flags)
                    writer.WriteLine($"{model.Index},0x{model.Pointer - MemoryScanner.Emulator.VirtualMemoryPointer:X8},{model.Order},\"{model.Name?.Replace("\"", "\"\"")}\"");
            }
            catch { }
        }

        private static void UpdateFlagNames(FlagCollection flags)
        {
            foreach (FlagModel model in flags)
            {
                string key = $"{model.Flag}.{model.Index}";

                if (Models.AppViewModel.FlagNames.ContainsKey(key))
                    Models.AppViewModel.FlagNames[key] = model.Name;
                else
                    Models.AppViewModel.FlagNames.Add(key, model.Name);
            }
        }

        public static FileInfo SelectFile() =>
            SelectFile((FileInfo)null);

        public static FileInfo SelectFile(string path) =>
            SelectFile(new FileInfo(path));

        public static FileInfo SelectFile(FileInfo path)
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = false,
                CheckFileExists = true,
                FileName = path != null && path.Exists ? path.FullName : string.Empty
            };

            return dialog.ShowDialog() == null || !File.Exists(dialog.FileName)
                ? null
                : new FileInfo(dialog.FileName);
        }

        public static DirectoryInfo SelectFolder() =>
            SelectFolder((DirectoryInfo)null);

        public static DirectoryInfo SelectFolder(string path) =>
            SelectFolder(new DirectoryInfo(path));

        public static DirectoryInfo SelectFolder(DirectoryInfo path)
        {
            OpenFolderDialog dialog = new()
            {
                Multiselect = false,
                FolderName = path != null && path.Exists ? path.FullName : string.Empty,
            };

            return dialog.ShowDialog() == null || !Directory.Exists(dialog.FolderName)
                ? null
                : new DirectoryInfo(dialog.FolderName);
        }

        public static FileInfo SaveFile() =>
            SaveFile(null);

        public static FileInfo SaveFile(FileInfo path)
        {
            SaveFileDialog dialog = new()
            {
                DefaultExt = "json",
                Filter = "JSON Files | *.json",
                FileName = path != null && path.Exists ? path.FullName : string.Empty
            };

            return dialog.ShowDialog() == null
                ? null
                : new FileInfo(dialog.FileName);
        }

        public static void CloseApp()
        {
            IsClosing = true;
            Windows.CloseAll();
        }

        public static class Windows
        {
            public static MainWindow Main { get; set; }
            public static AboutWindow About { get; set; } = new AboutWindow();

            public static void CloseAll() =>
                About?.Close();
        }

        public static class Models
        {
            public static AppViewModel AppViewModel { get; } = new AppViewModel();
        }

        public static void Dispose() =>
            MemoryScanner?.Dispose();
    }
}