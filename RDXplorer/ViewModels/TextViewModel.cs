using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class TextViewModel : PageViewModel<TextViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (TextModel item in AppViewModel.RDXDocument.Text)
                Entries.Add(new TextViewModelEntry(item));
        }
    }

    public class TextViewModelEntry(TextModel model) : PageViewModelEntry<TextModel>(model) { }
}