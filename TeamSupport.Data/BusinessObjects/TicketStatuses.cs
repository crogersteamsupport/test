using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TicketStatus 
  {
    public void FillWorkflow()
    { 
      
    }

  }

  public partial class TicketStatuses 
  {
    partial void AfterRowInsert(TicketStatus ticketStatus)
    {
      TicketStatuses statuses = new TicketStatuses(LoginUser);
      statuses.LoadByTicketTypeID(ticketStatus.TicketTypeID);
      TicketNextStatuses ticketNextStatuses = new TicketNextStatuses(LoginUser);
      
      for (int i = 0; i < statuses.Count; i++)
      {
        if (statuses[i].TicketStatusID != ticketStatus.TicketStatusID)
        {
          TicketNextStatus ticketNextStatus = ticketNextStatuses.AddNewTicketNextStatus();
          ticketNextStatus.CurrentStatusID = ticketStatus.TicketStatusID;
          ticketNextStatus.NextStatusID = statuses[i].TicketStatusID;
          ticketNextStatus.Position = i;
        }
      }

      foreach (TicketStatus status in statuses)
      {
        if (status.TicketStatusID != ticketStatus.TicketStatusID)
        {
          TicketNextStatus ticketNextStatus = ticketNextStatuses.AddNewTicketNextStatus();
          ticketNextStatus.CurrentStatusID = status.TicketStatusID;
          ticketNextStatus.NextStatusID = ticketStatus.TicketStatusID;
          ticketNextStatus.Position = TicketNextStatuses.GetMaxPosition(LoginUser, status.TicketStatusID) + 1;
        }
      }
      ticketNextStatuses.Save();



      ticketStatus.FillWorkflow();
    }


    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketStatuses WHERE OrganizationID = @OrganizationID  ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadTopOne(int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP 1 * FROM TicketStatuses WHERE TicketTypeID = @TicketTypeID  ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketTypeID", ticketTypeID);
        Fill(command);
      }
    }

    public static TicketStatus GetTop(LoginUser loginUser, int ticketTypeID)
    {
      TicketStatuses items = new TicketStatuses(loginUser);
      items.LoadTopOne(ticketTypeID);
      if (items.IsEmpty) return null; else return items[0];
    }

    public void LoadByTicketTypeID(int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketStatuses WHERE TicketTypeID = @TicketTypeID  ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketTypeID", ticketTypeID);
        Fill(command);
      }
    }

    public void LoadNotNextStatuses(int currentStatusID)
    {
      TicketStatuses statuses = new TicketStatuses(LoginUser);
      statuses.LoadByTicketStatusID(currentStatusID);
      if (statuses.IsEmpty) return;
      TicketStatus status = statuses[0];

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketStatuses ts WHERE (ts.TicketStatusID not in (SELECT tns.NextStatusID FROM TicketNextStatuses tns WHERE (tns.CurrentStatusID = @CurrentStatusID))) AND (ts.TicketTypeID = @TicketTypeID) AND (ts.TicketStatusID <> @CurrentStatusID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CurrentStatusID", currentStatusID);
        command.Parameters.AddWithValue("TicketTypeID", status.TicketTypeID);
        Fill(command, "TicketStatuses,TicketNextStatuses");
      }
    }

    public void LoadByPosition(int ticketTypeID, int position)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketStatuses WHERE (TicketTypeID = @TicketTypeID) AND (Position = @Position)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketTypeID", ticketTypeID);
        command.Parameters.AddWithValue("Position", position);
        Fill(command);
      }

    }

    public void LoadAllPositions(int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketStatuses WHERE (TicketTypeID = @TicketTypeID) ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketTypeID", ticketTypeID);
        Fill(command);
      }
    }

    public void ValidatePositions(int ticketTypeID)
    {
      TicketStatuses ticketStatuses = new TicketStatuses(LoginUser);
      ticketStatuses.LoadAllPositions(ticketTypeID);
      int i = 0;
      foreach (TicketStatus ticketStatus in ticketStatuses)
      {
        ticketStatus.Position = i;
        i++;
      }
      ticketStatuses.Save();
    }    

    public TicketStatus FindByName(string name, int ticketTypeID)
    {
      foreach (TicketStatus ticketStatus in this)
      {
        if (ticketStatus.Name.ToUpper() == name.ToUpper() && ticketStatus.TicketTypeID == ticketTypeID)
        {
          return ticketStatus;
        }
      }
      return null;
    }

    public void MovePositionUp(int ticketStatusID)
    {
      ValidatePositions(TicketStatuses.GetTicketStatus(LoginUser, ticketStatusID).TicketTypeID);

      TicketStatuses types1 = new TicketStatuses(LoginUser);
      types1.LoadByTicketStatusID(ticketStatusID);
      if (types1.IsEmpty || types1[0].Position < 1) return;

      TicketStatuses types2 = new TicketStatuses(LoginUser);
      types2.LoadByPosition(types1[0].TicketTypeID, types1[0].Position - 1);
      if (!types2.IsEmpty)
      {
        types2[0].Position = types2[0].Position + 1;
        types2.Save();
      }

      types1[0].Position = types1[0].Position - 1;
      types1.Save();
      ValidatePositions(types1[0].TicketTypeID);
    }

    public void MovePositionDown(int ticketStatusID)
    {
      ValidatePositions(TicketStatuses.GetTicketStatus(LoginUser, ticketStatusID).TicketTypeID);

      TicketStatuses types1 = new TicketStatuses(LoginUser);
      types1.LoadByTicketStatusID(ticketStatusID);
      if (types1.IsEmpty || types1[0].Position >= GetMaxPosition(types1[0].TicketTypeID)) return;

      TicketStatuses types2 = new TicketStatuses(LoginUser);
      types2.LoadByPosition(types1[0].TicketTypeID, types1[0].Position + 1);
      if (!types2.IsEmpty)
      {
        types2[0].Position = types2[0].Position - 1;
        types2.Save();
      }

      types1[0].Position = types1[0].Position + 1;
      types1.Save();
      ValidatePositions(types1[0].TicketTypeID);
    }

    public virtual int GetMaxPosition(int ticketTypeID)
    {
      int position = -1;
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT MAX(Position) FROM TicketStatuses WHERE TicketTypeID = @TicketTypeID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketTypeID", ticketTypeID);
        object o = ExecuteScalar(command);
        if (o == DBNull.Value) return -1;
        position = (int)o;
      }

      return position;
    }

    partial void BeforeDBDelete(int ticketStatusID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM TicketNextStatuses WHERE (CurrentStatusID = @TicketStatusID) OR (NextStatusID = @TicketStatusID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketStatusID", ticketStatusID);
        ExecuteNonQuery(command, "TicketNextStatuses");
      }

    }

    public static void DeleteByTicketTypeID(LoginUser loginUser, int ticketTypeID)
    {
      TicketStatuses statuses = new TicketStatuses(loginUser);
      statuses.LoadByTicketTypeID(ticketTypeID);
      foreach (TicketStatus status in statuses)
      {
        statuses.DeleteFromDB(status.TicketStatusID);
      }
    }



  }
}
