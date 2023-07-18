namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SponsorReportProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AnalyticsReference", "SponsorReport", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AnalyticsReference", "SponsorReport");
        }
    }
}
