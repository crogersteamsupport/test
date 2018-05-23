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

        [Column]
        public int TotalTicketsCreated;
        //[Column]
        //public int TicketsOpen;
        [Column]
        public int CreatedLast30;
        //[Column]
        //public int AvgTimeOpen;
        //[Column]
        //public int AvgTimeToClose;
        [Column]
        public int CustDisIndex;
#pragma warning restore CS0649

        // for test output...
        public static bool TryGet(int organizationID, out Organization organization)
        {
            if (_organizations == null)
                _organizations = LoadOrganizations();

            if (!_organizations.ContainsKey(organizationID))
            {
                organization = null;
                return false;
            }
            organization = _organizations[organizationID];
            return true;
        }

        static Dictionary<int, Organization> _organizations;
        static Dictionary<int, Organization> LoadOrganizations()
        {
            Dictionary<int, Organization> allOrganizations = null;
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

                allOrganizations = new Dictionary<int, Organization>();
                foreach (Organization og in organizations)
                    allOrganizations[og.OrganizationID] = og;
            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("Organization Read failed", e);
            }

            return allOrganizations;
        }
    }
}
