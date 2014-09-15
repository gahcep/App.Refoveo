﻿using System;
using System.IO;
using App.Refoveo.Verificator;
using NUnit.Framework;

namespace App.Refoveo.Tests.Verificator
{
    [TestFixture]
    [Category("Verification")]
    public class VerificatorTests
    {
        // Path to Test Data
        private string pathTestData;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            pathTestData = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\Data");

            // Set file attributes: 'Hidden' for 'TestHiddenFile.txt' and 'Read-Only' for 'TestReadOnlyFile.txt'
            var pathFileHidden = Path.Combine(pathTestData, "TestHiddenFile.txt");
            var pathFileReadOnly = Path.Combine(pathTestData, "TestReadOnlyFile.txt");
            var pathFileSystem = Path.Combine(pathTestData, "TestSystemFile.txt");

            if (File.Exists(pathFileHidden))
                File.SetAttributes(pathFileHidden, FileAttributes.Hidden);

            if (File.Exists(pathFileReadOnly))
                File.SetAttributes(pathFileReadOnly, FileAttributes.ReadOnly);

            if (File.Exists(pathFileSystem))
                File.SetAttributes(pathFileSystem, FileAttributes.System);
        }

        #region FileVerificator

        [Test]
        public void TestFileNameWithNull()
        {
            Assert.Catch(typeof (ArgumentException), () => FileVerificator.IsFileNameValid(null));
        }

        [Test]
        public void TestFileNameWithWhitespace()
        {
            Assert.Catch(typeof(ArgumentException), () => FileVerificator.IsFileNameValid("  "));
        }

        [Test]
        public void TestFileNameCorrect()
        {
            Assert.IsTrue(FileVerificator.IsFileNameValid("Valid_Name_For 1_File.ext"));
        }

        [Test]
        public void TestFileNameInvalid()
        {
            Assert.IsFalse(FileVerificator.IsFileNameValid("Invalid < Name_For_File.ext"));
        }

        [Test]
        public void TestFilePathWithNull()
        {
            Assert.Catch(typeof(ArgumentException), () => FileVerificator.IsFilePathValid(null));
        }

        [Test]
        public void TestFilePathWithWhitespace()
        {
            Assert.Catch(typeof(ArgumentException), () => FileVerificator.IsFilePathValid("  "));
        }

