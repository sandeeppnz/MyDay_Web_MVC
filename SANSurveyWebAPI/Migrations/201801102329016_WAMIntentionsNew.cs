namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WAMIntentionsNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WAMIntentions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        SameEmployer = c.Decimal(precision: 18, scale: 2),
                        SameIndustry = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WAMIntentions");
        }
    }
}
