namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobLogUpdateSurvey_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JobLogUpdateSurveys",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileRosterId = c.Int(),
                        SurveyId = c.Int(),
                        ProfileId = c.Int(nullable: false),
                        JobMethod = c.String(maxLength: 10),
                        HangfireJobId = c.Int(),
                        RunAfterMin = c.Double(),
                        CreatedDateTimeServer = c.DateTime(nullable: false),
                        CreatedDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.JobLogUpdateSurveys");
        }
    }
}
