namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WAMTasksNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WAMTasks",
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
            DropTable("dbo.WAMTasks");
        }
    }
}
