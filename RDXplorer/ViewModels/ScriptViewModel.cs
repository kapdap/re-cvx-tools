using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class ScriptViewModel : PageViewModel<ScriptViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (ScriptModel item in AppViewModel.RDXDocument.Script)
                Entries.Add(new ScriptViewModelEntry(item));
        }
    }

    public class ScriptViewModelEntry(ScriptModel model) : PageViewModelEntry<ScriptModel>(model) { }
}