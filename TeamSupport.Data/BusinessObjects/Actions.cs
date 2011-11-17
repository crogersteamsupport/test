using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class Action 
  {

    public Attachments GetAttachments()
    {
      Attachments attachments = new Attachments(BaseCollection.LoginUser);
      attachments.LoadByActionID(ActionID);
      return attachments;
    }

    public ActionsViewItem GetActionView()
    {
      return ActionsView.GetActionsViewItem(BaseCollection.LoginUser, ActionID);
    }

    public string ActionTypeName 
    {
      get
      {
        if (Row.Table.Columns.Contains("ActionTypeName") && Row["ActionTypeName"] != DBNull.Value)
        {
          return (string)Row["ActionTypeName"];
        }
        else return "";
      }
    
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
            title = ActionTypeName == "" ? "[No Action Type]" : ActionTypeName;
            if (!string.IsNullOrEmpty(Name)) title += ": " + Name;
            break;
        }
        return title;
      }
    
    }

  }

  public partial class Actions
  {

    partial void BeforeRowDelete(int actionID)
    {
      Action action = (Action)Actions.GetAction(LoginUser, actionID);
      string description = "Deleted action '" + action.Name + "' from " + Tickets.GetTicketLink(LoginUser, action.TicketID);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Tickets, action.TicketID, description);
    }

    partial void BeforeRowEdit(Action action)
    {
      string description = "Modified action '" + action.Name + "' on " + Tickets.GetTicketLink(LoginUser, action.TicketID);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, action.TicketID, description);
    }

    partial void AfterRowInsert(Action action)
    {
      string description = "Added action '" + action.Name + "' to " + Tickets.GetTicketLink(LoginUser, action.TicketID);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, action.TicketID, description);
      AddChildActions(action);
    }


    public void AddChildActions(Action action)
    {
      Tickets tickets = new Tickets(LoginUser);
      tickets.LoadChildren(action.TicketID);
      Actions actions = new Actions(LoginUser);
      foreach (Ticket ticket in tickets)
      {
        Action newAction = actions.AddNewAction();
        newAction.ActionTypeID = action.ActionTypeID;
        newAction.SystemActionTypeID = action.SystemActionTypeID;
        newAction.Name = action.Name;
        newAction.ActionSource = action.ActionSource;
        newAction.Description = action.Description;// +"<p>This action was added by the parent ticket.</p>";
        newAction.TimeSpent = action.TimeSpent;
        newAction.Row["DateStarted"] = action.Row["DateStarted"];
        newAction.IsVisibleOnPortal = action.IsVisibleOnPortal;
        newAction.IsKnowledgeBase = action.IsKnowledgeBase;
        newAction.ImportID = action.ImportID;
        newAction.TicketID = ticket.TicketID;
      }

      actions.Save();

    }

    /// <summary>
    /// Loads actions for a ticket and orders them by the latest date created first
    /// </summary>
    /// <param name="ticketID"></param>
    public void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT a.*, u.FirstName + ' ' + u.LastName AS UserName, at.Name AS ActionTypeName FROM Actions a LEFT JOIN Users u ON a.CreatorID = u.UserID LEFT JOIN ActionTypes at ON a.ActionTypeID = at.ActionTypeID WHERE a.TicketID = @TicketID ORDER BY a.DateCreated DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadLatestByTicket(int ticketID, bool onlyPublic)
    {
      using (SqlCommand command = new SqlCommand())
      {
        if (onlyPublic)
          command.CommandText = "SELECT TOP 1 a.* FROM Actions a WHERE a.TicketID = @TicketID AND IsVisibleOnPortal = 1 ORDER BY a.DateCreated DESC";
        else
          command.CommandText = "SELECT TOP 1 a.* FROM Actions a WHERE a.TicketID = @TicketID ORDER BY a.DateCreated DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadByActionIDs(int[] actionIDs)
    {
      if (actionIDs.Length < 1) return;
      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < actionIDs.Length; i++)
      {
        if (i != 0) builder.Append(",");
        builder.Append(actionIDs[i]);
      }
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Actions WHERE ActionID IN ("+builder.ToString()+") ORDER BY DateCreated DESC";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public static Action GetTicketDescription(LoginUser loginUser, int ticketID)
    {
      Actions actions = new Actions(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT a.*, u.FirstName + ' ' + u.LastName AS UserName, at.Name AS ActionTypeName FROM Actions a LEFT JOIN Users u ON a.CreatorID = u.UserID LEFT JOIN ActionTypes at ON a.ActionTypeID = at.ActionTypeID WHERE a.TicketID = @TicketID AND a.SystemActionTypeID = 1 ORDER BY a.DateCreated";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        actions.Fill(command);
      }

      if (actions.IsEmpty)
        return null;
      else
        return actions[0];
    }

    public void LoadAll()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT a.* FROM Actions a left join tickets t on a.ticketid = t.ticketid where organizationid = 1360";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public Action FindByImportID(string importID)
    {
      foreach (Action action in this)
      {
        if (action.ImportID == importID)
        {
          return action;
        }
      }
      return null;
    }   

    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT a.* FROM Actions a LEFT JOIN Tickets t on a.TicketID = t.TicketID WHERE OrganizationID = @OrganizationID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void ReplaceActionType(int oldID, int newID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE Actions SET ActionTypeID = @newID WHERE (ActionTypeID = @oldID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@oldID", oldID);
        command.Parameters.AddWithValue("@newID", newID);
        ExecuteNonQuery(command, "Actions");
      }
    }

    public static Action GetActionByID(LoginUser loginUser, int actionID)
    {
      Actions actions = new Actions(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT a.*, u.FirstName + ' ' + u.LastName AS UserName, at.Name AS ActionTypeName FROM Actions a LEFT JOIN Users u ON a.CreatorID = u.UserID LEFT JOIN ActionTypes at ON a.ActionTypeID = at.ActionTypeID WHERE a.ActionID = @ActionID ORDER BY a.DateCreated DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ActionID", actionID);
        actions.Fill(command);
      }
      if (actions.IsEmpty)
        return null;
      else
        return actions[0];
    }
  }
}
