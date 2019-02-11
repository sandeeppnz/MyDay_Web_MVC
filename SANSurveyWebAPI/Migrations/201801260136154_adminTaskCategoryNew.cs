namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adminTaskCategoryNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TaskCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Category = c.String(),
                        TaskType = c.String(),
                        IsDeleted = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TaskCategories");
        }
    }
}
