namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewExitV2survey : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExitV2Survey",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Uid = c.String(),
                        SurveyDate = c.DateTime(nullable: false),
                        EntryStartUTC = c.DateTime(),
                        EntryStartCurrent = c.DateTime(),
                        EndTimeUTC = c.DateTime(),
                        EndTimeCurrent = c.DateTime(),
                        SurveyProgress = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExitV2Survey", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.ExitV2Survey", new[] { "ProfileId" });
            DropTable("dbo.ExitV2Survey");
        }
    }
}
