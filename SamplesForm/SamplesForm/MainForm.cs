using System;
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
    }
}