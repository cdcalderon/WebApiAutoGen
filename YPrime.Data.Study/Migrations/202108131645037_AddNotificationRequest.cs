namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNotificationRequest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotificationRequest",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        PatientId = c.Guid(nullable: false),
                        AuthenticationHeader = c.String(),
                        RequestBody = c.String(),
                        ReponseCode = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            DropTable("dbo.NotificationRequest");
        }
    }
}
