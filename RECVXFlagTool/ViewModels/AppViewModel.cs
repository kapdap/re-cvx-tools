using RECVXFlagTool.Models.Base;
using System.Collections.Generic;
using System.IO;

namespace RECVXFlagTool.ViewModels
{
    public class AppViewModel : BaseNotifyModel
    {
        public GameMemory Memory { get; } = Program.MemoryScanner.Memory;

        private Dictionary<string, string> _flagNames = new();
        public Dictionary<string, string> FlagNames
        {
            get => _flagNames;
            set => SetField(ref _flagNames, value);
        }

        private FileInfo _flagFile;
        public FileInfo FlagFile
        {
            get => _flagFile;
            set => SetField(ref _flagFile, value);
        }

        public string _statusBar;
        public string StatusBar
        {
            set => SetField(ref _statusBar, value);
            get => _statusBar ?? "Memory scanner is disabled";
        }
    }
}