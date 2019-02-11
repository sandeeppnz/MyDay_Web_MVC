namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editKidsTraveltbl : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.KidsTravels", "ModeOfTransport", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.KidsTravels", "ModeOfTransport", c => c.Int(nullable: false));
        }
    }
}
