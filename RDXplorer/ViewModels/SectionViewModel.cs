using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class SectionViewModel : PageViewModel<SectionViewModelEntry>
    {
        public override void LoadData()
        {
            if (AppViewModel.RDXDocument == null)
            {
                Entries = new();
                return;
            }

            HeaderModel header = AppViewModel.RDXDocument.Header;

            Entries = new()
            {
                new SectionViewModelEntry(header.Version),
                new SectionViewModelEntry(header.Scripts),
                new SectionViewModelEntry(header.Model),
                new SectionViewModelEntry(header.Motion),
                new SectionViewModelEntry(header.Flags),
                new SectionViewModelEntry(header.Texture),
                new SectionViewModelEntry(header.Author),
                new SectionViewModelEntry(header.Camera),
                new SectionViewModelEntry(header.Lighting),
                new SectionViewModelEntry(header.Enemy),
                new SectionViewModelEntry(header.Object),
                new SectionViewModelEntry(header.Unknown1),
                new SectionViewModelEntry(header.Unknown2),
                new SectionViewModelEntry(header.SCA),
                new SectionViewModelEntry(header.AOT),
                new SectionViewModelEntry(header.Unknown3),
                new SectionViewModelEntry(header.Player),
                new SectionViewModelEntry(header.Unknown4),
                new SectionViewModelEntry(header.Unknown5),
                new SectionViewModelEntry(header.Unknown6),
                new SectionViewModelEntry(header.Event),
                new SectionViewModelEntry(header.Text),
                new SectionViewModelEntry(header.Sysmes)
            };
        }
    }

    public class SectionViewModelEntry
    {
        public HeaderEntryModel Model { get; set; }

        public string Value { get; set; }
        public string Count { get; set; }

        public SectionViewModelEntry(HeaderEntryModel model)
        {
            Model = model;

            Value = Model.IsText ? Model.Text : Model.Value.ToString("X8");
            Count = Model.HasCount ? Model.Count.Value.ToString() : string.Empty;
        }
    }
}