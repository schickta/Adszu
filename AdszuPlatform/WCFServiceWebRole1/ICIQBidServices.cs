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
     [ServiceContract]
    public interface ICIQBidServices
    {
         [OperationContract]
         CBid CreateBidInstance();

         [OperationContract]
         List<CBid> GetBids (int MarketerID);

         [OperationContract]
         bool SaveBid(CBid aBid, string flattenedProduct);

         [OperationContract]
         bool DeleteBid(int ID);

         [OperationContract]
         bool UpdateBid (CBid aBid, string flattenedProduct);

         [OperationContract]
         double ComputeScanHistory (CBid aBid, string bidProd, double howManyDaysBack);

         [OperationContract]
         List<CBid> AssociatedToAd (int adID);
    }
   
    [DataContract]
    public class CBid
    {
        private int _ID;
        private int _marketerID;        
        private string _title;
        private int _assocAdID;
        private string _region;
        private string _timeofday;
        private string _dayofweek;
        private string _productCat;
        private string _productSubcat;
        private string _productClass;
        private string _serializedProduct;

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
        public int AssociatedAdID
        {
            get { return _assocAdID; }
            set { _assocAdID = value; }
        }

        [DataMember]
        public string Region
        {
            get { return _region; }
            set { _region = value; }
        }

        [DataMember]
        public string TimeOfDay
        {
            get { return _timeofday; }
            set { _timeofday = value; }
        }

        [DataMember]
        public string DayOfWeek
        {
            get { return _dayofweek; }
            set { _dayofweek = value; }
        }

        [DataMember]
        public string ProductCategory
        {
            get { return _productCat; }
            set { _productCat = value; }
        }

        [DataMember]
        public string ProductSubcategory
        {
            get { return _productSubcat; }
            set { _productSubcat = value; }
        }

        [DataMember]
        public string ProductClass
        {
            get { return _productClass; }
            set { _productClass = value; }
        }

        [DataMember]
        public string SerializedProduct
        {
            get { return _serializedProduct; }
            set { _serializedProduct = value; }
        }
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// The code in this file is confidential and  
// Copyright (C) 2011-2012 by Todd Schick 
/////////////////////////////////////////////////////////////////////////////////////////////////////////