using System;
using JobSearchApp.Models;
using MVCJobSearchApp.Models;
using MVCJobSearchApp.Views;
using MySql.Data.MySqlClient;

namespace MVCJobSearchApp.Controllers
{
    public class CvController
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
        public static void ViewCV(int userId)
        {
            try
            {
                int applicantId = GetApplicantId(userId);
                if (applicantId == -1)
                {
                    Console.WriteLine("No applicant_id corresponding to this user_id was found.");
                    return;
                }

                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT cv_id, location, skill, education, description FROM Cvs WHERE applicant_id = @applicantId";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@applicantId", applicantId);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"CV ID: {reader.GetInt32("cv_id")}, Location: {reader.GetString("location")}, Skill: {reader.GetString("skill")}, Education: {reader.GetString("education")}, Description: {reader.GetString("description")}");
                            }
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

        public static void CreateCV(int userId)
        {
            if (!ApplicantController.IsApplicantInfoUpdated(userId))
            {
                Console.WriteLine("Please update your information before creating your CV.");
                return;
            }

            try
            {
                using (MySqlConnection connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    int applicantId = GetApplicantId(userId);
                    if (applicantId == -1)
                    {
                        Console.WriteLine("No applicant_id corresponding to this user_id was found.");
                        return;
                    }

                    CV cv = new CV
                    {
                        ApplicantId = applicantId
                    };

                   bool CheckForExit(string input)
                    {
                        if (input.Equals("x", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("\nExiting update process.");
                            return true;
                        }
                        return false;
                    }

                    // Nhập địa chỉ và kiểm tra không để trống
                    do
                    {
                        Console.Write("Enter Address (Press 'x' to exit): ");
                        cv.Location = Console.ReadLine();
                        if (CheckForExit( cv.Location)) return;
                        if (string.IsNullOrWhiteSpace(cv.Location))
                        {
                            Console.WriteLine("Address cannot be left blank. Please re-enter.");
                        }
                    } while (string.IsNullOrWhiteSpace(cv.Location));

                    // Nhập kỹ năng không được để trống
                    do
                    {
                        Console.Write("Enter Skill (Press 'x' to exit): ");
                        cv.Skill = Console.ReadLine();
                        if (CheckForExit(cv.Skill)) return;
                        if (string.IsNullOrWhiteSpace(cv.Skill))
                        {
                            Console.WriteLine("Skill cannot be left blank. Please re-enter.");
                        }
                    } while (string.IsNullOrWhiteSpace(cv.Skill));

                    // Nhập học vấn không được để trống
                    do
                    {
                        Console.Write("Enter Education (Press 'x' to exit): ");
                        cv.Education = Console.ReadLine();
                        if (CheckForExit(cv.Education)) return;
                        if (string.IsNullOrWhiteSpace(cv.Education))
                        {
                            Console.WriteLine("Education cannot be left blank. Please re-enter.");
                        }
                    } while (string.IsNullOrWhiteSpace(cv.Education));

                    // Nhập mô tả không được để trống
                    do
                    {
                        Console.Write("Enter Description (Press 'x' to exit): ");
                        cv.Description = Console.ReadLine();
                        if (CheckForExit(cv.Description)) return;
                        if (string.IsNullOrWhiteSpace(cv.Description))
                        {
                            Console.WriteLine("Description cannot be left blank. Please re-enter.");
                        }
                    } while (string.IsNullOrWhiteSpace(cv.Description));

                    // Lưu CV vào cơ sở dữ liệu
                    string insertCvQuery = "INSERT INTO Cvs (applicant_id, location, skill, education, description) " +
                                        "VALUES (@applicantId, @location, @skill, @education, @description)";
                    using (MySqlCommand insertCvCommand = new MySqlCommand(insertCvQuery, connection))
                    {
                        insertCvCommand.Parameters.AddWithValue("@applicantId", applicantId);
                        insertCvCommand.Parameters.AddWithValue("@location", cv.Location);
                        insertCvCommand.Parameters.AddWithValue("@skill", cv.Skill);
                        insertCvCommand.Parameters.AddWithValue("@education", cv.Education);
                        insertCvCommand.Parameters.AddWithValue("@description", cv.Description);

                        int rowsAffected = insertCvCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("CV has been saved successfully!");
                        }
                        else
                        {
                            Console.WriteLine("Error! Cannot save CV.");
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


       public static void UpdateCV(int userId)
        {
            try
            {
                int applicantId = GetApplicantId(userId);
                if (applicantId == -1)
                {
                    Console.WriteLine("No applicant_id corresponding to this user_id was found.");
                    return;
                }

                // Gọi hàm ViewCV để hiển thị danh sách CV của applicant
                ViewCV(userId);

                Console.Write("Enter the CV ID you want to update: ");
                if (int.TryParse(Console.ReadLine(), out int cvId))
                {
                    CV cv = new CV { ApplicantId = applicantId, CVId = cvId };

                    using (MySqlConnection connection = DatabaseHelper.GetConnection())
                    {
                        connection.Open();

                        bool CheckForExit(string input)
                        {
                            if (input.Equals("x", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("\nExiting update process.");
                                return true;
                            }
                            return false;
                        }

                        // Cập nhật các thông tin của CV
                        do
                        {
                            Console.Write("Enter new Address (Press 'x' to exit, or press Enter to keep current): ");
                            cv.Location = Console.ReadLine();
                            if (CheckForExit(cv.Location)) return;
                            if (string.IsNullOrWhiteSpace(cv.Location))
                            {
                                Console.WriteLine("Address cannot be left blank. Please re-enter.");
                            }
                        } while (string.IsNullOrWhiteSpace(cv.Location));

                        do
                        {
                            Console.Write("Enter new Skill (Press 'x' to exit, or press Enter to keep current): ");
                            cv.Skill = Console.ReadLine();
                            if (CheckForExit(cv.Skill)) return;
                            if (string.IsNullOrWhiteSpace(cv.Skill))
                            {
                                Console.WriteLine("Skill cannot be left blank. Please re-enter.");
                            }
                        } while (string.IsNullOrWhiteSpace(cv.Skill));

                        do
                        {
                            Console.Write("Enter new Education (Press 'x' to exit, or press Enter to keep current): ");
                            cv.Education = Console.ReadLine();
                            if (CheckForExit(cv.Education)) return;
                            if (string.IsNullOrWhiteSpace(cv.Education))
                            {
                                Console.WriteLine("Education cannot be left blank. Please re-enter.");
                            }
                        } while (string.IsNullOrWhiteSpace(cv.Education));

                        do
                        {
                            Console.Write("Enter new Description (Press 'x' to exit, or press Enter to keep current): ");
                            cv.Description = Console.ReadLine();
                            if (CheckForExit(cv.Description)) return;
                            if (string.IsNullOrWhiteSpace(cv.Description))
                            {
                                Console.WriteLine("Description cannot be left blank. Please re-enter.");
                            }
                        } while (string.IsNullOrWhiteSpace(cv.Description));

                        // Lưu thay đổi vào cơ sở dữ liệu
                        string updateCvQuery = "UPDATE Cvs SET location = @location, skill = @skill, education = @education, description = @description WHERE cv_id = @cvId";
                        using (MySqlCommand updateCvCommand = new MySqlCommand(updateCvQuery, connection))
                        {
                            updateCvCommand.Parameters.AddWithValue("@location", cv.Location);
                            updateCvCommand.Parameters.AddWithValue("@skill", cv.Skill);
                            updateCvCommand.Parameters.AddWithValue("@education", cv.Education);
                            updateCvCommand.Parameters.AddWithValue("@description", cv.Description);
                            updateCvCommand.Parameters.AddWithValue("@cvId", cv.CVId);

                            int rowsAffected = updateCvCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("CV has been updated successfully!");
                            }
                            else
                            {
                                Console.WriteLine("Error! Cannot update CV.");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid CV ID.");
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


        public static void DeleteCV(int userId)
        {
            try
            {
                int applicantId = GetApplicantId(userId);
                if (applicantId == -1)
                {
                    Console.WriteLine("No applicant_id corresponding to this user_id was found.");
                    return;
                }

                ViewCV(userId);

                Console.Write("Enter the CV ID you want to delete: ");
                if (int.TryParse(Console.ReadLine(), out int cvId))
                {
                    Console.Write("Are you sure you want to delete this CV? (yes/no): ");
                    string confirmation = Console.ReadLine();

                    if (confirmation.ToLower() == "yes")
                    {
                        using (MySqlConnection connection = DatabaseHelper.GetConnection())
                        {
                            connection.Open();

                            // Kiểm tra xem CV có thuộc về applicant không
                            string checkCvQuery = "SELECT COUNT(*) FROM Cvs WHERE cv_id = @cvId AND applicant_id = @applicantId";
                            using (MySqlCommand checkCvCommand = new MySqlCommand(checkCvQuery, connection))
                            {
                                checkCvCommand.Parameters.AddWithValue("@cvId", cvId);
                                checkCvCommand.Parameters.AddWithValue("@applicantId", applicantId);

                                int cvCount = Convert.ToInt32(checkCvCommand.ExecuteScalar());
                                if (cvCount == 0)
                                {
                                    Console.WriteLine("Invalid CV ID or CV does not belong to you.");
                                    return;
                                }
                            }

                            // Xóa CV từ cơ sở dữ liệu
                            string deleteCvQuery = "DELETE FROM Cvs WHERE cv_id = @cvId AND applicant_id = @applicantId";
                            using (MySqlCommand deleteCvCommand = new MySqlCommand(deleteCvQuery, connection))
                            {
                                deleteCvCommand.Parameters.AddWithValue("@cvId", cvId);
                                deleteCvCommand.Parameters.AddWithValue("@applicantId", applicantId);

                                int rowsAffected = deleteCvCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    Console.WriteLine("CV has been deleted successfully!");
                                }
                                else
                                {
                                    Console.WriteLine("Error! Cannot delete CV.");
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("CV deletion canceled.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid CV ID.");
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

    }
}