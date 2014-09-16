using System;
using App.Refoveo.Helpers;
using Microsoft.Win32;
using NUnit.Framework;

namespace App.Refoveo.Tests.Helpers
{
    [TestFixture]
    [Category("Helpers")]
    public class RegistryHelperTests
    {
        private string regValidRoot, 
            regNotValidRoot,
            regTmpSubKey,
            regExistingSubKey,
            regEmptySubKey, 
            regNotEmptyFirstSubKey, 
            regNotEmptySecondSubKey, 
            regNotExistingSubKey;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            const RegistryKeyPermissionCheck keyRights = RegistryKeyPermissionCheck.ReadWriteSubTree;

            regValidRoot = "Software\\AppRefoveoRegTest";
            regNotValidRoot = "Software\\NotExistingSubKey";

            regEmptySubKey = regValidRoot + "\\EmptySubKey";
            regNotEmptyFirstSubKey = regValidRoot + "\\NotEmptyFirstSubKey";
            regNotEmptySecondSubKey = regValidRoot + "\\NotEmptySecondSubKey";

            regExistingSubKey = regEmptySubKey;
            regNotExistingSubKey = regValidRoot + "\\NotExistingSubKey";

            // Is meant to be created and deleted
            regTmpSubKey = regValidRoot + "\\TempSubKey";

            // Root
            Registry.CurrentUser.CreateSubKey(regValidRoot, keyRights);

            // Empty SubKey
            Registry.CurrentUser.CreateSubKey(regEmptySubKey, keyRights);

            // SubKey with children
            Registry.CurrentUser.CreateSubKey(regNotEmptyFirstSubKey, keyRights);
            Registry.CurrentUser.CreateSubKey(regNotEmptySecondSubKey, keyRights);

            // First subkey will contain other subkeys as its children
            Registry.CurrentUser.CreateSubKey(regNotEmptyFirstSubKey + "\\SubKey1", keyRights);
            Registry.CurrentUser.CreateSubKey(regNotEmptyFirstSubKey + "\\SubKey2", keyRights);
            Registry.CurrentUser.CreateSubKey(regNotEmptyFirstSubKey + "\\SubKey3", keyRights);

            // Second subkey will contain keys (with default values)
            var key = Registry.CurrentUser.OpenSubKey(regNotEmptySecondSubKey, true);
            key.SetValue("Key1", "Default");
            key.SetValue("Key2", "Default");
            key.SetValue("Key3", 1000);
        }

        [Test]
        public void TestRootFromHive()
        {
            Assert.AreEqual(RegistryHelper.RootFromHive(RegistryHive.DynData), Registry.PerformanceData);
            Assert.AreEqual(RegistryHelper.RootFromHive(RegistryHive.PerformanceData), Registry.PerformanceData);
            Assert.AreEqual(RegistryHelper.RootFromHive(RegistryHive.ClassesRoot), Registry.ClassesRoot);
            Assert.AreEqual(RegistryHelper.RootFromHive(RegistryHive.CurrentConfig), Registry.CurrentConfig);
            Assert.AreEqual(RegistryHelper.RootFromHive(RegistryHive.CurrentUser), Registry.CurrentUser);
            Assert.AreEqual(RegistryHelper.RootFromHive(RegistryHive.LocalMachine), Registry.LocalMachine);
            Assert.AreEqual(RegistryHelper.RootFromHive(RegistryHive.Users), Registry.Users);
            Assert.AreEqual(RegistryHelper.RootFromHive((RegistryHive)10000000), Registry.CurrentUser);
        }

        [Test]
        public void TestSubKeyTreeExists()
        {
            Assert.True(RegistryHelper.SubKeyTreeExists(RegistryHive.CurrentUser, regValidRoot));
            Assert.False(RegistryHelper.SubKeyTreeExists(RegistryHive.CurrentUser, regNotValidRoot));
        }

        [Test]
        public void TestSubKeyExists()
        {
            Assert.True(RegistryHelper.SubKeyExists(RegistryHive.CurrentUser, regExistingSubKey));
            Assert.False(RegistryHelper.SubKeyExists(RegistryHive.CurrentUser, regNotExistingSubKey));
        }

        [Test]
        public void TestKeyExists()
        {
            Assert.True(RegistryHelper.KeyExists(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "Key1"));
            Assert.False(RegistryHelper.KeyExists(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "Key100"));
            Assert.False(RegistryHelper.KeyExists(RegistryHive.CurrentUser, regNotExistingSubKey, "Key100"));
        }

