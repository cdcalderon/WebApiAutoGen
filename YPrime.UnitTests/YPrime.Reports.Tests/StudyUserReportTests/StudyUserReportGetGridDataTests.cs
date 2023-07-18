using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.UnitTests;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.Reports.Reports;

namespace YPrime.UnitTests.YPrime.Reports.Tests
{
    [TestClass]
    public class StudyUserReportGetGridDataTests : BaseReportTest
    {
        private FakeDbSet<StudyUserRole> _dataSet;
        private Guid _siteId;
        private string _siteNumber;
        private Guid _studyRoleId;
        private StudyUserRole _studyUserRole;
        private Guid _userId;
        private List<dynamic> _userList;
        private StudyUserReport studyUserReport;
        private Mock<IAuthenticationUserRepository> MockUserRepository;
        private Mock<IStudyRoleService> MockStudyRoleService;

        [TestInitialize]
        public void TestInitialize()
        {
            MockUserRepository = new Mock<IAuthenticationUserRepository>();
            MockStudyRoleService = new Mock<IStudyRoleService>();
            MockStudyRoleService.Setup(s => s.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<StudyRoleModel>
            {
                new StudyRoleModel
                {
                    Id = _studyRoleId,
                    IsBlinded = true,
                    ShortName ="YP"
                }
            });
            studyUserReport = new StudyUserReport(Context.Object, MockUserRepository.Object, MockStudyRoleService.Object);

            _userId = new Guid();
            _studyRoleId = new Guid();
            _siteId = new Guid();
            _siteNumber = "10001";
            _userList = new List<dynamic>();

            dynamic user = new ExpandoObject();
            user.FirstName = "test";
            user.LastName = "user";
            user.StudyUser = new ExpandoObject();
            user.StudyUser.Id = _userId;
            user.StudyUser.Email = "test@yprime.com";
            user.StudyUser.UserName = "test@yprime.com";

            _userList.Add(user);

            var userIdList = new List<Guid>();
            userIdList.Add(_userId);

            MockUserRepository.Setup(x => x.GetUsersAsync(userIdList)).Returns(Task.FromResult(_userList.AsEnumerable()));

            var data = new List<StudyUserRole>();
            var site = new Site
            {
                Id = _siteId,
                SiteNumber = _siteNumber
            };

            _studyUserRole = new StudyUserRole
            {
                Id = _studyRoleId,
                StudyUserId = _userId,
                StudyRoleId = new Guid(),
                SiteId = _siteId,
                //StudyRole = _userRole,
                Site = site
            };

            data.Add(_studyUserRole);

            SetupContext(data);
        }

        [TestMethod]
        public async Task WhenCalled_WillReturnReportDataDto()
        {
            var result = await studyUserReport.GetGridData(new Dictionary<string, object>(), _userId);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            Assert.AreEqual(result[0]["SiteNumber"], _siteNumber);
            Assert.AreEqual(result[0]["UserName"], _userList[0].StudyUser.UserName);
            Assert.AreEqual(result[0]["FirstName"], _userList[0].FirstName);
            Assert.AreEqual(result[0]["LastName"], _userList[0].LastName);
        }

        [TestMethod]
        public async Task WhenCalledWithInvalidUserId_WillReturnEmptyResult()
        {
            var userID = Guid.NewGuid();
            var result = await studyUserReport.GetGridData(new Dictionary<string, object>(), userID);

            Assert.IsTrue(result.Count.Equals(0));
        }

        [TestMethod]
        public async Task WhenCalledWillReturnHeaderMappings()
        {
            var reportData = await studyUserReport.GetGridData(new Dictionary<string, object>(), _userId);
            var reportHeaders = studyUserReport.GetColumnHeadings();

            foreach (var header in reportHeaders)
            {
                Assert.IsNotNull(reportData[0][header.Key]);
            }

            Assert.IsTrue(reportHeaders.Keys.Count.Equals(reportData[0].Row.Count));
        }

        private void SetupContext(List<StudyUserRole> items)
        {
            _dataSet = new FakeDbSet<StudyUserRole>(items);
            Context.Setup(ctx => ctx.StudyUserRoles)
                .Returns(_dataSet.Object);
        }
    }
}