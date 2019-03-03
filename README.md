#NCAA STAT PROCESSOR
----

This console application takes a CSV file, aquired from the NCAA website, and aggrigates and averages the College-level data for the athletic conference chosen by the user. 

##Dependenacies
This C# application makes use of SQLite, LINQ, and its ADO.net provider for the user registration and statistical functions.
It also uses Newtonsoft's JSON functionality to write the aggregated data to a new file.


##Data Scrubbing
Each line in the CSV began with a leading comma. Also, the names of some colleges contained commas. These had to be manually removed.


