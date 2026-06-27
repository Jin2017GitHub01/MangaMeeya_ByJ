using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MangaMeeya_by_Jin
{
    public partial class HistoryWindow : Window
    {
        public delegate void FileOpenRequestedHandler(string filePath, int? pageIndex);
        public event FileOpenRequestedHandler FileOpenRequested;

        public HistoryWindow()
        {
            InitializeComponent();
            LoadHistory();
        }

        private void LoadHistory()
        {
            var entries = HistoryManager.GetEntries();
            var displayList = entries.Select(e => new HistoryDisplayItem
            {
                FileName = e.FileName,
                FullPath = e.FullPath,
                LastPage = e.LastPage,
                TotalPages = e.TotalPages,
                LastViewed = e.LastViewed,
                PageProgress = $"{e.LastPage + 1} / {e.TotalPages}",
                LastViewedString = FormatTimeAgo(e.LastViewed)
            }).ToList();

            HistoryDataGrid.ItemsSource = displayList;

            // 기록이 없으면 메시지 표시
            if (displayList.Count == 0)
            {
                this.Title = "📜 열람 기록 - 기록이 없습니다";
            }
            else
            {
                this.Title = $"📜 열람 기록 - 총 {displayList.Count}개";
            }
        }

        private string FormatTimeAgo(DateTime dateTime)
        {
            TimeSpan diff = DateTime.Now - dateTime;

            if (diff.TotalMinutes < 1)
                return "방금 전";
            if (diff.TotalHours < 1)
                return $"{(int)diff.TotalMinutes}분 전";
            if (diff.TotalDays < 1)
                return $"{(int)diff.TotalHours}시간 전";
            if (diff.TotalDays < 7)
                return $"{(int)diff.TotalDays}일 전";
            if (diff.TotalDays < 30)
                return $"{(int)(diff.TotalDays / 7)}주 전";
            if (diff.TotalDays < 365)
                return $"{(int)(diff.TotalDays / 30)}개월 전";

            return dateTime.ToString("yyyy-MM-dd");
        }

        private void OpenSelectedFile()
        {
            if (HistoryDataGrid.SelectedItem is HistoryDisplayItem selected)
            {
                // 파일이 존재하는지 확인
                if (!File.Exists(selected.FullPath))
                {
                    MessageBox.Show($"파일을 찾을 수 없습니다:\n{selected.FullPath}\n\n파일이 이동 또는 삭제되었습니다.",
                                    "파일 없음", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                FileOpenRequested?.Invoke(selected.FullPath, selected.LastPage);
                this.Close();
            }
        }

        private void HistoryDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenSelectedFile();
        }

        private void OpenSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            OpenSelectedFile();
        }

        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryDataGrid.SelectedItem is HistoryDisplayItem selected)
            {
                HistoryManager.RemoveEntry(selected.FullPath);
                LoadHistory();
            }
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryDataGrid.ItemsSource is ICollection<HistoryDisplayItem> items && items.Count > 0)
            {
                var result = MessageBox.Show("모든 열람 기록을 삭제하시겠습니까?", "기록 삭제",
                                              MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    HistoryManager.ClearAll();
                    LoadHistory();
                }
            }
        }

        // 화면 표시용 래퍼 클래스
        public class HistoryDisplayItem : INotifyPropertyChanged
        {
            public string FileName { get; set; }
            public string FullPath { get; set; }
            public int LastPage { get; set; }
            public int TotalPages { get; set; }
            public DateTime LastViewed { get; set; }
            public string PageProgress { get; set; }
            public string LastViewedString { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
