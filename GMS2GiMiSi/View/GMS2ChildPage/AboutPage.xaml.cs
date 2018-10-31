using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using static System.String;
using Version = GMS2GiMiSi.Class.Version;

namespace GMS2GiMiSi.View.GMS2ChildPage
{
    /// <summary>
    /// AboutPage.xaml 的交互逻辑
    /// </summary>
    public partial class AboutPage : Page
    {
        /// <summary>
        /// 版本号
        /// </summary>
        private readonly Version version = new Version(System.Windows.Forms.Application.ProductVersion);

        /// <summary>
        /// 构造函数
        /// </summary>
        public AboutPage()
        {
            InitializeComponent();
            VersionDisplay.Text = Format(VersionDisplay.Text, version); // 该软件版本
        }

        /// <summary>
        /// 打开日志
        /// </summary>
        private void LogHyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            ((Hyperlink)sender).NavigateUri = new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                                    @"\GMS2GiMiSi\Log", UriKind.Absolute);
            Process.Start(((Hyperlink)sender)?.NavigateUri.AbsoluteUri ?? throw new InvalidOperationException());
        }

        /// <summary>
        /// 源码页面
        /// </summary>
        private void Link2Code_Click(object sender, RoutedEventArgs e)
        {
            Process.Start((sender as Hyperlink)?.NavigateUri.AbsoluteUri ?? throw new InvalidOperationException());
        }

        /// <summary>
        /// GameMake 开发者之家链接
        /// </summary>
        private void GMCN_Link(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.52gmk.com/");
        }

    }
}
