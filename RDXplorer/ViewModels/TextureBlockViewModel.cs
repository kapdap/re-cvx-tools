using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class TextureBlockViewModel : PageViewModel<TextureBlockViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (TextureTableModel row in AppViewModel.RDXDocument.Texture)
                foreach (TextureBlockModel block in row.Blocks)
                    Entries.Add(new TextureBlockViewModelEntry(block));
        }
    }

    public class TextureBlockViewModelEntry(TextureBlockModel model) : PageViewModelEntry<TextureBlockModel>(model) { }
}