using RDXplorer.ViewModels;
using RDXplorer.Views;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace RDXplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AppViewModel AppViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Program.Initialize(this);
        }

        private void FileOpenMenu_Click(object sender, RoutedEventArgs e) =>
            Program.OpenRDX();

        private void FileCloseMenu_Click(object sender, RoutedEventArgs e) =>
            Program.CloseRDX();

        private void FileExitMenu_Click(object sender, RoutedEventArgs e) =>
            Close();

        private void ToolsHexEditorMenu_Click(object sender, RoutedEventArgs e) =>
            Program.Windows.HexEditor.ShowFile(AppViewModel.RDXFileInfo);

        private void HelpAboutMenu_Click(object sender, RoutedEventArgs e) =>
            Program.Windows.About.Show();

        private void Window_Closing(object sender, CancelEventArgs e) =>
            Program.CloseApp();

        private void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
            Program.LoadRDX((FileInfo)((ComboBox)sender).SelectedItem);
    }
}