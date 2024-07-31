using System;

namespace MVCJobSearchApp.Models
{
    public class Applicant : User
    {
           public int ApplicantId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Gender Gender { get; set; }
            public string PhoneAp { get; set; }
            public string EmailAp { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        public Applicant(int userId, string username, string role) : base()
        {
            this.UserId = userId;
            this.Username = username;
            this.Role = role;
        }
    }
}