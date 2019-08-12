using System;
using System.Collections.Generic;

namespace GuessAPI.Model
{
    public partial class Students
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime? Dob { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
