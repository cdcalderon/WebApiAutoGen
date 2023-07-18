namespace YPrime.Data.Study.Configurations.Context
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class correctionAdditionalData_update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CorrectionApprovalDataAdditional", "IgnorePropertyUpdate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CorrectionApprovalDataAdditional", "IgnorePropertyUpdate");
        }
    }
}
