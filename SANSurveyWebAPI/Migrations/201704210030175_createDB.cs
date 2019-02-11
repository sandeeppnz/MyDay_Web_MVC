namespace SANSurveyWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BirthYears",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Sequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Ethinicities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Sequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PageStats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PageName = c.String(maxLength: 100),
                        PageType = c.String(maxLength: 20),
                        PageAction = c.String(maxLength: 20),
                        TaskStartDateTime = c.DateTime(),
                        ProfileId = c.Int(nullable: false),
                        SurveyId = c.Int(),
                        TaskId = c.Int(),
                        WholePageIndicator = c.Boolean(),
                        PageDateTimeUtc = c.DateTime(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProfileContracts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        ContractType = c.String(maxLength: 20),
                        WorkingType = c.String(maxLength: 20),
                        HoursWorkedLastMonth = c.String(maxLength: 60),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProfileDemographics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        Gender = c.String(maxLength: 10),
                        MaritialStatus = c.String(maxLength: 60),
                        BirthYear = c.String(maxLength: 10),
                        IsCaregiverChild = c.String(maxLength: 10),
                        IsCaregiverAdult = c.String(maxLength: 10),
                        IsUniversityBritish = c.String(maxLength: 10),
                        UniversityAttended = c.String(maxLength: 60),
                        IsLeadership = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProfileEthinicities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        EthinicityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ethinicities", t => t.EthinicityId, cascadeDelete: true)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.EthinicityId);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedDateTimeUtc = c.DateTime(nullable: false),
                        RegisteredDateTimeUtc = c.DateTime(),
                        LastUpdatedDateTimeUtc = c.DateTime(),
                        UserId = c.String(maxLength: 128),
                        RegistrationProgressNext = c.String(maxLength: 60),
                        Uid = c.String(),
                        MaxStep = c.Int(nullable: false),
                        OffSetFromUTC = c.Int(nullable: false),
                        RegistrationEmailJobId = c.Int(),
                        RegistrationSmsJobId = c.Int(),
                        CurrentLevelOfTraining = c.String(maxLength: 60),
                        IsCurrentPlacement = c.String(maxLength: 10),
                        ProfileTaskType = c.String(maxLength: 20),
                        Name = c.String(),
                        MobileNumber = c.String(maxLength: 20),
                        LoginEmail = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ProfileTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        TaskItemId = c.Int(nullable: false),
                        Frequency = c.String(maxLength: 10),
                        CreatedDateTimeUtc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .ForeignKey("dbo.TaskItems", t => t.TaskItemId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.TaskItemId);
            
            CreateTable(
                "dbo.TaskItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShortName = c.String(maxLength: 100),
                        LongName = c.String(maxLength: 255),
                        Type = c.String(maxLength: 20),
                        Sequence = c.Int(nullable: false),
                        WardRoundIndicator = c.Boolean(),
                        OtherTaskIndicator = c.Boolean(),
                        IsWardRoundTask = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.ProfilePlacements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        PlacementStartYear = c.String(maxLength: 10),
                        PlacementStartMonth = c.String(maxLength: 20),
                        PlacementIsInHospital = c.String(maxLength: 10),
                        PlacementHospitalName = c.String(maxLength: 100),
                        PlacementHospitalNameOther = c.String(),
                        PlacementHospitalStartMonth = c.String(maxLength: 20),
                        PlacementHospitalStartYear = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProfileRosters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        IsAllDay = c.Boolean(nullable: false),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        StartUtc = c.DateTime(nullable: false),
                        EndUtc = c.DateTime(nullable: false),
                        RecurrenceRule = c.String(),
                        RecurrenceID = c.Int(),
                        RecurrenceException = c.String(),
                        ProfileId = c.Int(nullable: false),
                        Description = c.String(),
                        StartTimezone = c.String(),
                        EndTimezone = c.String(),
                        ShiftReminderEmailJobId = c.Int(),
                        ShiftReminderSmsJobId = c.Int(),
                        CreateSurveyJobId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProfileSpecialties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        SpecialtyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId, cascadeDelete: true)
                .ForeignKey("dbo.Specialties", t => t.SpecialtyId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.SpecialtyId);
            
            CreateTable(
                "dbo.Specialties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Sequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProfileTaskTimes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        ClinicalActualTime = c.Decimal(precision: 18, scale: 2),
                        ResearchActualTime = c.Decimal(precision: 18, scale: 2),
                        TeachingLearningActualTime = c.Decimal(precision: 18, scale: 2),
                        AdminActualTime = c.Decimal(precision: 18, scale: 2),
                        ClinicalDesiredTime = c.Decimal(precision: 18, scale: 2),
                        ResearchDesiredTime = c.Decimal(precision: 18, scale: 2),
                        TeachingLearningDesiredTime = c.Decimal(precision: 18, scale: 2),
                        AdminDesiredTime = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProfileTrainings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        TrainingStartYear = c.String(maxLength: 10),
                        IsTrainingBreak = c.String(maxLength: 10),
                        TrainingBreakLengthMonths = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProfileWellbeings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.Int(nullable: false),
                        SwbLife = c.String(maxLength: 60),
                        SwbHome = c.String(maxLength: 60),
                        SwbJob = c.String(maxLength: 60),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Responses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SurveyId = c.Int(nullable: false),
                        ProfileId = c.Int(nullable: false),
                        TaskId = c.Int(nullable: false),
                        TaskOther = c.String(maxLength: 255),
                        PageStatId = c.Int(),
                        StartResponseDateTimeUtc = c.DateTime(),
                        EndResponseDateTimeUtc = c.DateTime(),
                        ShiftStartDateTime = c.DateTime(),
                        ShiftEndDateTime = c.DateTime(),
                        TaskStartDateTime = c.DateTime(),
                        TaskEndDateTime = c.DateTime(),
                        Question = c.String(maxLength: 10),
                        Answer = c.String(maxLength: 255),
                        SurveyWindowStartDateTime = c.DateTime(),
                        SurveyWindowEndDateTime = c.DateTime(),
                        IsOtherTask = c.Boolean(),
                        IsWardRoundTask = c.Boolean(),
                        WardRoundTaskId = c.Int(),
                        WardRoundWindowStartDateTime = c.DateTime(),
                        WardRoundWindowEndDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Surveys",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Uid = c.String(),
                        ProfileId = c.Int(),
                        RosterItemId = c.Int(),
                        CreatedDateTimeUtc = c.DateTime(nullable: false),
                        SurveyUserStartDateTimeUtc = c.DateTime(),
                        SurveyUserCompletedDateTimeUtc = c.DateTime(),
                        SurveyProgressNext = c.String(maxLength: 20),
                        MaxStep = c.Int(nullable: false),
                        Status = c.String(),
                        SurveyWindowStartDateTime = c.DateTime(),
                        SurveyWindowStartDateTimeUtc = c.DateTime(),
                        SurveyWindowEndDateTime = c.DateTime(),
                        SurveyWindowEndDateTimeUtc = c.DateTime(),
                        SurveyExpiryDateTime = c.DateTime(),
                        SurveyExpiryDateTimeUtc = c.DateTime(),
                        SysGenRandomNumber = c.Int(),
                        SurveyDescription = c.String(),
                        CurrTaskStartTime = c.DateTime(),
                        CurrTaskEndTime = c.DateTime(),
                        NextTaskStartTime = c.DateTime(),
                        RemainingDuration = c.Int(nullable: false),
                        FirstQuestion = c.Boolean(nullable: false),
                        CurrTask = c.Int(nullable: false),
                        AddTaskId = c.Int(),
                        WRCurrTaskId = c.Int(),
                        WRCurrTasksId = c.String(),
                        WRRemainingDuration = c.Int(),
                        WRCurrTaskStartTime = c.DateTime(),
                        WRCurrTaskEndTime = c.DateTime(),
                        WRNextTaskStartTime = c.DateTime(),
                        WRCurrWindowEndTime = c.DateTime(),
                        WRCurrWindowStartTime = c.DateTime(),
                        SendSurveyEmailJobId = c.Int(),
                        SendSurveySmsJobId = c.Int(),
                        CompleteSurveyReminderEmailJobId = c.Int(),
                        CompleteSurveyReminderSmsJobId = c.Int(),
                        ExpiringSoonNotStartedSurveyReminderEmailJobId = c.Int(),
                        ExpiringSoonNotStartedSurveyReminderSmsJobId = c.Int(),
                        ExpiringSoonNotCompeletedSurveyReminderEmailJobId = c.Int(),
                        ExpiringSoonNotCompeletedSurveyReminderSmsJobId = c.Int(),
                        IsOnCall = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId)
                .ForeignKey("dbo.ProfileRosters", t => t.RosterItemId)
                .Index(t => t.ProfileId)
                .Index(t => t.RosterItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Surveys", "RosterItemId", "dbo.ProfileRosters");
            DropForeignKey("dbo.Surveys", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ProfileSpecialties", "SpecialtyId", "dbo.Specialties");
            DropForeignKey("dbo.ProfileSpecialties", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.ProfileEthinicities", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Profiles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProfileTasks", "TaskItemId", "dbo.TaskItems");
            DropForeignKey("dbo.ProfileTasks", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.ProfileEthinicities", "EthinicityId", "dbo.Ethinicities");
            DropIndex("dbo.Surveys", new[] { "RosterItemId" });
            DropIndex("dbo.Surveys", new[] { "ProfileId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ProfileSpecialties", new[] { "SpecialtyId" });
            DropIndex("dbo.ProfileSpecialties", new[] { "ProfileId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.ProfileTasks", new[] { "TaskItemId" });
            DropIndex("dbo.ProfileTasks", new[] { "ProfileId" });
            DropIndex("dbo.Profiles", new[] { "UserId" });
            DropIndex("dbo.ProfileEthinicities", new[] { "EthinicityId" });
            DropIndex("dbo.ProfileEthinicities", new[] { "ProfileId" });
            DropTable("dbo.Surveys");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Responses");
            DropTable("dbo.ProfileWellbeings");
            DropTable("dbo.ProfileTrainings");
            DropTable("dbo.ProfileTaskTimes");
            DropTable("dbo.Specialties");
            DropTable("dbo.ProfileSpecialties");
            DropTable("dbo.ProfileRosters");
            DropTable("dbo.ProfilePlacements");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.TaskItems");
            DropTable("dbo.ProfileTasks");
            DropTable("dbo.Profiles");
            DropTable("dbo.ProfileEthinicities");
            DropTable("dbo.ProfileDemographics");
            DropTable("dbo.ProfileContracts");
            DropTable("dbo.PageStats");
            DropTable("dbo.Ethinicities");
            DropTable("dbo.BirthYears");
        }
    }
}
