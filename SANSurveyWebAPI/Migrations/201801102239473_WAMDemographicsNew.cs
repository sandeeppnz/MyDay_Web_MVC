namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WAMDemographicsNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WAMDemographics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Gender = c.String(maxLength: 10),
                        BirthYear = c.String(maxLength: 60),
                        IsCaregiverChild = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WAMDemographics");
        }
    }
}
