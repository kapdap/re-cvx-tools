using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class BoundaryViewModel : PageViewModel<BoundaryViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (BoundaryModel item in AppViewModel.RDXDocument.Boundary)
                Entries.Add(new(item));
        }
    }

    public class BoundaryViewModelEntry
    {
        public BoundaryModel Model { get; set; }

        public BoundaryViewModelEntry(BoundaryModel model)
        {
            Model = model;
        }
    }
}