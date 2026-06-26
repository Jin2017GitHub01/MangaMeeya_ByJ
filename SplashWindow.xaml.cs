using System.Windows;

namespace MangaMeeya_by_Jin
{
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
            AppSettings settings = AppSettings.Load();
            LanguageManager.CurrentLanguage = settings.Language;
            SubTitleLabel.Text = LanguageManager.GetString("SplashSubtitle");
        }
    }
}
