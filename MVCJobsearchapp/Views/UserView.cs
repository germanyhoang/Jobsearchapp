using System;
using MVCJobSearchApp.Controllers;

namespace MVCJobSearchApp.Views
{
    public class UserView
    {
        public static void ShowMainMenu()
        {
            while (true)
            {
                Console.WriteLine("==============================================================");
                Console.WriteLine("||                                                          ||");
                Console.WriteLine("||                       JOBSEARCHAPP                       ||");
                Console.WriteLine("||                                                          ||");
                Console.WriteLine("==============================================================");
                Console.WriteLine("|| 1. Job listing                                           ||");
                Console.WriteLine("|| 2. Company list                                          ||");
                Console.WriteLine("|| 3. Quick search (company, job)                           ||");
                Console.WriteLine("|| 4. Login                                                 ||");
                Console.WriteLine("|| 5. Register                                              ||");
                Console.WriteLine("|| 6. Exit                                                  ||");
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
                        ApplicantController.ViewJobs();
                        break;
                    case 2:
                        ApplicantController.ViewCompanies();
                        break;
                    case 3:
                        ApplicantController.ContinuousSearch();
                        break;
                    case 4:
                        UserController.Login();
                        break;
                    case 5:
                        UserController.Register();
                        break;
                    case 6:
                        Console.WriteLine("Exiting the application...");
                        return;
                }
            }
        }
    }
}