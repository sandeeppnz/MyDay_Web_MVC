namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewKidsTravelDetails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KidsTravels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        KidsSurveyId = c.Int(nullable: false),
                        FromLocationId = c.Int(nullable: false),
                        ToLocationId = c.Int(nullable: false),
                        ModeOfTransport = c.Int(nullable: false),
                        TravelTimeInHours = c.Int(nullable: false),
                        TravelTimeInMins = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.KidsTravels");
        }
    }
}
