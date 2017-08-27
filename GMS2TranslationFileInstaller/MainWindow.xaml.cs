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
        private Version ver = new Version();

        private const string strInstallDirNotFound = "<!未找到GameMaker Studio 2的路径>";
        private const string strBrowseDirectoryPrompt = "请选择GameMaker Studio 2的安装目录";
        private const string strWarningMissingPath = "请选择GameMaker Studio 2的安装目录";
        private const string strWarningInvalidPath = "文件路径不合法，可能包含无效字符";
        private const string strWarningBrokenDirectory = "该目录下没有安装GameMaker Studio 2或已损坏";

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
                        throw new VerifyMissingExecutable();
                        //return false;
                        
                    }
                }
                else
                {
                    throw new VerifyMissingConfig();
                    //return false;
                }
            }
            else
            {
                throw new VerifyMissingLangDir();
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
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ChBoxAutoSearch_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\GameMakerStudio2");
                TextInstallDir.Text = key.GetValue("Install_Dir").ToString();
                TextInstallDir.IsEnabled = false;
                BtnInstallDirBrowse.IsEnabled = false;
            }
            catch (System.IO.IOException)
            {
                TextInstallDir.Text = strInstallDirNotFound;
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
                ComBoxVerSelector.Text = "无法找到版本";
                ComBoxVerSelector.IsEnabled = false;
                BtnInstallCHN.IsEnabled = false;
                BtnRepairENG.IsEnabled = false;
            }
            else if(!PathIsValid(path))
            {
                LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                LabelPathWarning.Content = strWarningInvalidPath;
                ComBoxVerSelector.Text = "无法找到版本";
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
                    ComBoxVerSelector.Text = fileVer.ProductVersion;
                    BtnInstallCHN.IsEnabled = true;
                    BtnRepairENG.IsEnabled = true;
                }
                catch (VerifyMissingLangDir)
                {
                    LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                    LabelPathWarning.Content = strWarningBrokenDirectory;
                    //FileVersionInfo fileVer = FileVersionInfo.GetVersionInfo(path + @"\GameMakerStudio.exe");
                    ComBoxVerSelector.Text = "无法找到版本";
                    ComBoxVerSelector.IsEnabled = false;
                }
                catch (VerifyMissingConfig)
                {
                    LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                    LabelPathWarning.Content = "能够进行安装，但GameMaker Studio 2可能已损坏";
                    FileVersionInfo fileVer = FileVersionInfo.GetVersionInfo(path + @"\GameMakerStudio.exe");
                    ComBoxVerSelector.Text = fileVer.ProductVersion;
                }
                catch (VerifyMissingExecutable)
                {
                    LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                    LabelPathWarning.Content = strWarningBrokenDirectory;
                    ComBoxVerSelector.Text = "无法找到版本";
                    ComBoxVerSelector.IsEnabled = false;
                    BtnInstallCHN.IsEnabled = false;
                    BtnRepairENG.IsEnabled = false;
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

public class VerifyMissingConfig : Exception
{

}

public class VerifyMissingExecutable : Exception
{

}

public class VerifyMissingLangDir : Exception
{

}

public enum PathState
{
    Absolute,
    Relative,
    Invalid
}