        [Test]
        public void TestFilePathCorrect()
        {
            Assert.IsTrue(FileVerificator.IsFilePathValid(@"C:\Program Files\Some Directory\"));
        }

        [Test]
        public void TestFilePathInvalid()
        {
            Assert.IsFalse(FileVerificator.IsFilePathValid(@"C:\Program Files\Some | Directory\"));
        }

        [Test]
        public void TestFileExistsTrue()
        {
            var testFilePath = Path.Combine(pathTestData, "TestFile.txt");

            Assert.IsTrue(FileVerificator.FileExists(testFilePath));
        }

        [Test]
        public void TestFileExistsFalse()
        {
            var testFilePath = Path.Combine(pathTestData, "FileNotExist.txt");

            Assert.IsFalse(FileVerificator.FileExists(testFilePath));
        }

        [Test]
        public void TestUncFileExistsTrue()
        {
            var testFilePath = Path.Combine(pathTestData, "TestFile.txt");
            var uncPath = new Uri(testFilePath).AbsoluteUri;

            Assert.IsTrue(FileVerificator.UncFileExists(uncPath));
        }

        [Test]
        public void TestUncFileExistsFalse()
        {
            var testFilePath = Path.Combine(pathTestData, "FileNotExist.txt");
            var uncPath = new Uri(testFilePath).AbsoluteUri;

            Assert.IsFalse(FileVerificator.UncFileExists(uncPath));
        }

        [Test]
        public void TestFileTypeIsFile()
        {
            var testFilePath = Path.Combine(pathTestData, "TestFile.txt");

            Assert.IsTrue(FileVerificator.Type.IsFile(testFilePath));
        }

        [Test]
        public void TestFileTypeIsReadOnly()
        {
            var fileReadOnly = Path.Combine(pathTestData, "TestReadOnlyFile.txt");
            var fileNotReadOnly = Path.Combine(pathTestData, "TestFile.txt");

            Assert.IsTrue(FileVerificator.Type.IsReadOnly(fileReadOnly));
            Assert.IsFalse(FileVerificator.Type.IsReadOnly(fileNotReadOnly));
        }

        [Test]
        public void TestFileTypeIsHidden()
        {
            var fileHidden = Path.Combine(pathTestData, "TestHiddenFile.txt");
            var fileNotHidden = Path.Combine(pathTestData, "TestFile.txt");

            Assert.IsTrue(FileVerificator.Type.IsHidden(fileHidden));
            Assert.IsFalse(FileVerificator.Type.IsHidden(fileNotHidden));
        }

        [Test]
        public void TestFileTypeIsSystem()
        {
            var fileSystem = Path.Combine(pathTestData, "TestSystemFile.txt");
            var fileNotSystem = Path.Combine(pathTestData, "TestFile.txt");

            Assert.IsTrue(FileVerificator.Type.IsSystem(fileSystem));
            Assert.IsFalse(FileVerificator.Type.IsSystem(fileNotSystem));
        }

        [Test]
        public void TestFileSize()
        {
            long size500Kb = 512;
            long size1Kb = 1*1024;
            long size5Kb = 5*1024;

            var testFile1Kb = Path.Combine(pathTestData, "TestSize1Kb.txt");

            var testFileNotExists = Path.Combine(pathTestData, "NotExisting.txt");

            // Validity
            Assert.Catch(typeof(ArgumentException), () => FileVerificator.Size.EqualTo(testFile1Kb, -1));
            Assert.Catch(typeof(ArgumentException), () => FileVerificator.Size.EqualTo(testFile1Kb, size1Kb, -1));
            Assert.Catch(typeof(ArgumentException), () => FileVerificator.Size.EqualTo(testFileNotExists, size1Kb));
            

            // Custom Condition
            Assert.Catch(typeof(ArgumentException), 
                () => FileVerificator.Size.CustomCondition(testFile1Kb, null));
            Assert.IsTrue(FileVerificator.Size.CustomCondition(testFile1Kb, size => size == size1Kb));

            // LessThan
            Assert.IsTrue(FileVerificator.Size.LessThan(testFile1Kb, size5Kb));
            Assert.IsFalse(FileVerificator.Size.LessThan(testFile1Kb, size500Kb));

            // LessThanOrEqual
            Assert.IsTrue(FileVerificator.Size.LessOrEqualTo(testFile1Kb, size5Kb));
            Assert.IsTrue(FileVerificator.Size.LessOrEqualTo(testFile1Kb, size1Kb));
            Assert.IsFalse(FileVerificator.Size.LessOrEqualTo(testFile1Kb, size500Kb));

            // GreaterThan
            Assert.IsTrue(FileVerificator.Size.GreaterThan(testFile1Kb, size500Kb));
            Assert.IsFalse(FileVerificator.Size.GreaterThan(testFile1Kb, size5Kb));

            // GreaterOrEqualTo
            Assert.IsTrue(FileVerificator.Size.GreaterOrEqualTo(testFile1Kb, size500Kb));
            Assert.IsTrue(FileVerificator.Size.GreaterOrEqualTo(testFile1Kb, size1Kb));
            Assert.IsFalse(FileVerificator.Size.GreaterOrEqualTo(testFile1Kb, size5Kb));

            // EqualTo
            Assert.IsTrue(FileVerificator.Size.EqualTo(testFile1Kb, size1Kb));
            Assert.IsTrue(FileVerificator.Size.EqualTo(testFile1Kb, size500Kb, size500Kb));
        }

        #endregion

        #region DirectoryVerificator
        
        [Test]
        public void TestDirExists()
        {
            var testDir = Path.Combine(pathTestData, "DeleteMeDir");
            if (!Directory.Exists(testDir))
                Directory.CreateDirectory(testDir);

            var testDirNotExists = Path.Combine(pathTestData, "NotExistingDir");

            Assert.Catch(typeof(ArgumentException), () => DirectoryVerificator.DirExists(null));
            Assert.IsTrue(DirectoryVerificator.DirExists(testDir));
            Assert.IsFalse(DirectoryVerificator.DirExists(testDirNotExists));

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

            Assert.Catch(typeof(ArgumentException), () => DirectoryVerificator.UncDirExists(null));
            Assert.IsTrue(DirectoryVerificator.UncDirExists(uncPathValid));
            Assert.IsFalse(DirectoryVerificator.UncDirExists(uncPathInvalid));

            Directory.Delete(testDir);
        }

        [Test]
        public void TestIsDir()
        {
            var testFilePath = Path.Combine(pathTestData, "TestFile.txt");

            Assert.IsTrue(DirectoryVerificator.IsDir(pathTestData));
            Assert.IsFalse(DirectoryVerificator.IsDir(testFilePath));
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

            Assert.Catch(typeof(ArgumentException), () => DirectoryVerificator.IsEmptyDir(filePath));
            Assert.IsTrue(DirectoryVerificator.IsEmptyDir(dirEmpty));
            Assert.IsFalse(DirectoryVerificator.IsEmptyDir(dirNotEmpty));

            Directory.Delete(emptyDir);
        }

        [Test]
        public void TestDirContains()
        {
            var testFilePath = Path.Combine(pathTestData, "TestFile.txt");
            var dirPath = Path.Combine(pathTestData, "NonEmptyDir");

            Assert.Catch(typeof(ArgumentException), () => DirectoryVerificator.ContainsFile(dirPath, null));
            Assert.Catch(typeof(ArgumentException), () => DirectoryVerificator.ContainsFile(dirPath, "  "));
            Assert.Catch(typeof(ArgumentException), () => DirectoryVerificator.ContainsFile(testFilePath, "TestFile.txt"));
            Assert.IsTrue(DirectoryVerificator.ContainsFile(dirPath, "TestFile.txt"));
            Assert.IsFalse(DirectoryVerificator.ContainsFile(dirPath, "NotExistingFile.txt"));
        }

        #endregion
    }
}
