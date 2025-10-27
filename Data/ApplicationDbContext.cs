using CareDev.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareDev.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //Core entities
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
       public DbSet<MedicationAdministration> MedicationAdministrations { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Bed> Beds { get; set; }
        public DbSet<Admission> Admissions { get; set; }
        public DbSet<AdministerMeds> AdministeredMeds { get; set; }
        public DbSet<MedicationDispensation> MedicationDispensations { get; set; }

        public DbSet<Discharge> Discharges { get; set; }
        public DbSet<PatientMovement> PatientMovements { get; set; }
        public DbSet<PatientFolder> PatientFolders { get; set; }
        public DbSet<Vital> Vitals { get; set; }  
        public DbSet<TreatPatient> TreatPatients { get; set; }
        public DbSet<DoctorInstruction> DoctorInstructions { get; set; }
        public DbSet<PatientVitals> PatientVitals { get; set; }

        //look-up tables
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<ChronicCondition> ChronicConditions { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<Microsoft.AspNetCore.Mvc.Rendering.SelectListGroup>();
            modelBuilder.Ignore<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            //configure compositr keys for junction tables
            modelBuilder.Entity<PatientFolder>().HasKey(pf => new { pf.PatientId, pf.PatientFolderId });
           // modelBuilder.Entity<Allergy>().HasKey(pa => new { pa.Patients, pa.AllergyId });
            modelBuilder.Entity<PatientAllergy>().HasKey(pa => new { pa.PatientId, pa.AllergyId });
            modelBuilder.Entity<PatientCondition>().HasKey(pc => new { pc.PatientId, pc.ChronicConditionId });
            //modelBuilder.Entity<ChronicCondition>().HasKey(pc => new { pc.Patients, pc.ChronicConditionId });
           // modelBuilder.Entity<Medication>().HasKey(m => new { m.Patients, m.MedicationId });
           modelBuilder.Entity<MedicationAdministration>().HasKey(ma => new { ma.PatientId, ma.MedicationId});

            //PATIENT relationships
            /* modelBuilder.Entity<Patient>()
                 .HasOne(p => p.Gender)
                 .WithMany()
                 .HasForeignKey(p => p.GenderOptionId)
                 .OnDelete(DeleteBehavior.Restrict);*/
            modelBuilder.Entity<PatientAllergy>()
                .HasOne(pa => pa.Patient)
                .WithMany(p => p.PatientAllergies)
                .HasForeignKey(pa => pa.PatientId);

            modelBuilder.Entity<PatientAllergy>()
               .HasOne(pa => pa.Allergy)
               .WithMany(p => p.PatientAllergies)
               .HasForeignKey(pa => pa.AllergyId);

            modelBuilder.Entity<PatientCondition>()
                .HasOne(pa => pa.Patient)
                .WithMany(p => p.PatientConditions)
                .HasForeignKey(pa => pa.PatientId);

            modelBuilder.Entity<PatientCondition>()
               .HasOne(pa => pa.ChronicCondition)
               .WithMany(p => p.PatientConditions)
               .HasForeignKey(pa => pa.ChronicConditionId);

            //modelBuilder.Entity<PatientVitals>()
            //    .HasOne(pv => pv.Patient)
            //    .WithMany()
            //    .HasForeignKey(pv => pv.Patient)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicationDispensation>()
                .HasOne(md => md.Patient)
                .WithMany() // or WithMany(p => p.MedicationDispensations)
                .HasForeignKey(md => md.PatientUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicationDispensation>()
                .HasOne(md => md.Dispenser)
                .WithMany()
                .HasForeignKey(md => md.DispenserUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdministerMeds>()
                .HasOne(am => am.AdministeredBy)
                .WithMany()
                .HasForeignKey(am => am.AdministeredById)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<PatientVitals>()
            //    .HasOne(pv => pv.Nurse)
            //    .WithMany()
            //    .HasForeignKey(pv => pv.NurseUserId)
            //    .OnDelete(DeleteBehavior.Restrict);


            /*modelBuilder.Entity<Patient>()
             .HasMany(p => p.Allergies)
             .WithOne(a => a.Patient)
             .HasForeignKey(pa => pa.PatientId); */

            /* modelBuilder.Entity<Patient>()
               .HasMany(p => p.ChronicConditions)
               .WithOne(pc => pc.Patient) 
               .HasForeignKey(pc => pc.PatientId);*/
            //modelBuilder.Entity<PatientVitals>()
            //    .HasOne(pv => pv.Nurse)
            //    .WithMany()
            //    .HasForeignKey(pv => pv.NurseUserId)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientAllergy>()
                .HasOne(pa => pa.Patient)
                .WithMany(p => p.PatientAllergies)
                .HasForeignKey(pa => pa.PatientId);

            modelBuilder.Entity<PatientAllergy>()
               .HasOne(pa => pa.Allergy)
               .WithMany(p => p.PatientAllergies)
               .HasForeignKey(pa => pa.AllergyId);

            modelBuilder.Entity<PatientCondition>()
                .HasOne(pa => pa.Patient)
                .WithMany(p => p.PatientConditions)
                .HasForeignKey(pa => pa.PatientId);

            modelBuilder.Entity<PatientCondition>()
               .HasOne(pa => pa.ChronicCondition)
               .WithMany(p => p.PatientConditions)
               .HasForeignKey(pa => pa.ChronicConditionId);

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.PatientFolder)
                .WithOne(pf => pf.Patient)
                .HasForeignKey<PatientFolder>(pf => pf.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            //EMPLOYEE relationships
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Role)
                .WithMany(r => r.Employees)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasMany (e => e.Admissions)
                .WithOne(a => a.Employee)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.TreatPatients)
                .WithOne(tp => tp.Employee)
                .HasForeignKey(tp => tp.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            /* modelBuilder.Entity<Employee>()
                 .HasMany(e => e.Medications)
                 .WithOne(m => m.Employee)
                 .HasForeignKey(m => m.EmployeeId)
                 .OnDelete(DeleteBehavior.Restrict);*/
            modelBuilder.Entity<MedicationAdministration>()
                 .HasOne(ma => ma.Medication)
                 .WithMany(m => m.MedicationAdministrations)
                 .HasForeignKey(ma => ma.MedicationId);

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

            //modelBuilder.Entity<Admission>()
            //    .HasOne(a => a.Bed)
            //    .WithOne() 
            //    .HasForeignKey<Admission>(a => a.BedId)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Admission>()
                .HasOne(a => a.Employee)
                .WithMany(d => d.Admissions)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            //Discharge relationships
           modelBuilder.Entity<Discharge>()
                .HasOne(d => d.Patient) 
                .WithMany() 
                .HasForeignKey(d => d.PatientId);

            modelBuilder.Entity<Discharge>()
                .HasOne(d => d.Admission)
                .WithOne()
                .HasForeignKey<Discharge>(d => d.AdmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            //PatinetMovement relationships
            modelBuilder.Entity<PatientMovement>()
            .HasOne(pm => pm.Admission)
            .WithMany() // or .WithMany(a => a.PatientMovements) if you add that collection
            .HasForeignKey(pm => pm.AdmissionId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientMovement>()
                .HasOne(pm => pm.Patient)
                .WithMany()
                .HasForeignKey(pm => pm.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientMovement>()
                .HasOne(pm => pm.FromWard)
                .WithMany()
                .HasForeignKey(pm => pm.FromWardId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientMovement>()
                .HasOne(pm => pm.ToWard)
                .WithMany()
                .HasForeignKey(pm => pm.ToWardId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientMovement>()
                .HasOne(pm => pm.FromBed)
                .WithMany()
                .HasForeignKey(pm => pm.FromBedId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientMovement>()
                .HasOne(pm => pm.ToBed)
                .WithMany()
                .HasForeignKey(pm => pm.ToBedId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientMovement>()
                .HasOne(pm => pm.MovedByUser)
                .WithMany()
                .HasForeignKey(pm => pm.MovedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<PatientMovement>()
            //   .HasOne(pm => pm.Ward)
            //   .WithMany(p => p.Movement)
            //   .HasForeignKey(pm => pm.WardId); 

            //Bed to Admission relationship
            modelBuilder.Entity<Bed>()
                .HasMany(b => b.Admissions)
                .WithOne(a => a.Bed)
                .HasForeignKey(a => a.BedId)
                .OnDelete(DeleteBehavior.Restrict);

            //Ward to Bed relationships 
            modelBuilder.Entity<Ward>()
                .HasMany(w => w.Beds)
                .WithOne(b => b.Ward)
                .HasForeignKey(b => b.WardId) 
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Bed>(entity =>
            {
                entity.HasIndex(b => new { b.BedNumber, b.WardId }).IsUnique();
            });

           modelBuilder.Entity<Bed>(entity => 
           {
               entity.HasOne(b => b.Ward)
                   .WithMany(w => w.Beds)
                   .HasForeignKey(b => b.WardId);
                    
            });
           

            //seed look up data
            SeedLookupData(modelBuilder);

        }
        private void SeedLookupData(ModelBuilder modelBuilder)
        {
            //seed GenderOptions
          /*  modelBuilder.Entity<GenderOption>().HasData(
                new GenderOption { GenderOptionId = 1, Name = "Male" },
                new GenderOption { GenderOptionId = 2, Name = "Female" },
                new GenderOption { GenderOptionId = 3, Name = "Other" }
                );*/

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
                new Ward { WardId = 3, Name = "Maternity Ward"},
                new Ward { WardId = 4, Name = "X Ray Ward" },
                new Ward { WardId = 6, Name = "Labour Ward" }
            );

            ////seed Beds
            modelBuilder.Entity<Bed>().HasData(
                new Bed { BedId = 1, WardId = 6, BedNumber = "L-101", IsAvailable = false },
                new Bed { BedId = 2, WardId = 6, BedNumber = "L-102", IsAvailable = false },
                new Bed { BedId = 3, WardId = 6, BedNumber = "L-103", IsAvailable = false },
                new Bed { BedId = 4, WardId = 2, BedNumber = "S-201", IsAvailable = false },
                new Bed { BedId = 5, WardId = 2, BedNumber = "S-202", IsAvailable = false },
                new Bed { BedId = 6, WardId = 2, BedNumber = "S-203", IsAvailable = false },
                new Bed { BedId = 7, WardId = 3, BedNumber = "M-301", IsAvailable = false },
                new Bed { BedId = 8, WardId = 3, BedNumber = "M-302", IsAvailable = false },
                new Bed { BedId = 9, WardId = 3, BedNumber = "M-303", IsAvailable = false }
            );

            //seed Medications
            modelBuilder.Entity<Medication>().HasData(
                new Medication {MedicationId  = 1, Name ="None", Schedule="N/A" },
                new Medication { MedicationId = 2, Name = "Paracetamol", Schedule="PRN" },
                new Medication { MedicationId = 3, Name = "Ibuprofen", Schedule = "PRN" },
                new Medication { MedicationId = 4, Name = "Amoxicillin" , Schedule = "Scheduled" },
                new Medication { MedicationId = 5, Name = "Aspirin", Schedule = "Schedules"},
                new Medication { MedicationId = 6, Name = "Metformin", Schedule = "PRN" }
            );

            
        }
        public DbSet<CareDev.Models.PatientAllergy> PatientAllergy { get; set; } = default!;
        public DbSet<CareDev.Models.PatientCondition> PatientCondition { get; set; } = default!;
    }
}
