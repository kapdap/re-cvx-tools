using RDXplorer.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace RDXplorer.Views
{
    public partial class EnemyView : View<EnemyViewModel, EnemyViewModelEntry>
    {
        public EnemyView()
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

            EnemyViewModelEntry entry = (EnemyViewModelEntry)grid.SelectedItem;

            Program.Windows.HexEditor.ShowFile(AppViewModel.RDXDocument.PathInfo);
            Program.Windows.HexEditor.SetPosition((long)entry.Model.Offset);
        }
    }
}