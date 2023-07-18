using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.UnitTests.YPrime.PatientPortal.Tests.Controllers;

namespace YPrime.UnitTests
{
    public class SiteTestSetup : BaseControllerTest
    {
        public Mock<IStudyDbContext> _dbContext;
        public Mock<IStudySettingService> MockStudySettingSerice;
        public Mock<MockableDbSetWithExtensions<Patient>> _patients;
        public Mock<MockableDbSetWithExtensions<PatientVisit>> _patientVisit;
        public Mock<MockableDbSetWithExtensions<Site>> _sites;
        public List<StudySettingModel> _studySettings;
        public Mock<MockableDbSetWithExtensions<SystemActionStudyRole>> _systemActionStudyRole;
        public Mock<MockableDbSetWithExtensions<StudyUserRole>> _userRole;
        public Mock<MockableDbSetWithExtensions<StudyUser>> _users;
        public Guid mainPatientGuid = Guid.Parse("32345678-1234-1234-1234-123456789123");

        public Guid mainSiteGuid = Guid.Parse("12345678-1234-1234-1234-123456789123");
        public Guid mainStudyUserGuid = Guid.Parse("42345678-1234-1234-1234-123456789123");
        public Guid secondarySiteGuid = Guid.Parse("22345678-1234-1234-1234-123456789123");
        public Guid countrySiteGuid = Guid.NewGuid();

        public string SiteUserCultureCode = "en-US";

        [TestInitialize]
        public override void Initialize()
        {
            MockStudySettingSerice = new Mock<IStudySettingService>();

            _sites = CreateDbSetMock(new List<Site>
            {
                new Site
                {
                    Name = "Test Site",
                    SiteNumber = "1",
                    Id = mainSiteGuid,
                    IsActive = true,
                    CountryId = countrySiteGuid
                },

                new Site
                {
                    Name = "Site 2",
                    SiteNumber = "2",
                    Id = secondarySiteGuid,
                    IsActive = false,
                    CountryId = countrySiteGuid
                }
            });

            var statusTypes = new List<PatientStatusModel>
            {
                new PatientStatusModel
                {
                    Id = 1,
                    Name = "Screened"
                },
                new PatientStatusModel
                {
                    Id = 2,
                    Name = "Removed"
                }
            };

            _patients = CreateDbSetMock(new List<Patient>
            {
                new Patient
                {
                    Id = mainPatientGuid,
                    PatientNumber = "12345",
                    SiteId = mainSiteGuid,
                    Site = new Site
                    {
                        Id = mainSiteGuid,
                        IsActive = true
                    },
                    PatientStatusTypeId = statusTypes.First().Id,
                    EnrolledDate = new DateTime(2017, 1, 5, 12, 0, 0),
                    PatientVisits = new List<PatientVisit>
                    {
                        new PatientVisit
                        {
                            VisitDate = new DateTimeOffset(2017, 2, 5, 12, 0, 0, TimeSpan.Zero),                           
                            ProjectedDate = new DateTimeOffset(2017, 2, 9, 12, 0, 0, TimeSpan.Zero)
                        }
                    }
                },

                new Patient
                {
                    Id = Guid.Parse("52345678-1234-1234-1234-123456789123"),
                    PatientNumber = "54321",
                    SiteId = secondarySiteGuid
                }
            });

            _studySettings = new List<StudySettingModel>
            {
                new StudySettingModel
                {
                    Value = "5",
                    Properties = new StudySettingProperties()
                    {
                        Key = "PatientNumberLength",
                        Group = "Study Variable"
                    }
                },

                new StudySettingModel
                {
                    Value = "1",
                    Properties = new StudySettingProperties()
                    {
                        Key = "PatientNumberIncludeSiteId",
                        Group = "group"
                    }
                },

                new StudySettingModel
                {
                    Value = "3",
                    Properties = new StudySettingProperties()
                    {
                        Key = "PatientNumberPrefix",
                        Group = "group"
                    }
                },

                new StudySettingModel
                {
                    Value = "2",
                    Properties = new StudySettingProperties()
                    {
                        Key = "PatientNumberPrefixSiteSeparator",
                        Group = "group"
                    }
                },

                new StudySettingModel
                {
                    Value = "4",
                    Properties = new StudySettingProperties()
                    {
                        Key = "PatientNumberSiteSubjectNumberSeparator",
                        Group = "group"
                    }
                },
                new StudySettingModel
                {
                    Properties = new StudySettingProperties()
                    {
                        Key = "Study Variable"
                    }
                }
            };

            MockStudySettingSerice
                .Setup(r => r.GetAll(It.IsAny<Guid?>()))
                .ReturnsAsync(_studySettings);

            _users = CreateDbSetMock(new List<StudyUser>
            {
                new StudyUser
                {
                    Id = mainStudyUserGuid,
                    UserName = "email@email.com"
                },

                new StudyUser
                {
                    Id = Guid.Parse("42345678-1234-1234-1234-123456789121"),
                    UserName = "email2@email.com",
                    StudyUserRoles = new List<StudyUserRole>
                    {
                        new StudyUserRole
                        {
                            Id = Guid.Parse("62345678-1234-1234-1234-123456789121"),
                            StudyUserId = Guid.Parse("42345678-1234-1234-1234-123456789121"),
                            StudyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789121"),
                            SiteId = mainSiteGuid,
                        }
                    }
                }
            });

            _userRole = CreateDbSetMock(new List<StudyUserRole>
            {
                new StudyUserRole
                {
                    Id = Guid.Parse("62345678-1234-1234-1234-123456789123"),
                    StudyUserId = mainStudyUserGuid,
                    StudyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789123"),
                    SiteId = mainSiteGuid,
                    Site = new Site
                    {
                        SiteNumber = "1"
                    },
                    StudyUser = new StudyUser
                    {
                        Id = mainStudyUserGuid,
                        UserName = "email@email.com"
                    }
                },

                new StudyUserRole
                {
                    Id = Guid.Parse("62345678-1234-1234-1234-123456789121"),
                    StudyUserId = Guid.Parse("42345678-1234-1234-1234-123456789121"),
                    StudyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789121"),
                    SiteId = mainSiteGuid,
                    StudyUser = new StudyUser
                    {
                        Id = Guid.Parse("42345678-1234-1234-1234-123456789121"),
                        UserName = "email2@email.com"
                    }
                }
            });

            _patientVisit = CreateDbSetMock(new List<PatientVisit>
            {
                new PatientVisit
                {
                    Id = Guid.Parse("12345678-1234-1234-1234-123456789124")
                }
            });

            _systemActionStudyRole = CreateDbSetMock(new List<SystemActionStudyRole>
            {
                new SystemActionStudyRole
                {
                    SystemActionId = Guid.Parse("77345678-1234-1234-1234-123456789123"),
                    StudyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789123")
                }
            });

            _dbContext = new Mock<IStudyDbContext>();
            _dbContext.Setup(x => x.Sites).Returns(_sites.Object);
            _dbContext.Setup(x => x.Patients).Returns(_patients.Object);
            _dbContext.Setup(x => x.StudyUsers).Returns(_users.Object);
            _dbContext.Setup(x => x.StudyUserRoles).Returns(_userRole.Object);
            _dbContext.Setup(x => x.PatientVisits).Returns(_patientVisit.Object);
            _dbContext.Setup(x => x.SystemActionStudyRoles).Returns(_systemActionStudyRole.Object);

            base.Initialize();
        }
    }
}