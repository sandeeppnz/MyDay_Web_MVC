namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CaseWorkersDemographicsEdited : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Demographics", "MaritalStatus", c => c.String(maxLength: 60));
            AddColumn("dbo.Demographics", "IsCaregiverAdult", c => c.String(maxLength: 10));
            AlterColumn("dbo.Demographics", "BirthYear", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Demographics", "BirthYear", c => c.String(maxLength: 60));
            DropColumn("dbo.Demographics", "IsCaregiverAdult");
            DropColumn("dbo.Demographics", "MaritalStatus");
        }
    }
}
