namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editKidsLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KidsLocations", "LocationSequence", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.KidsLocations", "LocationSequence");
        }
    }
}
