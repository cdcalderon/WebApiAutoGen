namespace YPrime.Data.Study.Configurations.Context
{
    using System.Data.Entity.Migrations;

    public partial class NonNullableConfigIds : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Answer", "ConfigurationId", c => c.Guid(nullable: false));
            AlterColumn("dbo.DiaryEntry", "ConfigurationId", c => c.Guid(nullable: false));
            AlterColumn("dbo.Patient", "ConfigurationId", c => c.Guid(nullable: false));
            AlterColumn("dbo.PatientVisit", "ConfigurationId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PatientVisit", "ConfigurationId", c => c.Guid());
            AlterColumn("dbo.Patient", "ConfigurationId", c => c.Guid());
            AlterColumn("dbo.DiaryEntry", "ConfigurationId", c => c.Guid());
            AlterColumn("dbo.Answer", "ConfigurationId", c => c.Guid());
        }
    }
}
