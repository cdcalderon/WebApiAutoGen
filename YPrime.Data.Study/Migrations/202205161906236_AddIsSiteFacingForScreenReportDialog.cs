namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsSiteFacingForScreenReportDialog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScreenReportDialog", "IsSiteFacing", c => c.Boolean(nullable: false,
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "0")
                    },
                }));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScreenReportDialog", "IsSiteFacing",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "SqlDefaultValue", "0" },
                });
        }
    }
}
