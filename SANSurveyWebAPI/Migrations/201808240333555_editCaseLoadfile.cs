namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editCaseLoadfile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CaseLoads", "Option", c => c.String());
            DropColumn("dbo.CaseLoads", "QnId");
            DropColumn("dbo.CaseLoads", "OtherOption");
            DropColumn("dbo.CaseLoads", "Options");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CaseLoads", "Options", c => c.String());
            AddColumn("dbo.CaseLoads", "OtherOption", c => c.String());
            AddColumn("dbo.CaseLoads", "QnId", c => c.Int(nullable: false));
            DropColumn("dbo.CaseLoads", "Option");
        }
    }
}
