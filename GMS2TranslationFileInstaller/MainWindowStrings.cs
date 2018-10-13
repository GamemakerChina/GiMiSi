using System.Windows;

namespace GMS2TranslationFileInstaller
{
    public partial class MainWindow : Window
    {
        private const string strNeedUpdating = "你必须安装更新后方能正常使用本软件的功能";
        private const string strInstallDirNotFound = "<!未找到 GameMaker Studio 2 的路径>";
        private const string strBrowseDirectoryPrompt = "请选择 GameMaker Studio 2 的安装目录";
        private const string strWarningMissingPath = "请选择 GameMaker Studio 2 的安装目录";
        private const string strWarningInvalidPath = "文件路径不合法，可能包含无效字符";
        private const string strWarningMissingDirectory = "目标路径不存在";
        private const string strWarningBrokenDirectory = "该目录下没有安装 GameMaker Studio 2 或已损坏";
        private const string strWarningBrokenGMS2 = "能够进行安装，但 GameMaker Studio 2 的关键组件可能已损坏\n建议您重新安装 GameMaker Studio 2 之后再安装";
    }
}