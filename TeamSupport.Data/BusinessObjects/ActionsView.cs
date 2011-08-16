using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ActionsViewItem
  {
    public TicketsViewItem GetTicket()
    {
      return TicketsView.GetTicketsViewItem(Collection.LoginUser, TicketID);
    }

    public string DisplayName
    {
      get
      {
        string title = "";

        switch (SystemActionTypeID)
        {
          case SystemActionType.Description: title = "Description"; break;
          case SystemActionType.Resolution: title = "Resolution"; break;
          case SystemActionType.Email: title = "Email: " + Name; break;
          case SystemActionType.PingUpdate: title = "Ping Updated"; break;
          case SystemActionType.Chat: title = "Chat"; break;
          default:
            title = ActionType == "" ? "[No Action Type]" : ActionType;
            if (!string.IsNullOrEmpty(Name)) title += ": " + Name;
            break;
        }
        return title;
      }

    }

  }
  
  public partial class ActionsView
  {
    public void LoadByTicketID(int ticketID, bool onlyVisibleOnPortal)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandType = CommandType.Text;
        if (onlyVisibleOnPortal)
          command.CommandText = "SELECT * FROM ActionsView WHERE TicketID = @TicketID AND IsVisibleOnPortal=1 ORDER BY DateCreated DESC";
        else
          command.CommandText = "SELECT * FROM ActionsView WHERE TicketID = @TicketID ORDER BY DateCreated DESC";

        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    
    }

    public void LoadKBByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandType = CommandType.Text;
        command.CommandText = "SELECT * FROM ActionsView WHERE TicketID = @TicketID AND IsKnowledgeBase=1 ORDER BY DateCreated ASC";
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }

    }


    public void LoadLatestByTicket(int ticketID, bool onlyPublic)
    {
      using (SqlCommand command = new SqlCommand())
      {
        if (onlyPublic)
          command.CommandText = "SELECT TOP 1 a.* FROM ActionsView a WHERE a.TicketID = @TicketID AND IsVisibleOnPortal = 1 ORDER BY a.DateCreated DESC";
        else
          command.CommandText = "SELECT TOP 1 a.* FROM ActionsView a WHERE a.TicketID = @TicketID ORDER BY a.DateCreated DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadByTicketID(int ticketID)
    {
      LoadByTicketID(ticketID, false);
    }
  }
  
}
