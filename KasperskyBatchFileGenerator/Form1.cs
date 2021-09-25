using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KasperskyBatchFileGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Internet Security");
            comboBox1.Items.Add("Total Security");

            string directory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string templatePath = System.IO.Path.Combine(directory, "templates", "template.txt");
            if (!System.IO.File.Exists(templatePath))
            {
                button1.Enabled = false;
                MessageBox.Show("Template file is missing...");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool valid = false;

            if (string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                valid = false;
                MessageBox.Show("Please select product...");
            }
            else
            {
                valid = true;
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                valid = false;
                MessageBox.Show("Please specify version...");
            }
            else
            {
                // e.g. 21.3.10.391
                Regex regex = new Regex(@"\b\d{1,2}\.\d{1}\.\d{1,2}\.\d{1,4}\b");
                valid = regex.Match(textBox1.Text).Success;

                if (!valid)
                {
                    MessageBox.Show("Please specify correct version format...");
                }
            }

            if (valid)
            {
                folderBrowserDialog1.ShowDialog();
                string targetFolder = folderBrowserDialog1.SelectedPath;

                // e.g. TS
                string abbreviation = string.Join("", comboBox1.Text
                    .Split(' ')
                    .Select(x => x.Substring(0, 1)));
                // e.g. KTS
                string fileName = $"K{abbreviation} {textBox1.Text} Trial Resetter Cheater.bat";

                string product = comboBox1.Text;

                // e.g. 21.3.10.391
                string version = textBox1.Text;

                // e.g. 21.3
                string shortVersion = string.Join(".", 
                    textBox1.Text
                    .Split(".")
                    .Select((x, i) => (i <= 1) ? x : "")
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
                
                string directory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                string templatePath = System.IO.Path.Combine(directory, "templates", "template.txt");

                string content = System.IO.File.ReadAllText(templatePath);
                string contentRevised = content.Replace("[PRODUCT]", product);
                contentRevised = contentRevised.Replace("[VERSION]", version);
                contentRevised = contentRevised.Replace("[SHORT_VERSION]", shortVersion);

                System.IO.File.WriteAllTextAsync(System.IO.Path.Combine(targetFolder, fileName), contentRevised).GetAwaiter().GetResult();

                textBox1.Text = "";
                comboBox1.Text = "";

                MessageBox.Show("Finished...");
            }
        }
    }
}
