using RDXplorer.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace RDXplorer.Views
{
    public partial class SectionView : View<SectionViewModel, SectionViewModelEntry>
    {
        public SectionView()
        {
            InitializeComponent();
            LoadModel();

            AppViewModel.PropertyChanged += UpdateOnDocumentChange;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            string column = GetDataGridColumnName(grid);

            if (string.IsNullOrEmpty(column) || column == "Label")
                return;

            SectionViewModelEntry entry = (SectionViewModelEntry)grid.SelectedItem;

            Program.Windows.HexEditor.ShowFile(AppViewModel.RDXDocument.PathInfo);

            if (column == "Value" && entry.Model.IsPointer)
                Program.Windows.HexEditor.SetPosition(entry.Model.Value);
            else if (column == "Count" && entry.Model.HasCount)
                Program.Windows.HexEditor.SetPosition((long)entry.Model.Count.Offset, entry.Model.Count.Size);
            else
                Program.Windows.HexEditor.SetPosition((long)entry.Model.Offset, entry.Model.Size);
        }
    }
}