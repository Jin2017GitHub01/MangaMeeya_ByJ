using System.Windows;
using System.Windows.Controls;

namespace MangaMeeya_by_Jin.Services
{
    /// <summary>
    /// UI 텍스트 업데이트 및 다국어 적용을 담당하는 클래스입니다.
    /// </summary>
    public class UIManager
    {
        private readonly Window _window;
        private readonly TextBlock _pageLabel;
        private readonly TextBlock _fileLabel;
        private readonly TextBlock _gestureWheelText;
        private readonly TextBlock _gestureUpText;
        private readonly TextBlock _gestureDownText;

        public UIManager(
            Window window,
            TextBlock pageLabel,
            TextBlock fileLabel,
            TextBlock gestureWheelText,
            TextBlock gestureUpText,
            TextBlock gestureDownText)
        {
            _window = window;
            _pageLabel = pageLabel;
            _fileLabel = fileLabel;
            _gestureWheelText = gestureWheelText;
            _gestureUpText = gestureUpText;
            _gestureDownText = gestureDownText;
        }

        /// <summary>
        /// 페이지 번호와 파일명 레이블을 현재 상태에 맞게 업데이트합니다.
        /// </summary>
        public void UpdatePageInfo(int currentIndex, int totalImages, string currentZipPath)
        {
            if (_pageLabel != null)
                _pageLabel.Text = $"{currentIndex + 1}/{totalImages}";

            if (_fileLabel != null)
            {
                if (string.IsNullOrEmpty(currentZipPath))
                {
                    _fileLabel.Text = LanguageManager.GetString("FileNone");
                }
                else
                {
                    _fileLabel.Text = $"{LanguageManager.GetString("FilePrefix")}{System.IO.Path.GetFileName(currentZipPath)}";
                }
            }
        }

        /// <summary>
        /// 현재 LanguageManager에 설정된 언어로 UI 텍스트를 업데이트합니다.
        /// </summary>
        public void ApplyLanguage(bool isFullscreen, int currentIndex, int totalImages, string currentZipPath)
        {
            _window.Title = LanguageManager.GetString("Title");

            // 버튼 텍스트는 MainWindow에서 직접 접근해야 하므로
            // 이 메서드는 필요한 문자열만 제공하고
            // 호출 측에서 버튼 텍스트를 설정합니다.
        }

        /// <summary>
        /// 제스처 설명 텍스트를 업데이트합니다.
        /// </summary>
        public void UpdateGestureTexts()
        {
            if (_gestureWheelText != null)
                _gestureWheelText.Text = LanguageManager.GetString("GestureWheel");
            if (_gestureUpText != null)
                _gestureUpText.Text = LanguageManager.GetString("GestureUp");
            if (_gestureDownText != null)
                _gestureDownText.Text = LanguageManager.GetString("GestureDown");
        }
    }
}