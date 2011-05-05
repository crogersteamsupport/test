using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Web.Services;

public partial class Frames_TicketQueue : System.Web.UI.Page
{
  public class QueueItem
  {
    public QueueItem(int id, string name, string user, string status, string severity, int userID, bool isClosed)
    {
      ID = id;
      Name = name;
      User = user;
      Severity = severity;
      Status = status;
      UserID = userID;
      IsClosed = isClosed;
    }

    public int ID { get; set; }
    public string Name { get; set; }
    public string User { get; set; }
    public string Status { get; set; }
    public string Severity { get; set; }
    public int UserID { get; set; }
    public bool IsClosed { get; set; }
  }

  [WebMethod(true)]
  public static QueueItem[] GetQueueItems(int userID)
  {
    TicketQueue queue = new TicketQueue(UserSession.LoginUser);
    queue.LoadByUser(userID);
    List<QueueItem> result = new List<QueueItem>();
    foreach (TicketQueueItem item in queue)
    {
      int assignedID = item.Row["AssignedUserID"] == DBNull.Value ? -1 : (int)item.Row["AssignedUserID"];
      result.Add(new QueueItem(item.TicketQueueID, item.Row["TicketNumber"].ToString() + ": " + item.Row["Name"].ToString(), item.Row["UserName"].ToString(), item.Row["Status"].ToString(), item.Row["Severity"].ToString(), assignedID, (bool)item.Row["IsClosed"]));
      //result.Add(new QueueItem(item.TicketQueueID, item.Position.ToString() + " : " + item.TicketQueueID.ToString(), item.Row["UserName"].ToString(), item.Row["Severity"].ToString()));
    }
    return result.ToArray();
  }

  [WebMethod(true)]
  public static void Enqueue(int ticketID, int userID)
  {
    if (Tickets.GetTicket(UserSession.LoginUser, ticketID).OrganizationID != UserSession.LoginUser.OrganizationID) return;
    if (Users.GetUser(UserSession.LoginUser, userID).OrganizationID != UserSession.LoginUser.OrganizationID) return;
    TicketQueue.Enqueue(UserSession.LoginUser, ticketID, userID);
  }

  [WebMethod(true)]
  public static void Dequeue(int id)
  {
    TicketQueueItem item = TicketQueue.GetTicketQueueItem(UserSession.LoginUser, id);

    if (UserSession.CurrentUser.UserID != item.UserID && !UserSession.CurrentUser.IsSystemAdmin) return;
    item.Delete();
    item.Collection.Save();
  }

  [WebMethod(true)]
  public static void SaveItems(int userID, int[] ids)
  {
    if (UserSession.CurrentUser.UserID != userID && !UserSession.CurrentUser.IsSystemAdmin) return;
    TicketQueue.RepositionTickets(UserSession.LoginUser, userID, ids);
  }

  [WebMethod(true)]
  public static int GetTicketID(int itemID)
  {
    return TicketQueue.GetTicketQueueItem(UserSession.LoginUser, itemID).TicketID;
  
  }

  [WebMethod(true)]
  public static int GetTicketNumber(int itemID)
  {
    return Tickets.GetTicket(UserSession.LoginUser, TicketQueue.GetTicketQueueItem(UserSession.LoginUser, itemID).TicketID).TicketNumber;

  }


}
