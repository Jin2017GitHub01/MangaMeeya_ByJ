using System.Collections.Generic;

namespace MangaMeeya_by_Jin
{
    public static class LanguageManager
    {
        private static string currentLanguage = "ko";

        public static string CurrentLanguage
        {
            get => currentLanguage;
            set
            {
                if (value == "ko" || value == "en" || value == "zh" || value == "ja")
                {
                    currentLanguage = value;
                }
            }
        }

        private static readonly Dictionary<string, Dictionary<string, string>> Translations = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "ko", new Dictionary<string, string>
                {
                    { "Title", "MangaMeeya by Jin - 만화 ZIP 뷰어" },
                    { "OpenButton", "📁 ZIP 파일 열기" },
                    { "PrevButton", "◀ 이전" },
                    { "NextButton", "다음 ▶" },
                    { "FullscreenEnter", "🖥 전체화면" },
                    { "FullscreenExit", "🖥 종료" },
                    { "FileNone", "파일: 없음" },
                    { "FilePrefix", "파일: " },
                    { "GestureWheel", "🖱 휠/좌우 드래그: 이전/다음 페이지" },
                    { "GestureUp", "🖱 위로 드래그: 전체화면 토글" },
                    { "GestureDown", "🖱 아래로 드래그: 전체화면 해제" },
                    { "ZipFilter", "ZIP 파일 (*.zip)|*.zip|모든 파일 (*.*)|*.*" },
                    { "OpenTitle", "만화 ZIP 파일 선택" },
                    { "ErrorTitle", "에러" },
                    { "ErrorPrefix", "오류 발생: " },
                    { "ImageLoadFailed", "이미지 로드 실패: " },
                    { "SplashSubtitle", "만화 ZIP 뷰어" },
                    { "HistoryButton", "📜 기록" },
                    { "HistoryWindowTitle", "📜 열람 기록" },
                    { "HistoryWindowTitleCount", "📜 열람 기록 - 총 {0}개" },
                    { "HistoryEmpty", "📜 열람 기록 - 기록이 없습니다" },
                    { "HistoryColumnFileName", "파일명" },
                    { "HistoryColumnPage", "진행 페이지" },
                    { "HistoryColumnTime", "마지막 본 시간" },
                    { "HistoryOpenSelected", "📂 선택한 파일 열기" },
                    { "HistoryDeleteSelected", "🗑 선택 삭제" },
                    { "HistoryClearAll", "🗑 전체 삭제" },
                    { "HistoryFileNotFound", "파일을 찾을 수 없습니다" },
                    { "HistoryFileNotFoundDetail", "파일이 이동 또는 삭제되었습니다." },
                    { "HistoryClearConfirm", "모든 열람 기록을 삭제하시겠습니까?" },
                    { "HistoryClearTitle", "기록 삭제" },
                    { "TimeAgoJustNow", "방금 전" },
                    { "TimeAgoMinute", "분 전" },
                    { "TimeAgoHour", "시간 전" },
                    { "TimeAgoDay", "일 전" },
                    { "TimeAgoWeek", "주 전" },
                    { "TimeAgoMonth", "개월 전" }
                }
            },
            {
                "en", new Dictionary<string, string>
                {
                    { "Title", "MangaMeeya by Jin - Manga ZIP Viewer" },
                    { "OpenButton", "📁 Open ZIP File" },
                    { "PrevButton", "◀ Prev" },
                    { "NextButton", "Next ▶" },
                    { "FullscreenEnter", "🖥 Fullscreen" },
                    { "FullscreenExit", "🖥 Exit" },
                    { "FileNone", "File: None" },
                    { "FilePrefix", "File: " },
                    { "GestureWheel", "🖱 Wheel/Drag L-R: Prev/Next Page" },
                    { "GestureUp", "🖱 Drag Up: Toggle Fullscreen" },
                    { "GestureDown", "🖱 Drag Down: Exit Fullscreen" },
                    { "ZipFilter", "ZIP Files (*.zip)|*.zip|All Files (*.*)|*.*" },
                    { "OpenTitle", "Select Manga ZIP File" },
                    { "ErrorTitle", "Error" },
                    { "ErrorPrefix", "Error occurred: " },
                    { "ImageLoadFailed", "Failed to load image: " },
                    { "SplashSubtitle", "Manga ZIP Viewer" },
                    { "HistoryButton", "📜 History" },
                    { "HistoryWindowTitle", "📜 Viewing History" },
                    { "HistoryWindowTitleCount", "📜 Viewing History - Total {0}" },
                    { "HistoryEmpty", "📜 Viewing History - No records" },
                    { "HistoryColumnFileName", "File Name" },
                    { "HistoryColumnPage", "Page" },
                    { "HistoryColumnTime", "Last Viewed" },
                    { "HistoryOpenSelected", "📂 Open Selected" },
                    { "HistoryDeleteSelected", "🗑 Delete Selected" },
                    { "HistoryClearAll", "🗑 Clear All" },
                    { "HistoryFileNotFound", "File not found" },
                    { "HistoryFileNotFoundDetail", "The file has been moved or deleted." },
                    { "HistoryClearConfirm", "Delete all viewing history?" },
                    { "HistoryClearTitle", "Clear History" },
                    { "TimeAgoJustNow", "Just now" },
                    { "TimeAgoMinute", "min ago" },
                    { "TimeAgoHour", "hour ago" },
                    { "TimeAgoDay", "day ago" },
                    { "TimeAgoWeek", "week ago" },
                    { "TimeAgoMonth", "month ago" }
                }
            },
            {
                "zh", new Dictionary<string, string>
                {
                    { "Title", "MangaMeeya by Jin - 漫画 ZIP 浏览器" },
                    { "OpenButton", "📁 打开 ZIP 文件" },
                    { "PrevButton", "◀ 上一页" },
                    { "NextButton", "下一页 ▶" },
                    { "FullscreenEnter", "🖥 全屏" },
                    { "FullscreenExit", "🖥 退出" },
                    { "FileNone", "文件: 无" },
                    { "FilePrefix", "文件: " },
                    { "GestureWheel", "🖱 滚轮/左右拖动: 上一页/下一页" },
                    { "GestureUp", "🖱 向上拖动: 切换全屏" },
                    { "GestureDown", "🖱 向下拖动: 退出全屏" },
                    { "ZipFilter", "ZIP 文件 (*.zip)|*.zip|所有文件 (*.*)|*.*" },
                    { "OpenTitle", "选择漫画 ZIP 文件" },
                    { "ErrorTitle", "错误" },
                    { "ErrorPrefix", "发生错误: " },
                    { "ImageLoadFailed", "加载图片失败: " },
                    { "SplashSubtitle", "漫画 ZIP 浏览器" }
                }
            },
            {
                "ja", new Dictionary<string, string>
                {
                    { "Title", "MangaMeeya by Jin - 漫画 ZIP ビューア" },
                    { "OpenButton", "📁 ZIP ファイルを開く" },
                    { "PrevButton", "◀ 前へ" },
                    { "NextButton", "次へ ▶" },
                    { "FullscreenEnter", "🖥 全画面" },
                    { "FullscreenExit", "🖥 終了" },
                    { "FileNone", "ファイル: なし" },
                    { "FilePrefix", "ファイル: " },
                    { "GestureWheel", "🖱 ホイール/左右ドラッグ: 前/次のページ" },
                    { "GestureUp", "🖱 上にドラッグ: 全画面切り替え" },
                    { "GestureDown", "🖱 下にドラッグ: 全画面解除" },
                    { "ZipFilter", "ZIP ファイル (*.zip)|*.zip|すべてのファイル (*.*)|*.*" },
                    { "OpenTitle", "漫画 ZIP ファイルを選択" },
                    { "ErrorTitle", "エラー" },
                    { "ErrorPrefix", "エラーが発生しました: " },
                    { "ImageLoadFailed", "画像の読み込みに失敗しました: " },
                    { "SplashSubtitle", "漫画 ZIP ビューア" }
                }
            }
        };

        public static string GetString(string key)
        {
            if (Translations.TryGetValue(currentLanguage, out var langDict))
            {
                if (langDict.TryGetValue(key, out var translation))
                {
                    return translation;
                }
            }
            // Fallback to English if not found
            if (Translations["en"].TryGetValue(key, out var fallback))
            {
                return fallback;
            }
            return key;
        }
    }
}
