using System.Windows;
using System.Windows.Controls;

namespace MangaMeeya_by_Jin.Services
{
    /// <summary>
    /// 전체화면 모드 전환 및 상태 관리를 담당하는 클래스입니다.
    /// </summary>
    public class FullscreenManager
    {
        private readonly Window _window;
        private readonly Grid _contentGrid;
        private bool _isFullscreen = false;

        // 전체화면 전환 전 상태 저장 필드
        private WindowStyle _previousWindowStyle;
        private WindowState _previousWindowState;
        private double _previousHeight;
        private double _previousWidth;
        private double _previousTop;
        private double _previousLeft;
        private GridLength _previousBottomPanelHeight;

        public bool IsFullscreen => _isFullscreen;

        public FullscreenManager(Window window, Grid contentGrid)
        {
            _window = window;
            _contentGrid = contentGrid;
        }

        /// <summary>
        /// 전체화면 모드를 토글합니다.
        /// </summary>
        public void Toggle()
        {
            if (_isFullscreen)
                Exit();
            else
                Enter();
        }

        /// <summary>
        /// 전체화면 모드로 전환합니다.
        /// </summary>
        public void Enter()
        {
            _isFullscreen = true;

            // 현재 상태 저장
            _previousWindowStyle = _window.WindowStyle;
            _previousWindowState = _window.WindowState;
            _previousHeight = _window.Height;
            _previousWidth = _window.Width;
            _previousTop = _window.Top;
            _previousLeft = _window.Left;
            _previousBottomPanelHeight = _contentGrid.RowDefinitions[1].Height;

            // 전체화면으로 설정
            _window.WindowStyle = WindowStyle.None;
            _window.WindowState = WindowState.Maximized;

            // 컨트롤 패널 숨기기
            _contentGrid.RowDefinitions[1].Height = new GridLength(0);
        }

        /// <summary>
        /// 전체화면 모드를 해제하고 이전 창 상태로 복원합니다.
        /// </summary>
        public void Exit()
        {
            _isFullscreen = false;

            // 이전 상태 복원
            _window.WindowStyle = _previousWindowStyle;
            _window.WindowState = _previousWindowState;
            _window.Height = _previousHeight;
            _window.Width = _previousWidth;
            _window.Top = _previousTop;
            _window.Left = _previousLeft;

            // 컨트롤 패널 다시 보이기
            _contentGrid.RowDefinitions[1].Height = _previousBottomPanelHeight;
        }
    }
}