using RDXplorer.Formats.RDX;
using RDXplorer.Models;
using RDXplorer.Models.RDX;
using System.Collections.Generic;
using System.IO;

namespace RDXplorer.ViewModels
{
    public class AppViewModel : BaseNotifyModel
    {
        private string _statusBarText;
        public string StatusBarText
        {
            get => _statusBarText;
            private set => SetField(ref _statusBarText, value);
        }

        public FileInfo CurrentFileInfo =>
            RDXDocument?.PRSFileInfo ?? RDXDocument?.RDXFileInfo;

        private DirectoryInfo _rdxFolderInfo;
        public DirectoryInfo RDXFolderInfo
        {
            get => _rdxFolderInfo;
            private set => SetField(ref _rdxFolderInfo, value);
        }

        public List<FileInfo> _rdxFileList;
        public List<FileInfo> RDXFileList
        {
            get => _rdxFileList;
            private set => SetField(ref _rdxFileList, value);
        }

        private DocumentModel _rdxDocument;
        public DocumentModel RDXDocument
        {
            get => _rdxDocument;
            private set => SetField(ref _rdxDocument, value);
        }

        public bool _rdxLoaded;
        public bool RDXLoaded
        {
            get => _rdxLoaded;
            private set => SetField(ref _rdxLoaded, value);
        }

        public void LoadRDX(FileInfo file)
        {
            RDXDocument = Reader.LoadFile(file);
            RDXLoaded = RDXDocument != null;

            StatusBarText = CurrentFileInfo?.FullName ?? string.Empty;
        }

        public void UnloadRDX()
        {
            RDXDocument = null;
            RDXLoaded = false;

            StatusBarText = string.Empty;
        }

        public void LoadFileList(FileInfo file) =>
            LoadFileList(new(file.Directory.FullName), $"*{file.Extension}");

        public void LoadFileList(DirectoryInfo folder, string filter = "*")
        {
            RDXFolderInfo = folder;
            RDXFileList = [.. RDXFolderInfo.GetFiles(filter)];
        }
    }
}