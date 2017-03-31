using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParserApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //for my pc
            //string path = "C:\\Users\\CSSE\\Desktop\\shawn files\\dat00001.csv";

            //for work laptop
            string path = "C:\\Users\\johnsonl\\Desktop\\shawn files\\dat00002.csv";
            var results = DataLoggerParse.ReadCSVFile(path);
            var definedresults = DataLoggerParse.getListOfTimeStamps(results);
            var intervals = DataLoggerParse.getListOfIntervals(definedresults);
            var definedresults2 = DataLoggerParse.getListOfDataEntries(results);
            var definedresults3 = DataLoggerParse.getListOfAbsoluteDataEntries(results);

            var parse = DataLoggerParse.getDelimitedParse(path);
            if (definedresults.Length == definedresults.Length && definedresults.Length == definedresults3.Length && definedresults.Length ==intervals.Length+1)
            {
                richTextBox1.AppendText("Shaun everything is ok, both arrays are equal as it should be\n");
                
            }
            else { richTextBox1.AppendText("Damn!!!Damn!!Damn!!\n"); }

            foreach (var line in parse)
            {// line.ToString("0.##########") for digit precision 
                richTextBox1.AppendText("" +line+"\n");
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
