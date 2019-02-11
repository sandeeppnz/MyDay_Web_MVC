namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class exitsurveypageone : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExitSurvey_Page1", "PresentLife", c => c.String(maxLength: 60));
            AddColumn("dbo.ExitSurvey_Page1", "PresentHome", c => c.String(maxLength: 60));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExitSurvey_Page1", "PresentHome");
            DropColumn("dbo.ExitSurvey_Page1", "PresentLife");
        }
    }
}
