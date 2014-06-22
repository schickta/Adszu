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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ICIQPlatform
    {
        [OperationContract]
        CProductAds GetAdList(Guid affiliateID, string scannedCode, double lon, double lat, int adsToReturn);

        [OperationContract]
        CProductAd GetAdDetails(Guid affiliateID, string scannedCode, double lon, double lat, Guid adGuid);

        [OperationContract]
        CProductAd GetAdCoupon (Guid affiliateID, string scannedCode, double lon, double lat, Guid redeemGuid);

        [OperationContract]
        CProductProperties GetProductInformation(Guid affiliateID, string scannedCode, double lon, double lat);
    }

    [CollectionDataContract
    (Name = "ProductProperties",
    ItemName = "entry",
    KeyName = "property",
    ValueName = "value")]
    public class CProductProperties : Dictionary<string, string> { }

    [CollectionDataContract
    (Name = "ProductAds")]
    public class CProductAds : List<CProductAd> { }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CProductAd
    {
        // Included in GetAdList
        string adTitle = "";
        string adText = "";
        double discountAmount = 0.0;
        bool isPercentage = false;
        Guid adGuid;

        // Included in GetAdDetails
        string imageUrl = "";
        string brandUrl = "";

        // Included in GetAdCoupon
        string manufacturerCode = "";
        int _marketerID = 0;

        [DataMember]
        public string AdTitle
        {
            get { return adTitle; }
            set { adTitle = value; }
        }

        [DataMember]
        public string AdText
        {
            get { return adText; }
            set { adText = value; }
        }

        [DataMember]
        public double DiscountAmount
        {
            get { return discountAmount; }
            set { discountAmount = value; }
        }

        [DataMember]
        public bool IsPercentage
        {
            get { return isPercentage; }
            set { isPercentage = value; }
        }    
        
        [DataMember]
        public Guid AdGuid
        {
            get { return adGuid; }
            set { adGuid = value; }
        }

        [DataMember]
        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }

        [DataMember]
        public string BrandImageUrl
        {
            get { return brandUrl; }
            set { brandUrl = value; }
        }

        [DataMember]
        public string ManufacturerCode
        {
            get { return manufacturerCode; }
            set { manufacturerCode = value; }
        }

        [DataMember]
        public int MarketerID
        {
            get { return _marketerID; }
            set { _marketerID = value; }
        }
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// The code in this file is confidential and  
// Copyright (C) 2011-2012 by Todd Schick 
/////////////////////////////////////////////////////////////////////////////////////////////////////////