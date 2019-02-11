namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewCurrentWorkPlaceContd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CurrentWorkplaceContds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        WorkStatus = c.String(),
                        WorkPosition = c.String(),
                        WorkCountry = c.String(),
                        OtherWorkCountry = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CurrentWorkplaceContds");
        }
    }
}
