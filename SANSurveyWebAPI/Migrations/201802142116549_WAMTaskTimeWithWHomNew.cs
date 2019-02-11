namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WAMTaskTimeWithWHomNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WAMWithWhomTaskTimes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        SurveyId = c.Int(nullable: false),
                        TaskId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                        WithWhomOptions = c.String(),
                        WithWhomOther = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .ForeignKey("dbo.Surveys", t => t.SurveyId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.SurveyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WAMWithWhomTaskTimes", "SurveyId", "dbo.Surveys");
            DropForeignKey("dbo.WAMWithWhomTaskTimes", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.WAMWithWhomTaskTimes", new[] { "SurveyId" });
            DropIndex("dbo.WAMWithWhomTaskTimes", new[] { "ProfileId" });
            DropTable("dbo.WAMWithWhomTaskTimes");
        }
    }
}
