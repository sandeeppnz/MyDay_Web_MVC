namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editKidsResponseTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.KidsResponses", "QuestionId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.KidsResponses", "QuestionId", c => c.Int(nullable: false));
        }
    }
}
