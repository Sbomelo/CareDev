using CareDev.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareDev.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //Core entities
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Bed> Beds { get; set; }
        public DbSet<Admission> Admissions { get; set; }
        public DbSet<Discharge> Discharges { get; set; }
        public DbSet<PatientMovement> PatientMovements { get; set; }
        public DbSet<PatientFolder> PatientFolders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vital> Vitals { get; set; } 
        public DbSet<TreatPatient> TreatPatients { get; set; }
        public DbSet<DoctorInstruction> DoctorInstructions { get; set; } 

        //look-up tables
        public DbSet<GenderOption> GenderOptions { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<ChronicCondition> ChronicConditions { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //configure compositr keys for junction tables
            modelBuilder.Entity<PatientFolder>().HasKey(pf => new { pf.PatientId, pf.PatientFolderId });
            modelBuilder.Entity<Allergy>().HasKey(pa => new { pa.Patients, pa.AllergyId });
            modelBuilder.Entity<ChronicCondition>().HasKey(pc => new { pc.Patients, pc.ChronicConditionId });
            modelBuilder.Entity<Medication>().HasKey(m => new { m.Patients, m.MedicationId });

            //PATIENT relationships
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Gender)
                .WithMany()
                .HasForeignKey(p => p.GenderOptionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Allergies)
                .WithOne(pa => pa.Patient)
                .HasForeignKey(pa => pa.PatientId);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.ChronicConditions)
                .WithOne(pc => pc.Patient) 
                .HasForeignKey(pc => pc.PatientId);

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.PatientFolder)
                .WithOne(pf => pf.Patient)
                .HasForeignKey<PatientFolder>(pf => pf.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            //ADMISSION relationships
            modelBuilder.Entity<Admission>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Admissions)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Admission>()
                 .HasOne(a => a.Ward)
                 .WithMany(w => w.Admissions)
                 .HasForeignKey(a => a.WardId);

            modelBuilder.Entity<Admission>()
                .HasOne(a => a.Bed)
                .WithMany(b => b.Admissions)
                .HasForeignKey(a => a.BedId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Admission>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Admissions)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            //Discharge relationships
            modelBuilder.Entity<Discharge>()
                .HasOne(d => d.Patient)
                .WithMany(p => p.PatientId) 
                .HasForeignKey(d => d.PatientId);

            modelBuilder.Entity<Discharge>()
                .HasOne(d => d.Admission)
                .WithOne()
                .HasForeignKey<Discharge>(d => d.AdmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            //PatinetMovement relationships
            modelBuilder.Entity<PatientMovement>()
                .HasOne(pm => pm.Patient)
                .WithMany(p => p.Movement)
                .HasForeignKey(pm => pm.PatientId);

            //Ward to Bed relationships 
            modelBuilder.Entity<Ward>()
                .HasMany(w => w.Beds)
                .WithOne(b => b.Ward)
                .HasForeignKey(b => b.WardId)
                .OnDelete(DeleteBehavior.Cascade);

            //seed look up data
            SeedLookupData(modelBuilder);

        }
        private void SeedLookupData(ModelBuilder modelBuilder)
        {
            //seed GenderOptions
            modelBuilder.Entity<GenderOption>().HasData(
                new GenderOption { GenderOptionId = 1, Name = "Male" },
                new GenderOption { GenderOptionId = 2, Name = "Female" },
                new GenderOption { GenderOptionId = 3, Name = "Other" }
                );

            //seed Allergy Options
            modelBuilder.Entity<Allergy>().HasData(
                new Allergy { AllergyId = 1, Name = "None" },
                new Allergy { AllergyId = 2, Name = "Peanuts" },
                new Allergy { AllergyId = 3, Name = "Shellfish" },
                new Allergy { AllergyId = 4, Name = "Penicillin" },
                new Allergy { AllergyId = 5, Name = "Latex" }
            );

            //seed Chronic Conditions
            modelBuilder.Entity<ChronicCondition>().HasData(
                new ChronicCondition { ChronicConditionId = 1, Name = "None" },
                new ChronicCondition { ChronicConditionId = 2, Name = "Diabetes" },
                new ChronicCondition { ChronicConditionId = 3, Name = "Hypertension" },
                new ChronicCondition { ChronicConditionId = 4, Name = "Asthma" },
                new ChronicCondition { ChronicConditionId = 5, Name = "Heart Disease" } 
            );

            //seed wards
            modelBuilder.Entity<Ward>().HasData(
                new Ward { WardId = 1, Name = "General Ward" },
                new Ward { WardId = 2, Name = "Surgical Ward" },
                new Ward { WardId = 3, Name = "Maternity Ward"} 
            );

            //seed Medications
            modelBuilder.Entity<Medication>().HasData(
                new Medication { MedicationId = 1, Name = "Paracetamol" },
                new Medication { MedicationId = 2, Name = "Ibuprofen" },
                new Medication { MedicationId = 3, Name = "Amoxicillin" },
                new Medication { MedicationId = 4, Name = "Aspirin" },
                new Medication { MedicationId = 5, Name = "Metformin" }
            );
        }
    }
}
