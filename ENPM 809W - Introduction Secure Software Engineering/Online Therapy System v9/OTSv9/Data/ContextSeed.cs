using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using OTSv9.Models;
using PowerArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTSv9.Data
{
    public class ContextSeed
    {
        private readonly IConfiguration Configuration;

        public ContextSeed(IConfiguration config)
        {
            Configuration = config;
        }
      
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            var defaultUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                FirstName = "Abhay",
                LastName = "Khattar",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true                
            };

            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    //var admin_password = _config["Admin:Password"];
                    var Password = "123Pa$$word.";
                    await userManager.CreateAsync(defaultUser, Password);
                    await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Admin.ToString()));
                    await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Doctor.ToString()));
                    await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Patient.ToString()));
                }
            }
        }
    }
}
