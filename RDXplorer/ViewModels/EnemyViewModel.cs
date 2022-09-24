using RDXplorer.Enumerations;
using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class EnemyViewModel : PageViewModel<EnemyViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (EnemyModel item in AppViewModel.RDXDocument.Enemy)
                Entries.Add(new(item));
        }
    }

    public class EnemyViewModelEntry
    {
        public EnemyModel Model { get; private set; }

        private string _name;
        public string Name
        { 
            get => _name;
        }

        public EnemyViewModelEntry(EnemyModel model)
        {
            Model = model;

            Lookups.Enemies.TryGetValue((EnemyEnumeration)model.Type.Value, out _name);
        }
    }
}