using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;

namespace TeamSupport.CDI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("AssignTicketSeverities"))
            {
                AssignTicketSeverities();
                return;
            }

            Stopwatch totalTimer = new Stopwatch();
            totalTimer.Start();
            int IntervalTimeSpanInDays = int.Parse(ConfigurationManager.AppSettings.Get("IntervalTimeSpanInDays"));
            int IntervalCount = int.Parse(ConfigurationManager.AppSettings.Get("IntervalCount"));
            CDI2 cdi = new CDI2(TimeSpan.FromDays(IntervalTimeSpanInDays), IntervalCount);
            cdi.Run(args);
            cdi.WriteCdiByOrganization();
            CDIEventLog.WriteLine(String.Format("{0:0.00} sec", totalTimer.ElapsedMilliseconds / 1000));
        }

        public static void AssignTicketSeverities()
        {
            try
            {
                TeamSupport.CDI.linq.TicketSeverity.AssignTicketSeverities();
                CDIEventLog.WriteLine("TicketSeverities assigned");
            }
            catch (Exception ex)
            {
                CDIEventLog.WriteLine(ex.ToString());
                CDIEventLog.WriteLine("\nERROR: Did you run the following command on the database?");
                CDIEventLog.WriteLine(" \"ALTER TABLE dbo.TicketSeverities ADD Severity int NULL\"");
            }
        }
    }
}
