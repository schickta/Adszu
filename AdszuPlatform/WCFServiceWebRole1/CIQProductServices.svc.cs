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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CIQProductServices" in code, svc and config file together.
    public class CIQProductServices : ICIQProductServices
    {
        public bool Serialize(CProduct aProduct, ref string outString)
        {
            bool ret = false;
            outString = "";

            try
            {
                if (aProduct != null)
                {
                    outString += aProduct.VersionID.ToString() + ";";
                    outString += aProduct.ProductClassification + ";";
                    outString += aProduct.ProductProperties.Count.ToString() + ";";

                    foreach (CProperty cprop in aProduct.ProductProperties)
                    {
                        outString += cprop.PropertyName + ";";
                        outString += cprop.PropertyValue + ";";
                    }

                    ret = true;
                }
            }
            catch (Exception) { }

            return (ret);
        }

        public CProduct Deserialize(string inString)
        {
            CProduct retVal = null;
            string[] parsedStrings;
            char[] delims = new char[1]; delims[0] = ';';

            if (inString != null && inString.Length > 0) {

                retVal = new CProduct();

                // take our blob and parse it into a series of strings (string array).
                parsedStrings = inString.Split (delims);
                
                // The first two elements are the blob version and product classification.
                retVal.VersionID = Convert.ToInt32 (parsedStrings[0]);
                retVal.ProductClassification = parsedStrings[1];
               
                // The next element is the number of properties for the product.
                int numProps = Convert.ToInt32 (parsedStrings[2]);

                // Loop through each name/value pair and create new CProperty objects.

                for (int i = 3; i < 3 + numProps*2; i+=2) {
                    CProperty cprop = new CProperty ();

                    cprop.PropertyName = parsedStrings[i];
                    cprop.PropertyValue = parsedStrings[i+1];
                    retVal.ProductProperties.Add(cprop);
                }
            }

            return (retVal);
        }

        private bool getProductSuperclasses(string productClassification, ref string subcat, ref string cat)
        {
            bool retVal = false;
            CIQPlatformEntities ce = new CIQPlatformEntities();

            try
            {
                var qry = from ProductClass pc in ce.ProductClasses
                          where pc.ProductClass1 == productClassification
                          select pc;

                if (qry.Count() > 0)
                {
                    ProductClass pc = (ProductClass)qry.First();

                    if (pc != null)
                    {
                        subcat = pc.Subcategory;
                        cat = pc.Category;

                        retVal = true;
                    }
                }
            }
            catch (Exception) { }

            return (retVal);
        }

        private List<string> getProductProperties (string productClassification, string subcategory, string category)
        {
            List<string> ret = new List<string>();

            try
            {            
                CIQPlatformEntities ce = new CIQPlatformEntities();

                var qry = from ProductTaxonomy pt in ce.ProductTaxonomies
                          where pt.ProductClass == "ROOTCLASS" ||
                                pt.ProductClass == category ||
                                pt.ProductClass == subcategory ||
                                pt.ProductClass == productClassification
                          select pt.Property;

                ret = qry.ToList();
            }
            catch (Exception) { }

            return (ret);
        }

        public CProperty createPropertyObject(string productClassification, string property, bool isForBidUsage)
        { 
            CProperty ret = new CProperty ();
            ret.PropertyName = property;
            ret.PropertyValue = "";

            try
            {
                CIQPlatformEntities ce = new CIQPlatformEntities();

                var qry = from ProductTaxonomy pt in ce.ProductTaxonomies
                          where pt.ProductClass == productClassification &&
                                pt.Property == property
                          select pt.ValidValues;

                if (qry.Count() > 0)
                {
                    string validValues = qry.First();

                    ret.ValidValues = validValues.Split(';').Select(i => i.ToString()).ToList();

                    if (isForBidUsage == true)
                    {
                        ret.ValidValues.Insert(0, "-Any-");
                    }
                }
                else
                {
                    if (isForBidUsage == true)
                    {
                        ret.ValidValues = new List<string>();
                        ret.ValidValues.Add("-Any-");
                    }
                }
            }
            catch (Exception) { }

            return (ret);
        }

        public CProduct CreateProductInstance(string productClassification, bool isForBidUsage)
        {
            CProduct ret = null;

            try
            {
                string subcategory = "";
                string category = "";
                List<string> properties;
                CProperty aProp = null;

                // obtain the class's superclasses.
                if (getProductSuperclasses(productClassification, ref subcategory, ref category))
                {
                    properties = getProductProperties(productClassification, subcategory, category);

                    if (properties.Count > 0)
                    {
                        ret = new CProduct();

                        ret.ProductClassification = productClassification;
                        ret.VersionID = 1;

                        foreach (string property in properties)
                        {
                            aProp = createPropertyObject(productClassification, property, isForBidUsage);

                            if (aProp != null)
                            {
                                ret.ProductProperties.Add(aProp);
                            }
                        }
                    }
                }
            }
            catch (Exception) { }

            return (ret);
        }

        public List<String> GetProductCategories(bool isForBidUsage)
        {
            List<string> ret = null;

            try
            {
                CIQPlatformEntities ce = new CIQPlatformEntities();

                var qry = from ProductClass pc in ce.ProductClasses
                          select pc.Category;


                if (qry.Count() > 0)
                {
                    ret = qry.ToList();
                }

                if (isForBidUsage == true)
                {
                    ret.Insert(0, "-Any-");
                }
            }
            catch (Exception) { }

            return (ret);
        }

        public List<String> GetProductSubcategories(string sCategory, bool isForBidUsage)
        {
            List<string> ret = null;

            try
            {
                CIQPlatformEntities ce = new CIQPlatformEntities();

                var qry = from ProductClass pc in ce.ProductClasses
                          where pc.Category == sCategory
                          select pc.Subcategory;

                if (qry.Count() > 0)
                {
                    ret = qry.ToList();
                }

                if (isForBidUsage == true)
                {
                    ret.Insert(0, "-Any-");
                }

            }
            catch (Exception) { }

            return (ret);
        }

        public List<String> GetProductClassifications(string sCategory, string sSubcategory, bool isForBidUsage)
        {
            List<string> ret = null;

            try
            {
                CIQPlatformEntities ce = new CIQPlatformEntities();

                var qry = from ProductClass pc in ce.ProductClasses
                          where pc.Category == sCategory && pc.Subcategory == sSubcategory
                          select pc.ProductClass1;

                if (qry.Count() > 0)
                {
                    ret = qry.ToList();
                }

                if (isForBidUsage == true)
                {
                    ret.Insert(0, "-Any-");
                }
            }
            catch (Exception) { }

            return (ret);
        }
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// The code in this file is confidential and  
// Copyright (C) 2011-2012 by Todd Schick 
/////////////////////////////////////////////////////////////////////////////////////////////////////////