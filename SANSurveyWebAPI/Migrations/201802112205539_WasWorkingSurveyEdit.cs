namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WasWorkingSurveyEdit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Surveys", "WasWorking", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Surveys", "WasWorking");
        }
    }
}
