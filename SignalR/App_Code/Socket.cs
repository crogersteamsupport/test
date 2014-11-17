using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Runtime.Serialization;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;


/// <summary>
/// Summary description for Socket
/// </summary>
public class Socket : Hub
{

    public Socket()
    {
    }

    public override Task OnDisconnected(bool stopCalled)
    {
      try
      {
        var context = GlobalHost.ConnectionManager.GetHubContext<TicketSocket>();

        LoginUser nulluser = new LoginUser(-1, -1);
        Users u = new Users(nulluser);
        u.LoadByChatID(Context.ConnectionId);
        if (!u.IsEmpty)
        {
          u[0].AppChatID = "";
          u[0].AppChatStatus = false;
          u[0].Collection.Save();

          Clients.Group(u[0].OrganizationID.ToString()).disconnect(u[0].UserID);
          Groups.Remove(Context.ConnectionId, u[0].OrganizationID.ToString());
          context.Clients.All.ticketViewingRemove(null, u[0].UserID.ToString());
        }
        Clients.All.serverleave(Context.ConnectionId, DateTime.Now.ToString());

        return base.OnDisconnected(stopCalled);
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Socket.Disconnect");
        return null;
      }
    }

    public override Task OnConnected()
    {
      try
      {
        Clients.All.joined(Context.ConnectionId, DateTime.Now.ToString());
        Groups.Add(Context.ConnectionId, TSAuthentication.OrganizationID.ToString());
        return base.OnConnected();
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Socket.Connect");
        return null;
      }
    }

    //public override Task OnReconnected(IEnumerable<string> groups)
    //{
    //  try
    //  {
    //    //User u = Users.GetUser(TSAuthentication.GetLoginUser(), Convert.ToInt32(Context.ConnectionId));
    //    //u.AppChatID = Context.ConnectionId;
    //    //u.AppChatStatus = true;
    //    //u.Collection.Save();

    //    return null;// Clients.rejoined(Context.ConnectionId, DateTime.Now.ToString());
    //  }
    //  catch (Exception ex)
    //  {
    //    ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Socket.Reconnect");
    //    return null;
    //  }
    //}

    public void Send(string message)
    {
      try
      {
        // Call the addMessage method on all clients
        Clients.All.addMessage(message);
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Socket.Send");
      }
    }

    public void SendChat(string message, int userID, string name)
    {
      try
      {
        User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
        Clients.All.addMessage("message from " + Context.ConnectionId + " to " + u.FirstLastName);
        Clients.Client(u.AppChatID).chatMessage(HtmlToText.ConvertHtml(message), TSAuthentication.GetLoginUser().UserID, name);
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Socket.SendChat");
      }
    }

    public void login(int userID)
    {
      try
      {
        User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
        u.AppChatID = Context.ConnectionId;
        u.AppChatStatus = true;
        u.Collection.Save();

        Groups.Add(Context.ConnectionId, u.OrganizationID.ToString());
        Clients.All.testupdateusers("Update users for  " + u.OrganizationID.ToString());
        Clients.Group(u.OrganizationID.ToString()).updateUsers();
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Socket.login");
      }
    }

