namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovePatientAssetTagColumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Patient", "AssetTag");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Patient", "AssetTag", c => c.String());
        }
    }
}
