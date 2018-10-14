using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMS2TranslationFileInstaller
{
    internal class FontRegedit
    {
        //[System.Security.Permissions.RegistryPermissionAttribute(System.Security.Permissions.SecurityAction.PermitOnly, Read = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts")]// 约束代码仅可读注册表
        public static System.Collections.Generic.SortedDictionary<string, string> ReadFontInformation()
        {
            var dictionary = new System.Collections.Generic.SortedDictionary<string, string>();

            Microsoft.Win32.RegistryKey localMachineKey = Microsoft.Win32.Registry.LocalMachine;
            // 打开注册表  
            Microsoft.Win32.RegistryKey localMachineKeySub = localMachineKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Fonts", false);

            //获取字体名  
            string[] mynames = localMachineKeySub.GetValueNames();

            foreach (string name in mynames)
            {
                //获取字体的文件名  
                string myvalue = localMachineKeySub.GetValue(name).ToString();

                if (myvalue.Substring(myvalue.Length - 4).ToUpper() == ".TTF" && myvalue.Substring(1, 2).ToUpper() != @":\")
                {
                    string val = name.Substring(0, name.Length - 11);
                    dictionary[val] = @"C:\Windows\Fonts\" + myvalue;
                }
            }
            localMachineKeySub.Close();
            return dictionary;
        }
    }
}
