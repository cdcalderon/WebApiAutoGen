using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class ExportStatusConfiguration
    {
        public ExportStatusConfiguration(EntityTypeConfiguration<ExportStatus> entityConfiguration)
        {
        }

        public static void Seed(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'ExportStatus Seeding'");
            context.ExportStatuses.AddOrUpdate(e => e.Id,
                new ExportStatus
                {
                    Id = 1,
                    Name = "In Queue"
                },
                new ExportStatus
                {
                    Id = 2,
                    Name = "Processing"
                },
                new ExportStatus
                {
                    Id = 3,
                    Name = "Complete"
                },
                new ExportStatus
                {
                    Id = 4,
                    Name = "Removed"
                });
            context.SaveChanges();
        }
    }
}