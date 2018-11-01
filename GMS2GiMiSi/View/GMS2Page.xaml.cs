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
            RootFrame.Navigate(Global.PageManager[0, 0]);
            ResourceDictionary.Source =
                new Uri("pack://application:,,,/GMS2GiMiSi;component/Dictionary/ListBoxItemDictionary.xaml",
                    UriKind.Absolute);
        }

        private static readonly ResourceDictionary ResourceDictionary = new ResourceDictionary();

        private void FillSecondListBox(string name, string text, bool isselected = false)
        {
            ListBoxItem ideBoxItem = new ListBoxItem
            {
                Name = name,
                IsSelected = isselected,
                Style = (Style) ResourceDictionary["ListBoxItemStyle"],
                Tag = "Second",
                Content = new TextBlock
                {
                    Text = text,
                    FontSize = 18,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(7, 0, 7, 0)
                }
            };
            SecondListBox.Items.Add(ideBoxItem);
        }

        private void TopListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var listBoxItem = (ListBoxItem)((ListBox)sender).SelectedItem;
                if (listBoxItem != null)
                {
                    var listBoxItemName = listBoxItem.Name;
                    SecondListBox.Items.Clear();
                    SecondRowDefinition.Height = new GridLength(40);
                    switch (listBoxItemName)
                    {
                        case "GMS2BoxItem":
                            FillSecondListBox("IDEBoxItem", "IDE 汉化", true);
                            FillSecondListBox("RuntimeBoxItem", "Runtime 管理", true);
                            SecondListBox.SelectedIndex = 0;
                            break;
                        case "GMS1BoxItem":
                            // TODO
                            break;
                        case "AboutBoxItem":
                            // AboutPage
                            RootFrame.Navigate(Global.PageManager[2, 0]);
                            SecondRowDefinition.Height = new GridLength(0);
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
        private void SecondListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                            RootFrame.Navigate(Global.PageManager[0, 0]);
                            break;
                        case "RuntimeBoxItem":
                            // RuntimePage
                            RootFrame.Navigate(Global.PageManager[0, 1]);
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
