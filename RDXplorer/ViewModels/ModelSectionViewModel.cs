using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class ModelSectionViewModel : PageViewModel<ModelSectionViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (ModelTableModel row in AppViewModel.RDXDocument.Model)
                foreach (ModelSectionModel section in row.Sections)
                    Entries.Add(new ModelSectionViewModelEntry(section));
        }
    }

    public class ModelSectionViewModelEntry
    {
        public ModelSectionModel Model { get; set; }

        public ModelSectionViewModelEntry(ModelSectionModel model)
        {
            Model = model;
        }
    }
}