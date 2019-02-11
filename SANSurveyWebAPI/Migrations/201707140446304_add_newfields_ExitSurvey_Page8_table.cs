namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_newfields_ExitSurvey_Page8_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExitSurvey_Page8", "Q3", c => c.String());
            AddColumn("dbo.ExitSurvey_Page8", "Q4", c => c.String());
            AddColumn("dbo.ExitSurvey_Page8", "Q5", c => c.String());
            AddColumn("dbo.ExitSurvey_Page8", "Q6", c => c.String());
            AddColumn("dbo.ExitSurvey_Page8", "Q7", c => c.String());
            AddColumn("dbo.ExitSurvey_Page8", "Q8", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExitSurvey_Page8", "Q8");
            DropColumn("dbo.ExitSurvey_Page8", "Q7");
            DropColumn("dbo.ExitSurvey_Page8", "Q6");
            DropColumn("dbo.ExitSurvey_Page8", "Q5");
            DropColumn("dbo.ExitSurvey_Page8", "Q4");
            DropColumn("dbo.ExitSurvey_Page8", "Q3");
        }
    }
}
