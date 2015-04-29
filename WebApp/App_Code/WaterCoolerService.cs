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
//using SignalR.Hubs;
using Microsoft.AspNet.SignalR.Hubs;

namespace TSWebServices
{
    //public class Chat1 : Hub
    //{
    //    public void Send(string message)
    //    {
    //        // Call the addMessage method on all clients
    //        Clients.addMessage(message);
    //    }

    //    public void NewThread(int messageID)
    //    {
    //        WaterCoolerThread thread = new WaterCoolerThread();

    //        WaterCoolerView wcv = new WaterCoolerView(TSAuthentication.GetLoginUser());
    //        wcv.LoadByMessageID(messageID);

    //        WaterCoolerView replies = new WaterCoolerView(TSAuthentication.GetLoginUser());
    //        replies.LoadReplies(messageID);

    //        WatercoolerAttachments wcgroups = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
    //        wcgroups.LoadByType(messageID, WaterCoolerAttachmentType.Group);

    //        WatercoolerAttachments wctickets = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
    //        wctickets.LoadByType(messageID, WaterCoolerAttachmentType.Ticket);

    //        WatercoolerAttachments wcprods = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
    //        wcprods.LoadByType(messageID, WaterCoolerAttachmentType.Product);

    //        WatercoolerAttachments wccompany = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
    //        wccompany.LoadByType(messageID, WaterCoolerAttachmentType.Company);

    //        WatercoolerAttachments wcuseratt = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
    //        wcuseratt.LoadByType(messageID, WaterCoolerAttachmentType.User);

    //        thread.Message = wcv.GetWaterCoolerViewItemProxies()[0];
    //        thread.Replies = replies.GetWaterCoolerViewItemProxies();
    //        thread.Groups = wcgroups.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Group);
    //        thread.Tickets = wctickets.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Ticket);
    //        thread.Products = wcprods.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Product);
    //        thread.Company = wccompany.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Company);
    //        thread.User = wcuseratt.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.User);

    //        //If this is a new thread
    //        if (thread.Message.MessageParent == -1)
    //        {
    //            Clients.addThread(thread);
    //        }
    //        else
    //        {
    //            Clients.addComment(thread);
    //            int parentThreadID = (int)thread.Message.MessageParent;

    //            WaterCoolerThread parentThread = new WaterCoolerThread();
    //            WaterCoolerView parentThreadwcv = new WaterCoolerView(TSAuthentication.GetLoginUser());
    //            parentThreadwcv.LoadByMessageID(parentThreadID);

    //            WatercoolerAttachments parentThreadwcgroups = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
    //            parentThreadwcgroups.LoadByType(parentThreadID, WaterCoolerAttachmentType.Group);

    //            WatercoolerAttachments parentThreadwctickets = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
    //            parentThreadwctickets.LoadByType(parentThreadID, WaterCoolerAttachmentType.Ticket);

    //            WatercoolerAttachments parentThreadwcprods = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
    //            parentThreadwcprods.LoadByType(parentThreadID, WaterCoolerAttachmentType.Product);

    //            WatercoolerAttachments parentThreadwccompany = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
    //            parentThreadwccompany.LoadByType(parentThreadID, WaterCoolerAttachmentType.Company);

    //            WatercoolerAttachments parentThreadwcuseratt = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
    //            parentThreadwcuseratt.LoadByType(parentThreadID, WaterCoolerAttachmentType.User);

    //            parentThread.Message = parentThreadwcv.GetWaterCoolerViewItemProxies()[0];
    //            parentThread.Groups = parentThreadwcgroups.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Group);
    //            parentThread.Tickets = parentThreadwctickets.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Ticket);
    //            parentThread.Products = parentThreadwcprods.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Product);
    //            parentThread.Company = parentThreadwccompany.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Company);
    //            parentThread.User = parentThreadwcuseratt.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.User);

    //            Clients.updateattachments(parentThread);
    //        }
    //    }

    //    public void Del(int messageID)
    //    {
    //        WaterCoolerView wcv = new WaterCoolerView(TSAuthentication.GetLoginUser());
    //        wcv.LoadByMessageID(messageID);

    //        Clients.deleteMessage(messageID, wcv[0].MessageParent);
    //    }

    //    public void AddLike(WatercoolerLikProxy[] likes, int messageID, int messageParentID)
    //    {
    //        Clients.updateLikes(likes, messageID, messageParentID);
    //    }

