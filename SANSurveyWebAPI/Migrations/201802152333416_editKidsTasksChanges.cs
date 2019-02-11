namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editKidsTasksChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KidsTasks", "InOutLocation", c => c.String());
            AddColumn("dbo.KidsTasks", "People", c => c.String());
            AlterColumn("dbo.KidsTasks", "StartTime", c => c.String());
            AlterColumn("dbo.KidsTasks", "EndTime", c => c.String());
            DropColumn("dbo.KidsTasks", "Guardian");
        }
        
        public override void Down()
        {
            AddColumn("dbo.KidsTasks", "Guardian", c => c.String());
            AlterColumn("dbo.KidsTasks", "EndTime", c => c.DateTime());
            AlterColumn("dbo.KidsTasks", "StartTime", c => c.DateTime());
            DropColumn("dbo.KidsTasks", "People");
            DropColumn("dbo.KidsTasks", "InOutLocation");
        }
    }
}
