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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICIQProductIdentityServices" in both code and config file together.
    [ServiceContract]
    public interface ICIQProductIdentityServices
    {
        [OperationContract]
        CProductIdentity CreateProductIdentityInstance ();

        [OperationContract]
        bool SaveProductIdentity(CProductIdentity cPI, string flattenedProduct);

        [OperationContract]
        List<CProductIdentity> QueryKind (string flattenedProduct);

        [OperationContract]
        bool MapIdentity (string mapping, string productIdentity);

        [OperationContract]
        CProductIdentity QueryIdentity (string productCode);

        [OperationContract]
        string QueryProduct (string productCode);
    }

    [DataContract]
    public class CProductIdentity
    {
        private string _productIdentity;
        private string _description;
        private string _productCat;
        private string _productSubcat;
        private string _productClass;

        [DataMember]
        public string ProductIdentity
        {
            get { return _productIdentity; }
            set { _productIdentity = value; }
        }

        [DataMember]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
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
    }

}

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// The code in this file is confidential and  
// Copyright (C) 2011-2012 by Todd Schick 
/////////////////////////////////////////////////////////////////////////////////////////////////////////

