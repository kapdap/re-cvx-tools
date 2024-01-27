using RDXplorer.Enumerations;
using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class ActorViewModel : PageViewModel<ActorViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (ActorModel item in AppViewModel.RDXDocument.Actor)
                Entries.Add(new(item));
        }
    }

    public class ActorViewModelEntry : PageViewModelEntry<ActorModel>
    {
        private string _name;
        public string Name => _name;

        public ActorViewModelEntry(ActorModel model) : base(model) =>
            Lookups.Actors.TryGetValue((ActorEnumeration)model.Fields.Type.Value, out _name);
    }
}