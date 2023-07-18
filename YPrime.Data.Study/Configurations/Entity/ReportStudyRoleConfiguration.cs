using System;
using System.Data.Entity.Migrations;
using YPrime.BusinessLayer.Enums;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public static class ReportStudyRoleConfiguration
    {
        public static void Seed(IStudyDbContext context)
        {
            context.ReportStudyRoles.AddOrUpdate(x => new { x.ReportId, x.StudyRoleId },
                new ReportStudyRole() { ReportId = new Guid("805DF81A-8CE2-4E7A-887D-399DCD481CC5"), ReportName = "Average Diary Duration (Unblinded)", StudyRoleId = Config.Defaults.StudyRoles.ClinicalResearchAssociate.Id },
                new ReportStudyRole() { ReportId = new Guid("805DF81A-8CE2-4E7A-887D-399DCD481CC5"), ReportName = "Average Diary Duration (Unblinded)", StudyRoleId = Config.Defaults.StudyRoles.Investigator.Id },
                new ReportStudyRole() { ReportId = new Guid("805DF81A-8CE2-4E7A-887D-399DCD481CC5"), ReportName = "Average Diary Duration (Unblinded)", StudyRoleId = Config.Defaults.StudyRoles.StudyCoordinator.Id },
                new ReportStudyRole() { ReportId = new Guid("805DF81A-8CE2-4E7A-887D-399DCD481CC5"), ReportName = "Average Diary Duration (Unblinded)", StudyRoleId = Config.Defaults.StudyRoles.SubInvestigator.Id },
                new ReportStudyRole() { ReportId = new Guid("805DF81A-8CE2-4E7A-887D-399DCD481CC5"), ReportName = "Average Diary Duration (Unblinded)", StudyRoleId = Config.Defaults.StudyRoles.Sponsor.Id },
                new ReportStudyRole() { ReportId = new Guid("805DF81A-8CE2-4E7A-887D-399DCD481CC5"), ReportName = "Average Diary Duration (Unblinded)", StudyRoleId = Config.Defaults.StudyRoles.YP.Id },
                new ReportStudyRole() { ReportId = new Guid("E9616F8E-58E2-4853-BC14-57BDB4742CE6"), ReportName = "Enrollment", StudyRoleId = Config.Defaults.StudyRoles.ClinicalResearchAssociate.Id },
                new ReportStudyRole() { ReportId = new Guid("E9616F8E-58E2-4853-BC14-57BDB4742CE6"), ReportName = "Enrollment", StudyRoleId = Config.Defaults.StudyRoles.Investigator.Id },
                new ReportStudyRole() { ReportId = new Guid("E9616F8E-58E2-4853-BC14-57BDB4742CE6"), ReportName = "Enrollment", StudyRoleId = Config.Defaults.StudyRoles.StudyCoordinator.Id },
                new ReportStudyRole() { ReportId = new Guid("E9616F8E-58E2-4853-BC14-57BDB4742CE6"), ReportName = "Enrollment", StudyRoleId = Config.Defaults.StudyRoles.SubInvestigator.Id },
                new ReportStudyRole() { ReportId = new Guid("E9616F8E-58E2-4853-BC14-57BDB4742CE6"), ReportName = "Enrollment", StudyRoleId = Config.Defaults.StudyRoles.Sponsor.Id },
                new ReportStudyRole() { ReportId = new Guid("E9616F8E-58E2-4853-BC14-57BDB4742CE6"), ReportName = "Enrollment", StudyRoleId = Config.Defaults.StudyRoles.YP.Id },
                new ReportStudyRole() { ReportId = new Guid("E4E4A581-85DC-4789-A321-6D53BE7C734A"), ReportName = "lblPatientScreening", StudyRoleId = Config.Defaults.StudyRoles.ClinicalResearchAssociate.Id },
                new ReportStudyRole() { ReportId = new Guid("58BEDFAE-1DEB-42D2-B10E-78516C36FE6F"), ReportName = "lblTotalEnrollmentUnblinded", StudyRoleId = Config.Defaults.StudyRoles.ClinicalResearchAssociate.Id },
                new ReportStudyRole() { ReportId = new Guid("58BEDFAE-1DEB-42D2-B10E-78516C36FE6F"), ReportName = "lblTotalEnrollmentUnblinded", StudyRoleId = Config.Defaults.StudyRoles.Investigator.Id },
                new ReportStudyRole() { ReportId = new Guid("58BEDFAE-1DEB-42D2-B10E-78516C36FE6F"), ReportName = "lblTotalEnrollmentUnblinded", StudyRoleId = Config.Defaults.StudyRoles.StudyCoordinator.Id },
                new ReportStudyRole() { ReportId = new Guid("58BEDFAE-1DEB-42D2-B10E-78516C36FE6F"), ReportName = "lblTotalEnrollmentUnblinded", StudyRoleId = Config.Defaults.StudyRoles.SubInvestigator.Id },
                new ReportStudyRole() { ReportId = new Guid("58BEDFAE-1DEB-42D2-B10E-78516C36FE6F"), ReportName = "lblTotalEnrollmentUnblinded", StudyRoleId = Config.Defaults.StudyRoles.Sponsor.Id },
                new ReportStudyRole() { ReportId = new Guid("4B04083A-39CD-4FA6-A8D4-89F173A0C878"), ReportName = "lblEnrollmentSummaryBlinded", StudyRoleId = Config.Defaults.StudyRoles.ClinicalResearchAssociate.Id },
                new ReportStudyRole() { ReportId = new Guid("4B04083A-39CD-4FA6-A8D4-89F173A0C878"), ReportName = "lblEnrollmentSummaryBlinded", StudyRoleId = Config.Defaults.StudyRoles.YP.Id },
                new ReportStudyRole() { ReportId = new Guid("15903EFC-B8B8-41CF-A20E-A335565AC0F5"), ReportName = "lblSiteDetailsBlinded", StudyRoleId = Config.Defaults.StudyRoles.ClinicalResearchAssociate.Id },
                new ReportStudyRole() { ReportId = new Guid("15903EFC-B8B8-41CF-A20E-A335565AC0F5"), ReportName = "lblSiteDetailsBlinded", StudyRoleId = Config.Defaults.StudyRoles.Investigator.Id },
                new ReportStudyRole() { ReportId = new Guid("15903EFC-B8B8-41CF-A20E-A335565AC0F5"), ReportName = "lblSiteDetailsBlinded", StudyRoleId = Config.Defaults.StudyRoles.StudyCoordinator.Id },
                new ReportStudyRole() { ReportId = new Guid("15903EFC-B8B8-41CF-A20E-A335565AC0F5"), ReportName = "lblSiteDetailsBlinded", StudyRoleId = Config.Defaults.StudyRoles.SubInvestigator.Id },
                new ReportStudyRole() { ReportId = new Guid("15903EFC-B8B8-41CF-A20E-A335565AC0F5"), ReportName = "lblSiteDetailsBlinded", StudyRoleId = Config.Defaults.StudyRoles.Sponsor.Id },
                new ReportStudyRole() { ReportId = new Guid("86AA839E-D43C-4E1D-9AD9-B7FF75810B4E"), ReportName = "lblDuplicateSubject", StudyRoleId = Config.Defaults.StudyRoles.ClinicalResearchAssociate.Id },
                new ReportStudyRole() { ReportId = new Guid("86AA839E-D43C-4E1D-9AD9-B7FF75810B4E"), ReportName = "lblDuplicateSubject", StudyRoleId = Config.Defaults.StudyRoles.Investigator.Id },
                new ReportStudyRole() { ReportId = new Guid("86AA839E-D43C-4E1D-9AD9-B7FF75810B4E"), ReportName = "lblDuplicateSubject", StudyRoleId = Config.Defaults.StudyRoles.StudyCoordinator.Id },
                new ReportStudyRole() { ReportId = new Guid("86AA839E-D43C-4E1D-9AD9-B7FF75810B4E"), ReportName = "lblDuplicateSubject", StudyRoleId = Config.Defaults.StudyRoles.SubInvestigator.Id },
                new ReportStudyRole() { ReportId = new Guid("86AA839E-D43C-4E1D-9AD9-B7FF75810B4E"), ReportName = "lblDuplicateSubject", StudyRoleId = Config.Defaults.StudyRoles.Sponsor.Id },
                new ReportStudyRole() { ReportId = new Guid("86AA839E-D43C-4E1D-9AD9-B7FF75810B4E"), ReportName = "lblDuplicateSubject", StudyRoleId = Config.Defaults.StudyRoles.YP.Id },
                new ReportStudyRole() { ReportId = new Guid("2789616C-8DE3-4300-ADE2-D783AD398E5A"), ReportName = "Randomization Ineligibility Report (Unblinded)", StudyRoleId = Config.Defaults.StudyRoles.YP.Id },
                new ReportStudyRole() { ReportId = new Guid("6BF147B0-C292-4C74-AF27-F180A2095A34"), ReportName = "lblStudyUserUnblinded", StudyRoleId = Config.Defaults.StudyRoles.ClinicalResearchAssociate.Id },
                new ReportStudyRole() { ReportId = new Guid("6BF147B0-C292-4C74-AF27-F180A2095A34"), ReportName = "lblStudyUserUnblinded", StudyRoleId = Config.Defaults.StudyRoles.Investigator.Id },
                new ReportStudyRole() { ReportId = new Guid("6BF147B0-C292-4C74-AF27-F180A2095A34"), ReportName = "lblStudyUserUnblinded", StudyRoleId = Config.Defaults.StudyRoles.StudyCoordinator.Id },
                new ReportStudyRole() { ReportId = new Guid("6BF147B0-C292-4C74-AF27-F180A2095A34"), ReportName = "Study User", StudyRoleId = Config.Defaults.StudyRoles.SubInvestigator.Id },
                new ReportStudyRole() { ReportId = new Guid("6BF147B0-C292-4C74-AF27-F180A2095A34"), ReportName = "lblStudyUserUnblinded", StudyRoleId = Config.Defaults.StudyRoles.Sponsor.Id }
                );
            context.SaveChanges(null);
        }
    }
}