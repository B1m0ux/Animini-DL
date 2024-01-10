using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Animini_DL.utils
{
    internal static class AnimeUtils
    {
        public static async Task<string> GetResponse(string url)
        {
            using (HttpClient httpClient = new())
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        public static async Task<AnimesClasses.Anime> GetAnimeInfos(string id)
        {
            try
            {
                string content = await AnimeUtils.GetResponse($"https://consu-api-2.vercel.app/anime/gogoanime/info/{id}");
                return JsonConvert.DeserializeObject<AnimesClasses.Anime>(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
                throw;
            }
        }

        public static async Task<AnimesClasses.AnimeResponse> SearchAnimes(MainWindow mainWindow)
        {
            try
            {
                string content = await GetResponse($"https://consu-api-2.vercel.app/anime/gogoanime/{mainWindow.SearchBar.Text}");
                return JsonConvert.DeserializeObject<AnimesClasses.AnimeResponse>(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
                return null;
            }
        }

    }
}
