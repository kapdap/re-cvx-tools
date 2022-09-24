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
                Entries.Add(new PlayerViewModelEntry(item));
        }
    }

    public class PlayerViewModelEntry
    {
        public PlayerModel Model { get; set; }

        public PlayerViewModelEntry(PlayerModel model)
        {
            Model = model;
        }
    }
}