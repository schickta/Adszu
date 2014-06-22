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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CIQAdServices" in code, svc and config file together.
    public class CIQAdServices : ICIQAdServices
    {
        public CAd CreateAdInstance()
        {
            CAd ret = null;

            try
            {
                ret = new CAd();

                ret.MarketerID = -1;
                ret.Title = "";
                ret.AdText = "";
                ret.ImageUrl = "";
                ret.BrandImageUrl = "";
                ret.DiscountAmount = 0.0F;
                ret.IsDiscountAPercentage = false;
                ret.DiscountCode = "";
            }

            catch (Exception) { }

            return (ret);
        }

        public List<CAd> GetAds(int MarketerID)
        {
            List<CAd> retVal = new List<CAd>();
            CAd anAd = null;
            try
            {
                CIQPlatformEntities ce = new CIQPlatformEntities();

                var qry = from DiscountAd da in ce.DiscountAds
                          where da.MarketerID == MarketerID
                          select da;

                if (qry.Count() > 0)
                {
                    foreach (DiscountAd da in qry)
                    {
                        anAd = new CAd();
                        anAd.ID = da.ID;
                        anAd.MarketerID = da.MarketerID;
                        anAd.AdText = da.AdText;
                        anAd.DiscountAmount = (float) da.DiscountAmount;
                        anAd.DiscountCode = da.DiscountCode;
                        anAd.BrandImageUrl = da.BrandImageUrl;
                        anAd.ImageUrl = da.ImageUrl;
                        anAd.IsDiscountAPercentage = da.DiscountIsPercentage;
                        anAd.Title = da.Title;

                        retVal.Add(anAd);
                    }
                }
            }
            catch (Exception) { }

            return (retVal);
        }

        public bool SaveAd(CAd anAd)
        {
            bool ret = false;
            int newID = 0;

            try
            {

                CIQPlatformEntities ce = new CIQPlatformEntities();

                newID = ce.DiscountAds.Max(a => a.ID) + 1;

                DiscountAd dAd = new DiscountAd();

                dAd.ID = newID;
                dAd.MarketerID = anAd.MarketerID;
                dAd.AdText = anAd.AdText;
                dAd.DiscountAmount = anAd.DiscountAmount;
                dAd.DiscountCode = anAd.DiscountCode;
                dAd.DiscountIsPercentage = anAd.IsDiscountAPercentage;
                dAd.ImageUrl = anAd.ImageUrl;
                dAd.BrandImageUrl = anAd.BrandImageUrl;
                dAd.Title = anAd.Title;

                ce.DiscountAds.AddObject(dAd);
                ce.SaveChanges();

                ret = true;
            }
            catch (Exception) { }

            return (ret);
        }

        public CAd GetAd(int ID)
        {
            CAd ad = null;
            try
            {
                CIQPlatformEntities ce = new CIQPlatformEntities();
                DiscountAd dAd = ce.DiscountAds.FirstOrDefault(d => d.ID == ID);

                ad = new CAd();
                ad.ID = dAd.ID;
                ad.MarketerID = dAd.MarketerID;
                ad.Title = dAd.Title;
                ad.AdText = dAd.AdText;
                ad.DiscountCode = dAd.DiscountCode;
                ad.DiscountAmount = (float)dAd.DiscountAmount;
                ad.IsDiscountAPercentage = dAd.DiscountIsPercentage;
                ad.ImageUrl = dAd.ImageUrl;
                ad.BrandImageUrl = dAd.BrandImageUrl;
            }
            catch (Exception)
            {

            }

            return ad;
        }

        public bool DeleteAd(int ID, bool deleteAssociatedBids)
        {
            bool retVal = false;

            try
            {
                CIQPlatformEntities ce = new CIQPlatformEntities();

                CIQBidServices bidServices = new CIQBidServices();
                List<CBid> associatedBids = bidServices.AssociatedToAd(ID); 

                if (associatedBids.Count () > 0 && deleteAssociatedBids == true) {

                    // Delete any Bids referencing the ad we actually want to delete.

                    foreach (CBid bd in associatedBids)
                    {
                        Bid aBid = ce.Bids.FirstOrDefault(b => b.ID == bd.ID);
                        ce.Bids.DeleteObject(aBid);
                    }

                    ce.SaveChanges(); //TAS - I'd rather not commit until the end...

                    // Make the call again, to make sure we now have no associated bids.
                    // before attempting to delete the ad.

                    associatedBids = bidServices.AssociatedToAd(ID); 
                }

                if (associatedBids.Count () == 0) {

                    DiscountAd dAd = ce.DiscountAds.FirstOrDefault(d => d.ID == ID);

                    ce.DiscountAds.DeleteObject(dAd);
                    ce.SaveChanges();

                    retVal = true;
                }    
            }
            catch (Exception) { }

            return (retVal);
        }

        public bool UpdateAd(CAd anAd)
        {
            bool retVal = false;

            try
            {
                CIQPlatformEntities ce = new CIQPlatformEntities();
                DiscountAd dAd = ce.DiscountAds.FirstOrDefault(d => d.ID == anAd.ID);

                if (dAd != null)
                {
                    dAd.AdText = anAd.AdText;

                    dAd.DiscountAmount = anAd.DiscountAmount;
                    dAd.DiscountCode = anAd.DiscountCode;
                    dAd.DiscountIsPercentage = anAd.IsDiscountAPercentage;
                    dAd.ImageUrl = anAd.ImageUrl;                    
                    dAd.BrandImageUrl = anAd.BrandImageUrl;
                    dAd.Title = anAd.Title;
                    dAd.AdText = anAd.AdText;
                   
                    ce.SaveChanges();
                    retVal = true;
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
