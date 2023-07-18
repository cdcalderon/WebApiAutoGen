using System;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using YPrime.Config.Enums;
using YPrime.Data.Study.Constants;

namespace YPrime.BusinessLayer.Enums
{
    public class ReportType : Enumeration<Guid>
    {
        public IEnumerable<SeriesChartType> SupportedChartTypes { get; private set; }
        public bool RestrictToPdfExport { get; private set; }

        public ReportType(
            Guid id,
            string name,
            IEnumerable<SeriesChartType> supportedChartTypes,
            bool restrictToPdfExport) : base(id, name)
        {
            SupportedChartTypes = supportedChartTypes;
            RestrictToPdfExport = restrictToPdfExport;
        }

        public ReportType(
            Guid id,
            string name,
            bool restrictToPdfExport) : base(id, name)
        {
            SupportedChartTypes = new List<SeriesChartType>();
            RestrictToPdfExport = restrictToPdfExport;
        }

        public static readonly ReportType AnswerAuditRecReportFiltered = new ReportType(
            Guid.Parse("EF01F9D9-7433-42A9-AE70-8F166F9DE8D3"),
            TranslationKeyTypes.lblAnswerAuditReport,
            true);

        public static readonly ReportType AverageDiaryDuration = new ReportType(
            Guid.Parse("805DF81A-8CE2-4E7A-887D-399DCD481CC5"),
            TranslationKeyTypes.lblAverageDiaryDurationReport,
            new List<SeriesChartType>
            {
                SeriesChartType.Area,
                SeriesChartType.Bar
            },
            false);

        public static readonly ReportType DailyDiaryComplianceReport = new ReportType(
            Guid.Parse("A73D28F5-697F-46A0-9A2D-D5A1C76D0ABE"),
            TranslationKeyTypes.lbleCOASubjectComplianceReport,
            false);

        public static readonly ReportType DCFStatusReport = new ReportType(
            Guid.Parse("9087C7CB-771C-41EE-9B8C-2213D8B9A02B"),
            TranslationKeyTypes.lblDCFStatusReport,
            false);

        public static readonly ReportType PatientAuditRecReportFiltered = new ReportType(
            Guid.Parse("4338F297-E844-40F9-BCCE-450FBD11523A"),
            TranslationKeyTypes.lblPatientInfoAuditReport,
            true);

        public static readonly ReportType PatientDuplicateReportByUser = new ReportType(
            Guid.Parse("86AA839E-D43C-4E1D-9AD9-B7FF75810B4E"),
            TranslationKeyTypes.lblDuplicateSubject,
            false);

        public static readonly ReportType PatientVisitAuditRecReportFiltered = new ReportType(
            Guid.Parse("7180E81A-49FA-472F-BDF8-E65CC3B494C4"),
            TranslationKeyTypes.lblPatientVisitAuditReport,
            true);

        public static readonly ReportType SiteDetailsReport = new ReportType(
            Guid.Parse("15903efc-b8b8-41cf-a20e-a335565ac0f5"),
            TranslationKeyTypes.lblSiteDetailsBlinded,
            new List<SeriesChartType>
            {
                SeriesChartType.Doughnut
            },
            false);

        public static readonly ReportType StudyUserReportByUser = new ReportType(
            Guid.Parse("6BF147B0-C292-4C74-AF27-F180A2095A34"),
            TranslationKeyTypes.lblStudyUserUnblinded,
            false);

        public static readonly ReportType SubjectComplianceReportByUser = new ReportType(
            Guid.Parse("EBDD4231-632A-4526-BD30-DC8F716F5448"),
            TranslationKeyTypes.lblECOAComplianceUnblinded,
            false);

        public static readonly ReportType TotalEnrollmentReport = new ReportType(
            Guid.Parse("58BEDFAE-1DEB-42D2-B10E-78516C36FE6F"),
            TranslationKeyTypes.lblTotalEnrollmentUnblinded,
            new List<SeriesChartType>
            {
                SeriesChartType.Doughnut,
                SeriesChartType.Bar
            },
            false);

        public static readonly ReportType VisitComplianceReport = new ReportType(
            Guid.Parse("43FA6902-311E-442C-A759-3B431916A7DF"),
            TranslationKeyTypes.lblVisitComplianceReport,
            false);
    }
}