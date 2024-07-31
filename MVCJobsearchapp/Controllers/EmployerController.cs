using System;
using System.Data;
using MVCJobSearchApp.Models;
using MVCJobSearchApp.Views;
using MySql.Data.MySqlClient;
namespace MVCJobSearchApp.Controllers
{
    public class EmployerController
    {
        private static int GetEmployerId(int userId)
            {
                int employerId = -1;
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT employer_id FROM Employers WHERE user_id = @user_id";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@user_id", userId);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        employerId = Convert.ToInt32(result);
                    }
                }
                return employerId;
            }
        public static void PostJob()
        {
            int employerId = GetEmployerId(Program.loggedInUserId);
            if (employerId == -1)
            {
                Console.WriteLine("Error employer_id not found");
                return;
            }

            Job job = new Job();

            bool CheckForExit(string input)
            {
                if (input.Equals("x", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("\nExiting update process.");
                    return true;
                }
                return false;
            }

            do
            {
                Console.Write("Enter job title (Press 'x' to exit): ");
                job.Title = Console.ReadLine();
                if (CheckForExit(job.Title)) return;
                if (string.IsNullOrWhiteSpace(job.Title) || ContainsSpecialCharacters(job.Title))
                {
                    Console.WriteLine("Invalid job title. Please try again.");
                }
            } while (string.IsNullOrWhiteSpace(job.Title) || ContainsSpecialCharacters(job.Title));

            do
            {
                Console.Write("Enter job description (Press 'x' to exit): ");
                job.Description = Console.ReadLine();
                if (CheckForExit(job.Description)) return;
                if (string.IsNullOrWhiteSpace(job.Description) || ContainsSpecialCharacters(job.Description))
                {
                    Console.WriteLine("Invalid job description. Please try again.");
                }
            } while (string.IsNullOrWhiteSpace(job.Description) || ContainsSpecialCharacters(job.Description));

            do
            {
                Console.Write("Enter job location (Press 'x' to exit): ");
                job.Location = Console.ReadLine();
                if (CheckForExit(job.Location)) return;
                if (string.IsNullOrWhiteSpace(job.Location) || ContainsSpecialCharacters(job.Location))
                {
                    Console.WriteLine("Invalid job location. Please try again.");
                }
            } while (string.IsNullOrWhiteSpace(job.Location) || ContainsSpecialCharacters(job.Location));

            decimal salary;
            do
            {
                Console.Write("Enter job salary (Press 'x' to exit): ");
                string salaryInput = Console.ReadLine();
                if (CheckForExit(salaryInput)) return;
                if (decimal.TryParse(salaryInput, out salary))
                {
                    job.Salary = salary;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid salary format. Please try again.");
                }
            } while (true);

            do
            {
                Console.Write("Enter job type (full-time/part-time/contract) (Press 'x' to exit): ");
                job.JobType = Console.ReadLine();
                if (CheckForExit(job.JobType)) return;
                if (string.IsNullOrWhiteSpace(job.JobType) || ContainsSpecialCharacters(job.JobType))
                {
                    Console.WriteLine("Invalid job type. Please try again.");
                }
            } while (string.IsNullOrWhiteSpace(job.JobType) || ContainsSpecialCharacters(job.JobType));

            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO Jobs (employer_id, title, description, location_job, salary, job_type, job_status) " +
                            "VALUES (@employer_id, @title, @description, @location_job, @salary, @job_type, @job_status)";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@employer_id", employerId);
                command.Parameters.AddWithValue("@title", job.Title);
                command.Parameters.AddWithValue("@description", job.Description);
                command.Parameters.AddWithValue("@location_job", job.Location);
                command.Parameters.AddWithValue("@salary", job.Salary);
                command.Parameters.AddWithValue("@job_type", job.JobType);
                command.Parameters.AddWithValue("@job_status", 0); // trạng thái mặc định là 0 tức là chưa được admin duyệt

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Job posted successfully!");
                }
                else
                {
                    Console.WriteLine("Failed to post job.");
                }
            }
        }

        public static void ViewPostedJobs()
        {   
            int employerId = GetEmployerId(Program.loggedInUserId);
            if (employerId== -1)
            {
                Console.WriteLine("Error employer_id not found");
                return;
            }
            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Jobs WHERE employer_id = @employer_id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@employer_id", employerId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Job ID: {reader["job_id"]}, Title: {reader["title"]}, Location: {reader["location_job"]}, Salary: {reader["salary"]}");
                    }
                }
            }
        }

        public static void ViewJobApplications()
            {
                int employerId = GetEmployerId(Program.loggedInUserId);
                if (employerId== -1)
                {
                    Console.WriteLine("Error employer_id not found");
                    return;
                }
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT ja.application_id, ja.applicant_id, ja.job_id, ja.application_status, " +
                                "a.first_name, a.last_name, a.gender, a.phone_ap, a.email_ap, " +
                                "c.location, c.skill, c.education, c.description " +
                                "FROM JobApplications ja " +
                                "JOIN Applicants a ON ja.applicant_id = a.applicant_id " +
                                "JOIN Cvs c ON ja.cv_id = c.cv_id " +
                                "WHERE ja.application_status IS NOT NULL AND ja.job_id IN (SELECT job_id FROM Jobs WHERE employer_id = @employer_id)";
                                
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@employer_id", employerId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"Application ID: {reader["application_id"]}, Applicant: {reader["first_name"]} {reader["last_name"]}, Gender: {reader["gender"]}, Phone: {reader["phone_ap"]}, Email: {reader["email_ap"]}, Location: {reader["location"]}, Skill: {reader["skill"]}, Education: {reader["education"]}, Description: {reader["description"]}, Application Status: {reader["application_status"]}");

                            Console.WriteLine("------------------------------------------------------------");
                        }
                    }
                }

                Console.Write("Enter Application ID to update status (or enter 0 to go back): ");
                string applicationIdInput = Console.ReadLine();
                if (int.TryParse(applicationIdInput, out int applicationId) && applicationId > 0)
                {
                    Console.WriteLine("Choose new status:");
                    Console.WriteLine("1. Interview");
                    Console.WriteLine("2. Offered");
                    Console.WriteLine("3. Rejected");
                    Console.Write("Choose an option: ");
                    int option;
                    if (!int.TryParse(Console.ReadLine(), out option))
                    {
                        Console.WriteLine("Invalid option. Try again.");
                        return;
                    }

                    string newStatus;
                    switch (option)
                    {
                        case 1:
                            newStatus = "interviewing";
                            break;
                        case 2:
                            newStatus = "offered";
                            break;
                        case 3:
                            newStatus = "rejected";
                            break;
                        default:
                            Console.WriteLine("Invalid option. Try again.");
                            return;
                    }

                    UpdateApplicationStatus(applicationId, newStatus);
                }
            }


        public static void UpdateApplicationStatus(int applicationId, string newStatus)
        {
            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "UPDATE JobApplications SET application_status = @status WHERE application_id = @application_id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", newStatus);
                command.Parameters.AddWithValue("@application_id", applicationId);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Application status updated successfully!");
                }
                else
                {
                    Console.WriteLine("Failed to update application status.");
                }
            }
        }

        public static void UpdateEmployerInformation(int userId, string username, string role)
            {
                Employer employer = new Employer(userId, username, role);

                bool CheckForExit(string input)
                {
                    if (input.Equals("x", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("\nExiting update process.");
                        return true;
                    }
                    return false;
                }

                string phoneNumberPattern = @"^0\d{9}$"; // Biểu thức chính quy cho số điện thoại bắt đầu bằng 0 và có 10 chữ số

                do
                {
                    Console.Write("Enter phone number (starts with 0 and includes 10 digits): ");
                    employer.PhoneEm = Console.ReadLine();
                    if (CheckForExit(employer.PhoneEm)) return;
                    if (string.IsNullOrWhiteSpace(employer.PhoneEm))
                    {
                        Console.WriteLine("Phone number cannot be left blank. Please re-enter.");
                    }
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(employer.PhoneEm, phoneNumberPattern))
                    {
                        Console.WriteLine("Invalid phone number. Please enter again (starts with 0 and includes 10 digits).");
                    }
                } while (string.IsNullOrWhiteSpace(employer.PhoneEm) || !System.Text.RegularExpressions.Regex.IsMatch(employer.PhoneEm, phoneNumberPattern));

                do
                {
                    Console.Write("Enter Company Name: ");
                    employer.CompanyName = Console.ReadLine();
                    if (CheckForExit(employer.CompanyName)) return;
                    if (string.IsNullOrWhiteSpace(employer.CompanyName) || ContainsSpecialCharacters(employer.CompanyName))
                    {
                        Console.WriteLine("Invalid Company Name. Please try again.");
                    }
                } while (string.IsNullOrWhiteSpace(employer.CompanyName) || ContainsSpecialCharacters(employer.CompanyName));

                do
                {
                    Console.Write("Enter Description: ");
                    employer.CompanyDescription = Console.ReadLine();
                    if (CheckForExit(employer.CompanyDescription)) return;
                    if (string.IsNullOrWhiteSpace(employer.CompanyDescription) || ContainsSpecialCharacters(employer.CompanyDescription))
                    {
                        Console.WriteLine("Invalid Description. Please try again.");
                    }
                } while (string.IsNullOrWhiteSpace(employer.CompanyDescription) || ContainsSpecialCharacters(employer.CompanyDescription));

                do
                {
                    Console.Write("Enter Website: ");
                    employer.CompanyWebsite = Console.ReadLine();
                    if (CheckForExit(employer.CompanyWebsite)) return;
                    if (string.IsNullOrWhiteSpace(employer.CompanyWebsite) || ContainsSpecialCharacters(employer.CompanyWebsite))
                    {
                        Console.WriteLine("Invalid Website. Please try again.");
                    }
                } while (string.IsNullOrWhiteSpace(employer.CompanyWebsite) || ContainsSpecialCharacters(employer.CompanyWebsite));

                do
                {
                    Console.Write("Enter Address: ");
                    employer.Address = Console.ReadLine();
                    if (CheckForExit(employer.Address)) return;
                    if (string.IsNullOrWhiteSpace(employer.Address) || ContainsSpecialCharacters(employer.Address))
                    {
                        Console.WriteLine("Invalid Address. Please try again.");
                    }
                } while (string.IsNullOrWhiteSpace(employer.Address) || ContainsSpecialCharacters(employer.Address));

                bool isUpdated = UpdateInformationEm(employer);

                if (isUpdated)
                {
                    Console.WriteLine("Successfully updated employer information!");
                }
                else
                {
                    Console.WriteLine("Error updating employer information!");
                }
            }
        private static bool UpdateInformationEm(Employer employer)
        {
            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "update Employers set phone_em = @Phone, company_name = @Name, company_description = @Description, company_website = @Website, address = @Address, updated_at = CURRENT_TIMESTAMP where user_id = @UserId";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", employer.UserId);
                    command.Parameters.AddWithValue("@Phone", employer.PhoneEm);
                    command.Parameters.AddWithValue("@Name", employer.CompanyName);
                    command.Parameters.AddWithValue("@Description", employer.CompanyDescription);
                    command.Parameters.AddWithValue("@Website", employer.CompanyWebsite);
                    command.Parameters.AddWithValue("@Address", employer.Address);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Successfully updated employer information!");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Error updating employer information!!");
                        return false;
                    }
                }
            }
        }
        public static void DeleteJob()
            {
                try
                {
                    int employerId = GetEmployerId(Program.loggedInUserId);
                    if (employerId == -1)
                    {
                        Console.WriteLine("Error employer_id not found.");
                        return;
                    }

                    ViewPostedJobs();

                    Console.Write("Enter the Job ID you want to delete: ");
                    if (int.TryParse(Console.ReadLine(), out int jobId))
                    {
                        Console.Write("Are you sure you want to delete this Job? (yes/no): ");
                        string confirmation = Console.ReadLine();

                        if (confirmation.ToLower() == "yes")
                        {
                            using (MySqlConnection connection = DatabaseHelper.GetConnection())
                            {
                                connection.Open();

                                // Kiểm tra xem Job có thuộc về employer không
                                string checkJobQuery = "SELECT COUNT(*) FROM Jobs WHERE job_id = @jobId AND employer_id = @employerId";
                                using (MySqlCommand checkJobCommand = new MySqlCommand(checkJobQuery, connection))
                                {
                                    checkJobCommand.Parameters.AddWithValue("@jobId", jobId);
                                    checkJobCommand.Parameters.AddWithValue("@employerId", employerId);

                                    int jobCount = Convert.ToInt32(checkJobCommand.ExecuteScalar());
                                    if (jobCount == 0)
                                    {
                                        Console.WriteLine("Invalid Job ID or Job does not belong to you.");
                                        return;
                                    }
                                }

                                // Xóa Job từ cơ sở dữ liệu
                                string deletePostQuery = "DELETE FROM Jobs WHERE job_id = @jobId AND employer_id = @employerId";
                                using (MySqlCommand deletePostCommand = new MySqlCommand(deletePostQuery, connection))
                                {
                                    deletePostCommand.Parameters.AddWithValue("@jobId", jobId);
                                    deletePostCommand.Parameters.AddWithValue("@employerId", employerId);

                                    int rowsAffected = deletePostCommand.ExecuteNonQuery();

                                    if (rowsAffected > 0)
                                    {
                                        Console.WriteLine("Job has been deleted successfully!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error! Cannot delete job.");
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Job deletion canceled.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Job ID.");
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