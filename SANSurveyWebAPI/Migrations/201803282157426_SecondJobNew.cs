namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SecondJobNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SecondJobs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        ExitSurveyId = c.Int(nullable: false),
                        Q1 = c.String(),
                        Q2 = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
            AddColumn("dbo.Profiles", "MaxExitV2Step", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SecondJobs", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.SecondJobs", new[] { "ProfileId" });
            DropColumn("dbo.Profiles", "MaxExitV2Step");
            DropTable("dbo.SecondJobs");
        }
    }
}
