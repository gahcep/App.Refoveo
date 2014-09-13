using System;
using System.IO;
using System.Linq;

namespace App.Refoveo.Verificator
{
    public static class DirectoryVerificator
    {
        public static bool IsDir(string path)
        {
            if (!DirectoryVerificator.DirExists(path))
                return false;

            return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public static bool DirExists(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Parameter is invalid", "path");

            return (new DirectoryInfo(path)).Exists;
        }

        public static bool UncDirExists(string uriPath)
        {
            if (String.IsNullOrWhiteSpace(uriPath))
                throw new ArgumentException("Parameter is invalid", "uriPath");

            return (new DirectoryInfo(new Uri(uriPath).LocalPath)).Exists;
        }

        public static bool IsEmptyDir(string path)
        {
            if (!DirectoryVerificator.IsDir(path))
                throw new ArgumentException("Not directory", path);

            return !Directory.EnumerateFiles(path).Any();
        }

        public static bool ContainsFile(string path, string filename)
        {
            if (String.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("Parameter is invalid", "filename");

            if (!DirectoryVerificator.IsDir(path))
                throw new ArgumentException("Not directory", path);

            return Directory.EnumerateFiles(path).Contains(Path.Combine(path, filename));
        }
    }
}
