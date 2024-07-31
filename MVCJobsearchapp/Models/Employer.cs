namespace MVCJobSearchApp.Models
{

    public class Employer : User
    {
            public int EmployerId { get; set; }
            public string PhoneEm { get; set; }
            public string EmailEm { get; set; }
            public string CompanyName { get; set; }
            public string CompanyDescription { get; set; }
            public string CompanyWebsite { get; set; }
            public string Address { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }

         public Employer() : base()
        {
        }
        public Employer(int userId, string username, string role) : base()
        {
            this.UserId = userId;
            this.Username = username;
            this.Role = role;
        }
    }
}