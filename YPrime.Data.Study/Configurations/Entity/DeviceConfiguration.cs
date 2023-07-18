using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class DeviceConfiguration
    {
        public DeviceConfiguration(EntityTypeConfiguration<Device> entityConfiguration)
        {
            entityConfiguration.HasMany(e => e.SyncLogs)
                .WithRequired(e => e.Device)
                .WillCascadeOnDelete(false);
        }
    }
}