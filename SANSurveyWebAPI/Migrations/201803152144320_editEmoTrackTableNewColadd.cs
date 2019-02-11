namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editEmoTrackTableNewColadd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KidsEmoStageTrackeds", "SequenceToQEmo", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.KidsEmoStageTrackeds", "SequenceToQEmo");
        }
    }
}
