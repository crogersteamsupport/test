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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            CDI2 cdi = new CDI2(TimeSpan.FromDays(7), 26);
            cdi.Run();
            long ellapsed = stopwatch.ElapsedMilliseconds;
        }
    }
}
