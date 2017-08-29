using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Web;
using System.Net.Http;
using System.IO.Compression;

namespace GMS2TranslationFileInstaller
{
    public partial class MainWindow : Window
    {
        private WebClient webClient = new WebClient();
        

        private void DownloadUpdate()
        {
            webClient.DownloadFile("http://liaronce.coding.me/gms2translation/UpdatePackages/vers.zip", ".\vers.zip");
            //GZipStream zipStream = new GZipStream()
            //GZipStream zipStrm = new GZipStream(new FileStream(".\vers.zip", FileMode.Open),CompressionLevel.Optimal);
        }
        
    }
}