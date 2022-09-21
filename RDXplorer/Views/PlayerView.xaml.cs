using RDXplorer.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace RDXplorer.Views
{
    public partial class PlayerView : View<PlayerViewModel>
    {
        public PlayerView()
        {
            InitializeComponent();
            LoadModel();

            AppViewModel.PropertyChanged += UpdateOnDocumentChange;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AppViewModel.RDXDocument == null)
                return;

            DataGrid grid = (DataGrid)sender;

            if (grid.CurrentColumn == null)
                return;

            string column = grid.CurrentColumn.Header.ToString().Trim();

            PlayerViewModel.Entry entry = (PlayerViewModel.Entry)grid.SelectedItem;

            Program.Windows.HexEditor.ShowFile(AppViewModel.RDXDocument.PathInfo);
            Program.Windows.HexEditor.SetPosition(entry.Model.Offset);
        }
    }
}