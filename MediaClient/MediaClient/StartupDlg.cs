using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaClient
{
    public partial class StartupDlg : Form
    {
        public String ServerName
        {
            get { return this.textBoxServerName.Text; }
            set { this.textBoxServerName.Text = value; }
        }
        public String ServerPort
        {
            get { return this.textBoxServerPort.Text; }
            set { this.textBoxServerPort.Text = value; }
        }
        public String Filename
        {
            get { return this.textBoxFileName.Text; }
            set { this.textBoxFileName.Text = value; }
        }

        public StartupDlg()
        {
            InitializeComponent();
        }

        private void StartupDlg_Load(object sender, EventArgs e)
        {

        }
    }
}
