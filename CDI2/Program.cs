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
            CDI2 cdi = new CDI2(TimeSpan.FromDays(7), 1 * 52);
            cdi.Run();
        }
    }
}
