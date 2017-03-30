using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserApp
{
    public static class DataLogger
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
                }
                using (System.IO.StreamReader csvReader = new System.IO.StreamReader(csvFileNameWithPath))
                {
                    string lineStr;
                    arrofcsvlines = new String[listofcsvlines.Count];
                    int i = 0;
                    while ((lineStr = csvReader.ReadLine()) != null)
                    {
                        arrofcsvlines[i]= lineStr;
                        ++i;
                    }
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
            for(int i = 1; i < rawlistoflines.Count(); i++)
            {
                //extracts timestamp method here
                timestamplist[i] = getSingleTimeStamp(rawlistoflines[i]);

            }
            
            return timestamplist;
        }
        //takes a string and returns timestamps string
        public static int getSingleTimeStamp(String line)
        {
            if (line == null)
            {
                return -1;
            }
            //locates timestamp position by pos of 1st comma + **/**/**** +space
            int startTag = line.IndexOf(",")+1;//excludes 1st occurance of ','
            //searches for comma, semi colons for parse
            //check if delimitation exits
            if (startTag < 0)
            {
                return -1;
            }
            //string variable used to collect string indexes
            String target="";
            //counter variable used to iterate thru string indexes
            int count = 0;

            //loop thru line until end tag ","
            while (!(target.Contains(",")))
            {
                //variable will collect all indexes between start and last count
                target = line.Substring(startTag+11, count);//11 excludes date in timestamp
                //check if target contains ':', -trim
                if (target.Contains(":"))
                {
                    target = target.Replace(":","");
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
    }
   }
