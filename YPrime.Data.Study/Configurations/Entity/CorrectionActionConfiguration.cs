using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class CorrectionActionConfiguration
    {
        public CorrectionActionConfiguration(EntityTypeConfiguration<CorrectionAction> entityConfiguration)
        {
            entityConfiguration.Property(e => e.Id).IsRequired();
        }

        public static void Seed(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'Correction Action Seeding'");

            context.CorrectionActions.AddOrUpdate(dt => dt.Id,
                new CorrectionAction
                {
                    Id = Guid.Parse("D3C17321-C013-4D9D-8639-E2E9CCFA6ADF"), TranslationKey = "PendingCorrection",
                    DisplayOrder = 1, IconCss = "fa fa-info-circle", StatusCss = "pending", Actionable = false
                },
                new CorrectionAction
                {
                    Id = Guid.Parse("E0B99BCB-0F09-4A65-A06D-7E48F5C41808"), TranslationKey = "ApproveCorrection",
                    DisplayOrder = 2, IconCss = "fa fa-check", StatusCss = "approved", Actionable = true
                },
                new CorrectionAction
                {
                    Id = Guid.Parse("0EF32937-FE9F-4D97-B345-701080F0CFAE"), TranslationKey = "RejectCorrection",
                    DisplayOrder = 3, IconCss = "fa fa-times", StatusCss = "rejected", Actionable = true
                },
                new CorrectionAction
                {
                    Id = Guid.Parse("70DA4CC0-ACE7-45F1-BF8A-DB720131A601"),
                    TranslationKey = "NeedsMoreInformationCorrection", DisplayOrder = 4, IconCss = "fa fa-info-circle",
                    StatusCss = "more-info", Actionable = true
                }
            );

            context.SaveChanges();
        }
    }
}