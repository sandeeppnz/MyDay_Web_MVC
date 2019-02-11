namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewCaseWorkersFeedback : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CaseWorkersFeedbacks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Comments = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CaseWorkersFeedbacks", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.CaseWorkersFeedbacks", new[] { "ProfileId" });
            DropTable("dbo.CaseWorkersFeedbacks");
        }
    }
}
