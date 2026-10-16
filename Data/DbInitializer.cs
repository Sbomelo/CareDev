using CareDev.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CareDev.Data
{
    public class DbInitializer
    {
        // Internal record to hold seed user data with all required fields
        private record SeedUser(
            string Role,
            string Email,
            string Password,
            string Name,
            string SurName,
            DateTime DateOfBirth,
            string IDNumber,
            string Gender,
            string? Address = null,
            string? City = null
        );

        public static async Task InitializeAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger? logger = null)  
        {
            // 1. Seed roles
            string[] roleNames = { "Admin", "WardAdmin", "Doctor", "Nurse", "Patient" };
            foreach (var role in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger?.LogInformation($"Role '{role}' created.");
                }
            }

            // 2. Seed default users with full ApplicationUser data
            var defaultUsers = new List<SeedUser>
            {
                new("Admin", "admin@site.com", "Admin@123!",
                    "System", "Admin", new DateTime(1985, 5, 15), "8505151234567", "Male"),

                new("WardAdmin", "wardAdmin@wadmin.com", "WardAdmin@123!",
                    "Ward", "Manager", new DateTime(1990, 8, 22), "9008229876543", "Female"),

                new("Doctor", "doctor@doctor.com", "Doctor@123!",
                    "Gregory", "House", new DateTime(1970, 6, 11), "7006111234987", "Male"),

                new("Nurse", "nurse@nurse.com", "Nurse@123!",
                    "Jackie", "Peyton", new DateTime(1982, 11, 2), "8211025678123", "Female"),

                new("Patient", "patient@gmail.com", "Patient@123!",
                    "John", "Doe", new DateTime(2000, 1, 1), "0001017890123", "Male",
                    "123 Main St", "Springfield")  // Optional Address & City
            };

            foreach (var userData in defaultUsers)
            {
                // Check if user already exists
                var existingUser = await userManager.FindByEmailAsync(userData.Email);
                if (existingUser != null)
                {
                    logger?.LogInformation($"User '{userData.Email}' already exists. Skipping.");
                    continue;
                }

                // Calculate age from DateOfBirth
                var today = DateTime.Today;
                var age = today.Year - userData.DateOfBirth.Year;
                if (userData.DateOfBirth.Date > today.AddYears(-age)) age--;

                // Build ApplicationUser with all required fields
                var user = new ApplicationUser
                {
                    UserName = userData.Email,
                    Email = userData.Email,
                    EmailConfirmed = true,

                    // Required properties from ApplicationUser
                    Name = userData.Name,
                    SurName = userData.SurName,
                    Age = age,
                    DateOfBirth = userData.DateOfBirth,
                    IDNumber = userData.IDNumber,
                    Gender = userData.Gender,

                    // Optional fields
                    Address = userData.Address,
                    City = userData.City,

                    // Defaults for nullable FK properties 
                    MedicationId = null,
                    AllergyId = null,
                    ChronicConditionId = null,

                    MustChangePassword = false  
                };

                // Create the user
                var createResult = await userManager.CreateAsync(user, userData.Password);
                if (createResult.Succeeded)
                {
                    // Assign role
                    await userManager.AddToRoleAsync(user, userData.Role);
                    logger?.LogInformation($"User '{userData.Email}' created and assigned to role '{userData.Role}'.");
                }
                else
                {
                    // Log errors 
                    foreach (var error in createResult.Errors)
                    {
                        var msg = $"Error creating {userData.Role} user '{userData.Email}': {error.Description}";
                        if (logger != null)
                            logger.LogError(msg);
                        else
                            Console.WriteLine(msg);
                    }
                }
            }

            logger?.LogInformation("Database initialization completed.");
        }
    }
}