using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class CameraHeaderViewModel : PageViewModel<CameraHeaderViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (CameraHeaderModel item in AppViewModel.RDXDocument.Camera)
                Entries.Add(new CameraHeaderViewModelEntry(item));
        }
    }

    public class CameraHeaderViewModelEntry(CameraHeaderModel model) : PageViewModelEntry<CameraHeaderModel>(model) { }
}