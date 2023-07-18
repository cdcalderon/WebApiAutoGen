namespace YPrime.Data.Study.Configurations.Context
{
    using System.Data.Entity.Migrations;

    public partial class DcfConfigId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Correction", "ConfigurationId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Correction", "ConfigurationId");
        }
    }
}
