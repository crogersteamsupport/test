﻿using System;
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
                _cdiData = _default;
                _cdiData._timeStamp = _organizationAnalysis._dateRange.EndDate;
                _cdiData._totalTicketsCreated = _organizationAnalysis.Intervals.Last()._totalTicketsCreated;
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
                    _medianDaysToClose = IntervalData.MedianTotalDaysToClose(_organizationAnalysis.Tickets),  // 5. AvgTimeToClose
                };
            }

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

        public void Save(linq.CDI cdi2, Table<linq.CDI> table)
        {
            try
            {
                if (cdi2 == null)
                {
                    cdi2 = new linq.CDI()
                    {
                        OrganizationID = _organizationAnalysis.OrganizationID,
                        ParentID = _organizationAnalysis.ParentID,
                        Timestamp = _cdiData._timeStamp,
                        TotalTicketsCreated = _cdiData._totalTicketsCreated,
                        TicketsOpen = _cdiData._totalTicketsCreated,
                        CreatedLast30 = _cdiData._newCount,
                        AvgTimeOpen = (int)Math.Round(_cdiData._medianDaysOpen),
                        AvgTimeToClose = _cdiData._medianDaysToClose.HasValue ? (int)Math.Round(_cdiData._medianDaysToClose.Value) : 0,
                        CustDisIndex = _cdiData.CDI.Value
                    };
                    table.InsertOnSubmit(cdi2);
                }
                else
                {
                    cdi2.OrganizationID = _organizationAnalysis.OrganizationID;
                    cdi2.ParentID = _organizationAnalysis.ParentID;
                    cdi2.Timestamp = _cdiData._timeStamp;
                    cdi2.TotalTicketsCreated = _cdiData._totalTicketsCreated;
                    cdi2.TicketsOpen = _cdiData._totalTicketsCreated;
                    cdi2.CreatedLast30 = _cdiData._newCount;
                    cdi2.AvgTimeOpen = (int)Math.Round(_cdiData._medianDaysOpen);
                    cdi2.AvgTimeToClose = _cdiData._medianDaysToClose.HasValue ? (int)Math.Round(_cdiData._medianDaysToClose.Value) : 0;
                    cdi2.CustDisIndex = _cdiData.CDI.Value;
                }
            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("Savbe failed", e);
            }

        }

    }
}