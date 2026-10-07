using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class EventLightViewModel : PageViewModel<EventLightViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (EventLightModel item in AppViewModel.RDXDocument.EventLight)
                Entries.Add(new EventLightViewModelEntry(item));
        }
    }

    public class EventLightViewModelEntry(EventLightModel model) : PageViewModelEntry<EventLightModel>(model) { }
}
