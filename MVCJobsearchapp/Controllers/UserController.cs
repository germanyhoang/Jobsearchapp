using System;
using MVCJobSearchApp.Models;
using MVCJobSearchApp.Views;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace MVCJobSearchApp.Controllers
{
    public class UserController
    {
        public static void Register()
        {
            while (true)
            {
                Console.Write("User name (or type 'cancel' to exit): ");
                string username = Console.ReadLine();

                if (username.ToLower() == "cancel")
                {
                    Console.WriteLine("Registration canceled.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(username) || ContainsSpecialCharacters(username))
                {
                    Console.WriteLine("Invalid username. Please try again.");
                    continue;
                }

                Console.Write("Password (or type 'cancel' to exit): ");
                string password = Console.ReadLine();

                if (password.ToLower() == "cancel")
                {
                    Console.WriteLine("Registration canceled.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(password) || ContainsSpecialCharacters(password))
                {
                    Console.WriteLine("Invalid password. Please try again.");
                    continue;
                }

                Console.Write("Email (or type 'cancel' to exit): ");
                string email = Console.ReadLine();

                if (email.ToLower() == "cancel")
                {
                    Console.WriteLine("Registration canceled.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                {
                    Console.WriteLine("Invalid email format. Please try again.");
                    continue;
                }

                string role;
                while (true)
                {
                    Console.Write("Role (employer/applicant) (or type 'cancel' to exit): ");
                    role = Console.ReadLine();

                    if (role.ToLower() == "cancel")
                    {
                        Console.WriteLine("Registration canceled.");
                        return;
                    }

                    if (role != "employer" && role != "applicant")
                    {
                        Console.WriteLine("Invalid role. Please enter 'employer' or 'applicant'.");
                    }
                    else
                    {
                        break;
                    }
                }

                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    MySqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        // Insert into Users table
                        string userQuery = "INSERT INTO Users (username, password, role) VALUES (@username, @password, @role)";
                        MySqlCommand userCommand = new MySqlCommand(userQuery, connection, transaction);
                        userCommand.Parameters.AddWithValue("@username", username);
                        userCommand.Parameters.AddWithValue("@password", password);
                        userCommand.Parameters.AddWithValue("@role", role);
                        userCommand.ExecuteNonQuery();

                        // Get the last inserted user_id
                        long userId = userCommand.LastInsertedId;

                        if (role == "employer")
                        {
                            // Insert into Employers table
                            string employerQuery = "INSERT INTO Employers (user_id, email_em) VALUES (@user_id, @employer_email)";
                            MySqlCommand employerCommand = new MySqlCommand(employerQuery, connection, transaction);
                            employerCommand.Parameters.AddWithValue("@user_id", userId);
                            employerCommand.Parameters.AddWithValue("@employer_email", email);
                            int employerResult = employerCommand.ExecuteNonQuery();

                            if (employerResult > 0)
                            {
                                Console.WriteLine("Successfully registered employer account.");
                            }
                            else
                            {
                                throw new Exception("Registration failed!");
                            }
                        }
                        else if (role == "applicant")
                        {
                            // Insert into Applicants table
                            string applicantQuery = "INSERT INTO Applicants (user_id, email_ap) VALUES (@user_id, @applicant_email)";
                            MySqlCommand applicantCommand = new MySqlCommand(applicantQuery, connection, transaction);
                            applicantCommand.Parameters.AddWithValue("@user_id", userId);
                            applicantCommand.Parameters.AddWithValue("@applicant_email", email);
                            int applicantResult = applicantCommand.ExecuteNonQuery();

                            if (applicantResult > 0)
                            {
                                Console.WriteLine("Successfully registered applicant account.");
                            }
                            else
                            {
                                throw new Exception("Registration failed!");
                            }
                        }

                        // Commit transaction
                        transaction.Commit();
                        Console.WriteLine("Successfully registered.");
                        return;
                    }
                    catch (MySqlException ex)
                    {
                        // Rollback transaction in case of error
                        transaction.Rollback();
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }
        public static string HandleUserInput(string prompt)
        {
            string input;
            
            while (true)
            {
                Console.Write(prompt);
                input = Console.ReadLine();
                
                if (!string.IsNullOrWhiteSpace(input) && !ContainsSpecialCharacters(input))
                {
                    break; // Exit loop if input is valid
                }
                
                Console.WriteLine("Invalid input. Please try again.");
            }

            return input;
        }

        public static (int userId, string role)? AuthenticateUser(string username, string password)
        {
            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT user_id, role FROM Users WHERE username = @username AND password = @password";
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int userId = reader.GetInt32("user_id");
                    string role = reader.GetString("role");
                    return (userId, role);
                }
            }

            return null;
        }

        public static void DisplayMenu(string role, int userId, string username)
        {
            switch (role)
            {
                case "admin":
                    AdminView.ShowAdminMenu(userId, username, role);
                    break;
                case "employer":
                    EmployerView.ShowEmployerMenu(userId, username, role);
                    break;
                case "applicant":
                    ApplicantView.ShowApplicantMenu(userId, username, role);
                    break;
                default:
                    Console.WriteLine("Role cannot be determined.");
                    break;
            }
        }

        public static void Login()
        {
            bool loginSuccess = false;

            while (!loginSuccess)
            {
                string username = HandleUserInput("User name (or type 'cancel' to exit): ");
               
                if (username.ToLower() == "cancel")
                {
                    Console.WriteLine("Login canceled.");
                    return;
                }

                string password = HandleUserInput("Password (or type 'cancel' to exit): ");

                if (password.ToLower() == "cancel")
                {
                    Console.WriteLine("Login canceled.");
                    return;
                }

                var authResult = AuthenticateUser(username, password);
                if (authResult.HasValue)
                {
                    int userId = authResult.Value.userId;
                    string role = authResult.Value.role;
                    Program.loggedInUserId = userId;
                    Console.WriteLine($"Logged in successfully. Role: {role}");

                    DisplayMenu(role, userId, username);
                    loginSuccess = true;
                }
                else
                {
                    Console.WriteLine("Login failed. Please try again.");
                }
            }
        }

        // public static void Login()
        //     {
        //         bool loginSuccess = false;

        //         while (!loginSuccess)
        //         {
        //             Console.Write("User name (or type 'cancel' to exit): ");
        //             string username = Console.ReadLine();

        //             if (username == null)
        //             {
        //                 Console.WriteLine("Username cannot be null. Please try again.");
        //                 continue;
        //             }

        //             if (username.ToLower() == "cancel")
        //             {
        //                 Console.WriteLine("Login canceled.");
        //                 return;
        //             }

        //             if (string.IsNullOrWhiteSpace(username) || ContainsSpecialCharacters(username))
        //             {
        //                 Console.WriteLine("Invalid username. Please try again.");
        //                 continue;
        //             }

        //             Console.Write("Password (or type 'cancel' to exit): ");
        //             string password = Console.ReadLine();
                    
        //              if (password == null)
        //             {
        //                 Console.WriteLine("Password cannot be null. Please try again.");
        //                 continue;
        //             }
        //             if (password.ToLower() == "cancel")
        //             {
        //                 Console.WriteLine("Login canceled.");
        //                 return;
        //             }

        //             if (string.IsNullOrWhiteSpace(password) || ContainsSpecialCharacters(password))
        //             {
        //                 Console.WriteLine("Invalid password. Please try again.");
        //                 continue;
        //             }
        //             using (MySqlConnection connection = DatabaseHelper.GetConnection())
        //             {
        //                 connection.Open();
        //                 string query = "SELECT user_id, role FROM Users WHERE username = @username AND password = @password";
        //                 MySqlCommand command = new MySqlCommand(query, connection);
        //                 command.Parameters.AddWithValue("@username", username);
        //                 command.Parameters.AddWithValue("@password", password);

        //                 MySqlDataReader reader = command.ExecuteReader();
        //                 if (reader.Read())
        //                 {
        //                     int userId = reader.GetInt32("user_id");
        //                     string role = reader.GetString("role");
        //                     Program.loggedInUserId = userId; // Cập nhật ID người dùng đã đăng nhập
        //                     Console.WriteLine($"Logged in successfully. Role: {role}");

        //                     loginSuccess = true;

        //                     switch (role)
        //                     {
        //                         case "admin":
        //                             AdminView.ShowAdminMenu(userId, username, role);
        //                             break;
        //                         case "employer":
        //                             EmployerView.ShowEmployerMenu(userId, username, role);
        //                             break;
        //                         case "applicant":
        //                             ApplicantView.ShowApplicantMenu(userId, username, role);
        //                             break;
        //                         default:
        //                             Console.WriteLine("Role cannot be determined.");
        //                             break;
        //                     }
        //                 }
        //                 else
        //                 {
        //                     Console.WriteLine("Login failed. Please try again.");
        //                 }
        //             }
        //         }
        //     }
        public static bool ContainsSpecialCharacters(string input)
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

            private static bool IsValidEmail(string email)
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern);
            }
    }
}
