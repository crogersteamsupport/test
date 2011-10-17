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
using Telerik.Web.UI;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using System.Runtime.Serialization;
using dtSearch.Engine;

namespace TeamSupport.Services
{
  [ScriptService]
  [WebService(Namespace = "http://teamsupport.com/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class PrivateServices : System.Web.Services.WebService
  {

    public PrivateServices()
    {

      //Uncomment the following line if using designed components 
      //InitializeComponent(); 
    }

    [WebMethod(true)]
    public RadComboBoxItemData[] GetUserOrOrganization(RadComboBoxContext context)
    {
      IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;

      Organizations organizations = new Organizations(UserSession.LoginUser);
      organizations.LoadByLikeOrganizationName(UserSession.LoginUser.OrganizationID, context["FilterString"].ToString(), true);

      UsersView users = new UsersView(UserSession.LoginUser);
      users.LoadByLikeName(UserSession.LoginUser.OrganizationID, context["FilterString"].ToString());

      List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
      foreach (Organization organization in organizations)
      {
        RadComboBoxItemData itemData = new RadComboBoxItemData();
        itemData.Text = organization.Name;
        itemData.Value = 'o' + organization.OrganizationID.ToString();
        list.Add(itemData);
      }

      foreach (UsersViewItem user in users)
      {
        RadComboBoxItemData itemData = new RadComboBoxItemData();
        itemData.Text = String.Format("{0}, {1} [{2}]", user.LastName, user.FirstName, user.Organization);
        itemData.Value = 'u' + user.UserID.ToString();
        list.Add(itemData);
      }

      return list.ToArray();
    }
    /*
    [WebMethod(true)]
    public RadComboBoxItemData[] GetOrganizationByLikeName(RadComboBoxContext context)
    {
      IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;

      Organizations organizations = new Organizations(UserSession.LoginUser);
      organizations.LoadByLikeOrganizationName(UserSession.LoginUser.OrganizationID, context["FilterString"].ToString(), !UserSession.CurrentUser.IsSystemAdmin);

      List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
      foreach (Organization organization in organizations)
      {
        RadComboBoxItemData itemData = new RadComboBoxItemData();
        itemData.Text = organization.Name;
        itemData.Value = organization.OrganizationID.ToString();
        list.Add(itemData);
      }

      return list.ToArray();
    }*/

    [WebMethod(true)]
    public RadComboBoxItemData[] GetUsers(RadComboBoxContext context)
    {
      IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;
      List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
      try
      {
        Users users = new Users(UserSession.LoginUser);
        //string search = context["FilterString"].ToString();
        //users.LoadByName(search, UserSession.LoginUser.OrganizationID, true);
        users.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, true);

        foreach (User user in users)
        {
          RadComboBoxItemData itemData = new RadComboBoxItemData();
          itemData.Text = user.FirstLastName;
          itemData.Value = user.UserID.ToString();
          list.Add(itemData);
        }
      }
      catch (Exception)
      {
      }
      if (list.Count < 1)
      {
        RadComboBoxItemData noData = new RadComboBoxItemData();
        noData.Text = "[No users to display.]";
        noData.Value = "-1";
        list.Add(noData);
      }

      return list.ToArray();
    }

    [WebMethod(true)]
    public RadComboBoxItemData[] GetQuickTicket(RadComboBoxContext context)
    {

      Options options = new Options();
      options.TextFlags = TextFlags.dtsoTfRecognizeDates;

      using (SearchJob job = new SearchJob())
      {
        string searchTerm = context["FilterString"].ToString().Trim();
        job.Request = searchTerm;
        job.FieldWeights = "TicketNumber: 5000, Name: 1000";
        job.BooleanConditions = "OrganizationID::" + TSAuthentication.OrganizationID.ToString();
        job.MaxFilesToRetrieve = 25;
        job.AutoStopLimit = 100000;
        job.TimeoutSeconds = 10;
        job.SearchFlags =
          SearchFlags.dtsSearchSelectMostRecent |
          SearchFlags.dtsSearchStemming |
          SearchFlags.dtsSearchDelayDocInfo;

        int num = 0;
        if (!int.TryParse(searchTerm, out num))
        {
          job.Fuzziness = 1;
          job.Request = job.Request + "*";
          job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchFuzzy;
        }

        if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;
        job.IndexesToSearch.Add(SystemSettings.ReadString(TSAuthentication.GetLoginUser(), "IndexerPathTickets", ""));
        job.Execute();
        SearchResults results = job.Results;


        IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;
        List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
        try
        {
          for (int i = 0; i < results.Count; i++)
          {
            results.GetNthDoc(i);
            RadComboBoxItemData itemData = new RadComboBoxItemData();
            itemData.Text = results.CurrentItem.DisplayName;
            itemData.Value = results.CurrentItem.Filename;
            list.Add(itemData);
          }
        }
        catch (Exception)
        {
        }
        if (list.Count < 1)
        {
          RadComboBoxItemData noData = new RadComboBoxItemData();
          noData.Text = "[No tickets to display.]";
          noData.Value = "-1";
          list.Add(noData);
        }

        return list.ToArray();
      }
    }


    [WebMethod(true)]
    public RadComboBoxItemData[] GetTicketByDescription(RadComboBoxContext context)
    {
      Options options = new Options();
      options.TextFlags = TextFlags.dtsoTfRecognizeDates;

      using (SearchJob job = new SearchJob())
      {
        string searchTerm = context["FilterString"].ToString().Trim();
        job.Request = searchTerm;
        job.FieldWeights = "TicketNumber: 5000, Name: 1000";
        job.BooleanConditions = "OrganizationID::" + TSAuthentication.OrganizationID.ToString();
        job.MaxFilesToRetrieve = 25;
        job.AutoStopLimit = 100000;
        job.TimeoutSeconds = 10;
        job.SearchFlags =
          SearchFlags.dtsSearchStemming |
          SearchFlags.dtsSearchDelayDocInfo;

        int num = 0;
        if (!int.TryParse(searchTerm, out num))
        {
          job.Fuzziness = 1;
          job.Request = job.Request + "*";
          job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchFuzzy | SearchFlags.dtsSearchSelectMostRecent;
        }

        if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;
        job.IndexesToSearch.Add(SystemSettings.ReadString(TSAuthentication.GetLoginUser(), "IndexerPathTickets", ""));
        job.Execute();
        SearchResults results = job.Results;

        IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;
        List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
        try
        {
          for (int i = 0; i < results.Count; i++)
          {
            results.GetNthDoc(i);
            RadComboBoxItemData itemData = new RadComboBoxItemData();
            itemData.Text = results.CurrentItem.DisplayName;
            itemData.Value = results.CurrentItem.Filename + "," + results.CurrentItem.UserFields["TicketNumber"].ToString();
            list.Add(itemData);
          }
        }
        catch (Exception)
        {
        }
        if (list.Count < 1)
        {
          RadComboBoxItemData noData = new RadComboBoxItemData();
          noData.Text = "[No tickets to display.]";
          noData.Value = "-1";
          list.Add(noData);
        }

        return list.ToArray();
      }
    }

    [WebMethod(true)]
    public RadComboBoxItemData[] GetKBTicketByDescription(RadComboBoxContext context)
    {
      Options options = new Options();
      options.TextFlags = TextFlags.dtsoTfRecognizeDates;

      using (SearchJob job = new SearchJob())
      {
        string searchTerm = context["FilterString"].ToString().Trim();
        job.Request = searchTerm;
        job.FieldWeights = "TicketNumber: 5000, Name: 1000";
        job.BooleanConditions = "(OrganizationID::" + TSAuthentication.OrganizationID.ToString() + ") AND (IsKnowledgeBase::True)";
        job.MaxFilesToRetrieve = 25;
        job.AutoStopLimit = 100000;
        job.TimeoutSeconds = 10;
        job.SearchFlags =
          SearchFlags.dtsSearchSelectMostRecent |
          SearchFlags.dtsSearchStemming |
          SearchFlags.dtsSearchDelayDocInfo;

        int num = 0;
        if (!int.TryParse(searchTerm, out num))
        {
          job.Fuzziness = 1;
          job.Request = job.Request + "*";
          job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchFuzzy;
        }

        if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;
        job.IndexesToSearch.Add(SystemSettings.ReadString(TSAuthentication.GetLoginUser(), "IndexerPathTickets", ""));
        job.Execute();
        SearchResults results = job.Results;


        IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;
        List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
        try
        {
          for (int i = 0; i < results.Count; i++)
          {
            results.GetNthDoc(i);
            RadComboBoxItemData itemData = new RadComboBoxItemData();
            itemData.Text = results.CurrentItem.DisplayName;
            itemData.Value = results.CurrentItem.Filename + "," + results.CurrentItem.UserFields["TicketNumber"].ToString();
            list.Add(itemData);
          }
        }
        catch (Exception)
        {
        }
        if (list.Count < 1)
        {
          RadComboBoxItemData noData = new RadComboBoxItemData();
          noData.Text = "[No tickets to display.]";
          noData.Value = "-1";
          list.Add(noData);
        }

        return list.ToArray();
      }
    }


    [WebMethod(true)]
    public RadComboBoxItemData[] GetTicketTags(RadComboBoxContext context)
    {
      IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;
      Tags tags = new Tags(UserSession.LoginUser);
      string search = context["FilterString"].ToString();
      tags.LoadBySearchTerm(search);
      

      List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
      foreach (Tag tag in tags)
      {
        RadComboBoxItemData itemData = new RadComboBoxItemData();
        itemData.Text = tag.Value;
        itemData.Value = tag.TagID.ToString();
        list.Add(itemData);
      }

      return list.ToArray();
    }

    [WebMethod(true)]
    public void UpdateUserPingTime()
    {
      Users.UpdateUserPingTime(UserSession.LoginUser, UserSession.LoginUser.UserID);
    }

    [WebMethod(true)]
    public void UpdateUserActivityTime()
    {
      Users.UpdateUserActivityTime(UserSession.LoginUser, UserSession.LoginUser.UserID);
    }

    [WebMethod(true)]
    public IEnumerable GetReportNodes(RadTreeNodeData node, IDictionary context) {
        Reports _reports = new Reports(UserSession.LoginUser);

        switch ((ReportType)Enum.Parse(typeof(ReportType), node.Value)) { 
            case ReportType.Standard:
                _reports.LoadStandard();
                break;
            case ReportType.Favorite:
                _reports.LoadFavorites(UserSession.CurrentUser.UserID);
                break;
            default:
                _reports.LoadCustom(UserSession.CurrentUser.OrganizationID);
                break;
        }

        List<RadTreeNodeData> list = new List<RadTreeNodeData>();
        foreach (Report rep in _reports) {
            RadTreeNodeData newNode = new RadTreeNodeData();
            newNode.Text = rep.Name;
            newNode.Value = rep.ReportID.ToString();
            newNode.Attributes.Add("ExternalURL", rep.ExternalURL);
            list.Add(newNode);
        }
        return list;
    }

    [WebMethod(true)]
    public IEnumerable GetVersionNodes(RadTreeNodeData node, IDictionary context)
    {
      ProductVersions versions = new ProductVersions(UserSession.LoginUser);
      versions.LoadByProductID(int.Parse(node.Value));

      List<RadTreeNodeData> list = new List<RadTreeNodeData>();
      foreach (ProductVersion version in versions)
      {
        RadTreeNodeData newNode = new RadTreeNodeData();
        newNode.Text = version.VersionNumber;
        newNode.Value = version.ProductVersionID.ToString();
        list.Add(newNode);
      }
      return list;
    }

    [WebMethod(true)]
    public void SubscribeToTicket(int ticketID)
    {
      Tickets tickets = new Tickets(UserSession.LoginUser);
      if (Tickets.IsUserSubscribed(UserSession.LoginUser, UserSession.LoginUser.UserID, ticketID))
        tickets.RemoveSubscription(UserSession.LoginUser.UserID, ticketID);
      else
        tickets.AddSubscription(UserSession.LoginUser.UserID, ticketID);
    }

    [WebMethod(true)]
    public bool IsSubscribedToTicket(int ticketID)
    {
      return Tickets.IsUserSubscribed(UserSession.LoginUser, UserSession.LoginUser.UserID, ticketID);
    }

    [WebMethod(true)]
    public void Subscribe(ReferenceType refType, int refID)
    {
      if (Subscriptions.IsUserSubscribed(UserSession.LoginUser, UserSession.LoginUser.UserID, refType, refID))
        Subscriptions.RemoveSubscription(UserSession.LoginUser, UserSession.LoginUser.UserID, refType, refID);
      else
        Subscriptions.AddSubscription(UserSession.LoginUser, UserSession.LoginUser.UserID, refType, refID);
    }

    [WebMethod(true)]
    public bool IsSubscribed(ReferenceType refType, int refID)
    {
      return Subscriptions.IsUserSubscribed(UserSession.LoginUser, UserSession.LoginUser.UserID, refType, refID);
    }

    [WebMethod(true)]
    public void RequestTicketUpdate(int ticketID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(UserSession.LoginUser, ticketID);
      if (ticket ==  null) return;
      EmailPosts.SendTicketUpdateRequest(UserSession.LoginUser, ticketID);
      
      string description = String.Format("{0} requested an update from {1} for {2}", UserSession.CurrentUser.FirstLastName, ticket.UserName,  Tickets.GetTicketLink(UserSession.LoginUser, ticketID));
      ActionLogs.AddActionLog(UserSession.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
    }

    [WebMethod(true)]
    public int GetTicketID(int ticketNumber)
    {
      Ticket ticket = Tickets.GetTicketByNumber(UserSession.LoginUser, ticketNumber);
      if (ticket != null && ticket.OrganizationID == UserSession.LoginUser.OrganizationID)
        return ticket.TicketID;
      else
        return -1;
    }

    [WebMethod(true)]
    public void TakeTicketOwnership(int ticketID)
    {
      Tickets tickets = new Tickets(UserSession.LoginUser);
      tickets.LoadByTicketID(ticketID);

      if (!tickets.IsEmpty)
      {
        tickets[0].UserID = UserSession.LoginUser.UserID;
        tickets.Save();
      }
    }


    [WebMethod(true)]
    public void SetUserSetting(string key, string value)
    {
      Settings.UserDB.WriteString(key, value);
      Users.UpdateUserActivityTime(UserSession.LoginUser, UserSession.LoginUser.UserID);
    }

    [WebMethod(true)]
    public void SetSessionSetting(string key, string value)
    {
      Settings.Session.WriteString(key, value);
      Users.UpdateUserActivityTime(UserSession.LoginUser, UserSession.LoginUser.UserID);
    }

    [WebMethod(true)]
    public string GetUserSetting(string key, string defaultValue)
    {
      return Settings.UserDB.ReadString(key, defaultValue);
    }

    [WebMethod(true)]
    public string GetSessionSetting(string key, string defaultValue)
    {
      return Settings.Session.ReadString(key, defaultValue);
    }

    [WebMethod(true)]
    public string GetUserStatusText()
    {
      User user = Users.GetUser(UserSession.LoginUser, UserSession.LoginUser.UserID);
      return user.InOfficeComment;
    }

    [WebMethod(true)]
    public bool GetUserAvailability()
    {
      User user = Users.GetUser(UserSession.LoginUser, UserSession.LoginUser.UserID);
      return user.InOffice;
    }

    [WebMethod(true)]
    public string SetUserStatusText(string text)
    {
      User user = Users.GetUser(UserSession.LoginUser, UserSession.LoginUser.UserID);
      user.InOfficeComment = Server.HtmlEncode(text);
      user.Collection.Save();
      WaterCooler watercooler = new WaterCooler(UserSession.LoginUser);
      WaterCoolerItem item = watercooler.AddNewWaterCoolerItem();
      item.Message = string.Format("<strong>{0} - </strong>{1}", user.FirstLastName, user.InOfficeComment);
      item.OrganizationID = user.OrganizationID;
      item.TimeStamp = DateTime.UtcNow;
      item.UserID = user.UserID;
      watercooler.Save();
      return user.InOfficeComment;
    }

    [WebMethod(true)]
    public bool ToggleUserAvailability()
    {
      User user = Users.GetUser(UserSession.LoginUser, UserSession.LoginUser.UserID);
      user.InOffice = !user.InOffice;
      user.Collection.Save();
      WaterCooler watercooler = new WaterCooler(UserSession.LoginUser);
      WaterCoolerItem item = watercooler.AddNewWaterCoolerItem();
      item.Message = string.Format("<strong>{0}</strong> {1}", user.FirstLastName, user.InOffice ? "is now in the office." : "has left the office.");
      item.OrganizationID = user.OrganizationID;
      item.TimeStamp = DateTime.UtcNow;
      item.UserID = user.UserID;
      watercooler.Save();

      return user.InOffice;
    }

    [WebMethod(true)]
    public bool ToggleUserChat()
    {
      ChatUserSetting setting = ChatUserSettings.GetSetting(UserSession.LoginUser, UserSession.LoginUser.UserID);
      setting.IsAvailable = !setting.IsAvailable;
      setting.Collection.Save();
      return setting.IsAvailable;
    }

    [WebMethod(true)]
    public bool CanEditReport(int reportID)
    {
      Report report = (Report)Reports.GetReport(UserSession.LoginUser, reportID);
      return (
              (report.OrganizationID != null && report.OrganizationID == UserSession.LoginUser.OrganizationID) ||
              (report.OrganizationID == null && UserSession.LoginUser.OrganizationID == 1078)
              ) 
              && 
              (UserSession.CurrentUser.IsSystemAdmin || report.CreatorID == UserSession.LoginUser.UserID)
              && 
              string.IsNullOrEmpty(report.Query);
    }

    [WebMethod(true)]
    public bool IsFavoriteReport(int reportID) {
        Report report = (Report)Reports.GetReport(UserSession.LoginUser, reportID);
        return report.IsFavorite;
    }

    [WebMethod(true)]
    public void ToggleFavoriteReport(int reportID)
    {
        Report report = (Report)Reports.GetReport(UserSession.LoginUser, reportID);
        if (report.IsFavorite) { report.IsFavorite = false; }
        else { report.IsFavorite = true; }
    }


    #region Admin Methods

    [WebMethod(true)]
    public void DeleteTicket(int ticketID)
    { 
      if (!UserSession.CurrentUser.IsSystemAdmin) return;
      Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, ticketID);
      if (ticket.OrganizationID != UserSession.LoginUser.OrganizationID) return;
      ticket.Delete();
      ticket.Collection.Save();
    }

    [WebMethod(true)]
    public void DeleteAction(int actionID)
    {
      if (!UserSession.CurrentUser.IsSystemAdmin) return;
      TeamSupport.Data.Action action = Actions.GetAction(UserSession.LoginUser, actionID);
      if (action == null) return;
      Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, action.TicketID);
      if (ticket == null) return;
      if (ticket.OrganizationID != UserSession.LoginUser.OrganizationID) return;

      action.Delete();
      action.Collection.Save();
    }

    [WebMethod(true)]
    public void DeleteProduct(int productID)
    {
      if (!UserSession.CurrentUser.IsSystemAdmin) return;
      try
      {
        Products.DeleteProduct(UserSession.LoginUser, productID);
      }
      catch (Exception ex)
      {
        DataUtils.LogException(UserSession.LoginUser, ex);
      }
    }

    [WebMethod(true)]
    public void DeleteVersion(int versionID)
    {
      if (!UserSession.CurrentUser.IsSystemAdmin) return;
      try
      {
        ProductVersions.DeleteProductVersion(UserSession.LoginUser, versionID);
      }
      catch (Exception ex)
      {
        DataUtils.LogException(UserSession.LoginUser, ex);
      }
    }

    [WebMethod(true)]
    public void DeleteGroup(int groupID)
    {
      if (!UserSession.CurrentUser.IsSystemAdmin) return;
      try
      {
        Groups groups = new Groups(UserSession.LoginUser);
        groups.DeleteFromDB(groupID);

      }
      catch (Exception ex)
      {
        DataUtils.LogException(UserSession.LoginUser, ex);
      }
    }

    [WebMethod(true)]
    public void DeleteAttachment(int attachmentID)
    {
      //if (!UserSession.CurrentUser.IsSystemAdmin) return;
      try
      {
        Attachments.DeleteAttachmentAndFile(UserSession.LoginUser, attachmentID);
      }
      catch (Exception ex)
      {
        DataUtils.LogException(UserSession.LoginUser, ex);
      }
    }

    [WebMethod(true)]
    public void DeleteGroupUser(int groupID, int userID)
    {
      if (!UserSession.CurrentUser.IsSystemAdmin) return;
      try
      {
        Groups groups = new Groups(UserSession.LoginUser);
        groups.DeleteGroupUser(groupID, userID);

      }
      catch (Exception ex)
      {
        DataUtils.LogException(UserSession.LoginUser, ex);
      }
    }

    [WebMethod(true)]
    public void DeleteUser(int userID)
    {
      if (!UserSession.CurrentUser.IsSystemAdmin) return;
      Users.MarkUserDeleted(UserSession.LoginUser, userID);
    }

    [WebMethod(true)]
    public void DeleteNote(int noteID)
    {
      Note note = Notes.GetNote(UserSession.LoginUser, noteID);
      if (note.CreatorID != UserSession.CurrentUser.UserID && !UserSession.CurrentUser.IsSystemAdmin) return;
      note.Delete();
      note.Collection.Save();
    }

    [WebMethod(true)]
    public void DeleteOrganization(int organizationID)
    {
      if (!UserSession.CurrentUser.IsSystemAdmin) return;
      try
      {
        Organizations.DeleteOrganizationAndAllReleatedData(UserSession.LoginUser, organizationID);

      }
      catch (Exception ex)
      {
        DataUtils.LogException(UserSession.LoginUser, ex);
      }
    }

    [WebMethod(true)]
    public void DeleteOrganizationProduct(int organizationProductID)
    {
      if (!UserSession.CurrentUser.IsSystemAdmin) return;
      try
      {
        OrganizationProducts organizationProducts = new OrganizationProducts(UserSession.LoginUser);
        organizationProducts.DeleteFromDB(organizationProductID);
      }
      catch (Exception ex)
      {
        DataUtils.LogException(UserSession.LoginUser, ex);
      }
    }

    [WebMethod(true)]
    public void DeleteTicketOrganization(int organizationID, int ticketID)
    {
      try
      {
        Tickets tickets = new Tickets(UserSession.LoginUser);
        tickets.RemoveOrganization(organizationID, ticketID);
      }
      catch (Exception ex)
      {
        DataUtils.LogException(UserSession.LoginUser, ex);
      }
    }

    [WebMethod(true)]
    public void DeleteTicketContact(int userID, int ticketID)
    {
      try
      {
        Tickets tickets = new Tickets(UserSession.LoginUser);
        tickets.RemoveContact(userID, ticketID);
      }
      catch (Exception ex)
      {
        DataUtils.LogException(UserSession.LoginUser, ex);
      }
    }

    [WebMethod(true)]
    public void AddTicketOrganization(string id, int ticketID)
    {
      try
      {
        id = id.Trim();
        if (id.Length < 1) return;
        bool isUser = id.ToLower()[0] == 'u';
        int i = int.Parse(id.Remove(0, 1));

        Tickets tickets = new Tickets(UserSession.LoginUser);

        if (isUser) tickets.AddContact(i, ticketID); else tickets.AddOrganization(i, ticketID);
      }
      catch (Exception ex)
      {
        DataUtils.LogException(UserSession.LoginUser, ex);
      }
    }

    [WebMethod(true)]
    public void DeleteReport(int reportID)
    {
      if (!UserSession.CurrentUser.IsSystemAdmin) return;
      Report report = (Report)Reports.GetReport(UserSession.LoginUser, reportID);
      if (report.OrganizationID != null && report.OrganizationID == UserSession.LoginUser.OrganizationID)
      {
        report.Delete();
        report.Collection.Save();
      }
    }
    
    #endregion

    [WebMethod(true)]
    public void AddRelatedTicket(int ticketID1, int ticketID2)
    {
      Ticket ticket1 = Tickets.GetTicket(UserSession.LoginUser, ticketID1);
      Ticket ticket2 = Tickets.GetTicket(UserSession.LoginUser, ticketID2);

      if (ticket1.ParentID == ticketID2 || ticket2.ParentID == ticketID1) return;
      if (ticketID1 == ticketID2) return;

      TicketRelationship item = TicketRelationships.GetTicketRelationship(UserSession.LoginUser, ticketID1, ticketID2);
      if (item == null)
      {
        item = (new TicketRelationships(UserSession.LoginUser)).AddNewTicketRelationship();
        item.OrganizationID = UserSession.LoginUser.OrganizationID;
        item.Ticket1ID = ticketID1;
        item.Ticket2ID = ticketID2;
        item.Collection.Save();
      }
    }

    [WebMethod(true)]
    public void AddParentTicket(int ticketID, int parentID)
    {
      if (ticketID == parentID) return;
      Ticket parent = Tickets.GetTicket(UserSession.LoginUser, parentID);
      if (parent.ParentID == ticketID) return;
      Ticket child = Tickets.GetTicket(UserSession.LoginUser, ticketID);
      child.ParentID = parentID;
      child.Collection.Save();
    }

    [WebMethod(true)]
    public void AddChildTicket(int ticketID, int childID)
    {
      if (ticketID == childID) return;
      Ticket parent = Tickets.GetTicket(UserSession.LoginUser, ticketID);
      if (parent.ParentID == childID) return;
      Ticket child = Tickets.GetTicket(UserSession.LoginUser, childID);
      child.ParentID = ticketID;
      child.Collection.Save();
    }

    [WebMethod(true)]
    public void RemoveRelatedTicket(int ticketID1, int ticketID2)
    {
      TicketRelationship item = TicketRelationships.GetTicketRelationship(UserSession.LoginUser, ticketID1, ticketID2);
      //if (item.CreatorID == UserSession.LoginUser.UserID || UserSession.CurrentUser.IsSystemAdmin)
      {
        item.Delete();
        item.Collection.Save();
      }
    }

    [WebMethod(true)]
    public void RemoveParentTicket(int ticketID)
    {
      Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, ticketID);
      ticket.ParentID = null;
      ticket.Collection.Save();
    }

