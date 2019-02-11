namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KidsEMoTrackTableAddNewColedit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KidsEmoStageTrackeds", "TravelDetails", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.KidsEmoStageTrackeds", "TravelDetails");
        }
    }
}
