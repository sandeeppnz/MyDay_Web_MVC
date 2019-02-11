namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KidsTraveladdColnew : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KidsTravels", "OtherModeOfTransport", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.KidsTravels", "OtherModeOfTransport");
        }
    }
}
