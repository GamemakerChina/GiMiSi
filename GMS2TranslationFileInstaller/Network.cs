using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Web;
//using System.Net.Http;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace GMS2TranslationFileInstaller
{
    public partial class MainWindow : Window
    {
        private readonly WebClient webClient = new WebClient();

        /// <summary>
        /// 下载文件
        /// </summary>
        private async Task DownloadFileAsync(bool chinese = true)
        {
            if (!Directory.Exists(@".\latest"))
            {
                Directory.CreateDirectory(@".\latest");
            }
            if (chinese)
            {
                DownloadFileName.Text = "chinese.csv";
                await webClient.DownloadFileTaskAsync(new Uri("https://raw.githubusercontent.com/GamemakerChina/gms2translation/gh-pages/latest/chinese.csv"), @".\latest\chinese.csv");
            }
            else
            {
                DownloadFileName.Text = "english.csv";
                await webClient.DownloadFileTaskAsync(new Uri("https://raw.githubusercontent.com/GamemakerChina/gms2translation/gh-pages/latest/english.csv"), @".\latest\english.csv");
            }
        }

        /// <summary>
        /// 更新百分比
        /// </summary>
        private void WebClient_DownloadProgressChangedHandler(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgDownload.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// 下载完毕
        /// </summary>
        private void WebClient_DownloadFileCompletedHandler(object sender, EventArgs e)
        {
            ProgDownload.Value = 0;
            DownloadFileName.Text = string.Empty;
        }
    }
}