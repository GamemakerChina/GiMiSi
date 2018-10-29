using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Windows;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Web;
using System.Windows.Controls;
using System.Xml.Serialization;
using static System.String;
using MessageBox = System.Windows.Forms.MessageBox;

namespace GMS2GiMiSi
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
        /// 判断GMS2进程是否存在
        /// </summary>
        /// <returns>进程是否存在</returns>
        private bool GMS2ProcessIsRun()
        {
            Process[] vProcesses = Process.GetProcesses();
            foreach (Process vProcess in vProcesses)
            {
                if (vProcess.ProcessName.Equals("GameMakerStudio".Replace(" ",""), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
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
                MessageBox.Show("汉化失败，未能找到对应版本的译文，请尝试更新后再汉化，如果还有问题，请联系QQ群或作者QQ", "译文缺失", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        
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
                return "<!未找到 GameMaker Studio 2 的路径>";
            }
            return keyString;
        }
    }
}
