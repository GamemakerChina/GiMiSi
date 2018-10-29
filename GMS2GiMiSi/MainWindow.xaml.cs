using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Mime;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Windows.Interop;
using GMS2GiMiSi.Properties;
using GMS2GiMiSi.View;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;

namespace GMS2GiMiSi
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>

    public partial class MainWindow : Window
    {
        /// <summary>
        /// 是否加载字体
        /// </summary>
        public static bool isLoadFont;

        /// <summary>
        /// 版本[Standalone, Steam]
        /// </summary>
        public enum Edition
        {
            Standalone,
            Steam
        }

        /// <summary>
        /// 版本号
        /// </summary>
        private readonly Version version = new Version(System.Windows.Forms.Application.ProductVersion);

        #region 控件行为代码

        public MainWindow()
        {
            InitializeComponent();
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChangedHandler;
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompletedHandler;
            // 窗口缩放
            SourceInitialized += delegate (object sender, EventArgs e) { _hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource; };
            MouseMove += Window_MouseMove;
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            VersionDisplay.Text = String.Format(VersionDisplay.Text, version); // 该软件版本
            ComboBoxFont.SelectedIndex = 0;
            LoadInstalledRuntime();
            RightFrame.Visibility = Visibility.Visible;
            RightFrame.NavigationService.Navigate(new GMS2Page());
        }

        /// <summary>
        /// GameMake 开发者之家链接
        /// </summary>
        private void GMCN_Link(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.52gmk.com/");
        }

        private void TextAnswer_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //TextAnswer.Foreground = new SolidColorBrush(Color.FromRgb())
        }

        /// <summary>
        /// 源码页面
        /// </summary>
        private void Link2Code_Click(object sender, RoutedEventArgs e)
        {
            Process.Start((sender as Hyperlink)?.NavigateUri.AbsoluteUri ?? throw new InvalidOperationException());
        }

        #endregion

        /// <summary>
        /// 安装按钮状态
        /// </summary>
        /// <param name="flag">状态值</param>
        private void EnableInstallation(bool flag)
        {
            BtnInstallCHN.IsEnabled = flag;
            BtnStartGMS2.IsEnabled = flag;
            //GroupBoxFont.IsEnabled = flag;
            //BtnRepairENG.IsEnabled = flag;
        }

        private void ShowPromptNotImplement()
        {
            MessageBox.Show("501 Not Implemented:\n    非常抱歉，该功能正在上线中，敬请期待！", "Coming soon！", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

    }
}

#region 异常类

public class VerifyMissingConfigException : Exception
{

}

public class VerifyMissingExecutableException : Exception
{

}

public class VerifyMissingLangDirException : Exception
{

}

public class VerifyMissingDirException : Exception
{

}

public class LocatingFailedException : Exception
{

}

#endregion
