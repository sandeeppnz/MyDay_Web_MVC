namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adminTaskCategoryEdit : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TaskCategories", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TaskCategories", "IsDeleted", c => c.String());
        }
    }
}
