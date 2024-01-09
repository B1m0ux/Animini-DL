using System;
using System.Windows;

namespace Animini_DL
{
    public partial class DownloadSettingsWindow : Window
    {
        public string SelectedQuality { get; private set; }
        public int StartEpisode { get; private set; }
        public int EndEpisode { get; private set; }

        public DownloadSettingsWindow()
        {
            InitializeComponent();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                SelectedQuality = GetSelectedQuality();
                StartEpisode = ParseEpisodeNumber(txtStartEpisode.Text);
                EndEpisode = ParseEpisodeNumber(txtEndEpisode.Text);

                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please enter valid input.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            return !string.IsNullOrWhiteSpace(GetSelectedQuality()) &&
                   ParseEpisodeNumber(txtStartEpisode.Text) > 0 &&
                   ParseEpisodeNumber(txtEndEpisode.Text) > 0;
        }

        private string GetSelectedQuality()
        {
            if (radio360p.IsChecked == true) return "360p";
            if (radio480p.IsChecked == true) return "480p";
            if (radio720p.IsChecked == true) return "720p";
            if (radio1080p.IsChecked == true) return "1080p";

            return null;
        }

        private static int ParseEpisodeNumber(string input)
        {
            if (int.TryParse(input, out int result))
            {
                return result;
            }
            return -1;
        }
    }
}
