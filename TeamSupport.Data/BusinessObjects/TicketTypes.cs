using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TicketType 
  {
  }

  public partial class TicketTypes 
  {

    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketTypes WHERE OrganizationID = @OrganizationID ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByOrganizationID(int organizationID, ProductType productType)
    {
      LoadByOrganizationID(organizationID);
    }
    
   public void LoadByTicketTemplate(int organizationID, int ticketTemplateID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        TicketTemplate template = TicketTemplates.GetTicketTemplate(LoginUser, ticketTemplateID);
        int ticketTypeID = -1;
        if (template != null && template.TicketTypeID != null) ticketTypeID = (int)template.TicketTypeID;

        command.CommandText = "SELECT t.* FROM TicketTypes t WHERE OrganizationID = @OrganizationID AND (NOT EXISTS (SELECT * FROM TicketTemplates tt WHERE t.TicketTypeID = tt.TicketTypeID AND tt.TemplateType = 0) OR t.TicketTypeID = @TicketTypeID) ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        command.Parameters.AddWithValue("TicketTypeID", ticketTypeID);
        Fill(command);
      }
    }

    partial void BeforeDBDelete(int ticketTypeID)
    {
      SlaTriggers.DeleteByTicketTypeID(LoginUser, ticketTypeID);
    }

    public TicketType FindByName(string name)
    {
      foreach (TicketType ticketType in this)
      {
        if (ticketType.Name.ToUpper() == name.ToUpper())
        {
          return ticketType;
        }
      }
      return null;
    }

    public void LoadTopOne(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP 1 * FROM TicketTypes WHERE OrganizationID = @OrganizationID ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }

    public static TicketType GetTop(LoginUser loginUser, int organizationID)
    {
      TicketTypes items = new TicketTypes(loginUser);
      items.LoadTopOne(organizationID);
      if (items.IsEmpty) return null; else return items[0];
    }


    
  }
}
