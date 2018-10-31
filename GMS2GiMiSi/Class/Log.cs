using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMS2GiMiSi.Class
{
    internal class Log
    {
        public static void WriteLog(string logText)
        {
            try
            {
                var logFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                  @"\GMS2GiMiSi\Log\"; //设置文件夹位置
                if (Directory.Exists(logFilePath) == false) //若文件夹不存在
                {
                    Directory.CreateDirectory(logFilePath);
                }
                var logFilename = Global.logfileName; //设置文件名
                var logPath = logFilePath + logFilename;
                if (!File.Exists(logPath))
                {
                    var fs = File.Create(logPath);
                    fs.Close();
                }
                var fileStream = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                var streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine(DateTime.Now.ToString("[hh:mm.ss]") + logText);
                streamWriter.Flush();
                streamWriter.Close();
                fileStream.Close();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
