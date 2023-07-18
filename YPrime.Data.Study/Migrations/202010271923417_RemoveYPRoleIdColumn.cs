namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveYPRoleIdColumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.StudyUserRole", "YPRoleId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StudyUserRole", "YPRoleId", c => c.Guid(nullable: false));
        }
    }
}
