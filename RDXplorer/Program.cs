using Microsoft.Win32;
using RDXplorer.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace RDXplorer.Views
{
    public static class Program
    {
        public static bool IsClosing { get; set; }

        public static void Initialize(MainWindow main)
        {
            Windows.Main = main;

            Models.AppView = new AppViewModel();
            main.AppViewModel = Models.AppView;
            main.DataContext = Models.AppView;
        }

        public static FileInfo SelectFile() =>
            SelectFile((FileInfo)null);

        public static FileInfo SelectFile(string path) =>
            SelectFile(new FileInfo(path));

        public static FileInfo SelectFile(FileInfo path)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Multiselect = false,
                CheckFileExists = true,
                FileName = path != null && path.Exists ? path.FullName : String.Empty
            };

            if (dialog.ShowDialog() == null || !File.Exists(dialog.FileName))
                return null;

            return new FileInfo(dialog.FileName);
        }

        public static void OpenURL(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    Process.Start("xdg-open", url);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    Process.Start("open", url);
                else
                    throw;
            }
        }

        public static void LoadRDX(FileInfo file)
        {
            try
            {
                if (file == null)
                    return;

                Models.AppView.LoadRDX(file);

                if (Windows.HexEditor.IsVisible)
                    Windows.HexEditor.ShowFile(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Application Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public static void OpenRDX()
        {
            try
            {
                LoadRDX(SelectFile());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Application Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public static void CloseRDX()
        {
            try
            {
                Windows.HexEditor.HexEdit.CloseProvider();
                Models.AppView.UnloadRDX();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Application Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public static void CloseApp()
        {
            IsClosing = true;

            Program.CloseRDX();
            Windows.CloseAll();
        }

        public static class Windows
        {
            public static MainWindow Main { get; set; }

            private static HexEditorWindow _hexEditor;
            public static HexEditorWindow HexEditor
            {
                get
                {
                    if (_hexEditor == null)
                        _hexEditor = new HexEditorWindow();

                    return _hexEditor;
                }

                set
                {
                    _hexEditor = value;
                }
            }

            private static AboutWindow _about;
            public static AboutWindow About
            {
                get
                {
                    if (_about == null)
                        _about = new AboutWindow();

                    return _about;
                }

                set
                {
                    _about = value;
                }
            }

            public static void CloseAll()
            {
                _hexEditor?.Close();
                _about?.Close();
            }
        }

        public static class Models
        {
            public static AppViewModel AppView { get; set; }
        }
    }
}