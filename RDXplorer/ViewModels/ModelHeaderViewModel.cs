using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class ModelHeaderViewModel : PageViewModel<ModelHeaderViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (ModelHeaderModel item in AppViewModel.RDXDocument.Model)
                Entries.Add(new ModelHeaderViewModelEntry(item));
        }
    }

    public class ModelHeaderViewModelEntry
    {
        public ModelHeaderModel Model { get; set; }
        public ModelEntryModel Entry { get; set; }

        public ModelHeaderViewModelEntry(ModelHeaderModel model)
        {
            Model = model;
            Entry = model.Entry.Count > 0 ? model.Entry[0] : new();
        }
    }
}