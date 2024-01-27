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

    public class LightingViewModelEntry(LightingModel model) : PageViewModelEntry<LightingModel>(model) { }
}