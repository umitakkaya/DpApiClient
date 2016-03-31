using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DpApiClient.Data
{
    public class HospitalContext : DbContext
    {

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<DoctorFacility> DoctorFacilities { get; set; }
        public DbSet<Specialization> Specializations { get; set; }

        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<VisitPatient> VisitPatients { get; set; }

        public DbSet<AppSetting> AppSettings { get; set; }

        //Bridge between the hospital system and 3rd party (Docplanner)
        public DbSet<DoctorMapping> DoctorMappings { get; set; }

        //3rd party data
        public DbSet<ForeignAddress> ForeignAddresses { get; set; }
        public DbSet<ForeignDoctor> ForeignDoctors { get; set; }
        public DbSet<ForeignDoctorService> ForeignDoctorServices { get; set; }
        public DbSet<ForeignFacility> ForeignFacilities { get; set; }
        public DbSet<ForeignSpecialization> ForeignSpecializations { get; set; }
        public DbSet<BookingExtraFields> BookingExtraFields { get; set; }



        public HospitalContext() : base("name=DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            ConfigureRelations(modelBuilder);
            ConfigureProperties(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigureProperties(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ForeignFacility>()
                .Property(f => f.Name)
                .IsRequired();

            modelBuilder.Entity<ForeignDoctor>()
                .Property(d => d.Name)
                .IsRequired();

            modelBuilder.Entity<ForeignAddress>()
                .Property(fa => fa.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<ForeignDoctor>()
                .Property(fd => fd.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<ForeignFacility>()
                .Property(ff => ff.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<ForeignDoctorService>()
                .Property(fds => fds.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<ForeignAddress>()
                .Property(d => d.Name)
                .IsRequired();

            modelBuilder.Entity<ForeignAddress>()
                .Property(d => d.ForeignDoctorId)
                .IsRequired();

            modelBuilder.Entity<ForeignAddress>()
                .Property(d => d.ForeignDoctorId)
                .IsRequired();


            modelBuilder.Entity<Facility>()
                .Property(f => f.Name)
                .IsRequired();

            modelBuilder.Entity<Doctor>()
                .Property(d => d.Name)
                .IsRequired();


            modelBuilder.Entity<DoctorSchedule>()
                .Property(ds => ds.Date)
                .HasColumnType("Date");

            modelBuilder.Entity<Visit>()
                .Property(v => v.ForeignVisitId)
                .HasMaxLength(128)
                .IsUnicode(false);

        }

        private static void ConfigureRelations(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppSetting>()
                .HasKey(s => s.SettingName);

            modelBuilder.Entity<ForeignAddress>()
                .HasKey(f => f.Id);


            modelBuilder.Entity<DoctorFacility>()
                .HasKey(df => new { df.DoctorId, df.FacilityId });

            modelBuilder.Entity<BookingExtraFields>()
                .HasKey(f => f.ForeignAddressId)
                .HasRequired(f => f.ForeignAddress)
                .WithRequiredDependent(f => f.BookingExtraFields)
                .WillCascadeOnDelete();

            modelBuilder.Entity<DoctorMapping>()
                .HasKey(dm => new { dm.DoctorId, dm.FacilityId });

            modelBuilder.Entity<DoctorMapping>()
                .HasRequired(dm => dm.DoctorFacility)
                .WithOptional(df => df.DoctorMapping)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DoctorMapping>()
                .HasRequired(dm => dm.ForeignAddress)
                .WithOptional(fa => fa.DoctorMapping)
                .Map(m => m.MapKey("ForeignAddressId"))
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DoctorMapping>()
               .HasRequired(dm => dm.ForeignDoctorService)
               .WithMany(fds => fds.DoctorMappings)
               .HasForeignKey(dm => dm.ForeignDoctorServiceId)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<ForeignDoctorService>()
                .HasMany(fds => fds.DoctorMappings)
                .WithRequired(dm => dm.ForeignDoctorService)
                .HasForeignKey(dm => dm.ForeignDoctorServiceId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Facility>()
                .HasMany(f => f.Branches)
                .WithOptional(f => f.ParentFacility)
                .HasForeignKey(f => f.ParentFacilityId);

            modelBuilder.Entity<DoctorFacility>()
                .HasRequired(df => df.Doctor)
                .WithMany(d => d.DoctorFacilities)
                .HasForeignKey(df => df.DoctorId);

            modelBuilder.Entity<DoctorFacility>()
                .HasRequired(df => df.Facility)
                .WithMany(d => d.DoctorFacilities)
                .HasForeignKey(df => df.FacilityId);

            modelBuilder.Entity<VisitPatient>()
                .HasKey(vp => vp.VisitId)
                .HasRequired(vp => vp.Visit)
                .WithRequiredDependent(v => v.VisitPatient)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Visit>()
                .HasRequired(v => v.DoctorFacility)
                .WithMany(df => df.Visits)
                .HasForeignKey(v => new { v.DoctorId, v.FacilityId })
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Visit>()
                .HasRequired(v => v.DoctorSchedule)
                .WithMany(ds => ds.Visits)
                .HasForeignKey(v => v.DoctorScheduleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ForeignDoctorService>()
                .HasRequired(fds => fds.ForeignDoctor)
                .WithMany(fd => fd.ForeignDoctorServices)
                .HasForeignKey(fds => fds.ForeignDoctorId);

            modelBuilder.Entity<ForeignAddress>()
                .HasRequired(fa => fa.ForeignDoctor)
                .WithMany(fd => fd.ForeignAddresses)
                .HasForeignKey(fa => fa.ForeignDoctorId);

            modelBuilder.Entity<ForeignAddress>()
                .HasRequired(fa => fa.ForeignFacility)
                .WithMany(ff => ff.ForeignAddresses)
                .HasForeignKey(fa => fa.ForeignFacilityId);

            modelBuilder.Entity<DoctorSchedule>()
                .HasOptional(ds => ds.ForeignDoctorService)
                .WithMany(fds => fds.DoctorSchedules)
                .HasForeignKey(ds => ds.ForeignDoctorServiceId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DoctorSchedule>()
                .HasRequired(ds => ds.DoctorFacility)
                .WithMany(df => df.DoctorSchedules)
                .HasForeignKey(ds => new { ds.DoctorId, ds.FacilityId });

            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Specializations)
                .WithMany(s => s.Doctors)
                .Map(ds =>
                {
                    ds.MapLeftKey("DoctorId");
                    ds.MapRightKey("SpecializationId");
                    ds.ToTable("DoctorSpecialization");
                });

            modelBuilder.Entity<ForeignSpecialization>()
                .HasKey(fs => new { fs.Id, fs.ForeignDoctorId });

            modelBuilder.Entity<ForeignDoctor>()
                .HasMany(d => d.ForeignSpecializations)
                .WithRequired(fs => fs.ForeignDoctor)
                .HasForeignKey(fs => fs.ForeignDoctorId);
        }
    }
}
