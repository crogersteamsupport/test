using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CalendarEvent
  {
  }
  
  public partial class CalendarEvents
  {
      public void LoadbyMonth(DateTime date, int orgID, string Type, string ID)
      {
          string additional = "";
          if (Type != "-1")
          {
              additional = " AND CalendarID in (select CalendarID from CalendarRef where RefID=@id and RefType=@type)";
          }
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = "SELECT * from CalendarEvents WHERE (Month(StartDate) = @month) AND (Year(StartDate) = @year) AND (OrganizationID = @orgID)" + additional;
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@month", date.Month);
              command.Parameters.AddWithValue("@year", date.Year);
              command.Parameters.AddWithValue("@orgID", orgID);
              command.Parameters.AddWithValue("@type", Type);
              command.Parameters.AddWithValue("@id", ID);
              Fill(command);
          }
      }
  }
  
}
