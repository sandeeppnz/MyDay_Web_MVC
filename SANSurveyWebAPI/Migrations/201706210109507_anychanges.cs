namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class anychanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JobLogCompleteSurveyReminderEmails", "ProfileRosterId", c => c.Int(nullable: false));
            AddColumn("dbo.JobLogExpiringSoonSurveyNotCompletedReminderEmails", "ProfileRosterId", c => c.Int(nullable: false));
            AddColumn("dbo.JobLogExpiringSoonSurveyNotStartedReminderEmails", "ProfileRosterId", c => c.Int(nullable: false));
            AddColumn("dbo.JobLogStartSurveyReminderEmails", "ProfileRosterId", c => c.Int(nullable: false));
            DropColumn("dbo.Surveys", "SendSurveyEmailJobId");
            DropColumn("dbo.Surveys", "SendSurveySmsJobId");
            DropColumn("dbo.Surveys", "CompleteSurveyReminderEmailJobId");
            DropColumn("dbo.Surveys", "CompleteSurveyReminderSmsJobId");
            DropColumn("dbo.Surveys", "ExpiringSoonNotStartedSurveyReminderEmailJobId");
            DropColumn("dbo.Surveys", "ExpiringSoonNotStartedSurveyReminderSmsJobId");
            DropColumn("dbo.Surveys", "ExpiringSoonNotCompeletedSurveyReminderEmailJobId");
            DropColumn("dbo.Surveys", "ExpiringSoonNotCompeletedSurveyReminderSmsJobId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Surveys", "ExpiringSoonNotCompeletedSurveyReminderSmsJobId", c => c.Int());
            AddColumn("dbo.Surveys", "ExpiringSoonNotCompeletedSurveyReminderEmailJobId", c => c.Int());
            AddColumn("dbo.Surveys", "ExpiringSoonNotStartedSurveyReminderSmsJobId", c => c.Int());
            AddColumn("dbo.Surveys", "ExpiringSoonNotStartedSurveyReminderEmailJobId", c => c.Int());
            AddColumn("dbo.Surveys", "CompleteSurveyReminderSmsJobId", c => c.Int());
            AddColumn("dbo.Surveys", "CompleteSurveyReminderEmailJobId", c => c.Int());
            AddColumn("dbo.Surveys", "SendSurveySmsJobId", c => c.Int());
            AddColumn("dbo.Surveys", "SendSurveyEmailJobId", c => c.Int());
            DropColumn("dbo.JobLogStartSurveyReminderEmails", "ProfileRosterId");
            DropColumn("dbo.JobLogExpiringSoonSurveyNotStartedReminderEmails", "ProfileRosterId");
            DropColumn("dbo.JobLogExpiringSoonSurveyNotCompletedReminderEmails", "ProfileRosterId");
            DropColumn("dbo.JobLogCompleteSurveyReminderEmails", "ProfileRosterId");
        }
    }
}
