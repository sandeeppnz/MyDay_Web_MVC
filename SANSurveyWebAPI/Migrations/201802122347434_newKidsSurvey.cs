namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newKidsSurvey : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KidsSurveys",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Uid = c.String(),
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
            DropForeignKey("dbo.KidsSurveys", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.KidsSurveys", new[] { "ProfileId" });
            DropTable("dbo.KidsSurveys");
        }
    }
}
