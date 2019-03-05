using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ncaa_grad_info
{
    class Program
    {
        // MAIN
        static void Main(string[] args)
        {
            string sqlPath = "Data Source=.\\user.db";
            User currentUser = new User();
            currentUser.Session = 0;
            currentUser.LoggedIn = 0;

            while (currentUser.Session == 0)
            {
                while (currentUser.LoggedIn == 0)
                {
                    currentUser = LoginMenu(currentUser, sqlPath);
                }

                int again = 1;
                while (again == 1 && currentUser.LoggedIn == 1)
                {
                    again = MainMenu(currentUser, sqlPath);
                }
            }
        }


        // UTILITIES
        // My PRINT function, because I'm lazy
        public static void PrintLn(string text)
        {
            Console.WriteLine(text);
        }


        // REGISTRATION (Hashing function from www.obviex.com/samples/hash.aspx under the GNUv3)
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

        // Generates Salted Hash for Password
        public static string ComputeHash(string plainText, string hashAlgorithm, byte[] saltBytes)
        {
            // If salt is not specified, generate it on the fly.
            if (saltBytes == null)
            {
                // Define min and max salt sizes.
                int minSaltSize = 4;
                int maxSaltSize = 8;

                // Generate a random number for the size of the salt.
                Random random = new Random();
                int saltSize = random.Next(minSaltSize, maxSaltSize);

                // Allocate a byte array, which will hold the salt.
                saltBytes = new byte[saltSize];

                // Initialize a random number generator.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

                // Fill the salt with cryptographically strong byte values.
                rng.GetNonZeroBytes(saltBytes);
            }

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

            // Because we support multiple hashing algorithms, we must define
            // hash object as a common (abstract) base class. We will specify the
            // actual hashing algorithm class later during object creation.
            HashAlgorithm hash;

            // Make sure hashing algorithm name is specified.
            if (hashAlgorithm == null)
                hashAlgorithm = "";

            // Initialize appropriate hashing algorithm class.
            switch (hashAlgorithm.ToUpper())
            {
                case "SHA1":
                    hash = new SHA1Managed();
                    break;

                case "SHA256":
                    hash = new SHA256Managed();
                    break;

                case "SHA384":
                    hash = new SHA384Managed();
                    break;

                case "SHA512":
                    hash = new SHA512Managed();
                    break;

                default:
                    hash = new MD5CryptoServiceProvider();
                    break;
            }

            // Compute hash value of our plain text with appended salt.
            byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

            // Create array which will hold hash and original salt bytes.
            byte[] hashWithSaltBytes = new byte[hashBytes.Length +
                                                saltBytes.Length];

            // Copy hash bytes into resulting array.
            for (int i = 0; i < hashBytes.Length; i++)
                hashWithSaltBytes[i] = hashBytes[i];

            // Append salt bytes to the result.
            for (int i = 0; i < saltBytes.Length; i++)
                hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];

            // Convert result into a base64-encoded string.
            string hashValue = Convert.ToBase64String(hashWithSaltBytes);

            // Return the result.
            return hashValue;
        }

        // Verifies Hash for logging in
        public static bool VerifyHash(string plainText, string hashAlgorithm, string hashValue)
        {
            // Convert base64-encoded hash value into a byte array.
            byte[] hashWithSaltBytes = Convert.FromBase64String(hashValue);

            // We must know size of hash (without salt).
            int hashSizeInBits, hashSizeInBytes;

            // Make sure that hashing algorithm name is specified.
            if (hashAlgorithm == null)
                hashAlgorithm = "";

            // Size of hash is based on the specified algorithm.
            switch (hashAlgorithm.ToUpper())
            {
                case "SHA1":
                    hashSizeInBits = 160;
                    break;

                case "SHA256":
                    hashSizeInBits = 256;
                    break;

                case "SHA384":
                    hashSizeInBits = 384;
                    break;

                case "SHA512":
                    hashSizeInBits = 512;
                    break;

                default: // Must be MD5
                    hashSizeInBits = 128;
                    break;
            }

            // Convert size of hash from bits to bytes.
            hashSizeInBytes = hashSizeInBits / 8;

            // Make sure that the specified hash value is long enough.
            if (hashWithSaltBytes.Length < hashSizeInBytes)
                return false;

            // Allocate array to hold original salt bytes retrieved from hash.
            byte[] saltBytes = new byte[hashWithSaltBytes.Length -
                                        hashSizeInBytes];

            // Copy salt from the end of the hash to the new array.
            for (int i = 0; i < saltBytes.Length; i++)
                saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];

            // Compute a new hash string.
            string expectedHashString =
                        ComputeHash(plainText, hashAlgorithm, saltBytes);

            // If the computed hash matches the specified hash,
            // the plain text value must be correct.
            return (hashValue == expectedHashString);
        }
    
        // LOGIN Primary Menu
        public static User LoginMenu(User currentUser, string sqlPath)
        {
            Console.Clear();
            PrintLn("************************* LOGIN MENU **************************");
            PrintLn("1. LogIn");
            PrintLn("2. Register");
            
            PrintLn("");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                currentUser = LogMeIn(currentUser, sqlPath);
                return currentUser;
            }
            else if (choice == "2")
            {
                currentUser = RegisterMe(currentUser, sqlPath);
                return currentUser;
            }
            else
            {
                currentUser.Session = 0;
                return currentUser;
            }




        }

        // Log In Existing User
        private static User LogMeIn(User currentUser, string sqlPath)
        {
            Console.Clear();
            PrintLn("*********************** LOGIN ************************");
            PrintLn("");

            string username = "";
            string pswd = "";


            int userRepeat = 1;
            while (userRepeat == 1)
            {
                PrintLn("Enter your username: ");
                username = Console.ReadLine();
                if (username == "" || !(System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9\n\r]+$")))
                {
                    PrintLn("");
                    PrintLn("You must use Letters and/or Numbers, only!");
                    PrintLn("Press Any Key To Continue!");
                    Console.ReadKey();
                    userRepeat = 1;
                }
                else { userRepeat = 0; }
            }


            int pswdRepeat = 1;
            while (pswdRepeat == 1)
            {
                PrintLn("");
                PrintLn("Enter your Password (no echo): ");
                pswd = PSWDBlank();

                if (pswd == "" || !(System.Text.RegularExpressions.Regex.IsMatch(pswd, @"^[a-zA-Z0-9\n\r]+$")))
                {
                    PrintLn("");
                    PrintLn("You must use Letters and/or Numbers, only!");
                    PrintLn("Press Any Key To Continue!");
                    Console.ReadKey();
                    pswdRepeat = 1;
                }
                else
                {
                    pswdRepeat = 0;
                }
            }

            // Open the DB
            string sql = "";
            string dbHash = "";
            SQLiteConnection sqlite_conn = new SQLiteConnection(sqlPath);
            sqlite_conn.Open();
            sql = "SELECT PSWDHash FROM users WHERE username='" + username + "';";

            using (SQLiteCommand cmd = sqlite_conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.Add(new SQLiteParameter("@username") { Value = username });
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader reader;
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dbHash = Convert.ToString(reader["PSWDHash"]);
                }
            }

            if (VerifyHash(pswd, "SHA256" ,dbHash))
            {
                using (SQLiteCommand cmd = sqlite_conn.CreateCommand())
                {
                    sql = "SELECT * FROM users WHERE username='" + username + "';";
                    cmd.CommandText = sql;
                    cmd.Parameters.Add(new SQLiteParameter("@username") { Value = username });
                    cmd.CommandType = System.Data.CommandType.Text;

                    SQLiteDataReader reader;
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {

                        currentUser.Username = Convert.ToString(reader["username"]);
                        currentUser.NameFirst = Convert.ToString(reader["NameFirst"]);
                        currentUser.NameLast = Convert.ToString(reader["NameLast"]);
                        currentUser.FavPrimaryConf = Convert.ToString(reader["FavFootballConf"]);
                        currentUser.FavFootballConf = Convert.ToString(reader["FavPrimaryConf"]);
                        currentUser.LoggedIn = 1;
                        currentUser.Session = 1;
                    }
                }

                return currentUser;
            }
            else
            {
                currentUser.LoggedIn = 0;
                currentUser.Session = 0;

                return currentUser;
            }
        }

        // Register a new user
        private static User RegisterMe(User currentUser, string sqlPath)
        {
            Console.Clear();
            PrintLn("*********************** Registration ************************");
            PrintLn("");

            string username = "";

            // Open the DB
            string sql ="";
            SQLiteConnection sqlite_conn = new SQLiteConnection(sqlPath);
            sqlite_conn.Open();

            int userRepeat = 1;
            while (userRepeat == 1)
            {
                PrintLn("Enter a username: ");
                username = Console.ReadLine();
                if (username == "" || !(System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9\n\r]+$")))
                {
                    PrintLn("");
                    PrintLn("You must use Letters and/or Numbers, only!");
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

            string nameFirst = "";
            int name1Repeat = 1;
            while (name1Repeat == 1)
            {
                PrintLn("Enter Your First Name: ");
                nameFirst = Console.ReadLine();
                if (nameFirst == "" || !(System.Text.RegularExpressions.Regex.IsMatch(nameFirst, @"^[a-zA-Z0-9\n\r]+$")))
                {
                    PrintLn("");
                    PrintLn("You must use Letters and/or Numbers, only!");
                    PrintLn("Press Any Key To Continue!");
                    Console.ReadKey();
                    name1Repeat = 1;
                }
                else
                {
                    name1Repeat = 0;
                }
            }

            string nameLast = "";
            int name2Repeat = 1;
            while (name2Repeat == 1)
            {
                PrintLn("");
                PrintLn("Enter Your Last Name: ");
                nameLast = Console.ReadLine();
                if (nameLast == "" || !(System.Text.RegularExpressions.Regex.IsMatch(nameLast, @"^[a-zA-Z0-9\n\r]+$")))
                {
                    PrintLn("");
                    PrintLn("You must use Letters and/or Numbers, only!");
                    PrintLn("Press Any Key To Continue!");
                    Console.ReadKey();
                    name2Repeat = 1;
                }
                else
                {
                    name2Repeat = 0;
                }

            }

            string pswd = "";
            int nomatch = 1;
            while (nomatch == 1)
            {
                int pswdRepeat = 1;
                while (pswdRepeat == 1)
                {
                    PrintLn("");
                    PrintLn("Enter a Password (no echo): ");
                    pswd = PSWDBlank();

                    if (pswd == "" || !(System.Text.RegularExpressions.Regex.IsMatch(pswd, @"^[a-zA-Z0-9\n\r]+$")))
                    {
                        PrintLn("");
                        PrintLn("You must use Letters and/or Numbers, only!");
                        PrintLn("Press Any Key To Continue!");
                        Console.ReadKey();
                        userRepeat = 1;
                    }
                    else
                    {
                        pswdRepeat = 0;
                    }
                }
                PrintLn("");
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
            var saltBytes = new byte[8];
            pswd = ComputeHash(pswd, "SHA256", saltBytes);

            string ffc = "";
            int ffcRepeat = 1;
            while (ffcRepeat == 1)
            {
                ffc = GetConf(5);
                if(ffc == "0")
                { continue; }
                else
                { ffcRepeat = 0; }
            }

            string fpc = "";
            int fpcRepeat = 1;
            while (fpcRepeat == 1)
            {
                fpc = GetConf(4);
                if (fpc == "0")
                { continue; }
                else
                { fpcRepeat = 0; }
            }
            
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

        // Edit User
        private static User EditUser(User currentUser, string sqlPath)
        {
            Console.Clear();
            PrintLn("*********************** EDIT USER ************************");
            PrintLn("");
            PrintLn("Username: " + currentUser.Username + "   <----Cannot Be Changed");
            PrintLn("");
            PrintLn("First Name: " + currentUser.NameFirst);
            PrintLn("Last Name: " + currentUser.NameLast);
            PrintLn("");
            PrintLn("Favorite Football Conference: ");
            PrintLn(currentUser.FavFootballConf);
            PrintLn("");
            PrintLn("Favorite Primary Conference: ");
            PrintLn(currentUser.FavPrimaryConf);
            PrintLn("");
            PrintLn("Password: ********");
            PrintLn("");
            PrintLn("");
            PrintLn("*** ARE YOU SURE YOU WANT TO EDIT THE CURRENT USER? ***");
            PrintLn("");
            PrintLn("*** TYPE 'Y' TO CONFIRM OR ANY OTHER KEY TO CANCEL ***");
            PrintLn("");

            string editSelect = Console.ReadLine().ToUpper();
            if (editSelect == "Y")
            {
                string nameFirst = "";
                int name1Repeat = 1;
                while (name1Repeat == 1)
                {
                    PrintLn("Enter Your First Name: ");
                    nameFirst = Console.ReadLine();
                    if (nameFirst == "" || !(System.Text.RegularExpressions.Regex.IsMatch(nameFirst, @"^[a-zA-Z0-9\n\r]+$")))
                    {
                        PrintLn("");
                        PrintLn("You must use Letters and/or Numbers, only!");
                        PrintLn("Press Any Key To Continue!");
                        Console.ReadKey();
                        name1Repeat = 1;
                    }
                    else
                    {
                        name1Repeat = 0;
                    }
                }

                string nameLast = "";
                int name2Repeat = 1;
                while (name2Repeat == 1)
                {
                    PrintLn("");
                    PrintLn("Enter Your Last Name: ");
                    nameLast = Console.ReadLine();
                    if (nameLast == "" || !(System.Text.RegularExpressions.Regex.IsMatch(nameLast, @"^[a-zA-Z0-9\n\r]+$")))
                    {
                        PrintLn("");
                        PrintLn("You must use Letters and/or Numbers, only!");
                        PrintLn("Press Any Key To Continue!");
                        Console.ReadKey();
                        name2Repeat = 1;
                    }
                    else
                    {
                        name2Repeat = 0;
                    }

                }

                string ffc = "";
                int ffcRepeat = 1;
                while (ffcRepeat == 1)
                {
                    ffc = GetConf(5);
                    if (ffc == "0")
                    { continue; }
                    else
                    { ffcRepeat = 0; }
                }

                string fpc = "";
                int fpcRepeat = 1;
                while (fpcRepeat == 1)
                {
                    fpc = GetConf(4);
                    if (fpc == "0")
                    { continue; }
                    else
                    { fpcRepeat = 0; }
                }

                // Open the DB
                string sql = "";
                SQLiteConnection sqlite_conn = new SQLiteConnection(sqlPath);
                sqlite_conn.Open();
                // Save to the DB
                sql = "UPDATE 'users' SET 'NameFirst'='" + nameFirst + "', 'NameLast'='" + nameLast + "', 'FavFootballConf'='" + ffc + "', 'FavPrimaryConf'='" + fpc + "' WHERE 'username'='" + currentUser.Username + "';";
                SQLiteCommand editUser = new SQLiteCommand(sql, sqlite_conn);
                PrintLn(sql);
                editUser.ExecuteNonQuery();
                sqlite_conn.Close();

                // Build the User
                currentUser.NameFirst = nameFirst;
                currentUser.NameLast = nameLast;
                currentUser.FavFootballConf = ffc;
                currentUser.FavPrimaryConf = fpc;
                currentUser.LoggedIn = 1;
                currentUser.Session = 1;

                return currentUser;


            }
            else
            {
                return currentUser;
            }


        }

        // Delete User
        private static User DeleteUser(User currentUser, string sqlPath)
        {
            Console.Clear();
            PrintLn("*********************** DELETE USER ************************");
            PrintLn("");
            PrintLn("");
            PrintLn("*** ARE YOU SURE YOU WANT TO DELETE THE CURRENT USER? ***");
            PrintLn("");
            PrintLn("*** TYPE 'Y' TO CONFIRM OR ANY OTHER KEY TO CANCEL ***");
            PrintLn("");

            string deleteSelect = Console.ReadLine().ToUpper();
            if (deleteSelect == "Y")
            {
                // Delete from the DB
                string sql = "";
                SQLiteConnection sqlite_conn = new SQLiteConnection(sqlPath);
                sqlite_conn.Open();
                sql = "DELETE FROM users WHERE username = '" + currentUser.Username + "';";
                SQLiteCommand addNewUser = new SQLiteCommand(sql, sqlite_conn);
                addNewUser.ExecuteNonQuery();
                sqlite_conn.Close();

                currentUser.Username = "";
                currentUser.NameFirst = "";
                currentUser.NameLast = "";
                currentUser.FavFootballConf = "";
                currentUser.FavPrimaryConf = "";
                currentUser.LoggedIn = 0;
                currentUser.Session = 0;

                return currentUser;
            }
            else
            {
                return currentUser;
            }
        }


        // PRIMARY FUNCTIONALITY
        // Prints menu and interprets user's choice
        public static int MainMenu(User currentUser, string sqlPath)
        {
            Console.Clear();
            PrintLn("************************* MAIN MENU **************************");
            PrintLn("1. Display Individual School Stats by Football Conference");
            PrintLn("2. Display Individual School Stats by Primary Conference");
            PrintLn("3. Display Favorite Football Conference");
            PrintLn("4. Display Favorite Primary Conference");
            PrintLn("...");
            PrintLn("6. Edit Profile");
            PrintLn("7. Delete Profile");
            PrintLn("...");
            PrintLn("9. Quit");
            PrintLn("");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                GetStats(5);
                return 1;
            }
            else if (choice == "2")
            {
                GetStats(4);
                return 1;
            }
            else if (choice == "3")
            {
                GetStats(5, currentUser.FavFootballConf);
                return 1;
            }
            else if (choice == "4")
            {
                GetStats(4, currentUser.FavPrimaryConf);
                return 1;
            }
            else if(choice =="6")
            {
                EditUser(currentUser, sqlPath);
                return 1;
            }
            else if (choice == "7")
            {
                DeleteUser(currentUser, sqlPath);
                return 0;
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
        public static string GetConf(int conf)
        {
            int confField = conf;
            Console.Clear();
            PrintLn("********************* Football Conf. Menu **********************");
            List<string> footballConfList = PrintSubMenu(GetField(confField));
            int maxValue = footballConfList.Count();
            PrintLn("Enter the number of your selection." + "\r\n");
            string footballConfSelection = Console.ReadLine();

            Int32.TryParse(footballConfSelection, out int number);
            if (number <= maxValue && number > 0)
            {
                int confType = confField;
                PrintLn(footballConfList.ElementAt(number - 1));
                string selectedConf = footballConfList.ElementAt(number - 1);

                return selectedConf;
            }
            else
            {
                PrintLn("I do not understand. Let's try that again...");
                PrintLn("Press any key to continue");
                Console.ReadKey();
                return "0";
            }

        }

        // Generates and displays statistics from the CSV
        public static int GetStats(int confField)
        {            
            string selectedConf = GetConf(confField);

            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directory = new DirectoryInfo(currentDirectory);
            var fileName = Path.Combine(directory.FullName, "ncaadata.csv");
            List<College> NCAACollegeData = ReadCollegeData(fileName, selectedConf, confField);
            Conference NCAAConfData = AggregateConfData(NCAACollegeData, selectedConf);

            DisplayConfData(NCAAConfData);

            return 0;
        }

        // Generates and displays statistics from the CSV
        public static int GetStats(int confField, string selectedConf)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directory = new DirectoryInfo(currentDirectory);
            var fileName = Path.Combine(directory.FullName, "ncaadata.csv");
            List<College> NCAACollegeData = ReadCollegeData(fileName, selectedConf, confField);
            Conference NCAAConfData = AggregateConfData(NCAACollegeData, selectedConf);

            DisplayConfData(NCAAConfData);

            return 0;
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
        public static int DisplayConfData(Conference NCAAConfData)
        {
            Console.Clear();
            PrintLn("******************** " + NCAAConfData.ChosenConf + " ********************");
            
            foreach (PropertyInfo prop in typeof(Conference).GetProperties())
            {
                Console.WriteLine("{0} = {1}", prop.Name, prop.GetValue(NCAAConfData, null));
            }

            PrintLn("Press 'S' Key To Write To JSON File Or Any Other Key To Return To Main Menu:");
            string choice = Console.ReadLine();
            if (choice.ToUpper() == "S")
            {
                string NCAA_Conf_JSON = JsonConvert.SerializeObject(NCAAConfData);
                string fileName = ".\\NCAA-" + NCAAConfData.ChosenConf + ".JSON";
                File.WriteAllText(fileName, NCAA_Conf_JSON);
                Process.Start("notepad.exe", fileName);
                return 0;
            }
            else
            { return 0; }

        }
 
    }
}
