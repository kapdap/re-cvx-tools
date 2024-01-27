using RDXplorer.Models.RDX;
using RDXplorer.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace RDXplorer.Views
{
    public partial class TriggerView : View<TriggerViewModel, TriggerViewModelEntry>
    {
        public TriggerView()
        {
            InitializeComponent();
            LoadModel();

            AppViewModel.PropertyChanged += UpdateOnDocumentChange;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) =>
            OpenHexEditorFromDataGrid<TriggerViewModelEntry, TriggerModel>((DataGrid)sender);
    }
}