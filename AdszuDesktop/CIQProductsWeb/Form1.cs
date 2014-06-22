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
    public partial class MainFrm : Form
    {
        private BidManager _bidMgr;
        private TagManager _tagMgr;
        private AdManager _adMgr;
        private CMember _cmember; // This is our authenticated user info. Null initially.

        public MainFrm()
        {
            InitializeComponent();

            _bidMgr = new BidManager(mainpanel);
            _tagMgr = new TagManager(tagpanel);
            _adMgr = new AdManager(adPanel);

            // We're not logged in. Don't make the tabs available.

            this.tabControl1.Enabled = false;
            _cmember = null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // We need different ProductServicesClients for bids vs. other uses because
            // valid values for Bids must include "Any" as a selection.

            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);

            this.mainpanel.Enter += new EventHandler(mainpanel_Enter);
            this.mainpanel.Leave += new EventHandler(mainpanel_Leave);

            this.tagpanel.Enter += new EventHandler(tagpanel_Enter);
            this.tagpanel.Leave += new EventHandler(tagpanel_Leave);

            this.adPanel.Enter += new EventHandler(adPanel_Enter);
            this.adPanel.Leave += new EventHandler(adPanel_Leave);
        }

        void LoginInitialize()
        {
            this.mainpanel_Enter(null, null);
            this.adPanel_Enter(null, null);
            this.tagpanel_Enter(null, null);
        }

        void LoginClear()
        {
            this.mainpanel_Leave (null, null);
            this.adPanel_Leave (null, null);
            this.tagpanel_Leave (null, null);
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _bidMgr = null;
            _tagMgr = null;
            _adMgr = null;
        }

        public void initProductUIControls(CIQProductServices.CProduct aProd, Panel apnl, string prefix, string productClass, bool isBidMode)
        {
            if (aProd != null && aProd.ProductProperties != null)
            {
                int i = 1;
                string controlName = "";

                Control ctrl;
                ComboBox cbo;

                foreach (CIQProductServices.CProperty cpy in aProd.ProductProperties)
                {
                    controlName = prefix + "L" + i.ToString();
                    ctrl = apnl.Controls[controlName];
                    ctrl.Text = cpy.PropertyName;

                    controlName = prefix + "C" + i.ToString();
                    cbo = (ComboBox)apnl.Controls[controlName];

                    cbo.DataSource = null;

                    if (cpy.ValidValues != null)
                    {
                        cbo.DataSource = cpy.ValidValues.Select(j => j.ToString()).ToList();
                    }

                    ctrl.Visible = true;
                    cbo.Visible = true;

                    i++;
                }
            }
        }

        private void productControlVisibility(Panel apnl, string prefix, bool visible)
        {
            int i;
            string controlName = "";
            Control ctrl;
            ComboBox cbo;

            for (i = 1; i <= 6; i++)
            {
                controlName = prefix + "L" + i.ToString();

                ctrl = apnl.Controls[controlName];
                //ctrl = this.mainpanel.Controls[controlName];
                ctrl.Visible = visible;

                controlName = prefix + "C" + i.ToString();

                //cbo = (ComboBox)this.mainpanel.Controls[controlName];
                cbo = (ComboBox)apnl.Controls[controlName];

                cbo.Visible = visible;
            }
        }

        private void updateProductProperty(ComboBox c, Label l, CIQProductServices.CProduct aProduct)
        {
            // - Any - indicates a null value. 
            if (c != null && c.SelectedItem != null)
            {
                // determine the property value. If it is "-Any-", use empty string, otherwise use
                // the value itself.

                string propVal = (c.SelectedItem.ToString() == "-Any-" ? "" : c.SelectedItem.ToString());

                // Get the name of my property. It's also the name of my associated label.
                string propName = l.Text.ToString();

                // Find the associated property
                foreach (CIQProductServices.CProperty cprop in aProduct.ProductProperties)
                {
                    if (cprop.PropertyName == propName)
                    {
                        cprop.PropertyValue = propVal;
                        break;
                    }
                }
            }
        }

        private void testTag_Click(object sender, EventArgs e)
        {
            CIQAffiliateServices.ProductProperties pp;
            CIQAffiliateServices.CIQPlatformClient afs = new CIQAffiliateServices.CIQPlatformClient ();

            pp = afs.GetProductInformation(Guid.NewGuid(), this.tagText.Text.ToString(), 0L, 0L);

            this.TagProperties.Items.Clear();

            foreach (KeyValuePair<string,string> cprop in pp)
            {
                this.TagProperties.Items.Add(cprop.Key + " - " + cprop.Value);
            }
        }

        private void LogInOut_Click(object sender, EventArgs e)
        {
            // See if we're logged in. If we are NOT, then the _cmember will be null.

            if (_cmember == null)
            {
                // Show our login form and get the result. The login form will do the 
                // authentication and won't allow OK until a valid user is logged in.

                Login lgForm = new Login();

                DialogResult dlgRes = lgForm.ShowDialog(this);

                // We have a valid login. Set the _cmember private and display accordingly.

                if (dlgRes == DialogResult.OK)
                {
                    _cmember = lgForm.MemberInfo;
                    this.tabControl1.Enabled = true;

                    welcomeLabel.Text = "Welcome " + _cmember.First.ToString();
                    LogInOut.Text = "Logout...";

                    LoginInitialize();
                }
            }

            // We're already logged in. So log out.
            else
            {
                _cmember = null;
                this.tabControl1.Enabled = false;
                welcomeLabel.Text = "";
                LogInOut.Text = "Log In...";

                LoginClear();
            }
        }
    }
}
