using System.Collections.Generic;
using Config.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.BusinessLayer.Extensions;

namespace YPrime.BusinessLayer.UnitTests.Extensions
{
    [TestClass]
    public class WebBackupTypeExtensionsTests
    {
        [TestMethod]
        public void WebBackupTypeExtensionsIsTabletTest()
        {
            var tabletTypes = new List<WebBackupType>
            {
                WebBackupType.TabletCaregiver,
                WebBackupType.TabletClinician,
                WebBackupType.TabletPatient
            };

            var nonTabletTypes = new List<WebBackupType>
            {
                WebBackupType.HandheldPatient,
                WebBackupType.None
            };

            foreach (var tabletType in tabletTypes)
            {
                Assert.IsTrue(tabletType.IsTabletType());
            }

            foreach (var nonTabletType in nonTabletTypes)
            {
                Assert.IsFalse(nonTabletType.IsTabletType());
            }
        }
    }
}