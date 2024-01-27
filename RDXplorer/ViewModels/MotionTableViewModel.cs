using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class MotionTableViewModel : PageViewModel<MotionTableViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (MotionTableModel item in AppViewModel.RDXDocument.Motion)
                Entries.Add(new MotionTableViewModelEntry(item));
        }
    }

    public class MotionTableViewModelEntry(MotionTableModel model) : PageViewModelEntry<MotionTableModel>(model) { }
}