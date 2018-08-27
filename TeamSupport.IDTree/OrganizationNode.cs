using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO;
using System.Diagnostics;

namespace TeamSupport.IDTree
{
    /// <summary>
    /// Wrapper for valid OrganizationID
    /// </summary>
    public class OrganizationNode : IDNode
    {
        public int OrganizationID { get { return Request.Authentication.OrganizationID; } }

        /// <summary> OrganizationID and UserID come from ConnectionContext.Authentication </summary>
        public OrganizationNode(ClientRequest connection) : base(connection)
        {
            //DBReader.VerifyOrganization(_db, OrganizationID); // connection already verified
        }

        public string AttachmentPath(int id)
        {
            string path = Request.AttachmentPath(id);
            path = Path.Combine(Path.Combine(path, "Organizations"), OrganizationID.ToString());
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public override void Verify()
        {
            Verify($"SELECT OrganizationID FROM Organizations WITH (NOLOCK) WHERE OrganizationID={OrganizationID}");
        }

    }
}
