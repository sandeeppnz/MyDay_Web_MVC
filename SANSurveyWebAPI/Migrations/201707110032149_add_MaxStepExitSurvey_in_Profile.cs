namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_MaxStepExitSurvey_in_Profile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "MaxStepExitSurvey", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "MaxStepExitSurvey");
        }
    }
}
