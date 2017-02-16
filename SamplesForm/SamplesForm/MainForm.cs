using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SamplesForm.Model.Data;

namespace SamplesForm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            this.InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var myMoneyZeroCracker = new MyMoneyZeroCracker();

            var email = "supershowwei@gmail.com";

            var licenseKey = myMoneyZeroCracker.GenerateLicenseKey(email);

            var result = myMoneyZeroCracker.DecodeLicenseKey(licenseKey).Equals(email);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var currentPath = Assembly.GetExecutingAssembly().Location;

            this.textBox1.AppendText("執行檔絕對路徑：" + currentPath + Environment.NewLine + Environment.NewLine);

            var directory = File.GetAttributes(currentPath).HasFlag(FileAttributes.Directory)
                                ? currentPath
                                : Path.GetDirectoryName(currentPath);

            this.textBox1.AppendText(
                "UpLevel 1: " + Path.GetFullPath(Path.Combine(directory, @"..\")) + Environment.NewLine);
            this.textBox1.AppendText(
                "UpLevel 2: " + Path.GetFullPath(Path.Combine(directory, @"..\..\")) + Environment.NewLine);
            this.textBox1.AppendText(
                "UpLevel 3: " + Path.GetFullPath(Path.Combine(directory, @"..\..\..\")) + Environment.NewLine);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new SqlBulkCopySample().BulkInsert(
                new List<SecuritiesTransaction> { new SecuritiesTransaction { Date = "20170216", StockNo = "2330" } });
        }
    }
}