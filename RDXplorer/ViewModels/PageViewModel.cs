using RDXplorer.ViewModels;
using RDXplorer.Views;
using System.ComponentModel;

namespace RDXplorer.Models
{
    public abstract class PageViewModel : BaseNotifyModel
    {
        public AppViewModel AppViewModel { get; set; }

        public PageViewModel()
        {
            AppViewModel = Program.Models.AppView;
            LoadData();
        }

        public abstract void LoadData();

        public void UpdateData(object sender, PropertyChangedEventArgs e) =>
            LoadData();
    }
}