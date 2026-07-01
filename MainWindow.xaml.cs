using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using MangaMeeya_by_Jin.Services;

namespace MangaMeeya_by_Jin
{
    /// <summary>
    /// 메인 윈도우 클래스입니다.
    /// ZIP 파일에서 이미지를 추출하여 만화를 페이지 단위로 보여주는 뷰어의 핵심 기능을 담당합니다.
    /// 마우스 제스처, 키보드 단축키, 전체화면 모드, 다국어 지원 등을 제공합니다.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ZipFileLoader _zipLoader;
        private readonly ImageNavigator _navigator;
        private readonly FullscreenManager _fullscreenManager;
        private readonly UIManager _uiManager;
        private string _readingDirection = "LTR"; // "LTR" or "RTL"

        /// <summary>마우스 제스처 인식을 위한 마우스 다운 위치</summary>
        private System.Windows.Point _mouseDownPosition;

        /// <summary>마우스 제스처 인식 최소 드래그 거리 (50픽셀)</summary>
        private const double GESTURE_THRESHOLD = 50;

        /// <summary>
        /// MainWindow 생성자. 서비스들을 초기화하고 이벤트 핸들러를 등록합니다.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _zipLoader = new ZipFileLoader();
            _navigator = new ImageNavigator(MangaImage, MangaImageOld, _zipLoader);
            _fullscreenManager = new FullscreenManager(this, (Grid)this.Content);
            _uiManager = new UIManager(this, PageLabel, FileLabel, GestureWheelText, GestureUpText, GestureDownText);

