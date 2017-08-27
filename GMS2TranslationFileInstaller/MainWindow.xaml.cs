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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        private Version thresVer = new Version(2,0,6,146);
        

        private const string strInstallDirNotFound = "<!未找到GameMaker Studio 2的路径>";
        private const string strBrowseDirectoryPrompt = "请选择GameMaker Studio 2的安装目录";
        private const string strWarningMissingPath = "请选择GameMaker Studio 2的安装目录";
        private const string strWarningInvalidPath = "文件路径不合法，可能包含无效字符";
        private const string strWarningBrokenDirectory = "该目录下没有安装GameMaker Studio 2或已损坏";

        private ListItem item = new ListItem();

        private DirectoryInfo dirInf = new DirectoryInfo(System.Windows.Forms.Application.StartupPath+@"\vers");

        private bool VerifyPath(string path)
        {
            string langpath = path + @"\Languages";
            string configpath = path + @"\GameMakerStudio.exe.config";
            string exepath = path + @"\GameMakerStudio.exe";
            if (Directory.Exists(langpath))
            {
                if(File.Exists(configpath))
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

        private bool PathIsValid(string path)
        {
            Regex reg = new Regex(@"^([a-zA-Z]:\\)?[^\/\:\*\?\""\<\>\|\,]+$");
            return reg.IsMatch(path);
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender,EventArgs e)
        {
            LabelPathWarning.Content = string.Empty;
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
            //ComBoxVerSelector.SelectedValue = progVer;
            //object selection = progVer;
            //ComBoxVerSelector.SelectedItem = selection;
            
            //SnapToProperVersion();
            ComBoxVerSelector.SelectedItem = progVer;
        }

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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string strVer = ComBoxVerSelector.SelectedItem.ToString().Replace('_','.');

            Version selectedVer = ComBoxVerSelector.SelectedValue as Version;
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
            ComBoxVerSelector.Foreground = new SolidColorBrush(c);
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
                ChBoxAutoSearch.IsChecked = false;
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
            string path = TextInstallDir.Text;
            if (path == strInstallDirNotFound||path == "")
            {
                LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(255,0,0));
                LabelPathWarning.Content = strWarningMissingPath;
                //ComBoxVerSelector.Text = "无法找到版本";
                ComBoxVerSelector.IsEnabled = false;
                BtnInstallCHN.IsEnabled = false;
                BtnRepairENG.IsEnabled = false;
            }
            else if(!PathIsValid(path))
            {
                LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                LabelPathWarning.Content = strWarningInvalidPath;
                //ComBoxVerSelector.Text = "无法找到版本";
                ComBoxVerSelector.IsEnabled = false;
                BtnInstallCHN.IsEnabled = false;
                BtnRepairENG.IsEnabled = false;
            }
            else
            {
                LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                LabelPathWarning.Content = string.Empty;
                try
                {
                    VerifyPath(path);
                    FileVersionInfo fileVer = FileVersionInfo.GetVersionInfo(path + @"\GameMakerStudio.exe");
                    ComBoxVerSelector.IsEnabled = true;
                    //ComBoxVerSelector.Text = fileVer.ProductVersion;
                    progVer = new Version(fileVer.ProductVersion);
                    //ComBoxVerSelector.SelectedIndex = -1;
                    BtnInstallCHN.IsEnabled = true;
                    BtnRepairENG.IsEnabled = true;
                }
                catch (VerifyMissingLangDirException)
                {
                    LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                    LabelPathWarning.Content = strWarningBrokenDirectory;
                    //FileVersionInfo fileVer = FileVersionInfo.GetVersionInfo(path + @"\GameMakerStudio.exe");
                    //ComBoxVerSelector.SelectedIndex = -1;
                    ComBoxVerSelector.IsEnabled = false;
                    BtnInstallCHN.IsEnabled = false;
                    BtnRepairENG.IsEnabled = false;
                }
                catch (VerifyMissingConfigException)
                {
                    LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                    LabelPathWarning.Content = "能够进行安装，但GameMaker Studio 2的关键组件可能已损坏\n建议您重新安装GameMaker Studio 2之后再安装";
                    FileVersionInfo fileVer = FileVersionInfo.GetVersionInfo(path + @"\GameMakerStudio.exe");
                    ComBoxVerSelector.SelectedItem = new Version(fileVer.ProductVersion);
                    //ComBoxVerSelector.Text = fileVer.ProductVersion;
                }
                catch (VerifyMissingExecutableException)
                {
                    LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                    LabelPathWarning.Content = strWarningBrokenDirectory;
                    //ComBoxVerSelector.Text = "无法找到版本";
                    ComBoxVerSelector.SelectedIndex = -1;
                    ComBoxVerSelector.IsEnabled = false;
                    BtnInstallCHN.IsEnabled = false;
                    BtnRepairENG.IsEnabled = false;
                }
            }

        }

        private void BtnRepairENG_Click(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine("{0}\t{1}", ComBoxVerSelector.SelectedItem, ComBoxVerSelector.SelectedIndex);
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

        private void GMCN_Link(object sender,RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.gamemaker.cn/");
        }

        private void BtnInstallCHN_Click(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine("{0}\t{1}",ComBoxVerSelector.SelectedItem,ComBoxVerSelector.SelectedIndex);
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
                System.Windows.Forms.MessageBox.Show("翻译内容已注入完毕，如果GameMaker Studio 2已经处于启动状态，请关闭后重新打开，如有发生乱码问题，请更新后重试或联系QQ群或作者。");
            }
        }

        private void CopyTransFile()
        {
            string srcPath = string.Empty;
            string destPath = string.Empty;
            if (ProperVersion >= thresVer)
            {
                srcPath = @".\vers\" + ComBoxVerSelector.Text.Replace('.', '_') + @"\chinese.csv";
                destPath = TextInstallDir.Text + @"\Languages\chinese.csv";
            }
            else
            {
                srcPath = @".\vers\" + ComBoxVerSelector.Text.Replace('.', '_') + @"\trans\english.csv";
                destPath = TextInstallDir.Text + @"\Languages\english.csv";
            }
            //string srcPath = @".\vers\" + ComBoxVerSelector.Text.Replace('.', '_') + @"\chinese.csv";
            //string destPath = TextInstallDir.Text + @"\Languages\chinese.csv";
            if (File.Exists(srcPath))
            {
                File.Copy(srcPath, destPath, true);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("汉化失败，未能找到对应版本的译文，请尝试更新后再汉化，如果还有问题，请联系QQ群或作者QQ","译文缺失",MessageBoxButtons.OK,MessageBoxIcon.Stop);
            }
        }

        private void CopyOrigFile()
        {
            string srcPath = string.Empty;
            string destPath = string.Empty;
            if(ProperVersion >= thresVer)
            {
                srcPath = @".\vers\" + ComBoxVerSelector.Text.Replace('.', '_') + @"\english.csv";
                destPath = TextInstallDir.Text + @"\Languages\english.csv";
            }
            else
            {
                srcPath = @".\vers\" + ComBoxVerSelector.Text.Replace('.', '_') + @"\orig\english.csv";
                destPath = TextInstallDir.Text + @"\Languages\english.csv";
            }

            if (File.Exists(srcPath))
            {
                File.Copy(srcPath, destPath, true);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("修复失败，未能找到对应版本的原文，请尝试更新后再修复，如果还有问题，请联系QQ群或作者QQ", "原文缺失", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("离线体验版暂不支持在线更新~~Sorry","体验版提醒",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
    }
}

public class VerifyMissingConfigException : Exception
{

}

public class VerifyMissingExecutableException : Exception
{

}

public class VerifyMissingLangDirException : Exception
{

}

public class LocatingFailedException : Exception
{

}

public enum PathState
{
    Absolute,
    Relative,
    Invalid
}