    [WebMethod(true)]
    public void RemoveChildTicket(int childID)
    {
      Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, childID);
      ticket.ParentID = null;
      ticket.Collection.Save();
    }

    [WebMethod(true)]
    public void AddTicketTag(int tagID, int ticketID)
    {
      Tag tag = Tags.GetTag(UserSession.LoginUser, tagID);
      Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, ticketID);
      if (tag.OrganizationID != UserSession.LoginUser.OrganizationID || ticket.OrganizationID != UserSession.LoginUser.OrganizationID) return;
      TagLink link = TagLinks.GetTagLink(UserSession.LoginUser, ReferenceType.Tickets, ticketID, tagID);
      if (link == null)
      { 
        TagLinks links = new TagLinks(UserSession.LoginUser);
        link = links.AddNewTagLink();
        link.RefType = ReferenceType.Tickets;
        link.RefID = ticketID;
        link.TagID = tagID;
        links.Save();
      }
    }

    [WebMethod(true)]
    public void AddTicketTagByValue(int ticketID, string value)
    {
      value = value.Trim();
      Tag tag = Tags.GetTag(UserSession.LoginUser, value);
      if (tag == null)
      {
        Tags tags = new Tags(UserSession.LoginUser);
        tag = tags.AddNewTag();
        tag.OrganizationID = UserSession.LoginUser.OrganizationID;
        tag.Value = value;
        tags.Save();
      }

      AddTicketTag(tag.TagID, ticketID);

    }

   
    [WebMethod(true)]
    public void DeletePhone(int phoneID)
    {
      PhoneNumbers phoneNumbers = new PhoneNumbers(UserSession.LoginUser);
      phoneNumbers.DeleteFromDB(phoneID);
    }

    [WebMethod(true)]
    public void DeleteAddress(int addressID)
    {
      Addresses addresses = new Addresses(UserSession.LoginUser);
      addresses.DeleteFromDB(addressID);
    }

    [WebMethod(true)]
    public TicketProxy GetTicket(int ticketID)
    {
      Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, ticketID);
      return ticket.GetProxy();
    }

    [WebMethod(true)]
    public TicketProxy GetTicketByNumber(int ticketNumber)
    {
      Tickets tickets = new Tickets(UserSession.LoginUser);
      tickets.LoadByTicketNumber(UserSession.LoginUser.OrganizationID, ticketNumber);
      return tickets[0].GetProxy();
    }


    [WebMethod(true)]
    public void SaveCustomFieldText(int refID, int fieldID, string value)
    {
      CustomValue customValue = CustomValues.GetValue(UserSession.LoginUser, fieldID, refID);
      customValue.Value = value;
      customValue.Collection.Save();
    }

    [WebMethod(true)]
    public void SaveCustomFieldNumber(int refID, int fieldID, int value)
    {
      CustomValue customValue = CustomValues.GetValue(UserSession.LoginUser, fieldID, refID);
      customValue.Value = value.ToString();
      customValue.Collection.Save();
    }

    [WebMethod(true)]
    public void SaveCustomFieldDate(int refID, int fieldID, DateTime? value)
    {
      DateTime? date;
      try
      {
        if (value == null) date = null; else date = Convert.ToDateTime(value);
      }
      catch (Exception)
      {
        date = null;
        
      }
      CustomValue customValue = CustomValues.GetValue(UserSession.LoginUser, fieldID, refID);
      if (date != null) customValue.Value = DataUtils.DateToUtc(UserSession.LoginUser, date).ToString();
      else customValue.Value = "";
      customValue.Collection.Save();
    }

    [WebMethod(true)]
    public void SaveCustomFieldBool(int refID, int fieldID, bool value)
    {
      CustomValue customValue = CustomValues.GetValue(UserSession.LoginUser, fieldID, refID);
      customValue.Value = value.ToString();
      customValue.Collection.Save();
    }

  }

}