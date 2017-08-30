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

namespace GMS2TranslationFileInstaller
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>

    public partial class MainWindow : Window
    {

        private Version progVer = null;
        private List<Version> verList = new List<Version>();
        private Version version = new Version(System.Windows.Forms.Application.ProductVersion.ToString());

        private Version thresVer = new Version(2, 0, 6, 146);
        private Version ProperVersion
        {
            get
            {

                for (int i = 0; i < ComBoxVerSelector.Items.Count; i++)
                {
                    if (progVer == (Version)ComBoxVerSelector.Items[i])
                    {
                        return ComBoxVerSelector.Items[i] as Version;

                    }
                    else
                    {
                        if (progVer > (Version)ComBoxVerSelector.Items[i] && (i == ComBoxVerSelector.Items.Count - 1 || progVer < (Version)ComBoxVerSelector.Items[i + 1]))
                        {
                            return ComBoxVerSelector.Items[i] as Version;
                        }
                    }
                }
                return null;
            }
        }
        private bool IsExpanded
        {
            get
            {
                return GrdUpdateSection.Visibility == Visibility.Visible;
            }
        }
            

        private DirectoryInfo dirInf = new DirectoryInfo(System.Windows.Forms.Application.StartupPath+@"\vers");

        #region 控件行为代码


        public MainWindow()
        {
            webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(WebC_DownloadCompleted);
            InitializeComponent();
        }

        private void Window_Loaded(object sender,EventArgs e)
        {
            LabelPathWarning.Text = string.Empty;
            ChBoxAutoSearch.IsChecked = true;
            foreach (var d in dirInf.GetDirectories())
            {
                Version ver = new Version(d.Name.Replace('_', '.'));
                verList.Add(ver);
            }
            foreach (var v in verList)
            {
                ComBoxVerSelector.Items.Add(v);
            }
            ComBoxVerSelector.SelectedItem = progVer;
            ToolTipService.SetShowDuration(TextAnswer, 10000);
            WindowCollapse();
            VersionDisplay.Text = String.Format(VersionDisplay.Text,version);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string strVer = ComBoxVerSelector.SelectedItem.ToString().Replace('_','.');

            Version selectedVer = (sender as System.Windows.Controls.ComboBox).SelectedValue as Version;
            Color c;
            if(selectedVer == progVer)
            {
                c = Color.FromRgb(0, 0, 0);
            }
            else
            {
                if(selectedVer == ProperVersion)
                {
                    c = Color.FromRgb(0, 255, 0);
                }
                else
                {
                    c = Color.FromRgb(255, 0, 0);
                }
            }
            (sender as System.Windows.Controls.ComboBox).Foreground = new SolidColorBrush(c);
        }

        private void ChBoxAutoSearch_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\GameMakerStudio2");
                TextInstallDir.Text = key.GetValue("Install_Dir").ToString();
                TextInstallDir.IsEnabled = false;
                BtnInstallDirBrowse.IsEnabled = false;
                ComBoxVerSelector.SelectedItem = progVer;
                //SnapToProperVersion();
                key.Close();
            }
            catch (System.IO.IOException)
            {
                TextInstallDir.Text = strInstallDirNotFound;
                System.Windows.Forms.MessageBox.Show("自动查找未能找到GameMaker Studio 2的安装位置，请检查安装路径或尝试手动查找");
                (sender as System.Windows.Controls.CheckBox).IsChecked = false;
            }
        }

        private void ChBoxAutoSearch_Unchecked(object sender, RoutedEventArgs e)
        {
            BtnInstallDirBrowse.IsEnabled = true;
            TextInstallDir.IsEnabled = true;
        }

        private void BtnInstallDir_Click(object sender,RoutedEventArgs e)
        {
            FolderBrowserDialog dial = new FolderBrowserDialog();
            dial.Description = strBrowseDirectoryPrompt;
            if(dial.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextInstallDir.Text = dial.SelectedPath;
            }
        }

        private void TextInstallDir_Changed(object sender, TextChangedEventArgs e)
        {
            string path = (sender as TextBlock).Text;
            if (path == strInstallDirNotFound||path == "")
            {
                LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(255,0,0));
                LabelPathWarning.Text = strWarningMissingPath;
                ComBoxVerSelector.IsEnabled = false;
                BtnInstallCHN.IsEnabled = false;
                BtnRepairENG.IsEnabled = false;
            }
            else if(!PathIsValid(path))
            {
                LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                LabelPathWarning.Text = strWarningInvalidPath;
                ComBoxVerSelector.IsEnabled = false;
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
                    ComBoxVerSelector.IsEnabled = true;
                    progVer = new Version(fileVer.ProductVersion);
                    EnableInstallation(true);
                }
                catch(VerifyMissingDirException)
                {
                    LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    LabelPathWarning.Text = strWarningMissingDirectory;
                    ComBoxVerSelector.IsEnabled = false;
                    EnableInstallation(false);

                }
                catch (VerifyMissingLangDirException)
                {
                    LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                    LabelPathWarning.Text = strWarningBrokenDirectory;
                    ComBoxVerSelector.IsEnabled = false;
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
                    ComBoxVerSelector.SelectedItem = new Version(fileVer.ProductVersion);
                    EnableInstallation(true);
                }
            }
        }

        private void GMCN_Link(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.gamemaker.cn/");
        }

        private void BtnRepairENG_Click(object sender, RoutedEventArgs e)
        {
            if (!(progVer == (Version)ComBoxVerSelector.SelectedItem))
            {
                if (System.Windows.Forms.MessageBox.Show("版本号与检测到的版本号不一致，执意进行安装可能会导致兼容性问题，您确定要继续安装吗？", "版本一致性警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
                {
                    CopyOrigFile();
                }
            }
            else
            {
                //System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.None;
                if(ProperVersion < thresVer)
                {
                    if(System.Windows.Forms.MessageBox.Show("由于版本历史原因，修复原文将会替换掉之前的译文，请问是否继续？","历史版本警告",MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                    {
                        return;
                    }
                }
                CopyOrigFile();
                System.Windows.Forms.MessageBox.Show("原文内容已修复完毕，如果GameMaker Studio 2已经处于启动状态，请关闭后重新打开，如有发生乱码问题，请更新后重试或联系QQ群或作者。");
            }

        }

        private void BtnInstallCHN_Click(object sender, RoutedEventArgs e)
        {
            if (!(progVer==(Version)ComBoxVerSelector.SelectedItem))
            {
                if(System.Windows.Forms.MessageBox.Show("版本号与检测到的版本号不一致，执意进行安装可能会导致兼容性问题，您确定要继续安装吗？", "版本一致性警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)==System.Windows.Forms.DialogResult.OK)
                {
                    CopyTransFile();
                }
            }
            else
            {
                if (ProperVersion < thresVer)
                {
                    if (System.Windows.Forms.MessageBox.Show("由于版本历史原因，注入译文将会替换掉之前的原文，请问是否继续？", "历史版本警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                    {
                        return;
                    }
                }
                CopyTransFile();
                System.Windows.Forms.MessageBox.Show("翻译内容已注入完毕，如果GameMaker Studio 2已经处于启动状态，请关闭后重新打开，如有发生乱码问题，请更新后重试或联系QQ群或作者。","翻译完成");
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show("离线体验版暂不支持在线更新~~Sorry","体验版提醒",MessageBoxButtons.OK,MessageBoxIcon.Information);
            WindowExpand();
            (sender as System.Windows.Controls.Button).IsEnabled = false;
        }

        private void BtnActOvInstallCHN_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnActOvRepairENG_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextAnswer_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //TextAnswer.Foreground = new SolidColorBrush(Color.FromRgb())
        }

        private void BtnUpdateCollapse_Click(object sender, RoutedEventArgs e)
        {
            BtnUpdate.IsEnabled = true;
            WindowCollapse();
        }

        private void Link2Code_Click(object sender, RoutedEventArgs e)
        {
            Process.Start((sender as Hyperlink).NavigateUri.AbsoluteUri);
        }

        private void BtnUpdateControl_Click(object sender, RoutedEventArgs e)
        {
            DownloadUpdate();
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

public enum PathState
{
    Absolute,
    Relative,
    Invalid
}