using RDXplorer.Extensions;
using RDXplorer.Models.RDX;
using RDXplorer.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace RDXplorer.Views
{
    public partial class ModelBlockView : View<ModelBlockViewModel, ModelBlockViewModelEntry>
    {
        public ModelBlockView()
        {
            InitializeComponent();
            LoadModel();

            AppViewModel.PropertyChanged += UpdateOnDocumentChange;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            string binding = GetDataGridColumnBinding(grid);

            if (string.IsNullOrEmpty(binding))
                return;

            ModelBlockViewModelEntry entry = (ModelBlockViewModelEntry)grid.SelectedItem;

            IntPtr offset = entry.Model.Position;
            long length = entry.Model.Size != 0 ? entry.Model.Size : 4;

            try
            {
                IDataEntryModel model = (IDataEntryModel)entry.GetPropertyValue(binding);

                offset = model.IsPointer ? (int)Utilities.GetValueType(model.Data, typeof(int)) : model.Position;
                length = model.Size;
            }
            catch { }

            Program.Windows.HexEditor.ShowFile(AppViewModel.RDXDocument.PathInfo);
            Program.Windows.HexEditor.SetPosition(offset, length);
        }
    }
}