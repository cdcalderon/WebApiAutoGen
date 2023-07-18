namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuthUserId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patient", "AuthUserId", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patient", "AuthUserId");
        }
    }
}
