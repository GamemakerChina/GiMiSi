using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GMS2GiMiSi.View.GMS2ChildPage;

namespace GMS2GiMiSi.Class
{
    public static class Global
    {
        /// <summary>
        /// 日志文件名
        /// </summary>
        public static string logfileName = DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".log";

        /// <summary>
        /// 系统盘盘符
        /// </summary>
        public static string WindowsFolder = Environment.ExpandEnvironmentVariables("%systemdrive%");

        /// <summary>
        /// MainWindow
        /// </summary>
        public static Window MainWindow = null;

        /// <summary>
        /// 下载文件名 TextBlock
        /// </summary>
        public static TextBlock DownloadFileName = null;
        /// <summary>
        ///  下载进度条 ProgressBar
        /// </summary>
        public static ProgressBar ProgressBarDownload = null;

        /// <summary>
        /// 下载进度条 RowDefinition
        /// </summary>
        public static RowDefinition DownloadRowDefinition = null;

        public static void DownloadRowDefinitionVisible(bool visible)
        {
            if (visible)
            {
                DownloadRowDefinition.Height = new GridLength(92);
                MainWindow.Height += 92;
            }
            else
            {
                DownloadRowDefinition.Height = new GridLength(0);
                MainWindow.Height -= 92;
            }
        }

        /// <summary>
        /// 页面管理 Page 数组
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
            Log.WriteLog(Log.LogLevel.信息, "判断 GameMaker Studio 2 进程是否存在");
            Process[] vProcesses = Process.GetProcesses();
            foreach (Process vProcess in vProcesses)
            {
                if (vProcess.ProcessName.Equals("GameMakerStudio".Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
                {
                    Log.WriteLog(Log.LogLevel.信息, "GameMaker Studio 2 进程存在");
                    return true;
                }
            }
            Log.WriteLog(Log.LogLevel.信息, "GameMaker Studio 2 进程不存在");
            return false;
        }
    }
}
