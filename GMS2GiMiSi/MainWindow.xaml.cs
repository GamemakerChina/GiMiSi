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
using GMS2GiMiSi.Class;
using GMS2GiMiSi.Properties;
using GMS2GiMiSi.View;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;
using Version = GMS2GiMiSi.Class.Version;

namespace GMS2GiMiSi
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>

    public partial class MainWindow : Window
    {
        #region 控件行为代码

        public MainWindow()
        {
            InitializeComponent();
            // 窗口缩放
            SourceInitialized += delegate (object sender, EventArgs e) { _hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource; };
            MouseMove += Window_MouseMove;
            Global.MainWindow = this;
            Global.DownloadFileName = DownloadFileName;
            Global.ProgressBarDownload = ProgressBarDownload;
            Global.DownloadRowDefinition = DownloadRowDefinition;
            Network.webClient.DownloadProgressChanged += Network.WebClient_DownloadProgressChangedHandler;
            Network.webClient.DownloadFileCompleted += Network.WebClient_DownloadFileCompletedHandler;
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.NavigationService.Navigate(new GMS2Page());
        }

        #endregion

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
