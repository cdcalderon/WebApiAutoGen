using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class CorrectionStatusConfiguration
    {
        public CorrectionStatusConfiguration(EntityTypeConfiguration<CorrectionStatus> entityConfiguration)
        {
            entityConfiguration.Property(e => e.Id).IsRequired();
        }

        public static void Seed(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'Correction Status Seeding'");

            context.CorrectionStatuses.AddOrUpdate(dt => dt.Id,
                new CorrectionStatus
                {
                    Id = Guid.Parse("5BDF72E8-D72C-45FA-ABE2-166CBC3520C7"), TranslationKey = "Pending",
                    Resolved = false
                },
                new CorrectionStatus
                {
                    Id = Guid.Parse("14FC9304-7684-4BB1-8F1D-3B21302BE582"), TranslationKey = "Completed",
                    Resolved = true
                },
                new CorrectionStatus
                {
                    Id = Guid.Parse("899D9619-0FD3-4525-ADED-62EEF2A84CF9"), TranslationKey = "InProgress",
                    Resolved = false
                },
                new CorrectionStatus
                {
                    Id = Guid.Parse("BB727B01-9FBF-4ACC-9E4F-F091ECFA44A6"), TranslationKey = "Rejected",
                    Resolved = true
                },
                new CorrectionStatus
                {
                    Id = Guid.Parse("6463E9BD-B152-4449-ADB8-4172D5E1885E"),
                    TranslationKey = "NeedsMoreInformationCorrection", Resolved = true, NeedsMoreInformation = true
                }
            );

            context.SaveChanges();
        }
    }
}