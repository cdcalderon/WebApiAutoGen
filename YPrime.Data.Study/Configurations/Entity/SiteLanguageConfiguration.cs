using System;
using YPrime.Data.Study.Configurations.Extensions;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class SiteLanguageConfiguration
    {
        public static void Seed(StudyDbContext context, Guid siteId, Guid languageId)
        {
            context.Database.ExecuteSqlCommand("PRINT 'SiteLangauge Seeding'");
            context.SiteLanguages.AddIfNotExists(
                new SiteLanguage
                {
                    Id = Guid.NewGuid(),
                    SiteId = siteId,
                    LanguageId = languageId
                },
                sl => sl.SiteId == siteId && sl.LanguageId == languageId);
            context.SaveChanges();
        }
    }
}