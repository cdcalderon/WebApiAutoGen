namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnswerIsArchived : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Answer", "IsArchived", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Answer", "IsArchived");
        }
    }
}
