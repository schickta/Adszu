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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CIQProductIdentityServices" in code, svc and config file together.
    public class CIQProductIdentityServices : ICIQProductIdentityServices
    {
        public CProductIdentity CreateProductIdentityInstance ()
        {
            CProductIdentity ret = null;

            try
            {
                ret = new CProductIdentity ();
            }
            catch (Exception) { }

            return (ret);
        }

        public List<CProductIdentity> QueryKind(string flattenedProduct)
        {
            List<CProductIdentity> retVal = new List<CProductIdentity>();
            CIQPlatformEntities ce = new CIQPlatformEntities();

            try
            {
                var qry = from ProductCode pc in ce.ProductCodes
                          where pc.ProdBlob == flattenedProduct
                          select pc;

                foreach (ProductCode pc in qry)
                {
                    CProductIdentity cpi = new CProductIdentity();

                    cpi.Description = pc.Description;
                    cpi.ProductCategory = pc.ProdCategory;
                    cpi.ProductClass = pc.ProdClass;
                    cpi.ProductIdentity = pc.ProductCode1;
                    cpi.ProductSubcategory = pc.ProdSubcategory;

                    retVal.Add(cpi);
                }
            }
            catch (Exception) { }

            return (retVal);
        }

        public bool SaveProductIdentity(CProductIdentity cPI, string flattenedProduct)
        {
            bool ret = false;

            try
            {
                CIQPlatformEntities ce = new CIQPlatformEntities();
                ProductCode pcode = new ProductCode();

                pcode.ID = (from o in ce.ProductCodes select o).Count(); 
                pcode.ProductCode1 = cPI.ProductIdentity;
                pcode.Description = cPI.Description;
                pcode.ProdCategory = cPI.ProductCategory;
                pcode.ProdSubcategory = cPI.ProductSubcategory;
                pcode.ProdClass = cPI.ProductClass;
                pcode.ProdBlob = flattenedProduct;

                ce.ProductCodes.AddObject(pcode);
                ce.SaveChanges();

                ret = true;
            }
            catch (Exception) { }

            return (ret);
        }

        public bool MapIdentity(string mapping, string productIdentity)
        {
            bool retVal = false;

            try
            {
                CIQPlatformEntities ce = new CIQPlatformEntities();
                MappingCode mp = new MappingCode();

                mp.ID = (from o in ce.MappingCodes select o).Count();
                mp.MappedCode = mapping;
                mp.ProductCode = productIdentity;

                ce.MappingCodes.AddObject(mp);
                ce.SaveChanges();

                retVal = true;
            }
            catch (Exception) { }

            return (retVal);
        }

        public CProductIdentity QueryIdentity(string productCode)
        {
            CProductIdentity retVal = null;
            CIQPlatformEntities ce = new CIQPlatformEntities();

            try
            {
                // First see if the input productCode is in the mapping table,
                // if so, use the MAPPED code to search for the product identity.
                // Otherwise, use the input productCode directly.

                var qry = from MappingCode mc in ce.MappingCodes
                          where mc.MappedCode == productCode
                          select mc;

                // We found a mapping code, so use the mapped product code
                // for our identity search, which comes next.

                if (qry.Count() > 0)
                {
                    MappingCode mc = (MappingCode)qry.First();
                    productCode = mc.ProductCode;
                }

                // Search the productcodes table.

                var pcQry = from ProductCode pc in ce.ProductCodes
                            where pc.ProductCode1 == productCode
                            select pc;

                // If we found one, we'll return that ProductIdentity.

                if (pcQry.Count() > 0)
                {
                    ProductCode pc = (ProductCode)pcQry.First();
                    retVal = new CProductIdentity();

                    retVal.Description = pc.Description;
                    retVal.ProductCategory = pc.ProdCategory;
                    retVal.ProductClass = pc.ProdClass;
                    retVal.ProductIdentity = pc.ProductCode1;
                    retVal.ProductSubcategory = pc.ProdSubcategory;
                }
            }
            catch (Exception) { }

            return (retVal);
        }

        public string QueryProduct(string productCode)
        {
            string retVal = null;
            CIQPlatformEntities ce = new CIQPlatformEntities();

            try
            {
                // First see if the input productCode is in the mapping table,
                // if so, use the MAPPED code to search for the product identity.
                // Otherwise, use the input productCode directly.

                var qry = from MappingCode mc in ce.MappingCodes
                          where mc.MappedCode == productCode
                          select mc;

                // We found a mapping code, so use the mapped product code
                // for our identity search, which comes next.

                if (qry.Count() > 0)
                {
                    MappingCode mc = (MappingCode)qry.First();
                    productCode = mc.ProductCode;
                }

                // Search the productcodes table.

                var pcQry = from ProductCode pc in ce.ProductCodes
                            where pc.ProductCode1 == productCode
                            select pc;

                // If we found one, we'll return that Product

                if (pcQry.Count() > 0)
                {
                    ProductCode pc = (ProductCode)pcQry.First();
                    retVal = pc.ProdBlob;
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

