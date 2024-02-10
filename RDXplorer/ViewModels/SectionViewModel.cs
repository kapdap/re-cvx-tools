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
                new(header.Version),

                new(header.Tables),
                new(header.Model),
                new(header.Motion),
                new(header.Script),
                new(header.Texture),

                new(header.Author),

                new(header.Camera),
                new(header.Lighting),
                new(header.Enemy),
                new(header.Object),
                new(header.Item),
                new(header.Effect),
                new(header.Boundary),
                new(header.AOT),
                new(header.Trigger),
                new(header.Player),
                new(header.Event),
                new(header.Unknown1),
                new(header.Unknown2),
                new(header.Action),
                new(header.Text),
                new(header.Sysmes)
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