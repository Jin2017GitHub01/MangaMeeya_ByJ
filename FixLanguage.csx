using System;
using System.Text.Json;
using System.IO;

namespace MangaMeeya_by_Jin
{
    class Program
    {
        static void Main()
        {
            string path = @"MainWindow.xaml.cs";
            string content = File.ReadAllText(path);
            
            string oldCode = @"                AppSettings settings = new AppSettings
                {
                    LastZipFilePath = currentZipPath,
                    LastModified = DateTime.Now
                };";
            
            string newCode = @"                AppSettings settings = AppSettings.Load();
                settings.LastZipFilePath = currentZipPath;
                settings.LastModified = DateTime.Now;";
            
            if (content.Contains(oldCode))
            {
                content = content.Replace(oldCode, newCode);
                File.WriteAllText(path, content);
                Console.WriteLine("OK");
            }
            else
            {
                Console.WriteLine("NOT FOUND");
            }
        }
    }
}
