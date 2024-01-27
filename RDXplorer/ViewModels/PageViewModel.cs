using RDXplorer.Models.RDX;
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

    public interface IPageViewModelEntry<T>
    {
        T Model { get; set; }
    }

    public abstract class PageViewModelEntry<T> : IPageViewModelEntry<T>
    {
        public virtual T Model { get; set; }

        public PageViewModelEntry(T model)
        {
            Model = model;
        }
    }
}