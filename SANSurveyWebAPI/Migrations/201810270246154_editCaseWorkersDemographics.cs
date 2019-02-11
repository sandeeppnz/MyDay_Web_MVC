namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editCaseWorkersDemographics : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Demographics", "EthnicityOrRace", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Demographics", "EthnicityOrRace");
        }
    }
}
