using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;

namespace MangaMeeya_by_Jin
{
    public partial class MainWindow : Window
    {
        private List<string> imageFiles = new List<string>();
        private int currentImageIndex = 0;
        private string currentZipPath = "";
        private string tempExtractPath = "";
        private bool isFullscreen = false;
        private WindowStyle previousWindowStyle;
        private WindowState previousWindowState;
        private double previousHeight;
        private double previousWidth;
        private double previousTop;
        private double previousLeft;
        private GridLength previousBottomPanelHeight;
        
        // 마우스 제스처용 변수
        private System.Windows.Point mouseDownPosition;
        private const double GESTURE_THRESHOLD = 50; // 50픽셀 이상 드래그시 인식

        public MainWindow()
        {
            InitializeComponent();
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            this.MouseDown += MainWindow_MouseDown;
            this.MouseUp += MainWindow_MouseUp;
            this.MouseWheel += MainWindow_MouseWheel;
            this.Loaded += MainWindow_Loaded;
        }

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

            ApplyLanguage();

            // 저장된 ZIP 파일이 있으면 자동으로 로드
            if (!string.IsNullOrEmpty(settings.LastZipFilePath) && File.Exists(settings.LastZipFilePath))
            {
                LoadZipFile(settings.LastZipFilePath);
            }
        }

