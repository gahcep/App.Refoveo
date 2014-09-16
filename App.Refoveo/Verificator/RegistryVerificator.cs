using System;
using App.Refoveo.Helpers;
using Microsoft.Win32;

namespace App.Refoveo.Verificator
{
    public static class RegistryVerificator
    {
        public static bool SubKeyTreeExists(RegistryHive regHive, string regSubKeyTree)
        {
            return RegistryHelper.SubKeyTreeExists(regHive, regSubKeyTree);
        }

        public static bool SubKeyExists(RegistryHive regHive, string regSubKey)
        {
            return RegistryHelper.SubKeyExists(regHive, regSubKey);
        }

        public static bool KeyExists(RegistryHive regHive, string regSubKey, string regKey)
        {
            return RegistryHelper.KeyExists(regHive, regSubKey, regKey);
        }

        public static bool IsKeyValueEqualTo(RegistryHive regHive, string regSubKey, string regKey, Object regValue)
        {
            return RegistryHelper.IsValueEqualTo(regHive, regSubKey, regKey, regValue);
        }
    }
}
