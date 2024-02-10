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

    public class EnemyViewModelEntry : PageViewModelEntry<EnemyModel>
    {
        private string _name;
        public string Name => _name;

        public EnemyViewModelEntry(EnemyModel model) : base(model) =>
            Lookups.Enemys.TryGetValue((EnemyEnumeration)model.Fields.Type.Value, out _name);
    }
}