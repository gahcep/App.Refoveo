using System;
using System.IO;
using System.Reflection;
using App.Refoveo.Helpers;
using Microsoft.Win32;
using NUnit.Framework;

namespace App.Refoveo.Tests.Helpers
{
    [TestFixture]
    [Category("Helpers")]
    public class VersionHelperTests
    {
        // Path to Test Data
        private string pathTestData;

        private string regValidRoot;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            const RegistryKeyPermissionCheck keyRights = RegistryKeyPermissionCheck.ReadWriteSubTree;

            pathTestData = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\Data");

            regValidRoot = "Software\\AppRefoveoVersionTest";
            
            // Root
            Registry.CurrentUser.CreateSubKey(regValidRoot, keyRights);

            // Keys
            var key = Registry.CurrentUser.OpenSubKey(regValidRoot, true);
            key.SetValue("KeyValid", "1.3.10");
            key.SetValue("KeyOnlyMajorChanged", "2.3.10");
            key.SetValue("KeyOnlyMinorChanged", "1.4.10");
            key.SetValue("KeyOnlyPatchChanged", "1.3.11");
            key.SetValue("KeyInvalid", "x.y.z");
        }

        [Test]
        public void TestCompareVersionEqual()
        {
            var versionLeft = new Tuple<int, int, int>(1, 4, 4);
            var versionRight = new Tuple<int, int, int>(1, 4, 4);

            Assert.AreEqual(VersionHelper.Compare(versionLeft, versionRight), 
                VersionHelper.CompareResult.CompareResultEqual);
            Assert.AreEqual(VersionHelper.Compare("1.4.4", "1.4.4"), 
                VersionHelper.CompareResult.CompareResultEqual);
        }

        [Test]
        public void TestCompareVersionLessThan()
        {
            Assert.AreEqual(VersionHelper.Compare(
                new Tuple<int, int, int>(3, 3, 3), new Tuple<int, int, int>(4, 3, 3)), 
                VersionHelper.CompareResult.CompareResultLess);
            Assert.AreEqual(VersionHelper.Compare(
                new Tuple<int, int, int>(3, 3, 3), new Tuple<int, int, int>(3, 4, 3)),
                VersionHelper.CompareResult.CompareResultLess);
            Assert.AreEqual(VersionHelper.Compare(
                new Tuple<int, int, int>(3, 3, 3), new Tuple<int, int, int>(3, 3, 4)),
                VersionHelper.CompareResult.CompareResultLess);
            Assert.AreEqual(VersionHelper.Compare("1.4.4", "1.4.5"), 
                VersionHelper.CompareResult.CompareResultLess);
        }

        [Test]
        public void TestCompareVersionGreaterThan()
        {
            Assert.AreEqual(VersionHelper.Compare(
                new Tuple<int, int, int>(4, 3, 3), new Tuple<int, int, int>(3, 3, 3)), 
                VersionHelper.CompareResult.CompareResultGreater);
            Assert.AreEqual(VersionHelper.Compare(
                new Tuple<int, int, int>(3, 4, 3), new Tuple<int, int, int>(3, 3, 3)),
                VersionHelper.CompareResult.CompareResultGreater);
            Assert.AreEqual(VersionHelper.Compare(
                new Tuple<int, int, int>(3, 3, 4), new Tuple<int, int, int>(3, 3, 3)),
                VersionHelper.CompareResult.CompareResultGreater);
            Assert.AreEqual(VersionHelper.Compare("1.4.5", "1.4.4"),
                VersionHelper.CompareResult.CompareResultGreater);
        }

        [Test]
        public void TestCompareVersionInvalidCases()
        {
            Assert.Catch<ArgumentNullException>(() => VersionHelper.Compare(null, "1.4.4"));
            Assert.Catch<ArgumentNullException>(() => VersionHelper.Compare("1.4.4", null));
            Assert.Catch<ArgumentException>(() => VersionHelper.Compare("x.y.z", "1.4.4"));
            Assert.Catch<ArgumentException>(() => VersionHelper.Compare("1.4.4", "x.y.z"));
        }

