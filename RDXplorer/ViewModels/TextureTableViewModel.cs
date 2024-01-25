using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class TextureTableViewModel : PageViewModel<TextureTableViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (TextureTableModel item in AppViewModel.RDXDocument.Texture)
                Entries.Add(new TextureTableViewModelEntry(item));
        }
    }

    public class TextureTableViewModelEntry
    {
        public TextureTableModel Model { get; set; }

        public TextureTableViewModelEntry(TextureTableModel model)
        {
            Model = model;
        }
    }
}