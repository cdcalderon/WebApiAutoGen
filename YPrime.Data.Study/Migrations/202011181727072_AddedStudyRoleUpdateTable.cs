namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStudyRoleUpdateTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StudyRoleUpdate",
                c => new
                    {
                        StudyRoleId = c.Guid(nullable: false),
                        LastUpdate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.StudyRoleId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.StudyRoleUpdate");
        }
    }
}
