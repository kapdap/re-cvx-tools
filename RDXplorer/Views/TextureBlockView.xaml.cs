using RDXplorer.Models.RDX;
using RDXplorer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RDXplorer.Views
{
    public partial class TextureBlockView : View<TextureBlockViewModel, TextureBlockViewModelEntry>
    {
        public TextureBlockView()
        {
            InitializeComponent();
            LoadModel();

            AppViewModel.PropertyChanged += UpdateOnDocumentChange;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            if (GetDataGridColumnName(grid) != "View")
                OpenHexEditorFromDataGrid<TextureBlockViewModelEntry, TextureBlockModel>(grid);
        }
        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DataGrid grid = Utilities.FindParent<DataGrid>(button);
            TextureBlockViewModelEntry entry = (TextureBlockViewModelEntry)button.DataContext;
            
            if (grid != null && entry != null)
            {
                grid.SelectedItem = entry;

                if (entry.Model.Fields.Type.Text == "TIM2")
                {
                    Program.Windows.Bitmap.DataContext = grid;
                    Program.Windows.Bitmap.Render();
                    Program.Windows.Bitmap.Show();
                }
            }
        }
    }
}