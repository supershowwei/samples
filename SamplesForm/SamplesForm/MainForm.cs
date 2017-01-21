using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SamplesForm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
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

            textBox1.AppendText("執行檔絕對路徑：" + currentPath + Environment.NewLine + Environment.NewLine);

            var directory = File.GetAttributes(currentPath).HasFlag(FileAttributes.Directory)
                ? currentPath
                : Path.GetDirectoryName(currentPath);

            textBox1.AppendText(
                "UpLevel 1: " + Path.GetFullPath(Path.Combine(directory, @"..\")) + Environment.NewLine);
            textBox1.AppendText(
                "UpLevel 2: " + Path.GetFullPath(Path.Combine(directory, @"..\..\")) + Environment.NewLine);
            textBox1.AppendText(
                "UpLevel 3: " + Path.GetFullPath(Path.Combine(directory, @"..\..\..\")) + Environment.NewLine);
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }
    }
}