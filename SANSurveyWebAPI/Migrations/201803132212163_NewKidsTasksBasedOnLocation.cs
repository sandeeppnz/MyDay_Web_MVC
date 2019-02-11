namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewKidsTasksBasedOnLocation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KidsTasksOnLocations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        KidsSurveyId = c.Int(nullable: false),
                        KidsLocationId = c.Int(nullable: false),
                        LocationName = c.String(),
                        SpentStartTime = c.String(),
                        SpentEndTime = c.String(),
                        TasksDone = c.String(),
                        TaskOther = c.String(),
                        IsEmotionalAffectCompleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KidsTasksOnLocations", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.KidsTasksOnLocations", new[] { "ProfileId" });
            DropTable("dbo.KidsTasksOnLocations");
        }
    }
}
