using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WatsonToneAnalyzer
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };

    public class WatsonToneAnalyzerService : ServiceBase
    {
        public WatsonToneAnalyzerService()
        {
            //InitializeComponent();

            // base.EventLog - The source is the ServiceName of the service, and the log is the computer's Application log.
            ServiceName = "TeamSupport.WatsonToneAnalyzerService";
            base.AutoLog = true;    // auto log to Application log for service start, stop...

            // timer to add a 1 minute delay between each execution
            _timer = new System.Timers.Timer();
            _timer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings.Get("WatsonInterval"));
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            _timer.AutoReset = false;
            _timerEnable = true;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        bool _timerEnable;
        System.Timers.Timer _timer;

        // expose so running as console app runs the same code
        public void StartTimer()
        {
            _timerEnable = true;
            _timer.Start();
        }

        // expose so running as console app runs the same code
        public void StopTimer()
        {
            _timer.Stop();
            _timerEnable = false;
        }

        protected override void OnStart(string[] args)
        {
            //base.EventLog.WriteEntry("In OnStart");

            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus
            {
                dwCurrentState = ServiceState.SERVICE_START_PENDING,
                dwWaitHint = 100000
            };
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            StartTimer();

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
            StopTimer();
        }

        protected override void OnContinue()
        {
        }

        // only query periodically
        static int WatsonQueryIntervalMinutes = Int32.Parse(ConfigurationManager.AppSettings.Get("WatsonQueryIntervalMinutes"));
        DateTime _lastQueryTime = DateTime.MinValue;

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // only do the Action table query every 20 minutes to catch what we might have missed?
            TimeSpan timeSince = DateTime.Now - _lastQueryTime;
            if(timeSince.Minutes >= WatsonQueryIntervalMinutes)
            {
                WatsonEventLog.WriteEntry("Query for ActionsToAnalyze");
                ActionsToAnalyzer.FindActionsToAnalyze();
                _lastQueryTime = DateTime.Now;
            }

            WatsonAnalyzer.AnalyzeActions();
            if(_timerEnable)
                _timer.Start();
        }

        private void InitializeComponent()
        {
        }
    }
}