            _navigator.ImageDisplayed += OnImageDisplayed;

            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            this.MouseDown += MainWindow_MouseDown;
            this.MouseUp += MainWindow_MouseUp;
            this.MouseWheel += MainWindow_MouseWheel;
            this.Loaded += MainWindow_Loaded;
        }

        /// <summary>
        /// 윈도우가 로드될 때 호출됩니다.
        /// 저장된 설정(언어, 마지막 ZIP 파일)을 불러와 적용합니다.
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AppSettings settings = AppSettings.Load();

            // 저장된 설정에서 언어 선택
            string savedLang = settings.Language ?? "ko";
            LanguageManager.CurrentLanguage = savedLang;
            foreach (ComboBoxItem item in LanguageComboBox.Items)
            {
                if (item.Tag?.ToString() == savedLang)
                {
                    LanguageComboBox.SelectedItem = item;
                    break;
                }
            }

            // 저장된 읽기 방향 불러오기
            _readingDirection = settings.ReadingDirection ?? "LTR";
            _navigator.ReadingDirection = _readingDirection;

            ApplyLanguage();

            // 저장된 ZIP 파일이 있으면 자동으로 로드
            if (!string.IsNullOrEmpty(settings.LastZipFilePath) && System.IO.File.Exists(settings.LastZipFilePath))
            {
                LoadZipFile(settings.LastZipFilePath);
            }
        }

        /// <summary>
        /// 키보드 키 입력을 처리합니다.
        /// F/F11: 전체화면 토글, Escape: 전체화면 해제,
        /// Left: 이전 페이지, Right: 다음 페이지
        /// </summary>
        private void MainWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F || e.Key == System.Windows.Input.Key.F11)
            {
                ToggleFullscreen();
                e.Handled = true;
            }
            else if (e.Key == System.Windows.Input.Key.Escape && _fullscreenManager.IsFullscreen)
            {
                ExitFullscreen();
                e.Handled = true;
            }
            else if (e.Key == System.Windows.Input.Key.Left)
            {
                _navigator.GoToPrev();
                e.Handled = true;
            }
            else if (e.Key == System.Windows.Input.Key.Right)
            {
                _navigator.GoToNext();
                e.Handled = true;
            }
        }

        /// <summary>
        /// 마우스 버튼을 누를 때 위치를 기록합니다.
        /// </summary>
        private void MainWindow_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _mouseDownPosition = e.GetPosition(this);
        }

        /// <summary>
        /// 마우스 버튼을 뗄 때 드래그 제스처를 인식합니다.
        /// </summary>
        private void MainWindow_MouseUp(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Windows.Point mouseUpPosition = e.GetPosition(this);

            double horizontalDrag = mouseUpPosition.X - _mouseDownPosition.X;
            double verticalDrag = mouseUpPosition.Y - _mouseDownPosition.Y;

            if (Math.Abs(verticalDrag) > Math.Abs(horizontalDrag) && Math.Abs(verticalDrag) > GESTURE_THRESHOLD)
            {
                if (verticalDrag < 0)
                    ToggleFullscreen();
                else if (_fullscreenManager.IsFullscreen)
                    ExitFullscreen();
            }
            else if (Math.Abs(horizontalDrag) > GESTURE_THRESHOLD)
            {
                if (horizontalDrag > 0)
                    _navigator.GoToPrev();
                else
                    _navigator.GoToNext();
            }
        }

        /// <summary>
        /// 마우스 휠 이벤트를 처리합니다.
        /// </summary>
        private void MainWindow_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                _navigator.GoToPrev();
            else if (e.Delta < 0)
                _navigator.GoToNext();
            e.Handled = true;
        }

        /// <summary>
        /// "ZIP 파일 열기" 버튼 클릭 시 파일 선택 대화상자를 열고 선택한 ZIP 파일을 로드합니다.
        /// </summary>
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = LanguageManager.GetString("ZipFilter"),
                Title = LanguageManager.GetString("OpenTitle")
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LoadZipFile(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// ZIP 파일을 열고 이미지 파일들을 추출하여 표시합니다.
        /// </summary>
        private void LoadZipFile(string zipPath, int? startPage = null)
        {
            try
            {
                _zipLoader.LoadZipFile(zipPath);
                _navigator.Initialize(startPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LanguageManager.GetString("ErrorPrefix")}{ex.Message}", LanguageManager.GetString("ErrorTitle"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 이미지가 표시될 때 UI를 업데이트하고 열람 기록을 저장합니다.
        /// </summary>
        private void OnImageDisplayed(int index)
        {
            // 열람 기록 저장
            if (!string.IsNullOrEmpty(_zipLoader.CurrentZipPath) && _zipLoader.ImageCount > 0)
            {
                HistoryManager.RecordView(_zipLoader.CurrentZipPath, index, _zipLoader.ImageCount);
            }

            UpdateUI();
        }

        /// <summary>
        /// 페이지 레이블과 파일 레이블의 텍스트를 현재 상태에 맞게 업데이트합니다.
        /// </summary>
        private void UpdateUI()
        {
            _uiManager.UpdatePageInfo(_navigator.CurrentIndex, _navigator.TotalImages, _zipLoader.CurrentZipPath);
        }

        /// <summary>
        /// "이전" 버튼 클릭 시 이전 페이지로 이동합니다.
        /// </summary>
        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            _navigator.GoToPrev();
        }

        /// <summary>
        /// "다음" 버튼 클릭 시 다음 페이지로 이동합니다.
        /// </summary>
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            _navigator.GoToNext();
        }

        /// <summary>
        /// "전체화면" 버튼 클릭 시 전체화면 모드를 토글합니다.
        /// </summary>
        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullscreen();
        }

        /// <summary>
        /// "읽기 방향 전환" 버튼 클릭 시 LTR/RTL 읽기 방향을 전환합니다.
        /// </summary>
        private void DirectionToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleReadingDirection();
        }

        /// <summary>
        /// 읽기 방향을 LTR과 RTL 사이에서 전환합니다.
        /// </summary>
        private void ToggleReadingDirection()
        {
            if (_readingDirection == "LTR")
                _readingDirection = "RTL";
            else
                _readingDirection = "LTR";

            // 이미지 네비게이터에도 읽기 방향 전달
            _navigator.ReadingDirection = _readingDirection;

            // 설정 저장
            AppSettings settings = AppSettings.Load();
            settings.ReadingDirection = _readingDirection;
            settings.Save();

            UpdateDirectionButtonText();
        }

        /// <summary>
        /// 읽기 방향 버튼의 텍스트를 현재 방향에 맞게 업데이트합니다.
        /// </summary>
        private void UpdateDirectionButtonText()
        {
            if (DirectionToggleButton == null) return;
            DirectionToggleButton.Content = _readingDirection == "LTR"
                ? LanguageManager.GetString("ReadingDirectionLTR")
                : LanguageManager.GetString("ReadingDirectionRTL");
        }

        /// <summary>
        /// "기록" 버튼 클릭 시 HistoryWindow를 엽니다.
        /// </summary>
        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            HistoryWindow historyWindow = new HistoryWindow();
            historyWindow.FileOpenRequested += (filePath, pageIndex) =>
            {
                if (_zipLoader.CurrentZipPath != filePath)
                {
                    LoadZipFile(filePath, pageIndex);
                }
                else if (pageIndex.HasValue && pageIndex.Value >= 0 && pageIndex.Value < _zipLoader.ImageCount)
                {
                    _navigator.DisplayImage(pageIndex.Value);
                }
            };
            historyWindow.ShowDialog();
        }

        /// <summary>
        /// 전체화면 모드를 토글합니다.
        /// </summary>
        private void ToggleFullscreen()
        {
            _fullscreenManager.Toggle();
            UpdateFullscreenButtonText();
        }

        /// <summary>
        /// 전체화면 모드를 해제합니다.
        /// </summary>
        private void ExitFullscreen()
        {
            _fullscreenManager.Exit();
            UpdateFullscreenButtonText();
        }

        /// <summary>
        /// 전체화면 버튼의 텍스트를 현재 상태에 맞게 업데이트합니다.
        /// </summary>
        private void UpdateFullscreenButtonText()
        {
            FullscreenButton.Content = _fullscreenManager.IsFullscreen
                ? LanguageManager.GetString("FullscreenExit")
                : LanguageManager.GetString("FullscreenEnter");
        }

        /// <summary>
        /// 언어 선택 ComboBox의 선택이 변경될 때 호출됩니다.
        /// </summary>
        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string tag = selectedItem.Tag?.ToString() ?? "ko";
                LanguageManager.CurrentLanguage = tag;

                ApplyLanguage();

                AppSettings settings = AppSettings.Load();
                settings.Language = tag;
                settings.Save();
            }
        }

        /// <summary>
        /// 현재 LanguageManager에 설정된 언어로 UI 텍스트를 업데이트합니다.
        /// </summary>
        private void ApplyLanguage()
        {
            this.Title = LanguageManager.GetString("Title");
            OpenButton.Content = LanguageManager.GetString("OpenButton");
            PrevButton.Content = LanguageManager.GetString("PrevButton");
            NextButton.Content = LanguageManager.GetString("NextButton");
            FullscreenButton.Content = _fullscreenManager.IsFullscreen
                ? LanguageManager.GetString("FullscreenExit")
                : LanguageManager.GetString("FullscreenEnter");
            HistoryButton.Content = LanguageManager.GetString("HistoryButton");

            UpdateDirectionButtonText();
            _uiManager.UpdateGestureTexts();
            UpdateUI();
        }

        /// <summary>
        /// 윈도우가 닫힐 때 호출됩니다.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            // 현재 ZIP 파일 경로 저장
            if (!string.IsNullOrEmpty(_zipLoader.CurrentZipPath))
            {
                AppSettings settings = AppSettings.Load();
                settings.LastZipFilePath = _zipLoader.CurrentZipPath;
                settings.LastModified = DateTime.Now;
                settings.Save();
            }

            // 마지막 열람 기록 저장
            if (!string.IsNullOrEmpty(_zipLoader.CurrentZipPath) && _zipLoader.ImageCount > 0)
            {
                HistoryManager.RecordView(_zipLoader.CurrentZipPath, _navigator.CurrentIndex, _zipLoader.ImageCount);
            }

            // ZIP 파일 핸들 정리
            _zipLoader.Cleanup();

            base.OnClosed(e);
        }
    }
}