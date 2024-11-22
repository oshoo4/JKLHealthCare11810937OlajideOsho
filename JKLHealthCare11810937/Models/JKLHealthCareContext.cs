using Microsoft.EntityFrameworkCore;
using JKLHealthCare11810937.Models.DBModels;

namespace JKLHealthCare11810937.Models
{
    public class JKLHealthCareContext : DbContext
    {
        public JKLHealthCareContext(DbContextOptions<JKLHealthCareContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Caregiver> Caregivers { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }
}