using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class DatabaseHelper
{
    private static string connectionString = "Server=localhost;Database=jobsearchapp;User Id=root;Password=ducdudon123;";

    public static MySqlConnection GetConnection()
    {
        return new MySqlConnection(connectionString);
    }

    public void TestConnection()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                Console.WriteLine("Ket noi thanh cong!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Phat hien loi khi ket noi du lieu: " + ex.Message);
            }
        }
    }
}
