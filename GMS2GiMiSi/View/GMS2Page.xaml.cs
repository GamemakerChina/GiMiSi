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
using GMS2GiMiSi.Class;

namespace GMS2GiMiSi.View
{
    /// <summary>
    /// GMS2Page.xaml 的交互逻辑
    /// </summary>
    public partial class GMS2Page : Page
    {
        public GMS2Page()
        {
            InitializeComponent();
            RootFrame.Navigate(new GMS2ChildPage.IDEPage());
        }

        private void TopListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var listBoxItem = (ListBoxItem)((ListBox)sender).SelectedItem;
                if (listBoxItem != null)
                {
                    var listBoxItemName = listBoxItem.Name;

                    switch (listBoxItemName)
                    {
                        case "IDEBoxItem":
                            // IDEPage
                            RootFrame.Navigate(Global.PageManager[0,0]);
                            break;
                        case "RuntimeBoxItem":
                            // RuntimePage
                            RootFrame.Navigate(Global.PageManager[0, 1]);
                            break;
                        case "AboutBoxItem":
                            // AboutPage
                            RootFrame.Navigate(Global.PageManager[0, 2]);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
