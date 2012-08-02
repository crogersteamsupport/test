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
using SignalR.Hubs;
using System.Threading.Tasks;
using SignalR;

/// <summary>
/// Summary description for Socket
/// </summary>
public class Socket : Hub, IConnected, IDisconnect
{
	public Socket()
	{
	}

    public Task Disconnect()
    {
        return Clients.leave(Context.ConnectionId, DateTime.Now.ToString());
    }

    public Task Connect()
    {
        return Clients.joined(Context.ConnectionId, DateTime.Now.ToString());
    }

    public Task Reconnect(IEnumerable<string> groups)
    {
        return Clients.rejoined(Context.ConnectionId, DateTime.Now.ToString());
    }

    public void Send(string message)
    {
        // Call the addMessage method on all clients
        Clients.addMessage(message);
    }

    public void NewThread(int messageID)
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
            Clients.addThread(thread);
        }
        else
        {
            Clients.addComment(thread);
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

            Clients.updateattachments(parentThread);
        }
    }

    public void Del(int messageID)
    {
        WaterCoolerView wcv = new WaterCoolerView(TSAuthentication.GetLoginUser());
        wcv.LoadByMessageID(messageID);

        Clients.deleteMessage(messageID, wcv[0].MessageParent);
    }

    public void AddLike(WatercoolerLikProxy[] likes, int messageID, int messageParentID)
    {
        Clients.updateLikes(likes, messageID, messageParentID);
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


