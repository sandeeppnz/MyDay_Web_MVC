namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_surveyId : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.JobLogCreateSurveys", "SurveyId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.JobLogCreateSurveys", "SurveyId", c => c.Int());
        }
    }
}
