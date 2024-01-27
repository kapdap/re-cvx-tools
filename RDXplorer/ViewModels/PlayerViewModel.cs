using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class PlayerViewModel : PageViewModel<PlayerViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (PlayerModel item in AppViewModel.RDXDocument.Player)
                Entries.Add(new(item));
        }
    }

    public class PlayerViewModelEntry(PlayerModel model) : PageViewModelEntry<PlayerModel>(model) { }
}