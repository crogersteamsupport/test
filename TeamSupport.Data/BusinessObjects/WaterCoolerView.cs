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
      public void LoadTop10Threads(int pageID, int itemID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = "WCLoadTop10";
              command.CommandType = CommandType.StoredProcedure;
              command.Parameters.AddWithValue("UserID", LoginUser.UserID);
              command.Parameters.AddWithValue("OrgID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("PageID", pageID);
              command.Parameters.AddWithValue("AttID", itemID);
              Fill(command);
          }

       
      }

      public void CheckMessage(int pageID, int itemID, int messageID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = "WCCheckMessage";
              command.CommandType = CommandType.StoredProcedure;
              command.Parameters.AddWithValue("UserID", LoginUser.UserID);
              command.Parameters.AddWithValue("OrgID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("MessageID", messageID);
              Fill(command);
          }
      }

      public void LoadMoreThreads(int pageID, int itemID, int msgcount)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = "WCLoadMoreThreads";
              command.CommandType = CommandType.StoredProcedure;
              command.Parameters.AddWithValue("UserID", LoginUser.UserID);
              command.Parameters.AddWithValue("OrgID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("@Start", msgcount + 1);
              command.Parameters.AddWithValue("@End", msgcount + 5);
              command.Parameters.AddWithValue("PageID", pageID);
              command.Parameters.AddWithValue("AttID", itemID);
              Fill(command);
          }
      }

      public int GetTicketWaterCoolerCount(int ticketID)
      {
          Object returnValue;

          using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
          {
              connection.Open();
              SqlCommand command = connection.CreateCommand();
              command.CommandText = "WCTicketCount";
              command.CommandType = CommandType.StoredProcedure;
              command.Parameters.AddWithValue("UserID", LoginUser.UserID);
              command.Parameters.AddWithValue("OrgID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("PageID", 0);
              command.Parameters.AddWithValue("AttID", ticketID);

              returnValue = command.ExecuteScalar();

              connection.Close();
              return (int)returnValue;

          }

      }

      public void LoadMessage(int messageID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              // This query isn't right just a sample to pull the top 25.
              command.CommandText = @"SELECT * FROM NewWaterCoolerView WHERE OrganizationID = @OrganizationID AND MessageID = @MsgID";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("@MsgID", messageID);
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
              command.CommandText = @"SELECT * FROM WaterCoolerMsg WHERE MessageParent = @ReplyTo AND isdeleted=0 AND OrganizationID = @OrganizationID ORDER BY TimeStamp ASC";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("@ReplyTo", messageID);
              Fill(command);
          }
      }

  }
  
}
