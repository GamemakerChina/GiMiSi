using System;
using Microsoft.Win32;

namespace GMS2GiMiSi.Class
{
    public class RegistryHelpers
    {
        /// <summary>
        /// 获取注册表键名
        /// </summary>
        /// <returns>null</returns>
        public static RegistryKey GetRegistryKey()
        {
            return GetRegistryKey(null);
        }

        /// <summary>
        /// 获取注册表键名
        /// </summary>
        /// <param name="keyPath">键路径</param>
        /// <returns>键名</returns>
        public static RegistryKey GetRegistryKey(string keyPath)
        {
            RegistryKey localMachineRegistry
                = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                                          Environment.Is64BitOperatingSystem
                                              ? RegistryView.Registry64
                                              : RegistryView.Registry32);

            return string.IsNullOrEmpty(keyPath)
                ? localMachineRegistry
                : localMachineRegistry.OpenSubKey(keyPath);
        }

        /// <summary>
        /// 获取注册表键值
        /// </summary>
        /// <param name="keyPath">键路径</param>
        /// <param name="keyName">键名</param>
        /// <returns>键值</returns>
        public static object GetRegistryValue(string keyPath, string keyName)
        {
            RegistryKey registry = GetRegistryKey(keyPath);
            return registry.GetValue(keyName);
        }
    }

}
