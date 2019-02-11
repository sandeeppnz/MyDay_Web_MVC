namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newCurrentWorkplace : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CurrentWorkplaces",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        OptionId = c.Int(nullable: false),
                        OptionValue = c.String(),
                        Ans = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CurrentWorkplaces");
        }
    }
}
