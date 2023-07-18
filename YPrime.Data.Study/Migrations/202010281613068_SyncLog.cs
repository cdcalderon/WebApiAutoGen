namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SyncLog", "ConfigurationVersionNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SyncLog", "ConfigurationVersionNumber");
        }
    }
}
