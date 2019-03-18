# NCAA STAT PROCESSOR
----

The National Colleget Athletics Associatioin (NCAA) provided graduation information for all of their member insitutions. This information is provided for each of the individual school. This console application reads a CSV file, aquired from the NCAA website, containing all the schools'graduation information, then aggrigates and averages the data from the College-level to generate conference-level statistics for the conference chosen by the user. It is then saved as a JSON file.

## Usage

### Registration
A new user will need to register and create a username and password. This user account will also store the user's favorite football and primary conferences for convinience. This profile is stored in user.db .

### Login
If a user already has a username, they may login using their existing user account.

### Main Menu
1. _Display Individual School Stats by Football Conference_ - Displays a list of NCAA FOOTBALL conferences. The user then selects one and the a Conference object is instantiated that will contain the totals and averages derived from the school-level data. It will then save a JSON file containing the results.
2. _Display Individual School Stats by Primary Conference_ - Displays a list of NCAA PRIMARY conferences. The user then selects one and the a Conference object is instantiated that will contain the totals and averages derived from the school-level data. It will then save a JSON file containing the results.
3. _Display Favorite Football Conference_ - The same functionality as number 1 except it process the user's favorite football conference.
4. _Display Favorite Primary Conference_ - The same functionality as number 2 except it process the user's favorite primary conference.
5. SKIPPED
6. _Edit Profile_ - allows the user to edit their profile. Username cannot be changed, as it is the primary key of the user.db file.
7. _Delete Profile_ - allows the user to delete their profile and dumps them out at the login menu.
8. SKIPPED
9. _Quit_ - allows the user to exit out of the program.

## Features & Requirements
The user registration actions contain all four C.R.U.D. functions, storing this data in user.db. It also instantiates a User object. During the function of the program, College objects are instantiated by reading a CSV. Those College objects' data are used to instantiate a Conference object, containing summed and averaged fields. The Conference object is then saved as a JSON file.

## Dependenacies
This C# application makes use of SQLite, LINQ, and its ADO.net provider for the user registration and statistical functions.
It also uses Newtonsoft's JSON functionality to write the aggregated data to a new file.


## Data Scrubbing
Each line in the CSV began with a leading comma. Also, the names of some colleges contained commas. These had to be manually removed.

----
-J. Nathan Allen
@nateonmission


