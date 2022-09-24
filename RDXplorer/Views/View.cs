using RDXplorer.Models;
using RDXplorer.ViewModels;
using System.ComponentModel;
using System.Windows.Controls;

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
    }
}