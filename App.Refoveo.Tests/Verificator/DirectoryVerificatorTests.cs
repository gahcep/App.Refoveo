using System;
using System.IO;
using NUnit.Framework;

namespace App.Refoveo.Tests.Verificator
{
    [TestFixture]
    [Category("Verification")]
    public class DirectoryVerificatorTests
    {
        // Path to Test Data
        private string pathTestData;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            pathTestData = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\Data");
        }

        [Test]
        public void TestDirExists()
        {
            var testDir = Path.Combine(pathTestData, "DeleteMeDir");
            if (!Directory.Exists(testDir))
                Directory.CreateDirectory(testDir);

            var testDirNotExists = Path.Combine(pathTestData, "NotExistingDir");

            Assert.Catch(typeof(ArgumentException), () => Refoveo.Verificator.DirectoryVerificator.DirExists(null));
            Assert.IsTrue(Refoveo.Verificator.DirectoryVerificator.DirExists(testDir));
            Assert.IsFalse(Refoveo.Verificator.DirectoryVerificator.DirExists(testDirNotExists));

            Directory.Delete(testDir);
        }

        [Test]
        public void TestUncDirExistsTrue()
        {
            var testDir = Path.Combine(pathTestData, "DeleteMeDir");
            if (!Directory.Exists(testDir))
                Directory.CreateDirectory(testDir);

            var uncPathValid = new Uri(testDir).AbsoluteUri;
            var dirPathInvalid = Path.Combine(pathTestData, "NotExistingDir");
            var uncPathInvalid = new Uri(dirPathInvalid).AbsoluteUri;

            Assert.Catch(typeof(ArgumentException), () => Refoveo.Verificator.DirectoryVerificator.UncDirExists(null));
            Assert.IsTrue(Refoveo.Verificator.DirectoryVerificator.UncDirExists(uncPathValid));
            Assert.IsFalse(Refoveo.Verificator.DirectoryVerificator.UncDirExists(uncPathInvalid));

            Directory.Delete(testDir);
        }

        [Test]
        public void TestIsDir()
        {
            var testFilePath = Path.Combine(pathTestData, "TestFile.txt");

            Assert.IsTrue(Refoveo.Verificator.DirectoryVerificator.IsDir(pathTestData));
            Assert.IsFalse(Refoveo.Verificator.DirectoryVerificator.IsDir(testFilePath));
        }

        [Test]
        public void TestIsDirEmpty()
        {
            var emptyDir = Path.Combine(pathTestData, "DeleteMeDir");
            if (!Directory.Exists(emptyDir))
                Directory.CreateDirectory(emptyDir);

            var filePath = Path.Combine(pathTestData, "TestFile.txt");
            var dirEmpty = Path.Combine(pathTestData, emptyDir);
            var dirNotEmpty = Path.Combine(pathTestData, "NonEmptyDir");

            Assert.Catch(typeof(ArgumentException), () => Refoveo.Verificator.DirectoryVerificator.IsEmptyDir(filePath));
            Assert.IsTrue(Refoveo.Verificator.DirectoryVerificator.IsEmptyDir(dirEmpty));
            Assert.IsFalse(Refoveo.Verificator.DirectoryVerificator.IsEmptyDir(dirNotEmpty));

            Directory.Delete(emptyDir);
        }

        [Test]
        public void TestDirContains()
        {
            var testFilePath = Path.Combine(pathTestData, "TestFile.txt");
            var dirPath = Path.Combine(pathTestData, "NonEmptyDir");

            Assert.Catch(typeof(ArgumentException), () => Refoveo.Verificator.DirectoryVerificator.ContainsFile(dirPath, null));
            Assert.Catch(typeof(ArgumentException), () => Refoveo.Verificator.DirectoryVerificator.ContainsFile(dirPath, "  "));
            Assert.Catch(typeof(ArgumentException), () => Refoveo.Verificator.DirectoryVerificator.ContainsFile(testFilePath, "TestFile.txt"));
            Assert.IsTrue(Refoveo.Verificator.DirectoryVerificator.ContainsFile(dirPath, "TestFile.txt"));
            Assert.IsFalse(Refoveo.Verificator.DirectoryVerificator.ContainsFile(dirPath, "NotExistingFile.txt"));
        }
    }
}
