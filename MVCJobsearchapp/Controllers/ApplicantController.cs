using System;
using JobSearchApp.Models;
using MVCJobSearchApp.Models;
using MVCJobSearchApp.Views;
using MySql.Data.MySqlClient;

namespace MVCJobSearchApp.Controllers
{
    public class ApplicantController
    {
       private static int GetApplicantId(int userId)
            {
                int applicantId = -1;
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT applicant_id FROM Applicants WHERE user_id = @user_id";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@user_id", userId);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        applicantId = Convert.ToInt32(result);
                    }
                }
                return applicantId;
            }

        public static void ViewJobs()
            {
                Console.WriteLine("Job list:");
                Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("{0, -5} | {1, -20} | {2, -30} | {3, -20} | {4, -20} | {5, -10}", "ID", "Title", "Description", "Location", "Salary", "JobType");
                Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------");
                try
                {
                    List<int> jobIds = new List<int>(); // Assume jobIds is populated somewhere
                    using (MySqlConnection connection = DatabaseHelper.GetConnection())
                    {
                        connection.Open();
                        string query = "SELECT job_id, title, description, location_job, salary, job_type  FROM Jobs WHERE job_status = @status";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue("@status", 1);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Job job = new Job
                                {
                                JobId = reader.GetInt32("job_id"),
                                Title = AdminController.TruncateString(reader["title"].ToString(),20),
                                Description= AdminController.TruncateString(reader["description"].ToString(),30),
                                Location = reader["location_job"].ToString(),
                                Salary = reader.GetDecimal("salary"),
                                JobType = reader["job_type"].ToString(),
                                };
                                

                                Console.WriteLine("{0, -5} | {1, -20} | {2, -30} | {3, -20} | {4, -20} | {5, -10}", job.JobId, job.Title, job.Description, job.Location, job.Salary, job.JobType);
                                 // Add jobId to jobIds list
                                jobIds.Add(job.JobId);
                            }
                        }
                    }
                    Console.Write("Enter the ID of the job you want to apply for (or enter 0 to go back): ");
                    string jobIdInput = Console.ReadLine();
                    if (int.TryParse(jobIdInput, out int selectedJobId) && selectedJobId > 0)
                    {
                        if (jobIds.Contains(selectedJobId)) // Kiểm tra nếu job_id tồn tại trong danh sách
                        {
                            if (Program.loggedInUserId != 0) // Kiểm tra nếu người dùng đã đăng nhập
                            {
                                // Hiển thị danh sách CVs của người dùng và yêu cầu nhập cv_id
                                CvController.ViewCV(Program.loggedInUserId);
                                Console.Write("Enter the ID of the CV you want to apply for (or enter 0 to go back): ");
                                string cvIdInput = Console.ReadLine();
                                if (int.TryParse(cvIdInput, out int selectedCvId) && selectedCvId > 0)
                                {
                                    ApplyForJob(selectedJobId, selectedCvId);
                                }
                                else if (selectedCvId != 0)
                                {
                                    Console.WriteLine("Invalid ID.");
                                }
                            }
            
                            else
                            {
                                Console.WriteLine("Please log in or register to apply.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Job ID does not exist.");
                        }
                    }
                    else if (selectedJobId != 0)
                    {
                        Console.WriteLine("Invalid ID.");
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Database error:" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error! An error occurred. Please try again later: " + ex.Message);
                }

                Console.WriteLine("--------------------------------------------------------------------------------------");
            }

        public static void ApplyForJob(int selectedJobId, int selectedCvId)
        {
            int applicantId = GetApplicantId(Program.loggedInUserId); // Lấy applicant_id từ user_id đã đăng nhập
            if (applicantId == -1)
            {
                Console.WriteLine("Error! Unable to get user applicant_id.");
                return;
            }

            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "UPDATE JobApplications SET job_id = @job_id, application_status = @status WHERE applicant_id = @applicant_id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@job_id", selectedJobId);
                command.Parameters.AddWithValue("@applicant_id", applicantId);
                command.Parameters.AddWithValue("@status", "applied");

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Update application status successfully!");
                }
                else
                {
                    Console.WriteLine("No record found to update application status.");
                }
            }
        }
        public static void ViewCompanies(int pageNumber = 1, int itemsPerPage = 5)
        {
            Console.WriteLine("List of companies:");
            Console.WriteLine("--------------------------------------------------------------------------------------");
            Console.WriteLine("{0, -20} | {1, -30} | {2, -20}", "Ten cong ty", "Mo ta", "Website");
            Console.WriteLine("--------------------------------------------------------------------------------------");

            try
            {
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    int offset = (pageNumber - 1) * itemsPerPage;
                    string query = "SELECT company_name, company_description, company_website FROM Employers LIMIT @itemsPerPage OFFSET @offset";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@itemsPerPage", itemsPerPage);
                    command.Parameters.AddWithValue("@offset", offset);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Employer employer = new Employer
                            {
                                CompanyName = reader["company_name"].ToString(),
                                CompanyDescription = AdminController.TruncateString(reader["company_description"].ToString(), 30),
                                CompanyWebsite = AdminController.TruncateString(reader["company_website"].ToString(), 20),
                            };

                            Console.WriteLine("{0, -20} | {1, -30} | {2, -20}", employer.CompanyName, employer.CompanyDescription, employer.CompanyWebsite);
                        }
                    }
                }

                // Ask user if they want to go to the next or previous page
                Console.WriteLine("--------------------------------------------------------------------------------------");
                Console.WriteLine("Page {0}", pageNumber);
                Console.WriteLine("1. Next Page");
                Console.WriteLine("2. Previous Page");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewCompanies(pageNumber + 1, itemsPerPage);
                        break;
                    case "2":
                        if (pageNumber > 1)
                        {
                            ViewCompanies(pageNumber - 1, itemsPerPage);
                        }
                        else
                        {
                            Console.WriteLine("You are already on the first page.");
                            ViewCompanies(pageNumber, itemsPerPage);
                        }
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Returning to menu.");
                        break;
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

        public static void ContinuousSearch()
        {
            string keyword = string.Empty;
            ConsoleKeyInfo keyInfo;

            System.Timers.Timer searchTimer = new System.Timers.Timer(500);
            searchTimer.Elapsed += (sender, e) => PerformSearch(keyword);
            searchTimer.AutoReset = false;
                Console.WriteLine("Quick search (tap and press ESC to exit): ");
                while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Escape )
                {
                    if (keyInfo.Key == ConsoleKey.Backspace && keyword.Length>0)
                    {
                        keyword = keyword.Substring(0, keyword.Length - 1);
                    }
                    else if(!char.IsControl(keyInfo.KeyChar))
                    {
                        keyword += keyInfo.KeyChar;
                    }

                    Console.Clear();
                    Console.WriteLine("Quick search (tap and press ESC to exit): ");
                    Console.WriteLine("Current keywords: " + keyword);
                    Console.WriteLine("==============================================================");

                    searchTimer.Stop();
                    searchTimer.Start();
                }
        }

        public static void PerformSearch(string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return;

            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string jobQuery = @"
                    SELECT * FROM Jobs
                    WHERE title LIKE @keyword 
                    OR description LIKE @keyword 
                    OR location_job LIKE @keyword 
                    OR salary LIKE @keyword 
                    OR job_type LIKE @keyword";
                
                MySqlCommand jobCommand = new MySqlCommand(jobQuery, connection);
                jobCommand.Parameters.AddWithValue("@keyword", $"%{keyword}%");

                Console.WriteLine("Job list:");
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("{0, -5} | {1, -20} | {2, -30} | {3, -20} | {4, -10} | {5, -10}", "ID", "Title", "Descreiptiom", "Location", "Salary", "Jobtype");
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------");

                using (MySqlDataReader jobReader = jobCommand.ExecuteReader())
                {
                    while (jobReader.Read())
                    {
                        Job job = new Job
                                {
                                JobId = jobReader.GetInt32("job_id"),
                                Title = AdminController.TruncateString(jobReader["title"].ToString(),20),
                                Description= AdminController.TruncateString(jobReader["description"].ToString(),30),
                                Location = jobReader["location_job"].ToString(),
                                Salary = jobReader.GetDecimal("salary"),
                                JobType = jobReader["job_type"].ToString(),
                                };
                                

                                Console.WriteLine("{0, -5} | {1, -20} | {2, -30} | {3, -20} | {4, -20} | {5, -10}", job.JobId, job.Title, job.Description, job.Location, job.Salary, job.JobType);
                    }
                }

                string companyQuery = @"
                    SELECT * FROM Employers
                    WHERE company_name LIKE @keyword 
                    OR company_description LIKE @keyword 
                    OR company_website LIKE @keyword 
                    OR address LIKE @keyword";
                
                MySqlCommand companyCommand = new MySqlCommand(companyQuery, connection);
                companyCommand.Parameters.AddWithValue("@keyword", $"%{keyword}%");

                Console.WriteLine("\nList of companies:");
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("{0, -5} | {1, -20} | {2, -30} | {3, -20} | {4, -20}", "ID", "Name", "Descriptiom", "Website", "Adress");
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------");

                using (MySqlDataReader companyReader = companyCommand.ExecuteReader())
                {
                    while (companyReader.Read())
                    {
                        Employer employer = new Employer
                        {
                        EmployerId = companyReader.GetInt32("employer_id"),
                        CompanyName = AdminController.TruncateString(companyReader["company_name"].ToString(), 20),
                        CompanyDescription = AdminController.TruncateString(companyReader["company_description"].ToString(), 30),
                        CompanyWebsite= companyReader["company_website"].ToString(),
                        Address= companyReader["address"].ToString(),

                        };
                        
                        Console.WriteLine("{0, -5} | {1, -20} | {2, -30} | {3, -20} | {4, -20}", employer.EmployerId, employer.CompanyName, employer.CompanyDescription, employer.CompanyWebsite, employer.Address);
                    }
                }
            }
        }

        public static void UpdateApplicantInformation(int userId, string username, string role)
            {
                Applicant applicant = new Applicant(userId, username, role);

                // Hàm kiểm tra phím nhấn
                bool CheckForExit(string input)
                {
                    if (input.Equals("x", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("\nExiting update process.");
                        return true;
                    }
                    return false;
                }

                // Nhap ten khong duoc de trong
                do
                {
                    Console.Write("Enter firstname (Press 'x' to exit): ");
                    applicant.FirstName = Console.ReadLine();
                    if (CheckForExit(applicant.FirstName)) return;

                    if (string.IsNullOrWhiteSpace(applicant.FirstName) || ContainsSpecialCharacters(applicant.FirstName))
                    {
                        Console.WriteLine("Invalid firstname. Please try again.");
                    }
                } while (string.IsNullOrWhiteSpace(applicant.FirstName) || ContainsSpecialCharacters(applicant.FirstName));

                // Nhap ho khong duoc de trong
                do
                {
                    Console.Write("Enter lastname (Press 'x' to exit): ");
                    applicant.LastName = Console.ReadLine();
                    if (CheckForExit(applicant.LastName)) return;

                    if (string.IsNullOrWhiteSpace(applicant.LastName) || ContainsSpecialCharacters(applicant.LastName))
                    {
                        Console.WriteLine("Invalid lastname. Please try again.");
                    }
                } while (string.IsNullOrWhiteSpace(applicant.LastName) || ContainsSpecialCharacters(applicant.LastName));

                // nhap gioi tinh dung va khong duoc de trong
                do
                {
                    Console.Write("Enter Gender (male, female, other) (Press 'x' to exit): ");
                    string genderInput = Console.ReadLine();
                    if (CheckForExit(genderInput)) return;

                    if (string.IsNullOrWhiteSpace(genderInput) || ContainsSpecialCharacters(genderInput))
                    {
                        Console.WriteLine("Invalid Gender. Please try again.");
                    }
                    else if (Enum.TryParse(genderInput, true, out Gender gender) && Enum.IsDefined(typeof(Gender), gender))
                    {
                        applicant.Gender = gender;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid gender. Please enter again (male, female, other).");
                    }
                } while (true);

                // Nhập số điện thoại và kiểm tra định dạng
                string phoneNumber;
                string phoneNumberPattern = @"^0\d{9}$"; // Biểu thức chính quy cho số điện thoại bắt đầu bằng 0 và có 10 chữ số

                do
                {
                    Console.Write("Enter phone number (starts with 0 and includes 10 digits) (Press 'x' to exit): ");
                    phoneNumber = Console.ReadLine();
                    if (CheckForExit(phoneNumber)) return;

                    if (string.IsNullOrWhiteSpace(phoneNumber) || ContainsSpecialCharacters(phoneNumber))
                    {
                        Console.WriteLine("Invalid phone number. Please try again.");
                    }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, phoneNumberPattern)) // kiểm tra sđt có đúng với biểu thức chính quy không
                    {
                        applicant.PhoneAp = phoneNumber;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid phone number. Please enter again (starts with 0 and includes 10 numbers).");
                    }
                } while (true);

                bool isUpdated = UpdateInformation(applicant);

                if (isUpdated)
                {
                    Console.WriteLine("Successfully updated applicant information!");
                }
                else
                {
                    Console.WriteLine("Error updating applicant information!");
                }
            }

        private static bool UpdateInformation(Applicant applicant)
        {
            string genderForDb = applicant.Gender.ToString();
            using (MySqlConnection connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "update Applicants set first_name = @FirstName, last_name = @LastName, gender = @Gender, phone_ap = @Phone, updated_at = CURRENT_TIMESTAMP where user_id = @UserId";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", applicant.UserId);
                    command.Parameters.AddWithValue("@FirstName", applicant.FirstName);
                    command.Parameters.AddWithValue("@LastName", applicant.LastName);
                    command.Parameters.AddWithValue("@Gender", genderForDb);
                    command.Parameters.AddWithValue("@Phone", applicant.PhoneAp);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Successfully updated applicant information!");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Error updating applicant information!");
                        return false;
                    }
                }
            }
        }

        public static bool IsApplicantInfoUpdated(int userId)
            {
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Applicants WHERE user_id = @userId AND first_name IS NOT NULL AND last_name IS NOT NULL";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        Console.WriteLine($"Count: {count}");  // In giá trị count để kiểm tra
                        return count > 0;
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
