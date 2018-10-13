using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Web;
using static System.String;

namespace GMS2TranslationFileInstaller
{
    /// <summary>
    /// MainWindow类的独立函数
    /// </summary>

    public partial class MainWindow : Window
    {
        /// <summary>
        /// 验证路径有效性
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>验证结果</returns>
        private bool PathIsValid(string path)
        {
            Regex reg = new Regex(@"^([a-zA-Z]:(\\|\/))?([^\:\/\*\?\""\<\>\|\,]+)?$");
            return reg.IsMatch(path);
        }

        /// <summary>
        /// 复制 IDE 汉化文件
        /// </summary>
        private async void CopyTransFileAsync()
        {
            await DownloadFileAsync();
            var sourcePath = @".\latest\chinese.csv";
            var targetPath = TextInstallDir.Text + @"\Languages\chinese.csv";
            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, targetPath, true);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("汉化失败，未能找到对应版本的译文，请尝试更新后再汉化，如果还有问题，请联系QQ群或作者QQ", "译文缺失", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        ///// <summary>
        ///// 复制 IDE 源文件
        ///// </summary>
        //private void CopyOrigFile()
        //{
        //    DownloadFileAsync(false);
        //    var sourcePath = @".\latest\english.csv";
        //    var targetPath = TextInstallDir.Text + @"\Languages\english.csv";
        //    if (File.Exists(sourcePath))
        //    {
        //        File.Copy(sourcePath, targetPath, true);
        //    }
        //    else
        //    {
        //        System.Windows.Forms.MessageBox.Show("修复失败，未能找到对应版本的原文，请尝试更新后再修复，如果还有问题，请联系QQ群或作者QQ", "原文缺失", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        //    }
        //}

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
        /// 安装按钮状态
        /// </summary>
        /// <param name="flag">状态值</param>
        private void EnableInstallation(bool flag)
        {
            BtnInstallCHN.IsEnabled = flag;
            //BtnRepairENG.IsEnabled = flag;
            //BtnActOvInstallCHN.IsEnabled = flag;
            //BtnActOvRepairENG.IsEnabled = flag;
        }
        
        private void ShowPromptNotImplement()
        {
            System.Windows.Forms.MessageBox.Show("501 Not Implemented:\n    非常抱歉，该功能正在上线中，敬请期待！", "Coming soon！", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        /// <summary>
        /// 获取自动搜索路径
        /// </summary>
        /// <returns>GMS2 安装路径</returns>
        private string GetAutoSearchPath()
        {
            string keyString;
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\GameMakerStudio2");
            if (key != null)
            {
                keyString = key.GetValue("Install_Dir").ToString();
                key.Close();
                return keyString;
            }
            keyString = RegistryHelpers
                .GetRegistryKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 585410")
                .GetValue("InstallLocation").ToString();
            if (IsNullOrEmpty(keyString))
            {
                return strInstallDirNotFound;
            }
            return keyString;
        }
    }
}