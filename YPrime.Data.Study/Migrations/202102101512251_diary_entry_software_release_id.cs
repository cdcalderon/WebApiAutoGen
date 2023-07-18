namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class diary_entry_software_release_id : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DiaryEntry", "SoftwareReleaseId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DiaryEntry", "SoftwareReleaseId");
        }
    }
}
