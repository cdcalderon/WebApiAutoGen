using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.UnitTests;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.Reports.Reports;
using YPrime.eCOA.DTOLibrary;
using static YPrime.Reports.Reports.AnswerAuditReport;

namespace YPrime.UnitTests.YPrime.Reports.Tests
{
    [TestClass]
    public class AnswerAuditReportGetGridDataTests : BaseReportTest
    {
        private FakeDbSet<StudyUserRole> _dataSet;
        private Guid _patientId;
        private Guid _siteId;
        private string _siteNumber;
        private Guid _studyRoleId;
        private StudyUserRole _studyUserRole;
        private Guid _userId;
        private List<dynamic> _userList;
        private AnswerAuditReport answerAuditReportReport;
        private Mock<ITranslationService> MockTranslationService;
        private Mock<IQuestionnaireService> MockQuestionnaireService;
        private Mock<IStudySettingService> MockStudySettingService;
        private List<AnswerAuditDto> _data;

        [TestInitialize]
        public void TestInitialize()
        {
            MockTranslationService = new Mock<ITranslationService>();
            MockQuestionnaireService = new Mock<IQuestionnaireService>();
            MockStudySettingService = new Mock<IStudySettingService>();

            MockStudySettingService.Setup(x => x.GetStringValue(It.IsAny<string>(), It.IsAny<Guid?>())).ReturnsAsync("Protocol");
            MockQuestionnaireService.Setup(x => x.GetAllInflatedQuestionnaires(null, It.IsAny<Guid?>())).ReturnsAsync(new List<QuestionnaireModel>());

            answerAuditReportReport = new AnswerAuditReport(Context.Object, MockTranslationService.Object, MockQuestionnaireService.Object, MockStudySettingService.Object);

            _patientId = Guid.NewGuid();
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

            _data = new List<AnswerAuditDto>()
            {
                new AnswerAuditDto
                {
                    Protocol = "test protocol",
                    SiteNumber = "001",
                    SubjectNumber = "999",
                    DiaryDate = DateTime.Now.ToString(),
                    Questionnaire = "test questionnaire",
                    Question = "test question",
                    OldValue = "old value",
                    NewValue = "new value",
                    ChangeReasonType = "Change Reason Type",
                    ChangedBy = "Changed By",
                    ChangedDate = "Changed Date",
                    CorrectionReason = "Correction Reason",
                    DCFNumber =  "DCF Number",
                    AssetTag = "Asset Tag"
                }
            };

            SetupContext(_data);

            var TestPatient = new Patient
            {
                Id = _patientId,
                PatientNumber = "S-10001-001",
                SiteId = _siteId
            };

            var patientDbSet = new FakeDbSet<Patient>(new List<Patient> { TestPatient });
            Context.Setup(c => c.Patients).Returns(patientDbSet.Object);
        }

        [TestMethod]
        public async Task WhenCalled_WillReturnReportDataDto()
        {
            var param = new Dictionary<string, object>();
            param.Add("SITES", "test site");
            param.Add("SUBJ", _patientId.ToString());

            var result = await answerAuditReportReport.GetGridData(param, _userId);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            Assert.AreEqual(result[0]["Protocol"], _data[0].Protocol);
            Assert.AreEqual(result[0]["SiteNumber"], _data[0].SiteNumber);
            Assert.AreEqual(result[0]["DiaryDate"], _data[0].DiaryDate);
            Assert.AreEqual(result[0]["Questionnaire"], _data[0].Questionnaire);
            Assert.AreEqual(result[0]["Question"], _data[0].Question);
            Assert.AreEqual(result[0]["OldValue"], _data[0].OldValue);
            Assert.AreEqual(result[0]["NewValue"], _data[0].NewValue);
        }


        [TestMethod]
        public async Task WhenCalledWillReturnHeaderMappings()
        {
            var param = new Dictionary<string, object>();
            param.Add("SITES", "test site");
            param.Add("SUBJ", _patientId.ToString());

            var reportData = await answerAuditReportReport.GetGridData(param, _userId);
            var reportHeaders = answerAuditReportReport.GetColumnHeadings();

            foreach (var header in reportHeaders)
            {
                Assert.IsNotNull(reportData[0][header.Key]);
            }

            Assert.IsTrue(reportHeaders.Keys.Count.Equals(reportData[0].Row.Count));
        }

        private void SetupContext(List<AnswerAuditDto> items)
        {
            Context.Setup(ctx => ctx.ExecuteSqlToList<AnswerAuditDto>(It.IsAny<string>(), It.IsAny<object[]>()))
                .Returns(items);
        }
    }
}