using RECVXFlagTool.ViewModels;
using System.Windows.Controls;

namespace RECVXFlagTool.Views
{
    public class View<T> : Page
        where T : AppViewModel, new()
    {
        public T Model { get; } = new T();

        public View() =>
            DataContext = Model;
    }
}