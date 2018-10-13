using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Deployment.Application;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Web;
using System.Text.RegularExpressions;

using Microsoft.Win32;
using Button = System.Windows.Controls.Button;
using ComboBox = System.Windows.Controls.ComboBox;

namespace GMS2TranslationFileInstaller
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>

    public partial class MainWindow : Window
    {
        /// <summary>
        /// 版本[Standalone, Steam]
        /// </summary>
        public enum Edition
        {
            Standalone,
            Steam
        }

        /// <summary>
        /// 程序版本
        /// </summary>
        private Version programVersion;
        
        /// <summary>
        /// 版本号
        /// </summary>
        private readonly Version version = new Version(System.Windows.Forms.Application.ProductVersion);
        
        /*
        /// <summary>
        /// 侧边栏是否展开
        /// </summary>
        private bool IsExpanded => GrdUpdateSection.Visibility == Visibility.Visible;
        */

        #region 控件行为代码

        public MainWindow()
        {
            webClient.DownloadFileCompleted += WebC_DownloadCompleted;
            webClient.DownloadProgressChanged += WebC_ProgChanged;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            VersionDisplay.Text = String.Format(VersionDisplay.Text, version); // 该软件版本
            TextInstallDir.Text = strInstallDirNotFound;
        }

        private void ChBoxAutoSearch_Checked(object sender, RoutedEventArgs e)
        {
            BtnInstallDirBrowse.IsEnabled = false;
            try
            {
                TextInstallDir.Text = GetAutoSearchPath();
                EnableInstallation(true);
            }
            catch (IOException)
            {
                TextInstallDir.Text = strInstallDirNotFound;
                EnableInstallation(false);
                System.Windows.Forms.MessageBox.Show("自动查找未能找到 GameMaker Studio 2 的安装位置，请检查安装路径或取消勾选自动查找并尝试手动查找", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            //SnapToProperVersion();
        }

        /// <summary>
        /// 自动查找复选框取消勾选
        /// </summary>
        private void ChBoxAutoSearch_Unchecked(object sender, RoutedEventArgs e)
        {
            TextInstallDir.Text = strInstallDirNotFound;
            BtnInstallDirBrowse.IsEnabled = true;
            EnableInstallation(false);
        }

        /// <summary>
        /// Steam复选框勾选
        /// </summary>
        private void ChBoxSteam_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                TextInstallDir.Text = GetAutoSearchPath();
            }
            catch (IOException)
            {
                TextInstallDir.Text = strInstallDirNotFound;
                System.Windows.Forms.MessageBox.Show("自动查找未能找到 GameMaker Studio 2 Steam 版的安装位置，请检查安装路径或取消勾选自动查找并尝试手动查找", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Steam复选框取消勾选
        /// </summary>
        private void ChBoxSteam_Unchecked(object sender, RoutedEventArgs e)
        {
            TextInstallDir.Text = GetAutoSearchPath();
        }

        /// <summary>
        /// 手动选择安装目录
        /// </summary>
        private void BtnInstallDir_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dial = new FolderBrowserDialog
            {
                Description = strBrowseDirectoryPrompt
            };
            if (dial.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextInstallDir.Text = dial.SelectedPath;
            }
        }

        /// <summary>
        /// 安装目录变更时
        /// </summary>
        private void TextInstallDir_Changed(object sender, TextChangedEventArgs e)
        {
            string path = TextInstallDir.Text;
            if (path == strInstallDirNotFound || path == "")
            {
                LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                LabelPathWarning.Text = strWarningMissingPath;
                BtnInstallCHN.IsEnabled = false;
                BtnRepairENG.IsEnabled = false;
            }
            else if (!PathIsValid(path))
            {
                LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                LabelPathWarning.Text = strWarningInvalidPath;
                BtnInstallCHN.IsEnabled = false;
                BtnRepairENG.IsEnabled = false;
            }
            else
            {
                LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                LabelPathWarning.Text = string.Empty;
                try
                {
                    VerifyPath(path);
                    FileVersionInfo fileVer = FileVersionInfo.GetVersionInfo(path + @"\GameMakerStudio.exe");
                    programVersion = new Version(fileVer.ProductVersion);
                    EnableInstallation(true);
                }
                catch (VerifyMissingDirException)
                {
                    LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    LabelPathWarning.Text = strWarningMissingDirectory;
                    EnableInstallation(false);

                }
                catch (VerifyMissingLangDirException)
                {
                    LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                    LabelPathWarning.Text = strWarningBrokenDirectory;
                    EnableInstallation(false);
                }
                catch (VerifyMissingExecutableException)
                {
                    LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                    LabelPathWarning.Text = strWarningBrokenDirectory;
                    EnableInstallation(false);
                }
                catch (VerifyMissingConfigException)
                {
                    LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(0, 128, 0));
                    LabelPathWarning.Text = strWarningBrokenGMS2;
                    FileVersionInfo fileVer = FileVersionInfo.GetVersionInfo(path + @"\GameMakerStudio.exe");
                    EnableInstallation(true);
                }
            }
        }

        /// <summary>
        /// GameMake 开发者之家链接
        /// </summary>
        private void GMCN_Link(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.52gmk.com/");
        }

        /// <summary>
        /// 修复英文界面文件
        /// </summary>
        private void BtnRepairENG_Click(object sender, RoutedEventArgs e)
        {
            CopyOrigFile();
            System.Windows.Forms.MessageBox.Show("原文内容已修复完毕，如果 GameMaker Studio 2 已经处于启动状态，请关闭后重新打开，如有发生乱码问题，请更新后重试或联系QQ群或作者。");
        }

        /// <summary>
        /// 安装汉化界面文件
        /// </summary>
        private void BtnInstallCHN_Click(object sender, RoutedEventArgs e)
        {
            CopyTransFileAsync();
            System.Windows.Forms.MessageBox.Show("翻译内容已注入完毕，请重启软件并打开偏好设置（File-Preferences），选择General Settings，在右侧的IDE Language选项中选择 “Chinese / 简体中文”并点击左下角的Apply，如有发生乱码问题，请更新后重试或联系QQ群或作者。", "翻译完成");
        }
        
        /// <summary>
        /// 安装汉化动作概述文件（未完成）
        /// </summary>
        private void BtnActOvInstallCHN_Click(object sender, RoutedEventArgs e)
        {
            ShowPromptNotImplement();
        }

        /// <summary>
        /// 修复英文动作概述文件（未完成）
        /// </summary>
        private void BtnActOvRepairENG_Click(object sender, RoutedEventArgs e)
        {
            ShowPromptNotImplement();
        }

        private void TextAnswer_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //TextAnswer.Foreground = new SolidColorBrush(Color.FromRgb())
        }
        
        private void Link2Code_Click(object sender, RoutedEventArgs e)
        {
            Process.Start((sender as Hyperlink)?.NavigateUri.AbsoluteUri ?? throw new InvalidOperationException());
        }
        
        #endregion

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
