namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class ScreenReportSiteFacingDefaultValue : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ScreenReportDialog", "IsSiteFacing", c => c.Boolean(nullable: false,
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "False", newValue: "0")
                    },
                }));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ScreenReportDialog", "IsSiteFacing", c => c.Boolean(nullable: false,
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: "0", newValue: "False")
                    },
                }));
        }
    }
}
