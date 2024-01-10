using Animini_DL.utils;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Animini_DL
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SizeChanged += MainWindow_SizeChanged;
        }

        private void AdjustLayoutBasedOnWindowSize(SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 650)
            {
                if (MainGrid.ColumnDefinitions.Count >= 2)
                {
                    infoPanel.Visibility = System.Windows.Visibility.Hidden;
                    MainGrid.ColumnDefinitions.RemoveAt(1);
                }
            }
            else if (MainGrid.ColumnDefinitions.Count <= 1)
            {
                infoPanel.Visibility = System.Windows.Visibility.Visible;
                ColumnDefinition column = new ColumnDefinition();
                MainGrid.ColumnDefinitions.Add(column);
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustLayoutBasedOnWindowSize(e);
        }
        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            AnimesClasses.AnimeResponse animeResponse = await AnimeUtils.SearchAnimes(this);
            AnimeListBox.ItemsSource = animeResponse.Results;
        }

        private void AnimeListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (AnimeListBox.SelectedItem != null)
            {
                AnimesClasses.Anime selectedAnime = (AnimesClasses.Anime)AnimeListBox.SelectedItem;
                InfoPanelTitle.Text = selectedAnime.Title;
                LargeImage.Source = new BitmapImage(new Uri(selectedAnime.Image));
            }
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            AnimesClasses.Anime selectedAnime = (AnimesClasses.Anime)AnimeListBox.SelectedItem;
            if (selectedAnime != null)
            {
                AnimesClasses.Anime animeInfoResponse = await AnimeUtils.GetAnimeInfos(selectedAnime.Id);
                DownloadSettingsWindow settingsWindow = new DownloadSettingsWindow();
                bool? result = settingsWindow.ShowDialog();
                settingsWindow.Owner = this;
                if (result == true)
                {
                    string selectedQuality = settingsWindow.SelectedQuality;
                    int startEpisode = settingsWindow.StartEpisode;
                    int endEpisode = settingsWindow.EndEpisode;
                    settingsWindow.Close();

                    foreach (AnimesClasses.Episode episode in animeInfoResponse.Episodes)
                    {
                        if (episode.Number >= startEpisode && episode.Number <= endEpisode)
                        {
                            Process ffmpegProcess = await DownloadManager.DownloadAnime(episode, selectedQuality, this, animeInfoResponse);
                            await ffmpegProcess.WaitForExitAsync();
                        }
                    }
                }
            }
        }
    }
}
