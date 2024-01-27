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

    public class BoundaryViewModelEntry(BoundaryModel model) : PageViewModelEntry<BoundaryModel>(model) { }
}