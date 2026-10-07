using RDXplorer.Models;
using RDXplorer.Models.RDX;

namespace RDXplorer.ViewModels
{
    public class RouteTableViewModel : PageViewModel<RouteTableViewModelEntry>
    {
        public override void LoadData()
        {
            Entries = new();

            if (AppViewModel.RDXDocument == null)
                return;

            int count = (int)AppViewModel.RDXDocument.Header.Route.Count.Value;

            if (count == 0)
                return;

            for (int i = 0; i < AppViewModel.RDXDocument.RouteTable.Count; i++)
                Entries.Add(new(AppViewModel.RDXDocument.RouteTable[i], i % count, i / count));
        }
    }

    public class RouteTableViewModelEntry : PageViewModelEntry<RouteTableModel>
    {
        public int From { get; }
        public int To { get; }
        public byte Next => Model.Fields.Value.Value;

        public RouteTableViewModelEntry(RouteTableModel model, int from, int to) : base(model)
        {
            From = from;
            To = to;
        }
    }
}
