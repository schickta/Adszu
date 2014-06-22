using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CIQDesktop.CIQMemberServices;
using CIQDesktop.CIQAdServices;
using CIQDesktop.CIQBidServices;

namespace CIQDesktop
{
    public partial class MainFrm : Form
    {
        private class AdManager
        {
            private CIQAdServicesClient _adClient;
            private CIQBidServicesClient _bidClient;
            private CAd _cAd;
            private Panel _parent;

            public AdManager(Panel parentPanel)
            {
                _parent = parentPanel;

                _adClient = new CIQAdServicesClient ();
                _bidClient = new CIQBidServicesClient();
                _cAd = null;
            }

            ~AdManager()
            {
                _cAd = null;
                _adClient.Close();
                _bidClient.Close();
            }

            public void Initialize()
            {
                if (_adClient != null)
                {
                    _cAd = _adClient.CreateAdInstance();
                }
            }

            public void Clear()
            {
                MainFrm mf = (MainFrm)_parent.TopLevelControl;
                mf.dataGridView1.DataSource = null;
                mf.dataGridView1.Refresh();

                _cAd = null;
            }

            public void clearFields()
            {
                MainFrm mf = (MainFrm)_parent.TopLevelControl;

                mf.adTitle.Text = "";
                mf.adDescription.Text = "";
                mf.imageUrl.Text = "";
                mf.brandImageUrl.Text = "";
                mf.discountAmount.Text = "";
                mf.isPercentage.Checked = false;
                mf.discountCode.Text = "";
                mf.WebImagePreview.ImageLocation = "";
            }

            public void initializeListView()
            {
                CAd[] cAds;
                MainFrm mf = (MainFrm)_parent.TopLevelControl;
                CMember cmem = mf._cmember;

                clearFields();

                if (_adClient != null && cmem != null)
                {
                    cAds = _adClient.GetAds (cmem.ID);
                    mf.dataGridView1.AutoGenerateColumns = false;
                    mf.dataGridView1.DataSource = cAds;
                }
            }

            public bool Save()
            {
                bool retVal = false;

                if (_cAd != null && _adClient != null)
                {
                    // Here we're gonna get the ID Of the logged in member and 
                    // set that as the MarketerID.

                    MainFrm mf = (MainFrm)_parent.TopLevelControl;
                    _cAd.MarketerID = mf._cmember.ID;

                    retVal = _adClient.SaveAd(_cAd);
                }

                return (retVal);
            }

            public bool Delete (int adID)
            {
                bool retVal = false;

                if (_adClient != null)
                {
                    retVal = _adClient.DeleteAd(adID, true);
                }

                return (retVal);
            }

            public List<CBid> AssociatedBids(int adID)
            {
                List<CBid> retVal = new List<CBid>();

                if (_bidClient != null)
                {
                    CBid[] bidList = _bidClient.AssociatedToAd (adID);

                    if (bidList != null)
                    {
                        retVal = bidList.ToList<CBid>();
                    }
                }
                 
                return (retVal);
            }

            public bool Modify(int ID)
            {
                bool retVal = false;

                if (_adClient != null && _cAd != null)
                {
                    _cAd.ID = ID;

                    retVal = _adClient.UpdateAd(_cAd);
                    retVal = true;
                }

                return (retVal);
            }

            public string Title
            {
                get { return (_cAd.Title); }
                set { _cAd.Title = value; }
            }

            public string AdText
            {
                get { return (_cAd.AdText); }
                set { _cAd.AdText = value; }
            }

            public string ImageUrl
            {
                get { return (_cAd.ImageUrl); }
                set { _cAd.ImageUrl = value; }
            }

            public string BrandImageUrl
            {
                get { return (_cAd.BrandImageUrl); }
                set { _cAd.BrandImageUrl = value; }
            }

            public float DiscountAmount
            {
                get { return (_cAd.DiscountAmount); }
                set { _cAd.DiscountAmount = value; }
            }

            public bool IsDiscountAPercentage
            {
                get { return (_cAd.IsDiscountAPercentage); }
                set { _cAd.IsDiscountAPercentage = value; }
            }

            public string DiscountCode
            {
                get { return (_cAd.DiscountCode); }
                set { _cAd.DiscountCode = value; }
            }
        }

