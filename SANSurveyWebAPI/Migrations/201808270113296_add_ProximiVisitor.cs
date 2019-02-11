namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_ProximiVisitor : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProximiVisitors",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        VisitorId = c.String(),
                        CreatedDate = c.String(),
                        TimeZone = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProximiVisitors");
        }
    }
}
