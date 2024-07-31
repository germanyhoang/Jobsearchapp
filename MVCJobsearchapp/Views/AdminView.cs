using System;
using MVCJobSearchApp.Controllers;

namespace MVCJobSearchApp.Views
{
    public static class AdminView
    {
        public static void ShowAdminMenu(int loggedInUserId, string username, string role)
        {
            while (true)
            {
               
                Console.WriteLine("==============================================================");
                Console.WriteLine("||                                                          ||");
                Console.WriteLine("||                       ADMIN MENU                         ||");
                Console.WriteLine("||                                                          ||");
                Console.WriteLine("==============================================================");
                Console.WriteLine("|| 1. Create Admin Account                                  ||");
                Console.WriteLine("|| 2. Browse job postings                                   ||");
                Console.WriteLine("|| 3. List Account                                          ||");
                Console.WriteLine("|| 4. Delete Account                                        ||");
                Console.WriteLine("|| 5. Logout                                                ||");
                Console.WriteLine("||                                                          ||");
                Console.WriteLine("==============================================================");
                Console.Write("Choose an option: ");
                int option;
                if (!int.TryParse(Console.ReadLine(), out option))
                {
                    Console.WriteLine("Invalid option. Try again.");
                    continue;
                }
                switch (option)
                {
                    case 1:
                        AdminController.CreateAccount("admin",loggedInUserId);
                        break;
                    case 2:
                        AdminController.ApproveJobs();
                        break;
                    case 3:
                        AdminController.ListUser();
                        break;
                    case 4:
                        AdminController.DeleteAccount();
                        break;
                    case 5:
                        Console.WriteLine("Logging out...");
                        return;
                }
            }
        }
    }
}