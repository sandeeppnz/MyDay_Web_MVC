namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniversityAttendedOtherText : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProfileDemographics", "UniversityAttendedOtherText", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProfileDemographics", "UniversityAttendedOtherText");
        }
    }
}
