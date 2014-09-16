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
        private RegistryHelper regHelper;
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

            regHelper = new RegistryHelper();
        }

        [Test]
        public void TestRootFromHive()
        {
            Assert.AreEqual(regHelper.RootFromHive(RegistryHive.DynData), Registry.PerformanceData);
            Assert.AreEqual(regHelper.RootFromHive(RegistryHive.PerformanceData), Registry.PerformanceData);
            Assert.AreEqual(regHelper.RootFromHive(RegistryHive.ClassesRoot), Registry.ClassesRoot);
            Assert.AreEqual(regHelper.RootFromHive(RegistryHive.CurrentConfig), Registry.CurrentConfig);
            Assert.AreEqual(regHelper.RootFromHive(RegistryHive.CurrentUser), Registry.CurrentUser);
            Assert.AreEqual(regHelper.RootFromHive(RegistryHive.LocalMachine), Registry.LocalMachine);
            Assert.AreEqual(regHelper.RootFromHive(RegistryHive.Users), Registry.Users);
            Assert.AreEqual(regHelper.RootFromHive((RegistryHive)10000000), Registry.CurrentUser);
        }

        [Test]
        public void TestSubKeyTreeExists()
        {
            Assert.True(regHelper.SubKeyTreeExists(RegistryHive.CurrentUser, regValidRoot));
            Assert.False(regHelper.SubKeyTreeExists(RegistryHive.CurrentUser, regNotValidRoot));
        }

        [Test]
        public void TestSubKeyExists()
        {
            Assert.True(regHelper.SubKeyExists(RegistryHive.CurrentUser, regExistingSubKey));
            Assert.False(regHelper.SubKeyExists(RegistryHive.CurrentUser, regNotExistingSubKey));
        }

        [Test]
        public void TestKeyExists()
        {
            Assert.True(regHelper.KeyExists(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "Key1"));
            Assert.False(regHelper.KeyExists(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "Key100"));
            Assert.False(regHelper.KeyExists(RegistryHive.CurrentUser, regNotExistingSubKey, "Key100"));
        }

        [Test]
        public void TestValueEqualTo()
        {
            Assert.True(regHelper.ValueEqualTo(
                RegistryHive.CurrentUser, regNotEmptySecondSubKey, "Key1", "Default"));
            Assert.False(regHelper.ValueEqualTo(
                RegistryHive.CurrentUser, regNotEmptySecondSubKey, "Key1", "WontMatch"));
            Assert.False(regHelper.ValueEqualTo(
                RegistryHive.CurrentUser, regNotExistingSubKey, "Key1", "WontMatch"));
            Assert.False(regHelper.ValueEqualTo(
                RegistryHive.CurrentUser, regNotEmptySecondSubKey, "Key100", "WontMatch"));

            Assert.True(regHelper.ValueEqualTo(
                RegistryHive.CurrentUser, regNotEmptySecondSubKey, "Key3", 1000));
            Assert.False(regHelper.ValueEqualTo(
                RegistryHive.CurrentUser, regNotEmptySecondSubKey, "Key3", 7777));
        }

        [Test]
        public void TestCountKeyChilds()
        {
            Assert.AreEqual(regHelper.CountKeyChilds(RegistryHive.CurrentUser, regValidRoot), 3);
            Assert.AreNotEqual(regHelper.CountKeyChilds(RegistryHive.CurrentUser, regValidRoot), 10);

            Assert.Catch<Exception>(() => regHelper.CountKeyChilds(RegistryHive.CurrentUser, regNotValidRoot));
        }

        [Test]
        public void TestCountValueChilds()
        {
            Assert.AreEqual(regHelper.CountValueChilds(RegistryHive.CurrentUser, regNotEmptySecondSubKey), 3);
            Assert.AreNotEqual(regHelper.CountValueChilds(RegistryHive.CurrentUser, regNotEmptySecondSubKey), 10);

            Assert.Catch<Exception>(() => regHelper.CountValueChilds(RegistryHive.CurrentUser, regNotExistingSubKey));
        }

        [Test]
        public void TestCreateDeleteSubKey()
        {
            Assert.False(regHelper.SubKeyExists(RegistryHive.CurrentUser, regTmpSubKey));
            regHelper.CreateSubKey(RegistryHive.CurrentUser, regTmpSubKey);
            //Assert.Catch<Exception>(() => regHelper.CreateSubKey(RegistryHive.CurrentUser, regNotExistingSubKey));
            Assert.True(regHelper.SubKeyExists(RegistryHive.CurrentUser, regTmpSubKey));
            regHelper.DeleteSubKey(RegistryHive.CurrentUser, regTmpSubKey);
            Assert.Catch<Exception>(() =>
                regHelper.DeleteSubKey(RegistryHive.CurrentUser, regNotExistingSubKey));
            Assert.False(regHelper.SubKeyExists(RegistryHive.CurrentUser, regTmpSubKey));
        }

        [Test]
        public void TestCreateDeleteUpdateKeyValue()
        {
            Assert.False(regHelper.KeyExists(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "KeyNew"));

            Assert.Catch<Exception>(() => 
                regHelper.CreateKey(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "KeyNew", true));
            
            Assert.Catch<Exception>(() =>
                regHelper.CreateKey(RegistryHive.CurrentUser, regNotExistingSubKey, "KeyNew", "KeyNew"));

            regHelper.CreateKey(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "KeyNew", "ValueNew");
            Assert.True(regHelper.ValueEqualTo(RegistryHive.CurrentUser, 
                regNotEmptySecondSubKey, "KeyNew", "ValueNew"));
            
            regHelper.UpdateKeyValue(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "KeyNew", "UpdatedValue");

            Assert.Catch<Exception>(() =>
                regHelper.UpdateKeyValue(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "KeyNew", true));

            Assert.Catch<Exception>(() =>
                regHelper.UpdateKeyValue(RegistryHive.CurrentUser, regNotExistingSubKey, "KeyNew", "KeyNew"));

            Assert.True(regHelper.ValueEqualTo(RegistryHive.CurrentUser,
                regNotEmptySecondSubKey, "KeyNew", "UpdatedValue"));
            Assert.AreEqual(regHelper.ReadKeyValue(
                RegistryHive.CurrentUser, regNotEmptySecondSubKey, "KeyNew"), "UpdatedValue");

            Assert.Catch<Exception>(() =>
                regHelper.ReadKeyValue(RegistryHive.CurrentUser, regNotExistingSubKey, "KeyNew"));

            regHelper.DeleteKeyValue(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "KeyNew");

            Assert.True(regHelper.DeleteKeyValue(RegistryHive.CurrentUser, regNotExistingSubKey, "KeyNew"));

            Assert.False(regHelper.KeyExists(RegistryHive.CurrentUser, regNotEmptySecondSubKey, "KeyNew"));
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            Registry.CurrentUser.DeleteSubKeyTree(regValidRoot);
        }
    }
}
