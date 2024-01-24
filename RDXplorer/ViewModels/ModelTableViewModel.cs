using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class ModelTableViewModel : PageViewModel<ModelTableViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (ModelTableModel item in AppViewModel.RDXDocument.Model)
                Entries.Add(new ModelTableViewModelEntry(item));
        }
    }

    public class ModelTableViewModelEntry
    {
        public ModelTableModel Model { get; set; }

        public ModelTableViewModelEntry(ModelTableModel model)
        {
            Model = model;
        }
    }
}