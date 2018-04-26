﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace TeamSupport.CDI
{
    public class TicketJoin : IComparable<TicketJoin>
    {
        public int TicketID;
        public string TicketStatusName;
        public string TicketTypeName;
        public int OrganizationID;
        public DateTime? DateClosed;
        public string TicketSource;
        public DateTime DateCreated;
        public int ActionsCount;
        public bool IsClosed;

        public int CompareTo(TicketJoin other) { return DateCreated.CompareTo(other.DateCreated); }
        public TimeSpan TimeOpen { get { return DateClosed.Value - DateCreated; } }
        public double TotalDaysOpen { get { return TimeOpen.TotalDays; } }
        public override string ToString() { return DateCreated.ToShortDateString(); }

        public double ScaledTimeOpen(TimeScale scale)
        {
            TimeSpan timeSpan = TimeOpen;
            double result;
            switch (scale)
            {
                case TimeScale.Minutes:
                    result = timeSpan.TotalMinutes;
                    break;
                case TimeScale.Hours:
                    result = timeSpan.TotalHours;
                    break;
                case TimeScale.Days:
                    result = timeSpan.TotalDays;
                    break;
                case TimeScale.Weeks:
                    result = timeSpan.TotalDays / 7;
                    break;
                default:
                    result = Double.PositiveInfinity;
                    break;
            }

            return result;
        }
    }
}
