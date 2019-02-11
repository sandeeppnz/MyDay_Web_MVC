namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSurveyId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Feedbacks", "SurveyId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Feedbacks", "SurveyId");
        }
    }
}
