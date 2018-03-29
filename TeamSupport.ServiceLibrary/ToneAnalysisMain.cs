using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace WatsonToneAnalyzer
{
    public static class Program
    {
        #region Nested classes to support running as service
        public const string ServiceName = "WatsonToneAnalyzer";

        public class WatsonToneAnalyzer : ServiceBase
        {
            public WatsonToneAnalyzer()
            {
                ServiceName = Program.ServiceName;
            }

            protected override void OnStart(string[] args)
            {
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings.Get("WatsonInterval"));
                timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
                timer.Start();
            }

            protected override void OnStop()
            {
                Program.Stop();
            }

            int _timerCount = 0;
            private int eventId = 1;
            public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
            {
                // TODO: Insert monitoring activities here.  

                // only do the Action table query every 20 minutes to catch what we might have missed?
                if (++_timerCount > 20)
                {
                    //_eventLog.WriteEntry("Query for ActionsToAnalyze", EventLogEntryType.Information, eventId++);
                    ActionsToAnalyzer.GetHTML();
                    _timerCount = 0;
                    //System.Threading.Thread.Sleep(1000);
                }
                WatsonAnalyzer.AnalyzeActions();
                //EventLog.WriteEntry(EVENT_SOURCE, "Elapsed =" + sw.Elapsed);
            }

        }
        #endregion

        //private static EventLog _eventLog = new EventLog();

        static void Main(string[] args)
        {
            //if (!EventLog.SourceExists("WatsonToneAnalyzer"))
            //    EventLog.CreateEventSource("WatsonToneAnalyzer", "MyNewLog");
            //_eventLog.Source = "WatsonToneAnalyzer";
            //_eventLog.Log = "MyNewLog";

            if (!Environment.UserInteractive)
                // running as service
                using (var service = new WatsonToneAnalyzer())
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

        private static void Start(string[] args)
        {
            //_eventLog.WriteEntry("In OnStart");

            //ActionsToAnalyzer.GetHTML();
            //System.Threading.Thread.Sleep(1000);
            WatsonAnalyzer.AnalyzeActions();

        }

        private static void Stop()
        {
            //_eventLog.WriteEntry("In OnStop");
            // onstop code here
        }
    }
}
