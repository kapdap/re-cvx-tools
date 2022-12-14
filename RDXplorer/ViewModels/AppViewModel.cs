using RDXplorer.Models;
using RDXplorer.Models.RDX;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RDXplorer.ViewModels
{
    public class AppViewModel : BaseNotifyModel
    {
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

        public void LoadRDX(FileInfo file)
        {
            RDXFileInfo = file;
            RDXDocument = new(RDXFileInfo);
            RDXLoaded = true;

            LoadFileList();
        }

        public void UnloadRDX()
        {
            RDXDocument = null;
            RDXFileInfo = null;
            RDXLoaded = false;
        }

        private void LoadFileList()
        {
            if (RDXFileList == null || (RDXFolderInfo != null && RDXFolderInfo.FullName != RDXFileInfo.Directory.FullName))
            {
                RDXFolderInfo = new(RDXFileInfo.Directory.FullName);
                RDXFileList = RDXFolderInfo.GetFiles("*.rdx").ToList();
            }
        }
    }
}