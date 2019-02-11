namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class incentivefile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "Incentive", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "Incentive");
        }
    }
}