    public void NewThread(int messageID, string organizationID)
    {
      try
      {
        WaterCoolerThread thread = new WaterCoolerThread();

        WaterCoolerView wcv = new WaterCoolerView(TSAuthentication.GetLoginUser());
        wcv.LoadByMessageID(messageID);

        WaterCoolerView replies = new WaterCoolerView(TSAuthentication.GetLoginUser());
        replies.LoadReplies(messageID);

        WatercoolerAttachments wcgroups = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
        wcgroups.LoadByType(messageID, WaterCoolerAttachmentType.Group);

        WatercoolerAttachments wctickets = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
        wctickets.LoadByType(messageID, WaterCoolerAttachmentType.Ticket);

        WatercoolerAttachments wcprods = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
        wcprods.LoadByType(messageID, WaterCoolerAttachmentType.Product);

        WatercoolerAttachments wccompany = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
        wccompany.LoadByType(messageID, WaterCoolerAttachmentType.Company);

        WatercoolerAttachments wcuseratt = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
        wcuseratt.LoadByType(messageID, WaterCoolerAttachmentType.User);

        thread.Message = wcv.GetWaterCoolerViewItemProxies()[0];
        thread.Replies = replies.GetWaterCoolerViewItemProxies();
        thread.Groups = wcgroups.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Group);
        thread.Tickets = wctickets.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Ticket);
        thread.Products = wcprods.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Product);
        thread.Company = wccompany.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Company);
        thread.User = wcuseratt.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.User);

        //If this is a new thread
        if (thread.Message.MessageParent == -1)
        {
            Clients.Group(organizationID).addThread(thread);
        }
        else
        {
            Clients.Group(organizationID).addComment(thread);
            int parentThreadID = (int)thread.Message.MessageParent;

            WaterCoolerThread parentThread = new WaterCoolerThread();
            WaterCoolerView parentThreadwcv = new WaterCoolerView(TSAuthentication.GetLoginUser());
            parentThreadwcv.LoadByMessageID(parentThreadID);

            WatercoolerAttachments parentThreadwcgroups = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
            parentThreadwcgroups.LoadByType(parentThreadID, WaterCoolerAttachmentType.Group);

            WatercoolerAttachments parentThreadwctickets = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
            parentThreadwctickets.LoadByType(parentThreadID, WaterCoolerAttachmentType.Ticket);

            WatercoolerAttachments parentThreadwcprods = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
            parentThreadwcprods.LoadByType(parentThreadID, WaterCoolerAttachmentType.Product);

            WatercoolerAttachments parentThreadwccompany = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
            parentThreadwccompany.LoadByType(parentThreadID, WaterCoolerAttachmentType.Company);

            WatercoolerAttachments parentThreadwcuseratt = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
            parentThreadwcuseratt.LoadByType(parentThreadID, WaterCoolerAttachmentType.User);

            parentThread.Message = parentThreadwcv.GetWaterCoolerViewItemProxies()[0];
            parentThread.Groups = parentThreadwcgroups.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Group);
            parentThread.Tickets = parentThreadwctickets.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Ticket);
            parentThread.Products = parentThreadwcprods.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Product);
            parentThread.Company = parentThreadwccompany.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Company);
            parentThread.User = parentThreadwcuseratt.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.User);

            Clients.Group(organizationID).updateattachments(parentThread);
        }
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Socket.NewThread");
      }
    }

    public void Del(int messageID)
    {
      try
      {
        WaterCoolerView wcv = new WaterCoolerView(TSAuthentication.GetLoginUser());
        wcv.LoadByMessageID(messageID);

        Clients.All.deleteMessage(messageID, wcv[0].MessageParent);
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Socket.Del");
      }
    }

    public void AddLike(WatercoolerLikProxy[] likes, int messageID, int messageParentID, string orgID)
    {
      try
      {
        Clients.Group(orgID).updateLikes(likes, messageID, messageParentID);
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Socket.AddLike");
      }
    }

    public void CurrentUsers()
    {
      try
      {
        LoginUser nulluser = new LoginUser(-1, -1);
        Users u = new Users(nulluser);

        Clients.All.userCount(u.GetChatConnections());
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Socket.CurrentUsers");
      }
    }
}


public class TicketSocket : Hub{
    public TicketSocket()
    {
    }

    public override Task OnDisconnected(bool stopCalled)
    {
        try
        {
            LoginUser nulluser = new LoginUser(-1, -1);
            Users u = new Users(nulluser);
            u.LoadByChatID(Context.ConnectionId);
            if (!u.IsEmpty)
            {
                Groups.Remove(Context.ConnectionId, u[0].OrganizationID.ToString());
                Clients.Group(u[0].OrganizationID.ToString()).disconnect(u[0].UserID);
                ticketViewingRemove(null, u[0].UserID.ToString());
            }
            Clients.All.serverleave(Context.ConnectionId, DateTime.Now.ToString());

            return base.OnDisconnected(stopCalled);
        }
        catch (Exception ex)
        {
            ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Socket.Disconnect");
            return null;
        }
    }

    public override Task OnConnected()
    {
        try
        {
            Clients.All.joined(Context.ConnectionId, DateTime.Now.ToString());
            Groups.Add(Context.ConnectionId, TSAuthentication.OrganizationID.ToString());
            return base.OnConnected();
        }
        catch (Exception ex)
        {
            ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Socket.Connect");
            return null;
        }
    }

    public void Send(string message)
    {
        try
        {
            // Call the addMessage method on all clients
            Clients.All.addMessage(message);
        }
        catch (Exception ex)
        {
            ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Socket.Send");
        }
    }

