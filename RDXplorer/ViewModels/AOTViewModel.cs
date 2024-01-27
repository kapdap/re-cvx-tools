using RDXplorer.Enumerations;
using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class AOTViewModel : PageViewModel<AOTViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (AOTModel item in AppViewModel.RDXDocument.AOT)
                Entries.Add(new(item));
        }
    }

    public class AOTViewModelEntry : PageViewModelEntry<AOTModel>
    {
        private string _aottype;
        public string AOTType
        {
            get => _aottype;
        }

        public AOTViewModelEntry(AOTModel model) : base(model) =>
            Lookups.AOTTypes.TryGetValue((AOTTypeEnumeration)model.Fields.Type.Value, out _aottype);
    }
}