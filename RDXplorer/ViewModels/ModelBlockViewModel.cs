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
                foreach (ModelBlockModel section in row.Blocks)
                    Entries.Add(new ModelBlockViewModelEntry(section));
        }
    }

    public class ModelBlockViewModelEntry
    {
        public ModelBlockModel Model { get; set; }

        public ModelBlockViewModelEntry(ModelBlockModel model)
        {
            Model = model;
        }
    }
}