using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animini_DL.utils
{
    public class AnimesClasses
    {
        public class Episode
        {
            public string Id { get; set; }
            public int Number { get; set; }
            public string Url { get; set; }
        }

        public class Anime
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Url { get; set; }
            public string Image { get; set; }
            public string ReleaseDate { get; set; }
            public string Description { get; set; }
            public List<string> Genres { get; set; }
            public string SubOrDub { get; set; }
            public string Type { get; set; }
            public string Status { get; set; }
            public string OtherName { get; set; }
            public int TotalEpisodes { get; set; }
            public List<Episode> Episodes { get; set; }
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
    }
}
