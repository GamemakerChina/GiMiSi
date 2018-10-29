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

namespace GMS2GiMiSi.Class
{
    public class Network
    {
        private Network()
        {
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChangedHandler;
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompletedHandler;
        }

        private static readonly WebClient webClient = new WebClient();

        /// <summary>
        /// 下载csv文件
        /// </summary>
        public static async Task DownloadFileAsync(bool chinese = true)
        {
            if (!Directory.Exists(@".\latest"))
            {
                Directory.CreateDirectory(@".\latest");
            }
            if (chinese)
            {
                Global.DownloadFileName.Text = "chinese.csv";
                await webClient.DownloadFileTaskAsync(new Uri("https://raw.githubusercontent.com/GamemakerChina/gms2translation/gh-pages/latest/chinese.csv"), @".\latest\chinese.csv");
            }
            else
            {
                Global.DownloadFileName.Text = "english.csv";
                await webClient.DownloadFileTaskAsync(new Uri("https://raw.githubusercontent.com/GamemakerChina/gms2translation/gh-pages/latest/english.csv"), @".\latest\english.csv");
            }
        }

        /// <summary>
        /// 下载runtime Rss xml文件
        /// </summary>
        public static async Task DownloadRssFileAsync()
        {
            if (!Directory.Exists(@".\rss"))
            {
                Directory.CreateDirectory(@".\rss");
            }
            try
            {
                await Task.Delay(1);
                await webClient.DownloadFileTaskAsync(new Uri("https://gms.magecorn.com/Zeus-Runtime.rss"), @".\rss\Zeus-Runtime.rss");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString().Substring(0, e.ToString().IndexOf("\r\n", StringComparison.Ordinal)));
                throw new Exception("下载 rss 文件失败");
            }
        }

        /// <summary>
        /// 下载runtime压缩包文件
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="path">下载到的位置</param>
        public static async Task DownloadRuntimeFileAsync(string url, string path)
        {
            try
            {
                var filename = url.Substring(url.LastIndexOf("/", StringComparison.Ordinal) + 1, url.Length - url.LastIndexOf("/", StringComparison.Ordinal) - 1);
                Global.DownloadFileName.Text = filename;
                await webClient.DownloadFileTaskAsync(new Uri(url), path + "\\" + filename);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw new Exception("下载 runtime 文件失败");
            }
        }

        /// <summary>
        /// 更新百分比
        /// </summary>
        private void WebClient_DownloadProgressChangedHandler(object sender, DownloadProgressChangedEventArgs e)
        {
            Global.ProgressBarDownload.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// 下载完毕
        /// </summary>
        private void WebClient_DownloadFileCompletedHandler(object sender, EventArgs e)
        {
            Global.ProgressBarDownload.Value = 0;
            Global.DownloadFileName.Text = string.Empty;
        }
    }
}