using RDXplorer.Views;
using System.ComponentModel;
using System.Windows;

namespace RDXplorer
{
    /// <summary>
    /// Interaction logic for ScriptingWindow.xaml
    /// </summary>
    public partial class ScriptingWindow : Window
    {
        public ScriptingWindow() =>
            InitializeComponent();

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Program.IsClosing) return;

            Visibility = Visibility.Hidden;
            e.Cancel = true;
        }
    }
}