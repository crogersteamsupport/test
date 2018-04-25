using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CDI2
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            CDIEngine cdi = new CDIEngine(TimeSpan.FromDays(7), 26);
            cdi.Run();
            long ellapsed = stopwatch.ElapsedMilliseconds;
        }
    }
}
