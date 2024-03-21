using Microsoft.Win32;
using RDXplorer.Extensions;
using RDXplorer.Views;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfHexaEditor;
using WpfHexaEditor.Core;
using WpfHexaEditor.Core.CharacterTable;
using WpfHexaEditor.Dialog;

namespace RDXplorer
{
    public partial class HexEditorWindow
    {
        public HexEditorWindow() =>
            InitializeComponent();

        private void ExitMenu_Click(object sender, RoutedEventArgs e) =>
            Close();

        private void CopyHexaMenu_Click(object sender, RoutedEventArgs e) =>
            HexEdit.CopyToClipboard(CopyPasteMode.HexaString);

        private void CopyStringMenu_Click(object sender, RoutedEventArgs e) =>
            HexEdit.CopyToClipboard();

        private void GOPosition_Click(object sender, RoutedEventArgs e)
        {
            if (long.TryParse(PositionText.Text, out long position))
                HexEdit.SetPosition(position, 1);
            else
                MessageBox.Show("Enter long value.");

            ViewMenu.IsSubmenuOpen = false;
        }

        private void PositionText_TextChanged(object sender, TextChangedEventArgs e) =>
            GoPositionButton.IsEnabled = long.TryParse(PositionText.Text, out long _);

        private void SetBookMarkButton_Click(object sender, RoutedEventArgs e) =>
            HexEdit.SetBookMark();

        private void DeleteBookmark_Click(object sender, RoutedEventArgs e) =>
            HexEdit.ClearScrollMarker(ScrollMarker.Bookmark);

        private void FindAllSelection_Click(object sender, RoutedEventArgs e) =>
            HexEdit.FindAllSelection(true);

        private void SelectAllButton_Click(object sender, RoutedEventArgs e) =>
            HexEdit.SelectAll();

        private void CTableASCIIButton_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.TypeOfCharacterTable = CharacterTableType.Ascii;
            CTableAsciiButton.IsChecked = true;
            CTableTblButton.IsChecked = false;
            CTableTblDefaultAsciiButton.IsChecked = false;
        }

        private void CTableTBLButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new();

            if (fileDialog.ShowDialog() == null) return;
            if (!File.Exists(fileDialog.FileName)) return;

            Application.Current.MainWindow.Cursor = Cursors.Wait;

            HexEdit.LoadTblFile(fileDialog.FileName);
            HexEdit.TypeOfCharacterTable = CharacterTableType.TblFile;
            CTableAsciiButton.IsChecked = false;
            CTableTblButton.IsChecked = true;
            CTableTblDefaultAsciiButton.IsChecked = false;

            Application.Current.MainWindow.Cursor = null;
        }

        private void CTableTBLDefaultASCIIButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Cursor = Cursors.Wait;

            HexEdit.TypeOfCharacterTable = CharacterTableType.TblFile;
            HexEdit.LoadDefaultTbl();

            Application.Current.MainWindow.Cursor = null;
        }

        private void CTableTblDefaultEBCDICButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Cursor = Cursors.Wait;

            HexEdit.TypeOfCharacterTable = CharacterTableType.TblFile;
            HexEdit.LoadDefaultTbl(DefaultCharacterTableType.EbcdicWithSpecialChar);

            Application.Current.MainWindow.Cursor = null;
        }

        private void CTableTblDefaultEBCDICNoSPButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Cursor = Cursors.Wait;

            HexEdit.TypeOfCharacterTable = CharacterTableType.TblFile;
            HexEdit.LoadDefaultTbl(DefaultCharacterTableType.EbcdicNoSpecialChar);

            Application.Current.MainWindow.Cursor = null;
        }

        private void FindMenu_Click(object sender, RoutedEventArgs e) =>
            new FindWindow(HexEdit, HexEdit.GetSelectionByteArray())
            {
                Owner = this
            }.Show();

        private void ReverseSelection_Click(object sender, RoutedEventArgs e) =>
            HexEdit.ReverseSelection();

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Program.IsClosing) return;

            Visibility = Visibility.Hidden;
            e.Cancel = true;
        }

        public void ShowFile(FileInfo path)
        {
            Show();
            Activate();
            OpenFile(path);
        }

        public void OpenFile(FileInfo path)
        {
            if (path == null)
                return;

            MemoryStream stream = new();

            using (FileStream fileStream = path.OpenReadShared())
                fileStream.CopyTo(stream);

            stream.Position = 0;

            HexEdit.Stream = stream;
        }

        public void SetPosition(long offset, long length = 1)
        {
            try
            {
                Activate();
                HexEdit.SetPosition(offset, length);
            }
            catch (Exception) { }
        }
    }
}