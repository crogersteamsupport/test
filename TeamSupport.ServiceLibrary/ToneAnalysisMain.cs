using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WatsonToneAnalyzer
{
    public static class Program
    {

        static void Main(string[] args)
        {
            if (!Environment.UserInteractive)
                // running as service
                using (var service = new WatsonToneAnalyzerService())
                    ServiceBase.Run(service);
            else
            {
                // running as console app
                Start(args);

                Console.WriteLine("Press any key to stop...");
                Console.ReadKey(true);

                Stop();
            }
        }

        public static void Start(string[] args)
        {
            //ActionsToAnalyzer.FindActionsToAnalyze();
            //System.Threading.Thread.Sleep(1000);
            WatsonAnalyzer.AnalyzeActions();

        }

        public static void Stop()
        {
            // onstop code here
        }
    }
}
