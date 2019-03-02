using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;

namespace ncaa_grad_info
{
    class Program
    {
        // MAIN
        static void Main(string[] args)
        {
            User currentUser = new User();
            currentUser.Session = 0;
            currentUser.LoggedIn = 0;

            while (currentUser.Session == 0)
            {
                while (currentUser.LoggedIn == 0)
                {
                    currentUser = loginMenu(currentUser);
                }

                int again = 1;
                while (again == 1 && currentUser.LoggedIn == 1)
                {
                    again = MainMenu();
                }
            }
        }

        // My PRINT function, because I'm lazy
        public static void PrintLn(string text)
        {
            Console.WriteLine(text);
        }

        // Supresses echoing the password
        public static string PSWDBlank()
        {
            StringBuilder passwordBuilder = new StringBuilder();
            bool continueReading = true;
            char newLineChar = '\r';
            while (continueReading)
            {
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                char passwordChar = consoleKeyInfo.KeyChar;

                if (passwordChar == newLineChar)
                {
                    continueReading = false;
                }
                else
                {
                    passwordBuilder.Append(passwordChar.ToString());
                }
            }

            return passwordBuilder.ToString();
        }





        public static string ComputeHash(string plainText)
        {
            // Generate a random number for the size of the salt.
            int minSaltSize = 4;
            int maxSaltSize = 8;
            Random random = new Random();
            int saltSize = random.Next(minSaltSize, maxSaltSize);
            byte[] saltBytes = new byte[saltSize];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(saltBytes);

            // Convert plain text into a byte array.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Allocate array, which will hold plain text and salt.
            byte[] plainTextWithSaltBytes =
                    new byte[plainTextBytes.Length + saltBytes.Length];

            // Copy plain text bytes into resulting array.
            for (int i = 0; i < plainTextBytes.Length; i++)
                plainTextWithSaltBytes[i] = plainTextBytes[i];

            // Append salt bytes to the resulting array.
            for (int i = 0; i < saltBytes.Length; i++)
                plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];

            HashAlgorithm hash = new SHA256Managed();

            // Compute hash value of our plain text with appended salt.
            byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

            // Create array which will hold hash and original salt bytes.
            byte[] hashWithSaltBytes = new byte[hashBytes.Length + saltBytes.Length];

            // Copy hash bytes into resulting array.
            for (int i = 0; i < hashBytes.Length; i++)
                hashWithSaltBytes[i] = hashBytes[i];

            // Append salt bytes to the result.
            for (int i = 0; i < saltBytes.Length; i++)
                hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];

            string hashValue = Convert.ToBase64String(hashWithSaltBytes);

