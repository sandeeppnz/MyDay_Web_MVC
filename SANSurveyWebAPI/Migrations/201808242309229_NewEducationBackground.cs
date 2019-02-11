namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewEducationBackground : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EducationBackgrounds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        BachelorsDegree = c.String(),
                        MasterDegree = c.String(),
                        PreServiceTraining = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EducationBackgrounds");
        }
    }
}
