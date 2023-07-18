using System;
using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class PatientAttributeConfigurationMapperConfiguration
    {
        public PatientAttributeConfigurationMapperConfiguration(EntityTypeConfiguration<PatientAttributeConfigurationMapper> entityConfiguration)
        {
            entityConfiguration.Property(e => e.Id).IsRequired();
            entityConfiguration.Property(e => e.PatientAttributeConfigurationDetailId).IsRequired();
            entityConfiguration.Property(e => e.CountryId).IsRequired();
            entityConfiguration.Property(e => e.SiteId).IsOptional();
        }

        public static void Seed(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'PatientAttributeConfigurationMapper Seeding'");

            context.PatientAttributeConfigurationMappers.AddOrUpdate(dt => dt.Id,
                new PatientAttributeConfigurationMapper() { Id = Guid.Parse("B7FB2617-E80A-4A55-A188-02C5E164E239"), PatientAttributeConfigurationDetailId = Guid.Parse("5DC71905-2FC8-48D3-82B8-2866606BD8A7"), CountryId = Guid.Parse("50E847B7-C81E-43F6-9E07-30FB675232DC") },
                new PatientAttributeConfigurationMapper() { Id = Guid.Parse("F8A84762-4726-4BFD-B502-1A920F7A47E1"), PatientAttributeConfigurationDetailId = Guid.Parse("53A82D08-3B84-4ED5-A3EC-C092930A0A7F"), CountryId = Guid.Parse("50E847B7-C81E-43F6-9E07-30FB675232DC") },
                new PatientAttributeConfigurationMapper() { Id = Guid.Parse("4F7DB01E-47BB-4812-B5AA-65ED27AC5864"), PatientAttributeConfigurationDetailId = Guid.Parse("A861358A-B911-42B9-8760-E5E0712BCD6D"), CountryId = Guid.Parse("50E847B7-C81E-43F6-9E07-30FB675232DC") },
                new PatientAttributeConfigurationMapper() { Id = Guid.Parse("72791E83-A263-439F-9BD5-6718F258D0E3"), PatientAttributeConfigurationDetailId = Guid.Parse("74C8AE81-9204-4E84-A741-5C72B1EB61C5"), CountryId = Guid.Parse("50E847B7-C81E-43F6-9E07-30FB675232DC") },
                new PatientAttributeConfigurationMapper() { Id = Guid.Parse("F3235813-30E6-4054-A375-B0D770532D6C"), PatientAttributeConfigurationDetailId = Guid.Parse("44D8E126-3582-48A4-BE2B-66ABBAAD55D5"), CountryId = Guid.Parse("50E847B7-C81E-43F6-9E07-30FB675232DC") },
                new PatientAttributeConfigurationMapper() { Id = Guid.Parse("2035D2EF-193C-4A92-B32B-C4FE8B05B1B1"), PatientAttributeConfigurationDetailId = Guid.Parse("3058B667-B346-4C5B-9393-2031F143C387"), CountryId = Guid.Parse("50E847B7-C81E-43F6-9E07-30FB675232DC") }
                );

            context.SaveChanges();
        }
    }
}
