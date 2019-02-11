namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewJobIntentions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JobIntentions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        CurrentWorkplace = c.String(maxLength: 60),
                        CurrentIndustry = c.String(maxLength: 60),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.JobIntentions");
        }
    }
}
