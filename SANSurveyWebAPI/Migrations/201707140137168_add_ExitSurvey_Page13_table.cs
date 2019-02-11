namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_ExitSurvey_Page13_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExitSurvey_Page13",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Q1 = c.String(),
                        Q2 = c.String(),
                        Q3 = c.String(),
                        Q4 = c.String(),
                        Q5 = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ExitSurvey_Page13");
        }
    }
}
