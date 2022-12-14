using System.ComponentModel;
using System.Windows;
using RECVXFlagTool.ViewModels;

namespace RECVXFlagTool
{
	public partial class MainWindow : Window
	{
		public AppViewModel AppViewModel { get; } = Program.Models.AppViewModel;

		public MainWindow()
		{
			InitializeComponent();
			DataContext = AppViewModel;
			Program.Initialize(this);
		}

		private void FileLoadFlags_Click(object sender, RoutedEventArgs e)
		{
			Program.LoadFlagNames();
		}

		private void FileSaveFlags_Click(object sender, RoutedEventArgs e)
		{
			Program.SaveFlagNames();
		}

		private void FileExitMenu_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void ClearLogMenu_Click(object sender, RoutedEventArgs e)
		{
			Program.MemoryScanner.Memory.FlagLog.Clear();
		}

		private void HelpAboutMenu_Click(object sender, RoutedEventArgs e)
		{
			Program.Windows.About.Show();
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			Program.CloseApp();
		}
	}
}