using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class WidgetCountConfiguration
    {
        public WidgetCountConfiguration(EntityTypeConfiguration<WidgetCount> entityConfiguration)
        {
            entityConfiguration.Property(e => e.Id).IsRequired();
        }
    }
}