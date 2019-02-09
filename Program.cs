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
        static void Main(string[] args)
        {


            int again = 1;
            while (again == 1)
            {
                again = MainMenu();
            }
        }

        public static void PrintLn(string text)
        {
            Console.WriteLine(text);
        }


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
                DisplayStatsFootball();
                return 1;
            }
            else if (choice == "2")
            {
                DisplayStatsPrimary();
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

        public static void DisplayStatsFootball()
        {
            Console.Clear();
            PrintLn("********************* Football Conf. Menu **********************");
            var footballConferenceNames = GetField(5);
            PrintLn("Enter the number of your selection or 'B' to return to the main menu." + "\r\n");
            string footballConfSelection = Console.ReadLine();
            PrintLn(footballConfSelection);
        }

        public static void DisplayStatsPrimary()
        {
            Console.Clear();
            PrintLn("********************* Primanry Conf. Menu **********************");
            var footballConferenceNames = GetField(4);
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

        // Read and List Football ConferenceList Names from Data File
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
                int count = 1;
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
                            if (count < 10)
                            {
                                PrintLn(count.ToString() + "...." + values[field]);
                            }
                            else
                            {
                                PrintLn(count.ToString() + "..." + values[field]);
                            }
                            count++;
                        } 
                    }
                }
            }

            return fieldValues;


        }




    }

        


 }
