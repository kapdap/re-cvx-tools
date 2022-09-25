using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class EffectViewModel : PageViewModel<EffectViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (EffectModel item in AppViewModel.RDXDocument.Effect)
                Entries.Add(new(item));
        }
    }

    public class EffectViewModelEntry
    {
        public EffectModel Model { get; set; }

        public EffectViewModelEntry(EffectModel model)
        {
            Model = model;
        }
    }
}