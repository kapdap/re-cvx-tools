using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class MotionBlockViewModel : PageViewModel<MotionBlockViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (MotionTableModel row in AppViewModel.RDXDocument.Motion)
                foreach (MotionBlockModel block in row.Blocks)
                    Entries.Add(new MotionBlockViewModelEntry(block));
        }
    }

    public class MotionBlockViewModelEntry
    {
        public MotionBlockModel Model { get; set; }

        public MotionBlockViewModelEntry(MotionBlockModel model)
        {
            Model = model;
        }
    }
}