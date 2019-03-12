using System;
using System.Collections.Generic;
using System.Linq;


namespace ncaa_grad_info
{
    class Program
    {
        // MAIN
        static void Main(string[] args)
        {
            // INITIALIZAION
            // Array of settings
            string[] settings = { "Data Source=.\\user.db", "SHA256", "8" };
            // Instantiates the User 
            User currentUser = new User();
            currentUser.Session = 0;
            currentUser.LoggedIn = 0;
            // Caches conference lists
            var confLists = ConfMgmt.BuildConfLists();

            while (currentUser.Session == 0)
            {
                while (currentUser.LoggedIn == 0)
                {
                    currentUser = UserMgmt.LoginMenu(currentUser, confLists, settings);
                }

                int again = 1;
                while (again == 1 && currentUser.LoggedIn == 1)
                {
                    again = MainMenu(currentUser, confLists, settings);
                }
            }
        }


        // PRIMARY FUNCTIONALITY
        // Prints menu and interprets user's choice
        public static int MainMenu(User currentUser, List<List<string>> confLists, string[] settings)
        {
            Console.Clear();
            Console.WriteLine("************************* MAIN MENU **************************");
            Console.WriteLine("1. Display Individual School Stats by Football Conference");
            Console.WriteLine("2. Display Individual School Stats by Primary Conference");
            Console.WriteLine("3. Display Favorite Football Conference");
            Console.WriteLine("4. Display Favorite Primary Conference");
            Console.WriteLine("...");
            Console.WriteLine("6. Edit Profile");
            Console.WriteLine("7. Delete Profile");
            Console.WriteLine("...");
            Console.WriteLine("9. Quit");
            Console.WriteLine("");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                ConfMgmt.GetStats(5, confLists);
                return 1;
            }
            else if (choice == "2")
            {
                ConfMgmt.GetStats(4, confLists);
                return 1;
            }
            else if (choice == "3")
            {
                ConfMgmt.GetStats(5, currentUser.FavFootballConf, confLists);
                return 1;
            }
            else if (choice == "4")
            {
                ConfMgmt.GetStats(4, currentUser.FavPrimaryConf, confLists);
                return 1;
            }
            else if(choice =="6")
            {
                UserMgmt.EditUser(currentUser, confLists, settings);
                return 1;
            }
            else if (choice == "7")
            {
                UserMgmt.DeleteUser(currentUser, settings);
                return 0;
            }
            else if (choice == "9")
            {
                Console.WriteLine("Goodbye");
                return 0;
            }
            else
            {
                return 1;
            }

        }

        // Generates and displays a list of Football conferences from the CSV
        public static string GetConf(int conf, List<List<string>> confLists )
        {
            int confIndex = 1;
            if(conf == 5)
            {
                confIndex = 0;
            }

            string confTypeStr = "Primary";
            if(conf == 5)
            {
                confTypeStr = "Football";
            }

                Console.Clear();
                Console.WriteLine("********************* " + confTypeStr + " Conf. Menu **********************");

                List<string> footballConfList = PrintSubMenu(confLists[confIndex]);

                int maxValue = footballConfList.Count();
                Console.WriteLine("Enter the number of your selection." + "\r\n");
                string footballConfSelection = Console.ReadLine();

                Int32.TryParse(footballConfSelection, out int number);
                if (number <= maxValue && number > 0)
                {
                    string selectedConf = footballConfList.ElementAt(number - 1);
                    return selectedConf;
                    
                }
                else
                {
                    Console.WriteLine("I do not understand. Let's try that again...");
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();

                    return "1";
                }
                
        }

        // Prints SubMenues with a selection number
        public static List<string> PrintSubMenu(List<string> fieldValues)
        {
            int count = 1;
            foreach (string fieldValue in fieldValues)
            {
                if (count < 10)
                {
                    Console.WriteLine(count.ToString() + "...." + fieldValue);
                }
                else
                {
                    Console.WriteLine(count.ToString() + "..." + fieldValue);
                }
                count++;
            }

            return fieldValues;
        }
    }
}
