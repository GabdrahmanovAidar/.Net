using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MayakElectronics.Database
{
    public class AppIdentityUser : IdentityUser
    {
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public DateTime DayOfBirth { get; set; }

        public AppIdentityUser(string userName, string email, string firstName, string lastName, DateTime dateOfBirth)
            : base(userName)
        {
            Email = email;
            //to do validate email!!!
            FirstName = firstName;
            LastName = lastName;
            DayOfBirth = dateOfBirth;
        }

        public AppIdentityUser()
            : base()
        {

        }
    }
}
