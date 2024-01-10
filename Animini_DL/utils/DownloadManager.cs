using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Animini_DL.utils
{
    internal static class DownloadManager
    {
        private static readonly Process ffmpegProcess = new Process();
        private static readonly List<Process> processList = new List<Process>();
        private static readonly string ffmpegPath = "ffmpeg\\ffmpeg.exe";

        private static string BuildOutputFilePath(string title, int episodeNumber)
        {
            string saveLocation = "C:\\Users\\tobias\\Animinid";
            string saveFolder = Path.Combine(saveLocation, title);
            Directory.CreateDirectory(saveFolder);
            string outputFileName = $"{title} - Episode {episodeNumber}.mp4";
            string outputFilePath = Path.Combine(saveFolder, outputFileName);
            return outputFilePath;
        }

        private static async Task<List<AnimesClasses.Source>> GetAnimeSources(AnimesClasses.Episode episode)
        {
            string content = await AnimeUtils.GetResponse($"https://consu-api-2.vercel.app/anime/gogoanime/watch/{episode.Id}?server=gogocdn");
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
            var sourcesArray = responseData?["sources"] as JArray;

            return sourcesArray?.ToObject<List<AnimesClasses.Source>>();
        }

        private static void RunFfmpegCommand(Process ffmpegProcess, string ffmpegCommand)
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

        public static async Task<Process> DownloadAnime(AnimesClasses.Episode episode, string selectedQuality, MainWindow mainWindow, AnimesClasses.AnimeInfo animeInfo)
        {
            try
            {
                List<AnimesClasses.Source> sources = await GetAnimeSources(episode);

                if (sources != null && sources.Count > 0)
                {
                    AnimesClasses.Source source = sources.Find(s => s.Quality.Equals(selectedQuality, StringComparison.OrdinalIgnoreCase));

                    if (source != null)
                    {
                        string title = animeInfo.Title;
                        int episodeNumber = episode.Number;

                        string outputFilePath = BuildOutputFilePath(title, episodeNumber);

                        string ffmpegCommand = $"-i \"{source.Url}\" -c:v copy -c:a copy -y \"{outputFilePath}\"";

                        Process ffmpegProcess = new Process();
                        RunFfmpegCommand(ffmpegProcess, ffmpegCommand);
                        return ffmpegProcess;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
                mainWindow.SearchBar.Text = "error";
                return null;
            }
        }
    }
}
