namespace MVCJobSearchApp.Models
{
    public class Admin : User
    {
        public Admin(int userId, string username, string role) : base()
        {
            this.UserId = userId;
            this.Username = username;
            this.Role = role;
        }
    }
}