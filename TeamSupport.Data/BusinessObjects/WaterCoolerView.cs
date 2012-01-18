using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class WaterCoolerViewItem
  {
  }
  
  public partial class WaterCoolerView
  {
    /// <summary>
    /// This loads the top 25 threads in the WC.  The logic is not complete.
    /// It needs a selection based on users and groups
    /// </summary>
    public void LoadTop25Threads()
    {
      using (SqlCommand command = new SqlCommand())
      {
        // This query isn't right just a sample to pull the top 25.
        command.CommandText = @"SELECT TOP 25 * FROM WaterCoolerView WHERE OrganizationID = @OrganizationID AND ReplyTo IS NULL ORDER BY TimeStamp DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        Fill(command);
      }
    }

    /// <summary>
    /// This loads replies to a WC message.
    /// </summary>
    /// <param name="messageID"></param>
    public void LoadReplies(int messageID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT * FROM WaterCoolerView WHERE ReplyTo = @ReplyTo AND OrganizationID = @OrganizationID ORDER BY TimeStamp ASC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        command.Parameters.AddWithValue("@ReplyTo", messageID);
        Fill(command);
      }
    }
  }
  
}
