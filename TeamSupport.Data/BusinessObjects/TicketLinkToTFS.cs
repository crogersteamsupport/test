using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class TicketLinkToTFSItem
    {
    }

    public partial class TicketLinkToTFS
    {
        //Changes to this method needs to be applied to TicketsView.LoadToPushToJira also.
        public void LoadToPushToJira(CRMLinkTableItem item)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText =
                @"SELECT tfs.* 
				FROM 
					TicketLinkToTFS tfs
					JOIN Tickets t
						ON tfs.TicketID = t.TicketID
				WHERE 
					tfs.SyncWithTFS = 1
					AND t.OrganizationID = @OrgID 
					AND tfs.CrmLinkID = @CrmLinkId
					AND 
					(
						tfs.DateModifiedByJiraTFS IS NULL
						OR 
						(
							tfs.DateModified > DATEADD(s, 2, tfs.DateModifiedByJiraTFS)
							AND t.DateModified > @DateModified
						)
					)
				ORDER BY 
					t.DateCreated DESC";

                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrgID", item.OrganizationID);
                command.Parameters.AddWithValue("@DateModified", item.LastLink == null ? new DateTime(1753, 1, 1) : item.LastLinkUtc.Value.AddHours(-1));
                command.Parameters.AddWithValue("@CrmLinkId", item.CRMLinkID);

                //command.CommandText = "TicketLinkToJiraGet";
                //command.CommandType = CommandType.StoredProcedure;
                //command.Parameters.AddWithValue("@organizationId", item.OrganizationID);
                //command.Parameters.AddWithValue("@dateModified", item.LastLink == null ? new DateTime(1753, 1, 1) : item.LastLinkUtc.Value.AddHours(-1));
                //command.Parameters.AddWithValue("@crmLinkId", item.CRMLinkID);
                //command.Parameters.AddWithValue("@isTicketLinkToJira", 1);
                command.CommandTimeout = 90;
                Fill(command);
            }
        }

        public void LoadByTicketID(int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText =
                @"SELECT * 
				FROM 
					TicketLinkToTFS
				WHERE 
					TicketID = @TicketID
				";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                command.CommandTimeout = 60;

                Fill(command);
            }
        }

        public TicketLinkToTFSItem FindByTicketID(int ticketID)
        {
            foreach (TicketLinkToTFSItem item in this)
            {
                if (item.TicketID == ticketID)
                {
                    return item;
                }
            }

            return null;
        }

        public void LoadByCrmLinkId(int crmLinkId, bool excludeNulls = false)
        {
            using (SqlCommand command = new SqlCommand())
            {
                string query = @"SELECT * 
				FROM 
					TicketLinkToTFS
				WHERE 
					CrmLinkID = @crmLinkId
				";

                if (excludeNulls)
                {
                    query += "AND TFSID IS NOT NULL";
                }

                command.CommandText = query;
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@crmLinkId", crmLinkId);

                Fill(command);
            }
        }
    }

}