        [Test]
        public void TestValueEqualTo()
        {
            Assert.True(RegistryHelper.IsValueEqualTo(
                RegistryHive.CurrentUser, regNotEmptySecondSubKey, "Key1", "Default"));
            Assert.False(RegistryHelper.IsValueEqualTo(
                RegistryHive.CurrentUser, regNotEmptySecondSubKey, "Key1", "WontMatch"));
            Assert.False(RegistryHelper.IsValueEqualTo(
                RegistryHive.CurrentUser, regNotExistingSubKey, "Key1", "WontMatch"));
            Assert.False(RegistryHelper.IsValueEqualTo(
                RegistryHive.CurrentUser, regNotEmptySecondSubKey, "Key100", "WontMatch"));

            Assert.True(RegistryHelper.IsValueEqualTo(
                RegistryHive.CurrentUser, regNotEmptySecondSubKey, "Key3", 1000));
            Assert.False(RegistryHelper.IsValueEqualTo(
                RegistryHive.CurrentUser, regNotEmptySecondSubKey, "Key3", 7777));
        }

        [Test]
        public void TestCountKeyChilds()
        {
            Assert.AreEqual(RegistryHelper.CountKeyChilds(RegistryHive.CurrentUser, regValidRoot), 3);
            Assert.AreNotEqual(RegistryHelper.CountKeyChilds(RegistryHive.CurrentUser, regValidRoot), 10);

            Assert.Catch<Exception>(() => RegistryHelper.CountKeyChilds(RegistryHive.CurrentUser, regNotValidRoot));
        }

        [Test]
        public void TestCountValueChilds()
        {
            Assert.AreEqual(RegistryHelper.CountValueChilds(RegistryHive.CurrentUser, regNotEmptySecondSubKey), 3);
            Assert.AreNotEqual(RegistryHelper.CountValueChilds(RegistryHive.CurrentUser, regNotEmptySecondSubKey), 10);

            Assert.Catch<Exception>(() => RegistryHelper.CountValueChilds(RegistryHive.CurrentUser, regNotExistingSubKey));
        }

        [Test]
        public void TestCreateDeleteSubKey()
        {
            Assert.False(RegistryHelper.SubKeyExists(RegistryHive.CurrentUser, regTmpSubKey));
            RegistryHelper.CreateSubKey(RegistryHive.CurrentUser, regTmpSubKey);
            //Assert.Catch<Exception>(() => RegistryHelper.CreateSubKey(RegistryHive.CurrentUser, regNotExistingSubKey));
            Assert.True(RegistryHelper.SubKeyExists(RegistryHive.CurrentUser, regTmpSubKey));
            RegistryHelper.DeleteSubKey(RegistryHive.CurrentUser, regTmpSubKey);
            Assert.Catch<Exception>(() =>
                RegistryHelper.DeleteSubKey(RegistryHive.CurrentUser, regNotExistingSubKey));
            Assert.False(RegistryHelper.SubKeyExists(RegistryHive.CurrentUser, regTmpSubKey));
        }

        [Test]
        public void TestCreateDeleteUpdateKeyValue()
        {
            Assert.False(RegistryHelper.KeyExists(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "KeyNew"));

            Assert.Catch<Exception>(() => 
                RegistryHelper.CreateKey(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "KeyNew", true));
            
            Assert.Catch<Exception>(() =>
                RegistryHelper.CreateKey(RegistryHive.CurrentUser, regNotExistingSubKey, "KeyNew", "KeyNew"));

            RegistryHelper.CreateKey(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "KeyNew", "ValueNew");
            Assert.True(RegistryHelper.IsValueEqualTo(RegistryHive.CurrentUser, 
                regNotEmptySecondSubKey, "KeyNew", "ValueNew"));
            
            RegistryHelper.UpdateKeyValue(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "KeyNew", "UpdatedValue");

            Assert.Catch<Exception>(() =>
                RegistryHelper.UpdateKeyValue(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "KeyNew", true));

            Assert.Catch<Exception>(() =>
                RegistryHelper.UpdateKeyValue(RegistryHive.CurrentUser, regNotExistingSubKey, "KeyNew", "KeyNew"));

            Assert.True(RegistryHelper.IsValueEqualTo(RegistryHive.CurrentUser,
                regNotEmptySecondSubKey, "KeyNew", "UpdatedValue"));
            Assert.AreEqual(RegistryHelper.ReadKeyValue(
                RegistryHive.CurrentUser, regNotEmptySecondSubKey, "KeyNew"), "UpdatedValue");

            Assert.Catch<Exception>(() =>
                RegistryHelper.ReadKeyValue(RegistryHive.CurrentUser, regNotExistingSubKey, "KeyNew"));

            RegistryHelper.DeleteKeyValue(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "KeyNew");

            Assert.True(RegistryHelper.DeleteKeyValue(RegistryHive.CurrentUser, regNotExistingSubKey, "KeyNew"));

            Assert.False(RegistryHelper.KeyExists(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "KeyNew"));
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            Registry.CurrentUser.DeleteSubKeyTree(regValidRoot);
        }
    }
}
