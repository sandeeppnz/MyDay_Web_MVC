namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newWAMResponses : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WAMResponses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        SurveyId = c.Int(nullable: false),
                        TaskId = c.Int(nullable: false),
                        TaskOther = c.String(maxLength: 255),
                        StartResponseDateTimeUtc = c.DateTime(),
                        EndResponseDateTimeUtc = c.DateTime(),
                        ShiftStartDateTime = c.DateTime(),
                        ShiftEndDateTime = c.DateTime(),
                        TaskStartDateTime = c.DateTime(),
                        TaskEndDateTime = c.DateTime(),
                        Question = c.String(maxLength: 10),
                        Answer = c.String(maxLength: 255),
                        SurveyWindowStartDateTime = c.DateTime(),
                        SurveyWindowEndDateTime = c.DateTime(),
                        IsOtherTask = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .ForeignKey("dbo.Surveys", t => t.SurveyId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.SurveyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WAMResponses", "SurveyId", "dbo.Surveys");
            DropForeignKey("dbo.WAMResponses", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.WAMResponses", new[] { "SurveyId" });
            DropIndex("dbo.WAMResponses", new[] { "ProfileId" });
            DropTable("dbo.WAMResponses");
        }
    }
}
