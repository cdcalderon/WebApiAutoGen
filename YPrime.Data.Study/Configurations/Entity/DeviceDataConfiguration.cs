using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class DeviceDataConfiguration
    {
        public DeviceDataConfiguration(EntityTypeConfiguration<DeviceData> entityConfiguration)
        {
            entityConfiguration.Property(d => d.Id).IsRequired();
            entityConfiguration.Property(d => d.DeviceId).IsRequired();
            entityConfiguration.Property(d => d.Fob).HasMaxLength(50).IsRequired();
        }
    }
}