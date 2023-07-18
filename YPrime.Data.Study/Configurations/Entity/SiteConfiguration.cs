using System;
using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Configurations.Extensions;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class SiteConfiguration
    {
        private static readonly string InitialSiteName = "Initial Site";

        public SiteConfiguration(EntityTypeConfiguration<Site> entityConfiguration)
        {
            entityConfiguration
                .HasMany(e => e.Patient)
                .WithRequired(e => e.Site)
                .WillCascadeOnDelete(false);
        }

        public static void Seed(StudyDbContext context, Guid siteId, Guid countryId)
        {
            context.Database.ExecuteSqlCommand("PRINT 'Site Seeding'");
            context.Sites.AddIfNotExists(
                new Site
                {
                    Id = siteId,
                    CountryId = countryId,
                    Name = InitialSiteName,
                    Investigator = "Investigator",
                    SiteNumber = "10001",
                    IsActive = true,
                    Address1 = "Address1",
                    City = "City",
                    State = "State",
                    Zip = "12345",
                    PhoneNumber = "123-456-7899",
                    PrimaryContact = "Primary Contact",
                    TimeZone = "Eastern Standard Time",
                    ConfigurationId = Config.Defaults.ConfigurationVersions.InitialVersion.Id
                },
                s => s.Id == siteId);
            context.SaveChanges();
        }
    }
}