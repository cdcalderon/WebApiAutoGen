using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Enums;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Repositories;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Models;
using YPrime.Data.Study.Proxies;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DataCopyRepositoryTests
{
    [TestClass]
    public class DataCopyRepositoryTests : LegacySiteTestSetup
    {
        protected string TestDevDataApiPath = string.Empty;
        private const string ExpectedSystemUser = "SYSTEM";

        protected Mock<MockableDbSetWithExtensions<SystemActionStudyRole>> _systemActionStudyRoleDbSet;
        protected Mock<MockableDbSetWithExtensions<SystemAction>> _systemActionDbSet;
        protected Mock<MockableDbSetWithExtensions<EmailContentStudyRole>> _emailContentStudyRoleDbSet;
        protected Mock<MockableDbSetWithExtensions<ReportStudyRole>> _reportStudyRoleDbSet;
        protected Mock<MockableDbSetWithExtensions<StudyRoleUpdate>> _studyRoleUpdateDbSet;
        protected Mock<MockableDbSetWithExtensions<StudyRoleWidget>> _studyRoleWidgetDbSet;
        protected Mock<MockableDbSetWithExtensions<AnalyticsReferenceStudyRole>> _analyticsReferenceStudyRoleDbSet;
        protected Mock<MockableDbSetWithExtensions<AnalyticsReference>> _analyticsReferenceDbSet;
        protected Mock<IDbContextTransactionProxy> _mockTransaction;

        private List<StudyRoleWidget> _studyRoleWidgets;
        private List<StudyRoleModel> _studyRoleModels;
        private List<SystemAction> _systemActions;
        private List<SystemActionStudyRole> _systemActionStudyRoles;
        private List<EmailContentStudyRole> _emailContentStudyRoles;
        private List<ReportStudyRole> _reportStudyRoles;
        private List<StudyRoleUpdate> _studyRoleUpdates;
        private List<AnalyticsReference> _analyticsReferences;
        private List<AnalyticsReferenceStudyRole> _analyticsReferenceStudyRoles;

        private List<ReportStudyRole> _reportStudyRolesToBeAdded;
        private List<ReportStudyRole> _reportStudyRolesToBeRemoved;

        private readonly Guid _studyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789123");
        private readonly Guid _studyRoleId2 = Guid.NewGuid();
        private readonly Guid _studyRoleId3 = Guid.NewGuid();
        private readonly Guid _systemActionId = Guid.NewGuid();

        private StudyPortalConfigDataDto DevApiData = null;

        [TestInitialize]
        public void InitializeRoleTests()
        {
            MockPatientVisitRepository = new Mock<IPatientVisitRepository>();
            MockAuthenticationRepository = new Mock<IAuthenticationUserRepository>();
            MockStudyRoleService = new Mock<IStudyRoleService>();

            _studyRoleModels = new List<StudyRoleModel>
            {
                new StudyRoleModel
                {
                    Id = _studyRoleId,
                    IsBlinded = true,
                    ShortName ="AB"
                },
                new StudyRoleModel
                {
                    Id = _studyRoleId2,
                    IsBlinded = true,
                    ShortName ="XY"
                },
                new StudyRoleModel
                {
                    Id = _studyRoleId3,
                    IsBlinded = false,
                    ShortName ="ZZ"
                }
            };

            _systemActions = new List<SystemAction>
            {
                new SystemAction
                {
                        Id = _systemActionId,
                        IsBlinded = true,
                        Name = "SA"
                }
            };

            _systemActionStudyRoles = new List<SystemActionStudyRole>
            {
                new SystemActionStudyRole
                {
                    SystemActionId = Guid.Parse("77345678-1234-1234-1234-123456789123"),
                    StudyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789123"),
                    SystemAction = new SystemAction
                    {
                        IsBlinded = true,
                        Name = "SA"
                    }
                }
            };

            _emailContentStudyRoles = new List<EmailContentStudyRole>
            {
                new EmailContentStudyRole
                {
                    EmailContentId = Guid.Parse("77345678-1234-1234-1234-123456789123"),
                    StudyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789123"),
                    EmailContent = new EmailContent
                    {
                        Id = Guid.Parse("77345678-1234-1234-1234-123456789123")
                    }
                }
            };

            _reportStudyRoles = new List<ReportStudyRole>
            {
                new ReportStudyRole
                {
                    ReportId = ReportType.SiteDetailsReport.Id,
                    StudyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789123")
                },
                new ReportStudyRole
                {
                    ReportId = ReportType.StudyUserReportByUser.Id,
                    StudyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789123")
                }
            };

            _studyRoleUpdates = new List<StudyRoleUpdate>
            {
                new StudyRoleUpdate
                {
                    StudyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789123"),
                    LastUpdate = DateTime.Now.AddDays(-1).ToUniversalTime()
                }
            };

            _studyRoleWidgets = new List<StudyRoleWidget>
            {
                new StudyRoleWidget
                {
                    Id = Guid.NewGuid(),
                    StudyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789123"),
                    WidgetId = Guid.NewGuid(),
                    WidgetPosition = 1,
                    Widget = new Widget
                    {
                        Id = Guid.NewGuid()
                    }

                }
            };

            var testAnalyticsReference = new AnalyticsReference
            {
                Id = Guid.NewGuid(),
                InternalName = "Sample Analytics Reference",
                DisplayName = "Sample Analytics Reference",
                Description = "Sample Analytics Reference",
                DisplayOrder = 1
            };

            _analyticsReferences = new List<AnalyticsReference>
            {
                testAnalyticsReference
            };

            _analyticsReferenceStudyRoles = new List<AnalyticsReferenceStudyRole>
            {
                new AnalyticsReferenceStudyRole
                {
                    AnalyticsReferenceId = testAnalyticsReference.Id,
                    StudyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789123"),
                    AnalyticsReference = testAnalyticsReference
                }
            };
                
            MockStudyRoleService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(_studyRoleModels);

            _systemActionDbSet = CreateDbSetMock(_systemActions);
            _systemActionStudyRoleDbSet = CreateDbSetMock(_systemActionStudyRoles);
            _emailContentStudyRoleDbSet = CreateDbSetMock(_emailContentStudyRoles);
            _reportStudyRoleDbSet = CreateDbSetMock(_reportStudyRoles);
            _studyRoleUpdateDbSet = CreateDbSetMock(_studyRoleUpdates);
            _studyRoleWidgetDbSet = CreateDbSetMock(_studyRoleWidgets);
            _analyticsReferenceDbSet = CreateDbSetMock(_analyticsReferences);
            _analyticsReferenceStudyRoleDbSet = CreateDbSetMock(_analyticsReferenceStudyRoles);

            _dbContext.Setup(x => x.SystemActionStudyRoles).Returns(_systemActionStudyRoleDbSet.Object);
            _dbContext.Setup(x => x.EmailContentStudyRoles).Returns(_emailContentStudyRoleDbSet.Object);
            _dbContext.Setup(x => x.ReportStudyRoles).Returns(_reportStudyRoleDbSet.Object);
            _dbContext.Setup(x => x.StudyRoleUpdates).Returns(_studyRoleUpdateDbSet.Object);
            _dbContext.Setup(x => x.SystemActions).Returns(_systemActionDbSet.Object);
            _dbContext.Setup(x => x.StudyRoleWidgets).Returns(_studyRoleWidgetDbSet.Object);
            _dbContext.Setup(x => x.AnalyticsReferenceStudyRoles).Returns(_analyticsReferenceStudyRoleDbSet.Object);
            _dbContext.Setup(x => x.AnalyticsReferences).Returns(_analyticsReferenceDbSet.Object);

            _dbContext.Setup(x => x.CorrectionStatuses).Returns(CreateDbSetMock(new List<CorrectionStatus>()).Object);
            _dbContext.Setup(x => x.EmailContents).Returns(CreateDbSetMock(new List<EmailContent>()).Object);
            _dbContext.Setup(x => x.InputFieldTypeResults).Returns(CreateDbSetMock(new List<InputFieldTypeResult>()).Object);
            _dbContext.Setup(x => x.MissedVisitReasons).Returns(CreateDbSetMock(new List<MissedVisitReason>()).Object);
            _dbContext.Setup(x => x.QuestionInputFieldTypeResults).Returns(CreateDbSetMock(new List<QuestionInputFieldTypeResult>()).Object);
            _dbContext.Setup(x => x.ScreenReportDialog).Returns(CreateDbSetMock(new List<ScreenReportDialog>()).Object);
            _dbContext.Setup(x => x.SecurityQuestions).Returns(CreateDbSetMock(new List<SecurityQuestion>()).Object);
            _dbContext.Setup(x => x.VisitComplianceReportQuestionnaireTypes).Returns(CreateDbSetMock(new List<VisitComplianceReportQuestionnaireType>()).Object);
            _dbContext.Setup(x => x.Widgets).Returns(CreateDbSetMock(new List<Widget>()).Object);
            _dbContext.Setup(x => x.WidgetCounts).Returns(CreateDbSetMock(new List<WidgetCount>()).Object);
            _dbContext.Setup(x => x.WidgetLinks).Returns(CreateDbSetMock(new List<WidgetLink>()).Object);
            _dbContext.Setup(x => x.WidgetSystemActions).Returns(CreateDbSetMock(new List<WidgetSystemAction>()).Object);

            _reportStudyRolesToBeAdded = new List<ReportStudyRole>
            {
                new ReportStudyRole
                {
                    ReportId = ReportType.PatientAuditRecReportFiltered.Id,
                    StudyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789123")
                }
            };

            _reportStudyRolesToBeRemoved = new List<ReportStudyRole>
            {
                _reportStudyRoles.Skip(1).First()
            };

            DevApiData = new StudyPortalConfigDataDto
            {
                EmailContentStudyRoles = _emailContentStudyRoles.ToList(),
                StudyRoleUpdates = _studyRoleUpdates.ToList(),
                StudyRoleWidgets = _studyRoleWidgets.ToList(),
                SystemActionStudyRoles = _systemActionStudyRoles.ToList(),
                AnalyticsReferenceStudyRoles = _analyticsReferenceStudyRoles.ToList()
            };

            DevApiData.ReportStudyRoles.AddRange(_reportStudyRoles.ToList());
            DevApiData.ReportStudyRoles.AddRange(_reportStudyRolesToBeAdded.ToList());

            foreach (var dataToRemove in _reportStudyRolesToBeRemoved)
            {
                DevApiData.ReportStudyRoles.Remove(dataToRemove);
            }

            _mockTransaction = new Mock<IDbContextTransactionProxy>();

            _dbContext
                .Setup(db => db.BeginTransaction())
                .Returns(_mockTransaction.Object);
        }

        [TestMethod]
        public async Task GetStudyPortalConfigDataTest()
        {
            var repository = new DataCopyRepository(_dbContext.Object);

            var result = await repository.GetStudyPortalConfigData();

            _dbContext.Verify(c => c.EmailContentStudyRoles, Times.Once);
            _dbContext.Verify(c => c.ReportStudyRoles, Times.Once);
            _dbContext.Verify(c => c.StudyRoleUpdates, Times.Once);
            _dbContext.Verify(c => c.StudyRoleWidgets, Times.Once);
            _dbContext.Verify(c => c.SystemActionStudyRoles, Times.Once);
            _dbContext.Verify(c => c.AnalyticsReferenceStudyRoles, Times.Once);
            _dbContext.Verify(c => c.AnalyticsReferences, Times.Once);

            Assert.AreEqual(_emailContentStudyRoles.Count, result.EmailContentStudyRoles.Count);
            Assert.AreEqual(_reportStudyRoles.Count, result.ReportStudyRoles.Count);
            Assert.AreEqual(_studyRoleUpdates.Count, result.StudyRoleUpdates.Count);
            Assert.AreEqual(_studyRoleWidgets.Count, result.StudyRoleWidgets.Count);
            Assert.AreEqual(_systemActionStudyRoles.Count, result.SystemActionStudyRoles.Count);
            Assert.AreEqual(_analyticsReferences.Count, result.AnalyticsReferences.Count);
            Assert.AreEqual(_analyticsReferenceStudyRoles.Count, result.AnalyticsReferenceStudyRoles.Count);

            foreach (var emailContentStudyRole in result.EmailContentStudyRoles)
            {
                Assert.IsNull(emailContentStudyRole.EmailContent);
            }

            foreach (var studyRoleWidget in result.StudyRoleWidgets)
            {
                Assert.IsNull(studyRoleWidget.Widget);
            }

            foreach (var systemActionStudyRole in result.SystemActionStudyRoles)
            {
                Assert.IsNull(systemActionStudyRole.SystemAction);
            }

            foreach(var analyticsReferenceStudyRole in result.AnalyticsReferenceStudyRoles)
            {
                Assert.IsNull(analyticsReferenceStudyRole.AnalyticsReference);
            }
        }

        [TestMethod]
        public async Task UpdateStudyPortalConifgDataTest()
        {
            var addedReportStudyRoles = new List<ReportStudyRole>();
            var removedReportStudyRoles = new List<ReportStudyRole>();

            _reportStudyRoleDbSet
                .Setup(m => m.AddOrUpdate(It.IsAny<ReportStudyRole[]>()))
                .Callback((ReportStudyRole[] passedInReportStudyRoles) =>
                {
                    addedReportStudyRoles.AddRange(passedInReportStudyRoles);
                });

            _reportStudyRoleDbSet
                .Setup(m => m.RemoveRange(It.IsAny<IEnumerable<ReportStudyRole>>()))
                .Callback((IEnumerable<ReportStudyRole> passedInReportStudyRoles) =>
                {
                    removedReportStudyRoles.AddRange(passedInReportStudyRoles);
                });

            var repository = new DataCopyRepository(_dbContext.Object);

            await repository.UpdateStudyPortalConfigData(DevApiData);

            _dbContext
                .Verify(db => db.BeginTransaction(), Times.Once);

            _mockTransaction
                .Verify(db => db.Commit(), Times.Once);

            _mockTransaction
                .Verify(db => db.Rollback(), Times.Never);

            _emailContentStudyRoleDbSet.Verify(ds => ds.AddOrUpdate(It.IsAny<EmailContentStudyRole[]>()), Times.Once);
            _emailContentStudyRoleDbSet.Verify(ds => ds.RemoveRange(It.IsAny<IEnumerable<EmailContentStudyRole>>()), Times.Never);

            _studyRoleUpdateDbSet.Verify(ds => ds.AddOrUpdate(It.IsAny<StudyRoleUpdate[]>()), Times.Once);
            _studyRoleUpdateDbSet.Verify(ds => ds.RemoveRange(It.IsAny<IEnumerable<StudyRoleUpdate>>()), Times.Never);

            _studyRoleWidgetDbSet.Verify(ds => ds.AddOrUpdate(It.IsAny<StudyRoleWidget[]>()), Times.Once);
            _studyRoleWidgetDbSet.Verify(ds => ds.RemoveRange(It.IsAny<IEnumerable<StudyRoleWidget>>()), Times.Never);

            _systemActionStudyRoleDbSet.Verify(ds => ds.AddOrUpdate(It.IsAny<SystemActionStudyRole[]>()), Times.Once);
            _systemActionStudyRoleDbSet.Verify(ds => ds.RemoveRange(It.IsAny<IEnumerable<SystemActionStudyRole>>()), Times.Never);

            _reportStudyRoleDbSet.Verify(ds => ds.AddOrUpdate(It.IsAny<ReportStudyRole[]>()), Times.Once);
            _reportStudyRoleDbSet.Verify(ds => ds.RemoveRange(It.IsAny<IEnumerable<ReportStudyRole>>()), Times.Once);

            _analyticsReferenceStudyRoleDbSet.Verify(ds => ds.AddOrUpdate(It.IsAny<AnalyticsReferenceStudyRole[]>()), Times.Once);
            _analyticsReferenceStudyRoleDbSet.Verify(ds => ds.RemoveRange(It.IsAny<AnalyticsReferenceStudyRole[]>()), Times.Never);

            Assert.AreEqual(_reportStudyRolesToBeRemoved.Count, removedReportStudyRoles.Count);

            foreach (var itemToHaveBeenRemoved in _reportStudyRolesToBeRemoved)
            {
                Assert.IsTrue(removedReportStudyRoles.Contains(itemToHaveBeenRemoved));
            }

            foreach (var itemThatWasAdded in addedReportStudyRoles)
            {
                Assert.IsTrue(_reportStudyRoles.Contains(itemThatWasAdded) || _reportStudyRolesToBeAdded.Contains(itemThatWasAdded));
            }
        }
    }
}
