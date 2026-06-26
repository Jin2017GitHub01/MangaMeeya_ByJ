using System;
using System.IO;
using System.Text.Json;

namespace MangaMeeya_by_Jin
{
    public class AppSettings
    {
        private static string settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MangaMeeya_by_Jin",
            "settings.json"
        );

        public string LastZipFilePath { get; set; }
        public DateTime LastModified { get; set; }
        public string Language { get; set; } = "ko";

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(settingsPath))
                {
                    string json = File.ReadAllText(settingsPath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    
                    // 파일이 여전히 존재하는지 확인
                    if (!string.IsNullOrEmpty(settings?.LastZipFilePath) && File.Exists(settings.LastZipFilePath))
                    {
                        return settings;
                    }
                }
            }
            catch { }

            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                string directory = Path.GetDirectoryName(settingsPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsPath, json);
            }
            catch { }
        }
    }
}