    //}

  [ScriptService]
  [WebService(Namespace = "http://teamsupport.com/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class WaterCoolerService : System.Web.Services.WebService
  {

    public WaterCoolerService() { }

    [WebMethod]
    public bool IsValid(int pageID, int itemID, int messageID)
    {
        WaterCoolerView wcv = new WaterCoolerView(TSAuthentication.GetLoginUser());
        wcv.CheckMessage(pageID, itemID, messageID);

        if (wcv.IsEmpty)
            return false;
        else
            return true;
    }

    [WebMethod]
    public WaterCoolerThread[] GetThreads(int pageType, int pageID, int messageID)
    {
        List<WaterCoolerThread> result = new List<WaterCoolerThread>();

        // Main page [group, user]
        // Ticket page
        // Product page
        // Company page

        WaterCoolerView wc = new WaterCoolerView(TSAuthentication.GetLoginUser());

        if (messageID == -1)
            wc.LoadTop10Threads(pageType, pageID);
        else
            wc.SearchLoadByMessageID(pageType, pageID, messageID);


        foreach (WaterCoolerViewItem item in wc)
        {
            WaterCoolerThread thread = new WaterCoolerThread();

            WaterCoolerView replies = new WaterCoolerView(wc.LoginUser);
            replies.LoadReplies(item.MessageID);

            WatercoolerAttachments threadAttachments = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
            threadAttachments.LoadByMessageID(item.MessageID);

            thread.Message = item.GetProxy();
            thread.Replies = replies.GetWaterCoolerViewItemProxies();
            thread.Groups = threadAttachments.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Group);
            thread.Tickets = threadAttachments.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Ticket);
            thread.Products = threadAttachments.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Product);
            thread.Company = threadAttachments.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Company);
            thread.User = threadAttachments.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.User);
            result.Add(thread);
        }

        return result.ToArray();
    }

    [WebMethod]
    public WaterCoolerThread GetMessage(int messageID)
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

        return thread;
    }


    [WebMethod]
    public int NewComment(string data)
    {
        WatercoolerJsonInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<WatercoolerJsonInfo>(data);
        WatercoolerMsgItem wc = (new WatercoolerMsg(TSAuthentication.GetLoginUser())).AddNewWatercoolerMsgItem();
        int parentmsgid = info.ParentTicketID;
        int pageType = info.PageType;
        int pageID = info.PageID;

        wc.UserID = TSAuthentication.UserID;
        wc.OrganizationID = TSAuthentication.OrganizationID;
        wc.TimeStamp = DateTime.UtcNow;
        wc.LastModified = DateTime.UtcNow;
        wc.Message = info.Description;
        wc.MessageParent = parentmsgid;
        
        wc.Collection.Save();

        if (info.ParentTicketID != -1)
        {
            WatercoolerMsg wcm = new WatercoolerMsg(TSAuthentication.GetLoginUser());
            wcm.LoadByMessageID(parentmsgid);

            wcm[0].LastModified = DateTime.UtcNow;
            wcm[0].Collection.Save();
        }

        if (wc.MessageParent == -1 && info.PageType == 0)
            AddAttachment((int)wc.MessageID, info.PageID, WaterCoolerAttachmentType.Ticket);
        foreach (int ticketID in info.Tickets)
        {
            AddAttachment(wc.MessageID, ticketID, WaterCoolerAttachmentType.Ticket);
            if (wc.MessageParent != -1)
                AddAttachment((int)wc.MessageParent, ticketID, WaterCoolerAttachmentType.Ticket);
        }

        if (wc.MessageParent == -1 && info.PageType == 1)
            AddAttachment((int)wc.MessageID, info.PageID, WaterCoolerAttachmentType.Product);
        foreach (int productID in info.Products)
        {
            AddAttachment(wc.MessageID, productID, WaterCoolerAttachmentType.Product);
            if (wc.MessageParent != -1)
                AddAttachment((int)wc.MessageParent, productID, WaterCoolerAttachmentType.Product);
        }

        if (wc.MessageParent == -1 && info.PageType == 2)
            AddAttachment((int)wc.MessageID, info.PageID, WaterCoolerAttachmentType.Company);
        foreach (int CompanyID in info.Company)
        {
            AddAttachment(wc.MessageID, CompanyID, WaterCoolerAttachmentType.Company);
            if (wc.MessageParent != -1)
                AddAttachment((int)wc.MessageParent, CompanyID, WaterCoolerAttachmentType.Company);
        }

        if (wc.MessageParent == -1 && info.PageType == 4)
            AddAttachment((int)wc.MessageID, info.PageID, WaterCoolerAttachmentType.Group);
        foreach (int groupID in info.Groups)
        {
            AddAttachment(wc.MessageID, groupID, WaterCoolerAttachmentType.Group);
            if (wc.MessageParent != -1)
                AddAttachment((int)wc.MessageParent, groupID, WaterCoolerAttachmentType.Group);
        }

        foreach (int UserID in info.User)
        {
            AddAttachment(wc.MessageID, UserID, WaterCoolerAttachmentType.User);
            if (wc.MessageParent != -1)
                AddAttachment((int)wc.MessageParent, UserID, WaterCoolerAttachmentType.User);
        }

        return wc.MessageID;
    }

    [WebMethod]
    public void AddAttachment(int messageID, int attachmentID, WaterCoolerAttachmentType attachmentType)
    {
        try
        {
            WatercoolerAttachment ticketAttachment = (new WatercoolerAttachments(TSAuthentication.GetLoginUser()).AddNewWatercoolerAttachment());
            ticketAttachment.MessageID = messageID;
            ticketAttachment.AttachmentID = attachmentID;
            ticketAttachment.RefType = attachmentType;
            ticketAttachment.DateCreated = DateTime.UtcNow;
            ticketAttachment.CreatorID = TSAuthentication.GetLoginUser().UserID;
            ticketAttachment.Collection.Save();
        }
        catch (Exception e)
        {

        }
    }

    [WebMethod]
    public AttachmentProxy[] GetAttachments(int msgid)
    {
        Attachments attachments = new Attachments(TSAuthentication.GetLoginUser());
        attachments.LoadByWatercoolerID(msgid);
        return attachments.GetAttachmentProxies();
    }

    [WebMethod]
    public WatercoolerLikProxy[] GetLikes(int msgid)
    {
        WatercoolerLikes likes = new WatercoolerLikes(TSAuthentication.GetLoginUser());
        likes.LoadByMessageID(msgid);

        return likes.GetWatercoolerLikProxies();
    }

    [WebMethod]
    public WaterCoolerThread[] GetMoreThreads(int pageType, int pageID, int msgcount)
    {
        List<WaterCoolerThread> result = new List<WaterCoolerThread>();

        //This reads the WaterCoolerView because it has UserNames and Groups.  This is readonly

        WaterCoolerView wc = new WaterCoolerView(TSAuthentication.GetLoginUser());
        wc.LoadMoreThreads(pageType, pageID, msgcount);

        foreach (WaterCoolerViewItem item in wc)
        {
            WaterCoolerThread thread = new WaterCoolerThread();

            WaterCoolerView replies = new WaterCoolerView(wc.LoginUser);
            replies.LoadReplies(item.MessageID);

            WatercoolerAttachments wcgroups = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
            wcgroups.LoadByType(item.MessageID, WaterCoolerAttachmentType.Group);

            WatercoolerAttachments wctickets = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
            wctickets.LoadByType(item.MessageID, WaterCoolerAttachmentType.Ticket);

            WatercoolerAttachments wcprods = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
            wcprods.LoadByType(item.MessageID, WaterCoolerAttachmentType.Product);

            WatercoolerAttachments wccompany = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
            wccompany.LoadByType(item.MessageID, WaterCoolerAttachmentType.Company);

            WatercoolerAttachments wcuseratt = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
            wcuseratt.LoadByType(item.MessageID, WaterCoolerAttachmentType.User);

            thread.Message = item.GetProxy();
            thread.Replies = replies.GetWaterCoolerViewItemProxies();
            thread.Groups = wcgroups.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Group);
            thread.Tickets = wctickets.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Ticket);
            thread.Products = wcprods.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Product);
            thread.Company = wccompany.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Company);
            thread.User = wcuseratt.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.User);

            result.Add(thread);
        }

        return result.ToArray();
    }


    [WebMethod]
    public WaterCoolerThread[] GetUpdatedThreads(int msgcount, int pausetime)
    {
        List<WaterCoolerThread> result = new List<WaterCoolerThread>();

        //This reads the WaterCoolerView because it has UserNames and Groups.  This is readonly

        WaterCoolerView wc = new WaterCoolerView(TSAuthentication.GetLoginUser());
        //wc.LoadUpdatedThreads(msgcount, pausetime);


        foreach (WaterCoolerViewItem item in wc)
        {
            WaterCoolerThread thread = new WaterCoolerThread();

            WaterCoolerView replies = new WaterCoolerView(wc.LoginUser);
            replies.LoadReplies(item.MessageID);

            //WatercoolerGroupAtt wcgroups = new WatercoolerGroupAtt(TSAuthentication.GetLoginUser());
            //wcgroups.LoadByMessageID(item.MessageID);
            //WatercoolerTicketAtt wctickets = new WatercoolerTicketAtt(TSAuthentication.GetLoginUser());
            //wctickets.LoadByMessageID(item.MessageID);
            //WatercoolerProductAtt wcprods = new WatercoolerProductAtt(TSAuthentication.GetLoginUser());
            //wcprods.LoadByMessageID(item.MessageID);

            //WatercoolerCompanyAtt wccompany = new WatercoolerCompanyAtt(TSAuthentication.GetLoginUser());
            //wccompany.LoadByMessageID(item.MessageID);

            //WatercoolerUserAtt wcuseratt = new WatercoolerUserAtt(TSAuthentication.GetLoginUser());
            //wcuseratt.LoadByMessageID(item.MessageID);

            thread.Message = item.GetProxy();
            thread.Replies = replies.GetWaterCoolerViewItemProxies();
            //thread.Groups = wcgroups.GetWatercoolerGroupAttItemProxies();
            //thread.Tickets = wctickets.GetWatercoolerTicketAttItemProxies();
            //thread.Products = wcprods.GetWatercoolerProductAttItemProxies();
            //thread.Company = wccompany.GetWatercoolerCompanyAttItemProxies();
            //thread.User = wcuseratt.GetWatercoolerUserAttItemProxies();
            result.Add(thread);
        }

        return result.ToArray();
    }

    [WebMethod]
    public WatercoolerLikProxy[] AddCommentLike(int MessageID)
    {
        WatercoolerLik wclikes = new WatercoolerLikes(TSAuthentication.GetLoginUser()).AddNewWatercoolerLik();
        wclikes.MessageID = MessageID;
        wclikes.UserID = TSAuthentication.GetLoginUser().UserID;
        wclikes.DateCreated = DateTime.UtcNow;
        wclikes.Collection.Save();

        WatercoolerLikes likes = new WatercoolerLikes(TSAuthentication.GetLoginUser());
        likes.LoadByMessageID(MessageID);

        return likes.GetWatercoolerLikProxies();

    }

    /// <summary>
    /// Saves a message to the watercooler DB from javascript
    /// </summary>
    /// <param name="messageID"></param>
    /// <param name="message"></param>
    /// <returns></returns>

    [WebMethod]
    public WaterCoolerViewItemProxy AddMessage(int? messageID, string message)
    {
      // DON'T user the view to save to the DB.  User the WaterCooler table.  The views are read only
      WaterCoolerItem wc = (new WaterCooler(TSAuthentication.GetLoginUser())).AddNewWaterCoolerItem();

      wc.GroupFor = null;
      wc.Message = message;
      wc.MessageType = messageID == null ? "Comment" : "Reply";
      wc.OrganizationID = TSAuthentication.OrganizationID;
      wc.ReplyTo = messageID;
      wc.TimeStamp = DateTime.UtcNow;
      wc.UserID = TSAuthentication.UserID;
      wc.Collection.Save();

      return WaterCoolerView.GetWaterCoolerViewItem(wc.Collection.LoginUser, wc.MessageID).GetProxy();
    }

    /// <summary>
    /// Deletes a WC message or reply
    /// </summary>
    /// <param name="messageID"></param>
    /// <returns></returns>
    [WebMethod]
    public bool DeleteMessage(int messageID)
    {

        //WatercoolerMsg wcm = new WatercoolerMsg(TSAuthentication.GetLoginUser());
        //wcm.LoadByMessageID(messageID);

        WatercoolerMsgItem wcm = WatercoolerMsg.GetWatercoolerMsgItem(TSAuthentication.GetLoginUser(), messageID);

        if (wcm.OrganizationID != TSAuthentication.OrganizationID) return false;
        if (wcm.UserID != TSAuthentication.UserID && !TSAuthentication.IsSystemAdmin) return false;

        wcm.IsDeleted = true;
        wcm.Collection.Save();

        //wcm[0].Delete();
        //wcm[0].Collection.Save();

        return true;
    }

    [WebMethod]
    public AutocompleteItem[] GetGroupsByTerm(string searchTerm)
    {
        Groups group = new Groups(TSAuthentication.GetLoginUser());
        group.LoadByGroupName(TSAuthentication.OrganizationID, searchTerm, 15);

        List<AutocompleteItem> list = new List<AutocompleteItem>();
        foreach (Group g in group)
        {
            list.Add(new AutocompleteItem(g.Name, g.GroupID.ToString()));
        }

        return list.ToArray();
    }

    [WebMethod]
    public AutocompleteItem[] GetProductsByTerm(string searchTerm)
    {
        Products product = new Products(TSAuthentication.GetLoginUser());
        product.LoadByProductName(TSAuthentication.OrganizationID, searchTerm, 15);

        List<AutocompleteItem> list = new List<AutocompleteItem>();
        foreach (Product p in product)
        {
            list.Add(new AutocompleteItem(p.Name, p.ProductID.ToString()));
        }

        return list.ToArray();
    }

    [WebMethod]
    public OnlineUser[] GetOnlineChatUsers(int orgID)
    {
        List<OnlineUser> onlineusers = new List<OnlineUser>();
        Users u = new Users(TSAuthentication.GetLoginUser());
        u.LoadChatOnlineUsers(orgID, TSAuthentication.GetLoginUser().UserID);

        foreach (User online in u)
        {
            OnlineUser ou = new OnlineUser();
            ou.Name = online.FirstLastName;
            ou.AppChatID = online.AppChatID;
            ou.UserID = online.UserID;
            ou.Avatar = GetUserPhoto(online.UserID);

            onlineusers.Add(ou);
        }

        return onlineusers.ToArray();
    }

    public string GetUserPhoto(int userID)
    {
        string path;
        //return Attachments.GetAttachmentPath(TSAuthentication.GetLoginUser(), ReferenceType.UserPhoto, userID);

        if (userID == -99)
            userID = TSAuthentication.GetLoginUser().UserID;

        User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
        Attachments att = new Attachments(TSAuthentication.GetLoginUser());
        att.LoadByReference(ReferenceType.UserPhoto, userID);

        if (att.Count > 0)
        {
            path = String.Format("/dc/{0}/avatar/{1}", user.OrganizationID, att[0].AttachmentID);
        }
        else
            path = "../images/blank_avatar.png";
        return path;
    }

  }

  [DataContract]
  public class OnlineUser
  {
      [DataMember]
      public string Name { get; set; }
      [DataMember]
      public string AppChatID { get; set; }
      [DataMember]
      public int UserID { get; set; }
      [DataMember]
      public string Avatar { get; set; }
  }


  [DataContract]
  public class WaterCoolerThread
  {
    [DataMember] public WaterCoolerViewItemProxy Message { get; set; }
    [DataMember] public WaterCoolerViewItemProxy[] Replies { get; set; }
    [DataMember] public WatercoolerAttachmentProxy[] Tickets { get; set; }
    [DataMember] public WatercoolerAttachmentProxy[] Groups { get; set; }
    [DataMember] public WatercoolerAttachmentProxy[] Products { get; set; }
    [DataMember] public WatercoolerAttachmentProxy[] Company { get; set; }
    [DataMember] public WatercoolerAttachmentProxy[] User { get; set; }
  }

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class WatercoolerJsonInfo
  {
      public WatercoolerJsonInfo() { }
      [DataMember] public string Description { get; set; }
      [DataMember] public List<int> Tickets { get; set; }
      [DataMember] public List<int> Groups { get; set; }
      [DataMember] public List<int> Products { get; set; }
      [DataMember] public List<int> Company { get; set; }
      [DataMember] public List<int> User { get; set; }
      [DataMember] public int ParentTicketID { get; set; }
      [DataMember] public int PageType { get; set; }
      [DataMember] public int PageID { get; set; }
  }


}