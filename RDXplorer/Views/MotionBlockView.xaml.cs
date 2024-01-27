using RDXplorer.Models.RDX;
using RDXplorer.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace RDXplorer.Views
{
    public partial class MotionBlockView : View<MotionBlockViewModel, MotionBlockViewModelEntry>
    {
        public MotionBlockView()
        {
            InitializeComponent();
            LoadModel();

            AppViewModel.PropertyChanged += UpdateOnDocumentChange;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) =>
            OpenHexEditorFromDataGrid<MotionBlockViewModelEntry, MotionBlockModel>((DataGrid)sender);
    }
}