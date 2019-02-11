namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_joblogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JobLogBaselineSurveyEmails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
                "dbo.JobLogCreateSurveys",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        JobMethod = c.String(maxLength: 10),
                        HangfireJobId = c.Int(),
                        RunAfterMin = c.Double(),
                        CreatedDateTimeServer = c.DateTime(nullable: false),
                        CreatedDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.JobLogRegistrationCompletedEmails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
                "dbo.JobLogShiftReminderEmails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
            DropTable("dbo.JobLogShiftReminderEmails");
            DropTable("dbo.JobLogRegistrationCompletedEmails");
            DropTable("dbo.JobLogCreateSurveys");
            DropTable("dbo.JobLogBaselineSurveyEmails");
        }
    }
}
