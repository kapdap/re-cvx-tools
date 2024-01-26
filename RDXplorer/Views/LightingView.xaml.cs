using RDXplorer.Extensions;
using RDXplorer.Models.RDX;
using RDXplorer.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace RDXplorer.Views
{
    public partial class LightingView : View<LightingViewModel, LightingViewModelEntry>
    {
        public LightingView()
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

            LightingViewModelEntry entry = (LightingViewModelEntry)grid.SelectedItem;

            IntPtr offset = entry.Model.Offset;
            long length = 4;

            try
            {
                IDataEntryModel model = (IDataEntryModel)entry.GetPropertyValue(binding);

                offset = model.Offset;
                length = model.Size;
            }
            catch { }

            Program.Windows.HexEditor.ShowFile(AppViewModel.RDXDocument.PathInfo);
            Program.Windows.HexEditor.SetPosition(offset, length);
        }
    }
}