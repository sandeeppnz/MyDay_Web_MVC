namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editinWAMTaskTimeLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WAMWithWhomTaskTimes", "NearestLocation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.WAMWithWhomTaskTimes", "NearestLocation");
        }
    }
}
