using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class SyncLogConfiguration
    {
        public SyncLogConfiguration(EntityTypeConfiguration<SyncLog> entityConfiguration)
        {
            entityConfiguration.HasRequired(e => e.Device)
                .WithMany()
                .WillCascadeOnDelete(false);
        }
    }
}