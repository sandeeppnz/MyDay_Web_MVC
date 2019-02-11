namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newKidsTasks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KidsTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        SurveyId = c.Int(nullable: false),
                        TaskName = c.String(),
                        Venue = c.String(),
                        Travel = c.String(),
                        Guardian = c.String(),
                        StartTime = c.DateTime(),
                        EndTime = c.DateTime(),
                        IsEmotionalStageCompleted = c.Boolean(nullable: false),
                        IsRandomlySelected = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .ForeignKey("dbo.Surveys", t => t.SurveyId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.SurveyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KidsTasks", "SurveyId", "dbo.Surveys");
            DropForeignKey("dbo.KidsTasks", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.KidsTasks", new[] { "SurveyId" });
            DropIndex("dbo.KidsTasks", new[] { "ProfileId" });
            DropTable("dbo.KidsTasks");
        }
    }
}
