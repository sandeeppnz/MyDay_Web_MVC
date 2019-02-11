namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class exitsurveypage13modified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExitSurvey_Page13", "Q1_Applicable", c => c.String());
            AddColumn("dbo.ExitSurvey_Page13", "Q1_Year", c => c.Int(nullable: false));
            AddColumn("dbo.ExitSurvey_Page13", "Q2_PTWork", c => c.String());
            AddColumn("dbo.ExitSurvey_Page13", "Q2_Other", c => c.String());
            AddColumn("dbo.ExitSurvey_Page13", "Q3_NoOfPeople", c => c.Int(nullable: false));
            AddColumn("dbo.ExitSurvey_Page13", "Q4_Martial", c => c.String());
            AddColumn("dbo.ExitSurvey_Page13", "Q5_PartnershipMarried", c => c.String());
            DropColumn("dbo.ExitSurvey_Page13", "Q1");
            DropColumn("dbo.ExitSurvey_Page13", "Q2");
            DropColumn("dbo.ExitSurvey_Page13", "Q3");
            DropColumn("dbo.ExitSurvey_Page13", "Q4");
            DropColumn("dbo.ExitSurvey_Page13", "Q5");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ExitSurvey_Page13", "Q5", c => c.String());
            AddColumn("dbo.ExitSurvey_Page13", "Q4", c => c.String());
            AddColumn("dbo.ExitSurvey_Page13", "Q3", c => c.String());
            AddColumn("dbo.ExitSurvey_Page13", "Q2", c => c.String());
            AddColumn("dbo.ExitSurvey_Page13", "Q1", c => c.String());
            DropColumn("dbo.ExitSurvey_Page13", "Q5_PartnershipMarried");
            DropColumn("dbo.ExitSurvey_Page13", "Q4_Martial");
            DropColumn("dbo.ExitSurvey_Page13", "Q3_NoOfPeople");
            DropColumn("dbo.ExitSurvey_Page13", "Q2_Other");
            DropColumn("dbo.ExitSurvey_Page13", "Q2_PTWork");
            DropColumn("dbo.ExitSurvey_Page13", "Q1_Year");
            DropColumn("dbo.ExitSurvey_Page13", "Q1_Applicable");
        }
    }
}
