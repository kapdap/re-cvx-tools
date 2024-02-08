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

        public static FileInfo FlagNamePath { get; set; } = new FileInfo("flagnames.json");
        public static Dictionary<string, string> FlagNames { get; set; } = new Dictionary<string, string>();

        public static void Initialize(MainWindow main)
        {
            Windows.Main = main;

            LoadFlagNames();
            ExecuteScanner();
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
            try
            {
                FlagNames = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(FlagNamePath.FullName));
            }
            catch { }
        }

        public static void SaveFlagNames()
        {
            UpdateFlagNames(MemoryScanner.Memory.DocumentFlags);
            UpdateFlagNames(MemoryScanner.Memory.KilledFlags);
            UpdateFlagNames(MemoryScanner.Memory.EventFlags);
            UpdateFlagNames(MemoryScanner.Memory.ItemFlags);
            UpdateFlagNames(MemoryScanner.Memory.MapFlags);

            try
            {
                File.WriteAllText(FlagNamePath.FullName, JsonSerializer.Serialize(FlagNames, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch { }
        }

        private static void UpdateFlagNames(FlagCollection flags)
        {
            foreach (FlagModel model in flags)
            {
                string key = $"{model.Flag}.{model.Offset}";

                if (FlagNames.ContainsKey(key))
                    FlagNames[key] = model.Name;
                else
                    FlagNames.Add(key, model.Name);
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

            public static void CloseAll()
            {
                About?.Close();
            }
        }

        public static class Models
        {
            public static AppViewModel AppViewModel { get; } = new AppViewModel();
        }

        public static void Dispose()
        {
            MemoryScanner?.Dispose();
        }
    }
}