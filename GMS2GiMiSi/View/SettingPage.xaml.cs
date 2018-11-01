using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GMS2GiMiSi.Class;
using Newtonsoft.Json;
using MessageBox = System.Windows.Forms.MessageBox;
using RadioButton = System.Windows.Controls.RadioButton;

namespace GMS2GiMiSi.View
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : Page
    {
        /// <summary>
        /// 状态检测 Timer
        /// </summary>
        private readonly Timer stateDetectionTimer = new Timer();

        /// <summary>
        /// 状态检测事件
        /// </summary>
        private void StateDetectionTimerEvent(object sender, EventArgs e)
        {
            GMS2RuntimeStackPanel.IsEnabled = !Global.GMS2RuntimeRssDownloading;
        }

        /// <summary>
        /// 默认 configRootObject
        /// </summary>
        private static ConfigRootObject configRootObject = new ConfigRootObject
        {
            GMS2RuntimeRss = "1"
        };

        public SettingPage()
        {
            InitializeComponent();
            ReadConfigJson();
            stateDetectionTimer.Interval = 100;
            stateDetectionTimer.Tick += StateDetectionTimerEvent;
            stateDetectionTimer.Start();
        }

        #region 日志
        /// <summary>
        /// 日志文件夹
        /// </summary>
        private static readonly string LogDir = Environment.CurrentDirectory + @"\GiMiSiTemp\Log\";
        
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
        #endregion

        /// <summary>
        /// Runtime 国内镜像站页面(来源于LiarOnce)
        /// </summary>
        private void Link2RuntimeMirrorSite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://gms.magecorn.com/");
        }

        /// <summary>
        /// 读取 config.json
        /// </summary>
        private void ReadConfigJson()
        {
            Log.WriteLog(Log.LogLevel.信息, "开始读取 config.json");
            if (File.Exists(@".\GiMiSiTemp\config.json"))
            {
                // 打开文件 
                FileStream fileStream = new FileStream(@".\GiMiSiTemp\config.json", FileMode.Open, FileAccess.Read, FileShare.Read);
                // 读取文件的 byte[] 
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
                fileStream.Close();
                // 把 byte[] 转换成 Stream 
                Stream stream = new MemoryStream(bytes);
                var configString = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
                var temp = JsonConvert.DeserializeObject<ConfigRootObject>(configString);
                if (temp != null)
                {
                    configRootObject = temp;
                }
                stream.Close();
            }
            Log.WriteLog(Log.LogLevel.信息, "读取 config.json 完成");
            var gms2RuntimeRss = configRootObject.GMS2RuntimeRss;
            switch (gms2RuntimeRss)
            {
                case "0":
                    gms2RuntimeRssYoYoRadioButton.IsChecked = true;
                    break;
                case "1":
                    gms2RuntimeRssLiarOnceRadioButton.IsChecked = true;
                    break;
                default:
                    CustomURLTextBox.IsEnabled = true;
                    CustomURLButton.IsEnabled = true;
                    CustomURLTextBox.Text = gms2RuntimeRss;
                    Global.GMS2RuntimeRss = new Uri(gms2RuntimeRss);
                    gms2RuntimeRssCustomRadioButton.IsChecked = true;
                    break;
            }
        }

        /// <summary>
        /// 写入 config.json
        /// </summary>
        private void WriteConfigJson()
        {
            Log.WriteLog(Log.LogLevel.信息, "开始写入 config.json");
            var SerializeText = JsonConvert.SerializeObject(configRootObject);
            if (!File.Exists(@".\GiMiSiTemp\config.json"))
            {
                var temp = File.Create(@".\GiMiSiTemp\config.json");
                temp.Close();
            }
            try
            {
                
                StreamWriter writer = new StreamWriter(@".\GiMiSiTemp\config.json", false);
                writer.WriteLine(SerializeText);
                writer.Close();
                Log.WriteLog(Log.LogLevel.信息, "写入 config.json 完成");
            }
            catch (Exception exception)
            {
                Log.WriteLog(Log.LogLevel.警告, "写入 config.json 失败");
                System.Windows.MessageBox.Show("文件写入失败！\r\n" + exception, "警告");
            }
        }

        private void Gms2RuntimeRss_OnChecked(object sender, RoutedEventArgs e)
        {
            var RadioButtonName = ((RadioButton)sender).Name;
            switch (RadioButtonName)
            {
                case "gms2RuntimeRssYoYoRadioButton":
                    Log.WriteLog(Log.LogLevel.信息, "选择了 YoYoGames 官方 runtime rss 订阅源");
                    configRootObject.GMS2RuntimeRss = "0";
                    Global.GMS2RuntimeRss = new Uri("http://gms.yoyogames.com/Zeus-Runtime.rss");
                    CustomURLTextBox.IsEnabled = false;
                    CustomURLButton.IsEnabled = false;
                    WriteConfigJson();
                    break;
                case "gms2RuntimeRssLiarOnceRadioButton":
                    Log.WriteLog(Log.LogLevel.信息, "选择了 LiarOnce 提供的 runtime rss 订阅源国内镜像站");
                    configRootObject.GMS2RuntimeRss = "1";
                    Global.GMS2RuntimeRss = new Uri("https://gms.magecorn.com/Zeus-Runtime.rss");
                    CustomURLTextBox.IsEnabled = false;
                    CustomURLButton.IsEnabled = false;
                    WriteConfigJson();
                    break;
                case "gms2RuntimeRssCustomRadioButton":
                    Log.WriteLog(Log.LogLevel.信息, "选择了自定义 runtime rss 订阅源");
                    CustomURLTextBox.IsEnabled = true;
                    CustomURLButton.IsEnabled = true;
                    break;
            }
        }

        private void CustomURLButton_OnClick(object sender, RoutedEventArgs e)
        {
            Log.WriteLog(Log.LogLevel.信息, "验证 URL");
            Regex re = new Regex(@"(?<url>http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?)");
            MatchCollection mc = re.Matches(CustomURLTextBox.Text);
            if (mc.Count > 0)
            {
                var url = mc[0].ToString();
                if (url.EndsWith(".rss"))
                {
                    MessageBox.Show("验证 URL 通过，设置已保存");
                    Global.GMS2RuntimeRss = new Uri(url);
                    configRootObject.GMS2RuntimeRss = url;
                    WriteConfigJson();
                    Log.WriteLog(Log.LogLevel.信息, "验证 URL 成功");
                }
                else
                {
                    Log.WriteLog(Log.LogLevel.警告, "验证 URL 失败");
                    MessageBox.Show("验证 URL 失败，请注意指定的 URL 是否指向 .rss 文件");
                }
            }
            else
            {
                Log.WriteLog(Log.LogLevel.警告, "验证 URL 失败");
                MessageBox.Show("验证 URL 失败，请填写正确格式的 URL");
            }
        }
    }
}
