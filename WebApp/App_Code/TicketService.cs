using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using System.Runtime.Serialization;

namespace TSWebServices
{
  [ScriptService]
  [WebService(Namespace = "http://teamsupport.com/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class TicketService : System.Web.Services.WebService
  {

    public TicketService()
    {

      //Uncomment the following line if using designed components 
      //InitializeComponent(); 
    }

    [WebMethod]
    public TicketPage GetTicketPage(int pageIndex, int pageSize, TicketLoadFilter filter)
    {
      TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());
      if (filter == null) filter = new TicketLoadFilter();
      tickets.LoadByFilter(pageIndex, pageSize, filter);
      return new TicketPage(pageIndex, pageSize, tickets.GetFilterCount(filter), tickets.GetTicketsViewItemProxies(), filter);
    }

    [WebMethod]
    public TicketRange GetTicketRange(int from, int to, TicketLoadFilter filter)
    {
      TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());
      if (filter == null) filter = new TicketLoadFilter();
      tickets.LoadByRange(from, to, filter);
      return new TicketRange(from, to, tickets.GetFilterCount(filter), tickets.GetTicketsViewItemProxies(), filter);
    }

    [WebMethod]
    public TicketTypeProxy[] GetTicketTypes()
    {
      TicketTypes types = new TicketTypes(TSAuthentication.GetLoginUser());
      types.LoadAllPositions(TSAuthentication.OrganizationID);
      return types.GetTicketTypeProxies();
    }

    [WebMethod]
    public TicketTypeProxy GetTicketType(int ticketTypeID)
    {
      TicketType type = TicketTypes.GetTicketType(TSAuthentication.GetLoginUser(), ticketTypeID);
      if (type.OrganizationID != TSAuthentication.OrganizationID) return null;
      return type.GetProxy();
    }

    [WebMethod]
    public TagProxy[] GetTags()
    {
      Tags tags = new Tags(TSAuthentication.GetLoginUser());
      tags.LoadByOrganization(TSAuthentication.OrganizationID);
      return tags.GetTagProxies();
    }

    [WebMethod]
    public string[] GetContactsAndCustomers(int ticketID)
    {
      List<string> result = new List<string>();
      Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
      organizations.LoadByTicketID(ticketID);

      ContactsView contacts = new ContactsView(TSAuthentication.GetLoginUser());
      contacts.LoadByTicketID(ticketID);

      foreach (Organization o in organizations) { result.Add(o.Name); }
      foreach (ContactsViewItem contact in contacts)
      {
        result.Add(contact.FirstName + " " + contact.LastName + " [" + contact.Organization + "]");
      }

      return result.ToArray();
    }

    [WebMethod]
    public ActionsViewItemProxy[] GetActions(int ticketID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
      ActionsView view = new ActionsView(TSAuthentication.GetLoginUser());
      view.LoadByTicketID(ticketID);
      return view.GetActionsViewItemProxies();
    }

    [WebMethod]
    public void DeleteTag(int tagID)
    {
      if (!TSAuthentication.IsSystemAdmin) return;
      Tag tag = Tags.GetTag(TSAuthentication.GetLoginUser(), tagID);
      tag.Delete();
      tag.Collection.Save();
    }

    [WebMethod]
    public int RenameTag(int tagID, string name)
    {
      int result = tagID;
      if (!TSAuthentication.IsSystemAdmin) return result;
      Tags tags = new Tags(TSAuthentication.GetLoginUser());
      Tag tag = Tags.GetTag(tags.LoginUser, tagID);
      tags.LoadByValue(TSAuthentication.OrganizationID, name);
      if (tags.Count > 0)
      {
        TagLinks links = new TagLinks(tags.LoginUser);
        links.ReplaceTags(tag.TagID, tags[0].TagID);
        tag.Delete();
        tag.Collection.Save();
        result = tags[0].TagID;
      }
      else
      {
        tag.Value = name;
        tag.Collection.Save();
      }
      return result;
    }

    [WebMethod]
    public AutocompleteItem[] GetTicketsByTerm(string term)
    {
      List<AutocompleteItem> result = new List<AutocompleteItem>();
      Tickets tickets = new Tickets(TSAuthentication.GetLoginUser());
      tickets.LoadByDescription(TSAuthentication.OrganizationID, DataUtils.BuildSearchString(term, true));

      foreach (Ticket ticket in tickets)
      {
        AutocompleteItem item = new AutocompleteItem();
        item.id = ticket.TicketNumber.ToString();
        item.label = ticket.Row["TicketDescription"].ToString();
        result.Add(item);
      }

      return result.ToArray();
    }

    [WebMethod]
    public void DeleteTicket(int ticketID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (ticket.OrganizationID == TSAuthentication.OrganizationID && (ticket.CreatorID == TSAuthentication.UserID || TSAuthentication.IsSystemAdmin))
      {
        ticket.Delete();
        ticket.Collection.Save();
      }
    }

    [WebMethod]
    public bool Subscribe(int ticketID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID) return false;
      return Subscriptions.ToggleSubscription(TSAuthentication.GetLoginUser(), TSAuthentication.UserID, ReferenceType.Tickets, ticketID);
    }

