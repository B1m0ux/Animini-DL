using System.Net.Http;
using System.Threading.Tasks;

namespace Animini_DL.utils
{
    internal class AnimeUtils
    {
        public static async Task<string> GetResponse(HttpClient httpClient, string url)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
