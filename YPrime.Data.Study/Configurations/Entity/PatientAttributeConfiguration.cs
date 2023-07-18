using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class PatientAttributeConfiguration
    {
        public PatientAttributeConfiguration(EntityTypeConfiguration<PatientAttribute> entityConfiguration)
        {
            entityConfiguration.Property(e => e.Id).IsRequired();
            entityConfiguration.Property(e => e.PatientId).IsRequired();
            entityConfiguration.Property(e => e.PatientAttributeConfigurationDetailId).IsRequired();
            entityConfiguration.Property(e => e.AttributeValue).IsUnicode(true).IsRequired();
        }
    }
}