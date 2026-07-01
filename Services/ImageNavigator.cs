using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace MangaMeeya_by_Jin.Services
{
    /// <summary>
    /// 이미지 표시 및 페이지 탐색을 담당하는 클래스입니다.
    /// 페이지 전환 시 현재 페이지가 좌/우로 슬라이드되어 사라지며
    /// 그 아래에 있던 다음 페이지가 드러나는 효과를 제공합니다.
    /// LTR/RTL 읽기 방향을 지원합니다.
    /// </summary>
    public class ImageNavigator
    {
        private readonly Image _mangaImage;    // 현재 페이지 (위, 앞에 있음)
        private readonly Image _mangaImageOld; // 다음 페이지 (아래, 뒤에 있음)
        private readonly ZipFileLoader _zipLoader;
        private int _currentImageIndex = 0;
        private bool _isAnimating = false;
        private string _readingDirection = "LTR"; // "LTR" or "RTL"

        /// <summary>애니메이션 지속 시간 (밀리초)</summary>
        private const double ANIMATION_DURATION_MS = 300;

        public int CurrentIndex => _currentImageIndex;
        public int TotalImages => _zipLoader.ImageCount;

        /// <summary>읽기 방향에 따라 이전 페이지로 이동 가능 여부</summary>
        public bool CanGoPrev => _currentImageIndex > 0;

        /// <summary>읽기 방향에 따라 다음 페이지로 이동 가능 여부</summary>
        public bool CanGoNext => _currentImageIndex < _zipLoader.ImageCount - 1;

        /// <summary>현재 읽기 방향 (LTR 또는 RTL)</summary>
        public string ReadingDirection
        {
            get => _readingDirection;
            set => _readingDirection = value;
        }

        /// <summary>
        /// 이미지가 표시될 때 발생하는 이벤트입니다.
        /// </summary>
        public event Action<int> ImageDisplayed;

        public ImageNavigator(Image mangaImage, Image mangaImageOld, ZipFileLoader zipLoader)
        {
            _mangaImage = mangaImage;
            _mangaImageOld = mangaImageOld;
            _zipLoader = zipLoader;

            // Z-order: MangaImageOld가 아래(뒤), MangaImage가 위(앞)
            // XAML에서 MangaImageOld가 먼저 선언되어 뒤에 위치함
            // MangaImage가 나중에 선언되어 앞에 위치함
            _mangaImageOld.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 읽기 방향에 따라 "이전" 페이지로 이동합니다.
        /// LTR: 인덱스 감소, RTL: 인덱스 증가
        /// </summary>
        public void GoToPrev()
        {
            if (_isAnimating) return;

            if (_readingDirection == "RTL")
            {
                // RTL: "이전" = 다음 페이지
                if (!CanGoNext) return;
                int targetIndex = _currentImageIndex + 1;
                // RTL 이전: 현재 페이지가 오른쪽으로 밀리며 다음 페이지(인덱스+1)가 드러남
                AnimateToPage(targetIndex, slideRight: true);
            }
            else
            {
                // LTR: "이전" = 이전 페이지
                if (!CanGoPrev) return;
                int targetIndex = _currentImageIndex - 1;
                // LTR 이전: 현재 페이지가 오른쪽으로 밀리며 이전 페이지(인덱스-1)가 드러남
                AnimateToPage(targetIndex, slideRight: true);
            }
        }

        /// <summary>
        /// 읽기 방향에 따라 "다음" 페이지로 이동합니다.
        /// LTR: 인덱스 증가, RTL: 인덱스 감소
        /// </summary>
        public void GoToNext()
        {
            if (_isAnimating) return;

            if (_readingDirection == "RTL")
            {
                // RTL: "다음" = 이전 페이지
                if (!CanGoPrev) return;
                int targetIndex = _currentImageIndex - 1;
                // RTL 다음: 현재 페이지가 왼쪽으로 밀리며 이전 페이지(인덱스-1)가 드러남
                AnimateToPage(targetIndex, slideRight: false);
            }
            else
            {
                // LTR: "다음" = 다음 페이지
                if (!CanGoNext) return;
                int targetIndex = _currentImageIndex + 1;
                // LTR 다음: 현재 페이지가 왼쪽으로 밀리며 다음 페이지(인덱스+1)가 드러남
                AnimateToPage(targetIndex, slideRight: false);
            }
        }

        /// <summary>
        /// ZIP 파일에서 MemoryStream으로 이미지를 로드하여 BitmapImage를 생성합니다.
        /// </summary>
        /// <param name="index">이미지 인덱스</param>
        /// <returns>로드된 BitmapImage (실패 시 null)</returns>
        private BitmapImage LoadImageFromZip(int index)
        {
            MemoryStream memStream = _zipLoader.GetImageStream(index);
            if (memStream == null) return null;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memStream; // MemoryStream에서 직접 읽기
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // 즉시 디코드하여 스트림 해제 가능
            bitmapImage.EndInit();
            bitmapImage.Freeze(); // 스레드 안전하게 만들기

            memStream.Close(); // BitmapImage가 이미 데이터를 로드했으므로 스트림 닫기
            return bitmapImage;
        }

        /// <summary>
        /// 지정된 인덱스의 이미지를 화면에 즉시 표시합니다 (애니메이션 없음).
        /// </summary>
        public void DisplayImage(int index)
        {
            if (index < 0 || index >= _zipLoader.ImageCount)
                return;

            try
            {
                // 새 이미지를 MangaImage(앞)에 로드
                var bitmapImage = LoadImageFromZip(index);
                if (bitmapImage == null) return;

                _mangaImage.Source = bitmapImage;
                _mangaImage.RenderTransform = new TranslateTransform();
                _currentImageIndex = index;

                // 뒤에 있는 이미지 숨김
                _mangaImageOld.Visibility = Visibility.Collapsed;
                _mangaImageOld.Source = null;

                ImageDisplayed?.Invoke(index);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LanguageManager.GetString("ImageLoadFailed")}{ex.Message}", LanguageManager.GetString("ErrorTitle"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 현재 페이지가 좌/우로 슬라이드되어 사라지며
        /// 그 아래에 있던 새 페이지가 드러나는 애니메이션을 실행합니다.
        /// </summary>
        /// <param name="targetIndex">이동할 페이지 인덱스</param>
        /// <param name="slideRight">true면 오른쪽으로, false면 왼쪽으로 슬라이드</param>
        private void AnimateToPage(int targetIndex, bool slideRight)
        {
            try
            {
                _isAnimating = true;

                // 1. 새 이미지를 ZIP에서 직접 로드하여 MangaImageOld(뒤)에 설정
                var newBitmap = LoadImageFromZip(targetIndex);
                if (newBitmap == null)
                {
                    _isAnimating = false;
                    return;
                }

                _mangaImageOld.Source = newBitmap;
                _mangaImageOld.Visibility = Visibility.Visible;
                _mangaImageOld.RenderTransform = new TranslateTransform();

                // 2. 현재 페이지(MangaImage, 앞)가 슬라이드되어 사라짐
                double containerWidth = _mangaImage.ActualWidth;
                if (containerWidth <= 0) containerWidth = _mangaImage.Width;
                if (containerWidth <= 0) containerWidth = 1200; // 기본값

                double slideOffset = slideRight ? containerWidth : -containerWidth;

                var slideAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = slideOffset,
                    Duration = TimeSpan.FromMilliseconds(ANIMATION_DURATION_MS),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
                };

                slideAnimation.Completed += (s, e) =>
                {
                    // 3. 애니메이션 완료 후: 뒤에 있던 이미지를 앞으로 가져오고 정리
                    _mangaImage.Source = _mangaImageOld.Source;
                    _mangaImage.RenderTransform = new TranslateTransform();

                    // MangaImageOld(뒤) 정리
                    _mangaImageOld.Visibility = Visibility.Collapsed;
                    _mangaImageOld.Source = null;
                    _mangaImageOld.RenderTransform = new TranslateTransform();

                    _currentImageIndex = targetIndex;
                    _isAnimating = false;

                    ImageDisplayed?.Invoke(targetIndex);
                };

                _mangaImage.RenderTransform = new TranslateTransform();
                _mangaImage.RenderTransform.BeginAnimation(TranslateTransform.XProperty, slideAnimation);
            }
            catch (Exception ex)
            {
                _isAnimating = false;
                MessageBox.Show($"{LanguageManager.GetString("ImageLoadFailed")}{ex.Message}", LanguageManager.GetString("ErrorTitle"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 시작 페이지를 설정하고 해당 페이지를 표시합니다.
        /// </summary>
        /// <param name="startPage">시작할 페이지 인덱스 (null이면 첫 페이지)</param>
        public void Initialize(int? startPage = null)
        {
            int initialPage = 0;
            if (startPage.HasValue && startPage.Value >= 0 && startPage.Value < _zipLoader.ImageCount)
            {
                initialPage = startPage.Value;
            }
            _currentImageIndex = initialPage;

            if (_zipLoader.ImageCount > 0)
            {
                // 초기 로드는 애니메이션 없이 즉시 표시
                var bitmapImage = LoadImageFromZip(initialPage);
                if (bitmapImage == null) return;

                _mangaImage.Source = bitmapImage;
                _mangaImage.RenderTransform = new TranslateTransform();
                _mangaImageOld.Visibility = Visibility.Collapsed;
                _mangaImageOld.Source = null;

                ImageDisplayed?.Invoke(initialPage);
            }
        }
    }
}