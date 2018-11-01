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
using GMS2GiMiSi.View;
using GMS2GiMiSi.View.GMS2ChildPage;

namespace GMS2GiMiSi.Class
{
    public class Network
    {
        private Network()
        {
            
        }

        public static readonly WebClient webClient = new WebClient();

        /// <summary>
        /// 下载csv文件
        /// </summary>
        public static async Task DownloadFileAsync(bool chinese = true)
        {
            if (!Directory.Exists(@".\latest"))
            {
                Directory.CreateDirectory(@".\latest");
            }
            try
            {
                if (chinese)
                {
                    Log.WriteLog(Log.LogLevel.信息, "开始下载 chinese.csv");
                    Global.DownloadRowDefinitionVisible(true);
                    Global.DownloadFileName.Text = "chinese.csv";
                    await webClient.DownloadFileTaskAsync(
                        new Uri(
                            "https://raw.githubusercontent.com/GamemakerChina/gms2translation/gh-pages/latest/chinese.csv"),
                        @".\latest\chinese.csv");
                    Global.DownloadRowDefinitionVisible(false);
                    Log.WriteLog(Log.LogLevel.信息, "下载 chinese.csv 完成");
                }
                else
                {
                    Log.WriteLog(Log.LogLevel.信息, "开始下载 english.csv");
                    Global.DownloadRowDefinitionVisible(true);
                    Global.DownloadFileName.Text = "english.csv";
                    await webClient.DownloadFileTaskAsync(
                        new Uri(
                            "https://raw.githubusercontent.com/GamemakerChina/gms2translation/gh-pages/latest/english.csv"),
                        @".\latest\english.csv");
                    Global.DownloadRowDefinitionVisible(false);
                    Log.WriteLog(Log.LogLevel.信息, "下载 english.csv 完成");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString().Substring(0, e.ToString().IndexOf("\r\n", StringComparison.Ordinal)));
                Global.DownloadRowDefinitionVisible(false);
                Log.WriteLog(Log.LogLevel.警告, "下载 csv 文件失败");
                throw new Exception("下载 csv 文件失败");
            }
        }

        /// <summary>
        /// 下载runtime Rss xml文件
        /// </summary>
        public static async Task DownloadRssFileAsync()
        {
            Log.WriteLog(Log.LogLevel.信息, "开始下载 Zeus-Runtime.rss");
            if (!Directory.Exists(@".\GiMiSiTemp\rss"))
            {
                Directory.CreateDirectory(@".\GiMiSiTemp\rss");
            }
            try
            {
                await Task.Delay(1);
                Global.DownloadRowDefinitionVisible(true);
                await webClient.DownloadFileTaskAsync(Global.GMS2RuntimeRss, @".\GiMiSiTemp\rss\Zeus-Runtime.rss");
                Global.DownloadRowDefinitionVisible(false);
                Log.WriteLog(Log.LogLevel.信息, "下载 Zeus-Runtime.rss 完毕");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString().Substring(0, e.ToString().IndexOf("\r\n", StringComparison.Ordinal)));
                Log.WriteLog(Log.LogLevel.警告, "下载 Zeus-Runtime.rss 失败");
                Global.DownloadRowDefinitionVisible(false);
                throw new Exception("下载 rss 文件失败");
            }
        }

        /// <summary>
        /// 下载 runtime 压缩包文件
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="path">下载到的位置</param>
        public static async Task DownloadRuntimeFileAsync(string url, string path)
        {
            var filename = url.Substring(url.LastIndexOf("/", StringComparison.Ordinal) + 1, url.Length - url.LastIndexOf("/", StringComparison.Ordinal) - 1);
            try
            {
                Log.WriteLog(Log.LogLevel.信息, "开始下载 "+ filename);
                Global.DownloadFileName.Text = filename;
                Global.DownloadRowDefinitionVisible(true);
                await webClient.DownloadFileTaskAsync(new Uri(url), path + "\\" + filename);
                Global.DownloadRowDefinitionVisible(false);
                Log.WriteLog(Log.LogLevel.信息, "下载 "+ filename +" 完毕");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                Global.DownloadRowDefinitionVisible(false);
                Log.WriteLog(Log.LogLevel.警告, "下载 "+ filename +" 失败");
                throw new Exception("下载 runtime 文件失败");
            }
        }

        /// <summary>
        /// 更新百分比
        /// </summary>
        public static void WebClient_DownloadProgressChangedHandler(object sender, DownloadProgressChangedEventArgs e)
        {
            Global.ProgressBarDownload.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// 下载完毕
        /// </summary>
        public static void WebClient_DownloadFileCompletedHandler(object sender, EventArgs e)
        {
            Global.ProgressBarDownload.Value = 0;
            Global.DownloadFileName.Text = string.Empty;
        }
    }
}