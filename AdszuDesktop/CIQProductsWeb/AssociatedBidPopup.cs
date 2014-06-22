using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CIQDesktop.CIQBidServices;

namespace CIQDesktop
{
    public partial class AssociatedBidPopup : Form
    {
        public AssociatedBidPopup()
        {
            InitializeComponent();

            this.assocGridView.AutoGenerateColumns = false;
        }

        public List<CBid> BidList
        {
            set { this.assocGridView.DataSource = value; }
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
