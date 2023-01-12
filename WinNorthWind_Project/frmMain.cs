using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinNorthWind_Project
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnFrmOrder_Click(object sender, EventArgs e)
        {
            frmOrder frm = new frmOrder();
            frm.MdiParent = this;
            frm.Show();
        }

        private void btnFrmEmployee_Click(object sender, EventArgs e)
        {
            frmEmployee frm = new frmEmployee();
            frm.MdiParent = this;
            frm.Show();
        }

        private void btnFrmBOM_Click(object sender, EventArgs e)
        {
            frmBOM frm = new frmBOM();
            frm.MdiParent = this;
            frm.Show();
        }
    }
}
