namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KidsFeedbackNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KidsFeedbacks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        KidsSurveyId = c.Int(nullable: false),
                        Comments = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KidsFeedbacks", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.KidsFeedbacks", new[] { "ProfileId" });
            DropTable("dbo.KidsFeedbacks");
        }
    }
}
