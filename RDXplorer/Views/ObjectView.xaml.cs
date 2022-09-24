using RDXplorer.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace RDXplorer.Views
{
    public partial class ObjectView : View<ObjectViewModel, ObjectViewModelEntry>
    {
        public ObjectView()
        {
            InitializeComponent();
            LoadModel();

            AppViewModel.PropertyChanged += UpdateOnDocumentChange;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            string column = GetDataGridColumnName(grid);

            if (string.IsNullOrEmpty(column))
                return;

            ObjectViewModelEntry entry = (ObjectViewModelEntry)grid.SelectedItem;

            Program.Windows.HexEditor.ShowFile(AppViewModel.RDXDocument.PathInfo);
            Program.Windows.HexEditor.SetPosition((long)entry.Model.Offset);
        }
    }
}