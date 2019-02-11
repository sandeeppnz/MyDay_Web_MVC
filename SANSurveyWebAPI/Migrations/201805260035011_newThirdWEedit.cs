namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newThirdWEedit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ThirdWorkEnvironments", "Q1", c => c.String());
            AddColumn("dbo.ThirdWorkEnvironments", "Q1Other", c => c.String());
            DropColumn("dbo.ThirdWorkEnvironments", "Qn");
            DropColumn("dbo.ThirdWorkEnvironments", "Ans");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ThirdWorkEnvironments", "Ans", c => c.String(maxLength: 60));
            AddColumn("dbo.ThirdWorkEnvironments", "Qn", c => c.String());
            DropColumn("dbo.ThirdWorkEnvironments", "Q1Other");
            DropColumn("dbo.ThirdWorkEnvironments", "Q1");
        }
    }
}
