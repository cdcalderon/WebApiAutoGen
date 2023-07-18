namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    using YPrime.Data.Study.Helpers;

    public partial class AddAnalyticsReferenceAndStudyRoles : DbMigration
    {
        public override void Up()
        {
            Sql(DbMigrationHelper.GetDisableChangeDataCaptureSql("dbo", "AnalyticsReference"));
            Sql(DbMigrationHelper.GetDisableChangeDataCaptureSql("dbo", "AnalyticsReferenceStudyRole"));
            CreateTable(
                "dbo.AnalyticsReference",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InternalName = c.String(maxLength: 50),
                        DisplayName = c.String(maxLength: 50),
                        Description = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AnalyticsReferenceStudyRole",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AnalyticsReferenceId = c.Guid(nullable: false),
                        StudyRoleId = c.Guid(nullable: false),
                        AuditUserID = c.String(),
                        LastModified = c.DateTime(nullable: false, precision: 7, storeType: "datetime2",
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "SqlDefaultValue",
                                    new AnnotationValues(oldValue: null, newValue: "GETUTCDATE()")
                                },
                            }),
                        LastModifiedByDatabaseUser = c.String(
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "SqlDefaultValue",
                                    new AnnotationValues(oldValue: null, newValue: "SUSER_NAME()")
                                },
                            }),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnalyticsReference", t => t.AnalyticsReferenceId)
                .Index(t => t.AnalyticsReferenceId);
            
        }
        
        public override void Down()
        {
            Sql(DbMigrationHelper.GetDisableChangeDataCaptureSql("dbo", "AnalyticsReference"));
            Sql(DbMigrationHelper.GetDisableChangeDataCaptureSql("dbo", "AnalyticsReferenceStudyRole"));
            DropForeignKey("dbo.AnalyticsReferenceStudyRole", "AnalyticsReferenceId", "dbo.AnalyticsReference");
            DropIndex("dbo.AnalyticsReferenceStudyRole", new[] { "AnalyticsReferenceId" });
            DropTable("dbo.AnalyticsReferenceStudyRole",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "LastModified",
                        new Dictionary<string, object>
                        {
                            { "SqlDefaultValue", "GETUTCDATE()" },
                        }
                    },
                    {
                        "LastModifiedByDatabaseUser",
                        new Dictionary<string, object>
                        {
                            { "SqlDefaultValue", "SUSER_NAME()" },
                        }
                    },
                });
            DropTable("dbo.AnalyticsReference");
        }
    }
}
