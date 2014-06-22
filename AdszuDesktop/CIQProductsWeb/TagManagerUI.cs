using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CIQDesktop
{
    public partial class MainFrm : Form
    {
        private class TagManager
        {
            private CIQProductIdentityServices.CIQProductIdentityServicesClient _pIdentityClient;
            private CIQProductServices.CIQProductServicesClient _tmProductClient;

            private CIQProductIdentityServices.CProductIdentity _tmProdIdentity;
            private CIQProductServices.CProduct _tmProduct;

            private Panel _parent;

            public TagManager(Panel parentPanel)
            {
                _parent = parentPanel;

                _tmProductClient = new CIQProductServices.CIQProductServicesClient();
                _pIdentityClient = new CIQProductIdentityServices.CIQProductIdentityServicesClient();

                _tmProdIdentity = _pIdentityClient.CreateProductIdentityInstance();
                _tmProduct = null;
            }

            ~TagManager()
            {
                _tmProduct = null;
                _tmProdIdentity = null;

                _tmProductClient.Close();
                _pIdentityClient.Close();
            }

            public void Initialize()
            {
                if (_pIdentityClient != null)
                {
                    _tmProdIdentity = _pIdentityClient.CreateProductIdentityInstance();
                }
            }

            public void Clear()
            {
                _tmProduct = null;
                _tmProdIdentity = null;
            }

            public string[] GetProductCategories()
            {
                return (_tmProductClient.GetProductCategories(false));
            }

            public string[] GetProductSubcategories()
            {
                return (_tmProductClient.GetProductSubcategories(_tmProdIdentity.ProductCategory, false));
            }

            public string[] GetProductClassifications()
            {
                return (_tmProductClient.GetProductClassifications(_tmProdIdentity.ProductCategory, _tmProdIdentity.ProductSubcategory, false));
            }

            public void SetClass(string productClass)
            {
                this.ProductClass = productClass;
                _tmProduct = _tmProductClient.CreateProductInstance(ProductClass, false);
            }

            public void Save(string productCode, string description)
            {
                _tmProdIdentity.ProductIdentity = productCode;
                _tmProdIdentity.Description = description;

                String flatObj = "";

                if (_tmProductClient.Serialize(_tmProduct, ref flatObj))
                {
                    if (_pIdentityClient.SaveProductIdentity(_tmProdIdentity, flatObj))
                    {
                        MessageBox.Show("Saved");
                    }
                }
            }

            public string ProductCategory
            {
                get { return (_tmProdIdentity.ProductCategory); }

                set
                {
                    if (_tmProdIdentity != null)
                    {
                        _tmProdIdentity.ProductCategory = value;
                    }
                }
            }

            public string ProductSubcategory
            {
                get { return (_tmProdIdentity.ProductSubcategory); }

                set
                {
                    if (_tmProdIdentity != null)
                    {
                        _tmProdIdentity.ProductSubcategory = value;
                    }
                }
            }

            public string ProductClass
            {
                get { return (_tmProdIdentity.ProductClass); }

                set
                {
                    if (_tmProdIdentity != null)
                    {
                        _tmProdIdentity.ProductClass = value;
                    }
                }
            }

            public string ProductIdentity
            {
                get { return (_tmProdIdentity.ProductIdentity); }

                set
                {
                    if (_tmProdIdentity != null)
                    {
                        _tmProdIdentity.ProductIdentity = value;
                    }
                }
            }

            public string Description
            {
                get { return (_tmProdIdentity.Description); }

                set
                {
                    if (_tmProdIdentity != null)
                    {
                        _tmProdIdentity.Description = value;
                    }
                }
            }

            public CIQProductServices.CProduct ProductObject
            {
                get { return (_tmProduct); }
            }
        }

        void tagpanel_Leave(object sender, EventArgs e)
        {
            _tagMgr.Clear();
        }

        void tagpanel_Enter(object sender, EventArgs e)
        {
            _tagMgr.Initialize();

            this.tmCategories.DataSource = _tagMgr.GetProductCategories ();
        }

        private void mtCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tmCategories.SelectedIndex >= 0)
            {
                _tagMgr.ProductCategory = this.tmCategories.SelectedItem.ToString();

                this.tmSubcategories.DataSource = _tagMgr.GetProductSubcategories();
            }
        }

        private void mtSubcategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tmSubcategories.SelectedIndex >= 0)
            {
                _tagMgr.ProductSubcategory = this.tmSubcategories.SelectedItem.ToString();

                this.tmProducts.DataSource = _tagMgr.GetProductClassifications();
            }
        }

        private void mtProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tmProducts.SelectedIndex >= 0)
            {
                _tagMgr.SetClass(this.tmProducts.SelectedItem.ToString());

                productControlVisibility(this.tagpanel, "t", false);
                initProductUIControls(_tagMgr.ProductObject, tagpanel, "t", _tagMgr.ProductClass, false);
            }
        }

        private void saveIdentity_Click(object sender, EventArgs e)
        {
            _tagMgr.Save(prodCode.Text.ToString(), prodDescription.Text.ToString());
        }

        private void tc1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateProductProperty(this.tc1, this.tl1, _tagMgr.ProductObject);
        }

        private void tc2_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateProductProperty(this.tc2, this.tl2, _tagMgr.ProductObject);
        }

        private void tc3_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateProductProperty(this.tc3, this.tl3, _tagMgr.ProductObject);
        }

        private void tc4_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateProductProperty(this.tc4, this.tl4, _tagMgr.ProductObject);
        }

        private void tc5_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateProductProperty(this.tc5, this.tl5, _tagMgr.ProductObject);
        }

        private void tc6_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateProductProperty(this.tc6, this.tl6, _tagMgr.ProductObject);
        }
    }
}
