using RDXplorer.Models;
using RDXplorer.Models.RDX;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        private FileInfo _prsFileInfo;
        public FileInfo PRSFileInfo
        {
            get => _prsFileInfo;
            private set => SetField(ref _prsFileInfo, value);
        }

        private FileInfo _rdxFileInfo;
        public FileInfo RDXFileInfo
        {
            get => _rdxFileInfo;
            private set => SetField(ref _rdxFileInfo, value);
        }

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

        public void LoadRDX(FileInfo file, FileInfo prs_file)
        {
            LoadRDX(file);

            PRSFileInfo = prs_file;

            StatusBarText = PRSFileInfo?.FullName ?? RDXFileInfo.FullName;
        }

        public void LoadRDX(FileInfo file)
        {
            PRSFileInfo = null;
            RDXFileInfo = file;
            RDXDocument = new(RDXFileInfo);
            RDXLoaded = true;

            StatusBarText = RDXFileInfo.FullName;
        }

        public void UnloadRDX()
        {
            PRSFileInfo = null;
            RDXDocument = null;
            RDXFileInfo = null;
            RDXLoaded = false;

            StatusBarText = string.Empty;
        }

        public void LoadFileList(FileInfo file)
        {
            LoadFileList(new(file.Directory.FullName), $"*{file.Extension}");
        }

        public void LoadFileList(DirectoryInfo folder, string filter = "*")
        {
            RDXFolderInfo = folder;
            RDXFileList = RDXFolderInfo.GetFiles(filter).ToList();
        }
    }
}