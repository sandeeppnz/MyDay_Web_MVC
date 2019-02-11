namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MyDayResponseAffectNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ResponseAffects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SurveyId = c.Int(nullable: false),
                        ProfileId = c.Int(nullable: false),
                        TaskId = c.Int(nullable: false),
                        TaskOther = c.String(maxLength: 255),
                        StartResponseDateTimeUtc = c.DateTime(),
                        EndResponseDateTimeUtc = c.DateTime(),
                        ShiftStartDateTime = c.DateTime(),
                        ShiftEndDateTime = c.DateTime(),
                        TaskStartDateTime = c.DateTime(),
                        TaskEndDateTime = c.DateTime(),
                        Question = c.String(maxLength: 10),
                        Answer = c.String(maxLength: 255),
                        SurveyWindowStartDateTime = c.DateTime(),
                        SurveyWindowEndDateTime = c.DateTime(),
                        IsOtherTask = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.MyDayTaskLists", "IsAffectStageCompleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MyDayTaskLists", "IsAffectStageCompleted");
            DropTable("dbo.ResponseAffects");
        }
    }
}
