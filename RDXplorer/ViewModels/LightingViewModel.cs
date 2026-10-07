using RDXplorer.Enumerations;
using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class LightingViewModel : PageViewModel<LightingViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (LightingModel item in AppViewModel.RDXDocument.Lighting)
                Entries.Add(new(item));
        }
    }

    public class LightingViewModelEntry : PageViewModelEntry<LightingModel>
    {
        private string _lightingtype;
        public string LightingType => _lightingtype;

        public LightingViewModelEntry(LightingModel model) : base(model) =>
            Lookups.LightingTypes.TryGetValue((LightingTypeEnumeration)model.Fields.Type.Value, out _lightingtype);
    }
}