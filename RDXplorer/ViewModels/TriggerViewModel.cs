using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class TriggerViewModel : PageViewModel<TriggerViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (TriggerModel item in AppViewModel.RDXDocument.Trigger)
                Entries.Add(new(item));
        }
    }

    public class TriggerViewModelEntry(TriggerModel model) : PageViewModelEntry<TriggerModel>(model) { }
}