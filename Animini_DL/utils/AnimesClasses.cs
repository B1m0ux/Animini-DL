using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animini_DL.utils
{
    internal class AnimesClasses
    {
        public class Episode
        {
            public string Id { get; set; }
            public int Number { get; set; }
            public string Url { get; set; }
        }

        public class AnimeInfo
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
    }
}
