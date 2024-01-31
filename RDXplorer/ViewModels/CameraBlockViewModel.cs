using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class CameraBlockViewModel : PageViewModel<CameraBlockViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (CameraHeaderModel row in AppViewModel.RDXDocument.Camera)
                foreach (CameraBlockModel block in row.Blocks)
                    Entries.Add(new CameraBlockViewModelEntry(block));
        }
    }

    public class CameraBlockViewModelEntry(CameraBlockModel model) : PageViewModelEntry<CameraBlockModel>(model) { }
}