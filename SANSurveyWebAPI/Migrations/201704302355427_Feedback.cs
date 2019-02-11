namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Feedback : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Feedbacks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedDateTimeUtc = c.DateTime(nullable: false),
                        Channel = c.String(maxLength: 50),
                        ProfileId = c.Int(),
                        Message = c.String(),
                        PreferedContact = c.String(maxLength: 50),
                        PreferedTime = c.String(maxLength: 50),
                        Email = c.String(maxLength: 256),
                        ContactNumber = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Feedbacks");
        }
    }
}
