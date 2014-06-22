using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFServiceWebRole
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CIQMemberServices" in code, svc and config file together.
    public class CIQMemberServices : ICIQMemberServices
    {
        public CMember AuthenticateMember(string userID, string password)
        {
            CMember retVal = null;
            CIQPlatformEntities ce = new CIQPlatformEntities();

            try
            {
                var qry = from Member m in ce.Members
                          join Company co in ce.Companies on m.CoID equals co.ID
                          where m.UserID == userID && m.Password == password
                          select new { ID = m.ID, First = m.First, Last = m.Last, 
                                       EMail = m.EMail, Phone = m.Phone,
                                       CompanyName = co.Name, Website = co.Website,
                                       MemberType = co.Type };

                if (qry.Count () > 0)
                {
                    var m = qry.First();

                    if (m != null)
                    {
                        retVal = new CMember();
                        retVal.ID = m.ID;
                        retVal.First = m.First;
                        retVal.Last = m.Last;
                        retVal.EMail = m.EMail;
                        retVal.Phone = m.Phone;
                        retVal.CompanyName = m.CompanyName;
                        retVal.Website = m.Website;
                        retVal.MemberType = m.MemberType;
                    }
                }
            }
            catch (Exception) { }

            return (retVal);
        }
    }
}
