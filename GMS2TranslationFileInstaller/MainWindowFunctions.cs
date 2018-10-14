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
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Web;
using System.Windows.Controls;
using Newtonsoft.Json;
using static System.String;
using FontFamily = System.Windows.Media.FontFamily;
using TextBox = System.Windows.Forms.TextBox;
using Media = System.Windows.Media;
using MessageBox = System.Windows.Forms.MessageBox;

namespace GMS2TranslationFileInstaller
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
                System.Windows.Forms.MessageBox.Show("汉化失败，未能找到对应版本的译文，请尝试更新后再汉化，如果还有问题，请联系QQ群或作者QQ", "译文缺失", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
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
        /// 安装按钮状态
        /// </summary>
        /// <param name="flag">状态值</param>
        private void EnableInstallation(bool flag)
        {
            BtnInstallCHN.IsEnabled = flag;
            GroupBoxFont.IsEnabled = flag;
            BtnStartGMS2.IsEnabled = flag;
            //BtnRepairENG.IsEnabled = flag;
            //BtnActOvInstallCHN.IsEnabled = flag;
            //BtnActOvRepairENG.IsEnabled = flag;
        }
        
        private void ShowPromptNotImplement()
        {
            System.Windows.Forms.MessageBox.Show("501 Not Implemented:\n    非常抱歉，该功能正在上线中，敬请期待！", "Coming soon！", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
                return strInstallDirNotFound;
            }
            return keyString;
        }

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
                var textBlock = ((TextBlock) ComboBoxFont.Items[i]).Text;
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
                return;
            var textBlockOpenSans = new TextBlock()
            {
                Text = "Open Sans",
                FontFamily = new FontFamily("Open Sans.ttf")
            };
            ComboBoxFont.Items.Add(textBlockOpenSans);
            FontSortedDictionary = FontRegedit.ReadFontInformation();
            foreach (var fonts in FontSortedDictionary)
            {
                //读取字体文件             
                var pfc = new PrivateFontCollection();
                pfc.AddFontFile(fonts.Value);
                var textBlock = new TextBlock
                {
                    Text = fonts.Key.Replace(" (TrueType)",""),
                    FontFamily = new FontFamily(pfc.Families[0].Name)
                };
                pfc.Dispose();
                ComboBoxFont.Items.Add(textBlock);
            }
        }

        /// <summary>
        /// 字体字典[字体名, 字体文件夹]
        /// </summary>
        SortedDictionary<string,string> FontSortedDictionary = new SortedDictionary<string, string>();

        /// <summary>
        /// 修改字号
        /// </summary>
        private void FontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = (System.Windows.Controls.TextBox)sender;
            NumTextCheck(textbox);
        }

        /// <summary>
        /// 检查数字文本框
        /// </summary>
        /// <param name="textbox">文本框对象</param>
        public static void NumTextCheck(System.Windows.Controls.TextBox textbox)
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
            if (GMS2ProcessIsRun())
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

    }
}