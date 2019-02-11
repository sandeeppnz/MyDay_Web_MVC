namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allNewTrainings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AboutYouES",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        ExitSurveyId = c.Int(nullable: false),
                        Q1_Applicable = c.String(),
                        Q1_Year = c.Int(nullable: false),
                        Q2_PTWork = c.String(),
                        Q2_Other = c.String(),
                        Q3_NoOfPeople = c.Int(nullable: false),
                        Q4_Martial = c.String(),
                        Q5_PartnershipMarried = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
            CreateTable(
                "dbo.SecondTrainings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        ExitSurveyId = c.Int(nullable: false),
                        Qn = c.String(),
                        Ans = c.String(maxLength: 60),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
            CreateTable(
                "dbo.ThirdTrainings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        ExitSurveyId = c.Int(nullable: false),
                        Qn = c.String(),
                        Ans = c.String(maxLength: 60),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ThirdTrainings", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.SecondTrainings", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.AboutYouES", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.ThirdTrainings", new[] { "ProfileId" });
            DropIndex("dbo.SecondTrainings", new[] { "ProfileId" });
            DropIndex("dbo.AboutYouES", new[] { "ProfileId" });
            DropTable("dbo.ThirdTrainings");
            DropTable("dbo.SecondTrainings");
            DropTable("dbo.AboutYouES");
        }
    }
}
