namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeviceDatasTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeviceData",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DeviceId = c.Guid(nullable: false),
                        Fob = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Device", t => t.DeviceId)
                .Index(t => t.DeviceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DeviceData", "DeviceId", "dbo.Device");
            DropIndex("dbo.DeviceData", new[] { "DeviceId" });
            DropTable("dbo.DeviceData");
        }
    }
}
