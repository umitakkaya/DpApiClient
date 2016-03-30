namespace DpApiClient.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultForeignDoctorService : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DoctorMapping", "ForeignDoctorServiceId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.DoctorMapping", "ForeignDoctorServiceId");
            AddForeignKey("dbo.DoctorMapping", "ForeignDoctorServiceId", "dbo.ForeignDoctorService", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DoctorMapping", "ForeignDoctorServiceId", "dbo.ForeignDoctorService");
            DropIndex("dbo.DoctorMapping", new[] { "ForeignDoctorServiceId" });
            DropColumn("dbo.DoctorMapping", "ForeignDoctorServiceId");
        }
    }
}
