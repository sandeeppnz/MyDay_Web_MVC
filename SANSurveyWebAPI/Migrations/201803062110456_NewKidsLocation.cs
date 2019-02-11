namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewKidsLocation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KidsLocations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        KidsSurveyId = c.Int(nullable: false),
                        Location = c.String(),
                        OtherLocation = c.String(),
                        TimeSpentInHours = c.Int(nullable: false),
                        TimeSpentInMins = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.KidsSurveys", t => t.KidsSurveyId, cascadeDelete: true)
                .Index(t => t.KidsSurveyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KidsLocations", "KidsSurveyId", "dbo.KidsSurveys");
            DropIndex("dbo.KidsLocations", new[] { "KidsSurveyId" });
            DropTable("dbo.KidsLocations");
        }
    }
}
