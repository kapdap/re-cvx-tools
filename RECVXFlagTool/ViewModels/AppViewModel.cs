using RECVXFlagTool.Models.Base;

namespace RECVXFlagTool.ViewModels
{
    public class AppViewModel : BaseNotifyModel
    {
        public GameMemory Memory { get; } = Program.MemoryScanner.Memory;

        public string _statusBar;
        public string StatusBar
        {
            set => SetField(ref _statusBar, value);
            get => _statusBar ?? "Memory scanner is disabled";
        }
    }
}