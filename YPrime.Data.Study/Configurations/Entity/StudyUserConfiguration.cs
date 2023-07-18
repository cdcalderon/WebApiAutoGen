using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class StudyUserConfiguration
    {
        public StudyUserConfiguration(EntityTypeConfiguration<StudyUser> entityConfiguration)
        {
        }

        public static void Seed(StudyDbContext context, Guid userId)
        {
            context.Database.ExecuteSqlCommand("PRINT 'StudyUser Seeding'");
            context.StudyUsers.AddOrUpdate(q => q.Id,
                new StudyUser
                {
                    Id = userId,
                    UserName = "ypadmin@yprime.com",
                    Email = "ypadmin@yprime.com"
                });
            context.SaveChanges();
        }
    }
}