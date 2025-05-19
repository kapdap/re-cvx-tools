using RDXplorer.Views;
using System.ComponentModel;
using System.Windows;

namespace RDXplorer
{
    /// <summary>
    /// Interaction logic for BitmapWindow.xaml
    /// </summary>
    public partial class BitmapWindow : Window
    {
        public BitmapWindow() =>
            InitializeComponent();

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Program.IsClosing) return;

            Visibility = Visibility.Hidden;
            e.Cancel = true;
        }
    }
}