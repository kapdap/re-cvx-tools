using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class EventCameraBlockViewModel : PageViewModel<EventCameraBlockViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (EventCameraModel row in AppViewModel.RDXDocument.EventCamera)
                foreach (EventCameraBlockModel block in row.Blocks)
                    Entries.Add(new EventCameraBlockViewModelEntry(block));
        }
    }

    public class EventCameraBlockViewModelEntry(EventCameraBlockModel model) : PageViewModelEntry<EventCameraBlockModel>(model) { }
}
