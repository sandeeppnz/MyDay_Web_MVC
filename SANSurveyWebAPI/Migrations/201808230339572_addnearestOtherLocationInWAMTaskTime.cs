namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnearestOtherLocationInWAMTaskTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WAMWithWhomTaskTimes", "NearestOtherLocation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.WAMWithWhomTaskTimes", "NearestOtherLocation");
        }
    }
}
