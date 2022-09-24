using RDXplorer.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace RDXplorer.Views
{
    public partial class ItemView : View<ItemViewModel, ItemViewModelEntry>
    {
        public ItemView()
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

            ItemViewModelEntry entry = (ItemViewModelEntry)grid.SelectedItem;

            Program.Windows.HexEditor.ShowFile(AppViewModel.RDXDocument.PathInfo);
            Program.Windows.HexEditor.SetPosition((long)entry.Model.Offset);
        }
    }
}