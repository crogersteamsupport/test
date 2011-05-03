using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class OrganizationsViewItem
  {
  }
  
  public partial class OrganizationsView
  {
    public void LoadByParentID(int parentID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM OrganizationsView WHERE (ParentID = @ParentID) ORDER BY Name ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ParentID", parentID);
        Fill(command);
      }
    }

    public void LoadByParentID(int parentID, bool includeCustomFields)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string sql = "SELECT * FROM OrganizationsView WHERE (ParentID = @ParentID) ORDER BY Name";
        if (includeCustomFields) sql = InjectCustomFields(sql, "OrganizationID", ReferenceType.Organizations);
        command.CommandText = sql;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ParentID", parentID);
        Fill(command);
      }
    }

    public void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT ov.* FROM OrganizationsView ov LEFT JOIN OrganizationTickets ot ON ot.OrganizationID = ov.OrganizationID WHERE ot.TicketID = @TicketID ORDER BY Name ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadByProductID(int productID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT ov.* FROM OrganizationsView ov LEFT JOIN OrganizationProducts op ON op.OrganizationID = ov.OrganizationID WHERE op.ProductID = @ProductID ORDER BY Name ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", productID);
        Fill(command);
      }
    }
    public void LoadByVersionID(int versionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT ov.* FROM OrganizationsView ov LEFT JOIN OrganizationProducts op ON op.OrganizationID = ov.OrganizationID WHERE op.ProductVersionID = @VersionID ORDER BY Name ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@VersionID", versionID);
        Fill(command);
      }
    }
  }
  
}
