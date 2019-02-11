namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WAMWellbeingNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WAMWellBeings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        SwbLife = c.String(maxLength: 60),
                        SwbHome = c.String(maxLength: 60),
                        SwbJob = c.String(maxLength: 60),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WAMWellBeings");
        }
    }
}
