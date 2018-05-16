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
    [Table(Name = "Organizations")]
    class Organization
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _organizationID;
        [Column(Storage = "_organizationID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int OrganizationID { get { return _organizationID; } }

        [Column]
        public int? ParentID;
        [Column]
        public string Name;

        //[Column]
        //public int TotalTicketsCreated;
        //[Column]
        //public int TicketsOpen;
        //[Column]
        //public int CreatedLast30;
        //[Column]
        //public int AvgTimeOpen;
        //[Column]
        //public int AvgTimeToClose;
        //[Column]
        //public int CustDisIndex;
#pragma warning restore CS0649


        public static string GetOrganizationName(int organizationID)
        {
            if (organizationNames == null)
                organizationNames = LoadOrganizationNames();

            if(organizationNames.ContainsKey(organizationID))
                return organizationNames[organizationID];
            return string.Empty;
        }

        static Dictionary<int, string> organizationNames;
        static Dictionary<int, string> LoadOrganizationNames()
        {
            Dictionary<int, string> allOrganizations = null;
            try
            {
                Organization[] organizations = null;
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    Table<Organization> OrganizationsTable = db.GetTable<Organization>();
                    organizations = (from o in OrganizationsTable select o).ToArray();
                }

                allOrganizations = new Dictionary<int, string>();
                foreach (Organization og in organizations)
                    allOrganizations[og.OrganizationID] = og.Name;
            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("Organization Read failed", e);
            }

            return allOrganizations;
        }

        public static Dictionary<int, HashSet<int>> LoadCustomerOrganizations()
        {
            Dictionary<int, HashSet<int>> allOrganizations = null;
            try
            {
                Organization[] organizations = null;
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    Table<Organization> OrganizationsTable = db.GetTable<Organization>();
                    organizations = (from o in OrganizationsTable select o).ToArray();
                }

                allOrganizations = new Dictionary<int, HashSet<int>>();
                Organization[] customers = organizations.Where(o => !o.ParentID.HasValue || (o.ParentID.Value == 1)).ToArray();
                foreach (Organization og in customers)
                    allOrganizations[og.OrganizationID] = new HashSet<int>();

                Organization[] clients = organizations.Where(o => o.ParentID.HasValue && (o.ParentID.Value != 1)).ToArray();
                foreach (Organization org in clients)
                {
                    if (allOrganizations.ContainsKey(org.ParentID.Value))
                        allOrganizations[org.ParentID.Value].Add(org.OrganizationID);
                }
            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("Organization Read failed", e);
            }

            return allOrganizations;
        }

    }
}
