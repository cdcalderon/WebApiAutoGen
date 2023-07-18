namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPinHashToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StudyUser", "PinHash", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StudyUser", "PinHash");
        }
    }
}
