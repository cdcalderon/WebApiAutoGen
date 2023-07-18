namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDataSyncColumnsToStudyUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StudyUser", "SyncVersion", c => c.Int(nullable: false));
            AddColumn("dbo.StudyUser", "IsDirty", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StudyUser", "IsDirty");
            DropColumn("dbo.StudyUser", "SyncVersion");
        }
    }
}
