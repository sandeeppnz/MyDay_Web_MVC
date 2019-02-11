namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MyDayTaskListEditfields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MyDayTaskLists", "TaskName", c => c.String());
            AddColumn("dbo.MyDayTaskLists", "TaskCategoryId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MyDayTaskLists", "TaskCategoryId");
            DropColumn("dbo.MyDayTaskLists", "TaskName");
        }
    }
}
