using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class BoundryViewModel : PageViewModel<BoundryViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (BoundryModel item in AppViewModel.RDXDocument.Boundry)
                Entries.Add(new(item));
        }
    }

    public class BoundryViewModelEntry
    {
        public BoundryModel Model { get; set; }

        public BoundryViewModelEntry(BoundryModel model)
        {
            Model = model;
        }
    }
}