    [WebMethod]
    public void Enqueue(int ticketID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID) return;
      TicketQueue.Enqueue(TSAuthentication.GetLoginUser(), ticketID, TSAuthentication.UserID);
    }

    [WebMethod]
    public void TakeOwnership(int ticketID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID) return;
      ticket.UserID = TSAuthentication.UserID;
      ticket.Collection.Save();
    }

    [WebMethod]
    public void RequestUpdate(int ticketID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
      if (ticket == null) return;
      EmailPosts.SendTicketUpdateRequest(TSAuthentication.GetLoginUser(), ticketID);

      string description = String.Format("{0} requested an update from {1} for {2}", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, ticket.UserName, Tickets.GetTicketLink(TSAuthentication.GetLoginUser(), ticketID));
      ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
    }

    [WebMethod]
    public int GetTicketID(int ticketNumber)
    {
      Ticket ticket = Tickets.GetTicketByNumber(TSAuthentication.GetLoginUser(), ticketNumber);
      return ticket.TicketID;
    }

    [WebMethod]
    public int GetTicketNumber(int ticketID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      return ticket.TicketNumber;
    }


    [WebMethod]
    public TicketsViewItemProxy AdminGetTicketByNumber(int orgID, int number)
    {
      if (TSAuthentication.OrganizationID != 1078) return null;
      Ticket ticket = Tickets.GetTicketByNumber(TSAuthentication.GetLoginUser(), orgID, number);
      if (ticket == null) return null;
      return ticket.GetTicketView().GetProxy();
    }

    [WebMethod]
    public TicketsViewItemProxy AdminGetTicketByID(int ticketID)
    {
      if (TSAuthentication.OrganizationID != 1078) return null;
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (ticket == null) return null;
      return ticket.GetTicketView().GetProxy();
    }

    [WebMethod]
    public ActionsViewItemProxy[] AdminGetActions(int ticketID)
    {
      if (TSAuthentication.OrganizationID != 1078) return null;
      ActionsView actions = new ActionsView(TSAuthentication.GetLoginUser());
      actions.LoadByTicketID(ticketID);
      return actions.GetActionsViewItemProxies();
    }

    [WebMethod]
    public void AdminCleanAction(int actionID)
    {
      if (TSAuthentication.OrganizationID != 1078) return;
      TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
      action.Description = HtmlUtility.RemoveInvalidHtmlTags(action.Description);
      action.Collection.Save();
    }

    [WebMethod]
    public void AdminCleanAllActions(int ticketID)
    {
      if (TSAuthentication.OrganizationID != 1078) return;
      Actions actions = new Actions(TSAuthentication.GetLoginUser());
      actions.LoadByTicketID(ticketID);
      foreach (TeamSupport.Data.Action action in actions)
      {
        action.Description = HtmlUtility.RemoveInvalidHtmlTags(action.Description);
      }
      actions.Save();
    }

    [WebMethod]
    public void AdminCleanAllEmailActions(int ticketID)
    {
      if (TSAuthentication.OrganizationID != 1078) return;
      Actions actions = new Actions(TSAuthentication.GetLoginUser());
      actions.LoadByTicketID(ticketID);
      foreach (TeamSupport.Data.Action action in actions)
      {
        if (action.SystemActionTypeID == SystemActionType.Email)
          action.Description = HtmlUtility.RemoveInvalidHtmlTags(action.Description);
      }
      actions.Save();
    }

    [WebMethod]
    public void MarkTicketAsRead(int ticketID)
    {
      UserTicketStatus uts = UserTicketStatuses.GetUserTicketStatus(TSAuthentication.GetLoginUser(), TSAuthentication.UserID, ticketID);
      Ticket ticket = Tickets.GetTicket(uts.Collection.LoginUser, ticketID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID) return;
      uts.DateRead = DateTime.UtcNow;
      uts.Collection.Save();
    }

    [WebMethod]
    public void SetTicketRead(int ticketID, bool value)
    {
      UserTicketStatus uts = UserTicketStatuses.GetUserTicketStatus(TSAuthentication.GetLoginUser(), TSAuthentication.UserID, ticketID);
      Ticket ticket = Tickets.GetTicket(uts.Collection.LoginUser, ticketID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID) return;
      uts.DateRead = value ? DateTime.UtcNow : new DateTime(2000, 1, 1);
      uts.Collection.Save();
    }

    [WebMethod]
    public void SetTicketFlag(int ticketID, bool value)
    {
      UserTicketStatus uts = UserTicketStatuses.GetUserTicketStatus(TSAuthentication.GetLoginUser(), TSAuthentication.UserID, ticketID);
      Ticket ticket = Tickets.GetTicket(uts.Collection.LoginUser, ticketID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID) return;
      uts.IsFlagged = value;
      uts.Collection.Save();
    }

    [WebMethod]
    public void SetSubscribed(int ticketID, bool value)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID) return;
      if (!value) Subscriptions.RemoveSubscription(ticket.Collection.LoginUser, TSAuthentication.UserID, ReferenceType.Tickets, ticketID);
      else Subscriptions.AddSubscription(ticket.Collection.LoginUser, TSAuthentication.UserID, ReferenceType.Tickets, ticketID);
    }

    [WebMethod]
    public string[] CreateDummyTickets()
    {

      List<string> result = new List<string>();
      LoginUser loginuser = TSAuthentication.GetLoginUser();

      for (int i = 0; i < 500; i++)
      {
        try
        {
          DateTime start = DateTime.UtcNow;
          Ticket ticket = (new Tickets(loginuser).AddNewTicket());
          ticket.OrganizationID = 363655;
          ticket.Name = "Dummy Ticket " + i.ToString();
          ticket.TicketSeverityID = 9212;
          ticket.TicketStatusID = 50658;
          ticket.TicketTypeID = 9743;
          ticket.Collection.Save();


          TeamSupport.Data.Action action = (new Actions(loginuser)).AddNewAction();
          action.TicketID = ticket.TicketID;
          action.SystemActionTypeID = SystemActionType.Description;
          action.Description = "Description";
          action.Collection.Save();

          for (int j = 0; j < 5; j++)
          {
            action = (new Actions(loginuser)).AddNewAction();
            action.TicketID = ticket.TicketID;
            action.SystemActionTypeID = SystemActionType.Custom;
            action.ActionTypeID = 4014;
            action.Description = "Action " + j.ToString();
            action.Collection.Save();
          }
          double time = DateTime.UtcNow.Subtract(start).TotalMilliseconds;
          result.Add(time.ToString());
        }
        catch (SqlException sqlEx)
        {
          if (sqlEx.Number == 1205)
          {
            result.Add("DeadLock!");
            throw sqlEx;
          }
        }
        catch (Exception ex)
        {
          ExceptionLogs.LogException(loginuser, ex, "Dummy Tickets");
          result.Add(ex.ToString());
          return result.ToArray();
        }
      }
      return result.ToArray();
    }

    [WebMethod]
    public string[] ReadDummyTickets()
    {
      DateTime start = DateTime.UtcNow;
      List<string> result = new List<string>();

      Tickets tickets = new Tickets(TSAuthentication.GetLoginUser());
      tickets.LoadByOrganizationID(363655);
      double time = DateTime.UtcNow.Subtract(start).TotalMilliseconds;
      result.Add(time.ToString());
      return result.ToArray();
    }
  }

  [DataContract]
  public class TicketRange
  {
    public TicketRange(int from, int to, int total, TicketsViewItemProxy[] tickets, TicketLoadFilter filter)
    {
      From = from;
      To = to;
      Total = total;
      Tickets = tickets;
      Filter = filter;
    }
    [DataMember]
    public int From { get; set; }
    [DataMember]
    public int To { get; set; }
    [DataMember]
    public int Total { get; set; }
    [DataMember]
    public TicketsViewItemProxy[] Tickets { get; set; }
    [DataMember]
    public TicketLoadFilter Filter { get; set; }

  }

  [DataContract]
  public class TicketPage
  {
    public TicketPage(int pageIndex, int pageSize, int count, TicketsViewItemProxy[] tickets, TicketLoadFilter filter)
    {
      PageIndex = pageIndex;
      PageSize = pageSize;
      PageCount = (int)(count / pageSize);
      if (count % pageSize > 0) PageCount++;
      Count = count;
      Tickets = tickets;
      Filter = filter;
    }
    [DataMember]
    public int PageIndex { get; set; }
    [DataMember]
    public int PageSize { get; set; }
    [DataMember]
    public int PageCount { get; set; }
    [DataMember]
    public int Count { get; set; }
    [DataMember]
    public TicketsViewItemProxy[] Tickets { get; set; }
    [DataMember]
    public TicketLoadFilter Filter { get; set; }

  }

}