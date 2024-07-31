using System;
using MVCJobSearchApp.Controllers;
using MVCJobSearchApp.Views;

namespace MVCJobSearchApp
{
    class Program
    {
        public static int loggedInUserId = 0;

        static void Main(string[] args)
        {
            DatabaseHelper dbHelper = new DatabaseHelper();

            // Test database connection
            dbHelper.TestConnection();

            UserView.ShowMainMenu();
        }
    }
}
