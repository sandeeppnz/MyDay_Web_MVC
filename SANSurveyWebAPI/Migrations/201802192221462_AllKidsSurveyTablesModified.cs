namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AllKidsSurveyTablesModified : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.KidsResponses", "SurveyId", "dbo.Surveys");
            DropIndex("dbo.KidsResponses", new[] { "SurveyId" });
            AddColumn("dbo.KidsResponses", "KidsSurveyId", c => c.Int(nullable: false));
            AddColumn("dbo.KidsResponses", "KidsTaskId", c => c.Int(nullable: false));
            AddColumn("dbo.KidsResponses", "TaskName", c => c.String());
            AddColumn("dbo.KidsResponses", "SurveyDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.KidsSurveys", "SurveyDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.KidsTasks", "KidsSurveyId", c => c.Int(nullable: false));
            AddColumn("dbo.KidsTasks", "SurveyDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.KidsResponses", "SurveyId");
            DropColumn("dbo.KidsResponses", "TaskId");
            DropColumn("dbo.KidsTasks", "SurveyId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.KidsTasks", "SurveyId", c => c.Int(nullable: false));
            AddColumn("dbo.KidsResponses", "TaskId", c => c.Int(nullable: false));
            AddColumn("dbo.KidsResponses", "SurveyId", c => c.Int(nullable: false));
            DropColumn("dbo.KidsTasks", "SurveyDate");
            DropColumn("dbo.KidsTasks", "KidsSurveyId");
            DropColumn("dbo.KidsSurveys", "SurveyDate");
            DropColumn("dbo.KidsResponses", "SurveyDate");
            DropColumn("dbo.KidsResponses", "TaskName");
            DropColumn("dbo.KidsResponses", "KidsTaskId");
            DropColumn("dbo.KidsResponses", "KidsSurveyId");
            CreateIndex("dbo.KidsResponses", "SurveyId");
            AddForeignKey("dbo.KidsResponses", "SurveyId", "dbo.Surveys", "Id", cascadeDelete: true);
        }
    }
}
