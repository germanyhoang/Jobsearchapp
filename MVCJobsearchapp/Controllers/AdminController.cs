using System;
using MVCJobSearchApp.Models;
using MVCJobSearchApp.Views;
using MySql.Data.MySqlClient;
namespace MVCJobSearchApp.Controllers
{
    public class AdminController
    {
        public static void CreateAccount(string role, int created_by)
        {
            while (true)
            {
                Console.Write($"Enter {role} username (or type 'cancel' to exit): ");
                string username = Console.ReadLine();

                if (username.ToLower() == "cancel")
                {
                    Console.WriteLine("Account creation canceled.");
                    break;
                }

                if (string.IsNullOrWhiteSpace(username) || ContainsSpecialCharacters(username))
                {
                    Console.WriteLine("Invalid username. Please try again.");
                    continue;
                }

                Console.Write($"Enter {role} password (or type 'cancel' to exit): ");
                string password = Console.ReadLine();

                if (password.ToLower() == "cancel")
                {
                    Console.WriteLine("Account creation canceled.");
                    break;
                }

                if (string.IsNullOrWhiteSpace(password) || ContainsSpecialCharacters(password))
                {
                    Console.WriteLine("Invalid password. Please try again.");
                    continue;
                }

                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    if (role == "admin")
                    {
                        string query = "INSERT INTO Users (username, password, role, created_by) VALUES (@username, @password, @role, @created_by)";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@role", role);
                        command.Parameters.AddWithValue("@created_by", created_by);

                        try
                        {
                            int result = command.ExecuteNonQuery();
                            if (result > 0)
                            {
                                Console.WriteLine($"{role} account creation successful.");
                            }
                            else
                            {
                                Console.WriteLine($"{role} account creation failed.");
                            }
                        }
                        catch (MySqlException ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        break; // Exit the loop after account creation attempt
                    }
                }
            }
        }

        

        public static void ApproveJobs()
        {
            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT job_id, title, description, location_job, salary FROM Jobs WHERE job_status = @status";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", 0); // Chỉ lấy các bài đăng có trạng thái 0

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    // Tạo header cho bảng
                    Console.WriteLine("+--------+------------------+-----------------------------+----------------+----------+");
                    Console.WriteLine("| Job ID | Title            | Description                 | Location       | Salary   |");
                    Console.WriteLine("+--------+------------------+-----------------------------+----------------+----------+");

                    while (reader.Read())
                    {
                        Job job = new Job
                        {
                        JobId = reader.GetInt32("job_id"),
                        Title = TruncateString(reader.GetString("title"),16),
                        Description = reader.GetString("description"),
                        Location = reader.GetString("location_job"),
                        Salary = reader.GetDecimal("salary")
                        };
                        // Lấy thông tin từng cột
                        

                        // Hiển thị thông tin từng dòng
                        Console.WriteLine($"| {job.JobId,-6} | {job.Title,-16} | {job.Description,-27} | {job.Location,-14} | {job.Salary,-8:C} |");

                        // Hiển thị yêu cầu duyệt bài đăng
                        Console.WriteLine("+--------+------------------+-----------------------------+----------------+----------+");
                        Console.WriteLine("Press 'y' to browse this post or any key to skip.");
                        string input = Console.ReadLine();
                        if (input.ToLower() == "y")
                        {
                            ApproveJob(job.JobId);
                        }
                    }
                }
            }
        }

        public static void ApproveJob(int jobId)
        {
            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "UPDATE Jobs SET job_status = @status WHERE job_id = @job_id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", 1); // Duyệt bài đăng
                command.Parameters.AddWithValue("@job_id", jobId);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("The post has been approved successfully!");
                }
                else
                {
                    Console.WriteLine("Cannot browse posts.");
                }
            }
        }

        public static  string TruncateString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
        }

        public static void ListUser()
        {
            Console.WriteLine("User list:");
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("{0, -5} | {1, -20} | {2, -30}", "ID", "UserName", "Role");
            Console.WriteLine("-------------------------------------------------------");
            try
            {
                using( MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = " select user_id , username, role from Users" ;
                    MySqlCommand command = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User
                            {
                                UserId = reader.GetInt32("user_id"),
                                Username = reader.GetString("username"),
                                Role = reader.GetString("role")
                            };
                            Console.WriteLine("{0, -5} | {1, -20} | {2, -30}", user.UserId,user.Username,user.Role);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! An error occurred. Please try again later: " + ex.Message);
            }
        }
        public static void DeleteAccount()
            {
                while (true)
                {
                    ListUser();
                     Console.Write("Enter the User ID to delete (or type 'cancel' to exit): ");
                    string userIdInput = Console.ReadLine();

                    if (userIdInput.ToLower() == "cancel")
                    {
                        Console.WriteLine("Account deletion canceled.");
                        break;
                    }
                    if (int.TryParse(userIdInput, out int userId))
                    {
                        Console.Write("Are you sure you want to delete this account? (yes/no): ");
                        string confirmation = Console.ReadLine();

                        if (confirmation.ToLower() == "yes")
                        {
                            try
                            {
                                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                                {
                                    connection.Open();

                                    // Xóa tài khoản từ bảng Users
                                    string deleteUserQuery = "DELETE FROM Users WHERE user_id = @userId";
                                    MySqlCommand deleteUserCommand = new MySqlCommand(deleteUserQuery, connection);
                                    deleteUserCommand.Parameters.AddWithValue("@userId", userId);

                                    int rowsAffected = deleteUserCommand.ExecuteNonQuery();

                                    if (rowsAffected > 0)
                                    {
                                        Console.WriteLine("The account has been successfully deleted.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("No account found with the entered ID.");
                                    }
                                }
                            }
                            catch (MySqlException ex)
                            {
                                Console.WriteLine("Database error: " + ex.Message);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error! An error occurred. Please try again later: " + ex.Message);
                            }
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Account deletion canceled.");
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("User ID is invalid. Please enter a valid User ID.");
                    }
                }
            }

        private static bool ContainsSpecialCharacters(string input)
        {
            foreach (char c in input)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    return true;
                }
            }
            return false;
        }

    }
}