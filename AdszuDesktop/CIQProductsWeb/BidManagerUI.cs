using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CIQDesktop.CIQMemberServices;
using CIQDesktop.CIQBidServices;
using CIQDesktop.CIQProductServices;
using CIQDesktop.CIQAdServices;

namespace CIQDesktop
{
    public partial class MainFrm : Form
    {
        private CAd[] _adList;

        private class BidManager
        {
            private CIQProductServices.CIQProductServicesClient _bidProductClient;
            private CIQBidServices.CIQBidServicesClient _bidClient;
            private CProduct _bmProduct;
            private CBid _bmBid;

            private Panel _parent;

            public BidManager(Panel parentPanel)
            {
                _parent = parentPanel;

                _bidProductClient = new CIQProductServices.CIQProductServicesClient();
                _bmProduct = null;

                _bidClient = new CIQBidServices.CIQBidServicesClient();
                _bmBid = _bidClient.CreateBidInstance();
            }

             ~BidManager()
            {
                _bmBid = null;
                _bmProduct = null;
                
                _bidProductClient.Close();
                _bidClient.Close();
            }

            public void Initialize ()
            {
                if (_bidClient != null)
                {
                    _bmBid = _bidClient.CreateBidInstance();
                }
            }

            public void Clear()
            {
                _bmProduct = null;
                _bmBid = null;
            }


            public void initializeListView()
            {
                CBid[] cBids;
                MainFrm mf = (MainFrm)_parent.TopLevelControl;
                CMember cmem = mf._cmember;

                //clearFields();

                if (_bidClient != null && cmem != null)
                {
                    cBids = _bidClient.GetBids(cmem.ID);
                    mf.bidGridView.AutoGenerateColumns = false;
                    mf.bidGridView.DataSource = cBids;
                }
            }

            public string [] GetProductCategories()
            {
                return (_bidProductClient.GetProductCategories (true));
            }

            public string[] GetProductSubcategories()
            {
                return (_bidProductClient.GetProductSubcategories (_bmBid.ProductCategory, true));
            }

            public string[] GetProductClassifications()
            {
                return (_bidProductClient.GetProductClassifications (_bmBid.ProductCategory, _bmBid.ProductSubcategory, true));
            }

            public void SetClass(string productClass)
            {
                this.ProductClass = productClass;

                if (productClass != null && productClass.Length > 0)
                {
                    _bmProduct = _bidProductClient.CreateProductInstance(ProductClass, true);
                }
                else
                {
                    _bmProduct = null;
                }
            }

            public CProduct SetProductObject(string prodObj)
            {
                _bmProduct = _bidProductClient.Deserialize (prodObj);

                return (_bmProduct);
            }

            public bool Save()
            {
                String flatObj = "";
                bool retVal = false;

                if (_bmBid != null && _bidClient != null)
                {
                    if (_bmProduct != null)
                    {
                        _bidProductClient.Serialize(_bmProduct, ref flatObj);
                    }

                    // Here we're gonna get the ID Of the logged in member and 
                    // set that as the MarketerID.

                    MainFrm mf = (MainFrm)_parent.TopLevelControl;
                    _bmBid.MarketerID = mf._cmember.ID;

                    retVal = _bidClient.SaveBid(_bmBid, flatObj);
                }

                return (retVal);
            }

            public bool Delete(int bidID)
            {
                bool retVal = false;

                if (_bidClient != null)
                {
                    retVal = _bidClient.DeleteBid(bidID);
                }

                return (retVal);
            }

            public bool Modify(int ID)
            {
                string flatObj = "";
                bool retVal = false;

                if (_bidClient != null && _bmBid != null)
                {
                    if (_bidProductClient != null && _bmProduct != null)
                    {
                        _bidProductClient.Serialize(_bmProduct, ref flatObj);
                    }

                    _bmBid.ID = ID;

                    retVal = _bidClient.UpdateBid (_bmBid, flatObj);
                }

                return (retVal);
            }

