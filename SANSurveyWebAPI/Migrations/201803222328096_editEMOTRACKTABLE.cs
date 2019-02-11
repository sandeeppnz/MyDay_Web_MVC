namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editEMOTRACKTABLE : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KidsTasksOnLocations", "IsEmoStageCompleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.KidsTasksOnLocations", "IsEmoStageCompleted");
        }
    }
}
