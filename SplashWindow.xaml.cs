using System.Windows;

namespace MangaMeeya_by_Jin
{
    /// <summary>
    /// 애플리케이션 시작 시 잠시 표시되는 스플래시 화면 창입니다.
    /// 메인 윈도우가 로드되는 동안 사용자에게 브랜딩 정보를 보여줍니다.
    /// </summary>
    public partial class SplashWindow : Window
    {
        /// <summary>
        /// SplashWindow 생성자. 컴포넌트를 초기화하고 저장된 언어 설정을 불러와 부제목을 표시합니다.
        /// </summary>
        public SplashWindow()
        {
            InitializeComponent();

            // 저장된 언어 설정을 불러와 스플래시 화면에 적용
            AppSettings settings = AppSettings.Load();
            LanguageManager.CurrentLanguage = settings.Language;
            SubTitleLabel.Text = LanguageManager.GetString("SplashSubtitle");
        }
    }
}