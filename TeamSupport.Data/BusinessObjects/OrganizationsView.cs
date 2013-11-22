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

    public void LoadByParentID(int parentID, bool includeCustomFields, string orderBy = "Name", int? limitNumber = null)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string limit = string.Empty;
        if (limitNumber != null)
        {
          limit = "TOP " + limitNumber.ToString();
        }
        string sql = "SELECT " + limit + " * FROM OrganizationsView WHERE (ParentID = @ParentID) ORDER BY " + orderBy;
        if (includeCustomFields) sql = InjectCustomFields(sql, "OrganizationID", ReferenceType.Organizations);
        command.CommandText = sql;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ParentID", parentID);
        Fill(command);
      }
    }

    public void LoadByTicketID(int ticketID, string orderBy = "Name")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT ov.* FROM OrganizationsView ov LEFT JOIN OrganizationTickets ot ON ot.OrganizationID = ov.OrganizationID WHERE ot.TicketID = @TicketID ORDER BY " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadByProductID(int productID, string orderBy = "Name")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = 
          @"
            SELECT 
              DISTINCT
              ov.* 
            FROM 
              OrganizationsView ov 
              JOIN OrganizationProducts op 
                ON op.OrganizationID = ov.OrganizationID 
            WHERE 
              op.ProductID = @ProductID 
            ORDER BY 
          " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        Fill(command);
      }
    }
    public void LoadByVersionID(int versionID, string orderBy = "Name")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT ov.* FROM OrganizationsView ov LEFT JOIN OrganizationProducts op ON op.OrganizationID = ov.OrganizationID WHERE op.ProductVersionID = @VersionID ORDER BY " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@VersionID", versionID);
        Fill(command);
      }
    }

    public void LoadForIndexing(int organizationID, int max, bool isRebuilding)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string text = @"
        SELECT TOP {0} OrganizationID
        FROM OrganizationsView ov WITH(NOLOCK)
        WHERE ov.NeedsIndexing = 1
        AND ov.ParentID = @OrganizationID
        ORDER BY DateModified DESC";

        if (isRebuilding)
        {
          text = @"
        SELECT OrganizationID
        FROM OrganizationsView ov WITH(NOLOCK)
        WHERE ov.ParentID = @OrganizationID
        ORDER BY DateModified DESC";
        }

        command.CommandText = string.Format(text, max.ToString());
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }
  }
  
}
