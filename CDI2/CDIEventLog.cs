using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CDI2
{
    /// <summary>
    /// Send detailed logging to TeamSupport custom log
    /// </summary>
    class CDIEventLog
    {
        public static EventLog _eventLog;

        /// <summary>
        /// Static constructor
        /// </summary>
        static CDIEventLog()
        {
            try
            {
                // To uninstall this log, run these commands in PowerShell and then reboot 
                //  Remove -EventLog -Source "WatsonToneAnalyzerService"
                //  Remove-EventLog -LogName "TeamSupport"
                const string logName = "TeamSupport";
                const string source = "CDI-2";

                _eventLog = new EventLog(logName);
                _eventLog.Source = source;
                if (!EventLog.SourceExists(source))
                    EventLog.CreateEventSource(source, logName);
            }
            catch(Exception)
            {
                EventLog.WriteEntry("Application", "Unable to open TeamSupport log on CDI-2 source");
            }
        }

        public static void WriteEntry(string message, EventLogEntryType type = EventLogEntryType.Information)
        {
            _eventLog.WriteEntry(message, type);
        }

        public static void WriteEntry(string message, Exception e)
        {
            Debugger.Break();
            WriteEntry(message + e.ToString() + " ----- STACK: " + e.StackTrace.ToString(), EventLogEntryType.Error);
        }
    }
}
