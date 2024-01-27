using RDXplorer.Models.RDX;
using RDXplorer.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace RDXplorer.Views
{
    public partial class MotionTableView : View<MotionTableViewModel, MotionTableViewModelEntry>
    {
        public MotionTableView()
        {
            InitializeComponent();
            LoadModel();

            AppViewModel.PropertyChanged += UpdateOnDocumentChange;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) =>
            OpenHexEditorFromDataGrid<MotionTableViewModelEntry, MotionTableModel>((DataGrid)sender);
    }
}