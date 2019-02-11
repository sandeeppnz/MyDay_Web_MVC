namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EDITkIDSrEACTIONnEWcOLaDD : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KidsReactions", "KidsEmoTrackId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.KidsReactions", "KidsEmoTrackId");
        }
    }
}
