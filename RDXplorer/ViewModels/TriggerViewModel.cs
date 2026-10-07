using RDXplorer.Enumerations;
using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class TriggerViewModel : PageViewModel<TriggerViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (TriggerModel item in AppViewModel.RDXDocument.Trigger)
                Entries.Add(new(item));
        }
    }

    public class TriggerViewModelEntry : PageViewModelEntry<TriggerModel>
    {
        private string _triggertype;
        public string TriggerType => _triggertype;

        public TriggerViewModelEntry(TriggerModel model) : base(model) =>
            Lookups.TriggerTypes.TryGetValue((TriggerTypeEnumeration)model.Fields.Type.Value, out _triggertype);
    }
}