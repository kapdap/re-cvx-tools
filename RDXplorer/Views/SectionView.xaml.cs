using RDXplorer.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace RDXplorer.Views
{
    public partial class SectionView : View<SectionViewModel>
    {
        public SectionView()
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

            if (column == "Label")
                return;

            SectionViewModel.Entry entry = (SectionViewModel.Entry)grid.SelectedItem;

            Program.Windows.HexEditor.ShowFile(AppViewModel.RDXDocument.PathInfo);

            if (column == "Count")
                Program.Windows.HexEditor.SetPosition(entry.Model.Count.Offset, entry.Model.Count.Size);
            else if (column == "Offset" || !entry.Model.IsPointer)
                Program.Windows.HexEditor.SetPosition(entry.Model.Offset, entry.Model.Size);
            else if (column == "Value")
                Program.Windows.HexEditor.SetPosition(entry.Model.Value);
        }
    }
}