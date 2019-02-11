namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_email_field_Joblogs : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.JobLogBaselineSurveyEmails", "Email");
            DropColumn("dbo.JobLogCompleteSurveyReminderEmails", "Email");
            DropColumn("dbo.JobLogExpiringSoonSurveyNotCompletedReminderEmails", "Email");
            DropColumn("dbo.JobLogExpiringSoonSurveyNotStartedReminderEmails", "Email");
            DropColumn("dbo.JobLogRegistrationCompletedEmails", "Email");
            DropColumn("dbo.JobLogShiftReminderEmails", "Email");
            DropColumn("dbo.JobLogStartSurveyReminderEmails", "Email");
        }
        
        public override void Down()
        {
            AddColumn("dbo.JobLogStartSurveyReminderEmails", "Email", c => c.Binary());
            AddColumn("dbo.JobLogShiftReminderEmails", "Email", c => c.Binary());
            AddColumn("dbo.JobLogRegistrationCompletedEmails", "Email", c => c.Binary());
            AddColumn("dbo.JobLogExpiringSoonSurveyNotStartedReminderEmails", "Email", c => c.Binary());
            AddColumn("dbo.JobLogExpiringSoonSurveyNotCompletedReminderEmails", "Email", c => c.Binary());
            AddColumn("dbo.JobLogCompleteSurveyReminderEmails", "Email", c => c.Binary());
            AddColumn("dbo.JobLogBaselineSurveyEmails", "Email", c => c.Binary());
        }
    }
}