            public double ComputeScanHistory()
            {
                double numScans;
                String flatObj = "";

                if (_bmProduct != null)
                {
                    _bidProductClient.Serialize(_bmProduct, ref flatObj);
                }

                numScans = _bidClient.ComputeScanHistory(_bmBid, flatObj, 30);

                return (numScans);
             }

            public int AssociatedAdID
            {
                get { return (_bmBid.AssociatedAdID); }

                set
                {
                    if (_bmBid != null)
                    {
                        _bmBid.AssociatedAdID = value;
                    }
                }
            }

            public string BidTitle
            {
                get { return (_bmBid.Title); }

                set
                {
                    if (_bmBid != null)
                    {
                        _bmBid.Title = value;
                    }
                }
            }

            public string Region
            {
                get { return (_bmBid.Region); }

                set
                {
                    if (_bmBid != null)
                    {
                        _bmBid.Region = value;
                    }
                }
            }

            public string TimeOfDay
            {
                get { return (_bmBid.TimeOfDay); }

                set
                {
                    if (_bmBid != null)
                    {
                        _bmBid.TimeOfDay = value;
                    }
                }
            }
  
            public string DayOfWeek
            {
                get { return (_bmBid.DayOfWeek); }

                set
                {
                    if (_bmBid != null)
                    {
                        _bmBid.DayOfWeek = value;
                    }
                }
            }

            public string ProductCategory
            {
                get { return (_bmBid.ProductCategory); }

                set
                {
                    if (_bmBid != null)
                    {
                        _bmBid.ProductCategory = value;
                    }
                }
            }

            public string ProductSubcategory
            {
                get { return (_bmBid.ProductSubcategory); }

                set
                {
                    if (_bmBid != null)
                    {
                        _bmBid.ProductSubcategory = value;
                    }
                }
            }

            public string ProductClass
            {
                get { return (_bmBid.ProductClass); }

                set
                {
                    if (_bmBid != null)
                    {
                        _bmBid.ProductClass = value;
                    }
                }
            }

            public CIQProductServices.CProduct ProductObject
            {
                get { return (_bmProduct); }
            }
        }

        void mainpanel_Leave(object sender, EventArgs e)
        {
            this.AdList.Items.Clear();
            _adList = null;
            _bidMgr.Clear();
        }

        private void initAdList()
        {
            CIQAdServicesClient cadClient = new CIQAdServicesClient();

            this.AdList.Items.Clear();
            CMember cmem = this._cmember;

            if (cmem != null)
            {
                _adList = cadClient.GetAds(cmem.ID);

                if (_adList.Count() > 0)
                {
                    string listItem;
                    string formatItem;

                    foreach (CIQAdServices.CAd cAd in _adList)
                    {
                        listItem = "";

                        if (cAd.IsDiscountAPercentage == true)
                            formatItem = cAd.DiscountAmount.ToString() + "%";

                        else
                            formatItem = String.Format("{0:C}", cAd.DiscountAmount);

                        listItem = listItem + String.Format("{0, -8}", formatItem);
                        listItem = listItem + cAd.Title;
                        this.AdList.Items.Add(listItem);
                    }
                }
            }
        }

        void mainpanel_Enter(object sender, EventArgs e)
        {                    
            this.SaveBid.Enabled = false;    
            initAdList();

            _bidMgr.Initialize();
            _bidMgr.initializeListView();

            this.categories.DataSource = _bidMgr.GetProductCategories();
        }

