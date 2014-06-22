using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFServiceWebRole
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CIQDataAnalytics" in code, svc and config file together.
    public class CIQDataAnalytics : ICIQDataAnalytics
    {
        public CScanDataPoints GetScanData()
        {
            CScanDataPoints retVal = new CScanDataPoints();
            CIQPlatformEntities ce = new CIQPlatformEntities();
            CScanDataPoint sdp;

            var qry = from ScanActivity sa in ce.ScanActivities
                      select sa;

            foreach (ScanActivity sa in qry)
            {
                sdp = new CScanDataPoint();

                sdp.Longitude = sa.Longitude;
                sdp.Latitude = sa.Latitude;
                sdp.ScannedCode = sa.ScanData;
                sdp.Classification = sa.Classification;

                retVal.Add(sdp);
            }

            return (retVal);
        }
    }
}
