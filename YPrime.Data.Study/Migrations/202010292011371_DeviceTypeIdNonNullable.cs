namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeviceTypeIdNonNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Site", "ConfigurationId", c => c.Guid(nullable: false));
            AlterColumn("dbo.Device", "DeviceTypeId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Device", "DeviceTypeId", c => c.Guid());
            AlterColumn("dbo.Site", "ConfigurationId", c => c.Guid());
        }
    }
}
