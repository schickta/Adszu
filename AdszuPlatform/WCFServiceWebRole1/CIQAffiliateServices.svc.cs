using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// The code in this file is confidential and  
// Copyright (C) 2011-2012 by Todd Schick 
/////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace WCFServiceWebRole
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class CIQAffiliateServices : ICIQPlatform
    {
        public CProductAds GetAdList(Guid affiliateID, string scannedCode, double lon, double lat, int adsToReturn)
        {
            CProductAds retCollection = new CProductAds();
            CProductAd prodAd = new CProductAd();

            try
            {
                // Add Scan Entry to our DB
                addScanEntry(affiliateID, scannedCode, lon, lat);

                // search the dummy database for Ads based on the scan code.
                retCollection = findProductAds(scannedCode);
            }

            catch (Exception) {}

            return (retCollection);
        }

        private CProductAds findProductAds(string scanCode)
        {
            CProductAds retVal = null;
            CIQProductIdentityServices pidSvc = new CIQProductIdentityServices ();
            CIQProductServices prodSvc = new CIQProductServices();

            try
            {
                // This function gathers up the information to pass to matchAdsToScan which
                // does the matching against bids to pick the most pertinent.

                string prodBlob = pidSvc.QueryProduct (scanCode);

                if (prodBlob != null && prodBlob.Length > 0)
                {
                    CProduct scannedProd = prodSvc.Deserialize(prodBlob);

                    if (scannedProd != null)
                    {
                        CProductIdentity cID = pidSvc.QueryIdentity(scanCode);

                        if (cID != null)
                        {
                            retVal = matchAdsToScan(cID.ProductCategory, 
                                cID.ProductSubcategory, 
                                cID.ProductClass, 
                                scannedProd);
                        }
                    }
                }
            }
            catch (Exception) {}

            return (retVal);
        }

        private CProductAds matchAdsToScan(string category, string subcategory,
                                            string classification, CProduct scannedProd)
        {
            CProductAds retVal = null;
            CIQPlatformEntities ce = new CIQPlatformEntities();
            List<Bid> bidList = new List<Bid>();

            // First step, match the cat/subcat/class against bids.

            var categoryMatch = from Bid aBid in ce.Bids
                                where aBid.ProdCategory == null || aBid.ProdCategory == "" || aBid.ProdCategory == category
                                select aBid;

            var subcatMatch = from Bid aBid in categoryMatch
                              where aBid.ProdSubcategory == null || aBid.ProdSubcategory == "" || aBid.ProdSubcategory == subcategory
                              select aBid;

            var classMatch = from Bid aBid in subcatMatch
                             where aBid.ProdClass == null || aBid.ProdClass == "" || aBid.ProdClass == classification
                             select aBid;

            // Now for each bid that contains a product classification (and therefore an 
            // associated product blob) we need to compare the product specs to make sure there
            // is a match. If there's not a match, don't put the bid into the bidList. The output
            // of this loop will be a matching list of Bids.

            if (classMatch.Count() > 0 && scannedProd != null)
            {
                CIQProductServices psvc = new CIQProductServices ();
                CProduct bidProduct = null;

                foreach (Bid aBid in classMatch)
                {
                    bidList.Add(aBid);

                    if (aBid.ProdClass != null && aBid.ProdClass.Length > 0 && 
                        aBid.ProdBlob != null && aBid.ProdBlob.Length > 0)
                    {
                        bidProduct = psvc.Deserialize(aBid.ProdBlob);

                        // If there is any property that doesn't match, we will ultimately
                        // remove the bid from the classMatch list.

                        if (compareProducts (bidProduct, scannedProd) == false) 
                        {
                            bidList.Remove(aBid);
                        }
                    }
                }
            }

            // Now for all of the selected Bids, we'll put their associated Ads into a collection
            // and return it.

            if (bidList.Count() > 0)
            {
                retVal = new CProductAds();
                CProductAd anAd;

                foreach (Bid aBid in bidList)
                {
                    var qry = from DiscountAd da in ce.DiscountAds
                              where da.ID == aBid.AssociatedAd
                              select da;

                    if (qry.Count() > 0)
                    {
                        DiscountAd da = (DiscountAd)qry.First();

                        if (da != null)
                        {
                            anAd = new CProductAd();
                            anAd.AdGuid = ToGuid((int)da.ID);
                            anAd.AdTitle = da.Title;
                            anAd.AdText = da.AdText;
                            anAd.ImageUrl = da.ImageUrl;
                            anAd.BrandImageUrl = da.BrandImageUrl;
                            anAd.DiscountAmount = (double)da.DiscountAmount;
                            retVal.Add(anAd);
                        }
                    }
                }
            }

            return (retVal);
        }

        // Compare the product associated with the bid and the scanned product. Bid products
        // are allowed to have null valuesd, meaning that any scanned value will apply. So we don't
        // care about them. We care when a bid property value DOES have a value, at which point we compare
        // it to the scanned product property value. 

        // Would be nice if the ProductProperties list were instead a dictionary, so we can do a quick
        // lookup.

        private bool compareProducts (CProduct bidProduct, CProduct scannedProd)
        {

            if (bidProduct == null ||
                bidProduct.ProductClassification != scannedProd.ProductClassification)
            {
                return (false);
            }

            foreach (CProperty bidProp in bidProduct.ProductProperties)
            {
                if (bidProp.PropertyValue.Length > 0)
                {
                    foreach (CProperty scanProp in scannedProd.ProductProperties)
                    {
                        if (scanProp.PropertyName == bidProp.PropertyName)
                        {
                            if (scanProp.PropertyValue != bidProp.PropertyValue)
                            {
                                return (false);
                            }
                            break;
                        }
                    }
                }
            }

            return (true);
        }

        public CProductAd GetAdDetails(Guid affiliateID, string scannedCode, double lon, double lat, Guid adGuid)
        {
            CProductAd prodAd = null;

            try
            {
                // Add Scan Entry to our DB
                addScanEntry(affiliateID, scannedCode, lon, lat);

                // Get ad details of the ad associated with adGuid.
                prodAd = findAdDetail(adGuid);
            }
            catch (Exception) {}

            return (prodAd);
        }

        private CProductAd findAdDetail (Guid adGuid)
        {
            CProductAd anAd = null;
            CIQPlatformEntities ce = new CIQPlatformEntities();

            try
            {
                int anInt = ToInt(adGuid);

                var qry = from DiscountAd da in ce.DiscountAds
                          where da.ID == anInt
                          select da;

                if (qry.Count() > 0)
                {
                    anAd = new CProductAd();
                    DiscountAd da = (DiscountAd)qry.First();

                    anAd.AdGuid = ToGuid((int)da.ID);
                    anAd.AdTitle = da.Title;
                    anAd.DiscountAmount = (double)da.DiscountAmount;
                    anAd.IsPercentage = da.DiscountIsPercentage;
                    anAd.AdText = da.AdText;
                    anAd.BrandImageUrl = da.BrandImageUrl;
                    anAd.ImageUrl = da.ImageUrl;
                    anAd.ManufacturerCode = da.DiscountCode;
                    anAd.MarketerID = da.MarketerID;
                }
            }
            catch (Exception) { }

            return (anAd);
        }

        public CProductAd GetAdCoupon(Guid affiliateID, string scannedCode, double lon, double lat, Guid redeemGuid)
        {
            CProductAd prodAd = null;

            try
            {
                // Add Scan Entry to our DB
                addScanEntry(affiliateID, scannedCode, lon, lat);

                // return the actual coupon (with manufacturer code).
                prodAd = findCoupon(redeemGuid);
            }
            catch (Exception) {}

            return (prodAd);
        }

        public CProductProperties GetProductInformation(Guid affiliateID, string scannedCode, double lon, double lat)
        {
            CProductProperties retCollection = new CProductProperties();

            try
            {
                // Add Scan Entry to our DB
                addScanEntry(affiliateID, scannedCode, lon, lat);

                findProductInfo(scannedCode, retCollection);
            }
            catch (Exception) { }

            return (retCollection);
        }

        private CProductAd findCoupon (Guid redeemGuid)
        {
            CProductAd anAd = null;
            CIQPlatformEntities ce = new CIQPlatformEntities();

            try
            {
                var qry = from DiscountAd da in ce.DiscountAds
                          where da.ID == ToInt(redeemGuid)
                          select da;

                if (qry.Count() > 0)
                {
                    anAd = new CProductAd();
                    DiscountAd da = qry.ElementAt(0);

                    anAd.AdGuid = ToGuid((int)da.ID);
                    anAd.AdTitle = da.Title;
                    anAd.DiscountAmount = (double)da.DiscountAmount;
                    anAd.IsPercentage = da.DiscountIsPercentage;
                    anAd.AdText = da.AdText;
                    anAd.BrandImageUrl = da.BrandImageUrl;
                    anAd.ImageUrl = da.ImageUrl;
                    anAd.ManufacturerCode = da.DiscountCode;
                    anAd.MarketerID = da.MarketerID;
                }
            }
            catch (Exception) { }

            return (anAd);
        }

        private void addScanEntry(Guid affiliateID, string scannedCode, double lon, double lat)
        {
            try
            {
                CIQProductIdentityServices pisvc = new CIQProductIdentityServices();
                CIQPlatformEntities ce = new CIQPlatformEntities();
                ScanActivity sa = new ScanActivity();

                string category = "";
                string subcategory = "";
                string classification = "";
                string productid = scannedCode;
                string prodBlob = "";

                // Get the product identity and product blob of the scan code. 
                // If the product identity isn't found, it was decided to save that
                // which was scanned anyway as a way to determine missing product types.

                CProductIdentity cpi = pisvc.QueryIdentity (scannedCode);

                if (cpi != null)
                {
                    productid = cpi.ProductIdentity;
                    category = cpi.ProductCategory;
                    subcategory = cpi.ProductSubcategory;
                    classification = cpi.ProductClass;
                   
                    prodBlob = pisvc.QueryProduct(productid);
                }

                // NOTE: I do not like the below code for calculating ID AT ALL! In the Entity Framework 
                // model, I set "StoreGeneratedPattern" property of ID to "Identity", which should cause
                // the field to be auto-calculated. But it's not working.

                sa.ID = (from o in ce.ScanActivities select o).Count();
                sa.DateTime = DateTime.Now;
                sa.ScanData = productid;
                sa.Category = category;
                sa.Subcategory = subcategory;
                sa.Classification = classification;
                sa.ProductBlob = prodBlob;
                sa.Longitude = lon;
                sa.Latitude = lat;

                ce.ScanActivities.AddObject(sa);
                ce.SaveChanges();
            }
            catch (Exception) { }
        }

        public bool findProductInfo(string scannedCode, CProductProperties prodProps)
        {
            bool retVal = false;

            try
            {
                // First we're looking for the record in the ProductCodes table that matched the 
                // input scannedCode.
                CIQPlatformEntities ce = new CIQPlatformEntities();
                
                var qry = from ProductCode pc in ce.ProductCodes
                          where pc.ProductCode1 == scannedCode
                          select pc;

                if (qry.Count() > 0)
                {
                    ProductCode foundPC = qry.First();

                    if (foundPC != null)
                    {
                        // from the found record, we're going to deserialize the blob field
                        // to a CProduct which has all of the property information.

                        CIQProductServices prodSvc = new CIQProductServices();
                        CProduct aProd = prodSvc.Deserialize(foundPC.ProdBlob);

                        if (aProd != null)
                        {
                            // Add meta data to the dictionary we're going to return.
                            prodProps.Add("Category", foundPC.ProdCategory);
                            prodProps.Add("Subcategory", foundPC.ProdSubcategory);                           
                            prodProps.Add("Class", aProd.ProductClassification);
                            prodProps.Add("Description", foundPC.Description);

                            // Lastly add the properties to the return dictionary
                            foreach (CProperty cprop in aProd.ProductProperties)
                            {
                                prodProps.Add(cprop.PropertyName, cprop.PropertyValue);
                            }

                            retVal = true;
                        }
                    }
                }
            }
            catch (Exception) { }

            return (retVal);
        }

        // Helper to convert an int to guid.
        public Guid ToGuid(int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        public static int ToInt(Guid value)
        {
            byte[] b = value.ToByteArray();
            int bint = BitConverter.ToInt32(b, 0);
            return bint;
        }
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// The code in this file is confidential and  
// Copyright (C) 2011-2012 by Todd Schick 
/////////////////////////////////////////////////////////////////////////////////////////////////////////