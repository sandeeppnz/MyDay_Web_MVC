namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WAMProfileRoleNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WAMProfileRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        MyProfileRole = c.String(),
                        StartYear = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WAMProfileRoles");
        }
    }
}
