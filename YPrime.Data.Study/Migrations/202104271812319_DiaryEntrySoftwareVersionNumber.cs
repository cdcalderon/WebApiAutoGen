namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DiaryEntrySoftwareVersionNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DiaryEntry", "SoftwareVersionNumber", c => c.String());
            DropColumn("dbo.DiaryEntry", "SoftwareReleaseId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DiaryEntry", "SoftwareReleaseId", c => c.Guid());
            DropColumn("dbo.DiaryEntry", "SoftwareVersionNumber");
        }
    }
}
