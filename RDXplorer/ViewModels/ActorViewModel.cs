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

    public class ActorViewModelEntry
    {
        public ActorModel Model { get; private set; }

        private string _name;
        public string Name
        { 
            get => _name;
        }

        public ActorViewModelEntry(ActorModel model)
        {
            Model = model;

            Lookups.Enemies.TryGetValue((ActorEnumeration)model.Type.Value, out _name);
        }
    }
}