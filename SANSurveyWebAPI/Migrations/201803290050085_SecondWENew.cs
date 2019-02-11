namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SecondWENew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SecondWorkEnvironments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        ExitSurveyId = c.Int(nullable: false),
                        Q1 = c.String(),
                        Q1Other = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SecondWorkEnvironments", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.SecondWorkEnvironments", new[] { "ProfileId" });
            DropTable("dbo.SecondWorkEnvironments");
        }
    }
}
