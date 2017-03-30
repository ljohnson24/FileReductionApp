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
            var definedresults2 = DataLoggerParse.getListOfDataEntries(results);
            if(definedresults.Length == definedresults.Length)
            {
                MessageBox.Show("Shaun everything is ok, both arrays are equal as it should be");
            }
            foreach (var line in definedresults)
            {
                MessageBox.Show(line);
            }
        }
    }
}
