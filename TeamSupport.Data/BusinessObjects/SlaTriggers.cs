using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class SlaTrigger
  {
  }
  
  public partial class SlaTriggers
  {

    public void LoadByTicketType(int organizationID, int slaLevelID, int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT st.*, ts.Position, ts.Name AS Severity  
FROM SlaTriggers st 
LEFT JOIN TicketSeverities ts ON ts.TicketSeverityID = st.TicketSeverityID
WHERE st.TicketType = @TicketType
AND st.OrganizationID = @OrganizationID
AND st.SlaLevelID = @SlaLevelID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@SlaLevelID", slaLevelID);
        command.Parameters.AddWithValue("@TicketType", ticketTypeID);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByTicketTypeAndSeverity(int slaLevelID, int ticketTypeID, int ticketSeverityID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"

SELECT * 
FROM SlaTriggers st 
WHERE st.TicketTypeID = @TicketType
AND st.TicketSeverityID = @TicketSeverityID
AND st.SlaLevelID = @SlaLevelID";

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@SlaLevelID", slaLevelID);
        command.Parameters.AddWithValue("@TicketType", ticketTypeID);
        command.Parameters.AddWithValue("@TicketSeverityID", ticketSeverityID);
        Fill(command);
      }
    }

    public static void DeleteByTicketTypeID(LoginUser loginUser, int ticketTypeID)
    {
      SlaTriggers triggers = new SlaTriggers(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM SlaTriggers WHERE (TicketTypeID = @ticketTypeID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@ticketTypeID", ticketTypeID);
        triggers.ExecuteNonQuery(command, "SlaTriggers");
      }
    }
    public static void DeleteByTicketSeverityID(LoginUser loginUser, int ticketSeverityID)
    {
      SlaTriggers triggers = new SlaTriggers(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM SlaTriggers WHERE (TicketSeverityID = @ticketSeverityID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@ticketSeverityID", ticketSeverityID);
        triggers.ExecuteNonQuery(command, "SlaTriggers");
      }
    }
  }

 
  
}
