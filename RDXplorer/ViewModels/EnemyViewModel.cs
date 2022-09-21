using RDXplorer.Models;
using RDXplorer.Models.RDX;
using System.Collections.ObjectModel;

namespace RDXplorer.ViewModels
{
    public class EnemyViewModel : PageViewModel
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

            foreach (EnemyModel item in AppViewModel.RDXDocument.Enemy)
                Entries.Add(new Entry(item));
        }

        public class Entry
        {
            public EnemyModel Model { get; private set; }

            public string Name { get; set; }
            public string Offset { get; set; }

            public string Unknown0 { get; set; }
            public string Type { get; set; }
            public string Unknown1 { get; set; }
            public string Unknown2 { get; set; }
            public string Unknown3 { get; set; }
            public string Slot { get; set; }
            public string Unknown4 { get; set; }
            public string X { get; set; }
            public string Y { get; set; }
            public string Z { get; set; }
            public string Unknown5 { get; set; }
            public string Rotation { get; set; }
            public string Unknown6 { get; set; }
            public string Unknown7 { get; set; }

            public Entry(EnemyModel model)
            {
                Model = model;
                Name = "Unknown";
                Offset = Model.Offset.ToString("X8");
                Unknown0 = Model.Unknown0.Value.ToString("X8");
                Type = Model.Type.Value.ToString();
                Unknown1 = Model.Unknown1.Value.ToString("X4");
                Unknown2 = Model.Unknown2.Value.ToString("X2");
                Unknown3 = Model.Unknown3.Value.ToString("X2");
                Slot = Model.Slot.Value.ToString();
                Unknown4 = Model.Unknown4.Value.ToString("X2");
                X = Model.X.Value.ToString();
                Y = Model.Y.Value.ToString();
                Z = Model.Z.Value.ToString();
                Unknown5 = Model.Unknown5.Value.ToString("X8");
                Rotation = Model.Rotation.Value.ToString();
                Unknown6 = Model.Unknown6.Value.ToString("X4");
                Unknown7 = Model.Unknown7.Value.ToString("X8");
            }
        }
    }
}