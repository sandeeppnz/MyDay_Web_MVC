namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KidsEmoTrackNewColAddForOther : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KidsEmoStageTrackeds", "OtherLocationName", c => c.String());
            AddColumn("dbo.KidsEmoStageTrackeds", "OtherModeOfTransport", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.KidsEmoStageTrackeds", "OtherModeOfTransport");
            DropColumn("dbo.KidsEmoStageTrackeds", "OtherLocationName");
        }
    }
}
