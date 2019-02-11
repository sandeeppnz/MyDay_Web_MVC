namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addColINEMoTrackNew : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KidsEmoStageTrackeds", "OtherTask", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.KidsEmoStageTrackeds", "OtherTask");
        }
    }
}
