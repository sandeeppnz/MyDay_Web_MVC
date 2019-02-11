namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_ExitSurveyProgressNext_field : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "ExitSurveyProgressNext", c => c.String(maxLength: 60));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "ExitSurveyProgressNext");
        }
    }
}
