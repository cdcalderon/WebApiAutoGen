using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.Shared.Helpers.Data;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SiteRepositoryTests
{
    [TestClass]
    public class SiteRepositorySiteImportValidationTests : SiteRepositoryTestBase
    {
        [TestMethod]
        public async Task CheckTimeZoneExists()
        {
            var dummyImport = new FileImport<SiteDto>();

            dummyImport.ImportedObjects.Add(new ImportObject<SiteDto>
            {
                Entity = new SiteDto
                {
                    SiteNumber = "123",
                    Name = "Test Site",
                    AllowedLanguages = "en-US",
                    CountryName = "Test Country",
                    TimeZone = "Fake Standard Time"
                }
            });

            await repository.ValidateSiteImport(dummyImport);

            Assert.IsTrue(dummyImport.ImportedObjects[0].ValidationErrors.Contains("Unable to find a TimeZone with the name 'Fake Standard Time'"));
        }

        [TestMethod]
        public async Task CheckCountryExists()
        {
            var dummyImport = new FileImport<SiteDto>();

            dummyImport.ImportedObjects.Add(new ImportObject<SiteDto>
            {
                Entity = new SiteDto
                {
                    SiteNumber = "123",
                    Name = "Test Site",
                    AllowedLanguages = "en-US",
                    CountryName = "Fake Country",
                    TimeZone = "Eastern Standard Time"
                }
            });

            await repository.ValidateSiteImport(dummyImport);

            Assert.IsTrue(dummyImport.ImportedObjects[0].ValidationErrors.Contains("Unable to find a Country with the name 'Fake Country'"));
        }

        [TestMethod]
        public async Task SiteNameAlreadyExists()
        {
            var dummyImport = new FileImport<SiteDto>();

            dummyImport.ImportedObjects.Add(new ImportObject<SiteDto>
            {
                Entity = new SiteDto
                {
                    SiteNumber = "123",
                    Name = "Base Site",
                    AllowedLanguages = "en-US",
                    CountryName = "Test Country",
                    TimeZone = "Eastern Standard Time"
                }
            });

            await repository.ValidateSiteImport(dummyImport);

            Assert.IsTrue(dummyImport.ImportedObjects[0].ValidationErrors.Contains("A site already exists with the name 'Base Site'"));
        }

        [TestMethod]
        public async Task SiteNumberAlreadyExists()
        {
            var dummyImport = new FileImport<SiteDto>();

            dummyImport.ImportedObjects.Add(new ImportObject<SiteDto>
            {
                Entity = new SiteDto
                {
                    SiteNumber = "100",
                    Name = "Test",
                    AllowedLanguages = "en-US",
                    CountryName = "Test Country",
                    TimeZone = "Eastern Standard Time"
                }
            });

            await repository.ValidateSiteImport(dummyImport);

            Assert.IsTrue(dummyImport.ImportedObjects[0].ValidationErrors.Contains("The site number '100' is already in use"));
        }

        [TestMethod]
        public async Task InvalidLanguageCheck()
        {
            var dummyImport = new FileImport<SiteDto>();

            dummyImport.ImportedObjects.Add(new ImportObject<SiteDto>
            {
                Entity = new SiteDto
                {
                    SiteNumber = "123",
                    Name = "Test",
                    AllowedLanguages = "en-GB",
                    CountryName = "Test Country",
                    TimeZone = "Eastern Standard Time"
                }
            });

            await repository.ValidateSiteImport(dummyImport);

            Assert.IsTrue(dummyImport.ImportedObjects[0].ValidationErrors.Contains("Unable to find a language code with the name 'en-GB'"));
        }

        [TestMethod]
        public async Task ValidateSiteNumberLengthCheck()
        {
            var dummyImport = new FileImport<SiteDto>();

            dummyImport.ImportedObjects.Add(new ImportObject<SiteDto>
            {
                Entity = new SiteDto
                {
                    SiteNumber = "1234",
                    Name = "Test",
                    AllowedLanguages = "en-US",
                    CountryName = "Test Country",
                    TimeZone = "Eastern Standard Time"
                }
            });

            await repository.ValidateSiteImport(dummyImport);

            Assert.IsTrue(dummyImport.ImportedObjects[0].ValidationErrors.Contains("site number must be exactly '3' numbers"));
        }

        [TestMethod]
        public async Task ValidateAtLeastOneLanguageSpecified()
        {    
            var dummyImport = new FileImport<SiteDto>();

            dummyImport.ImportedObjects.Add(new ImportObject<SiteDto>
            {
                Entity = new SiteDto
                {
                    SiteNumber="123",
                    Name="Test",
                    AllowedLanguages = null,
                    CountryName = "Test Country",
                    TimeZone = "Eastern Standard Time"
                }
            });

            await repository.ValidateSiteImport(dummyImport);

            Assert.IsTrue(dummyImport.ImportedObjects[0].ValidationErrors.Contains("At least one site language must be included"));
        }
    }
}
