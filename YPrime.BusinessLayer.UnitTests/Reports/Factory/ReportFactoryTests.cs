using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using YPrime.BusinessLayer.Enums;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Reports;
using YPrime.BusinessLayer.Reports.Factory;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Reports.Reports;

namespace YPrime.BusinessLayer.UnitTests.Reports.Factory
{
    [TestClass]
    public class ReportFactoryTests
    {
        [TestMethod]
        public void CreateReportTest()
        {
            var mockContext = new Mock<IStudyDbContext>();
            var mockAuthenticationUserRepository = new Mock<IAuthenticationUserRepository>();
            var mockUserRepository = new Mock<IUserRepository>();
            var mockRoleRepository = new Mock<IRoleRepository>();
            var mockStudyRoleService = new Mock<IStudyRoleService>();
            var mockApproverGroupService = new Mock<IApproverGroupService>();
            var mockTranslationService = new Mock<ITranslationService>();
            var mockCountryService = new Mock<ICountryService>();
            var mockCorrectionTypeService = new Mock<ICorrectionTypeService>();
            var mockQuestionnaireService = new Mock<IQuestionnaireService>();
            var mockStudySettingService = new Mock<IStudySettingService>();
            var mockVisitService = new Mock<IVisitService>();
            var mockLanguageService = new Mock<ILanguageService>();
            var mockPatientStatusService = new Mock<IPatientStatusService>();
            var mockSubjectInformationService = new Mock<ISubjectInformationService>();
            var mockDiaryEntryRepository = new Mock<IDiaryEntryRepository>();

            mockTranslationService
                .Setup(s => s.GetByKey("lblPatient", It.IsAny<Guid?>(), It.IsAny<Guid?>()))
                .ReturnsAsync("Patient");

            var factory = new ReportFactory(
                mockContext.Object,
                mockAuthenticationUserRepository.Object,
                mockUserRepository.Object,
                mockRoleRepository.Object,
                mockStudyRoleService.Object,
                mockApproverGroupService.Object,
                mockTranslationService.Object,
                mockCountryService.Object,
                mockCorrectionTypeService.Object,
                mockPatientStatusService.Object,
                mockQuestionnaireService.Object,
                mockStudySettingService.Object,
                mockVisitService.Object,
                mockLanguageService.Object,
                mockSubjectInformationService.Object,
                mockDiaryEntryRepository.Object);

            IReport result;

            result = factory.CreateReport(ReportType.AnswerAuditRecReportFiltered);
            Assert.IsTrue(result is AnswerAuditReport);

            result = factory.CreateReport(ReportType.AverageDiaryDuration);
            Assert.IsTrue(result is AverageDiaryDurationReport);

            result = factory.CreateReport(ReportType.DailyDiaryComplianceReport);
            Assert.IsTrue(result is eCOASubjectComplianceReport);

            result = factory.CreateReport(ReportType.DCFStatusReport);
            Assert.IsTrue(result is DCFStatusReport);

            result = factory.CreateReport(ReportType.PatientAuditRecReportFiltered);
            Assert.IsTrue(result is SubjectInformationAuditReport);

            result = factory.CreateReport(ReportType.PatientDuplicateReportByUser);
            Assert.IsTrue(result is DuplicateSubjectReport);

            result = factory.CreateReport(ReportType.PatientVisitAuditRecReportFiltered);
            Assert.IsTrue(result is SubjectVisitAuditReport);

            result = factory.CreateReport(ReportType.SiteDetailsReport);
            Assert.IsTrue(result is SiteDetailsReport);

            result = factory.CreateReport(ReportType.StudyUserReportByUser);
            Assert.IsTrue(result is StudyUserReport);

            result = factory.CreateReport(ReportType.SubjectComplianceReportByUser);
            Assert.IsTrue(result is eCOAComplianceReport);

            result = factory.CreateReport(ReportType.TotalEnrollmentReport);
            Assert.IsTrue(result is TotalEnrollmentReport);

            result = factory.CreateReport(ReportType.VisitComplianceReport);
            Assert.IsTrue(result is VisitComplianceReport);
        }
    }
}