        private void MainWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // F 키로 전체화면 토글
            if (e.Key == System.Windows.Input.Key.F || e.Key == System.Windows.Input.Key.F11)
            {
                ToggleFullscreen();
                e.Handled = true;
            }
            // Escape 키로 전체화면 해제
            else if (e.Key == System.Windows.Input.Key.Escape && isFullscreen)
            {
                ExitFullscreen();
                e.Handled = true;
            }
            // 왼쪽 화살표로 이전 이미지
            else if (e.Key == System.Windows.Input.Key.Left)
            {
                PrevButton_Click(null, null);
                e.Handled = true;
            }
            // 오른쪽 화살표로 다음 이미지
            else if (e.Key == System.Windows.Input.Key.Right)
            {
                NextButton_Click(null, null);
                e.Handled = true;
            }
        }

        private void MainWindow_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // 마우스 다운 위치 기록
            mouseDownPosition = e.GetPosition(this);
        }

        private void MainWindow_MouseUp(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // 마우스 업 위치 가져오기
            System.Windows.Point mouseUpPosition = e.GetPosition(this);
            
            // 수평 및 수직 이동 거리 계산
            double horizontalDrag = mouseUpPosition.X - mouseDownPosition.X;
            double verticalDrag = mouseUpPosition.Y - mouseDownPosition.Y;
            
            // 수직 이동이 수평 이동보다 큰 경우 (세로 제스처 우선)
            if (Math.Abs(verticalDrag) > Math.Abs(horizontalDrag) && Math.Abs(verticalDrag) > GESTURE_THRESHOLD)
            {
                if (verticalDrag < 0)
                {
                    // 위로 드래그 → 전체화면 토글
                    ToggleFullscreen();
                }
                else
                {
                    // 아래로 드래그 → 전체화면 해제
                    if (isFullscreen)
                    {
                        ExitFullscreen();
                    }
                }
            }
            // 수평 이동 (가로 제스처)
            else if (Math.Abs(horizontalDrag) > GESTURE_THRESHOLD)
            {
                if (horizontalDrag > 0)
                {
                    // 오른쪽으로 드래그 → 이전 페이지
                    PrevButton_Click(null, null);
                }
                else
                {
                    // 왼쪽으로 드래그 → 다음 페이지
                    NextButton_Click(null, null);
                }
            }
        }

        private void MainWindow_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            // 휠 위로 스크롤 → 이전 페이지
            if (e.Delta > 0)
            {
                PrevButton_Click(null, null);
            }
            // 휠 아래로 스크롤 → 다음 페이지
            else if (e.Delta < 0)
            {
                NextButton_Click(null, null);
            }
            e.Handled = true;
        }

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

        private void LoadZipFile(string zipPath, int? startPage = null)
        {
            try
            {
                // 기존 임시 폴더 정리
                if (!string.IsNullOrEmpty(tempExtractPath) && Directory.Exists(tempExtractPath))
                {
                    Directory.Delete(tempExtractPath, true);
                }

                currentZipPath = zipPath;
                tempExtractPath = Path.Combine(Path.GetTempPath(), "MangaMeeya_" + Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempExtractPath);

                // ZIP 파일 추출 (모든 이미지를 한 폴더에 평탄화)
                using (ZipFile zipFile = new ZipFile(zipPath))
                {
                    int fileCounter = 0;
                    foreach (ZipEntry entry in zipFile)
                    {
                        if (!entry.IsDirectory && IsImageFile(entry.Name))
                        {
                            // 파일명을 번호로 변경하여 경로 문제 해결
                            string fileExtension = Path.GetExtension(entry.Name);
                            string extractPath = Path.Combine(tempExtractPath, $"{fileCounter:D4}{fileExtension}");
                            
                            using (Stream zipStream = zipFile.GetInputStream(entry))
                            using (FileStream fileStream = new FileStream(extractPath, FileMode.Create))
                            {
                                zipStream.CopyTo(fileStream);
                            }
                            fileCounter++;
                        }
                    }
                }

                // 이미지 파일 검색
                imageFiles = Directory.GetFiles(tempExtractPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => IsImageFile(f))
                    .OrderBy(f => f)
                    .ToList();

                // 시작 페이지 결정
                int initialPage = 0;
                if (startPage.HasValue && startPage.Value >= 0 && startPage.Value < imageFiles.Count)
                {
                    initialPage = startPage.Value;
                }
                currentImageIndex = initialPage;

                if (imageFiles.Count > 0)
                {
                    DisplayImage(initialPage);
                    UpdateUI();
                    // MessageBox.Show($"{imageFiles.Count}개의 이미지를 로드했습니다.", "성공", 
                    //     MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // MessageBox.Show("ZIP 파일에 이미지가 없습니다.", "경고", 
                    //     MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LanguageManager.GetString("ErrorPrefix")}{ex.Message}", LanguageManager.GetString("ErrorTitle"), 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsImageFile(string filePath)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" };
            return imageExtensions.Contains(Path.GetExtension(filePath).ToLower());
        }

        private void DisplayImage(int index)
        {
            if (index < 0 || index >= imageFiles.Count)
                return;

            try
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(imageFiles[index], UriKind.Absolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                MangaImage.Source = bitmapImage;
                currentImageIndex = index;
                // 열람 기록 저장
                if (!string.IsNullOrEmpty(currentZipPath) && imageFiles.Count > 0)
                {
                    HistoryManager.RecordView(currentZipPath, currentImageIndex, imageFiles.Count);
                }

                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LanguageManager.GetString("ImageLoadFailed")}{ex.Message}", LanguageManager.GetString("ErrorTitle"), 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateUI()
        {
            if (PageLabel != null)
                PageLabel.Text = $"{currentImageIndex + 1}/{imageFiles.Count}";
            
            if (FileLabel != null)
            {
                if (string.IsNullOrEmpty(currentZipPath))
                {
                    FileLabel.Text = LanguageManager.GetString("FileNone");
                }
                else
                {
                    FileLabel.Text = $"{LanguageManager.GetString("FilePrefix")}{Path.GetFileName(currentZipPath)}";
                }
            }
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentImageIndex > 0)
            {
                DisplayImage(currentImageIndex - 1);
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentImageIndex < imageFiles.Count - 1)
            {
                DisplayImage(currentImageIndex + 1);
            }
        }

        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullscreen();
        }
        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            HistoryWindow historyWindow = new HistoryWindow();
            historyWindow.FileOpenRequested += (filePath, pageIndex) =>
            {
                // 이미 열려있는 ZIP 파일과 다른 경우에만 로드
                if (currentZipPath != filePath)
                {
                    LoadZipFile(filePath, pageIndex);
                }
                else if (pageIndex.HasValue && pageIndex.Value >= 0 && pageIndex.Value < imageFiles.Count)
                {
                    // 같은 파일이면 해당 페이지로 이동
                    DisplayImage(pageIndex.Value);
                }
            };
            historyWindow.ShowDialog();
        }



        private void ToggleFullscreen()
        {
            if (isFullscreen)
            {
                ExitFullscreen();
            }
            else
            {
                EnterFullscreen();
            }
        }

        private void EnterFullscreen()
        {
            isFullscreen = true;
            
            // 현재 상태 저장
            previousWindowStyle = this.WindowStyle;
            previousWindowState = this.WindowState;
            previousHeight = this.Height;
            previousWidth = this.Width;
            previousTop = this.Top;
            previousLeft = this.Left;
            previousBottomPanelHeight = (this.Content as Grid).RowDefinitions[1].Height;

            // 전체화면으로 설정
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
            
            // 컨트롤 패널 숨기기
            (this.Content as Grid).RowDefinitions[1].Height = new GridLength(0);
            
            // 버튼 텍스트 변경
            FullscreenButton.Content = LanguageManager.GetString("FullscreenExit");
        }

        private void ExitFullscreen()
        {
            isFullscreen = false;
            
            // 이전 상태 복원
            this.WindowStyle = previousWindowStyle;
            this.WindowState = previousWindowState;
            this.Height = previousHeight;
            this.Width = previousWidth;
            this.Top = previousTop;
            this.Left = previousLeft;
            
            // 컨트롤 패널 다시 보이기
            (this.Content as Grid).RowDefinitions[1].Height = previousBottomPanelHeight;
            
            // 버튼 텍스트 변경
            FullscreenButton.Content = LanguageManager.GetString("FullscreenEnter");
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string tag = selectedItem.Tag?.ToString() ?? "ko";
                LanguageManager.CurrentLanguage = tag;
                
                // UI 언어 적용
                ApplyLanguage();
                
                // 설정 저장
                AppSettings settings = AppSettings.Load();
                settings.Language = tag;
                settings.Save();
            }
        }

        private void ApplyLanguage()
        {
            this.Title = LanguageManager.GetString("Title");
            OpenButton.Content = LanguageManager.GetString("OpenButton");
            PrevButton.Content = LanguageManager.GetString("PrevButton");
            NextButton.Content = LanguageManager.GetString("NextButton");
            FullscreenButton.Content = isFullscreen ? LanguageManager.GetString("FullscreenExit") : LanguageManager.GetString("FullscreenEnter");
            
            HistoryButton.Content = LanguageManager.GetString("HistoryButton");

            // 파일 정보 표시 업데이트
            UpdateUI();
            
            // 마우스 제스처 설명서 번역
            if (GestureWheelText != null)
                GestureWheelText.Text = LanguageManager.GetString("GestureWheel");
            if (GestureUpText != null)
                GestureUpText.Text = LanguageManager.GetString("GestureUp");
            if (GestureDownText != null)
                GestureDownText.Text = LanguageManager.GetString("GestureDown");
        }

        protected override void OnClosed(EventArgs e)
        {
            // 현재 ZIP 파일 경로 저장
            if (!string.IsNullOrEmpty(currentZipPath))
            {
                AppSettings settings = new AppSettings
                {
                    LastZipFilePath = currentZipPath,
                    LastModified = DateTime.Now
                };
                settings.Save();
            }

            // 마지막 열람 기록 저장
            if (!string.IsNullOrEmpty(currentZipPath) && imageFiles.Count > 0)
            {
                HistoryManager.RecordView(currentZipPath, currentImageIndex, imageFiles.Count);
            }

            // 임시 폴더 정리
            if (!string.IsNullOrEmpty(tempExtractPath) && Directory.Exists(tempExtractPath))
            {
                try
                {
                    Directory.Delete(tempExtractPath, true);
                }
                catch { }
            }
            base.OnClosed(e);
        }
    }
}
