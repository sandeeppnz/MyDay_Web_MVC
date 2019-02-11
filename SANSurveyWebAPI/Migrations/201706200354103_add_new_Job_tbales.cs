namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_new_Job_tbales : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JobLogCompleteSurveyReminderEmails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SurveyId = c.Int(nullable: false),
                        Email = c.Binary(),
                        ProfileId = c.Int(nullable: false),
                        JobMethod = c.String(maxLength: 10),
                        HangfireJobId = c.Int(),
                        RunAfterMin = c.Double(),
                        CreatedDateTimeServer = c.DateTime(nullable: false),
                        CreatedDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.JobLogExpiringSoonSurveyNotCompletedReminderEmails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SurveyId = c.Int(nullable: false),
                        Email = c.Binary(),
                        ProfileId = c.Int(nullable: false),
                        JobMethod = c.String(maxLength: 10),
                        HangfireJobId = c.Int(),
                        RunAfterMin = c.Double(),
                        CreatedDateTimeServer = c.DateTime(nullable: false),
                        CreatedDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.JobLogExpiringSoonSurveyNotStartedReminderEmails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SurveyId = c.Int(nullable: false),
                        Email = c.Binary(),
                        ProfileId = c.Int(nullable: false),
                        JobMethod = c.String(maxLength: 10),
                        HangfireJobId = c.Int(),
                        RunAfterMin = c.Double(),
                        CreatedDateTimeServer = c.DateTime(nullable: false),
                        CreatedDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.JobLogStartSurveyReminderEmails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SurveyId = c.Int(nullable: false),
                        Email = c.Binary(),
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
            DropTable("dbo.JobLogStartSurveyReminderEmails");
            DropTable("dbo.JobLogExpiringSoonSurveyNotStartedReminderEmails");
            DropTable("dbo.JobLogExpiringSoonSurveyNotCompletedReminderEmails");
            DropTable("dbo.JobLogCompleteSurveyReminderEmails");
        }
    }
}
