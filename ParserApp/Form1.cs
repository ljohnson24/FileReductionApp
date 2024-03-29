﻿using System;
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
        //List<String> parse;

        public Form1()
        {
            InitializeComponent();
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
            importFoldername = importTb.Text;
            //check if file is not empty
            if (importFoldername == "" || importFoldername==null || !File.Exists(importFoldername))
            {
                MessageBox.Show("Please select a datalogger output .csv file");
            }
            else
            {
                //check if selected file is a specific data logger output csv file
                if (DataLoggerParse.ValidateCSVFile(importFoldername))
                {
                    richTextBox1.Text = "Please wait, Program is processing. For very large files, this could take a while. Do not open any files that are processing, this could interrupt the process.";
                    DataLoggerParse.getDelimitedParse(importFoldername, progressBar1);
                    richTextBox1.Text = "Program is ready for use.";
                }
                else
                {
                    MessageBox.Show("The file selected was not a data logger output .csv file.");
                    importFoldername = "";
                    importTb.Text = importFoldername;
                    richTextBox1.Clear();

                }
                
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void logsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var file = importFoldername;
            //if (!(file == null || file == ""))
            //{
            //    //int pos = file.LastIndexOf("/");
            //    file = file.Replace(".csv", "_extract.csv");
            //    using (var stream = File.CreateText(file))
            //    {
            //        foreach (string line in parse)
            //        {
            //            stream.WriteLine(line);
            //        }
            //    }
            //}
            //else { MessageBox.Show("Please select datalogger output csv file"); }
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

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