        private void AdList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.AdList.SelectedIndex >= 0)
            {
                _bidMgr.AssociatedAdID = _adList[this.AdList.SelectedIndex].ID;

                int aNum = _adList[this.AdList.SelectedIndex].ID;
                this.adIDLabel.Text = aNum.ToString ();

                this.SaveBid.Enabled = true;
            }
        }

        private void categories_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.categories.SelectedIndex >= 0)
            {
                if (this.categories.SelectedIndex == 0)
                    _bidMgr.ProductCategory = "";  
                else
                    _bidMgr.ProductCategory = this.categories.SelectedItem.ToString();

                this.subcategories.DataSource = _bidMgr.GetProductSubcategories();

                // We'll make the subcategory dropdown invisible if there are no 
                // items to select.
                if (this.subcategories.Items.Count == 0)
                {
                    this.subcategories.Visible = false;
                    this.subcatLabel.Visible = false;
                    this.products.Visible = false;
                    this.productLabel.Visible = false;

                    productControlVisibility(this.mainpanel, "", false);
                }
                else
                {
                    this.subcategories.Visible = true;
                    this.subcatLabel.Visible = true;
                }
            }
        }

        private void subcategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.subcategories.SelectedIndex >= 0)
            {
                if (this.subcategories.SelectedIndex == 0)
                    _bidMgr.ProductSubcategory = "";
                else
                    _bidMgr.ProductSubcategory = this.subcategories.SelectedItem.ToString ();

                this.products.DataSource = _bidMgr.GetProductClassifications ();

                // We'll make the products dropdown invisible if there are no 
                // items to select.
                if (this.products.Items.Count == 0)
                {
                    this.products.Visible = false;
                    this.productLabel.Visible = false;

                    productControlVisibility(this.mainpanel, "", false);
                }
                else
                {
                    this.products.Visible = true;
                    this.productLabel.Visible = true;
                }
            }
        }

        private void products_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.products.SelectedIndex >= 0)
            {
                string prodClass = "";

                // Assume that if selectedIndex == 0, that "Any" is the selection.
                if (this.products.SelectedIndex > 0)
                {
                    prodClass = this.products.SelectedItem.ToString();
                }                    
                
                _bidMgr.SetClass(prodClass);
                productControlVisibility(this.mainpanel, "", false);

                if (prodClass.Length > 0)
                {
                    initProductUIControls(_bidMgr.ProductObject, mainpanel, "", _bidMgr.ProductClass, true);
                }
            }
        }

        public void calcCPCButton_Click(object sender, EventArgs e)
        {
            double numScans;

            numScans = _bidMgr.ComputeScanHistory();

            this.cpcText.Text = String.Format("{0:######0}", numScans);
        }

        private void c1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateProductProperty(this.c1, this.l1, _bidMgr.ProductObject);
        }

        private void c2_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateProductProperty(this.c2, this.l2, _bidMgr.ProductObject);
        }

        private void c3_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateProductProperty(this.c3, this.l3, _bidMgr.ProductObject);
        }

        private void c4_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateProductProperty(this.c4, this.l4, _bidMgr.ProductObject);
        }

        private void c5_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateProductProperty(this.c5, this.l5, _bidMgr.ProductObject);
        }

        private void c6_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateProductProperty(this.c6, this.l6, _bidMgr.ProductObject);
        }

        private void SaveBid_Click(object sender, EventArgs e)
        {
            _bidMgr.BidTitle = this.BidTitle.Text.ToString();

            if (_bidMgr.Save() == true)
            {
                _bidMgr.initializeListView();

                if (this.bidGridView.Rows.Count > 0)
                {
                    int rowIdx = bidGridView.Rows.Count - 1;

                    bidGridView.Rows[rowIdx].Selected = true;
                    bidGridView.CurrentCell = bidGridView[0, rowIdx];
                    bidGridView.FirstDisplayedScrollingRowIndex = rowIdx;
                }
            }
        }

        private void DeleteBid_Click(object sender, EventArgs e)
        {
            if (_bidMgr != null)
            {
                try
                {
                    DataGridViewRow dgr = bidGridView.CurrentRow;

                    if (dgr != null)
                    {
                        int currRowNum = bidGridView.Rows.IndexOf(dgr);
                        var product = (CBid)dgr.DataBoundItem;

                        if (_bidMgr.Delete(product.ID) == true)
                        {
                            _bidMgr.initializeListView();

                            if (bidGridView.Rows.Count > 0)
                            {
                                currRowNum = (currRowNum < bidGridView.Rows.Count ?
                                        currRowNum : bidGridView.Rows.Count - 1);

                                bidGridView.Rows[currRowNum].Selected = true;
                                bidGridView.CurrentCell = bidGridView[0, currRowNum];
                                bidGridView.FirstDisplayedScrollingRowIndex = currRowNum;
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

        private void ModifyBid_Click(object sender, EventArgs e)
        {
            if (_bidMgr != null)
            {
                try
                {
                    DataGridViewRow dgr = this.bidGridView.CurrentRow;

                    if (dgr != null)
                    {
                        int currRowNum = bidGridView.Rows.IndexOf(dgr);
                        var product = (CBid)dgr.DataBoundItem;

                        if (product != null)
                        {    
                            _bidMgr.BidTitle = this.BidTitle.Text.ToString();

                            if (_bidMgr.Modify(product.ID)) {

                                _bidMgr.initializeListView();

                                if (bidGridView.Rows.Count > 0)
                                {
                                    currRowNum = (currRowNum < bidGridView.Rows.Count ?
                                            currRowNum : bidGridView.Rows.Count - 1);

                                    bidGridView.Rows[currRowNum].Selected = true;
                                    bidGridView.CurrentCell = bidGridView[0, currRowNum];
                                    bidGridView.FirstDisplayedScrollingRowIndex = currRowNum;
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

        // Account for the fact that an empty property value indicates
        // the default "-Any-" used in the UI.

        private string translateValue(string propertyValue)
        {
            string retVal = "-Any-";

            if (propertyValue.Length > 0)
            {
                retVal = propertyValue;
            }

            return (retVal);
        }

        // Given an Ad ID, find it in the list and select it.

        private void selectAdItem(int adID)
        {
            int rowCnt = 0;

            if (_adList != null && _adList.Count () > 0)
            {
                foreach (CAd anAd in _adList)
                {
                    if (anAd.ID == adID)
                    {
                        this.AdList.SelectedIndex = rowCnt;

                        break;
                    }

                    rowCnt++;
                }
            }
        }

        private void bidGridView_RowEnter (object sender, DataGridViewCellEventArgs e)
        {
             if (e.RowIndex >= 0)
            {
                DataGridViewRow dgr = bidGridView.Rows[e.RowIndex];

                if (dgr != null)
                {
                    var product = (CBid)dgr.DataBoundItem;

                    if (product != null)
                    {
                        selectAdItem(product.AssociatedAdID);

                        this.BidTitle.Text = product.Title.ToString();
                        this.categories.Text = translateValue(product.ProductCategory.ToString ());

                        // If there is a non-default value, go to the next level (subcategory).
                        if (product.ProductCategory.Length > 0)
                        {
                            this.subcategories.Text = translateValue(product.ProductSubcategory.ToString ());

                            // If there is a non-default value, go to the next level (class).
                            if (product.ProductSubcategory.Length > 0)
                            {
                                this.products.Text = translateValue(product.ProductClass.ToString());

                                // If there is a non-default value, go to the next level (object).
                                if (product.ProductClass.Length > 0)
                                {
                                    if (_bidMgr != null)
                                    {
                                        int ctrlNum = 1;
                                        ComboBox propControl;
                                        CProduct productObj = _bidMgr.SetProductObject(product.SerializedProduct);

                                        foreach (CProperty cprop in productObj.ProductProperties) {

                                            propControl = (ComboBox) this.mainpanel.Controls["c" + ctrlNum.ToString ()];

                                            if (propControl != null)
                                            {
                                                propControl.Text = translateValue(cprop.PropertyValue.ToString());
                                            }

                                            ctrlNum++;
                                        }
                                    }
                                }
                            }
                        } 
                    }
                }
            }
        }
    }
}
