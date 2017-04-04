using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParserApp
{
    public partial class Form1 : Form
    {
        string importFoldername;
        string importDirectory;
        List<String> parse;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //for my pc
            string path = "C:\\Users\\CSSE\\Desktop\\shawn files\\dat00002.csv";

            //for work laptop
            //string path = "C:\\Users\\johnsonl\\Desktop\\shawn files\\dat00002.csv";
            var results = DataLoggerParse.ReadCSVFile(path);
            var definedresults = DataLoggerParse.getListOfTimeStamps(results);
            var intervals = DataLoggerParse.getListOfIntervals(definedresults);
            var definedresults2 = DataLoggerParse.getListOfDataEntries(results);
            var definedresults3 = DataLoggerParse.getListOfAbsoluteDataEntries(results);

            var parse = DataLoggerParse.getDelimitedParse(path, progressBar1);
            if (definedresults.Count == definedresults.Count && definedresults.Count == definedresults3.Count && definedresults.Count ==intervals.Count)
            {
                richTextBox1.AppendText("Shaun everything is ok, all arrays are equal as it should be\n");
                richTextBox1.AppendText("timestamp array: " + definedresults.Count+"\n");
                richTextBox1.AppendText("data entries: " + definedresults2.Count + "\n");
                richTextBox1.AppendText("absolute data entries: " + definedresults3.Count + "\n");
                richTextBox1.AppendText("interval list: " + intervals.Count + "\n");
                richTextBox1.AppendText("Total parse array: " + parse.Count+"\n");
            }
            else
            {
                richTextBox1.AppendText("Damn!!!Damn!!Damn!!\n");
                richTextBox1.AppendText("timestamp array: " + definedresults.Count + "\n");
                richTextBox1.AppendText("data entries: " + definedresults2.Count + "\n");
                richTextBox1.AppendText("absolute data entries: " + definedresults3.Count + "\n");
                richTextBox1.AppendText("interval list: " + intervals.Count + "\n");
                richTextBox1.AppendText("Total parse array: " + parse.Count + "\n");
            }
            //richTextBox1.AppendText("interval element "+intervals[10877]+"\n");
            foreach (var line in parse)
            {// line.ToString("0.##########") for digit precision 
                richTextBox1.AppendText("" +line+"\n");
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void browseImportBtn_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();

            //default the folder view to user document
            openFileDialog1.InitialDirectory = "C:\\Users\\Public\\Documents";
            //title the window
            openFileDialog1.Title = "Select a Data Logger Output .CSV File";
            openFileDialog1.Filter = "My files (*.CSV)|*.CSV|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //store the returned path to variable
                importFoldername = Path.GetFullPath(openFileDialog1.FileName);
                //display returned path to ui text box
                importTb.Text = importFoldername;
            }
        }

        private void importTb_TextChanged(object sender, EventArgs e)
        {
            importFoldername = importTb.Text;
        }

        private void loadImportBtn_Click(object sender, EventArgs e)
        {
            //check if file is not empty
            if (importFoldername == "" || importFoldername==null)
            {
                MessageBox.Show("Please select a datalogger output .csv file");
            }
            else
            {
                parse = DataLoggerParse.getDelimitedParse(importFoldername, progressBar1);
                richTextBox1.Text = "Data Lines: " + parse.Count + "\n";
                
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void logsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var file = importFoldername;
            if (!(file == null || file == ""))
            {
                //int pos = file.LastIndexOf("/");
                file = file.Replace(".csv", "_extract.csv");
                using (var stream = File.CreateText(file))
                {
                    foreach (string line in parse)
                    {
                        stream.WriteLine(line);
                    }
                }
            }
            else { MessageBox.Show("Please select datalogger output csv file"); }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();

            //default the folder view to user document
            openFileDialog1.InitialDirectory = "C:\\Users\\Public\\Documents";
            //title the window
            openFileDialog1.Title = "Select a Data Logger Output .CSV File";
            openFileDialog1.Filter = "My files (*.CSV)|*.CSV|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //store the returned path to variable
                importFoldername = Path.GetFullPath(openFileDialog1.FileName);
                importDirectory = Path.GetDirectoryName(openFileDialog1.FileName);
                //display returned path to ui text box
                importTb.Text = importFoldername;
            }
        }
    }
}
