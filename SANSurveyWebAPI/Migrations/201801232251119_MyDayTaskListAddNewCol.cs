namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MyDayTaskListAddNewCol : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MyDayTaskLists", "TaskDuration", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MyDayTaskLists", "TaskDuration");
        }
    }
}
