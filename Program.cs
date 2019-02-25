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

            else if (number <= maxValue && number > 0)
            {
                int confType = 5;
                PrintLn(footballConfList.ElementAt(number - 1));
                string selectedConf = footballConfList.ElementAt(number - 1);


                string currentDirectory = Directory.GetCurrentDirectory();
                DirectoryInfo directory = new DirectoryInfo(currentDirectory);
                var fileName = Path.Combine(directory.FullName, "ncaadata.csv");
                List<College> NCAACollegeData = ReadCollegeData(fileName, selectedConf, confType);

                Conference NCAAConfData = AggregateConfData(NCAACollegeData, selectedConf);

                DisplayConfData(NCAAConfData);

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

        // Load CSV data into College Classes and adds then to a List<College>
        public static List<College> ReadCollegeData(string fileName, string selectedConf, int confType)
        {

            var NCAACollegeData = new List<College>();
            using (var reader = new StreamReader(fileName))
            {
                string line = "";
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var college = new College();
                    string[] values = line.Split(',');

                    // Identification Information
                    college.SchoolID = values[0];
                    college.SchoolName = values[1];
                    college.SchoolDivision = values[2];
                    college.SchoolSubdivision = values[3];
                    college.SchoolPrimaryConf = values[4];
                    college.SchoolFootballConf = values[5];
                    college.SchoolPrivate = values[7];

                    int parseInt;
                    // FEDERAL Population per year
                    if (int.TryParse(values[53], out parseInt))
                    {
                        college.FED_N_2011_SA = parseInt;
                    }
                    if (int.TryParse(values[54], out parseInt))
                    {
                        college.FED_N_2010_SA = parseInt;
                    }
                    if (int.TryParse(values[55], out parseInt))
                    {
                        college.FED_N_2009_SA = parseInt;
                    }
                    if (int.TryParse(values[56], out parseInt))
                    {
                        college.FED_N_2008_SA = parseInt;
                    }
                    if (int.TryParse(values[57], out parseInt))
                    {
                        college.FED_N_2007_SA = parseInt;
                    }
                    if (int.TryParse(values[58], out parseInt))
                    {
                        college.FED_N_2006_SA = parseInt;
                    }
                    if (int.TryParse(values[59], out parseInt))
                    {
                        college.FED_N_2005_SA = parseInt;
                    }
                    if (int.TryParse(values[60], out parseInt))
                    {
                        college.FED_N_2004_SA = parseInt;
                    }
                    if (int.TryParse(values[61], out parseInt))
                    {
                        college.FED_N_2003_SA = parseInt;
                    }
                    if (int.TryParse(values[62], out parseInt))
                    {
                        college.FED_N_2002_SA = parseInt;
                    }
                    if (int.TryParse(values[63], out parseInt))
                    {
                        college.FED_N_2001_SA = parseInt;
                    }
                    if (int.TryParse(values[64], out parseInt))
                    {
                        college.FED_N_2000_SA = parseInt;
                    }
                    if (int.TryParse(values[65], out parseInt))
                    {
                        college.FED_N_1999_SA = parseInt;
                    }
                    if (int.TryParse(values[66], out parseInt))
                    {
                        college.FED_N_1998_SA = parseInt;
                    }
                    if (int.TryParse(values[67], out parseInt))
                    {
                        college.FED_N_1997_SA = parseInt;
                    }
                    if (int.TryParse(values[68], out parseInt))
                    {
                        college.FED_N_1996_SA = parseInt;
                    }
                    if (int.TryParse(values[69], out parseInt))
                    {
                        college.FED_N_1995_SA = parseInt;
                    }

                    // FEDERAL Graduation Rate per year
                    if (int.TryParse(values[70], out parseInt))
                    {
                        college.FED_RATE_2011_SA = parseInt;
                    }
                    if (int.TryParse(values[71], out parseInt))
                    {
                        college.FED_RATE_2010_SA = parseInt;
                    }
                    if (int.TryParse(values[72], out parseInt))
                    {
                        college.FED_RATE_2009_SA = parseInt;
                    }
                    if (int.TryParse(values[73], out parseInt))
                    {
                        college.FED_RATE_2008_SA = parseInt;
                    }
                    if (int.TryParse(values[74], out parseInt))
                    {
                        college.FED_RATE_2007_SA = parseInt;
                    }
                    if (int.TryParse(values[75], out parseInt))
                    {
                        college.FED_RATE_2006_SA = parseInt;
                    }
                    if (int.TryParse(values[76], out parseInt))
                    {
                        college.FED_RATE_2005_SA = parseInt;
                    }
                    if (int.TryParse(values[77], out parseInt))
                    {
                        college.FED_RATE_2004_SA = parseInt;
                    }
                    if (int.TryParse(values[78], out parseInt))
                    {
                        college.FED_RATE_2003_SA = parseInt;
                    }
                    if (int.TryParse(values[79], out parseInt))
                    {
                        college.FED_RATE_2002_SA = parseInt;
                    }
                    if (int.TryParse(values[80], out parseInt))
                    {
                        college.FED_RATE_2001_SA = parseInt;
                    }
                    if (int.TryParse(values[81], out parseInt))
                    {
                        college.FED_RATE_2000_SA = parseInt;
                    }
                    if (int.TryParse(values[82], out parseInt))
                    {
                        college.FED_RATE_1999_SA = parseInt;
                    }
                    if (int.TryParse(values[83], out parseInt))
                    {
                        college.FED_RATE_1998_SA = parseInt;
                    }
                    if (int.TryParse(values[84], out parseInt))
                    {
                        college.FED_RATE_1997_SA = parseInt;
                    }
                    if (int.TryParse(values[85], out parseInt))
                    {
                        college.FED_RATE_1996_SA = parseInt;
                    }
                    if (int.TryParse(values[86], out parseInt))
                    {
                        college.FED_RATE_1995_SA = parseInt;
                    }

                    // NCAA calculated Population per year
                    if (int.TryParse(values[53], out parseInt))
                    {
                        college.GSR_N_2011_SA = parseInt;
                    }
                    if (int.TryParse(values[54], out parseInt))
                    {
                        college.GSR_N_2010_SA = parseInt;
                    }
                    if (int.TryParse(values[55], out parseInt))
                    {
                        college.GSR_N_2009_SA = parseInt;
                    }
                    if (int.TryParse(values[56], out parseInt))
                    {
                        college.GSR_N_2008_SA = parseInt;
                    }
                    if (int.TryParse(values[57], out parseInt))
                    {
                        college.GSR_N_2007_SA = parseInt;
                    }
                    if (int.TryParse(values[58], out parseInt))
                    {
                        college.GSR_N_2006_SA = parseInt;
                    }
                    if (int.TryParse(values[59], out parseInt))
                    {
                        college.GSR_N_2005_SA = parseInt;
                    }
                    if (int.TryParse(values[60], out parseInt))
                    {
                        college.GSR_N_2004_SA = parseInt;
                    }
                    if (int.TryParse(values[61], out parseInt))
                    {
                        college.GSR_N_2003_SA = parseInt;
                    }
                    if (int.TryParse(values[62], out parseInt))
                    {
                        college.GSR_N_2002_SA = parseInt;
                    }
                    if (int.TryParse(values[63], out parseInt))
                    {
                        college.GSR_N_2001_SA = parseInt;
                    }
                    if (int.TryParse(values[64], out parseInt))
                    {
                        college.GSR_N_2000_SA = parseInt;
                    }
                    if (int.TryParse(values[65], out parseInt))
                    {
                        college.GSR_N_1999_SA = parseInt;
                    }
                    if (int.TryParse(values[66], out parseInt))
                    {
                        college.GSR_N_1998_SA = parseInt;
                    }
                    if (int.TryParse(values[67], out parseInt))
                    {
                        college.GSR_N_1997_SA = parseInt;
                    }
                    if (int.TryParse(values[68], out parseInt))
                    {
                        college.GSR_N_1996_SA = parseInt;
                    }
                    if (int.TryParse(values[69], out parseInt))
                    {
                        college.GSR_N_1995_SA = parseInt;
                    }

                    // NCAA calulated Greaduation Rate per year
                    if (int.TryParse(values[70], out parseInt))
                    {
                        college.GSR_2011_SA = parseInt;
                    }
                    if (int.TryParse(values[71], out parseInt))
                    {
                        college.GSR_2010_SA = parseInt;
                    }
                    if (int.TryParse(values[72], out parseInt))
                    {
                        college.GSR_2009_SA = parseInt;
                    }
                    if (int.TryParse(values[73], out parseInt))
                    {
                        college.GSR_2008_SA = parseInt;
                    }
                    if (int.TryParse(values[74], out parseInt))
                    {
                        college.GSR_2007_SA = parseInt;
                    }
                    if (int.TryParse(values[75], out parseInt))
                    {
                        college.GSR_2006_SA = parseInt;
                    }
                    if (int.TryParse(values[76], out parseInt))
                    {
                        college.GSR_2005_SA = parseInt;
                    }
                    if (int.TryParse(values[77], out parseInt))
                    {
                        college.GSR_2004_SA = parseInt;
                    }
                    if (int.TryParse(values[78], out parseInt))
                    {
                        college.GSR_2003_SA = parseInt;
                    }
                    if (int.TryParse(values[79], out parseInt))
                    {
                        college.GSR_2002_SA = parseInt;
                    }
                    if (int.TryParse(values[80], out parseInt))
                    {
                        college.GSR_2001_SA = parseInt;
                    }
                    if (int.TryParse(values[81], out parseInt))
                    {
                        college.GSR_2000_SA = parseInt;
                    }
                    if (int.TryParse(values[82], out parseInt))
                    {
                        college.GSR_1999_SA = parseInt;
                    }
                    if (int.TryParse(values[83], out parseInt))
                    {
                        college.GSR_1998_SA = parseInt;
                    }
                    if (int.TryParse(values[84], out parseInt))
                    {
                        college.GSR_1997_SA = parseInt;
                    }
                    if (int.TryParse(values[85], out parseInt))
                    {
                        college.GSR_1996_SA = parseInt;
                    }
                    if (int.TryParse(values[86], out parseInt))
                    {
                        college.GSR_1995_SA = parseInt;
                    }


                    if (values[confType] == selectedConf)
                    {
                        NCAACollegeData.Add(college);
                    }
                }

            }
            return NCAACollegeData;
        }

        // Aggregates data from individual colleges and calculates stats from Conference
        public static Conference AggregateConfData(List<College> NCAACollegeData, string footballConfSelection)
        {
            var NCAAConfData = new Conference();
            NCAAConfData.ChosenConf = footballConfSelection;

            NCAAConfData.FED_N_2011_SA = 0 ;
            NCAAConfData.FED_N_2010_SA = 0 ;
            NCAAConfData.FED_N_2009_SA = 0 ;
            NCAAConfData.FED_N_2008_SA = 0 ;
            NCAAConfData.FED_N_2007_SA = 0 ;
            NCAAConfData.FED_N_2006_SA = 0 ;
            NCAAConfData.FED_N_2005_SA = 0 ;
            NCAAConfData.FED_N_2004_SA = 0 ;
            NCAAConfData.FED_N_2003_SA = 0 ;
            NCAAConfData.FED_N_2002_SA = 0 ;
            NCAAConfData.FED_N_2001_SA = 0 ;
            NCAAConfData.FED_N_2000_SA = 0 ;
            NCAAConfData.FED_N_1999_SA = 0 ;
            NCAAConfData.FED_N_1998_SA = 0 ;
            NCAAConfData.FED_N_1997_SA = 0 ;
            NCAAConfData.FED_N_1996_SA = 0 ;
            NCAAConfData.FED_N_1995_SA = 0 ;
            NCAAConfData.FED_RATE_2011_SA = 0 ;
            NCAAConfData.FED_RATE_2010_SA = 0 ;
            NCAAConfData.FED_RATE_2009_SA = 0 ;
            NCAAConfData.FED_RATE_2008_SA = 0 ;
            NCAAConfData.FED_RATE_2007_SA = 0 ;
            NCAAConfData.FED_RATE_2006_SA = 0 ;
            NCAAConfData.FED_RATE_2005_SA = 0 ;
            NCAAConfData.FED_RATE_2004_SA = 0 ;
            NCAAConfData.FED_RATE_2003_SA = 0 ;
            NCAAConfData.FED_RATE_2002_SA = 0 ;
            NCAAConfData.FED_RATE_2001_SA = 0 ;
            NCAAConfData.FED_RATE_2000_SA = 0 ;
            NCAAConfData.FED_RATE_1999_SA = 0 ;
            NCAAConfData.FED_RATE_1998_SA = 0 ;
            NCAAConfData.FED_RATE_1997_SA = 0 ;
            NCAAConfData.FED_RATE_1996_SA = 0 ;
            NCAAConfData.FED_RATE_1995_SA = 0 ;                
            NCAAConfData.GSR_N_2011_SA = 0 ;
            NCAAConfData.GSR_N_2010_SA = 0 ;
            NCAAConfData.GSR_N_2009_SA = 0 ;
            NCAAConfData.GSR_N_2008_SA = 0 ;
            NCAAConfData.GSR_N_2007_SA = 0 ;
            NCAAConfData.GSR_N_2006_SA = 0 ;
            NCAAConfData.GSR_N_2005_SA = 0 ;
            NCAAConfData.GSR_N_2004_SA = 0 ;
            NCAAConfData.GSR_N_2003_SA = 0 ;
            NCAAConfData.GSR_N_2002_SA = 0 ;
            NCAAConfData.GSR_N_2001_SA = 0 ;
            NCAAConfData.GSR_N_2000_SA = 0 ;
            NCAAConfData.GSR_N_1999_SA = 0 ;
            NCAAConfData.GSR_N_1998_SA = 0 ;
            NCAAConfData.GSR_N_1997_SA = 0 ;
            NCAAConfData.GSR_N_1996_SA = 0 ;
            NCAAConfData.GSR_N_1995_SA = 0 ;
            NCAAConfData.GSR_2011_SA = 0 ;
            NCAAConfData.GSR_2010_SA = 0 ;
            NCAAConfData.GSR_2009_SA = 0 ;
            NCAAConfData.GSR_2008_SA = 0 ;
            NCAAConfData.GSR_2007_SA = 0 ;
            NCAAConfData.GSR_2006_SA = 0 ;
            NCAAConfData.GSR_2005_SA = 0 ;
            NCAAConfData.GSR_2004_SA = 0 ;
            NCAAConfData.GSR_2003_SA = 0 ;
            NCAAConfData.GSR_2002_SA = 0 ;
            NCAAConfData.GSR_2001_SA = 0 ;
            NCAAConfData.GSR_2000_SA = 0 ;
            NCAAConfData.GSR_1999_SA = 0 ;
            NCAAConfData.GSR_1998_SA = 0 ;
            NCAAConfData.GSR_1997_SA = 0 ;
            NCAAConfData.GSR_1996_SA = 0 ;
            NCAAConfData.GSR_1995_SA = 0 ;

            










            return NCAAConfData;
        }

        // Dispay Aggregate Statistics for Conference
        public static void DisplayConfData(Conference NCAAConfData)
        {
            Console.Clear();
            PrintLn("******************** " + NCAAConfData.ChosenConf + " ********************");
            PrintLn("Average Student Athlete Population (Federal Cohort): " );
        }

    }
}
