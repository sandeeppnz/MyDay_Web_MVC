namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class exitsurveypagecontinued5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExitSurvey_PageContinued5",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Qn = c.String(),
                        Ans = c.String(maxLength: 60),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ExitSurvey_PageContinued5");
        }
    }
}
