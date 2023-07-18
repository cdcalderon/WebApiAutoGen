namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SiteDisplayLanguageId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Site", "SiteDisplayLanguageId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Site", "SiteDisplayLanguageId");
        }
    }
}
