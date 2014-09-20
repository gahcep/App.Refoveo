using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using App.Refoveo.Helpers;
using Microsoft.Win32;

namespace App.Refoveo.Verificator
{
    public static class VersionVerificator
    {
        public static bool InRange(string appVersion, IList<string> appVersions)
        {
            if (String.IsNullOrEmpty(appVersion))
                throw new ArgumentNullException("appVersion");

            return appVersions.Any(version => version == appVersion);
        }

        public static bool IsLess(string appVersionLeft, string appVersionRight)
        {
            var compareResult = VersionHelper.Compare(appVersionLeft, appVersionRight);

            return 
                compareResult == VersionHelper.CompareResult.CompareResultLess;
        }

        public static bool IsGreater(string appVersionLeft, string appVersionRight)
        {
            var compareResult = VersionHelper.Compare(appVersionLeft, appVersionRight);

            return
                compareResult == VersionHelper.CompareResult.CompareResultGreater;
        }

        public static bool IsLessOrEqual(string appVersionLeft, string appVersionRight)
        {
            var compareResult = VersionHelper.Compare(appVersionLeft, appVersionRight);

            return 
                compareResult == VersionHelper.CompareResult.CompareResultLess ||
                compareResult == VersionHelper.CompareResult.CompareResultEqual;
        }

        public static bool IsGreaterOrEqual(string appVersionLeft, string appVersionRight)
        {
            var compareResult = VersionHelper.Compare(appVersionLeft, appVersionRight);

            return
                compareResult == VersionHelper.CompareResult.CompareResultGreater ||
                compareResult == VersionHelper.CompareResult.CompareResultEqual;
        }

        public static bool IsEqual(string appVersionLeft, string appVersionRight)
        {
            var compareResult = VersionHelper.Compare(appVersionLeft, appVersionRight);

            return
                compareResult == VersionHelper.CompareResult.CompareResultEqual;
        }

        public static bool IsSameVersion(string appVersionLeft, string appVersionRight)
        {
            return VersionVerificator.IsEqual(appVersionLeft, appVersionRight);
        }

        public static bool IsSameVersion(string appVersion, Assembly assemblyToParse)
        {
            return VersionVerificator.IsEqual(appVersion, 
                VersionHelper.ExtractVersionFromAssembly(assemblyToParse));
        }

        public static bool IsSameVersion(string appVersion, RegistryHive regHive, string regPath, string regKey)
        {
            return VersionVerificator.IsEqual(appVersion, 
                VersionHelper.ExtractVersionFromRegistry(regHive, regPath, regKey));
        }

        public static bool IsVersionValid(string appVersion, Assembly assemblyToParse)
        {
            return VersionVerificator.IsGreater(appVersion,
                VersionHelper.ExtractVersionFromAssembly(assemblyToParse));
        }

        public static bool IsVersionValid(string appVersion, RegistryHive regHive, string regPath, string regKey)
        {
            return VersionVerificator.IsGreater(appVersion,
                VersionHelper.ExtractVersionFromRegistry(regHive, regPath, regKey));
        }

        public static bool IsMajorOnlyChanged(string appVersion, Assembly assemblyToParse)
        {
            return
                VersionHelper.OnlyChanged(appVersion, assemblyToParse) ==
                VersionHelper.ChangeResult.ChangeResultMajor;
        }

        public static bool IsMajorOnlyChanged(string appVersion, RegistryHive regHive, string regPath, string regKey)
        {
            return
                VersionHelper.OnlyChanged(appVersion, regHive, regPath, regKey) ==
                VersionHelper.ChangeResult.ChangeResultMajor;
        }

        public static bool IsMinorOnlyChanged(string appVersion, Assembly assemblyToParse)
        {
            return
                VersionHelper.OnlyChanged(appVersion, assemblyToParse) ==
                VersionHelper.ChangeResult.ChangeResultMinor;
        }

        public static bool IsMinorOnlyChanged(string appVersion, RegistryHive regHive, string regPath, string regKey)
        {
            return
                VersionHelper.OnlyChanged(appVersion, regHive, regPath, regKey) ==
                VersionHelper.ChangeResult.ChangeResultMinor;
        }

        public static bool IsPatchOnlyChanged(string appVersion, Assembly assemblyToParse)
        {
            return
                VersionHelper.OnlyChanged(appVersion, assemblyToParse) ==
                VersionHelper.ChangeResult.ChangeResultPatch;
        }

        public static bool IsPatchOnlyChanged(string appVersion, RegistryHive regHive, string regPath, string regKey)
        {
            return
                VersionHelper.OnlyChanged(appVersion, regHive, regPath, regKey) ==
                VersionHelper.ChangeResult.ChangeResultPatch;
        }
    }
}
