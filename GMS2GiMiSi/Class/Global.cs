using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GMS2GiMiSi.View.GMS2ChildPage;

namespace GMS2GiMiSi.Class
{
    public static class Global
    {
        /// <summary>
        /// 日志文件名
        /// </summary>
        public static string logfileName;

        /// <summary>
        /// 系统盘盘符
        /// </summary>
        public static string WindowsFolder = Environment.ExpandEnvironmentVariables("%systemdrive%");

        /// <summary>
        /// 下载文件名TextBlock
        /// </summary>
        public static TextBlock DownloadFileName = null;
        /// <summary>
        ///  下载进度条ProgressBar
        /// </summary>
        public static ProgressBar ProgressBarDownload = null;

        /// <summary>
        /// 页面管理Page数组
        /// </summary>
        public static Page[,] PageManager = new Page[3, 3]
        {
            { new IDEPage(), new RuntimePage(), null},
            { null, null, null},
            { new AboutPage(), null, null}
        };

        /// <summary>
        /// 判断GMS2进程是否存在
        /// </summary>
        /// <returns>进程是否存在</returns>
        public static bool GMS2ProcessIsRun()
        {
            Process[] vProcesses = Process.GetProcesses();
            foreach (Process vProcess in vProcesses)
            {
                if (vProcess.ProcessName.Equals("GameMakerStudio".Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
