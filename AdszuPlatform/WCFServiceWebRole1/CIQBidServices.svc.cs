using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// The code in this file is confidential and  
// Copyright (C) 2011-2012 by Todd Schick 
/////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace WCFServiceWebRole
{
    public class CIQBidServices : ICIQBidServices
    {
        public CBid CreateBidInstance()
        {
            CBid ret = null;

            try
            {
                ret = new CBid();
            }
            catch (Exception) { }

            return (ret);
        }

        public List<CBid> GetBids(int MarketerID)
        {
            List<CBid> retVal = new List<CBid>();
            CBid aBid = null;

            try
            {
                CIQPlatformEntities ce = new CIQPlatformEntities();

                var qry = from Bid bd in ce.Bids
                          where bd.MarketerID == MarketerID
                          select bd;

                if (qry.Count() > 0)
                {
                    foreach (Bid bd in qry)
                    {
                        aBid = new CBid();
                        aBid.ID = bd.ID;
                        aBid.MarketerID = (int)bd.MarketerID;
                        aBid.AssociatedAdID = (int)bd.AssociatedAd;
                        aBid.Title = bd.Title;
                        aBid.DayOfWeek = bd.DayOfWeek;
                        aBid.TimeOfDay = bd.TimeOfDay;
                        aBid.Region = bd.Region;
                        aBid.ProductCategory = bd.ProdCategory;
                        aBid.ProductSubcategory = bd.ProdSubcategory;
                        aBid.ProductClass = bd.ProdClass;
                        aBid.SerializedProduct = bd.ProdBlob;

                        retVal.Add(aBid);
                    }
                }
            }
            catch (Exception) { }

            return (retVal);
        }

        public bool SaveBid(CBid aBid, string flattenedProduct)
        {
            bool ret = false;

            try
            {
                CIQPlatformEntities ce = new CIQPlatformEntities();
                Bid bd = new Bid();

                // Determine the ID of the new record. If there are currently no records,
                // catch the exception and initialize.

                int newID;

                try
                {
                    newID = ce.Bids.Max(a => a.ID) + 1;
                }
                catch (Exception)
                {
                    newID = 1;
                }

                bd.ID = newID;
                bd.MarketerID = aBid.MarketerID;
                bd.Title = aBid.Title;
                bd.AssociatedAd = aBid.AssociatedAdID;
                bd.Region = aBid.Region;
                bd.TimeOfDay = aBid.TimeOfDay;
                bd.DayOfWeek = aBid.DayOfWeek;
                bd.ProdCategory = aBid.ProductCategory;
                bd.ProdSubcategory = aBid.ProductSubcategory;
                bd.ProdClass = aBid.ProductClass;
                bd.ProdBlob = flattenedProduct;

                ce.Bids.AddObject(bd);
                ce.SaveChanges();

                ret = true;
            }
            catch (Exception) { }

            return (ret);
        }

        public bool DeleteBid(int ID)
        {
            bool retVal = false;

            try
            {
                CIQPlatformEntities ce = new CIQPlatformEntities();
                Bid aBid = ce.Bids.FirstOrDefault(d => d.ID == ID);

                ce.Bids.DeleteObject(aBid);
                ce.SaveChanges();

                retVal = true;
            }
            catch (Exception) { }
            return (retVal);
        }

        public bool UpdateBid(CBid aBid, string flattenedProduct)
        {
            bool retVal = false;

            try
            {
                CIQPlatformEntities ce = new CIQPlatformEntities();
                Bid bd = ce.Bids.FirstOrDefault(d => d.ID == aBid.ID);

                if (bd != null)
                {
                    bd.Title = aBid.Title;
                    bd.AssociatedAd = aBid.AssociatedAdID;
                    bd.Region = aBid.Region;
                    bd.TimeOfDay = aBid.TimeOfDay;
                    bd.DayOfWeek = aBid.DayOfWeek;
                    bd.ProdBlob = flattenedProduct;
                    bd.ProdCategory = aBid.ProductCategory;
                    bd.ProdSubcategory = aBid.ProductSubcategory;
                    bd.ProdClass = aBid.ProductClass;

                    ce.SaveChanges();
                    retVal = true;
                }
            }
            catch (Exception) { }

            return (retVal);
        }

        public double ComputeScanHistory(CBid aBid, string bidProd, double howManyDaysBack)
        {
            CIQPlatformEntities ce = new CIQPlatformEntities();
            long retVal = 0;

            DateTime dt = DateTime.Now;
            TimeSpan daysBack = TimeSpan.FromDays(howManyDaysBack);
            DateTime earliestDate = dt.Subtract(daysBack);

            // Get the selected set of scans to work with dated from today, back to the
            // number of days specified in the howManyDaysBack parameter.

            var dateQry = from ScanActivity sa in ce.ScanActivities
                          where sa.DateTime >= earliestDate.Date
                          select sa;

            retVal = dateQry.Count();

            // Trim down the result set with bid matches of category, subcategory and class.

            if (aBid.ProductCategory != null && aBid.ProductCategory.Length > 0)
            {
                var categoryQry = from ScanActivity sa in dateQry
                                  where sa.Category == aBid.ProductCategory
                                  select sa;

                retVal = categoryQry.Count();

                if (aBid.ProductSubcategory != null && aBid.ProductSubcategory.Length > 0)
                {
                    var subcategoryQry = from ScanActivity sa in categoryQry
                                         where sa.Subcategory == aBid.ProductSubcategory
                                         select sa;

                    retVal = subcategoryQry.Count();

                    if (aBid.ProductClass != null && aBid.ProductClass.Length > 0)
                    {
                        var classQry = from ScanActivity sa in subcategoryQry
                                       where sa.Classification == aBid.ProductClass
                                       select sa;

                        retVal = classQry.Count();

                        // Now we'll do a product match if a bidProduct has been passed in.

                        if (bidProd != null && bidProd.Length > 0)
                        {
                            retVal = 0;

                            // Blow out the productBlob that was input.

                            CIQProductServices psv = new CIQProductServices();
                            CProduct bidCProd = psv.Deserialize(bidProd);

                            var productQry = from ScanActivity sa in classQry
                                             where sa.ProductBlob != null && sa.ProductBlob.Length > 0
                                             select sa;

                            // actually, this count should equal classQry count. Also, check that
                            // we got a valid CProduct returned from the Deserialize.

                            if (bidCProd != null && productQry.Count() > 0)
                            {
                                foreach (ScanActivity sa in productQry)
                                {
                                    CProduct scanProd = psv.Deserialize(sa.ProductBlob);
                                    CProperty foundProp;

                                    // Roll through each non-blank property in the BID object
                                    // and compare that to each scanned product property.

                                    foreach (CProperty bidCProp in bidCProd.ProductProperties)
                                    {
                                        if (bidCProp.PropertyValue != null &&
                                            bidCProp.PropertyValue.Length > 0)
                                        {
                                            foundProp = scanProd.ProductProperties.Find
                                                (scanProp => scanProp.PropertyName == bidCProp.PropertyName);

                                            if (foundProp == null ||
                                                foundProp.PropertyValue != bidCProp.PropertyValue)
                                            {
                                                retVal--;
                                                break;
                                            }
                                        }
                                    }

                                    retVal++;
                                }
                            }
                        }
                    }
                }
            }

            return (retVal);
        }

        public List<CBid> AssociatedToAd (int adID)
        {
            List<CBid> retVal = new List<CBid>();

            try
            {
                CIQPlatformEntities ce = new CIQPlatformEntities();

                var qry = from Bid bd in ce.Bids
                          where bd.AssociatedAd == adID
                          select bd;

                foreach (Bid bd in qry)
                {
                    CBid aBid = new CBid();
                    aBid.AssociatedAdID = (int)bd.AssociatedAd;
                    aBid.DayOfWeek = bd.DayOfWeek;
                    aBid.ID = bd.ID;
                    aBid.MarketerID = (int)bd.MarketerID;
                    aBid.ProductCategory = bd.ProdCategory;
                    aBid.ProductClass = bd.ProdClass;
                    aBid.ProductSubcategory = bd.ProdSubcategory;
                    aBid.Region = bd.Region;
                    aBid.SerializedProduct = bd.ProdBlob;
                    aBid.TimeOfDay = bd.TimeOfDay;
                    aBid.Title = bd.Title;

                    retVal.Add(aBid);
                }
            }
            catch (Exception) { }

            return (retVal);
        }
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// The code in this file is confidential and  
// Copyright (C) 2011-2012 by Todd Schick 
/////////////////////////////////////////////////////////////////////////////////////////////////////////