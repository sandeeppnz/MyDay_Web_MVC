namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewKidsReactionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KidsReactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        KidsSurveyId = c.Int(nullable: false),
                        SurveyDate = c.String(),
                        ResponseStartTime = c.DateTime(nullable: false),
                        ResponseEndTime = c.DateTime(nullable: false),
                        KidsLocationId = c.Int(),
                        KidsTravelId = c.Int(),
                        KidsTaskId = c.Int(),
                        TasksPerformed = c.String(),
                        QuestionId = c.String(),
                        Answer = c.String(),
                        TaskStartTime = c.String(),
                        TaskEndTime = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KidsReactions", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.KidsReactions", new[] { "ProfileId" });
            DropTable("dbo.KidsReactions");
        }
    }
}
