using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTSv9.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UsernameChangeLimit { get; set; } = 10;
        public DateTime BirthDate { get; set; }
        public ICollection<ApplicationUser> Details { get; set; }
    }
    //public class ProfilePage
    //{
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public DateTime BirthDate { get; set; }
    //    public ICollection<ApplicationUser> Details { get; set; }
    //}
}
