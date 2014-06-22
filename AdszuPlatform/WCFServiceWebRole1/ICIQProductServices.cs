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
    public interface ICIQProductServices
    {
        [OperationContract]
        CProduct CreateProductInstance(string productClassification, bool isForBidUsage);

        [OperationContract]
        bool Serialize (CProduct aProduct, ref string outString);

        [OperationContract]
        CProduct Deserialize(string inString);

        [OperationContract]
        List<String> GetProductCategories(bool isForBidUsage);

        [OperationContract]
        List<String> GetProductSubcategories(string sCategory, bool isForBidUsage);

        [OperationContract]
        List<String> GetProductClassifications(string sCategory, string sSubcategory, bool isForBidUsage);
    }

    [DataContract]
    public class CProduct
    {
        int _versionID;
        string _productClassification;
        List<CProperty> _properties;

        public CProduct()
        {
            _properties = new List<CProperty>();
        }

        [DataMember]
        public int VersionID
        {
            get { return _versionID; }
            set { _versionID = value; }
        }

        [DataMember]
        public string ProductClassification
        {
            get { return _productClassification; }
            set { _productClassification = value; }
        }

        [DataMember]
        public List<CProperty> ProductProperties
        {
            get { return _properties; }
            set { _properties = value; }
        }
    }

    [DataContract]
    public class CProperty
    {
        string _propertyName;
        string _propertyValue;
        List<string> _validValues;

        [DataMember]
        public string PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

        [DataMember]
        public string PropertyValue
        {
            get { return _propertyValue; }
            set { _propertyValue = value; }
        }

        [DataMember]
        public List<string> ValidValues
        {
            get { return _validValues; }
            set { _validValues = value; }
        }
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// The code in this file is confidential and  
// Copyright (C) 2011-2012 by Todd Schick 
/////////////////////////////////////////////////////////////////////////////////////////////////////////