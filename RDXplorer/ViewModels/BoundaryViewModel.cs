using RDXplorer.Enumerations;
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

    public class BoundaryViewModelEntry : PageViewModelEntry<BoundaryModel>
    {
        private string _boundarytype;
        public string BoundaryType => _boundarytype;

        public BoundaryViewModelEntry(BoundaryModel model) : base(model) =>
            Lookups.BoundaryTypes.TryGetValue((BoundaryTypeEnumeration)model.Fields.Type.Value, out _boundarytype);
    }
}