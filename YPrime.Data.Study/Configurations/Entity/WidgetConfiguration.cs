using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using YPrime.Config.Enums;
using YPrime.Data.Study.Models;

namespace YPrime.Data.Study.Configurations.Entity
{
    public class WidgetConfiguration
    {
        public const string configSchemaName = "config";
        public const string widgetIdColumnName = "WidgetId";
        public const string studyTypeIdColumnName = "StudyTypeId";

        public WidgetConfiguration(EntityTypeConfiguration<Widget> entityConfiguration)
        {
        }

        public static void Seed(StudyDbContext context)
        {
            context.Database.ExecuteSqlCommand("PRINT 'Widget Seeding'");
            context.Widgets.AddOrUpdate(dt => dt.Id,
                new Widget
                {
                    Id = Guid.Parse("828413A1-12DC-46DC-91A0-A56846DAB64C"),
                    WidgetTypeId = WidgetType.CustomScreen.Id,
                    Name = "DCFWidget",
                    TranslationTitleText = "DCFWidgetTitle",
                    TranslationDescriptionText = "DCFWidgetDescription",
                    TranslationButtonText = null,
                    ControllerName = "UI",
                    ControllerActionName = "DCFWidget",
                    IconName = "fa-globe",
                    ReportId = null,
                    WidgetPosition = 1,
                    ColumnWidth = 2,
                    ColumnHeight = 1
                },
                new Widget
                {
                    Id = Guid.Parse("2A6A55A1-D806-40B5-94B4-3A6CDAF21754"),
                    WidgetTypeId = WidgetType.CustomScreen.Id,
                    Name = "EnrollmentWidget",
                    TranslationTitleText = "EnrollmentWidgetTitle",
                    TranslationDescriptionText = "EnrollmentWidgetDescription",
                    TranslationButtonText = null,
                    ControllerName = "Patient",
                    ControllerActionName = "EnrolledChart",
                    IconName = "fa-globe",
                    ReportId = null,
                    WidgetPosition = 17,
                    ColumnWidth = 1,
                    ColumnHeight = 1
                },
                new Widget
                {
                    Id = Guid.Parse("2F29B1E3-B3DC-47EE-B57A-BA65279B771A"),
                    WidgetTypeId = WidgetType.CustomScreen.Id,
                    Name = "RecentlyViewedPatientsWidget",
                    TranslationTitleText = "RecentlyViewedPatientsWidgetTitle",
                    TranslationDescriptionText = "RecentlyViewedPatientsWidgetDescription",
                    TranslationButtonText = "RecentlyViewedPatientsWidgetButton",
                    ControllerName = "Patient",
                    ControllerActionName = "RecentlyViewedPatients",
                    IconName = "fa-globe",
                    ReportId = null,
                    WidgetPosition = 7,
                    ColumnWidth = 1,
                    ColumnHeight = 1
                },
                new Widget
                {
                    Id = Guid.Parse("befce744-9897-4543-8fbb-172989a30310"),
                    WidgetTypeId = WidgetType.CustomScreen.Id,
                    Name = "Device Inventory",
                    TranslationTitleText = "DeviceInventoryWidgetTitle",
                    TranslationDescriptionText = "DeviceInventoryWidgetDescription",
                    TranslationButtonText = "DeviceInventoryWidgetTitle",
                    ControllerName = "Devices",
                    ControllerActionName = "Widget",
                    IconName = "fa-mobile",
                    ReportId = null,
                    WidgetPosition = 4,
                    ColumnWidth = 1,
                    ColumnHeight = 2
                },
                new Widget
                {
                    Id = Guid.Parse("8F3CB31A-5FB6-4A82-8AA5-AC065B141AEF"),
                    WidgetTypeId = WidgetType.CustomScreen.Id,
                    Name = "Upcoming Patient Visits",
                    TranslationTitleText = "UpcomingPatientVisitWidgetTitle",
                    TranslationDescriptionText = "UpcomingPatientVisitWidgetDescription",
                    TranslationButtonText = "UpcomingPatientVisitWidgetButton",
                    ControllerName = "Patient",
                    ControllerActionName = "UpcomingVisitsWidget",
                    IconName = "fa-hospital-o",
                    ReportId = null,
                    WidgetPosition = 18,
                    ColumnWidth = 2,
                    ColumnHeight = 1
                });

            context.SaveChanges();
        }
    }
}