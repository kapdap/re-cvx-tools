using RDXplorer.Formats.RDX;
using RDXplorer.Models.RDX;
using RDXplorer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RDXplorer.Views
{
    public partial class ScriptView : View<ScriptViewModel, ScriptViewModelEntry>
    {
        public ScriptView()
        {
            InitializeComponent();
            LoadModel();

            AppViewModel.PropertyChanged += UpdateOnDocumentChange;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            if (GetDataGridColumnName(grid) != "View")
                OpenHexEditorFromDataGrid<ScriptViewModelEntry, ScriptModel>(grid);
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DataGrid grid = Utilities.FindParent<DataGrid>(button);
            ScriptViewModelEntry entry = (ScriptViewModelEntry)button.DataContext;

            if (grid != null && entry != null)
            {
                grid.SelectedItem = entry;

                Program.Windows.Scripting.DecompiledText.Text = new Scripting().Decompile(entry.Model.Fields.Data.Data);
                Program.Windows.Scripting.DecodedText.Text = new Scripting().Decode(entry.Model.Fields.Data.Data);
                Program.Windows.Scripting.Show();
            }
        }
    }
}