namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewColaddKidsLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KidsLocations", "IsTasksEntered", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.KidsLocations", "IsTasksEntered");
        }
    }
}
