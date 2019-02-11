namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KidsTasksOnLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KidsTasksOnLocations", "OtherLocationName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.KidsTasksOnLocations", "OtherLocationName");
        }
    }
}
