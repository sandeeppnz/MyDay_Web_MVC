namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editWellBeingFirstJobSurveyId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FirstJobs", "ExitSurveyId", c => c.Int(nullable: false));
            AddColumn("dbo.WellBeings", "ExitSurveyId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WellBeings", "ExitSurveyId");
            DropColumn("dbo.FirstJobs", "ExitSurveyId");
        }
    }
}
