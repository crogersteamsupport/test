using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TeamSupport.CDI
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch totalTimer = new Stopwatch();
            totalTimer.Start();
            CDI2 cdi = new CDI2(TimeSpan.FromDays(7), 52);
            cdi.Run();
            //cdi.WriteCdiByOrganization();
            cdi.WriteItervalData(1078, 2633);
            CDIEventLog.WriteEntry(String.Format("CDI Update complete. {0:0.00} sec", totalTimer.ElapsedMilliseconds / 1000));
        }
    }
}
