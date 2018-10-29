using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.Forms.MessageBox;

namespace GMS2GiMiSi.View.GMS2ChildPage
{
    /// <summary>
    /// IDEPage.xaml 的交互逻辑
    /// </summary>
    public partial class IDEPage : Page
    {
        public IDEPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 安装目录变更时
        /// </summary>
        private void TextInstallDir_Changed(object sender, TextChangedEventArgs e)
        {
            GroupBoxFont.IsEnabled = false;
            string path = TextInstallDir.Text;
            if (path == strInstallDirNotFound || path == "")
            {
                LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                LabelPathWarning.Text = strWarningMissingPath;
                EnableInstallation(false);
            }
            else if (!PathIsValid(path))
            {
                LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                LabelPathWarning.Text = strWarningInvalidPath;
                TextGMS2Verion.Text = TextInstallDir.Text.Contains(@"common\GameMaker Studio 2") ? "Steam版" : "官网下载版";
                EnableInstallation(false);
            }
            else
            {
                LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                LabelPathWarning.Text = string.Empty;
                try
                {
                    VerifyPath(path);
                    FileVersionInfo fileVer = FileVersionInfo.GetVersionInfo(path + @"\GameMakerStudio.exe");
                    new Version(fileVer.ProductVersion);
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
        /// 验证路径
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>验证成功</returns>
        private static bool VerifyPath(string path)
        {
            string languagePath = path + @"\Languages";
            string configpath = path + @"\GameMakerStudio.exe.config";
            string exepath = path + @"\GameMakerStudio.exe";
            string actpath = path + @"\DnDLibs\YoYo Games\Languages";
            if (Directory.Exists(path))
            {
                if (Directory.Exists(languagePath) && Directory.Exists(actpath))
                {
                    if (File.Exists(configpath))
                    {
                        if (File.Exists(exepath))
                        {
                            return true;
                        }
                        else
                        {
                            throw new VerifyMissingExecutableException();
                            //return false;
                        }
                    }
                    else
                    {
                        throw new VerifyMissingConfigException();
                        //return false;
                    }
                }
                else
                {
                    throw new VerifyMissingLangDirException();
                    //return false;
                }
            }
            else
            {
                throw new VerifyMissingDirException();
            }
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
                TextGMS2Verion.Text = TextInstallDir.Text.Contains(@"common\GameMaker Studio 2") ? "Steam版" : "官网下载版";
                LoadFont();
            }
        }

        /// <summary>
        /// 自动查找复选框勾选
        /// </summary>
        private void ChBoxAutoSearch_Checked(object sender, RoutedEventArgs e)
        {
            BtnInstallDirBrowse.IsEnabled = false;
            try
            {
                TextInstallDir.Text = GetAutoSearchPath();
                TextGMS2Verion.Text = TextInstallDir.Text.Contains(@"common\GameMaker Studio 2") ? "Steam版" : "官网下载版";
                // 加载字体
                LoadFont();
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
            if (GroupBoxFont.Header.ToString() == "字体加载中...")
            {
                MessageBox.Show("请等待字体加载完毕", "警告");
                CheckBoxAutoSearch.IsChecked = true;
                GroupBoxFont.IsEnabled = false;
                return;
            }
            TextInstallDir.Text = strInstallDirNotFound;
            TextGMS2Verion.Text = "";
            BtnInstallDirBrowse.IsEnabled = true;
            EnableInstallation(false);
        }

        /// <summary>
        /// 安装汉化界面文件
        /// </summary>
        private void BtnInstallCHN_Click(object sender, RoutedEventArgs e)
        {
            if (GMS2ProcessIsRun())
            {
                System.Windows.MessageBox.Show("检测到 GameMaker Studio 2 进程，请关闭程序后进行汉化操作！", "警告");
                return;
            }
            CopyTransFileAsync();
            MessageBox.Show("翻译内容已注入完毕\r\n请做以下操作：\r\n" +
                                                 "①启动软件打开偏好设置（File-Preferences）\r\n" +
                                                 "②选择 General Settings，在右侧的IDE Language选项中选择 “Chinese / 简体中文”并点击左下角的Apply\r\n" +
                                                 "如有发生乱码问题，请更新后重试或联系QQ群或作者。", "翻译完成");
        }

        /// <summary>
        /// 浏览中文帮助文档
        /// </summary>
        private void BtnManualCHN_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://gamemakerchina.github.io/GMS2_manual_en2ch/");
        }

        /// <summary>
        /// 浏览英文帮助文档
        /// </summary>
        private void BtnManualENG_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://docs2.yoyogames.com/");
        }

    }
}
