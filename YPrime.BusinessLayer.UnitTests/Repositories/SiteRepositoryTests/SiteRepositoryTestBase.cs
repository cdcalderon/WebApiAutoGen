using Moq;
using System;
using System.Collections.Generic;
using YPrime.BusinessLayer.Repositories;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SiteRepositoryTests
{
    public abstract class SiteRepositoryTestBase
    {
        protected readonly Mock<IStudyDbContext> Context;
        protected readonly Mock<IStudySettingService> StudySettingService;
        protected readonly Mock<ICountryService> MockCountryService;
        protected readonly Mock<ILanguageService> MockLanguageService;               

        protected readonly SiteRepository repository;
        protected List<Site> BaseSites;
        protected List<CountryModel> Countries;
        protected List<LanguageModel> Languages;

        protected FakeDbSet<Site> SiteDataset;
        protected Guid BaseSiteId;
        protected Guid JapaneseSiteId;
        protected LanguageModel English;
        protected LanguageModel Japanese;
        protected Site JapaneseSite;

        protected SiteRepositoryTestBase()
        {
            Context = new Mock<IStudyDbContext>();
            StudySettingService = new Mock<IStudySettingService>();
            MockCountryService = new Mock<ICountryService>();
            MockLanguageService = new Mock<ILanguageService>();
                      
            repository = new SiteRepository(Context.Object, null, 
                MockLanguageService.Object, 
                new Mock<IStudyRoleService>().Object,
                new Mock<IPatientStatusService>().Object,
                 MockCountryService.Object,
            StudySettingService.Object);

            SetupLanguages();
            SetupContext();
            SetupMocks();
        }

        private void SetupLanguages()
        {
            English = new LanguageModel
            {
                Id = Config.Defaults.Languages.English.Id,
                DisplayName = Config.Defaults.Languages.English.DisplayName,
                CultureName = Config.Defaults.Languages.English.CultureName
            };

            Japanese = new LanguageModel
            {
                Id = Guid.NewGuid(),
                DisplayName = "Japanese (Japan)",
                CultureName = "ja-JP"
            };

            Languages = new List<LanguageModel>
            {
                English,
                Japanese
            };
        }

        private void SetupContext()
        {
            BaseSiteId = Guid.NewGuid();
            JapaneseSiteId = Guid.NewGuid();

            var baseSite = new Site
            {
                Id = BaseSiteId,
                SiteNumber = "100",
                Name = "Base Site",
                TimeZone = "Eastern Standard Time"
            };

            JapaneseSite = new Site
            {
                Id = JapaneseSiteId,
                SiteNumber = "101",
                Name = nameof(JapaneseSite),
                TimeZone = "Japan Standard Time"
            };

            BaseSites = new List<Site>
            {
                baseSite
            };

            SiteDataset = new FakeDbSet<Site>(BaseSites);
            SiteDataset.Setup(d => d.Find(It.Is<Guid>(id => id == baseSite.Id))).Returns(baseSite);
            SiteDataset.Setup(d => d.Find(It.Is<Guid>(id => id == JapaneseSiteId))).Returns(JapaneseSite);

            Context.Setup(c => c.Sites).Returns(SiteDataset.Object);

            var siteLanguageDataset = new FakeDbSet<SiteLanguage>(new List<SiteLanguage>
            {
                new SiteLanguage
                {
                    Id = Guid.NewGuid(),
                    SiteId = BaseSiteId,
                    LanguageId = English.Id
                },
                new SiteLanguage
                {
                    Id = Guid.NewGuid(),
                    SiteId = BaseSiteId,
                    LanguageId = Japanese.Id
                },
                new SiteLanguage
                {
                    Id = Guid.NewGuid(),
                    SiteId = JapaneseSiteId,
                    LanguageId = Japanese.Id
                }
            });

            Context.Setup(c => c.SiteLanguages).Returns(siteLanguageDataset.Object);
        }
    
        private void SetupMocks()
        {
            Countries = new List<CountryModel>
            {
                new CountryModel
                {
                    Id = Guid.NewGuid(),
                    Name= "Test Country"
                }
            };

            MockCountryService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(Countries);

            MockLanguageService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(Languages);

            StudySettingService
            .Setup(r => r.Get(It.Is<string>(t => t == "WebBackupTabletEnabled"), It.IsAny<Guid?>()))
            .ReturnsAsync(new StudySettingModel
            {
                Value = "5",
                Properties = new StudySettingProperties()
                {
                    Key = "WebBackupTabletEnabled",
                }
            });

            StudySettingService
            .Setup(r => r.GetStringValue(It.Is<string>(t => t == "SiteNumberLength"), It.IsAny<Guid?>()))
            .ReturnsAsync("3");


        }
    }
}