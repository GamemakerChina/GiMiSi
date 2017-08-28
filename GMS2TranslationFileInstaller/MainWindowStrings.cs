using System;
using System.IO;
using System.Windows;

namespace GMS2TranslationFileInstaller
{
    public partial class MainWindow:Window
    {
        private const string strInstallDirNotFound = "<!未找到GameMaker Studio 2的路径>";
        private const string strBrowseDirectoryPrompt = "请选择GameMaker Studio 2的安装目录";
        private const string strWarningMissingPath = "请选择GameMaker Studio 2的安装目录";
        private const string strWarningInvalidPath = "文件路径不合法，可能包含无效字符";
        private const string strWarningBrokenDirectory = "该目录下没有安装GameMaker Studio 2或已损坏";
    }
}