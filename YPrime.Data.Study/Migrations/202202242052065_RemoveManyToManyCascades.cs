namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveManyToManyCascades : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SoftwareReleaseSite", "SoftwareRelease_Id", "dbo.SoftwareRelease");
            DropForeignKey("dbo.SoftwareReleaseSite", "Site_Id", "dbo.Site");
            AddForeignKey("dbo.SoftwareReleaseSite", "SoftwareRelease_Id", "dbo.SoftwareRelease", "Id");
            AddForeignKey("dbo.SoftwareReleaseSite", "Site_Id", "dbo.Site", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SoftwareReleaseSite", "Site_Id", "dbo.Site");
            DropForeignKey("dbo.SoftwareReleaseSite", "SoftwareRelease_Id", "dbo.SoftwareRelease");
            AddForeignKey("dbo.SoftwareReleaseSite", "Site_Id", "dbo.Site", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SoftwareReleaseSite", "SoftwareRelease_Id", "dbo.SoftwareRelease", "Id", cascadeDelete: true);
        }
    }
}
