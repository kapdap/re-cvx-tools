using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class RouteViewModel : PageViewModel<RouteViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            foreach (RouteModel item in AppViewModel.RDXDocument.Route)
                Entries.Add(new(item));
        }
    }

    public class RouteViewModelEntry(RouteModel model) : PageViewModelEntry<RouteModel>(model) { }
}