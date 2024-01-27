using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class CameraViewModel : PageViewModel<CameraViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (CameraModel item in AppViewModel.RDXDocument.Camera)
                Entries.Add(new CameraViewModelEntry(item));
        }
    }

    public class CameraViewModelEntry(CameraModel model) : PageViewModelEntry<CameraModel>(model) { }
}