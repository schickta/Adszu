using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CIQDesktop.CIQMemberServices;

namespace CIQDesktop
{
    public partial class Login : Form
    {
        public CMember _cmember;

        public CMember MemberInfo
        {
            get { return (_cmember); }
            set { _cmember = value; }
        }

        public Login()
        {
            InitializeComponent();

   
            this.AcceptButton = loginButton;
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            CIQMemberServicesClient memSvc = new CIQMemberServicesClient();
            
            MemberInfo = memSvc.AuthenticateMember(userName.Text.ToString (), 
                                                   password.Text.ToString ());
            loginStatus.Text = "";

            if (MemberInfo == null)
            {
                loginStatus.Text = "Invalid Login";
            }
            else
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
