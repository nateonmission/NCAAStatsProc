# NCAA STAT PROCESSOR
----

This console application reads a CSV file, aquired from the NCAA website, then aggrigates and averages the data from the College-level for the athletic conference chosen by the user. It is then saved as a JSON file.

## Dependenacies
This C# application makes use of SQLite, LINQ, and its ADO.net provider for the user registration and statistical functions.
It also uses Newtonsoft's JSON functionality to write the aggregated data to a new file.


## Data Scrubbing
Each line in the CSV began with a leading comma. Also, the names of some colleges contained commas. These had to be manually removed.


