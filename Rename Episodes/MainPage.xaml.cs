using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Rename_Episodes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static StorageFolder dir;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void resizeTextboxToText(object sender, TextChangedEventArgs e)
        {
            TextBox sendingTextbox = (TextBox)sender;
            TextBlock temporaryTextblock = new TextBlock { Text = sendingTextbox.Text, FontSize = sendingTextbox.FontSize };
            temporaryTextblock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            sendingTextbox.Width = temporaryTextblock.DesiredSize.Width + 75;
        }

        private async void browse_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                dir = folder;
                directory.Text = folder.Path;
            }
        }

        public int validateFields()
        {
            feedback.Text = "";
            feedback.Foreground = new SolidColorBrush(Colors.Red);
            before.Text = "";
            after.Text = "";

            // validate episodes directory
            if (dir == null)
            {
                feedback.Text = "Please select directory of episodes.";
                return -1;
            }

            // validate season integer
            int seasonNumber;
            if (!int.TryParse(season.Text, out seasonNumber))
            {
                feedback.Text = "Invalid number for season. Please specify a valid integer for the season.";
                return -1;
            }
            return seasonNumber;
        }

        private async void preview_Click(object sender, RoutedEventArgs e)
        {
            int seasonNumber = validateFields();
            if (seasonNumber == -1)
            {
                return;
            }
            String seasonString = "";
            if (seasonNumber < 10)
            {
                seasonString += "0";
            }
            seasonString += seasonNumber.ToString();

            // loop through files
            IReadOnlyList<StorageFile> files = await dir.GetFilesAsync();
            for (int i=0; i < files.Count; i++)
            {
                StorageFile file = files.ElementAt(i);

                // before textblock
                before.Text += "\n" + file.Name;

                // after textblock
                String episodeString = "";
                if (i < 9)
                {
                    episodeString += "0";
                }
                episodeString += (i+1).ToString();
                String newName = show.Text + " - " + "s" + seasonString + "e" + episodeString + file.FileType;
                after.Text = after.Text + "\n" + newName;
            }
        }

        private async void convert_Click(object sender, RoutedEventArgs e)
        {
            int seasonNumber = validateFields();
            if (seasonNumber == -1)
            {
                return;
            }
            String seasonString = "";
            if (seasonNumber < 10)
            {
                seasonString += "0";
            }
            seasonString += seasonNumber.ToString();

            // loop through files
            IReadOnlyList<StorageFile> files = await dir.GetFilesAsync();
            for (int i = 0; i < files.Count; i++)
            {
                StorageFile file = files.ElementAt(i);

                // before textblock
                before.Text += "\n" + file.Name;

                // after textblock

                String episodeString = "";
                if (i < 9)
                {
                    episodeString += "0";
                }
                episodeString += (i + 1).ToString();
                String newName = show.Text + " - " + "s" + seasonString + "e" + episodeString + file.FileType;
                after.Text = after.Text + "\n" + newName;
                await file.RenameAsync(newName);
            }

            feedback.Foreground = new SolidColorBrush(Colors.Green);
            feedback.Text = "Success!";
        }
    }
}
