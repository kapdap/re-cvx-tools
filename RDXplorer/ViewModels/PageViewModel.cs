using RDXplorer.ViewModels;
using RDXplorer.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RDXplorer.Models
{
    public abstract class PageViewModel<T> : BaseNotifyModel
    {
        private ObservableCollection<T> _entries;
        public ObservableCollection<T> Entries
        {
            get => _entries;
            protected set => SetField(ref _entries, value);
        }

        public AppViewModel AppViewModel { get; private set; }

        public PageViewModel()
        {
            AppViewModel = Program.Models.AppView;
        }

        public abstract void LoadData();

        public void UpdateData(object sender, PropertyChangedEventArgs e) =>
            LoadData();
    }
}