using System;
using System.Data.Entity.Migrations;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class MissedVisitReasonConfiguration
    { 
        public static void Seed(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand($"PRINT '{nameof(MissedVisitReason)} seeding'");

            context.MissedVisitReasons.AddOrUpdate(mvr => mvr.Id,
                new MissedVisitReason
                {
                    Id = Guid.Parse("C652A61D-F7E7-4F54-8069-3706509B546C"),
                    TranslationKey = "mvrkey_TechnicalProblems"
                },
                new MissedVisitReason
                {
                    Id = Guid.Parse("9241731A-7287-49EE-8362-49200B602834"),
                    TranslationKey = "mvrkey_MissedVisit"
                },
                new MissedVisitReason
                {
                    Id = Guid.Parse("2A6EBBE8-6207-46C8-94E3-B64D7B2423F7"),
                    TranslationKey = "mvrkey_RefusaltoParticipate"
                },
                new MissedVisitReason
                {
                    Id = Guid.Parse("1722EE71-60F6-48B3-BFF9-B75AF8842906"),
                    TranslationKey = "mvrkey_StaffError"
                },
                new MissedVisitReason
                {
                    Id = Guid.Parse("6E6F9D0D-DAC0-46A4-A85D-F824275EC5C2"),
                    TranslationKey = "mvrkey_UnableToDo"
                }
            );

            context.SaveChanges();
        }
    }
}
