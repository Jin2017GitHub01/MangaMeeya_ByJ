using System.Windows;
using System.Windows.Threading;

namespace MangaMeeya_by_Jin
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 스플래시 화면 표시
            SplashWindow splash = new SplashWindow();
            splash.Show();

            // 0.1초 후 메인 윈도우 표시 및 스플래시 화면 닫기
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
