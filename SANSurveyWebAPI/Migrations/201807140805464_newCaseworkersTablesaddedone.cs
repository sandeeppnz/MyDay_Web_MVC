namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newCaseworkersTablesaddedone : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CaseLoads",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        QnId = c.Int(nullable: false),
                        OtherOption = c.String(),
                        Options = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Demographics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Gender = c.String(maxLength: 10),
                        BirthYear = c.String(maxLength: 60),
                        IsCaregiverChild = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TimeAllocations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        ClinicalActualTime = c.Decimal(precision: 18, scale: 2),
                        ResearchActualTime = c.Decimal(precision: 18, scale: 2),
                        TeachingLearningActualTime = c.Decimal(precision: 18, scale: 2),
                        AdminActualTime = c.Decimal(precision: 18, scale: 2),
                        ClinicalDesiredTime = c.Decimal(precision: 18, scale: 2),
                        ResearchDesiredTime = c.Decimal(precision: 18, scale: 2),
                        TeachingLearningDesiredTime = c.Decimal(precision: 18, scale: 2),
                        AdminDesiredTime = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TimeAllocations");
            DropTable("dbo.Demographics");
            DropTable("dbo.CaseLoads");
        }
    }
}
