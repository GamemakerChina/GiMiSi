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
using Cursors = System.Windows.Input.Cursors;
using MessageBox = System.Windows.Forms.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace GMS2GiMiSi.View.GMS2ChildPage
{
    /// <summary>
    /// IDEPage.xaml 的交互逻辑
    /// </summary>
    public partial class IDEPage : Page
    {

        /// <summary>
        /// 版本[Standalone, Steam]
        /// </summary>
        public enum Edition
        {
            Standalone,
            Steam
        }

        public IDEPage()
        {
            InitializeComponent();
            Log.WriteLog(Log.LogLevel.信息, "吉米赛启动...");
            TextInstallDir.Text = "<!未找到 GameMaker Studio 2 的路径>";
            ComboBoxFont.SelectedIndex = 0;
            // 加载字体
            LoadFont();
        }

        /// <summary>
        /// 已安装GMS2目录字典
        /// </summary>
        private static readonly Dictionary<string, string> installedDictionary = new Dictionary<string, string>();

        /// <summary>
        /// 自动搜索安装目录
        /// </summary>
        private void AutoSearchDirBrowse_OnClick(object sender, RoutedEventArgs e)
        {
            Log.WriteLog(Log.LogLevel.信息, "自动搜索安装目录");
            GetInstalledGMS2Dir();
        }

        /// <summary>
        /// 获取安装目录
        /// </summary>
        private void GetInstalledGMS2Dir()
        {
            GetAutoSearchPath();
            if (installedDictionary.Count != 0)
            {
                Log.WriteLog(Log.LogLevel.信息, "检索到" + installedDictionary.Count + "个版本");
                GMS2VersionComboBox.Items.Clear();
                foreach (var key in installedDictionary.Keys)
                {
                    var textblock = new TextBlock
                    {
                        Text = key,
                        Cursor = Cursors.Hand
                    };
                    GMS2VersionComboBox.Items.Add(textblock);
                }
                GMS2VersionComboBox.IsEnabled = true;
                GMS2VersionComboBox.SelectedIndex = 0;
                AutoSearchDirButton.IsEnabled = false;
                BtnInstallCHN.IsEnabled = true;
                Log.WriteLog(Log.LogLevel.信息, "自动搜索完毕");
            }
            else
            {
                // TODO 未找到安装路径
            }
        }

        /// <summary>
        /// 获取自动搜索路径
        /// </summary>
        /// <returns>GMS2 安装路径</returns>
        private static void GetAutoSearchPath()
        {
            try
            {
                Log.WriteLog(Log.LogLevel.信息, "从注册表获取GameMaker Studio 2安装路径");
                installedDictionary.Clear();
                Log.WriteLog(Log.LogLevel.信息, "搜索官网下载版安装路径");
                RegistryKey standaloneKey = RegistryHelpers
                    .GetRegistryKey(RegistryHive.CurrentUser, @"Software\GameMakerStudio2");
                if (standaloneKey != null)
                {
                    // 官网下载板
                    installedDictionary.Add("官网下载版", standaloneKey.GetValue("Install_Dir").ToString());
                    standaloneKey.Close();
                }
                Log.WriteLog(Log.LogLevel.信息, "搜索 steam Desktop 版安装路径");
                RegistryKey steamDesktopKey = RegistryHelpers
                    .GetRegistryKey(RegistryHive.LocalMachine ,@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 585410");
                if (steamDesktopKey != null)
                {
                    // Steam Desktop 板
                    installedDictionary.Add("Steam Desktop 版", steamDesktopKey.GetValue("InstallLocation").ToString());
                    steamDesktopKey.Close();
                }
                Log.WriteLog(Log.LogLevel.信息, "搜索 steam Web 版安装路径");
                RegistryKey steamWebKey = RegistryHelpers
                    .GetRegistryKey(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 585600");
                if (steamWebKey != null)
                {
                    // Steam Web 版
                    installedDictionary.Add("Steam Web 版", steamWebKey.GetValue("InstallLocation").ToString());
                    steamWebKey.Close();
                }
                Log.WriteLog(Log.LogLevel.信息, "搜索 steam UWP 版安装路径");
                RegistryKey steamUwpKey = RegistryHelpers
                    .GetRegistryKey(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 585610");
                if (steamUwpKey != null)
                {
                    // Steam UWP 版
                    installedDictionary.Add("Steam UWP 版", steamUwpKey.GetValue("InstallLocation").ToString());
                    steamUwpKey.Close();
                }
                Log.WriteLog(Log.LogLevel.信息, "搜索 steam Mobile 版安装路径");
                RegistryKey steamMobileKey = RegistryHelpers
                    .GetRegistryKey(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 585620");
                if (steamMobileKey != null)
                {
                    // Steam Mobile 版
                    installedDictionary.Add("Steam Mobile 版", steamMobileKey.GetValue("InstallLocation").ToString());
                    steamMobileKey.Close();
                }
                Log.WriteLog(Log.LogLevel.信息, "搜索安装路径完毕");
            }
            catch
            {
                Log.WriteLog(Log.LogLevel.警告, "搜索安装路径失败");
                throw new Exception("搜索安装路径失败");
            }
        }

        private void GMS2VersionComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GMS2VersionComboBox.Items.Count != 0)
            {
                var version = ((TextBlock)GMS2VersionComboBox.Items[GMS2VersionComboBox.SelectedIndex]).Text;
                TextInstallDir.Text = installedDictionary[version];
                if (isLoadFont)
                {
                    default_macrosDeserialize();
                    GroupBoxFont.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// 安装目录变更时
        /// </summary>
        private void TextInstallDir_Changed(object sender, TextChangedEventArgs e)
        {
            GroupBoxFont.IsEnabled = false;
            #region 判断路径
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
            #endregion
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
            Log.WriteLog(Log.LogLevel.信息, "手动选择安装目录");
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "请选择 GameMaker Studio 2 的安装目录"
            };
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                Log.WriteLog(Log.LogLevel.信息, "已选择目录：" + folderBrowserDialog.SelectedPath);
                TextInstallDir.Text = folderBrowserDialog.SelectedPath;
                installedDictionary.Clear();
                var version = TextInstallDir.Text.Contains(@"common\GameMaker Studio 2") ? "Steam版" : "官网下载版";
                GMS2VersionComboBox.Items.Clear();
                GMS2VersionComboBox.IsEnabled = true;
                GMS2VersionComboBox.Items.Add(new TextBlock
                {
                    Text = version,
                    Cursor = Cursors.Hand
                });
                installedDictionary.Add(version, TextInstallDir.Text);
                GMS2VersionComboBox.SelectedIndex = 0;
                AutoSearchDirButton.IsEnabled = true;
                BtnInstallCHN.IsEnabled = true;
                Log.WriteLog(Log.LogLevel.信息, "手动选择安装目录完毕");
            }
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
            Log.WriteLog(Log.LogLevel.信息,"开始复制 IDE 汉化文件");
            await Network.DownloadFileAsync();
            var sourcePath = @".\latest\chinese.csv";
            var targetPath = TextInstallDir.Text + @"\Languages\chinese.csv";
            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, targetPath, true);
                Log.WriteLog(Log.LogLevel.信息,"复制 IDE 汉化文件完毕");
            }
            else
            {
                Log.WriteLog(Log.LogLevel.警告,"复制 IDE 汉化文件失败");
                MessageBox.Show("汉化失败，未能找到对应版本的译文，请尝试更新后再汉化，如果还有问题，请联系QQ群或作者QQ", "译文缺失", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
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

        private void TextAnswer_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //TextAnswer.Foreground = new SolidColorBrush(Color.FromRgb())
        }

        #region 加载字体

        /// <summary>
        /// 是否加载字体
        /// </summary>
        public static bool isLoadFont;

        private string default_font;
        private string default_font_size;
        /// <summary>
        /// 配置文档反序列化
        /// </summary>
        private void default_macrosDeserialize()
        {
            Log.WriteLog(Log.LogLevel.信息, "开始反序列化 default_macros.json");
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
            Log.WriteLog(Log.LogLevel.信息, "反序列化 default_macros.json 完毕");
        }

        /// <summary>
        /// 加载字体
        /// </summary>
        private void LoadFont()
        {
            Log.WriteLog(Log.LogLevel.信息, "加载初始 Open Sans 字体");
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
            Log.WriteLog(Log.LogLevel.信息, "开始异步加载字体");
            Task task = new Task(gb => ActionGroupBoxFont(), ComboBoxFont);
            GroupBoxFont.Header = "字体加载中...";
            task.Start();
        }

        /// <summary>
        /// 更新GroupBoxFont
        /// </summary>
        private async void UpdateGroupBoxFont()
        {
            Log.WriteLog(Log.LogLevel.信息, "从注册表获取系统已安装字体");
            FontSortedDictionary = ReadFontInformation();
            Log.WriteLog(Log.LogLevel.信息, "加载字体");
            foreach (var fonts in FontSortedDictionary)
            {
                // 读取字体文件             
                var pfc = new PrivateFontCollection();
                pfc.AddFontFile(fonts.Value);
                var textBlock = new TextBlock
                {
                    Text = fonts.Key,
                    FontFamily = new FontFamily(pfc.Families[0].Name)
                };
                pfc.Dispose();
                ComboBoxFont.Items.Add(textBlock);
                await Task.Delay(1);
            }
            GroupBoxFont.Header = "字体及字号设置";
            if (TextInstallDir.Text != "<!未找到 GameMaker Studio 2 的路径>")
            {
                default_macrosDeserialize();
                GroupBoxFont.IsEnabled = true;
            }
            isLoadFont = true;
            Log.WriteLog(Log.LogLevel.信息, "异步加载字体完毕");
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
        /// 从注册表读取字体
        /// </summary>
        /// <returns>读取的字体的排序字典</returns>
        //[System.Security.Permissions.RegistryPermissionAttribute(System.Security.Permissions.SecurityAction.PermitOnly, Read = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts")]// 约束代码仅可读注册表
        public static SortedDictionary<string, string> ReadFontInformation()
        {
            var dictionary = new SortedDictionary<string, string>();
            var localMachineKey = Registry.LocalMachine;
            // 打开注册表  
            RegistryKey localMachineKeySub = localMachineKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Fonts", false);
            //获取字体名  
            string[] mynames = localMachineKeySub.GetValueNames();
            foreach (string name in mynames)
            {
                //获取字体的文件名  
                string myvalue = localMachineKeySub.GetValue(name).ToString();

                if (myvalue.Substring(myvalue.Length - 4).ToUpper() == ".TTF" && myvalue.Substring(1, 2).ToUpper() != @":\")
                {
                    string val = name.Replace(" (TrueType)", "");
                    dictionary[val] = Global.WindowsFolder + @"\Windows\Fonts\" + myvalue;
                }
            }
            localMachineKeySub.Close();
            return dictionary;
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
            Log.WriteLog(Log.LogLevel.信息, "开始保存字体及字号设置");
            // 检测多余字体文件
            Log.WriteLog(Log.LogLevel.信息, "检测多余字体文件");
            FileInfo[] fontFiles = new DirectoryInfo(TextInstallDir.Text + @"\Fonts\").GetFiles();
            string[] defaultFontFiles = {
                "DroidSansMono.ttf","NanumGothic-Bold.ttf","NanumGothic-ExtraBold.ttf","NanumGothic-Regular.ttf",
                "OpenSans-Bold.ttf", "OpenSans-BoldItalic.ttf","OpenSans-ExtraBold.ttf","OpenSans-ExtraBoldItalic.ttf",
                "OpenSans-Italic.ttf","OpenSans-Light.ttf","OpenSans-LightItalic.ttf","OpenSans-Regular.ttf",
                "OpenSans-Semibold.ttf","OpenSans-SemiboldItalic.ttf","Oswald-Bold.ttf","Oswald-Light.ttf",
                "Oswald-Regular.ttf",
            };
            foreach (var file in fontFiles)
            {
                var filename = file.Name;
                bool isDefault = false;
                foreach (var defaultFontFile in defaultFontFiles)
                {
                    if (filename == defaultFontFile)
                    {
                        isDefault = true;
                        break;
                    }
                }
                if (!isDefault)
                {
                    Log.WriteLog(Log.LogLevel.信息, "删除多余的 " + filename + " 字体文件");
                    file.Delete();
                }
            }
            // 复制字体文件
            Log.WriteLog(Log.LogLevel.信息, "开始复制字体文件");
            var textBlock = (TextBlock)ComboBoxFont.Items[ComboBoxFont.SelectedIndex];
            if (ComboBoxFont.SelectedIndex != 0)
            {
                string sourceFilePath = FontSortedDictionary[textBlock.Text];
                string destinationFileName = sourceFilePath.Substring(sourceFilePath.LastIndexOf("\\", StringComparison.Ordinal) + 1, sourceFilePath.LastIndexOf(".", StringComparison.Ordinal) - sourceFilePath.LastIndexOf("\\", StringComparison.Ordinal) - 1);
                string destinationFilePath = TextInstallDir.Text + @"\Fonts\" + destinationFileName + ".ttf";
                try
                {
                    Log.WriteLog(Log.LogLevel.信息, "复制 " + destinationFileName + ".ttf");
                    File.Copy(sourceFilePath, destinationFilePath, true);
                    Log.WriteLog(Log.LogLevel.信息, "复制字体文件完毕");
                }
                catch (Exception exception)
                {
                    System.Windows.MessageBox.Show("复制字体文件失败！\r\n" + exception, "警告");
                    Log.WriteLog(Log.LogLevel.警告, "复制字体文件失败");
                }
            }
            Log.WriteLog(Log.LogLevel.信息, "写入序列化 default_macros.json 文件");
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
            Log.WriteLog(Log.LogLevel.信息, "保存字体及字号设置完毕");
        }

        #endregion

    }
}
