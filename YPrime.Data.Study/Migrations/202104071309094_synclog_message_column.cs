namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class synclog_message_column : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SyncLog", "SyncLogMessage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SyncLog", "SyncLogMessage");
        }
    }
}
