namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class AddDialogTypeToScreenReportDialog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScreenReportDialog", "DialogType", c => c.String(
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
            DropColumn("dbo.ScreenReportDialog", "DialogType",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "SqlDefaultValue", "'Both'" },
                });
        }
    }
}
