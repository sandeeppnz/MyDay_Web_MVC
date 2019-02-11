namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class page11exitsurveymodified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExitSurvey_Page11", "QnId", c => c.Int(nullable: false));
            AddColumn("dbo.ExitSurvey_Page11", "OtherOption", c => c.String());
            AddColumn("dbo.ExitSurvey_Page11", "Options", c => c.String());
            DropColumn("dbo.ExitSurvey_Page11", "Q1_Option1");
            DropColumn("dbo.ExitSurvey_Page11", "Q1_Option2");
            DropColumn("dbo.ExitSurvey_Page11", "Q1_Option3");
            DropColumn("dbo.ExitSurvey_Page11", "Q1_Option4");
            DropColumn("dbo.ExitSurvey_Page11", "Q1_Option5");
            DropColumn("dbo.ExitSurvey_Page11", "Q1_Option6");
            DropColumn("dbo.ExitSurvey_Page11", "Q1_Option7");
            DropColumn("dbo.ExitSurvey_Page11", "Q1_Option8");
            DropColumn("dbo.ExitSurvey_Page11", "Q1_Option9");
            DropColumn("dbo.ExitSurvey_Page11", "Q2_Option1");
            DropColumn("dbo.ExitSurvey_Page11", "Q2_Option2");
            DropColumn("dbo.ExitSurvey_Page11", "Q2_Option3");
            DropColumn("dbo.ExitSurvey_Page11", "Q2_Option4");
            DropColumn("dbo.ExitSurvey_Page11", "Q2_Option5");
            DropColumn("dbo.ExitSurvey_Page11", "Q2_Option6");
            DropColumn("dbo.ExitSurvey_Page11", "Q2_Option7");
            DropColumn("dbo.ExitSurvey_Page11", "Q2_Option8");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ExitSurvey_Page11", "Q2_Option8", c => c.String());
            AddColumn("dbo.ExitSurvey_Page11", "Q2_Option7", c => c.String());
            AddColumn("dbo.ExitSurvey_Page11", "Q2_Option6", c => c.String());
            AddColumn("dbo.ExitSurvey_Page11", "Q2_Option5", c => c.String());
            AddColumn("dbo.ExitSurvey_Page11", "Q2_Option4", c => c.String());
            AddColumn("dbo.ExitSurvey_Page11", "Q2_Option3", c => c.String());
            AddColumn("dbo.ExitSurvey_Page11", "Q2_Option2", c => c.String());
            AddColumn("dbo.ExitSurvey_Page11", "Q2_Option1", c => c.String());
            AddColumn("dbo.ExitSurvey_Page11", "Q1_Option9", c => c.String());
            AddColumn("dbo.ExitSurvey_Page11", "Q1_Option8", c => c.String());
            AddColumn("dbo.ExitSurvey_Page11", "Q1_Option7", c => c.String());
            AddColumn("dbo.ExitSurvey_Page11", "Q1_Option6", c => c.String());
            AddColumn("dbo.ExitSurvey_Page11", "Q1_Option5", c => c.String());
            AddColumn("dbo.ExitSurvey_Page11", "Q1_Option4", c => c.String());
            AddColumn("dbo.ExitSurvey_Page11", "Q1_Option3", c => c.String());
            AddColumn("dbo.ExitSurvey_Page11", "Q1_Option2", c => c.String());
            AddColumn("dbo.ExitSurvey_Page11", "Q1_Option1", c => c.String());
            DropColumn("dbo.ExitSurvey_Page11", "Options");
            DropColumn("dbo.ExitSurvey_Page11", "OtherOption");
            DropColumn("dbo.ExitSurvey_Page11", "QnId");
        }
    }
}
