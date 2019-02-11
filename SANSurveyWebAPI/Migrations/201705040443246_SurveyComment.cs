namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SurveyComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Surveys", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Surveys", "Comment");
        }
    }
}
