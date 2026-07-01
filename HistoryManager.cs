using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MangaMeeya_by_Jin
{
    /// <summary>
    /// 열람 기록의 각 항목을 나타내는 클래스입니다.
    /// 파일명, 경로, 마지막 페이지, 전체 페이지 수, 마지막 열람 시간을 저장합니다.
    /// </summary>
    public class HistoryEntry
    {
        /// <summary>파일명 (표시용)</summary>
        public string FileName { get; set; }

        /// <summary>전체 경로 (재열기용)</summary>
        public string FullPath { get; set; }

        /// <summary>마지막으로 본 페이지 (0-indexed)</summary>
        public int LastPage { get; set; }

        /// <summary>전체 페이지 수</summary>
        public int TotalPages { get; set; }

        /// <summary>마지막으로 본 시간</summary>
        public DateTime LastViewed { get; set; }
    }

    /// <summary>
    /// 열람 기록을 관리하는 정적 클래스입니다.
    /// JSON 파일에 기록을 저장하고 불러오며, 최대 100개의 항목을 유지합니다.
    /// </summary>
    public static class HistoryManager
    {
        /// <summary>
        /// 기록 파일이 저장되는 전체 경로입니다.
        /// %APPDATA%\MangaMeeya_by_Jin\history.json
        /// </summary>
        private static string historyPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MangaMeeya_by_Jin",
            "history.json"
        );

        /// <summary>메모리에 로드된 기록 항목 리스트</summary>
        private static List<HistoryEntry> _entries = null;

        /// <summary>스레드 동기화를 위한 잠금 객체</summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// 저장된 모든 열람 기록을 최근 본 순서로 반환합니다.
        /// </summary>
        /// <returns>최근 본 순서로 정렬된 HistoryEntry 리스트</returns>
        public static List<HistoryEntry> GetEntries()
        {
            EnsureLoaded();
            lock (_lock)
            {
                // 최근 본 순서로 정렬 (내림차순)
                return _entries.OrderByDescending(e => e.LastViewed).ToList();
            }
        }

        /// <summary>
        /// 파일 열람 기록을 저장합니다.
        /// 같은 파일이 이미 기록에 있으면 제거 후 새로 추가하며, 최대 100개까지만 유지합니다.
        /// </summary>
        /// <param name="fullPath">열람한 파일의 전체 경로</param>
        /// <param name="pageIndex">마지막으로 본 페이지 인덱스 (0부터 시작)</param>
        /// <param name="totalPages">파일의 전체 페이지 수</param>
        public static void RecordView(string fullPath, int pageIndex, int totalPages)
        {
            if (string.IsNullOrEmpty(fullPath)) return;

            EnsureLoaded();

            lock (_lock)
            {
                // 기존에 같은 파일이 있으면 제거
                _entries.RemoveAll(e => e.FullPath.Equals(fullPath, StringComparison.OrdinalIgnoreCase));

                // 새 항목 추가
                _entries.Add(new HistoryEntry
                {
                    FileName = Path.GetFileName(fullPath),
                    FullPath = fullPath,
                    LastPage = pageIndex,
                    TotalPages = totalPages,
                    LastViewed = DateTime.Now
                });

                // 최대 100개만 유지
                if (_entries.Count > 100)
                {
                    _entries = _entries.OrderByDescending(e => e.LastViewed)
                                       .Take(100)
                                       .ToList();
                }

                Save();
            }
        }

        /// <summary>
        /// 특정 파일의 열람 기록을 삭제합니다.
        /// </summary>
        /// <param name="fullPath">삭제할 파일의 전체 경로</param>
        public static void RemoveEntry(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)) return;

            EnsureLoaded();
            lock (_lock)
            {
                _entries.RemoveAll(e => e.FullPath.Equals(fullPath, StringComparison.OrdinalIgnoreCase));
                Save();
            }
        }

        /// <summary>
        /// 모든 열람 기록을 삭제합니다.
        /// </summary>
        public static void ClearAll()
        {
            lock (_lock)
            {
                _entries = new List<HistoryEntry>();
                Save();
            }
        }

        /// <summary>
        /// 특정 파일의 마지막으로 본 페이지를 반환합니다.
        /// </summary>
        /// <param name="fullPath">조회할 파일의 전체 경로</param>
        /// <returns>마지막 페이지 인덱스 (없으면 null)</returns>
        public static int? GetLastPage(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)) return null;

            EnsureLoaded();
            lock (_lock)
            {
                var entry = _entries.FirstOrDefault(e => e.FullPath.Equals(fullPath, StringComparison.OrdinalIgnoreCase));
                return entry?.LastPage;
            }
        }

        /// <summary>
        /// 기록 데이터를 파일에서 메모리로 로드합니다.
        /// 최초 호출 시에만 실제로 로드하며, 이후에는 무시합니다 (Lazy Loading).
        /// </summary>
        private static void EnsureLoaded()
        {
            if (_entries != null) return;

            lock (_lock)
            {
                if (_entries != null) return;

                try
                {
                    if (File.Exists(historyPath))
                    {
                        string json = File.ReadAllText(historyPath);
                        _entries = JsonSerializer.Deserialize<List<HistoryEntry>>(json) ?? new List<HistoryEntry>();
                    }
                    else
                    {
                        _entries = new List<HistoryEntry>();
                    }
                }
                catch
                {
                    _entries = new List<HistoryEntry>();
                }
            }
        }

        /// <summary>
        /// 현재 메모리의 기록 데이터를 JSON 파일로 저장합니다.
        /// </summary>
        private static void Save()
        {
            try
            {
                string directory = Path.GetDirectoryName(historyPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonSerializer.Serialize(_entries, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(historyPath, json);
            }
            catch { }
        }
    }
}