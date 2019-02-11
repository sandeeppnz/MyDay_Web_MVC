namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newWAMeditsinProfiles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "ClientName", c => c.String());
            AddColumn("dbo.Profiles", "ClientInitials", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "ClientInitials");
            DropColumn("dbo.Profiles", "ClientName");
        }
    }
}
