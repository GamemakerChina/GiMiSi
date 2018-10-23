using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace GMS2TranslationFileInstaller
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// GameMaker Studio 2 配置文件夹
        /// </summary>
        private static readonly string GMS2ConfigFilePath = Environment.GetEnvironmentVariable("systemdrive") + @"\ProgramData\GameMakerStudio2";
        private readonly string GMS2runtimesPath = GMS2ConfigFilePath + @"\Cache\runtimes";
        private List<Item> zeusRuntimeItem = new List<Item>();

        private async void RuntimeRssDownload()
        {
            try
            {
                if (!Directory.Exists(GMS2ConfigFilePath))
                {
                    Directory.CreateDirectory(GMS2ConfigFilePath);
                }
                if (!Directory.Exists(GMS2runtimesPath))
                {
                    Directory.CreateDirectory(GMS2runtimesPath);
                }
                await DownloadRssFileAsync();
                // 打开文件 
                FileStream fileStream = new FileStream(@".\rss\Zeus-Runtime.rss", FileMode.Open, FileAccess.Read, FileShare.Read);
                // 读取文件的 byte[] 
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
                fileStream.Close();
                // 把 byte[] 转换成 Stream 
                Stream stream = new MemoryStream(bytes);
                var rssStr = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
                Rss zeusRuntime = (Rss)XmlHelper.Deserialize(typeof(Rss), rssStr);
                // item节
                zeusRuntimeItem = zeusRuntime.Channel.Item;
                zeusRuntimeItem.Reverse();
                zeusRuntimeItem = zeusRuntimeItem.GetRange(0, zeusRuntimeItem.Count > 3 ? 3 : zeusRuntimeItem.Count);
                foreach (var item in zeusRuntimeItem)
                {
                    ComboBoxRuntimeVersion.Items.Add(item.Title.Replace("Version ", ""));
                }
                ComboBoxRuntimeVersion.SelectedIndex = 0;
            }
            catch (Exception e)
            {
                if (e.ToString().Contains("\r\n"))
                {
                    GroupBoxRuntime.Header = "Rumtime 安装 - " + e.ToString().Substring(0, e.ToString().IndexOf("\r\n", StringComparison.Ordinal));
                }
                else
                {
                    GroupBoxRuntime.Header = "Rumtime 安装 - " + e;
                }
            }
        }


        private async void ButtonRuntimeDownload_Click(object sender, RoutedEventArgs e)
        {
            GroupBoxIDE.IsEnabled = false;
            string runtimeVersion = ComboBoxRuntimeVersion.Text;
            string runtimeVersionPath = GMS2runtimesPath + @"\runtime-" + runtimeVersion;
            if (Directory.Exists(runtimeVersionPath))
            {
                if (!Directory.Exists(runtimeVersionPath + @"\download") &&
                    Directory.GetFileSystemEntries(runtimeVersionPath).Length > 0)
                {
                    var result = MessageBox.Show(runtimeVersion + " 版本可能已经安装，是否重新安装？", "警告", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        Directory.Delete(runtimeVersionPath, true);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                Directory.Delete(runtimeVersionPath, true);
            }
            GroupBoxRuntime.IsEnabled = false;
            Directory.CreateDirectory(runtimeVersionPath + @"\download");
            List<string> runtimeFileUrlList= new List<string>();
            Item runtimeItem = null;
            foreach (var item in zeusRuntimeItem)
            {
                if (item.Title.Contains(runtimeVersion))
                {
                    runtimeItem = item;
                    break;
                }
            }
            runtimeFileUrlList.Add(runtimeItem.Enclosure.Url);
            foreach (var module in runtimeItem.Enclosure.Module)
            {
                if (module.Name == "windows" || module.Name == "windowsYYC" || module.Name == "linux" ||
                    module.Name == "linuxYYC" || module.Name == "mac" || module.Name == "macYYC")
                {
                    runtimeFileUrlList.Add(module.Url);
                }
            }
            try
            {
                foreach (var runtimeFile in runtimeFileUrlList)
                {
                    await DownloadRuntimeFileAsync(runtimeFile, runtimeVersionPath + @"\download");
                }
                MessageBox.Show("下载 runtime 成功，请在Gamemaker Stuido 2 偏好设置 - 运行库管理中选择相应版本安装");
            }
            catch (Exception exception)
            {
                MessageBox.Show("下载 runtime 文件失败");
            }
            finally
            {
                GroupBoxRuntime.IsEnabled = true;
                GroupBoxIDE.IsEnabled = true;
            }
        }

        /// <summary>
        /// um文档反序列化 - 获取用户id以获取用户配置文件夹名
        /// </summary>
        private string umDeserialize()
        {
            if (!Directory.Exists(GMS2ConfigFilePath))
            {
                throw new Exception("GameMaker Studio 2 配置文件夹不存在");
            }
            var filePath = GMS2ConfigFilePath + @"\um.json";
            if (!File.Exists(filePath))
            {
                throw new Exception("GameMaker Studio 2 配置文件不存在");
            }
            // 打开文件 
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            // 读取文件的 byte[] 
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            // 把 byte[] 转换成 Stream 
            Stream stream = new MemoryStream(bytes);
            var umStr = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
            var um = JsonConvert.DeserializeObject<umRootObject>(umStr);
            // 获取userID以确认文件夹名
            return um.userID;
        }

    }
}
