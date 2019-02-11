namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class exitsurveypageonemodified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExitSurvey_Page1", "Qn", c => c.String());
            AddColumn("dbo.ExitSurvey_Page1", "Ans", c => c.String(maxLength: 60));
            DropColumn("dbo.ExitSurvey_Page1", "PresentLife");
            DropColumn("dbo.ExitSurvey_Page1", "PresentHome");
            DropColumn("dbo.ExitSurvey_Page1", "PresentJob");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ExitSurvey_Page1", "PresentJob", c => c.String(maxLength: 60));
            AddColumn("dbo.ExitSurvey_Page1", "PresentHome", c => c.String(maxLength: 60));
            AddColumn("dbo.ExitSurvey_Page1", "PresentLife", c => c.String(maxLength: 60));
            DropColumn("dbo.ExitSurvey_Page1", "Ans");
            DropColumn("dbo.ExitSurvey_Page1", "Qn");
        }
    }
}
