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
            public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
            {
                Stopwatch sw = new Stopwatch();

                sw.Start();
                // TODO: Insert monitoring activities here.  
                ActionsToAnalyzer.GetHTML();
                System.Threading.Thread.Sleep(1000);
                WatsonAnalyzer.GetAction();
                sw.Stop();
                //EventLog.WriteEntry("Application", "Elapsed =" + sw.Elapsed);
            }

        }
        #endregion

        static void Main(string[] args)
        {
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
            // onstart code here
            
            ActionsToAnalyzer.GetHTML();
            System.Threading.Thread.Sleep(1000);
            WatsonAnalyzer.GetAction();
            
        }

        private static void Stop()
        {
            // onstop code here
        }
    }
}
