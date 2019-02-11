namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mydayerrorlogcreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MyDayErrorLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        SurveyUID = c.String(),
                        AccessedDateTime = c.DateTime(nullable: false),
                        ErrorMessage = c.String(),
                        HtmlContent = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MyDayErrorLogs");
        }
    }
}
