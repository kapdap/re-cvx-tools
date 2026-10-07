using RDXplorer.Models.RDX;
using RDXplorer.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace RDXplorer.Views
{
    public partial class RouteView : View<RouteViewModel, RouteViewModelEntry>
    {
        public RouteView()
        {
            InitializeComponent();
            LoadModel();

            AppViewModel.PropertyChanged += UpdateOnDocumentChange;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) =>
            OpenHexEditorFromDataGrid<RouteViewModelEntry, RouteModel>((DataGrid)sender);
    }
}