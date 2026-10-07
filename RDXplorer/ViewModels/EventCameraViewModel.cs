using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class EventCameraViewModel : PageViewModel<EventCameraViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (EventCameraModel item in AppViewModel.RDXDocument.EventCamera)
                Entries.Add(new EventCameraViewModelEntry(item));
        }
    }

    public class EventCameraViewModelEntry(EventCameraModel model) : PageViewModelEntry<EventCameraModel>(model) { }
}
