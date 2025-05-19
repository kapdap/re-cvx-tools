using RDXplorer.Formats.TIM2;
using RDXplorer.Models.RDX;
using RDXplorer.ViewModels;
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

            string binding = GetDataGridColumnBinding(grid);

            if (binding == "Model.Fields.Data")
            {
                TextureBlockViewModelEntry entry = (TextureBlockViewModelEntry)grid.SelectedItem;

                if (entry.Model.Fields.Type.Text == "TIM2")
                {
                    Program.Windows.Bitmap.BitmapImage.Source = Tim2Converter.Decode(entry.Model.Fields.Data.Data);
                    Program.Windows.Bitmap.Show();

                    return;
                }
            }

            OpenHexEditorFromDataGrid<TextureBlockViewModelEntry, TextureBlockModel>(grid);
        }
    }
}