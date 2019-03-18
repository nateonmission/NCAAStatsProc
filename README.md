# NCAA STAT PROCESSOR
----

The National Colleget Athletics Associatioin (NCAA) provided graduation information for all of their member insitutions. This information is provided for each of the individual school. This console application reads a CSV file, aquired from the NCAA website, containing all the schools'graduation information, then aggrigates and averages the data from the College-level to generate conference-level statistics for the conference chosen by the user. It is then saved as a JSON file.

## Usage

### Registration
A new user will need to register and create a username and password. This user account will also store the user's favorite football and primary conferences for convinience. This profile is stored in user.db .

### Login
If a user already has a username, they may login using their existing user account.

### Main Menu
1. Display Individual School Stats by Football Conference - Displays a list of NCAA FOOTBALL conferences. The user then selects one and the a Conference object is instantiated that will contain the totals and averages derived from the school-level data. It will then save a JSON file containing the results.
2. Display Individual School Stats by Primary Conference - Displays a list of NCAA PRIMARY conferences. The user then selects one and the a Conference object is instantiated that will contain the totals and averages derived from the school-level data. It will then save a JSON file containing the results.
3. Display Favorite Football Conference - The same functionality as number 1 except it process the user's favorite FOOTBALL conference.
4. Display Favorite Primary Conference - The same functionality as number 2 except it process the user's favorite PRIMARY conference.
...
6. Edit Profile - allows the user to edit their profile. Username cannot be changed, as it is the primary key of the user.db file.
7. Delete Profile - allows the user to delete their profile and dumps them out at the login menu.
...
9. Quit - allows the user to exit out of the program.

## Dependenacies
This C# application makes use of SQLite, LINQ, and its ADO.net provider for the user registration and statistical functions.
It also uses Newtonsoft's JSON functionality to write the aggregated data to a new file.


## Data Scrubbing
Each line in the CSV began with a leading comma. Also, the names of some colleges contained commas. These had to be manually removed.

----
-J. Nathan Allen
@nateonmission


