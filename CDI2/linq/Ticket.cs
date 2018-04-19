using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace CDI2
{
    enum TimeScale
    {
        Minutes,
        Hours,
        Days,
        Weeks
    }

    [Table(Name = "Tickets")]
    class Ticket : IComparable<Ticket>
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _ticketID;
        [Column(Storage = "_ticketID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int TicketID { get { return _ticketID; } }

        [Column]
        public int TicketStatusID;
        [Column]
        public int TicketTypeID;
        [Column]
        public int OrganizationID;
        [Column]
        public DateTime? DateClosed;
        [Column]
        public DateTime DateCreated;
#pragma warning restore CS0649

        public int CompareTo(Ticket other)
        {
            return DateCreated.CompareTo(other.DateCreated);
        }

        static DateTime _now = DateTime.Now;
        public TimeSpan TimeOpen
        {
            get
            {
                if (!DateClosed.HasValue)
                    return _now - DateCreated;
                return DateClosed.Value - DateCreated;
            }
        }

        // default to days
        public double TotalDaysOpen {  get { return TimeOpen.TotalDays; } }

        public double ScaledTimeOpen(TimeScale scale)
        {
            if (!DateClosed.HasValue)
                return Double.PositiveInfinity;

            TimeSpan timeSpan = TimeOpen;
            double result;
            switch(scale)
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

        public override string ToString()
        {
            return DateCreated.ToShortDateString();
        }
    }
}
