using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class SoftwareReleaseConfiguration
    {
        public SoftwareReleaseConfiguration(EntityTypeConfiguration<SoftwareRelease> entityConfiguration)
        {
        }

        public static void Seed(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'Software Release Seeding'");
            context.SoftwareReleases.AddOrUpdate(a => a.Id,
                new SoftwareRelease
                {
                    Id = Guid.Parse("6559B308-785B-4A87-88D1-0B0F970EBA1F"),
                    SoftwareVersionId = Guid.Parse("84D3C71C-AF5C-4281-AAD6-74C630D0F523"),
                    Name = "Initial Software Release",
                    IsActive = true,
                    StudyWide = true,
                    ConfigurationId = Config.Defaults.ConfigurationVersions.InitialVersion.Id,
                    ConfigurationVersion = Config.Defaults.ConfigurationVersions.InitialVersion.ConfigurationVersionNumber
                }
            );
            context.SaveChanges();
        }
    }
}