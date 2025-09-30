using Microsoft.AspNetCore.Identity;
using System.Configuration;
using System.Drawing;
using CareDev.Models;

namespace CareDev.Data
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            //seeding roles
            string[] roleNames = { "Admin", "WardAdmin", "Doctor", "Nurse", "Patient" };
            foreach(var role in roleNames)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            //seeding default roles for each user
            var defaultUsers = new List<(string Role, string Email, string Password)>
            {
                ("Admin", "admin@site.com", "Admin@123!"),
                ("WardAdmin", "wardAdmin@admin.com", "WardAdmin@123!"),
                ("Doctor", "doctor@doctor.com", "Doctor@123!"),
                ("Nurse", "nurse@nurse.com", "Nurse@123!"),
                ("Patient", "patient@gmail.com", "Patient@123!")
            };

            foreach(var (role, email, password) in defaultUsers)
            {
                //check if user exists
                var existingUser = await userManager.FindByEmailAsync(email);
                if (existingUser == null)
                {
                    //Creating User
                    var user = new IdentityUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true

                    };
                    var createResult = await userManager.CreateAsync((ApplicationUser)user, password);
                    if (createResult.Succeeded)
                    {
                        //Assign role
                        await userManager.AddToRoleAsync((ApplicationUser)user, role);
                    }
                    else
                    {
                        //Debbuuing purposes - log if user creation failed
                        foreach(var error in createResult.Errors)
                        {
                            Console.WriteLine($"Error creating {role} user : {error.Description}");
                        }
                    }
                }
            }
        }
    }

}
