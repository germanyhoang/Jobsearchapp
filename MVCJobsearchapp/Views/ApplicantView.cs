using System;
using MVCJobSearchApp.Controllers;

namespace MVCJobSearchApp.Views
{
    public class ApplicantView
    {
        public static void  ShowApplicantMenu(int loggedInUserId, string username, string role)
        {
            while (true)
            { 
                Console.WriteLine("==============================================================");
                Console.WriteLine("||                                                          ||");
                Console.WriteLine("||                       APPLICANT MENU                     ||");
                Console.WriteLine("||                                                          ||");
                Console.WriteLine("==============================================================");
                Console.WriteLine("|| 1. Job listing                                           ||");
                Console.WriteLine("|| 2. Company list                                          ||");
                Console.WriteLine("|| 3. Quick search (company, job)                           ||");
                Console.WriteLine("|| 4. Update applicant information                          ||");
                Console.WriteLine("|| 5. Menu cv                                               ||");
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
                        ApplicantController.ViewJobs();
                        break;
                    case 2:
                        ApplicantController.ViewCompanies();
                        break;
                    case 3:
                        ApplicantController.ContinuousSearch();
                        break;
                    case 4:
                        ApplicantController.UpdateApplicantInformation(loggedInUserId, username, role);
                        break;
                    case 5:
                        CvView.ShowCvMenu(loggedInUserId, username, role);
                        break;
                    case 6:
                        Console.WriteLine("Logging out...");
                        return;
                    default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
                }
            }
        }
    }
}