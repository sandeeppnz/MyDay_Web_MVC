namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editKidsTasks : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.KidsTasks", "SurveyId", "dbo.Surveys");
            DropIndex("dbo.KidsTasks", new[] { "SurveyId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.KidsTasks", "SurveyId");
            AddForeignKey("dbo.KidsTasks", "SurveyId", "dbo.Surveys", "Id", cascadeDelete: true);
        }
    }
}
