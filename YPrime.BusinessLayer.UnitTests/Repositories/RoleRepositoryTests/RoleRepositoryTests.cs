using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Enums;
using YPrime.BusinessLayer.Repositories;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Repositories.RoleRepositoryTests
{
    [TestClass]
    public class RoleRepositoryTests : LegacySiteTestSetup
    {
        protected Mock<MockableDbSetWithExtensions<SystemActionStudyRole>> _systemActionStudyRoleDbSet;
        protected Mock<MockableDbSetWithExtensions<SystemAction>> _systemActionDbSet;
        protected Mock<MockableDbSetWithExtensions<EmailContentStudyRole>> _emailContentStudyRoleDbSet;
        protected Mock<MockableDbSetWithExtensions<ReportStudyRole>> _reportStudyRoleDbSet;
        protected Mock<MockableDbSetWithExtensions<StudyRoleUpdate>> _studyRoleUpdateDbSet;
        protected Mock<MockableDbSetWithExtensions<StudyRoleWidget>> _studyRoleWidgetDbSet;

        private List<StudyRoleWidget> _studyRoleWidgets;
        private List<StudyRoleModel> _studyRoleModels;
        private List<SystemAction> _systemActions;
        private List<SystemActionStudyRole> _systemActionStudyRoles;
        private List<EmailContentStudyRole> _emailContentStudyRoles;
        private List<ReportStudyRole> _reportStudyRoles;
        private List<StudyRoleUpdate> _studyRoleUpdates;

        private readonly Guid _studyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789123");
        private readonly Guid _studyRoleId2 = Guid.NewGuid();
        private readonly Guid _studyRoleId3 = Guid.NewGuid();
        private readonly Guid _systemActionId = Guid.NewGuid();

        [TestInitialize]
        public void InitializeRoleTests()
        {
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
                    StudyRoleId = Guid.Parse("72345678-1234-1234-1234-123456789123")
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
                    WidgetPosition = 1
                }
            };

            MockStudyRoleService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(_studyRoleModels);

            _systemActionDbSet = CreateDbSetMock(_systemActions);
            _systemActionStudyRoleDbSet = CreateDbSetMock(_systemActionStudyRoles);
            _emailContentStudyRoleDbSet = CreateDbSetMock(_emailContentStudyRoles);
            _reportStudyRoleDbSet = CreateDbSetMock(_reportStudyRoles);
            _studyRoleUpdateDbSet = CreateDbSetMock(_studyRoleUpdates);
            _studyRoleWidgetDbSet = CreateDbSetMock(_studyRoleWidgets);

            _dbContext.Setup(x => x.SystemActionStudyRoles).Returns(_systemActionStudyRoleDbSet.Object);
            _dbContext.Setup(x => x.EmailContentStudyRoles).Returns(_emailContentStudyRoleDbSet.Object);
            _dbContext.Setup(x => x.ReportStudyRoles).Returns(_reportStudyRoleDbSet.Object);
            _dbContext.Setup(x => x.StudyRoleUpdates).Returns(_studyRoleUpdateDbSet.Object);
            _dbContext.Setup(x => x.SystemActions).Returns(_systemActionDbSet.Object);
            _dbContext.Setup(x => x.StudyRoleWidgets).Returns(_studyRoleWidgetDbSet.Object);
        }

        [TestMethod]
        public async Task GetRolesTest()
        {
            var roleNames = new List<string>(new[] {"AB", "XY"});
            var roleRepository = new RoleRepository(_dbContext.Object, MockStudyRoleService.Object);
            var getRoles = await roleRepository.GetRoles(roleNames);

            Assert.AreEqual("AB", getRoles.Find(x => x.ShortName == "AB").ShortName);
        }

        [TestMethod]
        public async Task GetAllRolesTest()
        {
            var roleRepository = new RoleRepository(_dbContext.Object, MockStudyRoleService.Object);
            var getAllRoles = await roleRepository.GetAllRoles();

            var a = getAllRoles.Select(x => new {x.Id, x.ShortName}).ToList();

            Assert.AreEqual("AB", a.Find(x => x.ShortName == "AB").ShortName);
        }

        [TestMethod]
        public async Task GetRoleTest()
        {
            var roleRepository = new RoleRepository(_dbContext.Object, MockStudyRoleService.Object);
            var getRole = await roleRepository.GetRole("AB");

            Assert.AreEqual("AB", getRole.ShortName);
        }

        [TestMethod]
        public async Task GetUserRolesTest()
        {
            var roleRepository = new RoleRepository(_dbContext.Object, MockStudyRoleService.Object);
            var getUserRoles = await roleRepository.GetUserRoles(mainStudyUserGuid);

            var a = getUserRoles.Select(x => new {x.Id, x.ShortName}).ToList();

            Assert.AreEqual("AB", a.Find(x => x.ShortName == "AB").ShortName);
        }

        [TestMethod]
        public void GetRoleActionsTest()
        {
            var roleRepository = new RoleRepository(_dbContext.Object, MockStudyRoleService.Object);
            var getRoleActions = roleRepository.GetRoleActions(Guid.Parse("72345678-1234-1234-1234-123456789123"), true);

            Assert.AreEqual("SA", getRoleActions.Find(x => x.Name == "SA").Name);
        }       

        [TestMethod]
        public void AddActionToRoleTest()
        {
            var roleId = Guid.Parse("72345678-1234-1234-1234-123456789123");

            var roleRepository = new RoleRepository(_dbContext.Object, MockStudyRoleService.Object);
            roleRepository.AddActionToRole(roleId, _systemActionId);
            var role = new StudyRoleModel
            {
                Id = Guid.NewGuid()
            };

            YPrimeSession.CurrentUser.Roles.Add(role);

            _dbContext.Verify(c => c.SystemActionStudyRoles, Times.Once);
            _dbContext.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Exactly(2));

            var roles = YPrimeSession.CurrentUser.Roles;

            Assert.AreEqual(0, roles[0].SystemActions.Count);

            VerifyLastUpdate();
        }

        [TestMethod]
        public void AddActionToRoleAndCurrentUserTest()
        {
            var roleId = Guid.Parse("72345678-1234-1234-1234-123456789123");

            var roleRepository = new RoleRepository(_dbContext.Object, MockStudyRoleService.Object);
            var role = new StudyRoleModel
            {
                Id = roleId
            };

            YPrimeSession.CurrentUser.Roles.Add(role);

            roleRepository.AddActionToRole(roleId, _systemActionId);

            _dbContext.Verify(c => c.SystemActionStudyRoles, Times.Once);
            _dbContext.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Exactly(2));

            var roles = YPrimeSession.CurrentUser.Roles;

            Assert.AreEqual(1, roles[0].SystemActions.Count);
            Assert.AreEqual(_systemActionId, roles[0].SystemActions.FirstOrDefault()?.Id);

            VerifyLastUpdate();
        }

        [TestMethod]
        public void RemoveActionFromRoleTest()
        {
            var roleId = Guid.Parse("72345678-1234-1234-1234-123456789123");
            var sysActionId = Guid.Parse("77345678-1234-1234-1234-123456789123");
            var roleRepository = new RoleRepository(_dbContext.Object, MockStudyRoleService.Object);

            var role = new StudyRoleModel
            {
                Id = Guid.NewGuid()
            };

            YPrimeSession.CurrentUser.Roles.Add(role);

            roleRepository.RemoveActionFromRole(roleId, sysActionId);

            _dbContext.Verify(c => c.SystemActionStudyRoles, Times.Exactly(2));
            _dbContext.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Exactly(2));

            var roles = YPrimeSession.CurrentUser.Roles;

            Assert.AreEqual(0, roles[0].SystemActions.Count);

            VerifyLastUpdate();
        }

        [TestMethod]
        public void RemoveActionFromRoleAndCurrentUserTest()
        {
            var roleId = Guid.Parse("72345678-1234-1234-1234-123456789123");
            var sysActionId = Guid.Parse("77345678-1234-1234-1234-123456789123");
            var roleRepository = new RoleRepository(_dbContext.Object, MockStudyRoleService.Object);

            var role = new StudyRoleModel
            {
                Id = roleId, 
                SystemActions = new HashSet<SystemActionModel>
                {
                    new SystemActionModel
                    {
                        Id = sysActionId
                    }
                }
            };

            YPrimeSession.CurrentUser.Roles.Add(role);

            roleRepository.RemoveActionFromRole(roleId, sysActionId);

            _dbContext.Verify(c => c.SystemActionStudyRoles, Times.Exactly(2));
            _dbContext.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Exactly(2));

            var roles = YPrimeSession.CurrentUser.Roles;

            Assert.AreEqual(0, roles[0].SystemActions.Count);

            VerifyLastUpdate();
        }

        [TestMethod]
        public void AddReportToRoleTest()
        {
            var roleId = Guid.Parse("72345678-1234-1234-1234-123456789123");
            var reportId = ReportType.SiteDetailsReport.Id;
            var roleRepository = new RoleRepository(_dbContext.Object, MockStudyRoleService.Object);
            roleRepository.AddReportToRole(roleId, reportId);

            _dbContext.Verify(c => c.ReportStudyRoles, Times.Once);
            _dbContext.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Exactly(2));

            VerifyLastUpdate();
        }

        [TestMethod]
        public void RemoveReportFromRoleTest()
        {
            var roleId = Guid.Parse("72345678-1234-1234-1234-123456789123");
            var reportId = ReportType.SiteDetailsReport.Id;
            var roleRepository = new RoleRepository(_dbContext.Object, MockStudyRoleService.Object);
            roleRepository.RemoveReportFromRole(roleId, reportId);

            _dbContext.Verify(c => c.ReportStudyRoles, Times.Exactly(2));
            _dbContext.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Exactly(2));
            VerifyLastUpdate();
        }

        [TestMethod]
        public void AddSubscriptionToRoleTest()
        {
            var roleId = Guid.Parse("72345678-1234-1234-1234-123456789123");
            var emailContentId = Guid.Parse("77345678-1234-1234-1234-123456789123");
            var roleRepository = new RoleRepository(_dbContext.Object, MockStudyRoleService.Object);
            roleRepository.AddSubscriptionToRole(roleId, emailContentId);

            _dbContext.Verify(c => c.EmailContentStudyRoles, Times.Once);
            _dbContext.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Exactly(2));
            VerifyLastUpdate();
        }

        [TestMethod]
        public void RemoveSubscriptionFromRoleTest()
        {
            var roleId = Guid.Parse("72345678-1234-1234-1234-123456789123");
            var emailContentId = Guid.Parse("77345678-1234-1234-1234-123456789123");
            var roleRepository = new RoleRepository(_dbContext.Object, MockStudyRoleService.Object);
            roleRepository.RemoveSubscriptionFromRole(roleId, emailContentId);

            _dbContext.Verify(c => c.EmailContentStudyRoles, Times.Exactly(2));
            _dbContext.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Exactly(2));
            VerifyLastUpdate();
        }

        private void VerifyLastUpdate()
        {
            _dbContext.Verify(c => c.StudyRoleUpdates, Times.Once);
        }
    }
}