using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MangaMeeya_by_Jin
{
    public class HistoryEntry
    {
        public string FileName { get; set; }        // 파일명 (표시용)
        public string FullPath { get; set; }         // 전체 경로 (재열기용)
        public int LastPage { get; set; }            // 마지막으로 본 페이지 (0-indexed)
        public int TotalPages { get; set; }          // 전체 페이지 수
        public DateTime LastViewed { get; set; }     // 마지막으로 본 시간
    }

    public static class HistoryManager
    {
        private static string historyPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MangaMeeya_by_Jin",
            "history.json"
        );

        private static List<HistoryEntry> _entries = null;
        private static readonly object _lock = new object();

        public static List<HistoryEntry> GetEntries()
        {
            EnsureLoaded();
            lock (_lock)
            {
                // 최근 본 순서로 정렬 (내림차순)
                return _entries.OrderByDescending(e => e.LastViewed).ToList();
            }
        }

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

        public static void ClearAll()
        {
            lock (_lock)
            {
                _entries = new List<HistoryEntry>();
                Save();
            }
        }

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
