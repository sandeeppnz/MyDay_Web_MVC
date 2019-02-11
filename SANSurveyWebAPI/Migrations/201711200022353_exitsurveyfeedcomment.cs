namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class exitsurveyfeedcomment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExitSurvey_Feedback",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        feedbackComments = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ExitSurvey_Feedback");
        }
    }
}
