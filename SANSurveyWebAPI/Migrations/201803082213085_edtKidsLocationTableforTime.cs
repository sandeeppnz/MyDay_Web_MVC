namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edtKidsLocationTableforTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KidsLocations", "StartedAt", c => c.String());
            AddColumn("dbo.KidsLocations", "EndedAt", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.KidsLocations", "EndedAt");
            DropColumn("dbo.KidsLocations", "StartedAt");
        }
    }
}
