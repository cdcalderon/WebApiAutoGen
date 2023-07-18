using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class DiaryEntryConfiguration
    {
        public DiaryEntryConfiguration(EntityTypeConfiguration<DiaryEntry> entityConfiguration)
        {
            entityConfiguration.Property(e => e.VisitId);

            entityConfiguration.HasMany(e => e.Answers)
                .WithRequired(e => e.DiaryEntry)
                .WillCascadeOnDelete(false);
        }
    }
}