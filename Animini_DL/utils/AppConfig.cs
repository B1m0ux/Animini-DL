using Newtonsoft.Json;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animini_DL.utils
{
    public class AppConfig
    {
        public static string SaveLocationFolder { get; set; }

        public static AppConfig Load()
        {
            string json = File.ReadAllText("appSettings.json");
            return JsonConvert.DeserializeObject<AppConfig>(json);
        }
    }
}
