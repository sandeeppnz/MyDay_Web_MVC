namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MyDayMultipleTaskNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MyDayTaskLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        SurveyId = c.Int(nullable: false),
                        TaskId = c.Int(nullable: false),
                        TaskStartDateTimeUtc = c.DateTime(),
                        TaskEndDateTimeUtc = c.DateTime(),
                        TaskStartDateCurrentTime = c.DateTime(),
                        TaskEndDateCurrentTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MyDayTaskLists");
        }
    }
}
