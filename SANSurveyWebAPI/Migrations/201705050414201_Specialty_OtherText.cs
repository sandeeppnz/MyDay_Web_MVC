namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Specialty_OtherText : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProfileSpecialties", "OtherText", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProfileSpecialties", "OtherText");
        }
    }
}
