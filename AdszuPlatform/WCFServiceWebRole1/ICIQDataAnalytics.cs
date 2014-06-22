using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFServiceWebRole
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICIQDataAnalytics" in both code and config file together.
    [ServiceContract]
    public interface ICIQDataAnalytics
    {
        [OperationContract]
        CScanDataPoints GetScanData ();
    }

    [CollectionDataContract
    (Name = "CScanDataPoints")]
    public class CScanDataPoints : List<CScanDataPoint> { }

    [DataContract]
    public class CScanDataPoint
    {
        double _longitude;
        double _latitude;
        string _scannedCode;
        string _classification;

        [DataMember]
        public double Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        [DataMember]
        public double Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }

        [DataMember]
        public string ScannedCode
        {
            get { return _scannedCode; }
            set { _scannedCode = value; }
        }

        [DataMember]
        public string Classification
        {
            get { return _classification; }
            set { _classification = value; }
        }
    }
}
