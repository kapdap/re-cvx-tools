using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class EventViewModel : PageViewModel<EventViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (EventModel item in AppViewModel.RDXDocument.Event)
                Entries.Add(new(item));
        }
    }

    public class EventViewModelEntry(EventModel model) : PageViewModelEntry<EventModel>(model) { }
}