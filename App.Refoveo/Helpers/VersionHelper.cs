using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace App.Refoveo.Helpers
{
    public static class VersionHelper
    {
        /* FileVersion format (semver):
         *  ---> "XXX.YYY.ZZZ", where:
         *      XXX - Major Version
         *      YYY - Minor Version
         *      ZZZ - Patch/Fix/etc.
         *  Note: in version of types "XXX.YYY.ZZZ.AAA" or 
         *  "XXX.YYY.ZZZ.AAA-anychar" everything after ZZZ will be ignored
         */

        public enum CompareResult 
        {
            CompareResultEqual,
            CompareResultLess,
            CompareResultGreater
        }

        [Flags]
        public enum ChangeResult
        {
            ChangeResultNone = 0,
            ChangeResultMajor = 1,
            ChangeResultMinor = 2,
            ChangeResultPatch = 4
        }

        public static Tuple<int, int, int> ParseVersion(string fileVersion)
        {
            if (String.IsNullOrEmpty(fileVersion))
                return default(Tuple<int, int, int>);

            var matches = Regex.Match(fileVersion, @"^(\d+)\.(\d+)\.(\d+)");
            if (matches.Groups.Count < 4)
                return default(Tuple<int, int, int>);

            return new Tuple<int, int, int>(
                int.Parse(matches.Groups[1].Value),
                int.Parse(matches.Groups[2].Value),
                int.Parse(matches.Groups[3].Value));
        }

        public static string ExtractVersionFromAssembly(Assembly assemblyToParse)
        {
            return AssemblyHelper.FileVersion(assemblyToParse);
        }

        public static string ExtractVersionFromAssembly(string assemblyFilePath)
        {
            return AssemblyHelper.FileVersion(assemblyFilePath);
        }

        public static string ExtractVersionFromRegistry(RegistryHive regHive, string regSubKey, string regKey)
        {
            return (string)RegistryHelper.ReadKeyValue(regHive, regSubKey, regKey);
        }

        public static CompareResult Compare(string appVersionLeft, string appVersionRight)
        {
            if (String.IsNullOrEmpty(appVersionLeft) ||
                String.IsNullOrEmpty(appVersionRight))
                throw new ArgumentNullException("Invalid parameter");

            var versionLeft = ParseVersion(appVersionLeft);
            var versionRight = ParseVersion(appVersionRight);

            if (Equals(versionLeft, default(Tuple<int, int, int>)))
                throw new ArgumentException("Default value is not allowed", "versionLeft");

            if (Equals(versionRight, default(Tuple<int, int, int>)))
                throw new ArgumentException("Default value is not allowed", "versionRight");

            return Compare(versionLeft, versionRight);
        }

        public static CompareResult Compare(Tuple<int, int, int> appVersionLeft, 
            Tuple<int, int, int> appVersionRight)
        {
            #region Check Major part

            // Greater
            if (appVersionLeft.Item1 > appVersionRight.Item1)
                return CompareResult.CompareResultGreater;

            // Less
            if (appVersionLeft.Item1 < appVersionRight.Item1)
                return CompareResult.CompareResultLess;

            #endregion

            #region Check Minor part

            // Greater
            if (appVersionLeft.Item2 > appVersionRight.Item2)
                return CompareResult.CompareResultGreater;

            // Less
            if (appVersionLeft.Item2 < appVersionRight.Item2)
                return CompareResult.CompareResultLess;

            #endregion

            #region Check Patch part

            // Greater
            if (appVersionLeft.Item3 > appVersionRight.Item3)
                return CompareResult.CompareResultGreater;

            // Less
            if (appVersionLeft.Item3 < appVersionRight.Item3)
                return CompareResult.CompareResultLess;

            #endregion

            return CompareResult.CompareResultEqual;
        }

        public static ChangeResult OnlyChanged(string appVersion, Assembly assemblyToParse)
        {
            if (String.IsNullOrEmpty(appVersion))
                throw new ArgumentNullException("appVersion");

            var versionNext = VersionHelper.ParseVersion(appVersion);
            var versionCurrent = VersionHelper.ParseVersion(
                VersionHelper.ExtractVersionFromAssembly(assemblyToParse));

            if (Equals(versionNext, default(Tuple<int, int, int>)))
                throw new ArgumentException("Default value is not allowed", "versionNext");

            if (Equals(versionCurrent, default(Tuple<int, int, int>)))
                throw new ArgumentException("Default value is not allowed", "versionCurrent");

            return OnlyChanged(versionNext, versionCurrent);
        }

        public static ChangeResult OnlyChanged(string appVersion, 
            RegistryHive regHive, string regPath, string regKey)
        {
            if (String.IsNullOrEmpty(appVersion))
                throw new ArgumentNullException("appVersion");
            
            if (String.IsNullOrEmpty(regPath))
                throw new ArgumentNullException("regPath");

            if (String.IsNullOrEmpty(regKey))
                throw new ArgumentNullException("regKey");

            var versionNext = VersionHelper.ParseVersion(appVersion);
            var versionCurrent = VersionHelper.ParseVersion(
                VersionHelper.ExtractVersionFromRegistry(regHive, regPath, regKey));

            if (Equals(versionNext, default(Tuple<int, int, int>)))
                throw new ArgumentException("Default value is not allowed", "versionNext");

            if (Equals(versionCurrent, default(Tuple<int, int, int>)))
                throw new ArgumentException("Default value is not allowed", "versionCurrent");

            return OnlyChanged(versionNext, versionCurrent);
        }

        public static ChangeResult OnlyChanged(Tuple<int, int, int> appVersionLeft,
            Tuple<int, int, int> appVersionRight)
        {
            var changeResult = ChangeResult.ChangeResultNone;

            if (Equals(appVersionLeft, default(Tuple<int, int, int>)))
                throw new ArgumentException("Default value is not allowed", "appVersionLeft");
            
            if (Equals(appVersionRight, default(Tuple<int, int, int>)))
                throw new ArgumentException("Default value is not allowed", "appVersionRight");

            if (appVersionLeft.Item1 < appVersionRight.Item1 ||
                appVersionLeft.Item2 < appVersionRight.Item2 ||
                appVersionLeft.Item3 < appVersionRight.Item3)
                throw new Exception("appVersionLeft should be greater than or equal to appVersionRight");

            // Is Major Only Changed
            changeResult |= appVersionLeft.Item1 > appVersionRight.Item1 &&
                            appVersionLeft.Item2 == appVersionRight.Item2 &&
                            appVersionLeft.Item3 == appVersionRight.Item3
                ? ChangeResult.ChangeResultMajor
                : ChangeResult.ChangeResultNone;

            // Is Minor Only Changed
            changeResult |= appVersionLeft.Item1 == appVersionRight.Item1 &&
                            appVersionLeft.Item2 > appVersionRight.Item2 &&
                            appVersionLeft.Item3 == appVersionRight.Item3
                ? ChangeResult.ChangeResultMinor
                : ChangeResult.ChangeResultNone;

            // Is Patch Only Changed
            changeResult |= appVersionLeft.Item1 == appVersionRight.Item1 &&
                            appVersionLeft.Item2 == appVersionRight.Item2 &&
                            appVersionLeft.Item3 > appVersionRight.Item3
                ? ChangeResult.ChangeResultPatch
                : ChangeResult.ChangeResultNone;

            return changeResult;
        }
    }
}
