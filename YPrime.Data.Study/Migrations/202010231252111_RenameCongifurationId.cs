namespace YPrime.Data.Study.Configurations.Context
{
    using System.Data.Entity.Migrations;

    public partial class RenameCongifurationId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Answer", "ConfigurationId", c => c.Guid());
            AddColumn("dbo.DiaryEntry", "ConfigurationId", c => c.Guid());
            AddColumn("dbo.Patient", "ConfigurationId", c => c.Guid());
            AddColumn("dbo.PatientVisit", "ConfigurationId", c => c.Guid());
            DropColumn("dbo.Answer", "CongifurationId");
            DropColumn("dbo.DiaryEntry", "CongifurationId");
            DropColumn("dbo.Patient", "CongifurationId");
            DropColumn("dbo.PatientVisit", "CongifurationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PatientVisit", "CongifurationId", c => c.Guid());
            AddColumn("dbo.Patient", "CongifurationId", c => c.Guid());
            AddColumn("dbo.DiaryEntry", "CongifurationId", c => c.Guid());
            AddColumn("dbo.Answer", "CongifurationId", c => c.Guid());
            DropColumn("dbo.PatientVisit", "ConfigurationId");
            DropColumn("dbo.Patient", "ConfigurationId");
            DropColumn("dbo.DiaryEntry", "ConfigurationId");
            DropColumn("dbo.Answer", "ConfigurationId");
        }
    }
}
