namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editToProfilesforKids : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "MaxStepKidsSurvey", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "MaxStepKidsSurvey");
        }
    }
}
