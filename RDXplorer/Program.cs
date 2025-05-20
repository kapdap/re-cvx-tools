using Microsoft.Win32;
using RDXplorer.Formats.RDX;
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

        public static DirectoryInfo TempPath { get; set; }

        public static void Initialize(MainWindow main)
        {
            Windows.Main = main;

            Models.AppView = new();
            main.AppViewModel = Models.AppView;
            main.DataContext = Models.AppView;
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

        public static void SetTempPath()
        {
            try
            {
                TempPath = new DirectoryInfo($"{Properties.Settings.Default.tmp_path}\\{Guid.NewGuid()}");

                if (!TempPath.Exists)
                    TempPath.Create();

                TempPath.Refresh();
            }
            catch { }
        }

        public static void DeleteTempPath()
        {
            try
            {
                TempPath.Refresh();

                if (TempPath.Exists)
                    TempPath.Delete(true);

                TempPath.Refresh();
            }
            catch { }
        }

        public static void ExportDocument(DirectoryInfo folder)
        {
            if (folder == null || Models.AppView.CurrentFileInfo == null)
                return;

            try
            {
                if (Models.AppView.RDXDocument != null)
                    Export.Document(Models.AppView.RDXDocument, new(Path.Combine(folder.FullName, Models.AppView.CurrentFileInfo.Name)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Application Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public static void ExportTables(DirectoryInfo folder)
        {
            if (folder == null || Models.AppView.CurrentFileInfo == null)
                return;

            try
            {
                if (Models.AppView.RDXDocument != null)
                    Export.Tables(Models.AppView.RDXDocument, new(Path.Combine(folder.FullName, Models.AppView.CurrentFileInfo.Name)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Application Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public static void ExportModels(DirectoryInfo folder)
        {
            if (folder == null || Models.AppView.CurrentFileInfo == null)
                return;

            try
            {
                if (Models.AppView.RDXDocument != null)
                    Export.Models(Models.AppView.RDXDocument, new(Path.Combine(folder.FullName, Models.AppView.CurrentFileInfo.Name)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Application Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public static void ExportMotions(DirectoryInfo folder)
        {
            if (folder == null || Models.AppView.CurrentFileInfo == null)
                return;

            try
            {
                if (Models.AppView.RDXDocument != null)
                    Export.Motions(Models.AppView.RDXDocument, new(Path.Combine(folder.FullName, Models.AppView.CurrentFileInfo.Name)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Application Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public static void ExportScripts(DirectoryInfo folder)
        {
            if (folder == null || Models.AppView.CurrentFileInfo == null)
                return;

            try
            {
                if (Models.AppView.RDXDocument != null)
                    Export.Scripts(Models.AppView.RDXDocument, new(Path.Combine(folder.FullName, Models.AppView.CurrentFileInfo.Name)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Application Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public static void ExportTextures(DirectoryInfo folder)
        {
            if (folder == null || Models.AppView.CurrentFileInfo == null)
                return;

            try
            {
                if (Models.AppView.RDXDocument != null)
                    Export.Textures(Models.AppView.RDXDocument, new(Path.Combine(folder.FullName, Models.AppView.CurrentFileInfo.Name)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Application Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public static void ExportHeader(DirectoryInfo folder)
        {
            if (folder == null || Models.AppView.CurrentFileInfo == null)
                return;

            try
            {
                if (Models.AppView.RDXDocument != null)
                    Export.Header(Models.AppView.RDXDocument, new(Path.Combine(folder.FullName, Models.AppView.CurrentFileInfo.Name)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Application Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public static void ExportFiles(DirectoryInfo folder)
        {
            if (folder == null || Models.AppView.RDXFileList == null)
                return;

            try
            {
                Export.Files(Models.AppView.RDXFileList, folder);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Application Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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

                if (Windows.HexEditor.IsVisible && Models.AppView.RDXLoaded)
                {
                    Windows.HexEditor.ShowFile(Models.AppView.RDXDocument.RDXFileInfo);
                    Windows.Main.Focus();
                }
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
                FileInfo file = SelectFile();

                if (file == null)
                    return;

                Models.AppView.LoadFileList(file);

                // Fires Event MainWindow.FileList_SelectionChanged() > Program.LoadRDX()
                Windows.Main.FileList.SelectedValue = file.FullName;
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

            Program.DeleteTempPath();
            Program.CloseRDX();
            Windows.CloseAll();
        }

        public static class Windows
        {
            private static MainWindow _main;
            public static MainWindow Main
            {
                get
                {
                    _main ??= new();
                    return _main;
                }

                set => _main = value;
            }

            private static HexEditorWindow _hexEditor;
            public static HexEditorWindow HexEditor
            {
                get
                {
                    _hexEditor ??= new();
                    return _hexEditor;
                }

                set => _hexEditor = value;
            }

            private static ScriptingWindow _scripting;
            public static ScriptingWindow Scripting
            {
                get
                {
                    _scripting ??= new();
                    return _scripting;
                }

                set => _scripting = value;
            }

            private static BitmapWindow _bitmap;
            public static BitmapWindow Bitmap
            {
                get
                {
                    _bitmap ??= new();
                    return _bitmap;
                }

                set => _bitmap = value;
            }

            private static AboutWindow _about;
            public static AboutWindow About
            {
                get
                {
                    _about ??= new();
                    return _about;
                }

                set => _about = value;
            }

            public static void CloseAll()
            {
                _hexEditor?.Close();
                _scripting?.Close();
                _bitmap?.Close();
                _about?.Close();
            }
        }

        public static class Models
        {
            public static AppViewModel AppView { get; set; }
        }
    }
}