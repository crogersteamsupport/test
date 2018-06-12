using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Linq;

namespace TeamSupport.CDI.linq
{
    [Table(Name = "TicketSeverities")]
    class TicketSeverity
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _ticketSeverityID;
        [Column(Storage = "_ticketSeverityID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int TicketSeverityID { get { return _ticketSeverityID; } }

        [Column]
        public string Name;
        [Column]
        public int? Severity;
#pragma warning restore CS0649

        public static void AssignTicketSeverities()
        {
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    Table<TicketSeverity> severityTable = db.GetTable<TicketSeverity>();

                    var query = (from t in severityTable
                                 where !t.Severity.HasValue
                                 select t);

                    TicketSeverity[] needsValue = query.ToArray();
                    foreach (TicketSeverity severity in needsValue)
                        severity.AssignSeverity();
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("Ticket Read failed", e);
            }
        }

        static string[][] Priorities = new string[][] {
            new string[] { "urgent", "immediate", "highest", "emergency", "critical", "urgente", "blocker", "down", "crítica", "very severe", "a.s.a.p." },
            new string[] { "high", "major", "serious", "upset", "hour", "moderately severe", "important", "escalation", "expedite" },
            new string[] { "normal", "medium", "moderate", "average", "med", "non-urgent", "standard", "default", "broken", "day", "general", "significant", "help desk"},
            new string[] { "low", "minor", "insignificant", "inquiry", "curious", "query", "menor", "simple", "training",
                "very light", "days", "trivial", "information", "cosmetic" },
            new string[] { "development", "enhancement", "future", "no impact", "no action"},
            new string[] { "unassigned", "unclassified", "not set", "needs triage", "none", "new", "blank", "needs leibel", "select", "no priority", "to be" }
        };

        public void AssignSeverity()
        {
            if (Severity.HasValue)
                return;

            for (int i = 0; i < Priorities.Length; ++i)
            {
                foreach (string priorityName in Priorities[i])
                {
                    string lowerCase = Name.ToLower();

                    if (Name.ToLower().Contains(priorityName))
                    {
                        Severity = i;
                        return;
                    }
                }
            }
        }
    }
}
