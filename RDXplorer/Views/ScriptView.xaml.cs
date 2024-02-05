using RDXplorer.Formats.RDX;
using RDXplorer.Models.RDX;
using RDXplorer.ViewModels;
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

            string binding = GetDataGridColumnBinding(grid);

            if (binding != "Model.Fields.Data")
                OpenHexEditorFromDataGrid<ScriptViewModelEntry, ScriptModel>(grid);

            ScriptViewModelEntry entry = (ScriptViewModelEntry)grid.SelectedItem;

            Program.Windows.Scripting.DecompiledText.Text = new Scripting().Decompile(entry.Model.Fields.Data.Data);
            Program.Windows.Scripting.DecodedText.Text = new Scripting().Decode(entry.Model.Fields.Data.Data);
            Program.Windows.Scripting.Show();
        }
    }
}