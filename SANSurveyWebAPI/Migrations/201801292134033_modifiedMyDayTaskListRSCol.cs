namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifiedMyDayTaskListRSCol : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MyDayTaskLists", "IsRandomlySelected", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MyDayTaskLists", "IsRandomlySelected");
        }
    }
}
