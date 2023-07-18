using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class StudyRoleWidgetConfiguration
    {
        public StudyRoleWidgetConfiguration(EntityTypeConfiguration<StudyRoleWidget> entityConfiguration)
        {
        }

        public static void Seed(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'StudyRoleWidget Seeding'");
            var widgetNames = new List<string> {"DCFWidget", "Device Inventory", "RecentlyViewedPatientsWidget", "EnrollmentWidget"};

            var widgets = context.Widgets.Where(w => widgetNames.Contains(w.Name)).OrderBy(x => x.WidgetPosition);
            foreach (var role in Config.Defaults.StudyRoles.Defaults)
            {
                var count = 1;
                foreach (var widget in widgets)
                {
                    context.StudyRoleWidgets.AddOrUpdate(w => new { w.StudyRoleId, w.WidgetId },
                        new StudyRoleWidget
                        {
                            StudyRoleId = role.Value.Id,
                            Widget = widget,
                            WidgetId = widget.Id,
                            WidgetPosition = count
                        });
                    count++;
                }
            }
        }
    }
}