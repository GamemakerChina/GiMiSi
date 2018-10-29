using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GMS2GiMiSi.Class
{
    public static class Global
    {
        public static TextBlock DownloadFileName = null;
        public static ProgressBar ProgressBarDownload = null;

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
