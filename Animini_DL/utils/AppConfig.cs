using Newtonsoft.Json;
using System.IO;
using System;
using Microsoft.Win32;

namespace Animini_DL.utils
{
    public class AppConfig
    {
        public class Folders
        {
            public string SaveLocation { get; set; }
        }

        public Folders AppFolders { get; set; }

        public static AppConfig Load()
        {
            try
            {
                string json = File.ReadAllText("appSettings.json");
                return JsonConvert.DeserializeObject<AppConfig>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading AppConfig: {ex.Message}");
                return null;
            }
        }
    }
}
