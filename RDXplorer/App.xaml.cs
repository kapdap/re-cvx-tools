using RDXplorer.Views;
using System.IO;
using System.Windows;

namespace RDXplorer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Program.Windows.Main.Show();

            for (int i = 0; i < e.Args.Length; ++i)
            {
                if ((e.Args[i] == "--file" || e.Args[i] == "-f") && i < e.Args.Length)
                {
                    FileInfo file = new(e.Args[++i]);

                    if (file.Exists)
                    {
                        Program.Models.AppView.LoadFileList(file);
                        Program.Windows.Main.FileList.SelectedValue = file.FullName;
                    }
                }
            }
        }
    }
}