using System;
using System.Linq;
using Microsoft.Win32;

namespace App.Refoveo.Helpers
{
    public static class RegistryHelper
    {
        public static RegistryKey RootFromHive(RegistryHive regHive)
        {
            switch (regHive)
            {
                case RegistryHive.DynData:
                case RegistryHive.PerformanceData:
                    return Registry.PerformanceData;

                case RegistryHive.ClassesRoot:
                    return Registry.ClassesRoot;

                case RegistryHive.CurrentConfig:
                    return Registry.CurrentConfig;

                case RegistryHive.CurrentUser:
                    return Registry.CurrentUser;

                case RegistryHive.LocalMachine:
                    return Registry.LocalMachine;

                case RegistryHive.Users:
                    return Registry.Users;

                default:
                    return Registry.CurrentUser;
            }
        }

        #region Basic Validation

        public static bool SubKeyTreeExists(RegistryHive regHive, string regSubKeyTree)
        {
            var pathToOpen = RootFromHive(regHive).OpenSubKey(regSubKeyTree, false);
            if (pathToOpen == null)
                return false;

            var subKeyCount = pathToOpen.GetSubKeyNames().Count();

            pathToOpen.Close();

            return subKeyCount != 0;
        }

        public static bool SubKeyExists(RegistryHive regHive, string regSubKey)
        {
            var key = RootFromHive(regHive).OpenSubKey(regSubKey, false);
            if (key == null) 
                return false;

            key.Close();

            return true;
        }

        public static bool KeyExists(RegistryHive regHive, string regSubKey, string regKey)
        {
            var key = RootFromHive(regHive).OpenSubKey(regSubKey, false);
            if (key == null) 
                return false;

            var isKeyCreated = key.GetValueNames().AsEnumerable().Contains(regKey);
            
            key.Close();

            return isKeyCreated;
        }

        public static bool IsValueEqualTo(RegistryHive regHive, string regSubKey, string regKey, Object regValue)
        {
            var key = RootFromHive(regHive).OpenSubKey(regSubKey, false);
            if (key == null) 
                return false;

            var containsKey = key.GetValueNames().AsEnumerable().Contains(regKey);
            if (!containsKey)
                return false;

            var isValueCreated = Object.Equals(key.GetValue(regKey), regValue);

            key.Close();

            return isValueCreated;
        }

        #endregion

        #region Registry Counts

        public static int CountKeyChilds(RegistryHive regHive, string regSubKeyTree)
        {
            if (!SubKeyExists(regHive, regSubKeyTree))
                throw new Exception(String.Format("Registry SubKeyTree '{0}' is not existed", regSubKeyTree));

            var key = RootFromHive(regHive).OpenSubKey(regSubKeyTree, false);
            return key.GetSubKeyNames().Count();
        }

        public static int CountValueChilds(RegistryHive regHive, string regSubKey)
        {
            if (!SubKeyExists(regHive, regSubKey))
                throw new Exception(String.Format("Registry Path '{0}' is not existed", regSubKey));

            var key = RootFromHive(regHive).OpenSubKey(regSubKey, false);
            return key.GetValueNames().Count();
        }

        #endregion

        #region Registry Key & SubKey

        public static void CreateSubKey(RegistryHive regHive, string regSubKey)
        {
            RootFromHive(regHive).CreateSubKey(regSubKey);

            if (!SubKeyExists(regHive, regSubKey))
                throw new Exception(String.Format("Fail to create given subkey {0}", regSubKey));
        }

        public static void DeleteSubKey(RegistryHive regHive, string regSubKey)
        {
            if (!SubKeyExists(regHive, regSubKey))
                throw new Exception(String.Format("Given SubKey {0} not existed", regSubKey));

            RootFromHive(regHive).DeleteSubKey(regSubKey, throwOnMissingSubKey: false);
        }

        #endregion

        #region Registry Key Value

        public static void CreateKey(RegistryHive regHive, string regSubKey, string regKey, Object regValue)
        {
            if (regValue.GetType() != typeof(String) &&
                regValue.GetType() != typeof(int))
                throw new Exception("Wrong object type: should be String or int");

            if (!SubKeyExists(regHive, regSubKey))
                throw new Exception(String.Format("SubKey {0} not exists", regSubKey));

            var key = Registry.CurrentUser.OpenSubKey(regSubKey, true);
            key.SetValue(regKey, regValue);
        }

        public static Object ReadKeyValue(RegistryHive regHive, string regSubKey, string regKey)
        {
            if (!KeyExists(regHive, regSubKey, regKey))
                throw new Exception(String.Format("Can't read from {0}. Check registry path", regKey));

            var key = RootFromHive(regHive).OpenSubKey(regSubKey, false);
            var value = key.GetValue(regKey);
            
            key.Close();

            return value;
        }

        public static bool DeleteKeyValue(RegistryHive regHive, string regSubKey, string regKey)
        {
            if (!KeyExists(regHive, regSubKey, regKey))
                return true;

            var key = RootFromHive(regHive).OpenSubKey(regSubKey, true);
            key.DeleteValue(regKey, throwOnMissingValue: false);

            key.Close();

            return true;
        }

        public static void UpdateKeyValue(RegistryHive regHive, string regSubKey, string regKey, Object regNewValue)
        {
            if (regNewValue.GetType() != typeof(String) &&
                regNewValue.GetType() != typeof(int))
                throw new Exception("Wrong object type: should be String or int");

            if (!KeyExists(regHive, regSubKey, regKey))
                throw new Exception(String.Format("Can't read from {0}. Check registry path", regKey));

            RootFromHive(regHive).OpenSubKey(regSubKey, true).SetValue(regKey, regNewValue);
        }

        #endregion

    }
}
