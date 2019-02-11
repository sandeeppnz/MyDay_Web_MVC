namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_SurveyId_to_JobLogCreateSurvey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JobLogCreateSurveys", "SurveyId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.JobLogCreateSurveys", "SurveyId");
        }
    }
}
