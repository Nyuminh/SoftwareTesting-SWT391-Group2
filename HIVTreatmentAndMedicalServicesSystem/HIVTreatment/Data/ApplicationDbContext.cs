using HIVTreatment.Models;
using Microsoft.EntityFrameworkCore;

namespace HIVTreatment.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<BooksAppointment> BooksAppointments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<TreatmentPlan> TreatmentPlan { get; set; }
        public DbSet<Roles> Roles { get; set; }


        public DbSet<ARVProtocol> ARVProtocol { get; set; }
        public DbSet<Slot> Slot { get; set; }
        public DbSet<DoctorWorkSchedule> DoctorWorkSchedules { get; set; }
        public DbSet<Prescription> Prescription { get; set; }
        public DbSet<Medication> Medication { get; set; }
        public DbSet<LabTest> LabTests { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Thiết lập quan hệ Patient → User
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserID);
        }

    }
}