using RDXplorer.Models;
using RDXplorer.Models.RDX;
using System.Collections.ObjectModel;

namespace RDXplorer.ViewModels
{
    public class PlayerViewModel : PageViewModel
    {
        private ObservableCollection<Entry> _entries;
        public ObservableCollection<Entry> Entries
        {
            get
            {
                return _entries;
            }

            private set
            {
                _entries = value;
                OnPropertyChanged();
            }
        }

        public override void LoadData()
        {
            Entries = new ObservableCollection<Entry>();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (PlayerModel item in AppViewModel.RDXDocument.Player)
                Entries.Add(new Entry(item));
        }

        public class Entry
        {
            public PlayerModel Model { get; set; }

            public string Offset { get; set; }
            public string X { get; set; }
            public string Y { get; set; }
            public string Z { get; set; }
            public string Rotation { get; set; }

            public Entry(PlayerModel model)
            {
                Model = model;
                Offset = Model.Offset.ToString("X8");
                X = Model.X.Value.ToString();
                Y = Model.Y.Value.ToString();
                Z = Model.Z.Value.ToString();
                Rotation = Model.Rotation.Value.ToString();
            }
        }
    }
}