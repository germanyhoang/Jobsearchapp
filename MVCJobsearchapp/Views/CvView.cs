using System;
using MVCJobSearchApp.Controllers;

namespace MVCJobSearchApp.Views
{
    public static class CvView
    {
        public static void ShowCvMenu(int loggedInUserId, string username, string role)
        {
            while (true)
            {
                Console.WriteLine("==============================================================");
                Console.WriteLine("||                                                          ||");
                Console.WriteLine("||                       CV MENU                            ||");
                Console.WriteLine("||                                                          ||");
                Console.WriteLine("==============================================================");
                Console.WriteLine("|| 1. View CV                                               ||");
                Console.WriteLine("|| 2. Create CV                                             ||");
                Console.WriteLine("|| 3. Update CV                                             ||");
                Console.WriteLine("|| 4. Delete Cv                                             ||");
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
                        CvController.ViewCV(Program.loggedInUserId);
                        break;
                    case 2:
                        CvController.CreateCV(Program.loggedInUserId);
                        break;
                    case 3:
                        CvController.UpdateCV(Program.loggedInUserId);
                        break;
                    case 4:
                        CvController.DeleteCV(Program.loggedInUserId);
                        break;
                    case 5:
                        Console.WriteLine("Logging out...");
                        return;
                }
            }
        }
    }
}