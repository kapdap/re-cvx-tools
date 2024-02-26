using RECVXFlagTool.Utilities;
using System.ComponentModel;
using System.Windows;

namespace RECVXFlagTool
{
    public partial class AboutWindow : Window
    {
        public AboutWindow() =>
            InitializeComponent();

        private void WebsiteButton_Click(object sender, RoutedEventArgs e) =>
            URLHelper.OpenURL("https://github.com/kapdap/re-cvx-tools");

        private void GitHubButton_Click(object sender, RoutedEventArgs e) =>
            URLHelper.OpenURL("https://github.com/kapdap/re-cvx-tools");

        private void CloseButton_Click(object sender, RoutedEventArgs e) =>
            Close();

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Program.IsClosing) return;

            Visibility = Visibility.Hidden;
            e.Cancel = true;
        }
    }
}