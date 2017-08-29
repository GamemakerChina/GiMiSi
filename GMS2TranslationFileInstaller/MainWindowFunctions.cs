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
            if(Directory.Exists(path))
            {
                if (Directory.Exists(langpath))
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


    }
}