using Newtonsoft.Json;
using System;
using System.IO;

namespace Animini_DL.utils
{
    public class AppConfig
    {
        public class Folders
        {
            public string SaveLocation { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Animinid");
        }

        public Folders AppFolders { get; set; }

        public static AppConfig Load()
        {
            try
            {
                string json = File.ReadAllText("appSettings.json");
                var config = JsonConvert.DeserializeObject<AppConfig>(json);

                if (string.IsNullOrEmpty(config?.AppFolders?.SaveLocation))
                {
                    config.AppFolders.SaveLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Animinid");
                }

                return config;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading AppConfig: {ex.Message}");
                return null;
            }
        }
    }
}
