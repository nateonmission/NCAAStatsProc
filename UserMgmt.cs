using System;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace ncaa_grad_info
{
    class UserMgmt
    {

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
        public static string ComputeHash(string plainText, string[] settings, byte[] saltBytes)
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
            if (settings[1] == null)
                settings[1] = "";

            // Initialize appropriate hashing algorithm class.
            switch (settings[1].ToUpper())
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
        public static bool VerifyHash(string plainText, string[] settings, string hashValue)
        {
            // Convert base64-encoded hash value into a byte array.
            byte[] hashWithSaltBytes = Convert.FromBase64String(hashValue);

            // We must know size of hash (without salt).
            int hashSizeInBits, hashSizeInBytes;

            // Make sure that hashing algorithm name is specified.
            if (settings[1] == null)
                settings[1] = "";

            // Size of hash is based on the specified algorithm.
            switch (settings[1].ToUpper())
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
                        ComputeHash(plainText, settings, saltBytes);

            // If the computed hash matches the specified hash,
            // the plain text value must be correct.
            return (hashValue == expectedHashString);
        }

        // LOGIN Primary Menu
        public static User LoginMenu(User currentUser, List<List<string>> confLists, string[] settings)
        {
            Console.Clear();
            Console.WriteLine("************************* LOGIN MENU **************************");
            Console.WriteLine("1. LogIn");
            Console.WriteLine("2. Register");

            Console.WriteLine("");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                currentUser = LogMeIn(currentUser, settings);
                return currentUser;
            }
            else if (choice == "2")
            {
                currentUser = RegisterMe(currentUser, confLists, settings);
                return currentUser;
            }
            else
            {
                currentUser.Session = 0;
                return currentUser;
            }




        }

        // Log In Existing User
        private static User LogMeIn(User currentUser, string[] settings)
        {
            Console.Clear();
            Console.WriteLine("*********************** LOGIN ************************");
            Console.WriteLine("");

            string username = "";
            string pswd = "";


            int userRepeat = 1;
            while (userRepeat == 1)
            {
                Console.WriteLine("Enter your username: ");
                username = Console.ReadLine();
                if (username == "" || !(System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9\n\r]+$")))
                {
                    Console.WriteLine("");
                    Console.WriteLine("You must use Letters and/or Numbers, only!");
                    Console.WriteLine("Press Any Key To Continue!");
                    Console.ReadKey();
                    userRepeat = 1;
                }
                else { userRepeat = 0; }
            }


            int pswdRepeat = 1;
            while (pswdRepeat == 1)
            {
                Console.WriteLine("");
                Console.WriteLine("Enter your Password (no echo): ");
                pswd = PSWDBlank();

                if (pswd == "" || !(System.Text.RegularExpressions.Regex.IsMatch(pswd, @"^[a-zA-Z0-9\n\r]+$")))
                {
                    Console.WriteLine("");
                    Console.WriteLine("You must use Letters and/or Numbers, only!");
                    Console.WriteLine("Press Any Key To Continue!");
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
            SQLiteConnection sqlite_conn = new SQLiteConnection(settings[0]);
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

            if (VerifyHash(pswd, settings, dbHash))
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
        public static User RegisterMe(User currentUser, List<List<string>> confLists, string[] settings)
        {
            Console.Clear();
            Console.WriteLine("*********************** Registration ************************");
            Console.WriteLine("");

            string username = "";

            // Open the DB
            string sql = "";
            SQLiteConnection sqlite_conn = new SQLiteConnection(settings[0]);
            sqlite_conn.Open();

            int userRepeat = 1;
            while (userRepeat == 1)
            {
                Console.WriteLine("Enter a username: ");
                username = Console.ReadLine();
                if (username == "" || !(System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9\n\r]+$")))
                {
                    Console.WriteLine("");
                    Console.WriteLine("You must use Letters and/or Numbers, only!");
                    Console.WriteLine("Press Any Key To Continue!");
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
                        Console.WriteLine("");
                        Console.WriteLine("That Username Is Taken. Select Another!");
                        Console.WriteLine("Press Any Key To Continue!");
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
                Console.WriteLine("Enter Your First Name: ");
                nameFirst = Console.ReadLine();
                if (nameFirst == "" || !(System.Text.RegularExpressions.Regex.IsMatch(nameFirst, @"^[a-zA-Z0-9\n\r]+$")))
                {
                    Console.WriteLine("");
                    Console.WriteLine("You must use Letters and/or Numbers, only!");
                    Console.WriteLine("Press Any Key To Continue!");
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
                Console.WriteLine("");
                Console.WriteLine("Enter Your Last Name: ");
                nameLast = Console.ReadLine();
                if (nameLast == "" || !(System.Text.RegularExpressions.Regex.IsMatch(nameLast, @"^[a-zA-Z0-9\n\r]+$")))
                {
                    Console.WriteLine("");
                    Console.WriteLine("You must use Letters and/or Numbers, only!");
                    Console.WriteLine("Press Any Key To Continue!");
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
                    Console.WriteLine("");
                    Console.WriteLine("Enter a Password (no echo): ");
                    pswd = PSWDBlank();

                    if (pswd == "" || !(System.Text.RegularExpressions.Regex.IsMatch(pswd, @"^[a-zA-Z0-9\n\r]+$")))
                    {
                        Console.WriteLine("");
                        Console.WriteLine("You must use Letters and/or Numbers, only!");
                        Console.WriteLine("Press Any Key To Continue!");
                        Console.ReadKey();
                        userRepeat = 1;
                    }
                    else
                    {
                        pswdRepeat = 0;
                    }
                }
                Console.WriteLine("");
                Console.WriteLine("Please, Confirm Your Password (no echo): ");
                string pswdConfirm = PSWDBlank();
                if (pswd == pswdConfirm)
                {
                    nomatch = 0;
                }
                else
                {
                    Console.WriteLine("Passwords do not match. Try again.");
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadKey();
                    nomatch = 1;
                }

            }
            int parseInt = Int32.Parse(settings[2]);
            var saltBytes = new byte[parseInt];
            pswd = ComputeHash(pswd, settings, saltBytes);

            string ffc = "";
            int ffcRepeat = 1;
            while (ffcRepeat == 1)
            {
                ffc = Program.GetConf(5, confLists);
                if (ffc == "0")
                { continue; }
                else
                { ffcRepeat = 0; }
            }

            string fpc = "";
            int fpcRepeat = 1;
            while (fpcRepeat == 1)
            {
                fpc = Program.GetConf(4, confLists);
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
        public static User EditUser(User currentUser, List<List<string>> confLists, string[] settings)
        {
            Console.Clear();
            Console.WriteLine("*********************** EDIT USER ************************");
            Console.WriteLine("");
            Console.WriteLine("Username: " + currentUser.Username + "   <----Cannot Be Changed");
            Console.WriteLine("");
            Console.WriteLine("First Name: " + currentUser.NameFirst);
            Console.WriteLine("Last Name: " + currentUser.NameLast);
            Console.WriteLine("");
            Console.WriteLine("Favorite Football Conference: ");
            Console.WriteLine(currentUser.FavFootballConf);
            Console.WriteLine("");
            Console.WriteLine("Favorite Primary Conference: ");
            Console.WriteLine(currentUser.FavPrimaryConf);
            Console.WriteLine("");
            Console.WriteLine("Password: ********");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("*** ARE YOU SURE YOU WANT TO EDIT THE CURRENT USER? ***");
            Console.WriteLine("");
            Console.WriteLine("*** TYPE 'Y' TO CONFIRM OR ANY OTHER KEY TO CANCEL ***");
            Console.WriteLine("");

            string editSelect = Console.ReadLine().ToUpper();
            if (editSelect == "Y")
            {
                string nameFirst = "";
                int name1Repeat = 1;
                while (name1Repeat == 1)
                {
                    Console.WriteLine("Enter Your First Name: ");
                    nameFirst = Console.ReadLine();
                    if (nameFirst == "" || !(System.Text.RegularExpressions.Regex.IsMatch(nameFirst, @"^[a-zA-Z0-9\n\r]+$")))
                    {
                        Console.WriteLine("");
                        Console.WriteLine("You must use Letters and/or Numbers, only!");
                        Console.WriteLine("Press Any Key To Continue!");
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
                    Console.WriteLine("");
                    Console.WriteLine("Enter Your Last Name: ");
                    nameLast = Console.ReadLine();
                    if (nameLast == "" || !(System.Text.RegularExpressions.Regex.IsMatch(nameLast, @"^[a-zA-Z0-9\n\r]+$")))
                    {
                        Console.WriteLine("");
                        Console.WriteLine("You must use Letters and/or Numbers, only!");
                        Console.WriteLine("Press Any Key To Continue!");
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
                    ffc = Program.GetConf(5, confLists);
                    if (ffc == "0")
                    { continue; }
                    else
                    { ffcRepeat = 0; }
                }

                string fpc = "";
                int fpcRepeat = 1;
                while (fpcRepeat == 1)
                {
                    fpc = Program.GetConf(4, confLists);
                    if (fpc == "0")
                    { continue; }
                    else
                    { fpcRepeat = 0; }
                }

                // Open the DB
                string sql = "";
                SQLiteConnection sqlite_conn = new SQLiteConnection(settings[0]);
                sqlite_conn.Open();
                // Save to the DB
                sql = "UPDATE 'users' SET 'NameFirst'='" + nameFirst + "', 'NameLast'='" + nameLast + "', 'FavFootballConf'='" + ffc + "', 'FavPrimaryConf'='" + fpc + "' WHERE 'username'='" + currentUser.Username + "';";
                SQLiteCommand editUser = new SQLiteCommand(sql, sqlite_conn);
                Console.WriteLine(sql);
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
        public static User DeleteUser(User currentUser, string[] settings)
        {
            Console.Clear();
            Console.WriteLine("*********************** DELETE USER ************************");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("*** ARE YOU SURE YOU WANT TO DELETE THE CURRENT USER? ***");
            Console.WriteLine("");
            Console.WriteLine("*** TYPE 'Y' TO CONFIRM OR ANY OTHER KEY TO CANCEL ***");
            Console.WriteLine("");

            string deleteSelect = Console.ReadLine().ToUpper();
            if (deleteSelect == "Y")
            {
                // Delete from the DB
                string sql = "";
                SQLiteConnection sqlite_conn = new SQLiteConnection(settings[0]);
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



    }
}
