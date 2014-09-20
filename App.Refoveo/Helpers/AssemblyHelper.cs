using System;
using System.Diagnostics;
using System.Reflection;
using App.Refoveo.Verificator;

namespace App.Refoveo.Helpers
{
    public static class AssemblyHelper
    {
        public static string FileVersion(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            return FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
        }

        public static string FileVersion(string assemblyFilePath)
        {
            if (!FileVerificator.FileExists(assemblyFilePath))
                throw new ArgumentException("Invalid parameter", "assemblyFilePath");

            return FileVersion(Assembly.LoadFile(assemblyFilePath));
        }
    }
}
