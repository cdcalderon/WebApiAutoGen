using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class SecurityQuestionConfiguration
    {
        public SecurityQuestionConfiguration(EntityTypeConfiguration<SecurityQuestion> entityConfiguration)
        {
            entityConfiguration.Property(e => e.Id).IsRequired();
            entityConfiguration.Property(e => e.TranslationKey).IsUnicode(true);
        }

        public static void Seed(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'Security Question Seeding'");

            context.SecurityQuestions.AddOrUpdate(dt => dt.Id,
                new SecurityQuestion
                {
                    Id = Guid.Parse("81a2abd7-c78c-4d2e-bf3a-f015b450f721"), TranslationKey = "keyYearOfBirthQuestion"
                },
                new SecurityQuestion
                {
                    Id = Guid.Parse("a9039727-ce45-40d1-b790-b72faac953dd"), TranslationKey = "keyMemorableYearQuestion"
                },
                new SecurityQuestion
                {
                    Id = Guid.Parse("dd9dad4e-0ac0-4690-8304-2a64928f27a8"),
                    TranslationKey = "keyMemorableNumberQuestion"
                },
                new SecurityQuestion
                {
                    Id = Guid.Parse("8f5a0042-0304-4dcc-975c-097ab9634cbb"),
                    TranslationKey = "keyLast4DigitsOfPhoneQuestion"
                }
            );

            context.SaveChanges();
        }
    }
}