using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using App.Refoveo.Verificator;
using Microsoft.Win32;
using NUnit.Framework;

namespace App.Refoveo.Tests.Verificator
{
    [TestFixture]
    [Category("Verification")]
    public class VersionVerificatorTests
    {
        // Path to Test Data
        private string pathTestData;

        private string regValidRoot;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            const RegistryKeyPermissionCheck keyRights = RegistryKeyPermissionCheck.ReadWriteSubTree;

            pathTestData = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\Data");

            regValidRoot = "Software\\AppRefoveoVersion2Test";

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
        public void TestInRange()
        {
            var versions = new List<string> {"1.1.1", "1.3.4", "1.2.0"};

            Assert.Catch<ArgumentNullException>(() => VersionVerificator.InRange("", null));
            Assert.IsTrue(VersionVerificator.InRange("1.2.0", versions));
            Assert.IsFalse(VersionVerificator.InRange("1.10.0", versions));
        }

        [Test]
        public void TestIsLess()
        {
            Assert.IsTrue(VersionVerificator.IsLess("1.2.0", "1.3.0"));
            Assert.IsFalse(VersionVerificator.IsLess("1.2.0", "1.1.0"));
        }

        [Test]
        public void TestIsGreater()
        {
            Assert.IsTrue(VersionVerificator.IsGreater("1.2.0", "1.1.0"));
            Assert.IsFalse(VersionVerificator.IsGreater("1.2.0", "1.3.0"));
        }

        [Test]
        public void TestIsLessOrEqual()
        {
            Assert.IsTrue(VersionVerificator.IsLessOrEqual("1.2.0", "1.3.0"));
            Assert.IsTrue(VersionVerificator.IsLessOrEqual("1.2.0", "1.2.0"));
            Assert.IsFalse(VersionVerificator.IsLessOrEqual("1.4.0", "1.3.0"));
        }

        [Test]
        public void TestIsGreaterOrEqual()
        {
            Assert.IsTrue(VersionVerificator.IsGreaterOrEqual("1.2.0", "1.1.0"));
            Assert.IsTrue(VersionVerificator.IsGreaterOrEqual("1.1.0", "1.1.0"));
            Assert.IsFalse(VersionVerificator.IsGreaterOrEqual("1.2.0", "1.4.0"));
        }

        [Test]
        public void TestEquality()
        {
            var assembly = Assembly.LoadFile(Path.Combine(pathTestData, "DummyAssembly.4.5.6.0.dll"));

            Assert.IsTrue(VersionVerificator.IsEqual("1.1.0", "1.1.0"));
            Assert.IsTrue(VersionVerificator.IsSameVersion("1.1.0", "1.1.0"));
            Assert.IsFalse(VersionVerificator.IsEqual("2.1.0", "1.1.0"));
            Assert.IsFalse(VersionVerificator.IsSameVersion("2.1.0", "1.1.0"));

            Assert.IsTrue(VersionVerificator.IsSameVersion("4.5.6", assembly));
            Assert.IsTrue(VersionVerificator.IsSameVersion("1.3.10", 
                RegistryHive.CurrentUser, regValidRoot, "KeyValid"));
        }

        [Test]
        public void TestVersionValidity()
        {
            var assembly = Assembly.LoadFile(Path.Combine(pathTestData, "DummyAssembly.4.5.6.0.dll"));

            Assert.IsTrue(VersionVerificator.IsVersionValid("5.5.6", assembly));
            Assert.IsFalse(VersionVerificator.IsVersionValid("4.5.6", assembly));
            Assert.IsTrue(VersionVerificator.IsVersionValid("2.3.10",
                RegistryHive.CurrentUser, regValidRoot, "KeyValid"));
            Assert.IsFalse(VersionVerificator.IsVersionValid("1.3.10",
                RegistryHive.CurrentUser, regValidRoot, "KeyValid"));
        }

        [Test]
        public void TestIsMajorOnlyChanged()
        {
            var assembly = Assembly.LoadFile(Path.Combine(pathTestData, "DummyAssembly.4.5.6.0.dll"));

            Assert.IsTrue(VersionVerificator.IsMajorOnlyChanged("5.5.6", assembly));
            Assert.IsFalse(VersionVerificator.IsMajorOnlyChanged("4.5.6", assembly));
            Assert.IsTrue(VersionVerificator.IsMajorOnlyChanged("2.3.10",
                RegistryHive.CurrentUser, regValidRoot, "KeyValid"));
            Assert.IsFalse(VersionVerificator.IsMajorOnlyChanged("1.3.10",
                RegistryHive.CurrentUser, regValidRoot, "KeyValid"));
        }

        [Test]
        public void TestIsMinorOnlyChanged()
        {
            var assembly = Assembly.LoadFile(Path.Combine(pathTestData, "DummyAssembly.4.5.6.0.dll"));

            Assert.IsTrue(VersionVerificator.IsMinorOnlyChanged("4.6.6", assembly));
            Assert.IsFalse(VersionVerificator.IsMinorOnlyChanged("4.5.6", assembly));
            Assert.IsTrue(VersionVerificator.IsMinorOnlyChanged("1.4.10",
                RegistryHive.CurrentUser, regValidRoot, "KeyValid"));
            Assert.IsFalse(VersionVerificator.IsMinorOnlyChanged("1.3.10",
                RegistryHive.CurrentUser, regValidRoot, "KeyValid"));
        }

        [Test]
        public void TestIsPatchOnlyChanged()
        {
            var assembly = Assembly.LoadFile(Path.Combine(pathTestData, "DummyAssembly.4.5.6.0.dll"));

            Assert.IsTrue(VersionVerificator.IsPatchOnlyChanged("4.5.7", assembly));
            Assert.IsFalse(VersionVerificator.IsPatchOnlyChanged("4.5.6", assembly));
            Assert.IsTrue(VersionVerificator.IsPatchOnlyChanged("1.3.11",
                RegistryHive.CurrentUser, regValidRoot, "KeyValid"));
            Assert.IsFalse(VersionVerificator.IsPatchOnlyChanged("1.3.10",
                RegistryHive.CurrentUser, regValidRoot, "KeyValid"));
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            Registry.CurrentUser.DeleteSubKeyTree(regValidRoot);
        }
    }
}
