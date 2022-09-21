using RDXplorer.Models;
using RDXplorer.Models.RDX;
using System.IO;

namespace RDXplorer.ViewModels
{
    public class AppViewModel : BaseNotifyModel
    {
        private FileInfo _rdxFileInfo;
        public FileInfo RDXPathInfo
        {
            get
            {
                return _rdxFileInfo;
            }

            private set
            {
                _rdxFileInfo = value;
                OnPropertyChanged();
            }
        }

        private DocumentModel _rdxDocument;
        public DocumentModel RDXDocument
        {
            get
            {
                return _rdxDocument;
            }

            private set
            {
                _rdxDocument = value;
                OnPropertyChanged();
            }
        }

        public bool _rdxLoaded;
        public bool RDXLoaded
        {
            get
            {
                return _rdxLoaded;
            }

            private set
            {
                _rdxLoaded = value;
                OnPropertyChanged();
            }
        }

        public void LoadRDX(FileInfo file)
        {
            RDXPathInfo = file;
            RDXDocument = new DocumentModel(RDXPathInfo);
            RDXLoaded = true;
        }

        public void UnloadRDX()
        {
            RDXLoaded = false;
            RDXDocument = null;
            RDXPathInfo = null;
        }
    }
}