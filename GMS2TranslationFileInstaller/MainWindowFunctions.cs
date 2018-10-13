using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Web;

namespace GMS2TranslationFileInstaller
{
    ///<summary>
    ///MainWindow类的独立函数
    ///</summary>
    
    public partial class MainWindow : Window
    {
        private bool PathIsValid(string path)
        {
            Regex reg = new Regex(@"^([a-zA-Z]:(\\|\/))?([^\:\/\*\?\""\<\>\|\,]+)?$");
            return reg.IsMatch(path);
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
                System.Windows.Forms.MessageBox.Show("汉化失败，未能找到对应版本的译文，请尝试更新后再汉化，如果还有问题，请联系QQ群或作者QQ", "译文缺失", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void CopyOrigFile()
        {
            string srcPath = string.Empty;
            string destPath = string.Empty;
            if (ProperVersion >= thresVer)
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

        private bool VerifyPath(string path)
        {
            string langpath = path + @"\Languages";
            string configpath = path + @"\GameMakerStudio.exe.config";
            string exepath = path + @"\GameMakerStudio.exe";
            string actpath = path + @"\DnDLibs\YoYo Games\Languages";
            if (Directory.Exists(path))
            {
                if (Directory.Exists(langpath) && Directory.Exists(actpath))
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

        private void EnableInstallation(bool flag)
        {
            BtnInstallCHN.IsEnabled = flag;
            BtnRepairENG.IsEnabled = flag;
            BtnActOvInstallCHN.IsEnabled = flag;
            BtnActOvRepairENG.IsEnabled = flag;
        }

        private void WindowCollapse()
        {
            GrdUpdateSection.Visibility = Visibility.Hidden;
            WinMain.Width = 768;
            GrdMain.ColumnDefinitions[2].Width = new GridLength(0);
            
        }

        private void WindowExpand()
        {
            WinMain.Width = 1024;
            GrdMain.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
            GrdUpdateSection.Visibility = Visibility.Visible;
        }

        private void DirectoryDestroy(string pathDir)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(pathDir);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                    {
                        File.Delete(i.FullName);      //删除指定文件
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void ShowPromptNotImplement()
        {
            System.Windows.Forms.MessageBox.Show("501 Not Implemented:\n    非常抱歉，该功能正在上线中，敬请期待！", "Coming soon！", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private static string GetAutoSearchPath(Edition edition=Edition.Standalone)
        {
            switch(edition)
            {
                case Edition.Standalone:
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\GameMakerStudio2"))
                    {
                        string str = key.GetValue("Install_Dir").ToString();
                        key.Close();
                        return str;
                    }
                case Edition.Steam:
                    return RegistryHelpers.GetRegistryKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 585410").GetValue("InstallLocation").ToString();
                default:
                    return strInstallDirNotFound;
            }
        }
    }
}