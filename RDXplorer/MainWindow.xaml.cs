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
            Program.Windows.HexEditor.ShowFile(AppViewModel?.RDXDocument?.RDXFileInfo);

        private void HelpAboutMenu_Click(object sender, RoutedEventArgs e) =>
            Program.Windows.About.Show();

        private void Window_Closing(object sender, CancelEventArgs e) =>
            Program.CloseApp();

        private void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
            Program.LoadRDX((FileInfo)((ComboBox)sender).SelectedItem);

        private void ExportDocument_Click(object sender, RoutedEventArgs e) =>
            Program.ExportDocument(Program.SelectFolder());

        private void ExportTables_Click(object sender, RoutedEventArgs e) =>
            Program.ExportTables(Program.SelectFolder());

        private void ExportModels_Click(object sender, RoutedEventArgs e) =>
            Program.ExportModels(Program.SelectFolder());

        private void ExportMotions_Click(object sender, RoutedEventArgs e) =>
            Program.ExportMotions(Program.SelectFolder());

        private void ExportScripts_Click(object sender, RoutedEventArgs e) =>
            Program.ExportScripts(Program.SelectFolder());

        private void ExportTextures_Click(object sender, RoutedEventArgs e) =>
            Program.ExportTextures(Program.SelectFolder());

        private void ExportHeader_Click(object sender, RoutedEventArgs e) =>
            Program.ExportHeader(Program.SelectFolder());

        private void ExportFiles_Click(object sender, RoutedEventArgs e) =>
            Program.ExportFiles(Program.SelectFolder());
    }
}