        private void SaveAd_Click(object sender, EventArgs e)
        {
            if (_adMgr != null)
            {
                try
                {
                    _adMgr.Title = this.adTitle.Text.ToString();
                    _adMgr.AdText = this.adDescription.Text.ToString();
                    _adMgr.ImageUrl = this.imageUrl.Text.ToString();
                    _adMgr.BrandImageUrl = this.brandImageUrl.Text.ToString();
                    _adMgr.DiscountAmount = (float)System.Convert.ToDecimal(this.discountAmount.Text.ToString());
                    _adMgr.IsDiscountAPercentage = this.isPercentage.Checked;
                    _adMgr.DiscountCode = this.discountCode.Text.ToString();

                    if (_adMgr.Save())
                    {
                        _adMgr.initializeListView();

                        if (dataGridView1.Rows.Count > 0)
                        {
                            int rowIdx = dataGridView1.Rows.Count - 1;

                            dataGridView1.Rows[rowIdx].Selected = true;
                            dataGridView1.CurrentCell = dataGridView1[0, rowIdx];
                            dataGridView1.FirstDisplayedScrollingRowIndex = rowIdx;
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Problem with the save");
                }
            }
        }

        private bool associatedBidCheckOK(int anId)
        {
            bool retVal = true;
            List<CBid> assocBids = _adMgr.AssociatedBids(anId);

            if (assocBids.Count() > 0)
            {
                AssociatedBidPopup adRef = new AssociatedBidPopup();
                DialogResult dResult;

                adRef.BidList = assocBids;
                dResult = adRef.ShowDialog();

                retVal = (dResult == DialogResult.OK ? true : false);
            }

            return (retVal);
        }

        private void DeleteAd_Click(object sender, EventArgs e)
        {
            if (_adMgr != null)
            {
                try
                {
                    DataGridViewRow dgr = dataGridView1.CurrentRow;

                    if (dgr != null)
                    {
                        int currRowNum = dataGridView1.Rows.IndexOf(dgr);
                        var product = (CAd)dgr.DataBoundItem;



                        if (associatedBidCheckOK(product.ID))
                        {
                            if (_adMgr.Delete(product.ID) == true)
                            {
                                _adMgr.initializeListView();

                                if (dataGridView1.Rows.Count > 0)
                                {
                                    currRowNum = (currRowNum < dataGridView1.Rows.Count ?
                                            currRowNum : dataGridView1.Rows.Count - 1);

                                    dataGridView1.Rows[currRowNum].Selected = true;
                                    dataGridView1.CurrentCell = dataGridView1[0, currRowNum];
                                    dataGridView1.FirstDisplayedScrollingRowIndex = currRowNum;
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {                
                    MessageBox.Show("Delete Failed");
                }
            }
        }

        private void ModifyAd_Click(object sender, EventArgs e)
        {
            if (_adMgr != null)
            {
                try
                {
                    DataGridViewRow dgr = dataGridView1.CurrentRow;

                    if (dgr != null)
                    {
                        int currRowNum = dataGridView1.Rows.IndexOf(dgr);
                        var product = (CAd)dgr.DataBoundItem;

                        if (product != null) {

                            _adMgr.Title = this.adTitle.Text.ToString();
                            _adMgr.AdText = this.adDescription.Text.ToString();
                            _adMgr.ImageUrl = this.imageUrl.Text.ToString();
                            _adMgr.BrandImageUrl = this.brandImageUrl.Text.ToString(); 
                            _adMgr.DiscountAmount = (float) float.Parse (this.discountAmount.Text.ToString ());
                            _adMgr.IsDiscountAPercentage = this.isPercentage.Checked;
                            _adMgr.DiscountCode = this.discountCode.Text.ToString();

                            if (_adMgr.Modify(product.ID))
                            {
                                _adMgr.initializeListView();

                                if (dataGridView1.Rows.Count > 0)
                                {
                                    currRowNum = (currRowNum < dataGridView1.Rows.Count ?
                                            currRowNum : dataGridView1.Rows.Count - 1);

                                    dataGridView1.Rows[currRowNum].Selected = true;
                                    dataGridView1.CurrentCell = dataGridView1[0, currRowNum];
                                    dataGridView1.FirstDisplayedScrollingRowIndex = currRowNum;
                                }
                            }
                        }
                    }
                }
                catch (Exception) 
                {
                    MessageBox.Show("Modify Failed");
                }
            }
        }

        private void URLTestButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.WebImagePreview.ImageLocation = this.imageUrl.Text.ToString();
            }
            catch (Exception)
            {
                this.WebImagePreview.ImageLocation = null;
            }
        }

        private void brandUrlTestButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.LogoPreview.ImageLocation = this.brandImageUrl.Text.ToString();
            }
            catch (Exception)
            {
                this.LogoPreview.ImageLocation = null;
            }
        }

        void adPanel_Leave(object sender, EventArgs e)
        {
            _adMgr.Clear();
        }

        void adPanel_Enter(object sender, EventArgs e)
        {
            _adMgr.Initialize();
            _adMgr.initializeListView();
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {            
            if (e.RowIndex >= 0)
            {
                DataGridViewRow dgr = dataGridView1.Rows[e.RowIndex];

                if (dgr != null)
                {
                    var product = (CAd)dgr.DataBoundItem;

                    if (product != null)
                    {
                        this.adTitle.Text = product.Title.ToString();
                        this.adDescription.Text = product.AdText.ToString();
                        this.imageUrl.Text = product.ImageUrl.ToString();
                        this.brandImageUrl.Text = product.BrandImageUrl.ToString();
                        this.discountAmount.Text = String.Format ("{0:0,0.00}", product.DiscountAmount);
                        this.isPercentage.Checked = product.IsDiscountAPercentage;
                        this.discountCode.Text = product.DiscountCode.ToString();

                        if (product.ImageUrl != null && product.ImageUrl.Length > 0)
                        {
                            this.WebImagePreview.ImageLocation = product.ImageUrl.ToString();
                        }

                        if (product.BrandImageUrl != null && product.BrandImageUrl.Length > 0)
                        {
                            this.LogoPreview.ImageLocation = product.BrandImageUrl.ToString();
                        }
                    }
                }
            }
        }
    }
}
