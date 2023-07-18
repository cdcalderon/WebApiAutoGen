using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Reports;
using YPrime.BusinessLayer.UnitTests;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;

namespace YPrime.UnitTests.YPrime.Reports.Tests.VisitComplianceReportTests
{
    [TestClass]
    public class VisitComplianceReportGetGridDataTests : BaseReportTest
    {
        private FakeDbSet<StudyUserRole> _dataSet;
        private Guid _siteId;
        private string _siteNumber;
        private Guid _studyRoleId;
        private StudyUserRole _studyUserRole;
        private Guid _userId;
        private List<dynamic> _userList;
        private VisitComplianceReport visitComplianceReport;
        private Mock<IQuestionnaireService> MockQuestionnaireService;
        private Mock<IVisitService> MockVisitService;
        private Mock<IPatientStatusService> MockPatientStatusService;
        private Mock<ITranslationService> MockTranslationService;
        private List<dynamic> _data;

        [TestInitialize]
        public void TestInitialize()
        {
            MockQuestionnaireService = new Mock<IQuestionnaireService>();
            MockTranslationService = new Mock<ITranslationService>();
            MockVisitService = new Mock<IVisitService>();
            MockPatientStatusService = new Mock<IPatientStatusService>();

            MockVisitService.Setup(x => x.GetAll(It.IsAny<Guid?>())).ReturnsAsync(new List<VisitModel>());
            MockQuestionnaireService.Setup(x => x.GetAllInflatedQuestionnaires(null, It.IsAny<Guid?>())).ReturnsAsync(new List<QuestionnaireModel>());

            visitComplianceReport = new VisitComplianceReport(Context.Object, MockVisitService.Object, MockTranslationService.Object, MockQuestionnaireService.Object, MockPatientStatusService.Object);

            _userId = new Guid();
            _studyRoleId = new Guid();
            _siteId = Guid.NewGuid();
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

            var studyUserRoleData = new List<StudyUserRole>();
            var site = new Site
            {
                Id = _siteId,
                SiteNumber = _siteNumber,
                Investigator = "YP",
                Name = "Test site"
            };
            _studyUserRole = new StudyUserRole
            {
                Id = _studyRoleId,
                StudyUserId = _userId,
                StudyRoleId = new Guid(),
                SiteId = _siteId,
                Site = site
            };

            studyUserRoleData.Add(_studyUserRole);
            _data = new List<dynamic>();
            dynamic dynamicData = new ExpandoObject() as IDictionary<string,object>;

            dynamicData.Protocol = "test protocol";
            dynamicData.PatientId = Guid.NewGuid();
            dynamicData.SiteId = _siteId;
            dynamicData.SiteNumber = "001";
            dynamicData.PatientNumber = "999";
            dynamicData.Visit = "visit 1";
            dynamicData.VisitDate = DateTime.Now.ToString();
            dynamicData.VisitName = "visit Name";
            dynamicData.VisitCompliance = "visit compliance";
            dynamicData.SiteComplianceRate = "Site Compliance Rate";
            dynamicData.DateOfDeactivation = "deactivation date";
            dynamicData.DateOfReactivation = "reactivation date";

            _data.Add(dynamicData);

            SetupStudyUserRoleData(studyUserRoleData);
            SetupContext(_data);
        }

        [TestMethod]
        public async Task WhenCalled_WillReturnReportDataDto()
        {
            var param = new Dictionary<string, object>();
            param.Add("SITES", "test site");

            var result = await visitComplianceReport.GetGridData(param, _userId);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            Assert.AreEqual(result[0]["Protocol"], _data[0].Protocol);
            Assert.AreEqual(result[0]["SiteNumber"], _data[0].SiteNumber);
            Assert.AreEqual(result[0]["PatientNumber"], _data[0].PatientNumber);
            Assert.AreEqual(result[0]["Visit"], _data[0].Visit);
            Assert.AreEqual(result[0]["VisitDate"], _data[0].VisitDate);
            Assert.AreEqual(result[0]["VisitName"], _data[0].VisitName);
            Assert.AreEqual(result[0]["VisitCompliance"], _data[0].VisitCompliance);
            Assert.AreEqual(result[0]["SiteComplianceRate"], _data[0].SiteComplianceRate);
            Assert.AreEqual(result[0]["DateOfDeactivation"], _data[0].DateOfDeactivation);
            Assert.AreEqual(result[0]["DateOfReactivation"], _data[0].DateOfReactivation);
        }

        [TestMethod]
        public async Task WhenCalledWillReturnHeaderMappings()
        {
            var param = new Dictionary<string, object>();
            param.Add("SITES", "test site");

            var reportData = await visitComplianceReport.GetGridData(param, _userId);
            var reportHeaders = visitComplianceReport.GetColumnHeadings();

            foreach (var header in reportHeaders)
            {
                Assert.IsNotNull(reportData[0].Row[header.Key]);
            }

            Assert.IsTrue(reportHeaders.Keys.Count.Equals(reportData[0].Row.Count));
        }

        private void SetupContext(List<dynamic> items)
        {
            Context.Setup(ctx => ctx.CollectionFromSqlStoredProcedure(It.IsAny<string>(), It.IsAny<Dictionary<string,object>>()))
                .Returns(items);
        }

        private void SetupStudyUserRoleData(List<StudyUserRole> items)
        {
            _dataSet = new FakeDbSet<StudyUserRole>(items);
            Context.Setup(ctx => ctx.StudyUserRoles)
                .Returns(_dataSet.Object);
        }

    }
}
