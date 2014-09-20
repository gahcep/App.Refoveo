using System;
using System.Reflection;
using App.Refoveo.Helpers;
using NUnit.Framework;

namespace App.Refoveo.Tests.Helpers
{
    [TestFixture]
    [Category("Helpers")]
    public class AssemblyHelperTests
    {
        [Test]
        public void TestFileVersionInvalidCases()
        {
            Assembly empty = null;
            Assert.Catch<ArgumentNullException>(() => AssemblyHelper.FileVersion(empty));
            Assert.Catch<ArgumentException>(() => AssemblyHelper.FileVersion(""));
        }
    }
}
