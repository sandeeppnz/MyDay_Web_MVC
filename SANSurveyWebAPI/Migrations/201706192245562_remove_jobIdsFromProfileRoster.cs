namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_jobIdsFromProfileRoster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JobLogCreateSurveys", "ProfileRosterId", c => c.Int());
            AddColumn("dbo.JobLogCreateSurveys", "SurveyId", c => c.Int());
            AddColumn("dbo.JobLogShiftReminderEmails", "ProfileRosterId", c => c.Int(nullable: false));
            DropColumn("dbo.ProfileRosters", "ShiftReminderEmailJobId");
            DropColumn("dbo.ProfileRosters", "ShiftReminderSmsJobId");
            DropColumn("dbo.ProfileRosters", "CreateSurveyJobId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProfileRosters", "CreateSurveyJobId", c => c.Int());
            AddColumn("dbo.ProfileRosters", "ShiftReminderSmsJobId", c => c.Int());
            AddColumn("dbo.ProfileRosters", "ShiftReminderEmailJobId", c => c.Int());
            DropColumn("dbo.JobLogShiftReminderEmails", "ProfileRosterId");
            DropColumn("dbo.JobLogCreateSurveys", "SurveyId");
            DropColumn("dbo.JobLogCreateSurveys", "ProfileRosterId");
        }
    }
}