            return hashValue;
        }






            // LOGIN Primary Menu
            public static User loginMenu(User currentUser)
        {
            Console.Clear();
            PrintLn("************************* LOGIN MENU **************************");
            PrintLn("1. LogIn");
            PrintLn("2. Register");
            
            PrintLn("");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                currentUser = LogMeIn(currentUser);
                return currentUser;
            }
            else if (choice == "2")
            {
                currentUser = RegisterMe(currentUser);
                return currentUser;
            }
            else
            {
                currentUser.Session = 0;
                return currentUser;
            }




        }

        // Log In Existing User
        private static User LogMeIn(User currentUser)
        {
            // create a new database connection:
            SQLiteConnection sqlite_conn = new SQLiteConnection("Data Source=user.db");

            // open the connection:
            sqlite_conn.Open();

            return currentUser;
        }

        // Register a new user
        private static User RegisterMe(User currentUser)
        {
            Console.Clear();
            PrintLn("*********************** Registration ************************");
            PrintLn("");

            string username = "";

            // Open the DB
            string sql ="";
            SQLiteConnection sqlite_conn = new SQLiteConnection("Data Source=D:\\projects\\csharp\\ncaa-grad-info\\ncaa-grad-info\\ncaa-grad-info\\user.db");
            sqlite_conn.Open();

            int userRepeat = 1;
            while (userRepeat == 1)
            {
                PrintLn("Enter a username: ");
                username = Console.ReadLine();
                if (username == "")
                {
                    PrintLn("");
                    PrintLn("That Username CANNOT be NULL!");
                    PrintLn("Press Any Key To Continue!");
                    Console.ReadKey();
                    userRepeat = 1;
                }
                else
                {
                    SQLiteCommand command = new SQLiteCommand(sql, sqlite_conn);
                    command.CommandText = "SELECT count(username) from users WHERE username = '" + username + "';";
                    command.CommandType = System.Data.CommandType.Text;
                    int RowCount = 0;
                    RowCount = Convert.ToInt32(command.ExecuteScalar());
                    if (RowCount > 0)
                    {
                        PrintLn("");
                        PrintLn("That Username Is Taken. Select Another!");
                        PrintLn("Press Any Key To Continue!");
                        Console.ReadKey();
                        userRepeat = 1;
                    }
                    else { userRepeat = 0; }
                }
            }

            PrintLn("Enter Your First Name: ");
            string nameFirst = Console.ReadLine();

            PrintLn("Enter Your Last Name: ");
            string nameLast = Console.ReadLine();

            string pswd = "";
            int nomatch = 1;
            while (nomatch == 1)
            {
                int pswdRepeat = 1;
                while (pswdRepeat == 1)
                {
                    PrintLn("Enter a Password (no echo): ");
                    pswd = PSWDBlank();

                    if (pswd == "")
                    {
                        PrintLn("");
                        PrintLn("The Pasword CANNOT be NULL!");
                        PrintLn("Press Any Key To Continue!");
                        Console.ReadKey();
                        pswdRepeat = 1;
                    }
                    else
                    {
                        pswdRepeat = 0;
                    }
                }

                PrintLn("Please, Confirm Your Password (no echo): ");
                string pswdConfirm = PSWDBlank();
                if(pswd == pswdConfirm)
                {
                    nomatch = 0;
                }
                else
                {
                    PrintLn("Passwords do not match. Try again.");
                    PrintLn("Press any key to continue.");
                    Console.ReadKey();
                    nomatch = 1;
                }

            }
            pswd = ComputeHash(pswd);

            string ffc = "temp";
            string fpc = "temp";
            
            // Save to the DB
            sql = "insert into users (username, NameFirst, NameLast, PSWDHash, FavFootballConf, FavPrimaryConf) values ('" + username + "', '" + nameFirst + "', '" + nameLast + "', '" + pswd + "', '" + ffc + "', '" + fpc + "');";
            SQLiteCommand addNewUser = new SQLiteCommand(sql, sqlite_conn);
            addNewUser.ExecuteNonQuery();
            sqlite_conn.Close();

            // Build the User
            currentUser.Username = username;
            currentUser.NameFirst = nameFirst;
            currentUser.NameLast = nameLast;
            currentUser.FavFootballConf = ffc;
            currentUser.FavPrimaryConf = fpc;
            currentUser.LoggedIn = 1;
            currentUser.Session = 1;

            return currentUser;
        }






        // Prints menu and interprets user's choice
        public static int MainMenu()
        {
            Console.Clear();
            PrintLn("************************* MAIN MENU **************************");
            PrintLn("1. Display Individual School Stats by Football Conference");
            PrintLn("2. Display Individual School Stats by Primary Conference");
            PrintLn("3. Display Favorite Conferences");
            PrintLn("...");
            PrintLn("6. Create Profile");
            PrintLn("7. Edit Profile");
            PrintLn("...");
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

            // Conference totals across member colleges
            NCAAConfData.FED_N_2011_SA = NCAACollegeData.Sum(item => item.FED_N_2011_SA) ;
            NCAAConfData.FED_N_2010_SA = NCAACollegeData.Sum(item => item.FED_N_2010_SA) ;
            NCAAConfData.FED_N_2009_SA = NCAACollegeData.Sum(item => item.FED_N_2009_SA) ;
            NCAAConfData.FED_N_2008_SA = NCAACollegeData.Sum(item => item.FED_N_2008_SA) ;
            NCAAConfData.FED_N_2007_SA = NCAACollegeData.Sum(item => item.FED_N_2007_SA) ;
            NCAAConfData.FED_N_2006_SA = NCAACollegeData.Sum(item => item.FED_N_2006_SA) ;
            NCAAConfData.FED_N_2005_SA = NCAACollegeData.Sum(item => item.FED_N_2005_SA) ;
            NCAAConfData.FED_N_2004_SA = NCAACollegeData.Sum(item => item.FED_N_2004_SA) ;
            NCAAConfData.FED_N_2003_SA = NCAACollegeData.Sum(item => item.FED_N_2003_SA) ;
            NCAAConfData.FED_N_2002_SA = NCAACollegeData.Sum(item => item.FED_N_2002_SA) ;
            NCAAConfData.FED_N_2001_SA = NCAACollegeData.Sum(item => item.FED_N_2001_SA) ;
            NCAAConfData.FED_N_2000_SA = NCAACollegeData.Sum(item => item.FED_N_2000_SA) ;
            NCAAConfData.FED_N_1999_SA = NCAACollegeData.Sum(item => item.FED_N_1999_SA) ;
            NCAAConfData.FED_N_1998_SA = NCAACollegeData.Sum(item => item.FED_N_1998_SA) ;
            NCAAConfData.FED_N_1997_SA = NCAACollegeData.Sum(item => item.FED_N_1997_SA) ;
            NCAAConfData.FED_N_1996_SA = NCAACollegeData.Sum(item => item.FED_N_1996_SA) ;
            NCAAConfData.FED_N_1995_SA = NCAACollegeData.Sum(item => item.FED_N_1995_SA) ;
            NCAAConfData.FED_RATE_2011_SA = NCAACollegeData.Sum(item => item.FED_RATE_2011_SA) ;
            NCAAConfData.FED_RATE_2010_SA = NCAACollegeData.Sum(item => item.FED_RATE_2010_SA) ;
            NCAAConfData.FED_RATE_2009_SA = NCAACollegeData.Sum(item => item.FED_RATE_2009_SA) ;
            NCAAConfData.FED_RATE_2008_SA = NCAACollegeData.Sum(item => item.FED_RATE_2008_SA) ;
            NCAAConfData.FED_RATE_2007_SA = NCAACollegeData.Sum(item => item.FED_RATE_2007_SA) ;
            NCAAConfData.FED_RATE_2006_SA = NCAACollegeData.Sum(item => item.FED_RATE_2006_SA) ;
            NCAAConfData.FED_RATE_2005_SA = NCAACollegeData.Sum(item => item.FED_RATE_2005_SA) ;
            NCAAConfData.FED_RATE_2004_SA = NCAACollegeData.Sum(item => item.FED_RATE_2004_SA) ;
            NCAAConfData.FED_RATE_2003_SA = NCAACollegeData.Sum(item => item.FED_RATE_2003_SA) ;
            NCAAConfData.FED_RATE_2002_SA = NCAACollegeData.Sum(item => item.FED_RATE_2002_SA) ;
            NCAAConfData.FED_RATE_2001_SA = NCAACollegeData.Sum(item => item.FED_RATE_2001_SA) ;
            NCAAConfData.FED_RATE_2000_SA = NCAACollegeData.Sum(item => item.FED_RATE_2000_SA) ;
            NCAAConfData.FED_RATE_1999_SA = NCAACollegeData.Sum(item => item.FED_RATE_1999_SA) ;
            NCAAConfData.FED_RATE_1998_SA = NCAACollegeData.Sum(item => item.FED_RATE_1998_SA) ;
            NCAAConfData.FED_RATE_1997_SA = NCAACollegeData.Sum(item => item.FED_RATE_1997_SA) ;
            NCAAConfData.FED_RATE_1996_SA = NCAACollegeData.Sum(item => item.FED_RATE_1996_SA) ;
            NCAAConfData.FED_RATE_1995_SA = NCAACollegeData.Sum(item => item.FED_RATE_1995_SA) ;                
            NCAAConfData.GSR_N_2011_SA = NCAACollegeData.Sum(item => item.GSR_N_2011_SA) ;
            NCAAConfData.GSR_N_2010_SA = NCAACollegeData.Sum(item => item.GSR_N_2010_SA) ;
            NCAAConfData.GSR_N_2009_SA = NCAACollegeData.Sum(item => item.GSR_N_2009_SA) ;
            NCAAConfData.GSR_N_2008_SA = NCAACollegeData.Sum(item => item.GSR_N_2008_SA) ;
            NCAAConfData.GSR_N_2007_SA = NCAACollegeData.Sum(item => item.GSR_N_2007_SA) ;
            NCAAConfData.GSR_N_2006_SA = NCAACollegeData.Sum(item => item.GSR_N_2006_SA) ;
            NCAAConfData.GSR_N_2005_SA = NCAACollegeData.Sum(item => item.GSR_N_2005_SA) ;
            NCAAConfData.GSR_N_2004_SA = NCAACollegeData.Sum(item => item.GSR_N_2004_SA) ;
            NCAAConfData.GSR_N_2003_SA = NCAACollegeData.Sum(item => item.GSR_N_2003_SA) ;
            NCAAConfData.GSR_N_2002_SA = NCAACollegeData.Sum(item => item.GSR_N_2002_SA) ;
            NCAAConfData.GSR_N_2001_SA = NCAACollegeData.Sum(item => item.GSR_N_2001_SA) ;
            NCAAConfData.GSR_N_2000_SA = NCAACollegeData.Sum(item => item.GSR_N_2000_SA) ;
            NCAAConfData.GSR_N_1999_SA = NCAACollegeData.Sum(item => item.GSR_N_1999_SA) ;
            NCAAConfData.GSR_N_1998_SA = NCAACollegeData.Sum(item => item.GSR_N_1998_SA) ;
            NCAAConfData.GSR_N_1997_SA = NCAACollegeData.Sum(item => item.GSR_N_1997_SA) ;
            NCAAConfData.GSR_N_1996_SA = NCAACollegeData.Sum(item => item.GSR_N_1996_SA) ;
            NCAAConfData.GSR_N_1995_SA = NCAACollegeData.Sum(item => item.GSR_N_1995_SA) ;
            NCAAConfData.GSR_2011_SA = NCAACollegeData.Sum(item => item.GSR_2011_SA) ;
            NCAAConfData.GSR_2010_SA = NCAACollegeData.Sum(item => item.GSR_2010_SA) ;
            NCAAConfData.GSR_2009_SA = NCAACollegeData.Sum(item => item.GSR_2009_SA) ;
            NCAAConfData.GSR_2008_SA = NCAACollegeData.Sum(item => item.GSR_2008_SA) ;
            NCAAConfData.GSR_2007_SA = NCAACollegeData.Sum(item => item.GSR_2007_SA) ;
            NCAAConfData.GSR_2006_SA = NCAACollegeData.Sum(item => item.GSR_2006_SA) ;
            NCAAConfData.GSR_2005_SA = NCAACollegeData.Sum(item => item.GSR_2005_SA) ;
            NCAAConfData.GSR_2004_SA = NCAACollegeData.Sum(item => item.GSR_2004_SA) ;
            NCAAConfData.GSR_2003_SA = NCAACollegeData.Sum(item => item.GSR_2003_SA) ;
            NCAAConfData.GSR_2002_SA = NCAACollegeData.Sum(item => item.GSR_2002_SA) ;
            NCAAConfData.GSR_2001_SA = NCAACollegeData.Sum(item => item.GSR_2001_SA) ;
            NCAAConfData.GSR_2000_SA = NCAACollegeData.Sum(item => item.GSR_2000_SA) ;
            NCAAConfData.GSR_1999_SA = NCAACollegeData.Sum(item => item.GSR_1999_SA) ;
            NCAAConfData.GSR_1998_SA = NCAACollegeData.Sum(item => item.GSR_1998_SA) ;
            NCAAConfData.GSR_1997_SA = NCAACollegeData.Sum(item => item.GSR_1997_SA) ;
            NCAAConfData.GSR_1996_SA = NCAACollegeData.Sum(item => item.GSR_1996_SA) ;
            NCAAConfData.GSR_1995_SA = NCAACollegeData.Sum(item => item.GSR_1995_SA) ;

            // Conference averages across member colleges
            NCAAConfData.AVG_FED_N_2011_SA = NCAACollegeData.Average(item => item.FED_N_2011_SA);
            NCAAConfData.AVG_FED_N_2010_SA = NCAACollegeData.Average(item => item.FED_N_2010_SA);
            NCAAConfData.AVG_FED_N_2009_SA = NCAACollegeData.Average(item => item.FED_N_2009_SA);
            NCAAConfData.AVG_FED_N_2008_SA = NCAACollegeData.Average(item => item.FED_N_2008_SA);
            NCAAConfData.AVG_FED_N_2007_SA = NCAACollegeData.Average(item => item.FED_N_2007_SA);
            NCAAConfData.AVG_FED_N_2006_SA = NCAACollegeData.Average(item => item.FED_N_2006_SA);
            NCAAConfData.AVG_FED_N_2005_SA = NCAACollegeData.Average(item => item.FED_N_2005_SA);
            NCAAConfData.AVG_FED_N_2004_SA = NCAACollegeData.Average(item => item.FED_N_2004_SA);
            NCAAConfData.AVG_FED_N_2003_SA = NCAACollegeData.Average(item => item.FED_N_2003_SA);
            NCAAConfData.AVG_FED_N_2002_SA = NCAACollegeData.Average(item => item.FED_N_2002_SA);
            NCAAConfData.AVG_FED_N_2001_SA = NCAACollegeData.Average(item => item.FED_N_2001_SA);
            NCAAConfData.AVG_FED_N_2000_SA = NCAACollegeData.Average(item => item.FED_N_2000_SA);
            NCAAConfData.AVG_FED_N_1999_SA = NCAACollegeData.Average(item => item.FED_N_1999_SA);
            NCAAConfData.AVG_FED_N_1998_SA = NCAACollegeData.Average(item => item.FED_N_1998_SA);
            NCAAConfData.AVG_FED_N_1997_SA = NCAACollegeData.Average(item => item.FED_N_1997_SA);
            NCAAConfData.AVG_FED_N_1996_SA = NCAACollegeData.Average(item => item.FED_N_1996_SA);
            NCAAConfData.AVG_FED_N_1995_SA = NCAACollegeData.Average(item => item.FED_N_1995_SA);
            NCAAConfData.AVG_FED_RATE_2011_SA = NCAACollegeData.Average(item => item.FED_RATE_2011_SA);
            NCAAConfData.AVG_FED_RATE_2010_SA = NCAACollegeData.Average(item => item.FED_RATE_2010_SA);
            NCAAConfData.AVG_FED_RATE_2009_SA = NCAACollegeData.Average(item => item.FED_RATE_2009_SA);
            NCAAConfData.AVG_FED_RATE_2008_SA = NCAACollegeData.Average(item => item.FED_RATE_2008_SA);
            NCAAConfData.AVG_FED_RATE_2007_SA = NCAACollegeData.Average(item => item.FED_RATE_2007_SA);
            NCAAConfData.AVG_FED_RATE_2006_SA = NCAACollegeData.Average(item => item.FED_RATE_2006_SA);
            NCAAConfData.AVG_FED_RATE_2005_SA = NCAACollegeData.Average(item => item.FED_RATE_2005_SA);
            NCAAConfData.AVG_FED_RATE_2004_SA = NCAACollegeData.Average(item => item.FED_RATE_2004_SA);
            NCAAConfData.AVG_FED_RATE_2003_SA = NCAACollegeData.Average(item => item.FED_RATE_2003_SA);
            NCAAConfData.AVG_FED_RATE_2002_SA = NCAACollegeData.Average(item => item.FED_RATE_2002_SA);
            NCAAConfData.AVG_FED_RATE_2001_SA = NCAACollegeData.Average(item => item.FED_RATE_2001_SA);
            NCAAConfData.AVG_FED_RATE_2000_SA = NCAACollegeData.Average(item => item.FED_RATE_2000_SA);
            NCAAConfData.AVG_FED_RATE_1999_SA = NCAACollegeData.Average(item => item.FED_RATE_1999_SA);
            NCAAConfData.AVG_FED_RATE_1998_SA = NCAACollegeData.Average(item => item.FED_RATE_1998_SA);
            NCAAConfData.AVG_FED_RATE_1997_SA = NCAACollegeData.Average(item => item.FED_RATE_1997_SA);
            NCAAConfData.AVG_FED_RATE_1996_SA = NCAACollegeData.Average(item => item.FED_RATE_1996_SA);
            NCAAConfData.AVG_FED_RATE_1995_SA = NCAACollegeData.Average(item => item.FED_RATE_1995_SA);
            NCAAConfData.AVG_GSR_N_2011_SA = NCAACollegeData.Average(item => item.GSR_N_2011_SA);
            NCAAConfData.AVG_GSR_N_2010_SA = NCAACollegeData.Average(item => item.GSR_N_2010_SA);
            NCAAConfData.AVG_GSR_N_2009_SA = NCAACollegeData.Average(item => item.GSR_N_2009_SA);
            NCAAConfData.AVG_GSR_N_2008_SA = NCAACollegeData.Average(item => item.GSR_N_2008_SA);
            NCAAConfData.AVG_GSR_N_2007_SA = NCAACollegeData.Average(item => item.GSR_N_2007_SA);
            NCAAConfData.AVG_GSR_N_2006_SA = NCAACollegeData.Average(item => item.GSR_N_2006_SA);
            NCAAConfData.AVG_GSR_N_2005_SA = NCAACollegeData.Average(item => item.GSR_N_2005_SA);
            NCAAConfData.AVG_GSR_N_2004_SA = NCAACollegeData.Average(item => item.GSR_N_2004_SA);
            NCAAConfData.AVG_GSR_N_2003_SA = NCAACollegeData.Average(item => item.GSR_N_2003_SA);
            NCAAConfData.AVG_GSR_N_2002_SA = NCAACollegeData.Average(item => item.GSR_N_2002_SA);
            NCAAConfData.AVG_GSR_N_2001_SA = NCAACollegeData.Average(item => item.GSR_N_2001_SA);
            NCAAConfData.AVG_GSR_N_2000_SA = NCAACollegeData.Average(item => item.GSR_N_2000_SA);
            NCAAConfData.AVG_GSR_N_1999_SA = NCAACollegeData.Average(item => item.GSR_N_1999_SA);
            NCAAConfData.AVG_GSR_N_1998_SA = NCAACollegeData.Average(item => item.GSR_N_1998_SA);
            NCAAConfData.AVG_GSR_N_1997_SA = NCAACollegeData.Average(item => item.GSR_N_1997_SA);
            NCAAConfData.AVG_GSR_N_1996_SA = NCAACollegeData.Average(item => item.GSR_N_1996_SA);
            NCAAConfData.AVG_GSR_N_1995_SA = NCAACollegeData.Average(item => item.GSR_N_1995_SA);
            NCAAConfData.AVG_GSR_2011_SA = NCAACollegeData.Average(item => item.GSR_2011_SA);
            NCAAConfData.AVG_GSR_2010_SA = NCAACollegeData.Average(item => item.GSR_2010_SA);
            NCAAConfData.AVG_GSR_2009_SA = NCAACollegeData.Average(item => item.GSR_2009_SA);
            NCAAConfData.AVG_GSR_2008_SA = NCAACollegeData.Average(item => item.GSR_2008_SA);
            NCAAConfData.AVG_GSR_2007_SA = NCAACollegeData.Average(item => item.GSR_2007_SA);
            NCAAConfData.AVG_GSR_2006_SA = NCAACollegeData.Average(item => item.GSR_2006_SA);
            NCAAConfData.AVG_GSR_2005_SA = NCAACollegeData.Average(item => item.GSR_2005_SA);
            NCAAConfData.AVG_GSR_2004_SA = NCAACollegeData.Average(item => item.GSR_2004_SA);
            NCAAConfData.AVG_GSR_2003_SA = NCAACollegeData.Average(item => item.GSR_2003_SA);
            NCAAConfData.AVG_GSR_2002_SA = NCAACollegeData.Average(item => item.GSR_2002_SA);
            NCAAConfData.AVG_GSR_2001_SA = NCAACollegeData.Average(item => item.GSR_2001_SA);
            NCAAConfData.AVG_GSR_2000_SA = NCAACollegeData.Average(item => item.GSR_2000_SA);
            NCAAConfData.AVG_GSR_1999_SA = NCAACollegeData.Average(item => item.GSR_1999_SA);
            NCAAConfData.AVG_GSR_1998_SA = NCAACollegeData.Average(item => item.GSR_1998_SA);
            NCAAConfData.AVG_GSR_1997_SA = NCAACollegeData.Average(item => item.GSR_1997_SA);
            NCAAConfData.AVG_GSR_1996_SA = NCAACollegeData.Average(item => item.GSR_1996_SA);
            NCAAConfData.AVG_GSR_1995_SA = NCAACollegeData.Average(item => item.GSR_1995_SA);

            return NCAAConfData;
        }

        // Dispay Aggregate Statistics for Conference
        public static void DisplayConfData(Conference NCAAConfData)
        {
            Console.Clear();
            PrintLn("******************** " + NCAAConfData.ChosenConf + " ********************");
            
            foreach (PropertyInfo prop in typeof(Conference).GetProperties())
            {
                Console.WriteLine("{0} = {1}", prop.Name, prop.GetValue(NCAAConfData, null));
            }

        }

    }
}
