namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editKidsTravelNewColadd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KidsTravels", "TravelStartedAt", c => c.String());
            AddColumn("dbo.KidsTravels", "TravelEndedAt", c => c.String());
            AddColumn("dbo.KidsTravels", "TravelSequence", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.KidsTravels", "TravelSequence");
            DropColumn("dbo.KidsTravels", "TravelEndedAt");
            DropColumn("dbo.KidsTravels", "TravelStartedAt");
        }
    }
}
