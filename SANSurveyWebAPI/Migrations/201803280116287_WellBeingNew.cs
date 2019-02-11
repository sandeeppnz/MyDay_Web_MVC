namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WellBeingNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WellBeings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Qn = c.String(),
                        Ans = c.String(maxLength: 60),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WellBeings", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.WellBeings", new[] { "ProfileId" });
            DropTable("dbo.WellBeings");
        }
    }
}
