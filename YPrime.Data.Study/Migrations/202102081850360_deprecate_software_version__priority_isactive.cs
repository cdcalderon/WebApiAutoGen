namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class deprecate_software_version__priority_isactive : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SoftwareVersion", "Priority");
            DropColumn("dbo.SoftwareVersion", "IsActive",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "SqlDefaultValue", "1" },
                });
        }
        
        public override void Down()
        {
            AddColumn("dbo.SoftwareVersion", "IsActive", c => c.Boolean(nullable: false,
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "SqlDefaultValue",
                        new AnnotationValues(oldValue: null, newValue: "1")
                    },
                }));
            AddColumn("dbo.SoftwareVersion", "Priority", c => c.Boolean(nullable: false));
        }
    }
}
