using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ContactsViewItem
  {
  }
  
  public partial class ContactsView
  {
    public void LoadByParentOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ContactsView WHERE OrganizationParentID = @OrganizationID AND (MarkDeleted = 0) ORDER BY LastName, FirstName";
        command.CommandText = InjectCustomFields(command.CommandText, "UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }
    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ContactsView WHERE OrganizationID = @OrganizationID AND (MarkDeleted = 0) ORDER BY LastName, FirstName";
        command.CommandText = InjectCustomFields(command.CommandText, "UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }
    public void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT cv.* FROM ContactsView cv LEFT JOIN UserTickets ut ON ut.UserID = cv.UserID WHERE ut.TicketID = @TicketID AND (cv.MarkDeleted = 0) ORDER BY LastName, FirstName ";
        command.CommandText = InjectCustomFields(command.CommandText, "cv.UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }
  }
  
}
