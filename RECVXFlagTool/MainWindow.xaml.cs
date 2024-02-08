using RECVXFlagTool.ViewModels;
using System.ComponentModel;
using System.Windows;

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

        private void FileLoadFlags_Click(object sender, RoutedEventArgs e) => 
            Program.LoadFlagNames(Program.SelectFile());

        private void FileSaveFlags_Click(object sender, RoutedEventArgs e) => 
            Program.SaveFlagNames();

        private void FileSaveFlagsAs_Click(object sender, RoutedEventArgs e) => 
            Program.SaveFlagNames(Program.SaveFile());

        private void FileExitMenu_Click(object sender, RoutedEventArgs e) => 
            Close();

        private void ClearLogMenu_Click(object sender, RoutedEventArgs e) => 
            Program.MemoryScanner.Memory.FlagLog.Clear();

        private void HelpAboutMenu_Click(object sender, RoutedEventArgs e) => 
            Program.Windows.About.Show();

        private void Window_Closing(object sender, CancelEventArgs e) => 
            Program.CloseApp();
    }
}