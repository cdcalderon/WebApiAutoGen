namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NonNullableCorrectionConfigId : DbMigration
    {
        public override void Up()
        {
            Sql($"UPDATE dbo.Correction SET ConfigurationId = '24791b54-83f9-4b2c-85a0-c83993538726' WHERE ConfigurationId IS NULL");
            AlterColumn("dbo.Correction", "ConfigurationId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Correction", "ConfigurationId", c => c.Guid());
        }
    }
}
