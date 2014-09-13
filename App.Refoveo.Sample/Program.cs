using System;
using System.IO;
using App.Refoveo.Verificator;

namespace App.Refoveo.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var dirPath = Path.Combine(Directory.GetCurrentDirectory(), "NonEmptyDir");

            var res = DirectoryVerificator.ContainsFile(dirPath, "TestFile.txt");
            res = false;
        }
    }
}
