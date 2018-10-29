using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
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
using Microsoft.Win32;
using Newtonsoft.Json;
using MessageBox = System.Windows.Forms.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace GMS2GiMiSi.View.GMS2ChildPage
{
    /// <summary>
    /// IDEPage.xaml 的交互逻辑
    /// </summary>
    public partial class IDEPage : Page
    {
        public IDEPage()
        {
            InitializeComponent();
            TextInstallDir.Text = "<!未找到 GameMaker Studio 2 的路径>";
        }

        /// <summary>
        /// 安装目录变更时
        /// </summary>
        private void TextInstallDir_Changed(object sender, TextChangedEventArgs e)
        {
            GroupBoxFont.IsEnabled = false;
            //string path = TextInstallDir.Text;
            //if (path == "<!未找到 GameMaker Studio 2 的路径>" || path == "")
            //{
            //    LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            //    LabelPathWarning.Text = "请选择 GameMaker Studio 2 的安装目录";
            //    EnableInstallation(false);
            //}
            //else if (!PathIsValid(path))
            //{
            //    LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            //    LabelPathWarning.Text = "文件路径不合法，可能包含无效字符";
            //    TextGMS2Verion.Text = TextInstallDir.Text.Contains(@"common\GameMaker Studio 2") ? "Steam版" : "官网下载版";
            //    EnableInstallation(false);
            //}
            //else
            //{
            //    LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            //    LabelPathWarning.Text = string.Empty;
            //    try
            //    {
            //        VerifyPath(path);
            //        FileVersionInfo fileVer = FileVersionInfo.GetVersionInfo(path + @"\GameMakerStudio.exe");
            //        new Version(fileVer.ProductVersion);
            //        EnableInstallation(true);
            //    }
            //    catch (VerifyMissingDirException)
            //    {
            //        LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            //        LabelPathWarning.Text = "目标路径不存在";
            //        EnableInstallation(false);

            //    }
            //    catch (VerifyMissingLangDirException)
            //    {
            //        LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            //        LabelPathWarning.Text = "该目录下没有安装 GameMaker Studio 2 或已损坏";
            //        EnableInstallation(false);
            //    }
            //    catch (VerifyMissingExecutableException)
            //    {
            //        LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            //        LabelPathWarning.Text = "该目录下没有安装 GameMaker Studio 2 或已损坏";
            //        EnableInstallation(false);
            //    }
            //    catch (VerifyMissingConfigException)
            //    {
            //        LabelPathWarning.Foreground = new SolidColorBrush(Color.FromRgb(0, 128, 0));
            //        LabelPathWarning.Text = "能够进行安装，但 GameMaker Studio 2 的关键组件可能已损坏\n建议您重新安装 GameMaker Studio 2 之后再安装";
            //        FileVersionInfo fileVer = FileVersionInfo.GetVersionInfo(path + @"\GameMakerStudio.exe");
            //        EnableInstallation(true);
            //    }
            //}
        }

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
        /// 手动选择安装目录
        /// </summary>
        private void BtnInstallDir_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dial = new FolderBrowserDialog
            {
                Description = "请选择 GameMaker Studio 2 的安装目录"
            };
            if (dial.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextInstallDir.Text = dial.SelectedPath;
                TextGMS2Verion.Text = TextInstallDir.Text.Contains(@"common\GameMaker Studio 2") ? "Steam版" : "官网下载版";
                LoadFont();
            }
        }

        /// <summary>
        /// 自动查找复选框勾选
        /// </summary>
        private void ChBoxAutoSearch_Checked(object sender, RoutedEventArgs e)
        {
            BtnInstallDirBrowse.IsEnabled = false;
            try
            {
                TextInstallDir.Text = GetAutoSearchPath();
                TextGMS2Verion.Text = TextInstallDir.Text.Contains(@"common\GameMaker Studio 2") ? "Steam版" : "官网下载版";
                // 加载字体
                LoadFont();
                EnableInstallation(true);
            }
            catch (IOException)
            {
                TextInstallDir.Text = "<!未找到 GameMaker Studio 2 的路径>";
                EnableInstallation(false);
                System.Windows.Forms.MessageBox.Show("自动查找未能找到 GameMaker Studio 2 的安装位置，请检查安装路径或取消勾选自动查找并尝试手动查找", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            //SnapToProperVersion();
        }

        /// <summary>
        /// 自动查找复选框取消勾选
        /// </summary>
        private void ChBoxAutoSearch_Unchecked(object sender, RoutedEventArgs e)
        {
            if (GroupBoxFont.Header.ToString() == "字体加载中...")
            {
                MessageBox.Show("请等待字体加载完毕", "警告");
                CheckBoxAutoSearch.IsChecked = true;
                GroupBoxFont.IsEnabled = false;
                return;
            }
            TextInstallDir.Text = "<!未找到 GameMaker Studio 2 的路径>";
            TextGMS2Verion.Text = "";
            BtnInstallDirBrowse.IsEnabled = true;
            EnableInstallation(false);
        }

        /// <summary>
        /// 安装汉化界面文件
        /// </summary>
        private void BtnInstallCHN_Click(object sender, RoutedEventArgs e)
        {
            if (Global.GMS2ProcessIsRun())
            {
                System.Windows.MessageBox.Show("检测到 GameMaker Studio 2 进程，请关闭程序后进行汉化操作！", "警告");
                return;
            }
            CopyTransFileAsync();
            MessageBox.Show("翻译内容已注入完毕\r\n请做以下操作：\r\n" +
                                                 "①启动软件打开偏好设置（File-Preferences）\r\n" +
                                                 "②选择 General Settings，在右侧的IDE Language选项中选择 “Chinese / 简体中文”并点击左下角的Apply\r\n" +
                                                 "如有发生乱码问题，请更新后重试或联系QQ群或作者。", "翻译完成");
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
            if (string.IsNullOrEmpty(keyString))
            {
                return "<!未找到 GameMaker Studio 2 的路径>";
            }
            return keyString;
        }
        /// <summary>
        /// 浏览中文帮助文档
        /// </summary>
        private void BtnManualCHN_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://gamemakerchina.github.io/GMS2_manual_en2ch/");
        }

        /// <summary>
        /// 浏览英文帮助文档
        /// </summary>
        private void BtnManualENG_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://docs2.yoyogames.com/");
        }

        /// <summary>
        /// 启动GMS2
        /// </summary>
        private void StartGMS2Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(TextInstallDir.Text + "\\GameMakerStudio.exe");
        }

        #region 加载字体
        private string default_font;
        private string default_font_size;
        /// <summary>
        /// 配置文档反序列化
        /// </summary>
        private void default_macrosDeserialize()
        {
            // 打开文件 
            FileStream fileStream = new FileStream(TextInstallDir.Text + @"\defaults\default_macros.json", FileMode.Open, FileAccess.Read, FileShare.Read);
            // 读取文件的 byte[] 
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            // 把 byte[] 转换成 Stream 
            Stream stream = new MemoryStream(bytes);
            var default_macrosStr = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
            var default_macros = JsonConvert.DeserializeObject<default_macrosRootObject>(default_macrosStr);
            default_font = default_macros.default_font;
            default_font_size = default_macros.default_font_size;
            for (int i = 0; i < ComboBoxFont.Items.Count; i++)
            {
                var textBlock = ((TextBlock)ComboBoxFont.Items[i]).Text;
                if (textBlock == default_font)
                {
                    ComboBoxFont.SelectedIndex = i;
                    break;
                }
            }
            TextBoxFontSize.Text = default_font_size;
        }

        /// <summary>
        /// 加载字体
        /// </summary>
        private void LoadFont()
        {
            if (FontSortedDictionary.Count != 0)
            {
                GroupBoxFont.IsEnabled = true;
                return;
            }
            var textBlockOpenSans = new TextBlock
            {
                Text = "Open Sans",
                FontFamily = new FontFamily("Open Sans.ttf")
            };
            ComboBoxFont.Items.Add(textBlockOpenSans);

            // 异步加载字体，不卡界面
            Task task = new Task(gb => ActionGroupBoxFont(), ComboBoxFont);
            GroupBoxFont.Header = "字体加载中...";
            task.Start();
        }

        /// <summary>
        /// 更新GroupBoxFont
        /// </summary>
        private async void UpdateGroupBoxFont()
        {
            FontSortedDictionary = ReadFontInformation();
            foreach (var fonts in FontSortedDictionary)
            {
                //读取字体文件             
                var pfc = new PrivateFontCollection();
                pfc.AddFontFile(fonts.Value);
                var textBlock = new TextBlock
                {
                    Text = fonts.Key.Replace(" (TrueType)", ""),
                    FontFamily = new FontFamily(pfc.Families[0].Name)
                };
                pfc.Dispose();
                ComboBoxFont.Items.Add(textBlock);
                await Task.Delay(1);
            }
            default_macrosDeserialize();
            GroupBoxFont.Header = "字体及字号设置";
            isLoadFont = true;
            GroupBoxFont.IsEnabled = true;
        }

        /// <summary>
        /// GroupBoxFont Action
        /// </summary>
        private void ActionGroupBoxFont()
        {
            Action updateAction = UpdateGroupBoxFont;
            ComboBoxFont.Dispatcher.BeginInvoke(updateAction);
        }

        /// <summary>
        /// 字体字典[字体名, 字体文件夹]
        /// </summary>
        SortedDictionary<string, string> FontSortedDictionary = new SortedDictionary<string, string>();

        /// <summary>
        /// 修改字号
        /// </summary>
        private void FontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = (TextBox)sender;
            NumTextCheck(textbox);
        }

        /// <summary>
        /// 检查数字文本框
        /// </summary>
        /// <param name="textbox">文本框对象</param>
        public static void NumTextCheck(TextBox textbox)
        {
            try
            {
                if (!Regex.IsMatch(textbox.Text, "^\\d*\\.?\\d*$") && textbox.Text != "")
                {
                    int pos = textbox.SelectionStart - 1;
                    textbox.Text = textbox.Text.Remove(pos, 1);
                    textbox.SelectionStart = pos;
                }
                if (textbox.Text != "")
                {
                    if (int.Parse(textbox.Text) > 72)
                    {
                        textbox.Text = "72";
                    }
                    if (int.Parse(textbox.Text) < 1)
                    {
                        textbox.Text = "9";
                    }
                }
            }
            catch (Exception)
            {
                textbox.Text = "9";
            }
        }

        /// <summary>
        /// 保存字体及字号设置
        /// </summary>
        private void SaveFont_OnClick(object sender, RoutedEventArgs e)
        {
            if (Global.GMS2ProcessIsRun())
            {
                System.Windows.MessageBox.Show("检测到 GameMaker Studio 2 进程，请关闭程序后应用修改！", "警告");
                return;
            }
            // 复制字体文件
            var textBlock = (TextBlock)ComboBoxFont.Items[ComboBoxFont.SelectedIndex];
            if (ComboBoxFont.SelectedIndex != 0)
            {
                string sourceFilePath = FontSortedDictionary[textBlock.Text];
                string destinationFileName = sourceFilePath.Substring(sourceFilePath.LastIndexOf("\\", StringComparison.Ordinal) + 1, sourceFilePath.LastIndexOf(".", StringComparison.Ordinal) - sourceFilePath.LastIndexOf("\\", StringComparison.Ordinal) - 1);
                string destinationFilePath = TextInstallDir.Text + @"\Fonts\" + destinationFileName + ".ttf";
                try
                {
                    File.Copy(sourceFilePath, destinationFilePath, true);
                }
                catch (Exception exception)
                {
                    System.Windows.MessageBox.Show("复制字体文件失败！\r\n" + exception, "警告");
                }
            }
            // 打开文件 
            FileStream fileStream = new FileStream(TextInstallDir.Text + @"\defaults\default_macros.json", FileMode.Open, FileAccess.Read, FileShare.Read);
            // 读取文件的 byte[] 
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            // 把 byte[] 转换成 Stream 
            Stream stream = new MemoryStream(bytes);
            var default_macrosStr = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
            var default_macros = JsonConvert.DeserializeObject<default_macrosRootObject>(default_macrosStr);
            default_macros.default_font = ComboBoxFont.SelectedIndex == 0 ? "Open Sans" : textBlock.Text;
            default_macros.default_font_size = TextBoxFontSize.Text;
            var SerializeText = JsonConvert.SerializeObject(default_macros).Replace("{\"system_directory\":", "{\r\n\"system_directory\":").Replace(",", ",\r\n").Replace("\"}", "\"\r\n}");
            try
            {
                using (StreamWriter writer = new StreamWriter(TextInstallDir.Text + @"\defaults\default_macros.json", false))
                {
                    writer.WriteLine(SerializeText);
                    writer.Close();
                }
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show("文件写入失败！\r\n" + exception, "警告");
            }
            MessageBox.Show("设置成功保存！");
        }

        //[System.Security.Permissions.RegistryPermissionAttribute(System.Security.Permissions.SecurityAction.PermitOnly, Read = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts")]// 约束代码仅可读注册表
        public static SortedDictionary<string, string> ReadFontInformation()
        {
            var dictionary = new SortedDictionary<string, string>();

            Microsoft.Win32.RegistryKey localMachineKey = Microsoft.Win32.Registry.LocalMachine;
            // 打开注册表  
            Microsoft.Win32.RegistryKey localMachineKeySub = localMachineKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Fonts", false);

            //获取字体名  
            string[] mynames = localMachineKeySub.GetValueNames();

            foreach (string name in mynames)
            {
                //获取字体的文件名  
                string myvalue = localMachineKeySub.GetValue(name).ToString();

                if (myvalue.Substring(myvalue.Length - 4).ToUpper() == ".TTF" && myvalue.Substring(1, 2).ToUpper() != @":\")
                {
                    string val = name.Substring(0, name.Length - 11);
                    dictionary[val] = @"C:\Windows\Fonts\" + myvalue;
                }
            }
            localMachineKeySub.Close();
            return dictionary;
        }
        #endregion
    }
}
