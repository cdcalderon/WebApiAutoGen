using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class PatientConfiguration
    {
        public PatientConfiguration(EntityTypeConfiguration<Patient> entityConfiguration)
        {
            entityConfiguration.Property(e => e.PatientNumber)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute()));

            entityConfiguration.HasMany(e => e.PatientVisits)
                .WithRequired(e => e.Patient)
                .HasForeignKey(e => e.PatientId);
        }
    }
}