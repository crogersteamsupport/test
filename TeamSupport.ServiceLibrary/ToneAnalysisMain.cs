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

        //private static EventLog _eventLog = new EventLog();

        static void Main(string[] args)
        {
            //if (!EventLog.SourceExists("WatsonToneAnalyzer"))
            //    EventLog.CreateEventSource("WatsonToneAnalyzer", "MyNewLog");
            //_eventLog.Source = "WatsonToneAnalyzer";
            //_eventLog.Log = "MyNewLog";

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
            //_eventLog.WriteEntry("In OnStart");

            //ActionsToAnalyzer.GetHTML();
            //System.Threading.Thread.Sleep(1000);
            WatsonAnalyzer.AnalyzeActions();

        }

        public static void Stop()
        {
            //_eventLog.WriteEntry("In OnStop");
            // onstop code here
        }
    }
}
