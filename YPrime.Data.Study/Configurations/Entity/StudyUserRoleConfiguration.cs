using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class StudyUserRoleConfiguration
    {
        public StudyUserRoleConfiguration(EntityTypeConfiguration<StudyUserRole> entityConfiguration)
        {
        }

        public static void Seed(StudyDbContext context, Guid userId, Guid roleId, Guid siteId)
        {
            context.Database.ExecuteSqlCommand("PRINT 'StudyUserRole Seeding'");
            context.StudyUserRoles.AddOrUpdate(q => new { q.StudyUserId, q.StudyRoleId, q.SiteId },
                new StudyUserRole
                {
                    StudyUserId = userId,
                    StudyRoleId = roleId,
                    SiteId = siteId
                });
            context.SaveChanges();
        }
    }
}