using RDXplorer.Formats.TIM2;
using RDXplorer.ViewModels;
using RDXplorer.Views;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RDXplorer
{
    /// <summary>
    /// Interaction logic for BitmapWindow.xaml
    /// </summary>
    public partial class BitmapWindow : Window
    {
        private bool isDragging;
        private Point lastMousePosition;

        private readonly double[] zoomLevels = { 0.25, 0.5, 0.75, 1.0, 1.25, 1.5, 2.0, 3.0, 4.0 };
        public int CurrentZoom { get; set; } = 3;

        public DataGrid ImageGrid { get; set; }
        public int CurrentIndex { get; set; } = -1;

        public TextureBlockViewModelEntry CurrentEntry { get; set; }

        // TODO: Implement generic interface for textures
        public Tim2Document CurrentDocument { get; set; }
        public Tim2Picture CurrentPicture { get; set; }

        public BitmapWindow()
        {
            InitializeComponent();
            InitializeZoomLevels();
            DataContextChanged += UpdateDataContext;
        }

        private void UpdateDataContext(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is not DataGrid grid)
                return;
            ImageGrid = grid;
        }

        public void Render()
        {
            if (ImageGrid != null && ImageGrid.Items.Count != 0)
                Render(ImageGrid.SelectedIndex);
        }

        public void Render(int index)
        {
            if (ImageGrid == null || index < 0 || index >= ImageGrid.Items.Count)
                return;

            CurrentIndex = index;
            CurrentEntry = (TextureBlockViewModelEntry)ImageGrid.Items[CurrentIndex];

            if (CurrentEntry.Model.Fields.Type.Text == "TIM2")
            {
                CurrentDocument = new(CurrentEntry.Model.Fields.Data.Data);
                CurrentPicture = CurrentDocument.Pictures[0];

                BitmapImage.Source = Tim2Converter.Decode(CurrentPicture, true);
            }

            UpdateInterface();
        }

        private void UpdateInterface()
        {
            if (ImageGrid == null)
            {
                PreviousButton.IsEnabled = false;
                NextButton.IsEnabled = false;
                return;
            }

            PreviousButton.IsEnabled = CurrentIndex > 0;
            NextButton.IsEnabled = CurrentIndex < ImageGrid.Items.Count - 1;

            ImageStatusBar.Text = $"{CurrentPicture.Width} × {CurrentPicture.Height} ({Utilities.FormatFileSize(CurrentPicture.ImageSize)}) ({CurrentPicture.ImageColorType}) ({CurrentPicture.ClutColorType})"; ;
            IndexStatusBar.Text = $"{CurrentIndex + 1} / {ImageGrid.Items.Count}";
        }

        private void InitializeZoomLevels()
        {
            foreach (double zoom in zoomLevels)
                ZoomLevels.Items.Add($"{zoom * 100:0}%");
            ZoomLevels.SelectedIndex = CurrentZoom;
            UpdateZoom();
        }

        private void UpdateZoom()
        {
            double zoom = zoomLevels[CurrentZoom];
            ImageScale.ScaleX = zoom;
            ImageScale.ScaleY = zoom;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Delta > 0 && CurrentZoom < zoomLevels.Length - 1)
                    CurrentZoom++;
                else if (e.Delta < 0 && CurrentZoom > 0)
                    CurrentZoom--;

                ZoomLevels.SelectedIndex = CurrentZoom;
                UpdateZoom();
                e.Handled = true;
            }
        }

        private void ZoomLevels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ZoomLevels.SelectedIndex >= 0)
            {
                CurrentZoom = ZoomLevels.SelectedIndex;
                UpdateZoom();
            }
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentZoom < zoomLevels.Length - 1)
            {
                CurrentZoom++;
                ZoomLevels.SelectedIndex = CurrentZoom;
                UpdateZoom();
            }
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentZoom > 0)
            {
                CurrentZoom--;
                ZoomLevels.SelectedIndex = CurrentZoom;
                UpdateZoom();
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.OemPlus || e.Key == Key.Add)
                    ZoomInButton_Click(sender, null);
                else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
                    ZoomOutButton_Click(sender, null);
            }
            else
            {
                if (e.Key == Key.Left)
                    PreviousButton_Click(sender, null);
                else if (e.Key == Key.Right)
                    NextButton_Click(sender, null);
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            lastMousePosition = e.GetPosition(ImageScrollViewer);
            ((UIElement)sender).CaptureMouse();
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            ((UIElement)sender).ReleaseMouseCapture();
            Mouse.OverrideCursor = null;
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging) return;

            Point position = e.GetPosition(ImageScrollViewer);
            double deltaX = lastMousePosition.X - position.X;
            double deltaY = lastMousePosition.Y - position.Y;

            ImageScrollViewer.ScrollToHorizontalOffset(ImageScrollViewer.HorizontalOffset + deltaX);
            ImageScrollViewer.ScrollToVerticalOffset(ImageScrollViewer.VerticalOffset + deltaY);

            lastMousePosition = position;
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentIndex > 0)
                Render(CurrentIndex - 1);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (ImageGrid != null && CurrentIndex < ImageGrid.Items.Count - 1)
                Render(CurrentIndex + 1);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Program.IsClosing) return;

            Visibility = Visibility.Hidden;
            e.Cancel = true;
        }
    }
}