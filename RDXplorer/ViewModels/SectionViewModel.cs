using RDXplorer.Models;
using RDXplorer.Models.RDX;
using System;
using System.Collections.Generic;

namespace RDXplorer.ViewModels
{
    public class SectionViewModel : PageViewModel
    {
        private List<Entry> _entries;
        public List<Entry> Entries
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
            if (AppViewModel.RDXDocument == null)
            {
                Entries = new List<Entry>();
                return;
            }

            HeaderModel header = AppViewModel.RDXDocument.Header;

            Entries = new List<Entry>
            {
                new Entry("Version", header.Version, false),
                new Entry("Scripts", header.Scripts, false),
                new Entry("Model", header.Model, false),
                new Entry("Motion", header.Motion, false),
                new Entry("Flags", header.Flags, false),
                new Entry("Texture", header.Texture, false),
                new Entry("Author", header.Author, false),
                new Entry("Camera", header.Camera),
                new Entry("Lighting", header.Lighting),
                new Entry("Enemy", header.Enemy),
                new Entry("Object", header.Object),
                new Entry("Unknown1", header.Unknown1),
                new Entry("Unknown2", header.Unknown2),
                new Entry("SCA", header.SCA),
                new Entry("AOT", header.AOT),
                new Entry("Unknown3", header.Unknown3),
                new Entry("Player", header.Player),
                new Entry("Unknown4", header.Unknown4),
                new Entry("Unknown5", header.Unknown5),
                new Entry("Unknown6", header.Unknown6),
                new Entry("Event", header.Event),
                new Entry("Text", header.Text),
                new Entry("Sysmes", header.Sysmes)
            };
        }

        public class Entry
        {
            public HeaderEntryModel Model { get; set; }

            public string Offset { get; set; }
            public string Label { get; set; }
            public string Value { get; set; }
            public string Count { get; set; }

            public Entry(string label, HeaderEntryModel model, bool hasCount = true)
            {
                Model = model;

                Offset = Model.Offset.ToString("X8");
                Label = label;
                Value = label == "Author" ? Model.Text : Model.Value.ToString("X8");
                Count = hasCount ? Model.Count.Value.ToString() : String.Empty;
            }
        }
    }
}