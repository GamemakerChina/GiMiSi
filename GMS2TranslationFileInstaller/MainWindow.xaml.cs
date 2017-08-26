using System;
using System.Collections.Generic;
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

using Microsoft.Win32;

namespace GMS2TranslationFileInstaller
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>

    public partial class MainWindow : Window
    {
        Version ver = new Version("a.b");
        private const string strInstallDirNotFound = "<!未找到GameMaker Studio 2的路径>";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender,EventArgs e)
        {
            ChBoxAutoSearch.IsChecked = true;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ChBoxAutoSearch_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\GameMakerStudio2");
                TextInstallDir.Text = key.GetValue("Install_Dir").ToString();
                TextInstallDir.IsEnabled = false;
                BtnInstallDirBrowse.IsEnabled = false;
            }
            catch (System.IO.IOException)
            {
                TextInstallDir.Text = strInstallDirNotFound;
                ChBoxAutoSearch.IsChecked = false;
            }
        }


        private void ChBoxAutoSearch_Unchecked(object sender, RoutedEventArgs e)
        {
            BtnInstallDirBrowse.IsEnabled = true;
            TextInstallDir.IsEnabled = true;
        }
    }
}
