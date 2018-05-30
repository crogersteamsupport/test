using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.Linq;

namespace TeamSupport.CDI
{
    class Client
    {
        OrganizationAnalysis _organizationAnalysis;
        IntervalData _cdiData;
        //ICDIStrategy _iCdiStrategy;

        public Client(OrganizationAnalysis organizationAnalysis)
        {
            _organizationAnalysis = organizationAnalysis;
        }

        public int OrganizationID { get { return _organizationAnalysis.OrganizationID; } }

        public void GenerateIntervals()
        {
            _organizationAnalysis.GenerateIntervals();
        }

        IntervalData _default = new IntervalData();

        public IntervalData GetCDIIntervalData()
        {
            IntervalData end = _organizationAnalysis.Current();
            if (end == null)
            {
                IntervalData last = _organizationAnalysis.Intervals.Last();
                _cdiData = new IntervalData()
                {
                    _timeStamp = _organizationAnalysis._dateRange.EndDate,
                    _totalTicketsCreated = last._totalTicketsCreated,
                    _newCount = 0,
                    _openCount = last._openCount,
                    _medianDaysOpen = _organizationAnalysis.MedianDaysOpen()
                };
            }
            else
            {
                _cdiData = new IntervalData()
                {
                    _timeStamp = end._timeStamp,
                    _totalTicketsCreated = end._totalTicketsCreated, // 1. TotalTicketsCreated
                    _newCount = end._newCount, // 2. CreatedLast30
                    _openCount = end._openCount,   // 3. TicketsOpen
                    _medianDaysOpen = end._medianDaysOpen, // 4. AvgTimeOpen
                };
            }


            _cdiData._medianDaysToClose = IntervalData.MedianTotalDaysToClose(_organizationAnalysis.Tickets);  // 5. AvgTimeToClose

            return _cdiData;
        }

        public void InvokeCDIStrategy(IntervalPercentiles clientPercentiles, linq.CDI_Settings weights)
        {
            clientPercentiles.CDI1(_cdiData, weights);
        }

        public string ToStringCDI1()
        {
            //return _organizationAnalysis.ToString();
            if (_cdiData == null)
                return string.Empty;
            return _cdiData.ToStringCDI1();
        }

        public void WriteCdiByOrganization(DateTime timestamp)
        {
            //if (!_organizationAnalysis.ClientOrganizationID.HasValue)
            //    return;

            //IntervalData last = _organizationAnalysis.Intervals.Last();
            //if (last._timeStamp < timestamp.AddDays(-90))
            //    return; // too old...

            //int clientID = _organizationAnalysis.ClientOrganizationID.Value;
            //linq.Organization organization;
            //if (linq.Organization.TryGet(clientID, out organization))
            //{
            //    //CDIEventLog.Write("ClientID\tClient\tCreatedLast30\tTotalTicketsCreated\tCDI\tnew\ttotal\tCDI-2\t");
            //    CDIEventLog.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", clientID, organization.Name,
            //        organization.CreatedLast30, organization.TotalTicketsCreated, organization.CustDisIndex,
            //        last._newCount, last._totalTicketsCreated, last.CDI));
            //    //CDIEventLog.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", clientID, organization.Name, _organizationAnalysis.Intervals.Count, organization.CustDisIndex, last.CDI, last.ToString()));
            //}
        }

        public void WriteIntervals()
        {
            //int clientID = _organizationAnalysis.ClientOrganizationID.Value;
            //linq.Organization organization;
            //if (!linq.Organization.TryGet(clientID, out organization))
            //    return;

            //string clientName = organization.Name;
            //foreach (IntervalData interval in _organizationAnalysis.Intervals)
            //    CDIEventLog.WriteLine(String.Format("{0}\t{1}\t{2}", clientID, clientName, interval.ToString()));
        }

        public void Write()
        {
            ////CDIEventLog.Write("ClientID\tClient\tTotalTicketsCreated\tTicketsOpen\tCreatedLast30\tAvgTimeOpen\tAvgTimeToClose\tCustDisIndex\tCDI-2");
            //int clientID = _organizationAnalysis.ClientOrganizationID.Value;
            //linq.Organization organization;
            //linq.Organization.TryGet(clientID, out organization);
            //CDIEventLog.WriteLine(String.Format("{0}\t{1}", organization.ToStringCDI1(), ToStringCDI1()));
        }

        public void Save(Dictionary<int, linq.CDI> distressIndices, Table<linq.CDI> table)
        {
            try
            {
                linq.CDI cdi = null;
                if(distressIndices.ContainsKey(OrganizationID))
                    cdi = distressIndices[OrganizationID];
                if (cdi == null)
                {
                    cdi = new linq.CDI();
                    table.InsertOnSubmit(cdi);
                }

                cdi.OrganizationID = _organizationAnalysis.OrganizationID;
                cdi.ParentID = _organizationAnalysis.ParentID;
                cdi.Timestamp = _cdiData._timeStamp;
                cdi.TotalTicketsCreated = _cdiData._totalTicketsCreated;
                cdi.TicketsOpen = _cdiData._openCount;
                cdi.CreatedLast30 = _cdiData._newCount;
                cdi.AvgTimeOpen = (int)Math.Round(_cdiData._medianDaysOpen);
                cdi.AvgTimeToClose = _cdiData._medianDaysToClose.HasValue ? (int)Math.Round(_cdiData._medianDaysToClose.Value) : 0;
                cdi.CustDisIndex = _cdiData.CDI.Value;
            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("Save failed", e);
            }

        }

        public void Save(Dictionary<int, linq.Organization> organizations, Table<linq.Organization> table)
        {
            try
            {
                linq.Organization organization = organizations[OrganizationID];
                organization.TotalTicketsCreated = _cdiData._totalTicketsCreated;
                organization.TicketsOpen = _cdiData._openCount;
                organization.CreatedLast30 = _cdiData._newCount;
                organization.AvgTimeOpen = (int)Math.Round(_cdiData._medianDaysOpen);
                organization.AvgTimeToClose = _cdiData._medianDaysToClose.HasValue ? (int)Math.Round(_cdiData._medianDaysToClose.Value) : 0;
                organization.CustDisIndex = _cdiData.CDI.Value;
            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("Save failed", e);
            }

        }


    }
}