using System;
using System.Configuration;
using System.Data.Entity.Migrations;
using YPrime.Data.Study.Configurations.Entity;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Properties;

namespace YPrime.Data.Study.Configurations.Context
{
    internal sealed class MainConfiguration : DbMigrationsConfiguration<StudyDbContext>
    {
        private readonly Guid ypUserId = Guid.Parse("9E771F5C-EE6C-421A-A349-15AA2DBE0C6C");
        private readonly Guid initialSiteId = Guid.Parse("6BE2187B-3E94-4608-BE8E-52A66D0CE80F");

        public MainConfiguration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
            CommandTimeout = int.MaxValue;
            SetSqlGenerator("System.Data.SqlClient", new DefaultValueSqlServerMigrationSqlGenerator());
        }

        protected override void Seed(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'Begin Seeding'");

            //SQLScripts
            SqlScriptConfiguration.Seed(context, Settings.Default.SeedSQLScripts);

            //verify audit columns
            context.Database.ExecuteSqlCommand("exec CheckAuditTableColumns");          

            //Data seed
            if (Settings.Default.SeedCoreData)
            {
                SeedCoreData(context);
            }

            if (Settings.Default.SeedPermissions)
            {
                SeedPermissions(context);
            }

            context.Database.ExecuteSqlCommand("PRINT 'End Seeding'");
        }


        private void SeedCoreData(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'Core Data Seeding'");

            SiteConfiguration.Seed(context, initialSiteId, Config.Defaults.Countries.UnitedStates.Id);
            SiteLanguageConfiguration.Seed(context, initialSiteId, Config.Defaults.Languages.English.Id);
            StudyUserConfiguration.Seed(context, ypUserId);
            StudyUserRoleConfiguration.Seed(context, ypUserId, Config.Defaults.StudyRoles.YP.Id, initialSiteId);
            SystemActionConfiguration.Seed(context);
            WidgetConfiguration.Seed(context);            
            InputFieldTypeResultConfiguration.Seed(context);
            SecurityQuestionConfiguration.Seed(context);
            ScreenReportDialogConfiguration.Seed(context);
            ExportStatusConfiguration.Seed(context);
            WidgetLinkConfiguration.Seed(context);
            EmailContentConfiguration.Seed(context);
            CorrectionStatusConfiguration.Seed(context);
            CorrectionActionConfiguration.Seed(context);
            SoftwareVersionConfiguration.Seed(context);
            SoftwareReleaseConfiguration.Seed(context);
            MissedVisitReasonConfiguration.Seed(context);

            context.SaveChanges();
        }

        private void SeedPermissions(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'Permissions Seeding'");

            ReportStudyRoleConfiguration.Seed(context);
            SystemActionStudyRoleConfiguration.Seed(context);
            StudyRoleWidgetConfiguration.Seed(context);

            context.SaveChanges();
        }
    }
}