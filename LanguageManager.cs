using System.Collections.Generic;

namespace MangaMeeya_by_Jin
{
    /// <summary>
    /// 다국어 지원을 위한 언어 관리자 정적 클래스입니다.
    /// 한국어(ko), 영어(en), 중국어(zh), 일본어(ja)를 지원하며,
    /// 현재 선택된 언어에 따라 UI 문자열을 반환합니다.
    /// </summary>
    public static class LanguageManager
    {
        /// <summary>현재 선택된 언어 코드 (기본값: "ko")</summary>
        private static string currentLanguage = "ko";

        /// <summary>
        /// 현재 언어 코드를 가져오거나 설정합니다.
        /// 설정 가능한 값: "ko", "en", "zh", "ja"
        /// </summary>
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

        /// <summary>
        /// 각 언어별 번역 문자열을 저장하는 딕셔너리입니다.
        /// 키: 언어 코드 (ko/en/zh/ja)
        /// 값: 키-값 쌍의 번역 딕셔너리
        /// </summary>
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
                    { "TimeAgoMonth", "개월 전" },
                    { "ReadingDirectionLTR", "왼←오 읽기" },
                    { "ReadingDirectionRTL", "오→왼 읽기" },
                    { "DirectionToggle", "📖 방향 전환" }
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
                    { "TimeAgoMonth", "month ago" },
                    { "ReadingDirectionLTR", "← LTR Reading" },
                    { "ReadingDirectionRTL", "RTL Reading →" },
                    { "DirectionToggle", "📖 Toggle Direction" }
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
                    { "SplashSubtitle", "漫画 ZIP 浏览器" },
                    { "ReadingDirectionLTR", "← 左→右阅读" },
                    { "ReadingDirectionRTL", "右→左阅读 →" },
                    { "DirectionToggle", "📖 切换方向" }
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
                    { "SplashSubtitle", "漫画 ZIP ビューア" },
                    { "ReadingDirectionLTR", "← 左→右読み" },
                    { "ReadingDirectionRTL", "右→左読み →" },
                    { "DirectionToggle", "📖 方向切替" }
                }
            }
        };

        /// <summary>
        /// 현재 언어에 해당하는 번역 문자열을 반환합니다.
        /// 현재 언어에 해당하는 키가 없으면 영어(en)를 기본값으로 찾고,
        /// 그래도 없으면 키 문자열을 그대로 반환합니다.
        /// </summary>
        /// <param name="key">번역 키</param>
        /// <returns>번역된 문자열</returns>
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