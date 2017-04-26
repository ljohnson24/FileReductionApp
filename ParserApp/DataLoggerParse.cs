using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace ParserApp
{
    public static class DataLoggerParse
    {

        //method - validate datalogger csv file 
        public static bool ValidateCSVFile(String csvFileNameWithPath)
        {
            try
            {
                using (var fileStream = File.OpenRead(csvFileNameWithPath))
                {
                    using (TextReader csvReader = new StreamReader(csvFileNameWithPath))
                    {
                        string lineStr = csvReader.ReadLine();

                        if (lineStr != null && lineStr.Contains("Sweep #"))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception objError)
            {
                throw objError;
            }

            return false;
        }
        //function for splitting files into smaller files
        public static String SplitCSVFile(String csvFileNameWithPath)
        {
            //container for storing lines of a csv
            StreamWriter outputfile = null;
            string lineStr;
            int i = 0;
            int count = 0;
            int max = 1000000;
            int loc = csvFileNameWithPath.Length;
            string dir = csvFileNameWithPath.Substring(0, loc - 4) + "\\";
            string filename = csvFileNameWithPath.Substring((loc - 12), 8);
            
            try
            {
                using (var fileStream = new System.IO.StreamReader(csvFileNameWithPath))
                {
                    Directory.CreateDirectory(dir);
                    while (!fileStream.EndOfStream)
                    {//check if read is below max lines
                        lineStr = fileStream.ReadLine();

                        if (outputfile == null)
                        {
                            outputfile = new System.IO.StreamWriter(
                                string.Format(dir + filename + "_{0}.csv", i++),
                                false,
                                fileStream.CurrentEncoding);
                        }

                        if (count < max)
                        {//write to current subfile if under max lines
                            if (count == 0)
                            {
                                if (!lineStr.Contains('#'))
                                {
                                    outputfile.WriteLine("Sweep #,Time,Chan 222 (ADC)");
                                    ++count;
                                }

                            }
                            outputfile.WriteLine(lineStr);

                            ++count;
                        }

                        if (count >= max)
                        {

                            count = 0;
                            outputfile.Close();
                            outputfile = null;
                        }
                    }
                }
            }



            finally
            {
                if (outputfile != null)
                    outputfile.Dispose();
            }

            return dir;
        }

        // method - takes csv file with path and returns a list of csv lines
        public static List<string> ReadCSVFile(String csvFileNameWithPath)
        {
            //container for storing lines of a csv
            List<string> listofcsvlines = new List<string>();

            try
            {
                using (var fileStream = File.OpenRead(csvFileNameWithPath))
                {
                    using (TextReader csvReader = new StreamReader(csvFileNameWithPath))
                    {
                        string lineStr;


                        while ((lineStr = csvReader.ReadLine()) != null)
                        {
                            //excludes the header line
                            if (!lineStr.Contains("#"))
                            {
                                listofcsvlines.Add(lineStr);

                            }
                        }
                    }
                }
            }
            catch (Exception objError)
            {
                throw objError;
            }

            return listofcsvlines;
        }

        //method takes a raw list of data lines, extracts timestamps and returns array
        public static List<int> getListOfTimeStamps(List<String> rawlistoflines)
        {

            //array variable that will store extracted timestamps
            List<int> timestamplist = new List<int>();

            //iterate thru each line in list, store extracted timestamp string
            for (int i = 0; i < rawlistoflines.Count(); i++)
            {
                //extracts timestamp method here
                timestamplist.Add(getSingleTimeStamp(rawlistoflines[i]));
            }

            return timestamplist;
        }

        //method takes a raw list of data lines, extracts data entries and returns array
        public static List<String> getListOfDataEntries(List<String> rawlistoflines)
        {

            //array variable that will store extracted timestamps
            List<String> dataentrylist = new List<String>();

            //iterate thru each line in list, store extracted timestamp string
            for (int i = 0; i < rawlistoflines.Count(); i++)
            {
                //extracts timestamp method here
                dataentrylist.Add(getSingleDataEntry(rawlistoflines[i]));
            }

            return dataentrylist;
        }
        //method takes a raw list of data lines, extracts data entries, convert to absolute value and returns array
        public static List<string> getListOfAbsoluteDataEntries(List<String> rawlistoflines)
        {
            //array variable that will store extracted timestamps
            List<string> dataentrylist = new List<string>();

            //iterate thru each line in list, store extracted timestamp string
            for (int i = 0; i < rawlistoflines.Count(); i++)
            {
                string temp = getSingleDataEntry(rawlistoflines[i]);
                if (temp[0] == '-')
                {
                    temp = temp.TrimStart('-');
                    dataentrylist.Add(temp);
                }
                else
                {
                    //extracts timestamp method here
                    dataentrylist.Add(temp);
                }
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
            //int endTag = IndexOfOccurence(line, ",", 2);
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
                    target = target.Replace(":", "");
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
            int startTag = IndexOfOccurence(line, ",", 2) + 1;//+1 to exclude the comma

            //check if delimitation - exits
            if (startTag < 0)
            {
                return null;
            }
            //string variable used to collect string indexes
            String target = "";

            //variable will store start to end of string
            target = line.Substring(startTag, line.Length - startTag);//customized digit character length of all dataentries

            return target;
        }
        //check occurences of string 
        public static int IndexOfOccurence(string s, string match, int occurence)
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
        public static List<int> getListOfIntervals(List<int> rawtimestamps)
        {

            //list that will store time intervals for count number
            List<int> listofintervals = new List<int>();
            //starts the first element at 0
            listofintervals.Add(0);
            for (int i = 0; i < rawtimestamps.Count - 1; i++)
            {
                listofintervals.Add(rawtimestamps[i] - rawtimestamps[i + 1]);
            }
            return listofintervals;
        }

        public static List<string> getListofDirectory(string folderpath)
        {
            //container for file directories
            List<string> files;

            //check if path is not empty - show message
            if (folderpath == "" || folderpath == null)
            {
                MessageBox.Show("Please select a folder containing csv");
            }
            else
            {
                //stores each pathstring w/ .csv ext in container - from root
                files = System.IO.Directory.GetFiles(folderpath, "*.csv").ToList<string>();
                return files;
            }

            return null;
        }
        //takes a path string [datalogger csv file],parses 3 arrays [time, raw data, absolute data]
        public static void getDelimitedParse(String path, ProgressBar bar)
        {
            string csvfolder = SplitCSVFile(path);
            var listofcsvfiles = getListofDirectory(csvfolder);

            //array for compiled array
            var iracompiled = new List<String>();

            //progress bar values
            bar.Minimum = 0;
            bar.Value = 1;
            bar.Step = 1;
            bar.Maximum = 4;
            //arrays containing interval, raw data, and absolute data
            //increments progress bar status
            bar.PerformStep();
            foreach (string splitstringpath in listofcsvfiles)
            {
                var rawresults = ReadCSVFile(splitstringpath);
                //increments progress bar status
                bar.PerformStep();
                var intervals = getListOfIntervals(getListOfTimeStamps(rawresults));
                //increments progress bar status
                bar.PerformStep();
                var rawdataentries = getListOfDataEntries(rawresults);
                //increments progress bar status
                bar.PerformStep();
                var absolutedataentries = getListOfAbsoluteDataEntries(rawresults);
                //increments progress bar status
                bar.PerformStep();

                bar.Maximum = rawresults.Count - 4;

                //counter
                int onlydataeveryms = 1000;
                int target = 1000;
                //variable for tracking total time
                int collector = 0;
                //time object
                TimeSpan t;
                //format string time
                String tlapse;
                int alldatauntilms = 10000;

                //header  items for cvs file
                iracompiled.Add("Interval, Time(ms), Raw Value, Absolute Value");

                //account for every raw results
                for (int i = 1; i < rawresults.Count; i++)
                {
                    //Processes all Windows messages currently in the message queue. Prevents timeout exceptions do too long operations
                    System.Windows.Forms.Application.DoEvents();
                    //increments progress bar status
                    bar.PerformStep();
                    //condition that adds all data entries for the first 10 secs or 10000 ms
                    if (collector < 10000)
                    {
                        // calculate hr, mins,sec, remaining ms using total ms
                        t = TimeSpan.FromMilliseconds(collector);

                        tlapse = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                        t.Hours,
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);

                        int mslapse = (t.Hours * 60 * 60 * 1000) + (t.Minutes * 60 * 1000) + (t.Seconds * 1000) + t.Milliseconds;

                        //formats parse
                        iracompiled.Add(tlapse + "," + mslapse + "," + rawdataentries[i - 1].ToString() + "," + absolutedataentries[i - 1].ToString());
                        //update collector
                        collector += (intervals[i] * -1) % 60;
                    }
                    if (collector > 10000)
                    {
                        if (collector > target)
                        {
                            // calculate hr, mins,sec, remaining ms using total ms
                            t = TimeSpan.FromMilliseconds(collector);
                            tlapse = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                            t.Hours,
                            t.Minutes,
                            t.Seconds,
                            t.Milliseconds);

                            int mslapse = (t.Hours * 60 * 60 * 1000) + (t.Minutes * 60 * 1000) + (t.Seconds * 1000) + t.Milliseconds;

                            //formats parse
                            iracompiled.Add(tlapse + "," + mslapse + "," + rawdataentries[i - 1].ToString() + "," + absolutedataentries[i - 1].ToString());
                            //set target time 1sec increments
                            target = collector + 1000; //need seperate variable for 1000
                        }
                        //update collector
                        collector += (intervals[i] * -1) % 60;
                    }
                }
                //reset status bar
                bar.Value = 0;
                var file = splitstringpath;
                
                file = file.Replace(".csv", "_extract.csv");
                using (var stream = File.CreateText(file))
                {
                    foreach (string line in iracompiled)
                    {
                        stream.WriteLine(line);
                    }
                }
            }
        }

    }
}
