namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DisciplineOrRolesTableNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DisciplineOrRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Designation = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DisciplineOrRoles");
        }
    }
}
