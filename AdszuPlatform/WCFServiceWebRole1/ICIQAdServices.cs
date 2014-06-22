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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICIQAdServices" in both code and config file together.
    [ServiceContract]
    public interface ICIQAdServices
    {
        [OperationContract]
        CAd CreateAdInstance();

        [OperationContract]
        List<CAd> GetAds (int MarketerID);

        [OperationContract]
        CAd GetAd(int ID);

        [OperationContract]
        bool SaveAd(CAd anAd);

        [OperationContract]
        bool DeleteAd (int ID, bool deleteAssociatedBids);

        [OperationContract]
        bool UpdateAd(CAd anAd);
    }

    [DataContract]
    public class CAd
    {
        private int _ID;
        private int _marketerID;
        private string _title;
        private string _adText;
        private string _imageUrl;
        private string _brandImageUrl;
        private float _discountAmount;
        private bool _isDiscountAPercentage;
        private string _discountCode;

        [DataMember]
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        [DataMember]
        public int MarketerID
        {
            get { return _marketerID; }
            set { _marketerID = value; }
        }

        [DataMember]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        [DataMember]
        public string AdText
        {
            get { return _adText; }
            set { _adText = value; }
        }

        [DataMember]
        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }

        [DataMember]
        public string BrandImageUrl
        {
            get { return _brandImageUrl; }
            set { _brandImageUrl = value; }
        }

        [DataMember]
        public float DiscountAmount
        {
            get { return _discountAmount; }
            set { _discountAmount = value; }
        }

        [DataMember]
        public bool IsDiscountAPercentage
        {
            get { return _isDiscountAPercentage; }
            set { _isDiscountAPercentage = value; }
        }

        [DataMember]
        public string DiscountCode
        {
            get { return _discountCode; }
            set { _discountCode = value; }
        }
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// The code in this file is confidential and  
// Copyright (C) 2011-2012 by Todd Schick 
/////////////////////////////////////////////////////////////////////////////////////////////////////////