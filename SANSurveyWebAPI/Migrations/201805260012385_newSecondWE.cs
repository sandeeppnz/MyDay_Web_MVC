namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newSecondWE : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SecondWorkEnvironments", "Qn", c => c.String());
            AddColumn("dbo.SecondWorkEnvironments", "Ans", c => c.String());
            DropColumn("dbo.SecondWorkEnvironments", "Q1");
            DropColumn("dbo.SecondWorkEnvironments", "Q1Other");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SecondWorkEnvironments", "Q1Other", c => c.String());
            AddColumn("dbo.SecondWorkEnvironments", "Q1", c => c.String());
            DropColumn("dbo.SecondWorkEnvironments", "Ans");
            DropColumn("dbo.SecondWorkEnvironments", "Qn");
        }
    }
}
