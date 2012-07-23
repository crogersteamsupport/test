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
              // This query isn't right just a sample to pull the top 25.
              command.CommandText = @"select top 10 MessageID, UserID, OrganizationID, TimeStamp, Message, MessageParent, IsDeleted, LastModified
                                    from WatercoolerMsg where MessageID in (select MessageID from NewWaterCoolerView where  organizationid=@OrganizationID and messageparent = -1 and isdeleted=0 ";
              switch (pageID)
              {
                  case -1://main page
                      command.CommandText += string.Format(@"and(
                                    -- Groups
                                    (RefType is not null and RefType={0} and (AttachmentID in (select groupid from groupusers where userid = {1})) and isdeleted = 0 )
                                    -- User Created
                                    or (UserID = {2})
                                    -- User Reference
                                    or ((attachmentid = {3}) and reftype={4} )
                                    -- No Attachments
                                    or (ISNULL(Reftype,-1) = -1)
                                    -- Other Attachments
                                    or (RefType in (0,1,2))) group by MessageID)
                                    order by LastModified desc", WaterCoolerAttachmentType.Group.GetHashCode(), LoginUser.UserID, LoginUser.UserID, LoginUser.UserID, WaterCoolerAttachmentType.User.GetHashCode());
                      break;
                  case 0:  //ticket page
                      command.CommandText += string.Format(@"and((RefType is not null and RefType={0} and AttachmentID = {1})) group by MessageID)
                                    order by LastModified desc", WaterCoolerAttachmentType.Ticket.GetHashCode(), itemID);
                      break;
                  case 1: //product page
                      command.CommandText += string.Format(@"and((RefType is not null and RefType={0} and AttachmentID = {1})) group by MessageID)
                                    order by LastModified desc", WaterCoolerAttachmentType.Product.GetHashCode(), itemID);
                      break;
                  case 2: //company page
                      command.CommandText += string.Format(@"and((RefType is not null and RefType={0} and AttachmentID = {1})) group by MessageID)
                                    order by LastModified desc", WaterCoolerAttachmentType.Company.GetHashCode(), itemID);
                      break;
                  default:
                      command.CommandText += @"group by MessageID)
                                    order by LastModified desc";
                      break;
              }


              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
              Fill(command);
          }
      }

      public void CheckMessage(int pageID, int itemID, int messageID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              // This query isn't right just a sample to pull the top 25.
              command.CommandText = @"select messageID from watercoolermsg where @MessageID in (select MessageID
                                    from WatercoolerMsg where MessageID in (select MessageID from NewWaterCoolerView where  organizationid=@OrganizationID and messageparent = -1 and isdeleted=0 ";
              switch (pageID)
              {
                  case -1://main page
                      command.CommandText += string.Format(@"and(
                                    -- Groups
                                    (RefType is not null and RefType={0} and (AttachmentID in (select groupid from groupusers where userid = {1})) and isdeleted = 0 )
                                    -- User Created
                                    or (UserID = {2})
                                    -- User Reference
                                    or ((attachmentid = {3}) and reftype={4} )
                                    -- No Attachments
                                    or (ISNULL(Reftype,-1) = -1)
                                    -- Other Attachments
                                    or (RefType in (0,1,2))) group by MessageID))
                                    order by LastModified desc", WaterCoolerAttachmentType.Group.GetHashCode(), LoginUser.UserID, LoginUser.UserID, LoginUser.UserID, WaterCoolerAttachmentType.User.GetHashCode());
                      break;
                  case 1:  //ticket page
                      command.CommandText += string.Format(@"and((RefType is not null and RefType={0) and AttachmentID = {1})) group by MessageID))
                                    order by LastModified desc", WaterCoolerAttachmentType.Ticket.GetHashCode(), itemID);
                      break;
                  case 2: //product page
                      command.CommandText += string.Format(@"and((RefType is not null and RefType={0) and AttachmentID = {1})) group by MessageID))
                                    order by LastModified desc", WaterCoolerAttachmentType.Product.GetHashCode(), itemID);
                      break;
                  case 3: //company page
                      command.CommandText += string.Format(@"and((RefType is not null and RefType={0) and AttachmentID = {1})) group by MessageID))
                                    order by LastModified desc", WaterCoolerAttachmentType.Company.GetHashCode(), itemID);
                      break;
                  default:
                      command.CommandText += @"group by MessageID))
                                    order by LastModified desc";
                      break;
              }


              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("@MessageID", messageID);
              Fill(command);
          }
      }

      public void LoadMoreThreads(int pageID, int itemID, int msgcount)
      {
          using (SqlCommand command = new SqlCommand())
          {
              //command.CommandText = @"WITH threads AS(SELECT   *, ROW_NUMBER() OVER (ORDER BY TimeStamp desc) AS rowid FROM WaterCoolerView)select rowid, MessageID, UserID, OrganizationID, TimeStamp, Message, MessageParent, IsDeleted, Avatar, UserName from threads WHERE OrganizationID = @OrganizationID AND MessageParent = -1 and rowid BETWEEN @start AND @end ORDER BY TimeStamp DESC";
              command.CommandText = @"WITH threads AS(SELECT wcv.MessageID, wcv.UserID, OrganizationID, TimeStamp, LastModified, Message, MessageParent, RefType, AttachmentID, IsDeleted, ROW_NUMBER() OVER (ORDER BY TimeStamp desc) AS rowid FROM NewWaterCoolerView as wcv where OrganizationID = @OrganizationID AND MessageParent = -1 and isdeleted=0 and (ISNULL(Reftype,-1) = -1))
                                    select rowid, threads.MessageID, threads.UserID, OrganizationID, TimeStamp, LastModified, Message, MessageParent, RefType, AttachmentID, IsDeleted from threads
                                    WHERE OrganizationID = @OrganizationID AND MessageParent = -1 and isdeleted=0 and (ISNULL(Reftype,-1) = -1) and rowid BETWEEN @start AND @end ORDER BY LastModified DESC";


              command.CommandText = @"WITH threads AS(select  ROW_NUMBER() OVER (ORDER BY LastModified desc) AS rowid, MessageID, UserID, OrganizationID, TimeStamp, Message, MessageParent, IsDeleted, LastModified
                                    from WatercoolerMsg where MessageID in (select MessageID from NewWaterCoolerView where  organizationid=@OrganizationID and messageparent = -1 and isdeleted=0 ";
              switch (pageID)
              {
                  case -1://main page
                      command.CommandText += string.Format(@"and(
                                    -- Groups
                                    (RefType is not null and RefType={0} and (AttachmentID in (select groupid from groupusers where userid = {1})) and isdeleted = 0 )
                                    -- User Created
                                    or (UserID = {2})
                                    -- User Reference
                                    or ((attachmentid = {3}) and reftype={4} )
                                    -- No Attachments
                                    or (ISNULL(Reftype,-1) = -1)
                                    -- Other Attachments
                                    or (RefType in (0,1,2))) group by MessageID))
                                    select * from threads where rowid between @start and @end
                                    order by LastModified desc", WaterCoolerAttachmentType.Group.GetHashCode(), LoginUser.UserID, LoginUser.UserID, LoginUser.UserID, WaterCoolerAttachmentType.User.GetHashCode());
                      break;
                  case 1:  //ticket page
                      command.CommandText += string.Format(@"and((RefType is not null and RefType={0) and AttachmentID = {1})) group by MessageID))
                                    select * from threads where rowid between @start and @end
                                    order by LastModified desc", WaterCoolerAttachmentType.Ticket.GetHashCode(), itemID);
                      break;
                  case 2: //product page
                      command.CommandText += string.Format(@"and((RefType is not null and RefType={0) and AttachmentID = {1})) group by MessageID))
                                    select * from threads where rowid between @start and @end
                                    order by LastModified desc", WaterCoolerAttachmentType.Product.GetHashCode(), itemID);
                      break;
                  case 3: //company page
                      command.CommandText += string.Format(@"and((RefType is not null and RefType={0) and AttachmentID = {1})) group by MessageID))
                                    select * from threads where rowid between @start and @end
                                    order by LastModified desc", WaterCoolerAttachmentType.Company.GetHashCode(), itemID);
                      break;
                  default:
                      command.CommandText += @"group by MessageID))
                                    select * from threads where rowid between @start and @end
                                    order by LastModified desc";
                      break;
              }              
              
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("@start", msgcount + 1);
              command.Parameters.AddWithValue("@end", msgcount + 5);
              Fill(command);
          }
      }


      public void LoadUpdatedThreads(int msgcount, int pausetime)
      {
          if (msgcount == 0)
              msgcount += 10;
          if (pausetime < 10)
              pausetime = 10;
          using (SqlCommand command = new SqlCommand())
          {
              // This query isn't right just a sample to pull the top 25.
              //command.CommandText = @"WITH threads AS(SELECT   *, ROW_NUMBER() OVER (ORDER BY TimeStamp desc) AS rowid FROM WaterCoolerView)select rowid, MessageID, UserID, OrganizationID, TimeStamp, Message, MessageParent, IsDeleted, Avatar, UserName from threads WHERE OrganizationID = @OrganizationID AND MessageParent = -1 and rowid BETWEEN @start AND @end ORDER BY TimeStamp DESC";
              command.CommandText = @"WITH threads AS(
                                    SELECT wcv.MessageID, wcv.UserID, OrganizationID, TimeStamp, Message, MessageParent, IsDeleted, Avatar, UserName, lastmodified, LastModifiedUserID, ROW_NUMBER() OVER (ORDER BY TimeStamp desc) AS rowid FROM NewWaterCoolerView as wcv left join watercoolergroupatt as wcg on wcv.messageid = wcg.messageid
                                    WHERE organizationid = @OrganizationID and MessageParent = -1 and DATEDIFF(second,wcv.LastModified,GETUTCDATE()) <= @timedelay and isdeleted=0 and isnull(wcg.groupid,0) = 0 or isnull(wcg.groupid,0) in (select groupid from groupusers where userid = @UserID))
                                    select rowid, threads.MessageID, threads.UserID, OrganizationID, TimeStamp, Message, MessageParent, IsDeleted, Avatar, UserName from threads where DATEDIFF(second,threads.LastModified,GETUTCDATE()) <= @timedelay
                                    ORDER BY LastModified asc";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
              command.Parameters.AddWithValue("@start", 0);
              command.Parameters.AddWithValue("@end", msgcount);
              command.Parameters.AddWithValue("@timedelay", pausetime + 2);

              Fill(command);
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
