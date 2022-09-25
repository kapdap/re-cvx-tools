using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class DoorViewModel : PageViewModel<DoorViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (DoorModel item in AppViewModel.RDXDocument.Door)
                Entries.Add(new(item));
        }
    }

    public class DoorViewModelEntry
    {
        public DoorModel Model { get; set; }

        public DoorViewModelEntry(DoorModel model)
        {
            Model = model;
        }
    }
}