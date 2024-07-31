using System;
using MVCJobSearchApp.Controllers;

namespace MVCJobSearchApp.Views
{
    public class EmployerView
    {
        public static void ShowEmployerMenu(int loggedInUserId, string username, string role)
        {
            while (true)
            {
                Console.WriteLine("==============================================================");
                Console.WriteLine("||                                                          ||");
                Console.WriteLine("||                       EMPLOYER MENU                      ||");
                Console.WriteLine("||                                                          ||");
                Console.WriteLine("==============================================================");
                Console.WriteLine("|| 1. Post a Job                                            ||");
                Console.WriteLine("|| 2. View Posted Jobs                                      ||");
                Console.WriteLine("|| 3. View Jobs Application                                 ||");
                Console.WriteLine("|| 4. Update employer information                           ||");
                Console.WriteLine("|| 5. Delete Job                                            ||");
                Console.WriteLine("|| 6. Logout                                                ||");
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
                        EmployerController.PostJob();
                        break;
                    case 2:
                        EmployerController.ViewPostedJobs();
                        break;
                    case 3:
                        EmployerController.ViewJobApplications();
                        break;
                    case 4:
                        EmployerController.UpdateEmployerInformation(loggedInUserId, username, role);
                        break;
                    case 5:
                        EmployerController.DeleteJob();
                        break;
                    case 6:
                        Console.WriteLine("Logging out...");
                        return;
                }
            }
        }
    }
}