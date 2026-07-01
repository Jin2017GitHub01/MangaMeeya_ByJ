using System;
using System.IO;
using System.Text.Json;

namespace MangaMeeya_by_Jin
{
    /// <summary>
    /// 애플리케이션 설정을 저장하고 불러오는 클래스입니다.
    /// 마지막으로 열었던 ZIP 파일 경로, 언어 설정 등을 JSON 파일로 관리합니다.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// 설정 파일이 저장되는 전체 경로입니다.
        /// %APPDATA%\MangaMeeya_by_Jin\settings.json
        /// </summary>
        private static string settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MangaMeeya_by_Jin",
            "settings.json"
        );

        /// <summary>마지막으로 열었던 ZIP 파일의 전체 경로</summary>
        public string LastZipFilePath { get; set; }

        /// <summary>마지막으로 수정된 시간</summary>
        public DateTime LastModified { get; set; }

        /// <summary>선택한 언어 (ko, en, zh, ja)</summary>
        public string Language { get; set; } = "ko";

        /// <summary>만화 읽기 방향 (LTR: 왼→오, RTL: 오→왼)</summary>
        public string ReadingDirection { get; set; } = "LTR";

        /// <summary>
        /// 저장된 설정 파일을 읽어서 AppSettings 인스턴스를 반환합니다.
        /// 파일이 없거나 오류가 발생하면 기본값으로 새 인스턴스를 반환합니다.
        /// </summary>
        /// <returns>로드된 AppSettings 인스턴스</returns>
        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(settingsPath))
                {
                    string json = File.ReadAllText(settingsPath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    
                    // 파일이 여전히 존재하는지 확인
                    if (settings != null)
                    {
                        return settings;
                    }
                }
            }
            catch { }

            return new AppSettings();
        }

        /// <summary>
        /// 현재 설정을 JSON 파일로 저장합니다.
        /// 설정 디렉토리가 없으면 자동으로 생성합니다.
        /// </summary>
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