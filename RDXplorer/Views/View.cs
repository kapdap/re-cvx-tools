using RDXplorer.Models;
using RDXplorer.ViewModels;
using System.ComponentModel;
using System.Windows.Controls;

namespace RDXplorer.Views
{
    public class View<T> : Page
        where T : PageViewModel, new()
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

            if (AppViewModel.RDXDocument != null)
                AppViewModel.RDXDocument.PropertyChanged += Model.UpdateData;

            Model.LoadData();
        }
    }
}