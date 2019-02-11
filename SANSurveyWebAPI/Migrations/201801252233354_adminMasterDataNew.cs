namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adminMasterDataNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MasterDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecurrentSurveyTimeSlot = c.Int(nullable: false),
                        RecurrentSurveyTaskSelectionLimit = c.Int(nullable: false),
                        NoOfSurveyPerParticipant = c.Int(nullable: false),
                        TaskTypes = c.String(),
                        SurveyForUsers = c.String(),
                        SurveyTypes = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MasterDatas");
        }
    }
}
