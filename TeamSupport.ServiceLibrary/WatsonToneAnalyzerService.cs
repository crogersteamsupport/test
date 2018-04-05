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
        const string source = "MySource";
        const string logName = "MyNewLog";

        public WatsonToneAnalyzerService()
        {
            ServiceName = "WatsonToneAnalyzer";

            // Event Log
            _eventLog = new EventLog(source);
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, logName);
            }
            _eventLog.Source = source;
            _eventLog.Log = logName;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        System.Timers.Timer _timer = new System.Timers.Timer();

        protected override void OnStart(string[] args)
        {
            _eventLog.WriteEntry("In OnStart");

            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus
            {
                dwCurrentState = ServiceState.SERVICE_START_PENDING,
                dwWaitHint = 100000
            };
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // service is a simple timer...
            _timer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings.Get("WatsonInterval"));
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            _timer.Start();

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _eventLog.WriteEntry("In OnStop");
            Program.Stop();
        }

        protected override void OnContinue()
        {
            _eventLog.WriteEntry("In OnContinue.");
        }

        int _timerCount = 0;

        private System.Diagnostics.EventLog _eventLog;
        private int eventId = 1;

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // only do the Action table query every 20 minutes to catch what we might have missed?
            if (++_timerCount > 20)
            {
                _eventLog.WriteEntry("Query for ActionsToAnalyze", EventLogEntryType.Information, eventId++);
                ActionsToAnalyzer.FindActionsToAnalyze();
                _timerCount = 0;
            }

            WatsonAnalyzer.AnalyzeActions();
        }

        private void InitializeComponent()
        {
            this._eventLog = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this._eventLog)).BeginInit();
            this.ServiceName = "WatsonToneAnalyzerService";
            ((System.ComponentModel.ISupportInitialize)(this._eventLog)).EndInit();
        }
    }
}
