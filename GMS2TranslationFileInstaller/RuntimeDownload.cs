using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Newtonsoft.Json;
using Binding = System.Windows.Data.Binding;
using MessageBox = System.Windows.MessageBox;

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

        #region 下载runtime
        private void RuntimeRssDownloadTask()
        {
            // 异步加载runtime，不卡界面
            Task task = new Task(tb => ActionRuntimeRssDownload(), ComboBoxRuntimeVersion);
            task.Start();
        }

        /// <summary>
        /// RuntimeRssDownload Action
        /// </summary>
        private void ActionRuntimeRssDownload()
        {
            Action updateAction = RuntimeRssDownload;
            ComboBoxRuntimeVersion.Dispatcher.BeginInvoke(updateAction);
        }

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
                if (File.Exists(@".\rss\Zeus-Runtime.rss"))
                {
                    File.Delete(@".\rss\Zeus-Runtime.rss");
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
                ComboBoxRuntimeVersion.IsEnabled = false;
                ButtonRuntimeDownload.IsEnabled = false;
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
            if (GMS2ProcessIsRun())
            {
                MessageBox.Show("检测到 GameMaker Studio 2 进程，请关闭程序后进行 runtime 下载操作！", "警告");
                return;
            }
            GroupBoxIDE.IsEnabled = false;
            string runtimeVersion = ComboBoxRuntimeVersion.Text;
            int gms2Version = ComboBoxGms2Version.SelectedIndex;
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
                        GroupBoxIDE.IsEnabled = true;
                        return;
                    }
                }
                else
                {
                    Directory.Delete(runtimeVersionPath, true);
                }
            }
            GroupBoxRuntime.IsEnabled = false;
            Directory.CreateDirectory(runtimeVersionPath + @"\download");
            List<string> runtimeFileUrlList = new List<string>();
            Item runtimeItem = null;
            foreach (var item in zeusRuntimeItem)
            {
                if (item.Title.Contains(runtimeVersion))
                {
                    runtimeItem = item;
                    break;
                }
            }
            if (runtimeItem != null)
            {
                runtimeFileUrlList.Add(runtimeItem.Enclosure.Url);
                foreach (var module in runtimeItem.Enclosure.Module)
                {
                    switch (gms2Version)
                    {
                        case 0:// Desktop
                            if (module.Name == "windows" || module.Name == "windowsYYC" || module.Name == "linux" ||
                                module.Name == "linuxYYC" || module.Name == "mac" || module.Name == "macYYC")
                            {
                                runtimeFileUrlList.Add(module.Url);
                            }
                            break;
                        case 1:// Mac
                            if (module.Name == "mac" || module.Name == "macYYC")
                            {
                                runtimeFileUrlList.Add(module.Url);
                            }
                            break;
                        case 2:// Windows
                            if (module.Name == "windows" || module.Name == "windowsYYC")
                            {
                                runtimeFileUrlList.Add(module.Url);
                            }
                            break;
                        case 3:// Fire
                            if (module.Name == "amazonfire")
                            {
                                runtimeFileUrlList.Add(module.Url);
                            }
                            break;
                        case 4:// Web
                            if (module.Name == "html5")
                            {
                                runtimeFileUrlList.Add(module.Url);
                            }
                            break;
                        case 5:// Mobile
                            if (module.Name == "amazonfire" || module.Name == "android" || module.Name == "ios")
                            {
                                runtimeFileUrlList.Add(module.Url);
                            }
                            break;
                        case 6:// UWP
                            if (module.Name == "windowsuap")
                            {
                                runtimeFileUrlList.Add(module.Url);
                            }
                            break;
                        case 7:// PlayStation 4
                            if (module.Name == "ps4")
                            {
                                runtimeFileUrlList.Add(module.Url);
                            }
                            break;
                        case 8:// Nintendo Switch
                            if (module.Name == "switch")
                            {
                                runtimeFileUrlList.Add(module.Url);
                            }
                            break;
                        case 9:// XboxOne
                            if (module.Name == "xboxone")
                            {
                                runtimeFileUrlList.Add(module.Url);
                            }
                            break;
                        default:// 旗舰版
                                runtimeFileUrlList.Add(module.Url);
                            break;
                    }
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
                MessageBox.Show("下载 runtime 文件失败，" + exception.ToString().Substring(0, exception.ToString().IndexOf("\r\n", StringComparison.Ordinal)));
            }
            finally
            {
                GroupBoxRuntime.IsEnabled = true;
                GroupBoxIDE.IsEnabled = true;
            }
        }

        /*/// <summary>
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
        }*/
        #endregion

        #region 加载已安装runtime

        private void LoadInstalledRuntime()
        {
            DataGridInstalledRuntime.Columns.Add(new DataGridTextColumn
            {
                Width = 80,
                Header = "版本号",
                Binding = new Binding($"[{0}]")
            });
            DataGridInstalledRuntime.Columns.Add(new DataGridTextColumn
            {
                Width = 420,
                Header = "位置",
                Binding = new Binding($"[{1}]")
            });
            RefreshInstalledRuntime(null, null);
        }

        private void RefreshInstalledRuntime(object sender, RoutedEventArgs e)
        {
            DirectoryInfo Dir = new DirectoryInfo(GMS2runtimesPath);
            DirectoryInfo[] DirSub = Dir.GetDirectories();
            List<string[]> installedRuntimeList = new List<string[]>();
            for (int i = DirSub.Length - 1; i >= 0; i--)
            {
                installedRuntimeList.Add(new[] { DirSub[i].Name.Replace("runtime-", ""), GMS2runtimesPath + "\\" + DirSub[i].Name });
            }
            DataGridInstalledRuntime.ItemsSource = installedRuntimeList;
            foreach (var column in DataGridInstalledRuntime.Columns)
            {
                column.IsReadOnly = true;
            }
        }

        private void DeleteInstalledRuntime(object sender, RoutedEventArgs e)
        {
            if (GMS2ProcessIsRun())
            {
                MessageBox.Show("检测到 GameMaker Studio 2 进程，请关闭程序后进行 runtime 删除操作！", "警告");
                return;
            }
            var DialogResult = MessageBox.Show("确定要删除 " + ((string[])DataGridInstalledRuntime.SelectedCells[1].Item)[0] + " 版本runtime吗？",
                "警告", MessageBoxButton.OKCancel);
            var runtimePath = ((string[])DataGridInstalledRuntime.SelectedCells[1].Item)[1];
            if (DialogResult == MessageBoxResult.OK)
            {
                if (Directory.Exists(runtimePath))
                {
                    Directory.Delete(runtimePath, true);
                }
                RefreshInstalledRuntime(null, null);
            }
        }

        #endregion
    }
}
