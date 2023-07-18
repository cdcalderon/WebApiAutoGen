using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class InputFieldTypeResultConfiguration
    {
        public InputFieldTypeResultConfiguration(EntityTypeConfiguration<InputFieldTypeResult> entityConfiguration)
        {
        }

        public static void Seed(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'InputFieldTypeResult seeding'");

            context.InputFieldTypeResults.AddOrUpdate(i => i.Id,
                new InputFieldTypeResult
                {
                    Id = 1, ResultCode = "FVC", Description = "Fvc_cL", InputFieldTypeId = 34,
                    IsPatientAttribute = false, UnitOfMeasure = "l", PatientAttributeConfigurationDetailId = null
                },
                new InputFieldTypeResult
                {
                    Id = 2, ResultCode = "FEV1", Description = "Fev1_cL", InputFieldTypeId = 34,
                    IsPatientAttribute = false, UnitOfMeasure = "l", PatientAttributeConfigurationDetailId = null
                },
                new InputFieldTypeResult
                {
                    Id = 3, ResultCode = "FEV6", Description = "Fev6_cl", InputFieldTypeId = 34,
                    IsPatientAttribute = false, UnitOfMeasure = "l", PatientAttributeConfigurationDetailId = null
                },
                new InputFieldTypeResult
                {
                    Id = 4, ResultCode = "FEV1/FVC", Description = "Fev1_Fvc_pcnt", InputFieldTypeId = 34,
                    IsPatientAttribute = false, UnitOfMeasure = "%", PatientAttributeConfigurationDetailId = null
                },
                new InputFieldTypeResult
                {
                    Id = 5, ResultCode = "FEF25-75", Description = "Fef2575_cLs", InputFieldTypeId = 34,
                    IsPatientAttribute = false, UnitOfMeasure = "lpm", PatientAttributeConfigurationDetailId = null
                },
                new InputFieldTypeResult
                {
                    Id = 6, ResultCode = "PEF", Description = "Pef_cLs", InputFieldTypeId = 34,
                    IsPatientAttribute = false, UnitOfMeasure = "lpm", PatientAttributeConfigurationDetailId = null
                },
                new InputFieldTypeResult
                {
                    Id = 7, ResultCode = "Birth Date", Description = "Birth Date", InputFieldTypeId = 34,
                    IsPatientAttribute = true, UnitOfMeasure = null, PatientAttributeConfigurationDetailId = null
                },
                new InputFieldTypeResult
                {
                    Id = 8, ResultCode = "Height", Description = "Height", InputFieldTypeId = 34,
                    IsPatientAttribute = true, UnitOfMeasure = null, PatientAttributeConfigurationDetailId = null
                },
                new InputFieldTypeResult
                {
                    Id = 9, ResultCode = "Weight", Description = "Weight", InputFieldTypeId = 34,
                    IsPatientAttribute = true, UnitOfMeasure = null, PatientAttributeConfigurationDetailId = null
                },
                new InputFieldTypeResult
                {
                    Id = 10, ResultCode = "Gender", Description = "Gender", InputFieldTypeId = 34,
                    IsPatientAttribute = true, UnitOfMeasure = null, PatientAttributeConfigurationDetailId = null
                },
                new InputFieldTypeResult
                {
                    Id = 11, ResultCode = "Ethnicity", Description = "Ethnicity", InputFieldTypeId = 34,
                    IsPatientAttribute = true, UnitOfMeasure = null, PatientAttributeConfigurationDetailId = null
                });
            context.SaveChanges();
        }
    }
}