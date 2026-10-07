using RDXplorer.Models.RDX;
using RDXplorer.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace RDXplorer.Views
{
    public partial class EventCameraBlockView : View<EventCameraBlockViewModel, EventCameraBlockViewModelEntry>
    {
        public EventCameraBlockView()
        {
            InitializeComponent();
            LoadModel();

            AppViewModel.PropertyChanged += UpdateOnDocumentChange;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) =>
            OpenHexEditorFromDataGrid<EventCameraBlockViewModelEntry, EventCameraBlockModel>((DataGrid)sender);
    }
}
