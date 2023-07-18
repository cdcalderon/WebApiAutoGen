namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class screenreport_dialog_table_add_devicetype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScreenReportDialog", "DeviceType", c => c.String(
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "'Both'")
                    },
                }));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScreenReportDialog", "DeviceType",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "SqlDefaultValue", "'Both'" },
                });
        }
    }
}
