using RDXplorer.Enumerations;
using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class ItemViewModel : PageViewModel<ItemViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (ItemModel item in AppViewModel.RDXDocument.Item)
                Entries.Add(new(item));
        }
    }

    public class ItemViewModelEntry
    {
        public ItemModel Model { get; set; }

        public string _name;
        public string Name
        {
            get => _name;
        }

        public ItemViewModelEntry(ItemModel model)
        {
            Model = model;

            Lookups.Items.TryGetValue((ItemEnumeration)model.Fields.Type.Value, out _name);
        }
    }
}