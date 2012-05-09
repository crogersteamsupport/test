using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class Asset
  {
  }
  
  public partial class Assets
  {
    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Assets WHERE OrganizationID = @OrganizationID ORDER BY Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Assets WHERE AssetID IN (SELECT AssetID FROM AssetTickets WHERE TicketID = @TicketID ) ORDER BY Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadByLikeNameOrSerial(int organizationID, string searchTerm, int maxRows)
    {

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP (@MaxRows) * FROM Assets WHERE (Name LIKE '%'+@Term+'%' OR SerialNumber LIKE '%'+@Term+'%') AND (OrganizationID = @OrganizationID) ORDER BY Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@MaxRows", maxRows);
        command.Parameters.AddWithValue("@Term", searchTerm);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);

        Fill(command);
      }

    }


  }
  
}
