using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using GMS2GiMiSi.Properties;
using Application = System.Windows.Application;
using Cursors = System.Windows.Input.Cursors;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace GMS2GiMiSi
{
    public partial class MainWindow : Window
    {
        #region "窗口尺寸/拖动窗口"
        // 引用光标资源字典
        private static readonly ResourceDictionary CursorDictionary = new ResourceDictionary();
        private const int WmSyscommand = 0x112;
        private HwndSource _hwndSource;
        private enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) return;
            if (e.OriginalSource is FrameworkElement element && !element.Name.Contains("Resize"))
            {
                Cursor = ((TextBlock)CursorDictionary["CursorPointer"]).Cursor;
            }
        }

        /// <summary>
        /// MainWindow拖动窗口
        /// </summary>
        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var positionUiGrid = e.GetPosition(UiGrid);
            var positionRightGridFrame = e.GetPosition(RightFrame);
            var inUiGrid = false;
            var inRightGridFrame = false;
            var inDedicatedServerFrame = false;
            if (positionUiGrid.X >= 0 && positionUiGrid.X < UiGrid.ActualWidth && positionUiGrid.Y >= 0 && positionUiGrid.Y < UiGrid.ActualHeight)
            {
                inUiGrid = true;
            }
            if (positionRightGridFrame.X >= 0 && positionRightGridFrame.X < RightFrame.ActualWidth && positionRightGridFrame.Y >= 0 && positionRightGridFrame.Y < RightFrame.ActualHeight)
            {
                inRightGridFrame = true;
            }
            // 如果鼠标位置在标题栏内，允许拖动  
            if (e.LeftButton != MouseButtonState.Pressed || (!inUiGrid && !inRightGridFrame && !inDedicatedServerFrame)) return;
            Cursor = Cursors.SizeAll;
            DragMove();
        }

        /// <summary>
        /// 切换鼠标指针为默认状态
        /// </summary>
        private void MainWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        #endregion

        #region 右上角按钮
        /// <summary>
        /// 最小化按钮
        /// </summary>
        private void UI_btn_minimized_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 关闭按钮
        /// </summary>
        private void UI_btn_close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
        }
        #endregion

        #region 自定义Alt+F4 和 屏蔽Alt+Space
        private bool _altDown;
        private bool _ctrlDown;
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                _altDown = true;
            }
            else if (e.SystemKey == Key.LeftCtrl || e.SystemKey == Key.RightCtrl)
            {
                _ctrlDown = true;
            }
            // 调用自定义Alt+F4事件
            else if (e.SystemKey == Key.F4 && _altDown)
            {
                e.Handled = true;
                UI_btn_close_Click(null, null);
            }
            // Alt+Space直接屏蔽
            else if (e.SystemKey == Key.Space && _altDown)
            {
                e.Handled = true;
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                _altDown = false;
            }
            if (e.SystemKey == Key.LeftCtrl || e.SystemKey == Key.RightCtrl)
            {
                _ctrlDown = false;
            }
        }
        #endregion

    }
}
