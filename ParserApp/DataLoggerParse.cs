﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace ParserApp
{
    public static class DataLoggerParse
    {
        public static void CallToChildThread()
        {

        }

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
            //Processes all Windows messages currently in the message queue. Prevents timeout exceptions do too long operations
            System.Windows.Forms.Application.DoEvents();
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
                    {
                        //Processes all Windows messages currently in the message queue. Prevents timeout exceptions do too long operations
                        System.Windows.Forms.Application.DoEvents();
                        //check if read is below max lines
                        lineStr = fileStream.ReadLine();

                        if (outputfile == null)
                        {
                            outputfile = new System.IO.StreamWriter(
                                string.Format(dir + filename + "_{0:D2}.csv", i++),
                                false,
                                fileStream.CurrentEncoding);
                        }

                        if (count < max)
                        {
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
                            //Processes all Windows messages currently in the message queue. Prevents timeout exceptions do too long operations
                            System.Windows.Forms.Application.DoEvents();
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
        //split method
        public static void Split(string inputfile, string outputfilesformat)
        {
            int i = 0;
            System.IO.StreamWriter outfile = null;
            string line;

            try
            {
                using (var infile = new System.IO.StreamReader(inputfile))
                {
                    while (!infile.EndOfStream)
                    {
                        //Processes all Windows messages currently in the message queue. Prevents timeout exceptions do too long operations
                        System.Windows.Forms.Application.DoEvents();
                        line = infile.ReadLine();
                        
                            outfile = new System.IO.StreamWriter(
                                string.Format(outputfilesformat, i++),
                                false,
                                infile.CurrentEncoding);
                        
                        outfile.WriteLine(line);
                    }

                }
            }
            finally
            {
                if (outfile != null)
                    outfile.Dispose();
            }
        }

        //method takes a raw list of data lines, extracts timestamps and returns array
        public static List<int> getListOfTimeStamps(List<String> rawlistoflines)
        {
            int max = 86345000;
            bool maxday = false;//tracks days
            int hrsover = 1; // tracks hrs over a day
            //array variable that will store extracted timestamps
            List<int> timestamplist = new List<int>();

            //iterate thru each line in list, store extracted timestamp string
            for (int i = 0; i < rawlistoflines.Count(); i++)
            {
                //Processes all Windows messages currently in the message queue. Prevents timeout exceptions do too long operations
                System.Windows.Forms.Application.DoEvents();
                //evaluate timestamp to verify max day
                if (getSingleTimeStamp(rawlistoflines[i]) > 86399990 * hrsover)
                {
                    maxday = true;
                }
                if (maxday && getSingleTimeStamp(rawlistoflines[i]) < 86399990 * hrsover)
                {
                    timestamplist.Add(getSingleTimeStamp(rawlistoflines[i])+(max*hrsover));
                    continue;
                }
                if (getSingleTimeStamp(rawlistoflines[i]) > 86399990 * (hrsover + 1))
                {
                    maxday = false;
                    ++hrsover;
                }
                
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
                //Processes all Windows messages currently in the message queue. Prevents timeout exceptions do too long operations
                System.Windows.Forms.Application.DoEvents();
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
                //Processes all Windows messages currently in the message queue. Prevents timeout exceptions do too long operations
                System.Windows.Forms.Application.DoEvents();
                string temp = getSingleDataEntry(rawlistoflines[i]);
                if (temp[0] == '-')
                {
                    temp = temp.TrimStart('-');
                    dataentrylist.Add(temp);
                }
                else if (temp[0] == '+')
                {
                    temp = temp.TrimStart('+');
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
                //Processes all Windows messages currently in the message queue. Prevents timeout exceptions do too long operations
                System.Windows.Forms.Application.DoEvents();
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
            //parse and convert string timestamp to ms
            string hr = ""+target[0]+target[1];
            string min = "" + target[2] + target[3];
            string sec = "" + target[4] + target[5];
            string ms = "" + target[6] + target[7]+target[8];

            int mslapse = (Convert.ToInt32(hr) * 60 * 60 * 1000) + (Convert.ToInt32(min) * 60 * 1000) + (Convert.ToInt32(sec) * 1000) + Convert.ToInt32(ms);
            return mslapse;
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
                //Processes all Windows messages currently in the message queue. Prevents timeout exceptions do too long operations
                System.Windows.Forms.Application.DoEvents();
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
                //Processes all Windows messages currently in the message queue. Prevents timeout exceptions do too long operations
                System.Windows.Forms.Application.DoEvents();
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
                files.Sort();
                return files;
            }

            return null;
        }
        //takes a path string [datalogger csv file],parses 3 arrays [time, raw data, absolute data]
        public static void getDelimitedParse(String path, ProgressBar bar)
        {
            //progress bar values
            bar.Minimum = 0;
            bar.Value = 10;
            bar.Step = 10;
            bar.Maximum = 50;
            
            bar.PerformStep();
            
            string csvfolder = SplitCSVFile(path);
            bar.PerformStep();
            bar.PerformStep();
            var listofcsvfiles = getListofDirectory(csvfolder);
            bar.PerformStep();
            //counter

            int target = 1000;
            //variable for tracking total time
            int collector = 0;
            //time object
            TimeSpan t;
            //format string time
            String tlapse;
            int mslapse;
            int j=0;//file multipler for collector

            bar.Maximum = listofcsvfiles.Count * 10 * 7;
            bar.Value = 10;
            
            
            foreach (string splitstringpath in listofcsvfiles)
            {
                //Processes all Windows messages currently in the message queue. Prevents timeout exceptions do too long operations
                System.Windows.Forms.Application.DoEvents();
                ++j;// increment j for every file iteration
                //array for compiled array
                bar.PerformStep();
                var iracompiled = new List<String>();
                //arrays containing interval, raw data, and absolute data
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
                
                //header  items for cvs file
                iracompiled.Add("Interval, Time(ms), Raw Value, Absolute Value");

                //account for every raw results
                for (int i = 1; i < rawresults.Count; i++)
                {
                    //Processes all Windows messages currently in the message queue. Prevents timeout exceptions do too long operations
                    System.Windows.Forms.Application.DoEvents();
                    
                    //condition that adds all data entries for the first 10 secs or 10000 ms
                    if (collector < 100000*j)
                    {
                        // calculate hr, mins,sec, remaining ms using total ms
                        t = TimeSpan.FromMilliseconds(collector);

                        tlapse = string.Format("{0:D2}d:{1:D2}h:{2:D2}m:{3:D2}s:{4:D3}ms",
                        t.Days,
                        t.Hours,
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);

                        mslapse = (t.Days * 24 * 60 * 60 * 1000) + (t.Hours * 60 * 60 * 1000) + (t.Minutes * 60 * 1000) + (t.Seconds * 1000) + t.Milliseconds;
                        //excludes lines contain zero data
                        if (!rawdataentries[i-1].ToString().Contains("0.00000000E"))
                        {
                            //formats parse
                            iracompiled.Add(tlapse + "," + mslapse + "," + rawdataentries[i - 1].ToString() + "," + absolutedataentries[i - 1].ToString());

                            //update collector
                            collector += (intervals[i] * -1);
                            
                        }
                        
                    }
                    if (collector >= 100000*j)
                    {
                        if (collector >= target)
                        {
                            // calculate hr, mins,sec, remaining ms using total ms
                            t = TimeSpan.FromMilliseconds(collector);
                            tlapse = string.Format("{0:D2}d:{1:D2}h:{2:D2}m:{3:D2}s:{4:D3}ms",
                            t.Days,
                            t.Hours,
                            t.Minutes,
                            t.Seconds,
                            t.Milliseconds);

                            mslapse = (t.Days * 24 * 60 * 60 * 1000)+(t.Hours * 60 * 60 * 1000) + (t.Minutes * 60 * 1000) + (t.Seconds * 1000) + t.Milliseconds;
                            if (!rawdataentries[i - 1].ToString().Contains("0.00000000E"))
                            {
                                //formats parse
                                iracompiled.Add(tlapse + "," + mslapse + "," + rawdataentries[i - 1].ToString() + "," + absolutedataentries[i - 1].ToString());
                                
                            }
                            //set target time 1sec increments
                            target = collector + 1000; //need seperate variable for 1000
                        }

                            //update collector
                            collector += (intervals[i] * -1);
                    }
                }

                //Processes all Windows messages currently in the message queue. Prevents timeout exceptions do too long operations
                System.Windows.Forms.Application.DoEvents();
                bar.PerformStep();
                
                var file = splitstringpath.Replace(".csv", "_Parse.csv");
                using (var stream = File.CreateText(file))
                {
                    foreach (string line in iracompiled)
                    {
                        //Processes all Windows messages currently in the message queue. Prevents timeout exceptions do too long operations
                        System.Windows.Forms.Application.DoEvents();
                        
                        stream.WriteLine(line);
                    }
                    
                }
                
            }
            bar.Value = 0;
        }

    }
}
