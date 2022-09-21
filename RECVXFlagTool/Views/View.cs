using System.Windows.Controls;
using RECVXFlagTool.ViewModels;

namespace RECVXFlagTool.Views
{
	public class View<T> : Page
		where T : AppViewModel, new()
	{
		public T Model { get; } = new T();

		public View()
		{
			DataContext = Model;
		}
	}
}