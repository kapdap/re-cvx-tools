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

    public class AOTViewModelEntry
    {
        public AOTModel Model { get; set; }

        public string _aottype;
        public string AOTType
        {
            get => _aottype;
        }

        public AOTViewModelEntry(AOTModel model)
        {
            Model = model;

            Lookups.AOTTypes.TryGetValue((AOTTypeEnumeration)model.Type.Value, out _aottype);
        }
    }
}