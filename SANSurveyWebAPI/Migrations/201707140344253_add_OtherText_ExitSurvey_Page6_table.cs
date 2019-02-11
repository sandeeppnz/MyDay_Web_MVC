namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_OtherText_ExitSurvey_Page6_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExitSurvey_Page6", "Q1Other", c => c.String());
            AddColumn("dbo.ExitSurvey_Page6", "Q2Other", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExitSurvey_Page6", "Q2Other");
            DropColumn("dbo.ExitSurvey_Page6", "Q1Other");
        }
    }
}
