using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using System.IO;
using System.Data.SqlClient;
using System.Net.Mail;
using LumenWorks.Framework.IO.Csv;
using System.Data;
using System.Text.RegularExpressions;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  public class ImportProcessor : ServiceThread
  {
    const int BULK_LIMIT = 1000;

    private int _organizationID;
    private LoginUser _importUser;
    private CsvReader _csv;
    private SortedList<string, string> _map;

    public override void Run()
    {
      try
      {
        Imports imports = new Imports(LoginUser);
        imports.LoadWaiting();
        if (!imports.IsEmpty)
        {
          Import import = imports[0];
          import.IsRunning = true;
          imports.Save();
          ProcessImport(import);
          import.IsDone = true;
          imports.Save();
        }
        UpdateHealth();
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "ImportProcessor"); 
      }
    }

    private void ProcessImport(Import import)
    {
      Logs.WriteLine();
      Logs.WriteEvent("***********************************************************************************");
      Logs.WriteEvent("Processing Import  ImportID: " + import.ImportID.ToString());
      Logs.WriteData(import.Row);
      Logs.WriteLine();
      Logs.WriteEvent("***********************************************************************************");
      _importUser = new Data.LoginUser(LoginUser.ConnectionString, -2, import.OrganizationID, null);

      string path = AttachmentPath.GetPath(_importUser, import.OrganizationID, AttachmentPath.Folder.Imports);
      string csvFile = Path.Combine(path, import.FileName);
      _organizationID = import.OrganizationID;

      _map = new SortedList<string, string>();
      // load maps
      using (_csv = new CsvReader(new StreamReader(csvFile), true))
      {
        switch (import.RefType)
        {
          case ReferenceType.Actions:
            ImportActions(import);
            break;
          default:
            Logs.WriteEvent("ERROR: Unknown Reference Type");
            break;
        }
      }
    }

    private void ImportActions(Import import)
    {
      SortedList<string, int> userList = GetUserAndContactList();
      SortedList<string, int> ticketList = GetTicketList();

      ActionTypes actionTypes = new ActionTypes(_importUser);
      actionTypes.LoadAllPositions(_organizationID);

      Actions actions = new Actions(_importUser);
      int count = 0;

      while (_csv.ReadNextRecord())
      {
        int ticketID;
        int creatorID = -1;

        if (!ticketList.TryGetValue(ReadString("TicketNumber").ToUpper(), out ticketID))
        {
          //_log.AppendError(row, "Action skipped due to missing ticket");
          continue;
        }

        string actionType = ReadString("ActionType");
        if (string.IsNullOrWhiteSpace(actionType))
        {
          //_log.AppendError(row, "Action skipped due to missing action type");
          continue;
        }

        userList.TryGetValue(ReadString("CreatorID").ToUpper(), out creatorID);

        string desc = ConvertHtmlLineBreaks(ReadString("Description"));

        TeamSupport.Data.Action action = actions.AddNewAction();

        action.SystemActionTypeID = GetSystemActionTypeID(actionType);
        action.ActionTypeID = GetActionTypeID(actionTypes, actionType);
        action.CreatorID = (int)creatorID;
        action.DateCreated = (DateTime)ReadDate("DateCreated", DateTime.UtcNow);
        action.DateModified = DateTime.UtcNow;
        action.DateStarted = ReadDateNull("DateStarted");
        action.Description = desc;
        action.ActionSource = "Import";
        action.IsVisibleOnPortal = ReadBool("VisibleOnPortal");
        action.ModifierID = -2;
        action.Name = "";
        action.TicketID = ticketID;
        action.ImportID = import.ImportGUID.ToString();
        action.TimeSpent = ReadIntNull("TimeSpent");
        //if (action.IsVisibleOnPortal && !ticket.IsVisibleOnPortal) ticket.IsVisibleOnPortal = true;

        action.Pinned = ReadBool("IsPinned");

        if (++count % BULK_LIMIT == 0)
        {
          EmailPosts.DeleteImportEmails(_importUser);
          actions.BulkSave();
          actions = new Actions(_importUser);
        }
      }
      EmailPosts.DeleteImportEmails(_importUser);
      actions.BulkSave();
      // _log.AppendMessage(count.ToString() + " Actions Imported.");
    }

    private string GetMappedName(string field)
    {
      //return _map[field];
      return field;
    }

    private string GetMappedValue(string field)
    {
      return _csv[GetMappedName(field)];
    }
    
    private DateTime ReadDate(string field, DateTime defaultValue)
    {
      string value = GetMappedValue(field);
      DateTime result = defaultValue;
      DateTime.TryParse(value, out result);
      return result;
    }

    private DateTime? ReadDateNull(string field)
    {
      string value = GetMappedValue(field);
      DateTime result;
      if (!DateTime.TryParse(value, out result))
      {
        return null;
      }
      return result;
    }

    private bool ReadBool(string field)
    {
      string value = GetMappedValue(field);
      value = value.ToLower();
      return value.IndexOf('t') > -1 || value.IndexOf('y') > -1;
    }

    private int ReadInt(string field, int defaultValue = 0)
    {
      string value = GetMappedValue(field);
      int result = defaultValue;
      int.TryParse(value, out result);
      return result;
    }

    private int? ReadIntNull(string field)
    {
      string value = GetMappedValue(field);
      int result;
      if (!int.TryParse(value, out result))
      {
        return null;
      }
      return result;
    }

    private string ReadString(string field)
    {
      return GetMappedValue(field).Trim();
    }

    private SystemActionType GetSystemActionTypeID(string name)
    {
      SystemActionType result = SystemActionType.Custom;
      switch (name.ToLower().Trim())
      {
        case "description": result = SystemActionType.Description; break;
        case "resolution": result = SystemActionType.Resolution; break;
        case "updaterequest": result = SystemActionType.UpdateRequest; break;
        case "email": result = SystemActionType.Email; break;
        case "reminder": result = SystemActionType.Reminder; break;
        default:
          break;
      }
      return result;
    }

    private int? GetActionTypeID(ActionTypes actionTypes, string name)
    {
      name = name.Trim();
      if (GetSystemActionTypeID(name) != SystemActionType.Custom) return null;

      ActionType actionType = actionTypes.FindByName(name);

      if (actionType == null)
      {
        ActionTypes ats = new ActionTypes(_importUser);
        actionType = ats.AddNewActionType();
        actionType.Name = name;
        actionType.Description = "";
        actionType.IsTimed = true;
        actionType.OrganizationID = _organizationID;
        actionType.Position = actionTypes.GetMaxPosition(_organizationID) + 1;
        ats.Save();
        int? result = actionType.ActionTypeID;
        actionTypes.LoadAllPositions(_organizationID);
        return result;
      }
      else
      {
        return actionType.ActionTypeID;
      }
    }

    private string ConvertHtmlLineBreaks(string text, string lineBreak = "<br />")
    {
      return Regex.Replace(text, @"\r\n?|\n", lineBreak);
    }

    private SortedList<string, int> GetTicketList()
    {
      SqlCommand command = new SqlCommand();
      command.CommandText = @"SELECT TicketNumber, TicketID
                              FROM Tickets
                              WHERE (OrganizationID = @OrganizationID)";
      command.CommandType = CommandType.Text;
      command.Parameters.AddWithValue("@OrganizationID", _organizationID);
      return GetList(command);
    }

    private SortedList<string, int> GetUserList()
    {
      SqlCommand command = new SqlCommand();
      command.CommandText = @"SELECT u.Email, u.UserID
                              FROM Users u 
                              WHERE (u.OrganizationID = @OrganizationID)
                              AND (u.MarkDeleted = 0)";
      command.CommandType = CommandType.Text;
      command.Parameters.AddWithValue("@OrganizationID", _organizationID);
      return GetList(command);
    }

    private SortedList<string, int> GetUserAndContactList()
    {
      SqlCommand command = new SqlCommand();
      command.CommandText = @"SELECT u.Email, u.UserID
                              FROM Users u 
                              LEFT JOIN Organizations o
                              ON o.OrganizationID = u.OrganizationID
                              WHERE (o.OrganizationID = @OrganizationID OR o.ParentID = @OrganizationID)
                              AND (u.MarkDeleted = 0)";
      command.CommandType = CommandType.Text;
      command.Parameters.AddWithValue("@OrganizationID", _organizationID);
      return GetList(command);
    }

    private SortedList<string, int> GetContactList()
    {
      SqlCommand command = new SqlCommand();
      command.CommandText = @"SELECT u.Email, u.UserID
                              FROM Users u 
                              LEFT JOIN Organizations o
                              ON o.OrganizationID = u.OrganizationID
                              WHERE (o.ParentID = @OrganizationID)
                              AND (u.MarkDeleted = 0)";
      command.CommandType = CommandType.Text;
      command.Parameters.AddWithValue("@OrganizationID", _organizationID);
      return GetList(command);
    }

    private SortedList<string, int> GetList(SqlCommand command)
    {
      SortedList<string, int> result = new SortedList<string, int>();
      using (SqlConnection connection = new SqlConnection(_importUser.ConnectionString))
      {
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
        command.Connection = connection;
        command.Transaction = transaction;
        SqlDataReader reader = command.ExecuteReader();
        try
        {
          while (reader.Read())
          {
            result.Add((reader[0] as string).Trim().ToUpper(), reader[1] as int? ?? -1);
          }
        }
        finally
        {
          reader.Close();
        }

        transaction.Commit();
      }
      return result;
    
    }
  }
}
