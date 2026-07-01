using System.Windows;
using System.Windows.Threading;

namespace MangaMeeya_by_Jin
{
    /// <summary>
    /// 애플리케이션의 주요 동작을 정의하는 App 클래스입니다.
    /// WPF 애플리케이션의 시작 이벤트를 처리합니다.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 애플리케이션이 시작될 때 호출됩니다.
        /// 스플래시 화면을 먼저 표시한 후, 타이머를 사용하여 잠시 후 메인 윈도우를 열고 스플래시 화면을 닫습니다.
        /// </summary>
        /// <param name="e">시작 이벤트 인자</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 스플래시 화면 표시
            SplashWindow splash = new SplashWindow();
            splash.Show();

            // 0.3초 후 메인 윈도우 표시 및 스플래시 화면 닫기
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = System.TimeSpan.FromSeconds(0.3);
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                splash.Close();
            };
            timer.Start();
        }
    }
}