using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFServiceWebRole
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICIQMemberServices" in both code and config file together.
    [ServiceContract]
    public interface ICIQMemberServices
    {
        [OperationContract]
        CMember AuthenticateMember (string userID, string password);
    }

    [DataContract(Name = "MemberTypes")]
    public enum MemberTypes
    {
        [EnumMember]
        Agency = 0,

        [EnumMember]
        Manufacturer = 1,

        [EnumMember]
        Retailers = 2
    }

    [DataContract]
    public class CMember
    {
        private int _ID;
        private string _first;
        private string _last;
        private string _email;
        private string _phone;
        private string _companyName;
        private string _website;
        private int _memberType;  // 0 = merchant, 1 = manufacturer, 2 = retailer.

        [DataMember]
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        [DataMember]
        public string First
        {
            get { return _first; }
            set { _first = value; }
        }

        [DataMember]
        public string Last
        {
            get { return _last; }
            set { _last = value; }
        }

        [DataMember]
        public string EMail
        {
            get { return _email; }
            set { _email = value; }
        }

        [DataMember]
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        [DataMember]
        public string CompanyName
        {
            get { return _companyName; }
            set { _companyName = value; }
        }

        [DataMember]
        public string Website
        {
            get { return _website; }
            set { _website = value; }
        }

        [DataMember]
        public int MemberType
        {
            get { return _memberType; }
            set { _memberType = value; }
        }
    }
}
