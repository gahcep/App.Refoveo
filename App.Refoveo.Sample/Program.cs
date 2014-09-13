using System;
using System.IO;
using App.Refoveo.Verificator;

namespace App.Refoveo.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            long size500Kb = 512;
            long size1Kb = 1*1024;
            long size5Kb = 5*1024;

            var testFile1Kb = Path.Combine(Directory.GetCurrentDirectory(), "TestSize1Kb.txt");

            var res = FileVerificator.Size.LessOrEqualTo(testFile1Kb, size1Kb);
            res = FileVerificator.Size.GreaterOrEqualTo(testFile1Kb, size1Kb);
        }
    }
}
