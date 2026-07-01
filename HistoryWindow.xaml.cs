using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MangaMeeya_by_Jin
{
    /// <summary>
    /// 열람 기록을 표시하고 관리하는 창입니다.
    /// 사용자가 이전에 열었던 ZIP 파일 목록을 보여주고, 선택하여 다시 열 수 있습니다.
    /// </summary>
    public partial class HistoryWindow : Window
    {
        /// <summary>
        /// 파일 열기 요청을 처리하는 델리게이트입니다.
        /// </summary>
        /// <param name="filePath">열 파일의 전체 경로</param>
        /// <param name="pageIndex">이동할 페이지 인덱스 (null이면 첫 페이지)</param>
        public delegate void FileOpenRequestedHandler(string filePath, int? pageIndex);

        /// <summary>파일 열기 요청 이벤트</summary>
        public event FileOpenRequestedHandler FileOpenRequested;

        /// <summary>
        /// HistoryWindow 생성자. 컴포넌트를 초기화하고 기록 목록을 로드합니다.
        /// </summary>
        public HistoryWindow()
        {
            InitializeComponent();
            LoadHistory();
        }

        /// <summary>
        /// HistoryManager에서 기록을 불러와 DataGrid에 표시합니다.
        /// 기록이 없으면 제목에 "기록이 없습니다" 메시지를 표시합니다.
        /// </summary>
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

        /// <summary>
        /// DateTime을 현재 시간과 비교하여 상대적인 시간 문자열로 변환합니다.
        /// 예: "방금 전", "5분 전", "2시간 전", "3일 전" 등
        /// </summary>
        /// <param name="dateTime">변환할 DateTime</param>
        /// <returns>상대적 시간 문자열</returns>
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

        /// <summary>
        /// DataGrid에서 선택된 항목의 파일을 엽니다.
        /// 파일이 존재하지 않으면 경고 메시지를 표시합니다.
        /// </summary>
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

        /// <summary>
        /// DataGrid 항목을 더블클릭하면 선택한 파일을 엽니다.
        /// </summary>
        private void HistoryDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenSelectedFile();
        }

        /// <summary>
        /// "선택한 파일 열기" 버튼 클릭 시 선택한 파일을 엽니다.
        /// </summary>
        private void OpenSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            OpenSelectedFile();
        }

        /// <summary>
        /// "선택 삭제" 버튼 클릭 시 선택한 기록 항목을 삭제하고 목록을 새로고침합니다.
        /// </summary>
        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryDataGrid.SelectedItem is HistoryDisplayItem selected)
            {
                HistoryManager.RemoveEntry(selected.FullPath);
                LoadHistory();
            }
        }

        /// <summary>
        /// "전체 삭제" 버튼 클릭 시 확인 대화상자를 표시하고,
        /// 사용자가 확인하면 모든 기록을 삭제합니다.
        /// </summary>
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

        /// <summary>
        /// 화면 표시용 래퍼 클래스입니다.
        /// HistoryEntry의 데이터를 UI에 표시하기 적합한 형태로 가공하여 보여줍니다.
        /// INotifyPropertyChanged를 구현하여 데이터 바인딩을 지원합니다.
        /// </summary>
        public class HistoryDisplayItem : INotifyPropertyChanged
        {
            /// <summary>파일명</summary>
            public string FileName { get; set; }

            /// <summary>전체 경로</summary>
            public string FullPath { get; set; }

            /// <summary>마지막 페이지 인덱스</summary>
            public int LastPage { get; set; }

            /// <summary>전체 페이지 수</summary>
            public int TotalPages { get; set; }

            /// <summary>마지막 열람 시간</summary>
            public DateTime LastViewed { get; set; }

            /// <summary>페이지 진행률 문자열 (예: "3 / 20")</summary>
            public string PageProgress { get; set; }

            /// <summary>마지막 열람 시간 상대 표시 문자열</summary>
            public string LastViewedString { get; set; }

            /// <summary>속성 변경 이벤트</summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// 속성 변경 시 PropertyChanged 이벤트를 발생시킵니다.
            /// </summary>
            /// <param name="propertyName">변경된 속성 이름</param>
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}