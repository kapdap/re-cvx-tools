using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class TextureViewModel : PageViewModel<TextureViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (TextureTableModel row in AppViewModel.RDXDocument.Texture)
                foreach (TextureBlockModel block in row.Blocks)
                    Entries.Add(new TextureViewModelEntry(block));
        }
    }

    public class TextureViewModelEntry
    {
        public TextureBlockModel Model { get; set; }

        public TextureViewModelEntry(TextureBlockModel model)
        {
            Model = model;
        }
    }
}