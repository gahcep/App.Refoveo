using System;
using System.IO;
using System.Text.RegularExpressions;

namespace App.Refoveo.Verificator
{
    public static class FileVerificator
    {
        public static bool IsFileNameValid(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentException("Parameter is invalid", "fileName");

            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));

            return !(new Regex(String.Format("[{0}]", invalidChars)).IsMatch(fileName));
        }

        public static bool IsFilePathValid(string pathFull)
        {
            if (String.IsNullOrEmpty(pathFull))
                throw new ArgumentException("Parameter is invalid", "pathFull");

            var invalidChars = Regex.Escape(new string(Path.GetInvalidPathChars()));

            return !(new Regex(String.Format("[{0}]", invalidChars)).IsMatch(pathFull));
        }

        public static bool FileExists(string path)
        {
            return !String.IsNullOrEmpty(path) && (new FileInfo(path)).Exists;
        }

        public static bool UncFileExists(string path)
        {
            return !String.IsNullOrEmpty(path) && (new FileInfo(new Uri(path).LocalPath)).Exists;
        }

        public static class Type
        {
            private static bool CheckFile(string path)
            {
                return
                    FileVerificator.IsFilePathValid(path) &&
                    FileVerificator.FileExists(path);
            }

            public static bool IsFile(string path)
            {
                return 
                    CheckFile(path) && 
                    (File.GetAttributes(path) & FileAttributes.Directory) != FileAttributes.Directory;
            }

            public static bool IsSystem(string path)
            {
                return
                    CheckFile(path) && 
                    (File.GetAttributes(path) & FileAttributes.System) == FileAttributes.System;
            }

            public static bool IsReadOnly(string path)
            {
                return
                    CheckFile(path) && 
                    (File.GetAttributes(path) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
            }

            public static bool IsHidden(string path)
            {
                return
                    CheckFile(path) && 
                    (File.GetAttributes(path) & FileAttributes.Hidden) == FileAttributes.Hidden;
            }
        }

        public static class Size
        {
            private static bool CheckSize(string path, Func<long, bool> predicate, long? allowedSize = null)
            {
                if (allowedSize != null)
                    if (allowedSize < 0)
                        throw new ArgumentException("Size can't be less than zero", "allowedSize");

                if (!FileVerificator.FileExists(path))
                    throw new ArgumentException("Given file doesn't exist", "path");

                return predicate((new FileInfo(path)).Length);
            }

            public static bool CustomCondition(string path, Func<long, bool> action)
            {
                if (action == null)
                    throw new ArgumentNullException("action");

                return CheckSize(path, action);
            }

            public static bool LessThan(string path, long allowedSize)
            {
                return CheckSize(path, size => size < allowedSize, allowedSize);
            }

            public static bool LessOrEqualTo(string path, long allowedSize)
            {
                return CheckSize(path, size => size <= allowedSize, allowedSize);
            }

            public static bool GreaterThan(string path, long allowedSize)
            {
                return CheckSize(path, size => size > allowedSize, allowedSize);
            }

            public static bool GreaterOrEqualTo(string path, long allowedSize)
            {
                return CheckSize(path, size => size >= allowedSize, allowedSize);
            }

            public static bool EqualTo(string path, long allowedSize, long delta = 0)
            {
                if (delta < 0)
                    throw new ArgumentException("Parameter is invalid", "delta");

                return CheckSize(path, size => Math.Abs(size - allowedSize) <= delta, allowedSize);
            }
        }
    }
}
