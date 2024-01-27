using RDXplorer.Extensions;
using RDXplorer.Models;
using RDXplorer.Models.RDX;
using RDXplorer.ViewModels;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace RDXplorer.Views
{
    public class View<T, TEntry> : Page
        where T : PageViewModel<TEntry>, new()
    {
        public T Model;
        public AppViewModel AppViewModel;

        public void LoadModel()
        {
            AppViewModel = Program.Models.AppView;

            Model = new T();
            DataContext = Model;
        }

        public void OpenHexEditorFromDataGrid<TViewEntry, TModel>(DataGrid grid)
            where TViewEntry : IPageViewModelEntry<TModel>
            where TModel : IBaseModel
        {
            string binding = GetDataGridColumnBinding(grid);

            if (string.IsNullOrEmpty(binding))
                return;

            TViewEntry entry = (TViewEntry)grid.SelectedItem;

            IntPtr offset = entry.Model.Position;
            long length = entry.Model.Size != 0 ? entry.Model.Size : 4;

            try
            {
                IDataEntryModel model = (IDataEntryModel)entry.GetPropertyValue(binding);

                offset = model.Position;
                length = model.Size;
            }
            catch { }

            Program.Windows.HexEditor.ShowFile(AppViewModel.RDXDocument.PathInfo);
            Program.Windows.HexEditor.SetPosition(offset, length);
        }

        public void UpdateOnDocumentChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "RDXDocument")
                return;

            Model.LoadData();
        }

        protected string GetDataGridColumnName(DataGrid grid)
        {
            if (AppViewModel.RDXDocument == null || grid.CurrentColumn == null)
                return string.Empty;
            return grid.CurrentColumn.Header.ToString().Trim();
        }

        protected string GetDataGridColumnBinding(DataGrid grid)
        {
            if (AppViewModel.RDXDocument == null || grid.CurrentColumn == null)
                return string.Empty;

            return ((Binding)((DataGridBoundColumn)grid.CurrentColumn).Binding)?.Path.Path.RemoveEnd(".Value").RemoveEnd(".Text") ?? string.Empty;
        }
    }
}