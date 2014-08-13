using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ReportTicketsViewItem
  {
  }
  
  public partial class ReportTicketsView
  {
    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ReportTicketsView WHERE (OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command, "Tickets,Actions");
      }
    }

    public void LoadByTicketTypeID(int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ReportTicketsView WHERE (TicketTypeID = @TicketTypeID) ORDER BY TicketNumber";
        command.CommandText = InjectCustomFields(command.CommandText, "TicketID", ReferenceType.Tickets, ticketTypeID);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
        Fill(command);
      }
    }

  }
  
}
