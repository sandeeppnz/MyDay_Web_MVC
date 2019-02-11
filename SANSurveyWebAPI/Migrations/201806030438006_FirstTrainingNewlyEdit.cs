namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstTrainingNewlyEdit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FirstTrainings", "QId", c => c.Int(nullable: false));
            AddColumn("dbo.FirstTrainings", "Qn", c => c.String());
            AddColumn("dbo.FirstTrainings", "Ans", c => c.String());
            DropColumn("dbo.FirstTrainings", "QnId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FirstTrainings", "QnId", c => c.Int(nullable: false));
            DropColumn("dbo.FirstTrainings", "Ans");
            DropColumn("dbo.FirstTrainings", "Qn");
            DropColumn("dbo.FirstTrainings", "QId");
        }
    }
}
