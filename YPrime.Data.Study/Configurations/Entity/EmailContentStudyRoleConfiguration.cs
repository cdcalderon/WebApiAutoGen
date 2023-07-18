using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class EmailContentStudyRoleConfiguration
    {
        public EmailContentStudyRoleConfiguration(EntityTypeConfiguration<EmailContentStudyRole> entityConfiguration)
        {
            entityConfiguration.HasKey(t => new {t.EmailContentId, t.StudyRoleId});
        }
    }
}