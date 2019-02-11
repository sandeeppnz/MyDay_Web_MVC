namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewFirstTrainingN : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FifthWorkEnvironments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        ExitSurveyId = c.Int(nullable: false),
                        Q1 = c.String(),
                        Q2 = c.String(),
                        Q3 = c.String(),
                        Q4 = c.String(),
                        Q5 = c.String(),
                        Q6 = c.String(),
                        Q7 = c.String(),
                        Q8 = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
            CreateTable(
                "dbo.FirstTrainings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        ExitSurveyId = c.Int(nullable: false),
                        QnId = c.Int(nullable: false),
                        OtherOption = c.String(),
                        Options = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FirstTrainings", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.FifthWorkEnvironments", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.FirstTrainings", new[] { "ProfileId" });
            DropIndex("dbo.FifthWorkEnvironments", new[] { "ProfileId" });
            DropTable("dbo.FirstTrainings");
            DropTable("dbo.FifthWorkEnvironments");
        }
    }
}
