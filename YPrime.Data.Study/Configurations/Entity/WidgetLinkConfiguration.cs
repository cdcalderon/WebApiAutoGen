using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class WidgetLinkConfiguration
    {
        public WidgetLinkConfiguration(EntityTypeConfiguration<Widget> entityConfiguration)
        {
            entityConfiguration.Property(e => e.Id).IsRequired();
        }

        public static void Seed(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'Widget Link Seeding'");

            context.WidgetLinks.AddOrUpdate(dt => dt.Id,
                new WidgetLink
                {
                    Id = Guid.Parse("EC7BBA69-2BD0-471F-A1AE-9AA030B4DAFD"),
                    WidgetId = Guid.Parse("BEFCE744-9897-4543-8FBB-172989A30310"),
                    ControllerName = "Device",
                    ControllerActionName = "Index",
                    TranslationText = "ViewInventoryClick"
                }
            );
        }
    }
}