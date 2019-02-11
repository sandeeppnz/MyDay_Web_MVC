namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KidsResponsesNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KidsResponses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        SurveyId = c.Int(nullable: false),
                        TaskId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                        Answer = c.String(),
                        ResponseStartTimeUTC = c.DateTime(),
                        ResponseEndTimeUTC = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .ForeignKey("dbo.Surveys", t => t.SurveyId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.SurveyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KidsResponses", "SurveyId", "dbo.Surveys");
            DropForeignKey("dbo.KidsResponses", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.KidsResponses", new[] { "SurveyId" });
            DropIndex("dbo.KidsResponses", new[] { "ProfileId" });
            DropTable("dbo.KidsResponses");
        }
    }
}
