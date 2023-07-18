namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDcfAuditHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CorrectionHistory", "UserName", c => c.String());
            AddColumn("dbo.CorrectionHistory", "FirstName", c => c.String());
            AddColumn("dbo.CorrectionHistory", "LastName", c => c.String());
            AddColumn("dbo.CorrectionHistory", "DateTimeStamp", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CorrectionHistory", "DateTimeStamp");
            DropColumn("dbo.CorrectionHistory", "LastName");
            DropColumn("dbo.CorrectionHistory", "FirstName");
            DropColumn("dbo.CorrectionHistory", "UserName");
        }
    }
}
