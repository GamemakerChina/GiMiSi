using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using MessageBox = System.Windows.Forms.MessageBox;

namespace GMS2TranslationFileInstaller
{
    public partial class MainWindow : Window
    {

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
                return;
            var textBlockOpenSans = new TextBlock
            {
                Text = "Open Sans",
                FontFamily = new FontFamily("Open Sans.ttf")
            };
            ComboBoxFont.Items.Add(textBlockOpenSans);

            // 异步加载字体，不卡界面
            Task task = new Task(tb => ActionGroupBoxFont(), ComboBoxFont);
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
    }
}
