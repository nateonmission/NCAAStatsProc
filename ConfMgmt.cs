using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using System.Diagnostics;


namespace ncaa_grad_info
{
    class ConfMgmt
    {



        // Generates and displays statistics from the CSV
        public static int GetStats(int confField)
        {
            string selectedConf = Program.GetConf(confField);

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
            NCAAConfData.FED_N_2011_SA = NCAACollegeData.Sum(item => item.FED_N_2011_SA);
            NCAAConfData.FED_N_2010_SA = NCAACollegeData.Sum(item => item.FED_N_2010_SA);
            NCAAConfData.FED_N_2009_SA = NCAACollegeData.Sum(item => item.FED_N_2009_SA);
            NCAAConfData.FED_N_2008_SA = NCAACollegeData.Sum(item => item.FED_N_2008_SA);
            NCAAConfData.FED_N_2007_SA = NCAACollegeData.Sum(item => item.FED_N_2007_SA);
            NCAAConfData.FED_N_2006_SA = NCAACollegeData.Sum(item => item.FED_N_2006_SA);
            NCAAConfData.FED_N_2005_SA = NCAACollegeData.Sum(item => item.FED_N_2005_SA);
            NCAAConfData.FED_N_2004_SA = NCAACollegeData.Sum(item => item.FED_N_2004_SA);
            NCAAConfData.FED_N_2003_SA = NCAACollegeData.Sum(item => item.FED_N_2003_SA);
            NCAAConfData.FED_N_2002_SA = NCAACollegeData.Sum(item => item.FED_N_2002_SA);
            NCAAConfData.FED_N_2001_SA = NCAACollegeData.Sum(item => item.FED_N_2001_SA);
            NCAAConfData.FED_N_2000_SA = NCAACollegeData.Sum(item => item.FED_N_2000_SA);
            NCAAConfData.FED_N_1999_SA = NCAACollegeData.Sum(item => item.FED_N_1999_SA);
            NCAAConfData.FED_N_1998_SA = NCAACollegeData.Sum(item => item.FED_N_1998_SA);
            NCAAConfData.FED_N_1997_SA = NCAACollegeData.Sum(item => item.FED_N_1997_SA);
            NCAAConfData.FED_N_1996_SA = NCAACollegeData.Sum(item => item.FED_N_1996_SA);
            NCAAConfData.FED_N_1995_SA = NCAACollegeData.Sum(item => item.FED_N_1995_SA);
            NCAAConfData.FED_RATE_2011_SA = NCAACollegeData.Sum(item => item.FED_RATE_2011_SA);
            NCAAConfData.FED_RATE_2010_SA = NCAACollegeData.Sum(item => item.FED_RATE_2010_SA);
            NCAAConfData.FED_RATE_2009_SA = NCAACollegeData.Sum(item => item.FED_RATE_2009_SA);
            NCAAConfData.FED_RATE_2008_SA = NCAACollegeData.Sum(item => item.FED_RATE_2008_SA);
            NCAAConfData.FED_RATE_2007_SA = NCAACollegeData.Sum(item => item.FED_RATE_2007_SA);
            NCAAConfData.FED_RATE_2006_SA = NCAACollegeData.Sum(item => item.FED_RATE_2006_SA);
            NCAAConfData.FED_RATE_2005_SA = NCAACollegeData.Sum(item => item.FED_RATE_2005_SA);
            NCAAConfData.FED_RATE_2004_SA = NCAACollegeData.Sum(item => item.FED_RATE_2004_SA);
            NCAAConfData.FED_RATE_2003_SA = NCAACollegeData.Sum(item => item.FED_RATE_2003_SA);
            NCAAConfData.FED_RATE_2002_SA = NCAACollegeData.Sum(item => item.FED_RATE_2002_SA);
            NCAAConfData.FED_RATE_2001_SA = NCAACollegeData.Sum(item => item.FED_RATE_2001_SA);
            NCAAConfData.FED_RATE_2000_SA = NCAACollegeData.Sum(item => item.FED_RATE_2000_SA);
            NCAAConfData.FED_RATE_1999_SA = NCAACollegeData.Sum(item => item.FED_RATE_1999_SA);
            NCAAConfData.FED_RATE_1998_SA = NCAACollegeData.Sum(item => item.FED_RATE_1998_SA);
            NCAAConfData.FED_RATE_1997_SA = NCAACollegeData.Sum(item => item.FED_RATE_1997_SA);
            NCAAConfData.FED_RATE_1996_SA = NCAACollegeData.Sum(item => item.FED_RATE_1996_SA);
            NCAAConfData.FED_RATE_1995_SA = NCAACollegeData.Sum(item => item.FED_RATE_1995_SA);
            NCAAConfData.GSR_N_2011_SA = NCAACollegeData.Sum(item => item.GSR_N_2011_SA);
            NCAAConfData.GSR_N_2010_SA = NCAACollegeData.Sum(item => item.GSR_N_2010_SA);
            NCAAConfData.GSR_N_2009_SA = NCAACollegeData.Sum(item => item.GSR_N_2009_SA);
            NCAAConfData.GSR_N_2008_SA = NCAACollegeData.Sum(item => item.GSR_N_2008_SA);
            NCAAConfData.GSR_N_2007_SA = NCAACollegeData.Sum(item => item.GSR_N_2007_SA);
            NCAAConfData.GSR_N_2006_SA = NCAACollegeData.Sum(item => item.GSR_N_2006_SA);
            NCAAConfData.GSR_N_2005_SA = NCAACollegeData.Sum(item => item.GSR_N_2005_SA);
            NCAAConfData.GSR_N_2004_SA = NCAACollegeData.Sum(item => item.GSR_N_2004_SA);
            NCAAConfData.GSR_N_2003_SA = NCAACollegeData.Sum(item => item.GSR_N_2003_SA);
            NCAAConfData.GSR_N_2002_SA = NCAACollegeData.Sum(item => item.GSR_N_2002_SA);
            NCAAConfData.GSR_N_2001_SA = NCAACollegeData.Sum(item => item.GSR_N_2001_SA);
            NCAAConfData.GSR_N_2000_SA = NCAACollegeData.Sum(item => item.GSR_N_2000_SA);
            NCAAConfData.GSR_N_1999_SA = NCAACollegeData.Sum(item => item.GSR_N_1999_SA);
            NCAAConfData.GSR_N_1998_SA = NCAACollegeData.Sum(item => item.GSR_N_1998_SA);
            NCAAConfData.GSR_N_1997_SA = NCAACollegeData.Sum(item => item.GSR_N_1997_SA);
            NCAAConfData.GSR_N_1996_SA = NCAACollegeData.Sum(item => item.GSR_N_1996_SA);
            NCAAConfData.GSR_N_1995_SA = NCAACollegeData.Sum(item => item.GSR_N_1995_SA);
            NCAAConfData.GSR_2011_SA = NCAACollegeData.Sum(item => item.GSR_2011_SA);
            NCAAConfData.GSR_2010_SA = NCAACollegeData.Sum(item => item.GSR_2010_SA);
            NCAAConfData.GSR_2009_SA = NCAACollegeData.Sum(item => item.GSR_2009_SA);
            NCAAConfData.GSR_2008_SA = NCAACollegeData.Sum(item => item.GSR_2008_SA);
            NCAAConfData.GSR_2007_SA = NCAACollegeData.Sum(item => item.GSR_2007_SA);
            NCAAConfData.GSR_2006_SA = NCAACollegeData.Sum(item => item.GSR_2006_SA);
            NCAAConfData.GSR_2005_SA = NCAACollegeData.Sum(item => item.GSR_2005_SA);
            NCAAConfData.GSR_2004_SA = NCAACollegeData.Sum(item => item.GSR_2004_SA);
            NCAAConfData.GSR_2003_SA = NCAACollegeData.Sum(item => item.GSR_2003_SA);
            NCAAConfData.GSR_2002_SA = NCAACollegeData.Sum(item => item.GSR_2002_SA);
            NCAAConfData.GSR_2001_SA = NCAACollegeData.Sum(item => item.GSR_2001_SA);
            NCAAConfData.GSR_2000_SA = NCAACollegeData.Sum(item => item.GSR_2000_SA);
            NCAAConfData.GSR_1999_SA = NCAACollegeData.Sum(item => item.GSR_1999_SA);
            NCAAConfData.GSR_1998_SA = NCAACollegeData.Sum(item => item.GSR_1998_SA);
            NCAAConfData.GSR_1997_SA = NCAACollegeData.Sum(item => item.GSR_1997_SA);
            NCAAConfData.GSR_1996_SA = NCAACollegeData.Sum(item => item.GSR_1996_SA);
            NCAAConfData.GSR_1995_SA = NCAACollegeData.Sum(item => item.GSR_1995_SA);

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
            Console.WriteLine("******************** " + NCAAConfData.ChosenConf + " ********************");

            foreach (PropertyInfo prop in typeof(Conference).GetProperties())
            {
                Console.WriteLine("{0} = {1}", prop.Name, prop.GetValue(NCAAConfData, null));
            }

            Console.WriteLine("Press 'S' Key To Write To JSON File Or Any Other Key To Return To Main Menu:");
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
