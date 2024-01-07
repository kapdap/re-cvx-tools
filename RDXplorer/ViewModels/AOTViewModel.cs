using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class AOTViewModel : PageViewModel<AOTViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (AOTModel item in AppViewModel.RDXDocument.AOT)
                Entries.Add(new(item));
        }
    }

    public class AOTViewModelEntry
    {
        public AOTModel Model { get; set; }

        public AOTViewModelEntry(AOTModel model)
        {
            Model = model;
        }
    }
}