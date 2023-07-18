using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class SoftwareVersionConfiguration
    {
        public SoftwareVersionConfiguration(EntityTypeConfiguration<SoftwareVersion> entityConfiguration)
        {
        }

        public static void Seed(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'SoftwareVersion Seeding'");

            var initialSeedId = Guid.Parse("84D3C71C-AF5C-4281-AAD6-74C630D0F523");
            var hasInitial = context.SoftwareVersions.Any(s => s.Id == initialSeedId);
            var versionId = hasInitial ? Guid.NewGuid() : initialSeedId;

            context.SoftwareVersions.AddOrUpdate(s => s.VersionNumber,
                new SoftwareVersion
                {
                    Id = versionId,
                    VersionNumber = "6.0.0.1609",
                    PackagePath = "https://ypstorageprd.azureedge.net/release/6.0.0/Android/yprime.dct.Droid_6.0.0.1609.zip",
                    PlatformTypeId = PlatformType.Android.Id
                }
            );

            context.SaveChanges();
        }
    }
}