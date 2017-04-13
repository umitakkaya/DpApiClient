namespace DpApiClient.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBInit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppSetting",
                c => new
                    {
                        SettingName = c.String(nullable: false, maxLength: 128),
                        SettingValue = c.String(),
                    })
                .PrimaryKey(t => t.SettingName);
            
            CreateTable(
                "dbo.BookingExtraFields",
                c => new
                    {
                        ForeignAddressId = c.String(nullable: false, maxLength: 128),
                        IsBirthDate = c.Boolean(nullable: false),
                        IsGender = c.Boolean(nullable: false),
                        IsNin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ForeignAddressId)
                .ForeignKey("dbo.ForeignAddress", t => t.ForeignAddressId, cascadeDelete: true)
                .Index(t => t.ForeignAddressId);
            
            CreateTable(
                "dbo.ForeignAddress",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Street = c.String(),
                        ForeignDoctorId = c.String(nullable: false, maxLength: 128),
                        ForeignFacilityId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForeignDoctor", t => t.ForeignDoctorId, cascadeDelete: true)
                .ForeignKey("dbo.ForeignFacility", t => t.ForeignFacilityId, cascadeDelete: true)
                .Index(t => t.ForeignDoctorId)
                .Index(t => t.ForeignFacilityId);
            
            CreateTable(
                "dbo.DoctorMapping",
                c => new
                    {
                        DoctorId = c.Int(nullable: false),
                        FacilityId = c.Int(nullable: false),
                        ForeignAddressId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.DoctorId, t.FacilityId })
                .ForeignKey("dbo.DoctorFacility", t => new { t.DoctorId, t.FacilityId })
                .ForeignKey("dbo.ForeignAddress", t => t.ForeignAddressId)
                .Index(t => new { t.DoctorId, t.FacilityId })
                .Index(t => t.ForeignAddressId);
            
            CreateTable(
                "dbo.DoctorFacility",
                c => new
                    {
                        DoctorId = c.Int(nullable: false),
                        FacilityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DoctorId, t.FacilityId })
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .ForeignKey("dbo.Facility", t => t.FacilityId, cascadeDelete: true)
                .Index(t => t.DoctorId)
                .Index(t => t.FacilityId);
            
            CreateTable(
                "dbo.Doctor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Specialization",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DoctorSchedule",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Duration = c.Int(nullable: false),
                        Start = c.Time(nullable: false, precision: 7),
                        End = c.Time(nullable: false, precision: 7),
                        IsFullfilled = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false, storeType: "date"),
                        DoctorId = c.Int(nullable: false),
                        FacilityId = c.Int(nullable: false),
                        ForeignDoctorServiceId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DoctorFacility", t => new { t.DoctorId, t.FacilityId }, cascadeDelete: true)
                .ForeignKey("dbo.ForeignDoctorService", t => t.ForeignDoctorServiceId)
                .Index(t => new { t.DoctorId, t.FacilityId })
                .Index(t => t.ForeignDoctorServiceId);
            
            CreateTable(
                "dbo.ForeignDoctorService",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Duration = c.Int(),
                        PriceMin = c.Double(),
                        PriceMax = c.Double(),
                        ServiceId = c.String(),
                        ForeignDoctorId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForeignDoctor", t => t.ForeignDoctorId, cascadeDelete: true)
                .Index(t => t.ForeignDoctorId);
            
            CreateTable(
                "dbo.ForeignDoctor",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Surname = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ForeignSpecialization",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ForeignDoctorId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => new { t.Id, t.ForeignDoctorId })
                .ForeignKey("dbo.ForeignDoctor", t => t.ForeignDoctorId, cascadeDelete: true)
                .Index(t => t.ForeignDoctorId);
            
            CreateTable(
                "dbo.Visit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartAt = c.DateTime(nullable: false),
                        EndAt = c.DateTime(nullable: false),
                        VisitStatus = c.Int(nullable: false),
                        DoctorId = c.Int(nullable: false),
                        FacilityId = c.Int(nullable: false),
                        DoctorScheduleId = c.Int(nullable: false),
                        ForeignVisitId = c.String(maxLength: 128, unicode: false),
                        ForeignDoctorService_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DoctorFacility", t => new { t.DoctorId, t.FacilityId })
                .ForeignKey("dbo.DoctorSchedule", t => t.DoctorScheduleId)
                .ForeignKey("dbo.ForeignDoctorService", t => t.ForeignDoctorService_Id)
                .Index(t => new { t.DoctorId, t.FacilityId })
                .Index(t => t.DoctorScheduleId)
                .Index(t => t.ForeignDoctorService_Id);
            
            CreateTable(
                "dbo.VisitPatient",
                c => new
                    {
                        VisitId = c.Int(nullable: false),
                        Name = c.String(),
                        Surname = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        NIN = c.String(),
                        Gender = c.Int(),
                        Birthdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.VisitId)
                .ForeignKey("dbo.Visit", t => t.VisitId, cascadeDelete: true)
                .Index(t => t.VisitId);
            
            CreateTable(
                "dbo.Facility",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ParentFacilityId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Facility", t => t.ParentFacilityId)
                .Index(t => t.ParentFacilityId);
            
            CreateTable(
                "dbo.ForeignFacility",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DoctorSpecialization",
                c => new
                    {
                        DoctorId = c.Int(nullable: false),
                        SpecializationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DoctorId, t.SpecializationId })
                .ForeignKey("dbo.Doctor", t => t.DoctorId, cascadeDelete: true)
                .ForeignKey("dbo.Specialization", t => t.SpecializationId, cascadeDelete: true)
                .Index(t => t.DoctorId)
                .Index(t => t.SpecializationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BookingExtraFields", "ForeignAddressId", "dbo.ForeignAddress");
            DropForeignKey("dbo.ForeignAddress", "ForeignFacilityId", "dbo.ForeignFacility");
            DropForeignKey("dbo.ForeignAddress", "ForeignDoctorId", "dbo.ForeignDoctor");
            DropForeignKey("dbo.DoctorMapping", "ForeignAddressId", "dbo.ForeignAddress");
            DropForeignKey("dbo.DoctorMapping", new[] { "DoctorId", "FacilityId" }, "dbo.DoctorFacility");
            DropForeignKey("dbo.DoctorFacility", "FacilityId", "dbo.Facility");
            DropForeignKey("dbo.Facility", "ParentFacilityId", "dbo.Facility");
            DropForeignKey("dbo.DoctorSchedule", "ForeignDoctorServiceId", "dbo.ForeignDoctorService");
            DropForeignKey("dbo.Visit", "ForeignDoctorService_Id", "dbo.ForeignDoctorService");
            DropForeignKey("dbo.VisitPatient", "VisitId", "dbo.Visit");
            DropForeignKey("dbo.Visit", "DoctorScheduleId", "dbo.DoctorSchedule");
            DropForeignKey("dbo.Visit", new[] { "DoctorId", "FacilityId" }, "dbo.DoctorFacility");
            DropForeignKey("dbo.ForeignDoctorService", "ForeignDoctorId", "dbo.ForeignDoctor");
            DropForeignKey("dbo.ForeignSpecialization", "ForeignDoctorId", "dbo.ForeignDoctor");
            DropForeignKey("dbo.DoctorSchedule", new[] { "DoctorId", "FacilityId" }, "dbo.DoctorFacility");
            DropForeignKey("dbo.DoctorFacility", "DoctorId", "dbo.Doctor");
            DropForeignKey("dbo.DoctorSpecialization", "SpecializationId", "dbo.Specialization");
            DropForeignKey("dbo.DoctorSpecialization", "DoctorId", "dbo.Doctor");
            DropIndex("dbo.DoctorSpecialization", new[] { "SpecializationId" });
            DropIndex("dbo.DoctorSpecialization", new[] { "DoctorId" });
            DropIndex("dbo.Facility", new[] { "ParentFacilityId" });
            DropIndex("dbo.VisitPatient", new[] { "VisitId" });
            DropIndex("dbo.Visit", new[] { "ForeignDoctorService_Id" });
            DropIndex("dbo.Visit", new[] { "DoctorScheduleId" });
            DropIndex("dbo.Visit", new[] { "DoctorId", "FacilityId" });
            DropIndex("dbo.ForeignSpecialization", new[] { "ForeignDoctorId" });
            DropIndex("dbo.ForeignDoctorService", new[] { "ForeignDoctorId" });
            DropIndex("dbo.DoctorSchedule", new[] { "ForeignDoctorServiceId" });
            DropIndex("dbo.DoctorSchedule", new[] { "DoctorId", "FacilityId" });
            DropIndex("dbo.DoctorFacility", new[] { "FacilityId" });
            DropIndex("dbo.DoctorFacility", new[] { "DoctorId" });
            DropIndex("dbo.DoctorMapping", new[] { "ForeignAddressId" });
            DropIndex("dbo.DoctorMapping", new[] { "DoctorId", "FacilityId" });
            DropIndex("dbo.ForeignAddress", new[] { "ForeignFacilityId" });
            DropIndex("dbo.ForeignAddress", new[] { "ForeignDoctorId" });
            DropIndex("dbo.BookingExtraFields", new[] { "ForeignAddressId" });
            DropTable("dbo.DoctorSpecialization");
            DropTable("dbo.ForeignFacility");
            DropTable("dbo.Facility");
            DropTable("dbo.VisitPatient");
            DropTable("dbo.Visit");
            DropTable("dbo.ForeignSpecialization");
            DropTable("dbo.ForeignDoctor");
            DropTable("dbo.ForeignDoctorService");
            DropTable("dbo.DoctorSchedule");
            DropTable("dbo.Specialization");
            DropTable("dbo.Doctor");
            DropTable("dbo.DoctorFacility");
            DropTable("dbo.DoctorMapping");
            DropTable("dbo.ForeignAddress");
            DropTable("dbo.BookingExtraFields");
            DropTable("dbo.AppSetting");
        }
    }
}
