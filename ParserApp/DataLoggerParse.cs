using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserApp
{
    public static class DataLoggerParse
    {
        // method - takes csv file with path and returns a list of csv lines
        public static String[] ReadCSVFile(String csvFileNameWithPath)
        {
            //will store each file line as a string list just for an array count
            List<String> listofcsvlines = new List<String>();
            String[] arrofcsvlines;
            try
            {
                using (System.IO.StreamReader csvReader = new System.IO.StreamReader(csvFileNameWithPath))
                {
                    string lineStr;

                    while ((lineStr = csvReader.ReadLine()) != null)
                    {
                        listofcsvlines.Add(lineStr);
                    }
                    arrofcsvlines = listofcsvlines.ToArray();
                }
                
            }
            catch (Exception objError)
            {
                throw objError;
            }

            return arrofcsvlines;
        }

        //method takes a raw list of data lines, extracts timestamps and returns array
        public static int[] getListOfTimeStamps(String[] rawlistoflines)
        {

            //array variable that will store extracted timestamps
            int[] timestamplist = new int[rawlistoflines.Count()];

            //iterate thru each line in list, store extracted timestamp string
            for (int i = 1; i < rawlistoflines.Count(); i++)
            {
                //extracts timestamp method here
                timestamplist[i] = getSingleTimeStamp(rawlistoflines[i]);
            }

            return timestamplist;
        }

        //method takes a raw list of data lines, extracts data entries and returns array
        public static String[] getListOfDataEntries(String[] rawlistoflines)
        {

            //array variable that will store extracted timestamps
            String[] dataentrylist = new String[rawlistoflines.Count()];

            //iterate thru each line in list, store extracted timestamp string
            for (int i = 1; i < rawlistoflines.Count(); i++)
            {
                //extracts timestamp method here
                dataentrylist[i] = getSingleDataEntry(rawlistoflines[i]);
            }

            return dataentrylist;
        }
        //method takes a raw list of data lines, extracts data entries, convert to absolute value and returns array
        public static decimal[] getListOfAbsoluteDataEntries(String[] rawlistoflines)
        {

            //array variable that will store extracted timestamps
            decimal[] dataentrylist = new decimal[rawlistoflines.Count()];

            //iterate thru each line in list, store extracted timestamp string
            for (int i = 1; i < rawlistoflines.Count(); i++)
            {
                //extracts timestamp method here
                dataentrylist[i] = decimal.Parse(getSingleDataEntry(rawlistoflines[i]),System.Globalization.NumberStyles.Float);
            }

            return dataentrylist;
        }
        //takes a string and returns timestamps string
        public static int getSingleTimeStamp(String line)
        {
            if (line == null)
            {
                return -1;
            }
            //locates timestamp position by pos of 1st comma + **/**/**** +space
            int startTag = line.IndexOf(",") + 1;//excludes 1st occurance of ','
            //searches for comma, semi colons for parse
            //check if delimitation exits
            if (startTag < 0)
            {
                return -1;
            }
            //string variable used to collect string indexes
            String target = "";
            //counter variable used to iterate thru string indexes
            int count = 0;

            //loop thru line until end tag ","
            while (!(target.Contains(",")))
            {
                //variable will collect all indexes between start and last count
                target = line.Substring(startTag + 11, count);//11 excludes date in timestamp
                //check if contains ':' - remove
                if (target.Contains(":"))
                {
                    target=target.Replace(":","");
                }
                count++;
            }

            //condition that checks if collected indexes contain the end tag "," - trim
            if (target.Contains(","))
            {
                target = target.TrimEnd(',');
            }

            return Convert.ToInt32(target);
        }

        //takes a string and returns dataentry string 
        public static String getSingleDataEntry(String line)
        {
            //locates dataentry position
            int startTag = IndexOfOccurence(line, ",", 2)+1;//+1 to exclude the comma

            //check if delimitation - exits
            if (startTag < 0)
            {
                return null;
            }
            //string variable used to collect string indexes
            String target = "";

            //variable will store start to end of string
            target = line.Substring(startTag,15);//15 for digit/character length of all dataentries

            return target;
        }
        //check occurences of string 
        private static int IndexOfOccurence(string s, string match, int occurence)
        {
            int i = 1;
            int index = 0;

            while (i <= occurence && (index = s.IndexOf(match, index + 1)) != -1)
            {
                if (i == occurence)
                    return index;

                i++;
            }

            return -1;
        }
        
        //takes an array of time integer and return array of time elapse
        public static int[] getListOfIntervals(int[] rawtimestamps)
        {
            int[] arrofintervals;
            //list that will store time intervals for count number
            List<int> listofintervals = new List<int>();
            //starts the first element at 0
            listofintervals.Add(0);
           for (int i = 0; i < rawtimestamps.Length-1; i++)
            {
                listofintervals.Add(rawtimestamps[i] - rawtimestamps[i+1]);
            }

            arrofintervals = listofintervals.ToArray();

            return arrofintervals;
        }
        //takes a path string [datalogger csv file],parses 3 arrays [time, raw data, absolute data]
        public static String[] getDelimitedParse(String path)
        {
            //array for compiled array
            var iracompiled = new List<String>();
            
            //arrays containing interval, raw data, and absolute data
            var rawresults = ReadCSVFile(path);
            var intervals = getListOfIntervals(getListOfTimeStamps(rawresults));
            var rawdataentries = getListOfDataEntries(rawresults);
            var absolutedataentries = getListOfAbsoluteDataEntries(rawresults);

            //variable for tracking time
            int collector = intervals[0];
            //indexer
            int i = 1;
            //time object
            TimeSpan t;
            //header  items for cvs file
            iracompiled.Add("Interval, Raw Value, Absolute Value");
            //collects data for the first 10 seconds
            while (collector < 10000)
            {
                //calculate hr, mins,sec, remaining ms using total ms
                t = TimeSpan.FromMilliseconds(collector - intervals[i]);
                //format string time
                String tlapse= string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                        t.Hours,
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);
                //formats parse
                iracompiled.Add(tlapse + "," + rawdataentries[i].ToString() + "," + absolutedataentries[i].ToString("0.##########"));
                ++i;
                //collects total intervals times in ms
                collector = collector - intervals[i];
            }

            while (collector > 10000 && i<intervals.Length-2)
            {
                //calculate hr, mins,sec, remaining ms using total ms
                t = TimeSpan.FromMilliseconds(collector - intervals[i]);
                //format string time
                String tlapse = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                        t.Hours,
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);
                //formats parse
                iracompiled.Add(tlapse + "," + rawdataentries[i].ToString() + "," + absolutedataentries[i].ToString("0.##########"));
                ++i;
                //collects total intervals times in ms
                collector = collector - intervals[i];
            }

            var ira = iracompiled.ToArray();
            return ira;
        }
        
    }

}
