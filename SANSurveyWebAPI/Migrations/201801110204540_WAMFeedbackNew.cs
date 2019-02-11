namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WAMFeedbackNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WAMFeedbacks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Feedback = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WAMFeedbacks");
        }
    }
}
