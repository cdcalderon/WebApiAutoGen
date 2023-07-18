using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Repositories;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.Reports.Reports;

namespace YPrime.BusinessLayer.UnitTests.Reports
{
    [TestClass]
    public class SiteDetailsReportTests
    {
        private Mock<IStudyDbContext> MockContext = new Mock<IStudyDbContext>();
        private Mock<ICountryService> MockCountryService = new Mock<ICountryService>();
        private Mock<IVisitService> MockVisitService = new Mock<IVisitService>();
        private UserRepository _userRepository;
        private List<VisitModel> Visits;
        private VisitModel ScreeningVisit;
        private List<Site> Sites;
        private Site SiteA;
        private Site SiteB;
        private Site SiteC;
        private CountryModel UnitedStatesCountry;
        private Guid TestStudyUserId;
        private DateTimeOffset FirstExpectedScreeningDate;


        [TestInitialize]
        public void TestInitialize()
        {
            TestStudyUserId = Guid.NewGuid();

            MockCountryService = new Mock<ICountryService>();

            UnitedStatesCountry = new CountryModel
            {
                Id = Guid.NewGuid(),
                Name = "United States"
            };

            MockCountryService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(new List<CountryModel>
                {
                    UnitedStatesCountry
                });

            MockVisitService = new Mock<IVisitService>();

            var visitOne = new VisitModel
            {
                Id = Guid.NewGuid(),
                VisitOrder = 1
            };

            ScreeningVisit = new VisitModel
            {
                Id = Guid.NewGuid(),
                VisitOrder = 2
            };

            Visits = new List<VisitModel>
            {
                visitOne,
                ScreeningVisit
            };

            MockVisitService
                .Setup(s => s.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(Visits);

            MockContext = new Mock<IStudyDbContext>();

            FirstExpectedScreeningDate = DateTime.Now.AddDays(-5);

            SiteA = new Site
            {
                Id = Guid.NewGuid(),
                CountryId = UnitedStatesCountry.Id,
                SiteNumber = "01",
                Investigator = "Investigator A",
                IsActive = true,
                Patient = new List<Patient>
                {
                    new Patient
                    {
                        Id = Guid.NewGuid(),
                        PatientVisits = new List<PatientVisit>
                        {
                            new PatientVisit
                            {
                                Id = Guid.NewGuid(),
                                VisitId = ScreeningVisit.Id,
                                VisitDate = FirstExpectedScreeningDate
                            }
                        }
                    }
                },
                SiteActiveHistory = new List<SiteActiveHistory>
                {
                    new SiteActiveHistory
                    {
                        Previous = false,
                        Current = true,
                    },
                    new SiteActiveHistory
                    {
                        Previous = false,
                        Current = true,
                    },
                     new SiteActiveHistory
                    {
                        Previous = true,
                        Current = false,
                    }
                }
            };

            SiteA.StudyUserRoles = new List<StudyUserRole>
                {
                    new StudyUserRole
                    {
                        StudyUserId = TestStudyUserId,
                        Site = SiteA
                    }
                };

            SiteB = new Site
            {
                Id = Guid.NewGuid(),
                CountryId = UnitedStatesCountry.Id,
                SiteNumber = "02",
                Investigator = "Investigator B",
                IsActive = false
            };

            SiteB.StudyUserRoles = new List<StudyUserRole>
                {
                    new StudyUserRole
                    {
                        StudyUserId = Guid.NewGuid(),
                        Site = SiteB
                    }
                };

            SiteC = new Site
            {
                Id = Guid.NewGuid(),
                CountryId = UnitedStatesCountry.Id,
                SiteNumber = "03",
                Investigator = "Investigator C",
                IsActive = false,
                Patient = new List<Patient>
                {
                    new Patient
                    {
                        Id = Guid.NewGuid(),
                        PatientVisits = new List<PatientVisit>
                        {
                            new PatientVisit
                            {
                                Id = Guid.NewGuid(),
                                VisitId = ScreeningVisit.Id,
                                VisitDate = FirstExpectedScreeningDate
                            }
                        }
                    }
                },
                SiteActiveHistory = new List<SiteActiveHistory>
                {
                    new SiteActiveHistory
                    {
                        Previous = true,
                        Current = false,
                    }
                }
            };

            SiteC.StudyUserRoles = new List<StudyUserRole>
                {
                    new StudyUserRole
                    {
                        StudyUserId = TestStudyUserId,
                        Site = SiteC
                    }
                };

            Sites = new List<Site>
            {
                SiteC,
                SiteA,
                SiteB,
            };

            var siteDbSet = new FakeDbSet<Site>(Sites);
            MockContext
                .Setup(c => c.Sites)
                .Returns(siteDbSet.Object);

            var studyUserRolesDbSet = new FakeDbSet<StudyUserRole>(Sites.SelectMany(s => s.StudyUserRoles));
            MockContext
                .Setup(c => c.StudyUserRoles)
                .Returns(studyUserRolesDbSet.Object);

            var mockAuthenticationUserRepository = new Mock<IAuthenticationUserRepository>();
            var mockStudyRoleService = new Mock<IStudyRoleService>();
            var mockSoftwareReleaseRepository = new Mock<ISoftwareReleaseRepository>();

            _userRepository = new UserRepository(
                MockContext.Object, 
                mockAuthenticationUserRepository.Object,
                mockStudyRoleService.Object,
                mockSoftwareReleaseRepository.Object);
        }

        [TestMethod]
        public async Task GetGridDataTest()
        {
            var report = GetReport();

            var result = await report
                .GetGridData(null, TestStudyUserId);

            var expectedScreeningDate = FirstExpectedScreeningDate.ToString("dd-MMM-yyyy");
            var expectedAcviationDate = DateTime.Today.ToString("dd-MMM-yyyy");

            Assert.AreEqual(2, result.Count);

            var firstResult = result.First();

            Assert.AreEqual(UnitedStatesCountry.Name, firstResult["CountryName"]);
            Assert.AreEqual(SiteA.SiteNumber, firstResult["SiteNumber"]);
            Assert.AreEqual(SiteA.Investigator, firstResult["Investigator"]);
            Assert.AreEqual("Active", firstResult["IsActive"]);
            Assert.AreEqual(expectedScreeningDate, firstResult["ScreeningFirst"]);
            Assert.AreEqual(expectedAcviationDate, firstResult["DateOfActivation"]);
            Assert.AreEqual(expectedAcviationDate, firstResult["DateOfDeactivation"]);
            Assert.AreEqual(expectedAcviationDate, firstResult["DateOfReactivation"]);

            var secondResult = result.Skip(1).First();

            Assert.AreEqual(UnitedStatesCountry.Name, secondResult["CountryName"]);
            Assert.AreEqual(SiteC.SiteNumber, secondResult["SiteNumber"]);
            Assert.AreEqual(SiteC.Investigator, secondResult["Investigator"]);
            Assert.AreEqual("Inactive", secondResult["IsActive"]);
            Assert.AreEqual(expectedScreeningDate, secondResult["ScreeningFirst"]);
            Assert.IsNull(secondResult["DateOfActivation"]);
            Assert.AreEqual(expectedAcviationDate, secondResult["DateOfDeactivation"]);
            Assert.IsNull(secondResult["DateOfReactivation"]);
        }

        [TestMethod]
        public void GetColumnHeadingsTest()
        {
            var report = GetReport();

            var result = report.GetColumnHeadings();

            Assert.AreEqual(8, result.Count);
        }

        [TestMethod]
        public async Task GetReportChartDataTest()
        {
            var report = GetReport();

            var result = await report.GetReportChartData(
                null,
                Guid.NewGuid());

            Assert.IsNull(result);
        }

        private SiteDetailsReport GetReport()
        {
            var report = new SiteDetailsReport(
                MockContext.Object,
                MockCountryService.Object,
                MockVisitService.Object,
                _userRepository);

            return report;
        }
    }
}
