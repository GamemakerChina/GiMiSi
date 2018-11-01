using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GMS2GiMiSi.Class;

namespace GMS2GiMiSi.View
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : Page
    {
        public SettingPage()
        {
            InitializeComponent();
        }

        private static readonly string LogDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\GMS2GiMiSi\Log";
        
        /// <summary>
        /// 打开日志文件夹
        /// </summary>
        private void OpenLogDirButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new Uri(LogDir, UriKind.Absolute).AbsoluteUri);
        }

        /// <summary>
        /// 清除日志
        /// </summary>
        private void ClearLogDirButton_OnClick(object sender, RoutedEventArgs e)
        {
            DelLogDirButton.IsEnabled = false;
            var LogDirDirectoryInfo = new DirectoryInfo(LogDir);
            if (LogDirDirectoryInfo.Exists)
            {
                foreach (var fileInfo in LogDirDirectoryInfo.GetFiles())
                {
                    if (fileInfo.Name != Global.logfileName)
                    {
                        try
                        {
                            Log.WriteLog( Log.LogLevel.信息,"删除日志文件 " + fileInfo.Name);
                            File.Delete(fileInfo.FullName);
                            Log.WriteLog( Log.LogLevel.信息,"删除日志文件 " + fileInfo.Name+ " 成功");
                        }
                        catch (Exception)
                        {
                            Log.WriteLog( Log.LogLevel.警告,"删除日志文件 " + fileInfo.Name+ " 失败");
                            throw;
                        }
                    }
                }
            }
            DelLogDirButton.IsEnabled = true;
        }
    }
}
