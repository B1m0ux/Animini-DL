using Animini_DL.utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Animini_DL
{
    public partial class MainWindow : Window
    {
        private AnimesClasses.AnimeInfo animeInfoResponse;
        private readonly HttpClient httpClient = new HttpClient(); private readonly Process ffmpegProcess = new Process();
        private List<Process> processList = new List<Process>();
        private readonly string ffmpegPath = "ffmpeg\\ffmpeg.exe";
        private bool isDownloadsCanceled;

        public class Anime
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Url { get; set; }
            public string Image { get; set; }
            public string ReleaseDate { get; set; }
            public string SubOrDub { get; set; }
        }

        public class AnimeResponse
        {
            public int CurrentPage { get; set; }
            public bool HasNextPage { get; set; }
            public List<Anime> Results { get; set; }
        }

        public class Source
        {
            public string Url { get; set; }
            public bool IsM3U8 { get; set; }
            public string Quality { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            SizeChanged += MainWindow_SizeChanged;
            AppConfig.Load();
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
            await SearchAnimes();
        }

        private async Task SearchAnimes()
        {
            try
            {
                string content = await AnimeUtils.GetResponse(httpClient, $"https://consu-api-2.vercel.app/anime/gogoanime/{SearchBar.Text}");
                AnimeResponse animeResponse = JsonConvert.DeserializeObject<AnimeResponse>(content);
                AnimeListBox.ItemsSource = animeResponse.Results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }
        }

        private void AnimeListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (AnimeListBox.SelectedItem != null)
            {
                Anime selectedAnime = (Anime)AnimeListBox.SelectedItem;
                InfoPanelTitle.Text = selectedAnime.Title;
                LargeImage.Source = new BitmapImage(new Uri(selectedAnime.Image));
            }
        }

        private async Task GetAnimeInfos(string id)
        {
            try
            {
                string content = await AnimeUtils.GetResponse(httpClient, $"https://consu-api-2.vercel.app/anime/gogoanime/info/{id}");
                animeInfoResponse = JsonConvert.DeserializeObject<AnimesClasses.AnimeInfo>(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            Anime selectedAnime = (Anime)AnimeListBox.SelectedItem;
            if (selectedAnime != null)
            {
                await GetAnimeInfos(selectedAnime.Id);
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
                        if (episode.Number >= startEpisode && episode.Number <= endEpisode && !isDownloadsCanceled)
                        {
                            await DownloadAnime(episode, selectedQuality);
                            await ffmpegProcess.WaitForExitAsync();
                        }
                        else if (isDownloadsCanceled)
                        {
                            isDownloadsCanceled = false;
                            break;
                        }
                    }
                }
            }
        }

        private async Task<List<Source>> GetAnimeSources(AnimesClasses.Episode episode)
        {
            string content = await AnimeUtils.GetResponse(httpClient, $"https://consu-api-2.vercel.app/anime/gogoanime/watch/{episode.Id}?server=gogocdn");
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
            var sourcesArray = responseData["sources"] as JArray;

            List<Source> sources = sourcesArray?.ToObject<List<Source>>();
            return sources;

        }

        private static string BuildOutputFilePath(string title, int episodeNumber)
        {
            string saveLocation = AppConfig.SaveLocationFolder;
            string saveFolder = Path.Combine(saveLocation, title);
            Directory.CreateDirectory(saveFolder);
            string outputFileName = $"{title} - Episode {episodeNumber}.mp4";
            string outputFilePath = Path.Combine(saveFolder, outputFileName);
            return outputFilePath;
        }

        private void RunFfmpegCommand(string ffmpegPath, string ffmpegCommand)
        {
            ffmpegProcess.StartInfo.FileName = ffmpegPath;
            ffmpegProcess.StartInfo.Arguments = ffmpegCommand;
            ffmpegProcess.StartInfo.UseShellExecute = false;
            ffmpegProcess.StartInfo.RedirectStandardOutput = false;
            ffmpegProcess.StartInfo.RedirectStandardError = false;
            ffmpegProcess.StartInfo.CreateNoWindow = true;

            ffmpegProcess.Start();
            processList.Add(ffmpegProcess);
        }

        private async Task DownloadAnime(AnimesClasses.Episode episode, string selectedQuality)
        {
            try
            {
                List<Source> sources = await GetAnimeSources(episode);

                if (sources != null && sources.Count > 0)
                {
                    Source source = sources.Find(s => s.Quality.Equals(selectedQuality, StringComparison.OrdinalIgnoreCase));
                    try
                    {
                        string title = animeInfoResponse.Title;
                        int episodeNumber = episode.Number;

                        string outputFilePath = BuildOutputFilePath(title, episodeNumber);

                        string ffmpegCommand = $"-i \"{source.Url}\" -c:v copy -c:a copy -y \"{outputFilePath}\"";

                        RunFfmpegCommand(ffmpegPath, ffmpegCommand);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("echec");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
                SearchBar.Text = "error";
            }
        }

        private void CancelAllDownloads(object sender, RoutedEventArgs e)
        {
            isDownloadsCanceled = true;
            foreach (Process currentProcess in processList)
            {
                currentProcess.Kill();
            }
        }
    }
}
