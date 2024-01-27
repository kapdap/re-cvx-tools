using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class ModelBlockViewModel : PageViewModel<ModelBlockViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (ModelTableModel row in AppViewModel.RDXDocument.Model)
                foreach (ModelBlockModel block in row.Blocks)
                    Entries.Add(new ModelBlockViewModelEntry(block));
        }
    }

    public class ModelBlockViewModelEntry(ModelBlockModel model) : PageViewModelEntry<ModelBlockModel>(model) { }
}