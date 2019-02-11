namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KidsEmoStage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KidsEmoStageTrackeds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        KidsSurveyId = c.Int(nullable: false),
                        TaskWhile = c.String(),
                        SurveyDate = c.String(),
                        TaskStartTime = c.String(),
                        TaskEndTime = c.String(),
                        KidsLocationId = c.Int(),
                        KidsTravelId = c.Int(),
                        LocationName = c.String(),
                        ModeOfTransport = c.String(),
                        KidsTaskId = c.Int(),
                        TaskPerformed = c.String(),
                        IsEmoAffStageCompleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
            DropColumn("dbo.KidsTasksOnLocations", "IsEmotionalAffectCompleted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.KidsTasksOnLocations", "IsEmotionalAffectCompleted", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.KidsEmoStageTrackeds", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.KidsEmoStageTrackeds", new[] { "ProfileId" });
            DropTable("dbo.KidsEmoStageTrackeds");
        }
    }
}
