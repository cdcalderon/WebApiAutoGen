namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConsentParticipantId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patient", "ConsentParticipantId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patient", "ConsentParticipantId");
        }
    }
}