    public void TicketUpdate(string ticketNum, string updateType, string modUser)
    {
        string changeType = "";
        //Change types
        switch (updateType)
        {
            case "removecontact":
                changeType = " removed a contact from the customers.";break;
            case "removecompant":
                changeType = " removed a company from the customers.";break;
            case "addcustomer":
                changeType = " added a customer.";break;
            case "removeasset":
                changeType = " removed an asset.";break;
            case "addasset":
                changeType = " added an asset.";break;
            case "removesubscriber":
                changeType = " removed a subscriber.";break;
            case "addsubscriber":
                changeType = " added a subscriber.";break;
            case "removequeue":
                changeType = " removed a user from the queue.";break;
            case "addqueue":
                changeType = " added a user to the queue.";break;
            case "removetag":
                changeType = " removed a tag."; break;
            case "addtag":
                changeType = " added a tag."; break;
            case "removerelationship":
                changeType = " removed a ticket relationship."; break;
            case "addrelationship":
                changeType = " added a ticket relationship."; break;
            case "removereminder":
                changeType = " removed a reminder."; break;
            case "addreminder":
                changeType = " added a reminder."; break;
            case "addaction":
                changeType = " added a new action.";break;
            case "deleteaction":
                changeType = " deleted an action."; break;
            case "changeticketname":
                changeType = " modified the ticket name."; break;
            case "changeisportal":
                changeType = " changed the portal visibility."; break;
            case "changeiskb":
                changeType = " changed is knowledgebase."; break;
            case "changekbcat":
                changeType = " changed the knowledgebase category."; break;
            case "changetype":
                changeType = " changed the ticket type."; break;
            case "changestatus":
                changeType = " changed the ticket status."; break;
            case "changeseverity":
                changeType = " changed the ticket severity."; break;
            case "changecommunity":
                changeType = " changed the ticket community."; break;
            case "changeassigned":
                changeType = " changed the ticket assignment."; break;
            case "changegroup":
                changeType = " changed the ticket group."; break;
            case "changeproduct":
                changeType = " changed the product."; break;
            case "changereported":
                changeType = " changed the reported version."; break;
            case "changeresolved":
                changeType = " changed the resolved version."; break;
            case "changecustom":
                changeType = " changed a custom property."; break;
            case "merged":
                changeType = " merged these tickets [" + ticketNum + "]";break;
            case "delete":
                changeType = " deleted this ticket."; break;


        }
        string updateString = modUser + changeType;


        //Group(TSAuthentication.OrganizationID.ToString())
        Clients.Group(TSAuthentication.OrganizationID.ToString(),Context.ConnectionId).DisplayTicketUpdate(ticketNum, updateString);
    }

    public void getTicketViewing(string ticketID)
    {
        Clients.Group(TSAuthentication.OrganizationID.ToString(),Context.ConnectionId).getTicketViewing(ticketID);
        ticketViewingAdd(ticketID, TSAuthentication.UserID.ToString());
    }

    public void ticketViewingAdd(string ticketID, string userID)
    {
        Clients.Group(TSAuthentication.OrganizationID.ToString(),Context.ConnectionId).ticketViewingAdd(ticketID, userID);
    }

    public void ticketViewingRemove(string ticketNum, string userID)
    {
        Clients.Group(TSAuthentication.OrganizationID.ToString(),Context.ConnectionId).ticketViewingRemove(ticketNum,userID);
    }

}

[DataContract]
public class WaterCoolerThread
{
    [DataMember]
    public WaterCoolerViewItemProxy Message { get; set; }
    [DataMember]
    public WaterCoolerViewItemProxy[] Replies { get; set; }
    [DataMember]
    public WatercoolerAttachmentProxy[] Tickets { get; set; }
    [DataMember]
    public WatercoolerAttachmentProxy[] Groups { get; set; }
    [DataMember]
    public WatercoolerAttachmentProxy[] Products { get; set; }
    [DataMember]
    public WatercoolerAttachmentProxy[] Company { get; set; }
    [DataMember]
    public WatercoolerAttachmentProxy[] User { get; set; }
}

[DataContract(Namespace = "http://teamsupport.com/")]
public class WatercoolerJsonInfo
{
    public WatercoolerJsonInfo() { }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public List<int> Tickets { get; set; }
    [DataMember]
    public List<int> Groups { get; set; }
    [DataMember]
    public List<int> Products { get; set; }
    [DataMember]
    public List<int> Company { get; set; }
    [DataMember]
    public List<int> User { get; set; }
    [DataMember]
    public int ParentTicketID { get; set; }
    [DataMember]
    public int PageType { get; set; }
    [DataMember]
    public int PageID { get; set; }
}