        [Test]
        public void TestOnlyChanged()
        {
            var assemblyPath = Path.Combine(pathTestData, "DummyAssembly.4.5.6.0.dll");

            Assert.AreEqual(VersionHelper.OnlyChanged("4.5.6", Assembly.LoadFile(assemblyPath)),
                VersionHelper.ChangeResult.ChangeResultNone);

            Assert.AreEqual(VersionHelper.OnlyChanged("1.3.10", RegistryHive.CurrentUser, regValidRoot, "KeyValid"),
                VersionHelper.ChangeResult.ChangeResultNone);
            Assert.AreEqual(VersionHelper.OnlyChanged("2.3.10", RegistryHive.CurrentUser, regValidRoot, "KeyValid"),
                VersionHelper.ChangeResult.ChangeResultMajor);
            Assert.AreEqual(VersionHelper.OnlyChanged("1.4.10", RegistryHive.CurrentUser, regValidRoot, "KeyValid"),
                VersionHelper.ChangeResult.ChangeResultMinor);
            Assert.AreEqual(VersionHelper.OnlyChanged("1.3.11", RegistryHive.CurrentUser, regValidRoot, "KeyValid"),
                VersionHelper.ChangeResult.ChangeResultPatch);
            Assert.Catch<ArgumentException>(
                () => VersionHelper.OnlyChanged(default(Tuple<int, int, int>), new Tuple<int, int, int>(1, 1, 1)));
            Assert.Catch<ArgumentException>(
                () => VersionHelper.OnlyChanged(new Tuple<int, int, int>(1, 1, 1), default(Tuple<int, int, int>)));
        }

        [Test]
        public void TestOnlyChangedInvalidGeneralCases()
        {
            Assert.Catch<ArgumentNullException>(() => VersionHelper.OnlyChanged("", null));
            Assert.Catch<Exception>(
                () => VersionHelper.OnlyChanged(new Tuple<int, int, int>(1, 1, 1), new Tuple<int, int, int>(2, 2, 2)));
        }

        [Test]
        public void TestOnlyChangedInvalidRegistryCases()
        {
            Assert.Catch<ArgumentNullException>(
                () => VersionHelper.OnlyChanged("", RegistryHive.CurrentUser, null, null));
            Assert.Catch<ArgumentNullException>(
                () => VersionHelper.OnlyChanged("x.y.z", RegistryHive.CurrentUser, null, null));
            Assert.Catch<ArgumentNullException>(
                () => VersionHelper.OnlyChanged("1.4.4", RegistryHive.CurrentUser, null, null));
            Assert.Catch<ArgumentNullException>(
                () => VersionHelper.OnlyChanged("1.4.4", RegistryHive.CurrentUser, regValidRoot, null));
            Assert.Catch<ArgumentException>(
                () => VersionHelper.OnlyChanged("x.y.z", RegistryHive.CurrentUser, regValidRoot, "KeyValid"));
            Assert.Catch<ArgumentException>(
                () => VersionHelper.OnlyChanged("5.5.5", RegistryHive.CurrentUser, regValidRoot, "KeyInvalid"));
        }

        [Test]
        public void TestOnlyChangedInvalidAssemblyCases()
        {
            Assert.Catch<ArgumentException>(
                () => VersionHelper.OnlyChanged("x.y.z", Assembly.GetExecutingAssembly()));
        }

        [Test]
        public void TestParseVersion()
        {
            Assert.AreEqual(VersionHelper.ParseVersion(null), default(Tuple<int, int, int>));
        }

        [Test]
        public void TestExtractVersion()
        {
            var curVersion = AssemblyHelper.FileVersion(Assembly.GetExecutingAssembly());

            Assert.AreEqual(
                VersionHelper.ExtractVersionFromAssembly(pathTestData + "\\DummyAssembly.4.5.6.0.dll"), "4.5.6.0");
            Assert.AreEqual(
                VersionHelper.ExtractVersionFromAssembly(Assembly.GetExecutingAssembly()), curVersion);
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            Registry.CurrentUser.DeleteSubKeyTree(regValidRoot);
        }
    }
}
