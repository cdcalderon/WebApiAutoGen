namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_softwarereleaseCountry : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.SoftwareReleaseCountry", name: "SoftwareRelease_Id", newName: "SoftwareReleaseId");
            RenameColumn(table: "dbo.SoftwareReleaseCountry", name: "Country_Id", newName: "CountryId");
            RenameIndex(table: "dbo.SoftwareReleaseCountry", name: "IX_SoftwareRelease_Id", newName: "IX_SoftwareReleaseId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.SoftwareReleaseCountry", name: "IX_SoftwareReleaseId", newName: "IX_SoftwareRelease_Id");
            RenameColumn(table: "dbo.SoftwareReleaseCountry", name: "CountryId", newName: "Country_Id");
            RenameColumn(table: "dbo.SoftwareReleaseCountry", name: "SoftwareReleaseId", newName: "SoftwareRelease_Id");
        }
    }
}
