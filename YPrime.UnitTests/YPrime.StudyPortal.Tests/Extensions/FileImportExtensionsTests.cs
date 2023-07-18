using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using YPrime.eCOA.DTOLibrary;
using YPrime.Shared.Helpers.Data;
using YPrime.StudyPortal.Extensions;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Extensions
{
    [TestClass]
    public class FileImportExtensionsTests
    {
        private const string TestSite1BrokenName = "<script>alert(123)</script>";
        private const string TestSite1SanitizedName = "&lt;script&gt;alert(123)&lt;/script&gt;";
        private const string TestSite2BrokenName = "><script>alert(document.cookie)</script>";
        private const string TestSite2SanitizedName = "&gt;&lt;script&gt;alert(document.cookie)&lt;/script&gt;";
        private const string TestSite3ValidName = "Test Site";

        [TestMethod]
        public void SanitizeTest()
        {
            var testSite1 = new SiteDto
            {
                Name = TestSite1BrokenName
            };

            var testSite2 = new SiteDto
            {
                Name = TestSite2BrokenName
            };

            var testSite3 = new SiteDto
            {
                Name = TestSite3ValidName
            };

            var siteImport = new FileImport<SiteDto>()
            {
                ImportedObjects = new List<Shared.Helpers.Data.Interfaces.IImportObject>
                {
                    ImportObject.Create(testSite1),
                    ImportObject.Create(testSite2),
                    ImportObject.Create(testSite3)
                }
            };

            siteImport.Sanitize();

            var testSite1Result = siteImport.ImportedObjects[0].Entity as SiteDto;
            var testSite2Result = siteImport.ImportedObjects[1].Entity as SiteDto;
            var testSite3Result = siteImport.ImportedObjects[2].Entity as SiteDto;

            Assert.AreEqual(TestSite1SanitizedName, testSite1Result.Name);
            Assert.AreEqual(TestSite2SanitizedName, testSite2Result.Name);
            Assert.AreEqual(TestSite3ValidName, testSite3Result.Name);
        }
    }
}
