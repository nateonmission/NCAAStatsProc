using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;

namespace ncaa_grad_info
{
    class Program
    {
        // MAIN
        static void Main(string[] args)
        {
            int again = 1;
            while (again == 1)
            {
                again = MainMenu();
            }
        }

        // My PRINT function, because I'm lazy
        public static void PrintLn(string text)
        {
            Console.WriteLine(text);
        }

        // Prints menu and interprets user's choice
        public static int MainMenu()
        {
            Console.Clear();
            PrintLn("************************* MAIN MENU **************************");
            PrintLn("1. Display Individual School Stats by Football ConferenceList");
            PrintLn("2. Display Individual School Stats by Primary ConferenceList");
            PrintLn("9. Quit");
            PrintLn("");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                DisplayMenuFootball();
                return 1;
            }
            else if (choice == "2")
            {
                DisplayMenuPrimary();
                return 1;
            }
            else if (choice == "9")
            {
                PrintLn("Goodbye");
                return 0;
            }
            else
            {
                return 1;
            }

        }

        // Generates and displays a list of Football conferences from the CSV
        public static int DisplayMenuFootball()
        {
            Console.Clear();
            PrintLn("********************* Football Conf. Menu **********************");
            List<string> footballConfList = PrintSubMenu(GetField(5));
            int maxValue = footballConfList.Count();
            PrintLn("Enter the number of your selection or 'B' to return to the main menu." + "\r\n");
            string footballConfSelection = Console.ReadLine();
            Int32.TryParse(footballConfSelection, out int number);
            if (footballConfSelection.ToUpper() == "B")
            {
                return 0;
            }

            else if(number <= maxValue && number > 0)
            {
                PrintLn(footballConfList.ElementAt(number - 1));
                string selectedConf = footballConfList.ElementAt(number - 1);
                Console.ReadKey();
                return 0;
            }
            else
            {
                PrintLn("I do not understand. Let's try that again...");
                PrintLn("Press any key to continue");
                Console.ReadKey();
                return 0;
            }
                   
        }

        // Generates and displays a list of Primary conferences from the CSV
        public static void DisplayMenuPrimary()
        {
            Console.Clear();
            PrintLn("********************* Primanry Conf. Menu **********************");
            PrintSubMenu(GetField(4));
            PrintLn("Enter the number of your selection or 'B' to return to the main menu." + "\r\n");
            string primaryConfSelection = Console.ReadLine();
            PrintLn(primaryConfSelection);
        }


        // FileReader
        public static string ReadFile(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                return reader.ReadToEnd();
            }
        }

        // Reads and returns a List<string> of all entries from one column in CSV without repeats or empty strings
        public static List<string> GetField(int field)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directory = new DirectoryInfo(currentDirectory);
            var fileName = Path.Combine(directory.FullName, "ncaadata.csv");

            var fieldValues = new List<string>();
            using (var reader = new StreamReader(fileName))
            {

                string line = "";
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');

                    if (values[field] == "")
                    {
                        continue;
                    }
                    else
                    {
                        if (!fieldValues.Contains(values[field]))
                        {
                            fieldValues.Add(values[field]);
                           
                        } 
                    }
                }
            }

            return fieldValues;
        }

        // Prints SubMenues with a selection number
        public static List<string> PrintSubMenu(List<string> fieldValues)
        {
            int count = 1;
            foreach (string fieldValue in fieldValues)
            {
                if (count < 10)
                {
                    PrintLn(count.ToString() + "...." + fieldValue);
                }
                else
                {
                    PrintLn(count.ToString() + "..." + fieldValue);
                }
                count++;
            }

            return fieldValues;
        }



    }

        


 }
