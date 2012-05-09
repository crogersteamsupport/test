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
using dtSearch.Engine;
using HtmlAgilityPack;
using System.IO;
using TidyNet;

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
    public TicketSeverityProxy[] GetTicketSeverities()
    {
      TicketSeverities severities = new TicketSeverities(TSAuthentication.GetLoginUser());
      severities.LoadAllPositions(TSAuthentication.OrganizationID);
      return severities.GetTicketSeverityProxies();
    }

    [WebMethod]
    public TicketStatusProxy[] GetTicketStatuses()
    {
      TicketStatuses statuses = new TicketStatuses(TSAuthentication.GetLoginUser());
      statuses.LoadByOrganizationID(TSAuthentication.OrganizationID);
      return statuses.GetTicketStatusProxies();
    }

    [WebMethod]
    public TicketNextStatusProxy[] GetNextTicketStatuses()
    {
      TicketNextStatuses statuses = new TicketNextStatuses(TSAuthentication.GetLoginUser());
      statuses.LoadAll(TSAuthentication.OrganizationID);
      return statuses.GetTicketNextStatusProxies();
    }

    [WebMethod]
    public TicketStatusProxy[] GetAvailableTicketStatuses(int statusID)
    {
      TicketStatuses statuses = new TicketStatuses(TSAuthentication.GetLoginUser());
      statuses.LoadNextStatuses(statusID);
      return statuses.GetTicketStatusProxies();
    }

    [WebMethod]
    public ActionTypeProxy[] GetActionTypes()
    {
      ActionTypes types = new ActionTypes(TSAuthentication.GetLoginUser());
      types.LoadAllPositions(TSAuthentication.OrganizationID);
      return types.GetActionTypeProxies();
    }

    [WebMethod]
    public string[] ActionToText(int actionID)
    {
      List<string> result = new List<string>();
      TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
     
      result.Add(HtmlToText.ConvertHtml(HtmlUtility.TidyHtml(action.Description)));
      result.Add(action.Description);
      return result.ToArray();

    }

    [WebMethod]
    public TicketTypeProxy GetTicketType(int ticketTypeID)
    {
      TicketType type = TicketTypes.GetTicketType(TSAuthentication.GetLoginUser(), ticketTypeID);
      if (type.OrganizationID != TSAuthentication.OrganizationID) return null;
      return type.GetProxy();
    }
    /*
    [WebMethod]
    public TicketsViewItemProxy SetTicketType(int ticketID, int ticketTypeID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (CanEditTicket(ticket))
      {
        TicketStatuses statuses = new TicketStatuses(ticket.Collection.LoginUser);
        statuses.LoadAvailableTicketStatuses(ticket.TicketTypeID, null);
        if (statuses.IsEmpty) return null;

        ticket.TicketTypeID = ticketTypeID;
        ticket.TicketStatusID = statuses[0].TicketStatusID;
        ticket.Collection.Save();
        return ticket.GetTicketView().GetProxy();
      }
      return null;
    }*/

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
    public object[] GetKBTicketAndActions(int ticketID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
      ActionsView view = new ActionsView(TSAuthentication.GetLoginUser());
      view.LoadKBByTicketID(ticketID);

      List<object> result = new List<object>();
      result.Add(ticket.GetProxy());
      result.Add(view.GetActionsViewItemProxies());
      return result.ToArray();
    }

    [WebMethod]
    public ActionsViewItemProxy GetAction(int actionID)
    {
      ActionsViewItem action = ActionsView.GetActionsViewItem(TSAuthentication.GetLoginUser(), actionID);
      Ticket ticket = Tickets.GetTicket(action.Collection.LoginUser, action.TicketID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
      //action.Name = action.ActionTitle;
      return action.GetProxy();
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
    public AutocompleteItem[] SearchTickets(string searchTerm, TicketLoadFilter filter)
    {
      try
      {
        SearchResults results = TicketsView.GetQuickSearchTicketResults(searchTerm, TSAuthentication.GetLoginUser(), filter);
        List<AutocompleteItem> items = new List<AutocompleteItem>();
        //for (int i = 0; i < job.Errors.Count; i++) { items.Add(new AutocompleteItem(job.Errors.Message(i), "")); }

        for (int i = 0; i < results.Count; i++)
        {
          results.GetNthDoc(i);

          items.Add(new AutocompleteItem(results.CurrentItem.DisplayName, results.CurrentItem.UserFields["TicketNumber"].ToString(), results.CurrentItem.UserFields["TicketID"].ToString()));
        }
        return items.ToArray();
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "TicketService.SearchTickets");
      }
      return null;
    }



    [WebMethod]
    public string[] SearchTicketsTest(string searchTerm, int organizationID)
    {
      if (TSAuthentication.OrganizationID != 1078) return null;
      LoginUser loginUser = new LoginUser(TSAuthentication.GetLoginUser().ConnectionString, -1, organizationID, null);
      SearchResults results = TicketsView.GetSearchTicketResults(searchTerm, loginUser, null);

      SearchReportJob report = results.NewSearchReportJob();
      report.SelectAll();
      report.Flags = ReportFlags.dtsReportStoreInResults | ReportFlags.dtsReportWholeFile | ReportFlags.dtsReportGetFromCache | ReportFlags.dtsReportIncludeAll;
      report.OutputFormat = OutputFormats.itUnformattedHTML;
      report.MaxContextBlocks = 3;
      report.MaxWordsToRead = 50000;
      report.OutputStringMaxSize = 50000;
      //report.WordsOfContext = 10;
      report.BeforeHit = "123456789";// "<strong style=\"color:red;\">";
      report.AfterHit = "987654321"; //"</strong>";
      report.OutputToString = true;
      report.Execute();
      report.ContextSeparator = "<br/><br/>";
      List<string> result = new List<string>();


      for (int i = 0; i < results.Count; i++)
      {
        results.GetNthDoc(i);

        using (FileConverter fc = new FileConverter ()) 
				{
			
					// This sets up FileConverter with the input file, index location, and hits
          fc.SetInputItem(results, 0);
					
					fc.OutputToString = true;
					fc.OutputStringMaxSize = 2000000;
					fc.OutputFormat = OutputFormats.itHTML;

					//String 	scriptName = Request.ServerVariables.Get ("SCRIPT_NAME");
					//String virtPath = MakeVirtual (res.get_DocDetailItem("_filename"));
					//String HitNavSrc = GetVirtualFolder(scriptName) + "HitNav.js";
					//String HitNavLink =  "\r\n<script language=JavaScript src=\"" + HitNavSrc + "\"></script>\r\n";
					fc.Header = "<A NAME=hit0></A>"    
						+ "<TABLE border=0>"  
						+ "<TR><TD bgcolor=#003399 color=#ffffff>"
						+ "<font face=verdana color=#ffffff size=2></TD></TR>"
						+ "<TR><TD bgcolor=#eeeeee color=#000000>"
						+ "<font face=verdana color=#003399 size=2>"
            + "<B>" + results.get_DocDetailItem("_shortName") + "   </B><BR>"
						+ "<font face=verdana color=#000000 size=1>"
            + results.DocHitCount + " Hits."
						+ "  <B>Location: </B>  <B>Date: </B>"
            + results.get_DocDetailItem("_date") + "<BR>"
						+ "</FONT>"
						+ "</TD></TR><TR><TD bgcolor=#003399 color=#ffffff>"
						+"<font face=verdana color=#ffffff size=2></TD></TR></TABLE>";


					// Set up additional settings for the file converter
					fc.BeforeHit = "<span style=\"background-color: #FF0000\">";
					fc.AfterHit = "</span>";
					
					// If the index was built with cached text, get the document from the cache
					fc.Flags = (fc.Flags | ConvertFlags.dtsConvertGetFromCache);

					// Execute the file conversion
					fc.Execute ();

          StringBuilder builder = new StringBuilder();
          //builder.Append("<h1>" + results.CurrentItem.DisplayName + "</h1>");
          //builder.Append("<p class=\"ui-helper-hiddenx\">" + results.CurrentItem.Synopsis + "</p>");
          
          JobErrorInfo errors = fc.Errors;
          if ((errors.Count > 0) || (fc.OutputString.Length < 2))
          {
            builder.AppendLine("<html><h1>Error converting document to HTML</h1>");
            builder.AppendLine("<p>" + Server.HtmlEncode(fc.InputFile));
            if (errors.Count > 0)
            {
              builder.AppendLine("<p>");
              builder.AppendLine(Server.HtmlEncode(errors.Message(0)));
            }
            builder.AppendLine("<p>Note: To prevent conversion errors when highlighting hits " +
              "use caching when building the index.  For more information, see " +
              "<a href=\"http://www.dtsearch.com/index7.html\">http://www.dtsearch.com/index7.html</a> " +
              "and the dtsIndexCacheOriginalFiles flag in the dtSearch Engine API documentation");

          }
          else
          {
            // Output the result of the file conversion
            builder.AppendLine(fc.OutputString);
          }
          result.Add(builder.ToString());
				}
        


      }
      return result.ToArray();
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
    public TicketsViewItemProxy GetTicket(int ticketID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
      return ticket.GetProxy();
    }

    [WebMethod]
    public string GetTicketTypeTemplateText(int ticketTypeID)
    {
      if (TicketTemplates.GetTicketTypeCount(TSAuthentication.GetLoginUser()) > 0)
      {
        TicketTemplate template = TicketTemplates.GetByTicketType(TSAuthentication.GetLoginUser(), ticketTypeID);
        if (template == null) return "";
        if (template.OrganizationID != TSAuthentication.OrganizationID) return "";
        return template.TemplateText;
      }

      return null;
    }

    [WebMethod]
    public string GetValueTemplateText(string value)
    {
      int count = TicketTemplates.GetTriggerTextCount(TSAuthentication.GetLoginUser());
      if (count < 1) return null;

      TicketTemplate template = TicketTemplates.GetByTriggerText(TSAuthentication.GetLoginUser(), value);
      if (template == null) return "";
      if (template.OrganizationID != TSAuthentication.OrganizationID) return "";
      return template.TemplateText;
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
    public void DeleteAction(int actionID)
    {
      TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
      if (!CanDeleteAction(action)) return;
      action.Delete();
      action.Collection.Save();
    }

    [WebMethod]
    public ActionInfo UpdateAction(ActionProxy proxy)
    {
      TeamSupport.Data.Action action = Actions.GetActionByID(TSAuthentication.GetLoginUser(), proxy.ActionID);

      if (action == null)
      { 
        action = (new Actions(TSAuthentication.GetLoginUser())).AddNewAction();
        action.TicketID = proxy.TicketID;
        action.CreatorID = TSAuthentication.UserID;
      }
      
      if (!CanEditAction(action)) return null;
      action.Description = proxy.Description;
      action.ActionTypeID = proxy.ActionTypeID;
      action.DateStarted = proxy.DateStarted;
      action.TimeSpent = proxy.TimeSpent;
      action.IsKnowledgeBase = proxy.IsKnowledgeBase;
      action.IsVisibleOnPortal = proxy.IsVisibleOnPortal;
      action.Collection.Save();

      return GetActionInfo(action.ActionID);
    }

    [WebMethod]
    public bool SetIsVisibleOnPortal(int ticketID, bool value)
    { 
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (!CanEditTicket(ticket)) return value;
      ticket.IsVisibleOnPortal = value;
      ticket.Collection.Save();
      return ticket.IsVisibleOnPortal;
    }

    private void Delay(int seconds)
    {
      System.Threading.Thread.Sleep(seconds * 1000);
    }

    [WebMethod]
    public bool SetIsKB(int ticketID, bool value)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (!CanEditTicket(ticket)) return value;
      ticket.IsKnowledgeBase = value;
      ticket.Collection.Save();
      return ticket.IsKnowledgeBase;
    }

    [WebMethod]
    public string SetTicketName(int ticketID, string name)
    {
      if (name.Trim() == "") name = "[Untitled Ticket]";
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (!CanEditTicket(ticket)) return name;
      ticket.Name = HttpUtility.HtmlEncode(name);
      ticket.Collection.Save();
      return ticket.Name;
    }

    [WebMethod]
    public object[] SetTicketType(int ticketID, int ticketTypeID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (ticketTypeID == ticket.TicketTypeID) return null;
      if (!CanEditTicket(ticket)) return null;
      TicketType ticketType = TicketTypes.GetTicketType(ticket.Collection.LoginUser, ticketTypeID);
      if (ticketType.OrganizationID != TSAuthentication.OrganizationID) return null;
      ticket.TicketTypeID = ticketTypeID;

      TicketStatuses statuses = new TicketStatuses(ticket.Collection.LoginUser);
      statuses.LoadAvailableTicketStatuses(ticketTypeID, null);
      ticket.TicketStatusID = statuses[0].TicketStatusID;
      ticket.Collection.Save();
      List<object> result = new List<object>();
      result.Add(statuses[0].GetProxy());
      result.Add(GetCustomValues(ticketID));
      return result.ToArray();
    }

    [WebMethod]
    public TicketStatusProxy SetTicketStatus(int ticketID, int ticketStatusID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (ticketStatusID == ticket.TicketStatusID) return null;
      if (!CanEditTicket(ticket)) return null;
      TicketStatus status = TicketStatuses.GetTicketStatus(ticket.Collection.LoginUser, ticketStatusID);
      if (status.OrganizationID != TSAuthentication.OrganizationID) return null;
      ticket.TicketStatusID = ticketStatusID;
      ticket.Collection.Save();
      return status.GetProxy();
    }

    [WebMethod]
    public TicketSeverityProxy SetTicketSeverity(int ticketID, int ticketSeverityID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (ticketSeverityID == ticket.TicketSeverityID) return null;
      if (!CanEditTicket(ticket)) return null;
      TicketSeverity severity = TicketSeverities.GetTicketSeverity(ticket.Collection.LoginUser, ticketSeverityID);
      if (severity.OrganizationID != TSAuthentication.OrganizationID) return null;
      ticket.TicketSeverityID = ticketSeverityID;
      ticket.Collection.Save();
      return severity.GetProxy();
    }

    [WebMethod]
    public ForumCategoryProxy SetTicketCommunity(int ticketID, int? categoryID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (!CanEditTicket(ticket)) return  null;
      if (categoryID == null)
      {
        ticket.RemoveCommunityTicket();
      }
      else
      {
        ticket.AddCommunityTicket((int)categoryID);
        return ForumCategories.GetForumCategory(ticket.Collection.LoginUser, (int)categoryID).GetProxy();
      }
      return null;
    }
    

    [WebMethod]
    public UserInfo SetTicketUser(int ticketID, int? userID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (!CanEditTicket(ticket)) return null;

      UsersViewItem user = userID != null ? UsersView.GetUsersViewItem(TSAuthentication.GetLoginUser(), (int)userID) : null; 
      if (userID == ticket.UserID) return null;
      if (user != null && user.OrganizationID != TSAuthentication.OrganizationID) return null;
      ticket.UserID = userID;
      ticket.Collection.Save();
      return user == null ? null : new UserInfo(user);
    }

    [WebMethod]
    public string SetTicketGroup(int ticketID, int? groupID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (!CanEditTicket(ticket)) return null;

      Group group = groupID != null ? Groups.GetGroup(TSAuthentication.GetLoginUser(), (int)groupID) : null;
      if (groupID == ticket.GroupID) return null;
      if (group != null && group.OrganizationID != TSAuthentication.OrganizationID) return null;
      ticket.GroupID = groupID;
      ticket.Collection.Save();
      return group == null ? "" : group.Name;
    }

    [WebMethod]
    public AutocompleteItem SetProduct(int ticketID, int? productID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (!CanEditTicket(ticket)) return null;

      Product product = productID != null ? Products.GetProduct(TSAuthentication.GetLoginUser(), (int)productID) : null;
      if (productID == ticket.ProductID) return null;
      if (product != null && product.OrganizationID != TSAuthentication.OrganizationID) return null;
      ticket.ProductID = productID;
      ticket.ReportedVersionID = null;
      ticket.SolvedVersionID = null;
      ticket.Collection.Save();
      if (product != null) return new AutocompleteItem(product.Name, product.ProductID.ToString());
      return new AutocompleteItem(null, null);
    }

    [WebMethod]
    public AutocompleteItem SetReportedVersion(int ticketID, int? versionID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (!CanEditTicket(ticket)) return null;

      ProductVersion version = versionID != null ? ProductVersions.GetProductVersion(ticket.Collection.LoginUser, (int)versionID) : null;
      if (version != null)
      {
        Product product = Products.GetProduct(TSAuthentication.GetLoginUser(), version.ProductID);
        if (product.OrganizationID != TSAuthentication.OrganizationID) return null;
      }

      if (versionID == ticket.ReportedVersionID) return null;
      ticket.ReportedVersionID = versionID;
      ticket.Collection.Save();
      return version == null ? new AutocompleteItem(null, null) : new AutocompleteItem(version.VersionNumber, version.ProductVersionID.ToString());
    }

    [WebMethod]
    public AutocompleteItem SetSolvedVersion(int ticketID, int? versionID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (!CanEditTicket(ticket)) return null;

      ProductVersion version = versionID != null ? ProductVersions.GetProductVersion(ticket.Collection.LoginUser, (int)versionID) : null;
      if (version != null)
      {
        Product product = Products.GetProduct(TSAuthentication.GetLoginUser(), version.ProductID);
        if (product.OrganizationID != TSAuthentication.OrganizationID) return null;
      }

      if (versionID == ticket.SolvedVersionID) return null;
      ticket.SolvedVersionID = versionID;
      ticket.Collection.Save();
      return version == null ? new AutocompleteItem(null, null) : new AutocompleteItem(version.VersionNumber, version.ProductVersionID.ToString());
    }

    [WebMethod]
    public UserInfo AssignUser(int ticketID, int? userID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (!CanEditTicket(ticket)) return null;

      UsersViewItem user = userID != null ? UsersView.GetUsersViewItem(ticket.Collection.LoginUser, (int)userID) : null;
      if (user != null)
      {
        if (user.OrganizationID != TSAuthentication.OrganizationID) return null;
      }
      if (ticket.UserID == userID) return null;
      ticket.UserID = userID;
      ticket.Collection.Save();
      return user == null ? null : new UserInfo(user);
    }

   
    private void TransferCustomFields(int ticketID, int oldTicketTypeID, int newTicketTypeID)
    {
      CustomFields oldFields = new CustomFields(TSAuthentication.GetLoginUser());
      oldFields.LoadByTicketTypeID(TSAuthentication.OrganizationID, oldTicketTypeID);

      CustomFields newFields = new CustomFields(oldFields.LoginUser);
      newFields.LoadByTicketTypeID(TSAuthentication.OrganizationID, newTicketTypeID);


      foreach (CustomField oldField in oldFields)
      {
        CustomField newField = newFields.FindByName(oldField.Name);
        if (newField != null)
        {
          CustomValue newValue = CustomValues.GetValue(oldFields.LoginUser, newField.CustomFieldID, ticketID);
          CustomValue oldValue = CustomValues.GetValue(oldFields.LoginUser, oldField.CustomFieldID, ticketID);

          if (newValue != null && oldValue != null)
          {
            newValue.Value = oldValue.Value;
            newValue.Collection.Save();
          }
        }

      }
    }

    private bool CanEditTicket(Ticket ticket)
    {
      return true;
    }

    private bool CanEditAction(TeamSupport.Data.Action action)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), action.TicketID);
      return (ticket.OrganizationID == TSAuthentication.OrganizationID && (action.CreatorID == TSAuthentication.UserID || TSAuthentication.IsSystemAdmin));
    }

    private bool CanDeleteAction(TeamSupport.Data.Action action)
    {
      return CanEditAction(action) && action.SystemActionTypeID != SystemActionType.Description;
    }

    [WebMethod]
    public bool SetActionKb(int actionID, bool isKb)
    {
      TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
      if (CanEditAction(action))
      {
        action.IsKnowledgeBase = isKb;
        action.Collection.Save();
      }
      return action.IsKnowledgeBase;
    }

    [WebMethod]
    public bool SetActionPortal(int actionID, bool isVisibleOnPortal)
    {
      TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
      if (CanEditAction(action))
      {
        action.IsVisibleOnPortal = isVisibleOnPortal;
        action.Collection.Save();
      }
      return action.IsVisibleOnPortal;
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
    public void Dequeue(int ticketID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID) return;
      TicketQueue.Dequeue(TSAuthentication.GetLoginUser(), ticketID, TSAuthentication.UserID);
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
    public AssetProxy[] AddTicketAsset(int ticketID, int assetID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (!CanEditTicket(ticket)) return null;

      Asset asset = Assets.GetAsset(ticket.Collection.LoginUser, assetID);
      if (asset.OrganizationID != TSAuthentication.OrganizationID) return null;
      ticket.Collection.AddAsset(assetID, ticketID);

      Assets assets = new Assets(ticket.Collection.LoginUser);
      assets.LoadByTicketID(ticketID);
      return assets.GetAssetProxies();
    }

    [WebMethod]
    public AssetProxy[] RemoveTicketAsset(int ticketID, int assetID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (!CanEditTicket(ticket)) return null;
      ticket.Collection.RemoveAsset(assetID, ticketID);
      Assets assets = new Assets(ticket.Collection.LoginUser);
      assets.LoadByTicketID(ticketID);
      if (assets.IsEmpty) return null;
      return assets.GetAssetProxies();
    }

    [WebMethod]
    public TicketCustomer[] AddTicketCustomer(int ticketID, string type, int id)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (!CanEditTicket(ticket)) return null;
      if (type == "u")
      {
        ContactsViewItem contact = ContactsView.GetContactsViewItem(TSAuthentication.GetLoginUser(), id);
        if (contact.OrganizationParentID != TSAuthentication.OrganizationID) return null;
        ticket.Collection.AddContact(id, ticketID);
      }
      else
      {
        Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), id);
        if (organization.ParentID != TSAuthentication.OrganizationID) return null;
        ticket.Collection.AddOrganization(id, ticketID);
      }

      return GetTicketCustomers(ticketID);
    }

    [WebMethod]
    public TicketCustomer GetTicketCustomer(string type, int id)
    {
      if (type == "u")
      {
        ContactsViewItem contact = ContactsView.GetContactsViewItem(TSAuthentication.GetLoginUser(), id);
        if (contact.OrganizationParentID != TSAuthentication.OrganizationID) return null;
        TicketCustomer customer = new TicketCustomer();
        customer.Company = contact.Organization;
        customer.OrganizationID = contact.OrganizationID;
        customer.Contact = contact.FirstName + " " + contact.LastName;
        customer.UserID = contact.UserID;
        return customer;
      }
      else
      {
        Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), id);
        if (organization.ParentID != TSAuthentication.OrganizationID) return null;
        TicketCustomer customer = new TicketCustomer();
        customer.Company = organization.Name;
        customer.OrganizationID = organization.OrganizationID;
        customer.UserID = null;
        return customer;
      }

    }

    [WebMethod]
    public TicketCustomer GetChatCustomer(int chatID)
    { 
      Chat chat = Chats.GetChat(TSAuthentication.GetLoginUser(), chatID);
      if (chat == null || chat.OrganizationID != TSAuthentication.OrganizationID) return null;
      return GetTicketCustomer("u", chat.GetInitiatorLinkedUserID());
    }

    [WebMethod]
    public TicketCustomer[] RemoveTicketContact(int ticketID, int userID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (!CanEditTicket(ticket)) return null;
      ticket.Collection.RemoveContact(userID, ticketID);
      return GetTicketCustomers(ticketID);
    }

    [WebMethod]
    public TicketCustomer[] RemoveTicketCompany(int ticketID, int organizationID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (!CanEditTicket(ticket)) return null;
      ticket.Collection.RemoveOrganization(organizationID, ticketID);
      return GetTicketCustomers(ticketID);
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
    public UserInfo[] SetSubscribed(int ticketID, bool value, int? userID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
      UsersViewItem user = UsersView.GetUsersViewItem(ticket.Collection.LoginUser, userID == null ? TSAuthentication.UserID : (int)userID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID || user.OrganizationID != TSAuthentication.OrganizationID) return null;
      if (!value) Subscriptions.RemoveSubscription(ticket.Collection.LoginUser, user.UserID, ReferenceType.Tickets, ticketID);
      else Subscriptions.AddSubscription(ticket.Collection.LoginUser, user.UserID, ReferenceType.Tickets, ticketID);
      return GetSubscribers(ticket);
    }

    [WebMethod]
    public UserInfo[] SetQueue(int ticketID, bool value, int? userID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
      UsersViewItem user = UsersView.GetUsersViewItem(ticket.Collection.LoginUser, userID == null ? TSAuthentication.UserID : (int)userID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID || user.OrganizationID != TSAuthentication.OrganizationID) return null;
      if (!value) TicketQueue.Dequeue(user.Collection.LoginUser, ticketID, user.UserID);
      else TicketQueue.Enqueue(user.Collection.LoginUser, ticketID, user.UserID);
      return GetQueuers(ticket);
    }

    [WebMethod]
    public void EmailTicket(int ticketID, string addresses)
    {
      EmailPosts posts = new EmailPosts(TSAuthentication.GetLoginUser());
      EmailPost post = posts.AddNewEmailPost();
      post.EmailPostType = EmailPostType.TicketSendEmail;
      post.HoldTime = 0;

      post.Param1 = TSAuthentication.UserID.ToString();
      post.Param2 = ticketID.ToString();
      post.Param3 = addresses;
      posts.Save();
    }

    [WebMethod]
    public TicketsViewItemProxy[] GetContactTickets(int userID)
    {
      TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());
      tickets.LoadByContactID(userID, "DateModified DESC");
      return tickets.GetTicketsViewItemProxies();
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

    [WebMethod]
    public ActionLogProxy[] GetTicketHistory(int ticketID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
      ActionLogs logs = new ActionLogs(ticket.Collection.LoginUser);
      
      logs.LoadByTicketID(ticketID);
      return logs.GetActionLogProxies();
    }

    [WebMethod]
    public TicketInfo GetTicketInfo(int ticketNumber)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItemByNumber(TSAuthentication.GetLoginUser(), ticketNumber);
      if (ticket == null) return null;
      if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
      MarkTicketAsRead(ticket.TicketID);

      TicketInfo info = new TicketInfo();
      info.Ticket = ticket.GetProxy();

      info.Customers = GetTicketCustomers(ticket.TicketID);
      info.Related = GetRelatedTickets(ticket.TicketID);
      info.Tags = GetTicketTags(ticket.TicketID);
      info.CustomValues = GetCustomValues(ticket.TicketID);
      info.Subscribers = GetSubscribers(ticket);
      info.Queuers = GetQueuers(ticket);

      Reminders reminders = new Reminders(ticket.Collection.LoginUser);
      reminders.LoadByItem(ReferenceType.Tickets, ticket.TicketID, TSAuthentication.UserID);
      info.Reminders = reminders.GetReminderProxies();

      Assets assets = new Assets(ticket.Collection.LoginUser);
      assets.LoadByTicketID(ticket.TicketID);
      info.Assets = assets.GetAssetProxies();

      Actions actions = new Actions(ticket.Collection.LoginUser);
      actions.LoadByTicketID(ticket.TicketID);

      List<ActionInfo> actionInfos = new List<ActionInfo>();
      actions.Table.Columns["Description"].AllowDBNull = true;

      for (int i = 0; i < actions.Count; i++)
      {
        ActionInfo actionInfo = GetActionInfo(ticket.Collection.LoginUser, actions[i], i < 10 || actions[i].SystemActionTypeID == SystemActionType.Description);
        actionInfos.Add(actionInfo);
      }

      info.Actions = actionInfos.ToArray();

      return info;
    }

    private ActionInfo GetActionInfo(LoginUser loginUser, TeamSupport.Data.Action action)
    {
      return GetActionInfo(loginUser, action, true);
    }

    private ActionInfo GetActionInfo(LoginUser loginUser, TeamSupport.Data.Action action, bool includeDescription)
    {
      ActionInfo actionInfo = new ActionInfo();
      actionInfo.Action = action.GetProxy();
      if (!includeDescription) actionInfo.Action.Description = null;
      if (actionInfo.Action.Description != null)
      {
        actionInfo.Action.Description = HtmlUtility.TagHtml(TSAuthentication.GetLoginUser(), HtmlUtility.Sanitize(HtmlUtility.CheckScreenR(loginUser, actionInfo.Action.Description)));
      }

      UsersViewItem creator = UsersView.GetUsersViewItem(loginUser, action.CreatorID);
      if (creator != null) actionInfo.Creator = new UserInfo(creator);
      actionInfo.Attachments = action.GetAttachments().GetAttachmentProxies();
      return actionInfo;
    }

    [WebMethod]
    public ActionInfo GetActionInfo(int actionID)
    {
      TeamSupport.Data.Action action = Actions.GetActionByID(TSAuthentication.GetLoginUser(), actionID);
      Ticket ticket = Tickets.GetTicket(action.Collection.LoginUser, action.TicketID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
      return GetActionInfo(action.Collection.LoginUser, action);
    }

    [WebMethod]
    public CustomValueProxy[] GetCustomValues(int ticketID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
      CustomValues values = new CustomValues(ticket.Collection.LoginUser);
      values.LoadByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Tickets, ticket.TicketTypeID, ticket.TicketID);
      return values.GetCustomValueProxies();
    }

    [WebMethod]
    public TagProxy[] GetTicketTags(int ticketID)
    {
      Tags tags = new Tags(TSAuthentication.GetLoginUser());
      tags.LoadByReference(ReferenceType.Tickets, ticketID);
      return tags.GetTagProxies();
    }

    [WebMethod]
    public TagProxy[] AddTag(int ticketID, string value)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (!CanEditTicket(ticket)) return null;
      value = value.Trim();
      if (value == "") return null;
      Tag tag = Tags.GetTag(ticket.Collection.LoginUser, value);
      if (tag == null)
      {
        Tags tags = new Tags(ticket.Collection.LoginUser);
        tag = tags.AddNewTag();
        tag.OrganizationID = TSAuthentication.OrganizationID;
        tag.Value = value;
        tags.Save();
      }

      TagLink link = TagLinks.GetTagLink(ticket.Collection.LoginUser, ReferenceType.Tickets, ticketID, tag.TagID);
      if (link == null)
      { 
        TagLinks links = new TagLinks(ticket.Collection.LoginUser);
        link = links.AddNewTagLink();
        link.RefType = ReferenceType.Tickets;
        link.RefID = ticketID;
        link.TagID = tag.TagID;
        links.Save();
      }
      return GetTicketTags(ticketID);
    }

    [WebMethod]
    public TagProxy[] RemoveTag(int ticketID, int tagID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      if (!CanEditTicket(ticket)) return null;
      TagLink link = TagLinks.GetTagLink(TSAuthentication.GetLoginUser(), ReferenceType.Tickets, ticketID, tagID);
      Tag tag = Tags.GetTag(TSAuthentication.GetLoginUser(), tagID);
      int count = tag.GetLinkCount();
      link.Delete();
      link.Collection.Save();
      if (count < 2)
      {
        tag.Delete();
        tag.Collection.Save();
      }

      return GetTicketTags(ticketID);
    }

    private bool IsTicketRelated(Ticket ticket1, Ticket ticket2)
    {
      if (ticket1.ParentID != null && ticket1.ParentID == ticket2.TicketID) return true;
      if (ticket2.ParentID != null && ticket2.ParentID == ticket1.TicketID) return true;
      TicketRelationship item = TicketRelationships.GetTicketRelationship(ticket1.Collection.LoginUser, ticket1.TicketID, ticket2.TicketID);
      return item != null;
    }

    [WebMethod]
    public RelatedTicket[] AddRelated(int ticketID1, int ticketID2, bool? isTicket1Parent)
    {
      if (ticketID1 == ticketID2) return null;

      Ticket ticket1 = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID1);
      Ticket ticket2 = Tickets.GetTicket(ticket1.Collection.LoginUser, ticketID2);

      if (!CanEditTicket(ticket1)) return null;

      if (isTicket1Parent == null) // just related
      {
        
        if (IsTicketRelated(ticket1, ticket2)) 
        {
          throw new Exception("The ticket is already associated.");
        }

        TicketRelationship item = (new TicketRelationships(ticket1.Collection.LoginUser)).AddNewTicketRelationship();
        item.OrganizationID = TSAuthentication.OrganizationID;
        item.Ticket1ID = ticketID1;
        item.Ticket2ID = ticketID2;
        item.Collection.Save();
      }
      else if (isTicket1Parent == true) // parent
      {
        if (ticket2.ParentID != null)
        {
          if (ticket1.ParentID == ticket2.TicketID) return null;
          throw new Exception("Ticket " + ticket2.TicketNumber + " is already a child of another ticket.");
        }
        TicketRelationship item = TicketRelationships.GetTicketRelationship(ticket1.Collection.LoginUser, ticketID1, ticketID2);
        if (item != null)
        {
          item.Delete();
          item.Collection.Save();
        }

        ticket2.ParentID = ticket1.TicketID;
        ticket2.Collection.Save();
      }
      else // child
      {
        if (ticket1.ParentID != null && ticket1.ParentID == ticket2.TicketID) return null;

        TicketRelationship item = TicketRelationships.GetTicketRelationship(ticket1.Collection.LoginUser, ticketID1, ticketID2);
        if (item != null)
        {
          item.Delete();
          item.Collection.Save();
        }

        ticket1.ParentID = ticket2.TicketID;
        ticket1.Collection.Save();
      }
      return GetRelatedTickets(ticket1.TicketID);
    
    }

    [WebMethod]
    public int[] GetCustomerProductIDs(string customerIDs)
    {
      int[] list = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(customerIDs);
      if (list.Length < 1) return null;

      Products products = new Products(TSAuthentication.GetLoginUser());
      products.LoadByCustomerIDs(list);

      if (products.IsEmpty) return null;
      List<int> productIDs = new List<int>();
      foreach (Product product in products)
      {
        productIDs.Add(product.ProductID);
      }

      return productIDs.ToArray();

    }

    [WebMethod]
    public int[] GetTicketCustomerProductIDs(int ticketID)
    {
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
      Organizations organizations = new Organizations(ticket.Collection.LoginUser);
      organizations.LoadByTicketID(ticketID);
      if (organizations.IsEmpty) return null;

      List<int> organizationIDs = new List<int>();
      foreach (Organization organization in organizations)
      {
        organizationIDs.Add(organization.OrganizationID);
      }
      Products products = new Products(ticket.Collection.LoginUser);
      products.LoadByCustomerIDs(organizationIDs.ToArray());

      if (products.IsEmpty) return null;
      List<int> productIDs = new List<int>();
      foreach (Product product in products)
      {
        productIDs.Add(product.ProductID);
      }

      return productIDs.ToArray();

    }

    [WebMethod]
    public bool? RemoveRelated(int ticketID1, int ticketID2)
    {
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      Ticket ticket1 = Tickets.GetTicket(loginUser, ticketID1);
      Ticket ticket2 = Tickets.GetTicket(loginUser, ticketID2);
      if (!CanEditTicket(ticket1)) return null;
      
      TicketRelationship item = TicketRelationships.GetTicketRelationship(loginUser, ticketID1, ticketID2);
      if (item != null)
      {
        item.Delete();
        item.Collection.Save();
      }

      if (ticket1.ParentID != null && ticket1.ParentID == (int)ticketID2)
      {
        ticket1.ParentID = null;
        ticket1.Collection.Save();
      }

      if (ticket2.ParentID != null && ticket2.ParentID == (int)ticketID1)
      {
        ticket2.ParentID = null;
        ticket2.Collection.Save();
      }

      return true;
    }

    [WebMethod]
    public AutocompleteItem[] SearchTags(string term)
    {
      List<AutocompleteItem> result = new List<AutocompleteItem>();
      Tags tags = new Tags(TSAuthentication.GetLoginUser());
      tags.LoadBySearchTerm(term);
      foreach (Tag tag in tags)
      {
        result.Add(new AutocompleteItem(tag.Value, tag.TagID.ToString()));
      }

      return result.ToArray();
    }

    public RelatedTicket[] GetRelatedTickets(int ticketID)
    {
      List<RelatedTicket> relatedTickets = new List<RelatedTicket>();
      Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);

      if (ticket.ParentID != null)
      {
        TicketsViewItem parent = TicketsView.GetTicketsViewItem(ticket.Collection.LoginUser, (int)ticket.ParentID);
        if (parent != null)
        {
          RelatedTicket relatedTicket = new RelatedTicket();
          relatedTicket.TicketID = parent.TicketID;
          relatedTicket.TicketNumber = parent.TicketNumber;
          relatedTicket.Name = parent.Name;
          relatedTicket.Severity = parent.Severity;
          relatedTicket.Status = parent.Status;
          relatedTicket.Type = parent.TicketTypeName;
          relatedTicket.IsParent = true;
          relatedTickets.Add(relatedTicket);
        }
      }


      TicketsView tickets = new TicketsView(ticket.Collection.LoginUser);
      tickets.LoadChildren(ticket.TicketID);
      foreach (TicketsViewItem item in tickets)
      {
        RelatedTicket relatedTicket = new RelatedTicket();
        relatedTicket.TicketID = item.TicketID;
        relatedTicket.TicketNumber = item.TicketNumber;
        relatedTicket.Name = item.Name;
        relatedTicket.Severity = item.Severity;
        relatedTicket.Status = item.Status;
        relatedTicket.Type = item.TicketTypeName;
        relatedTicket.IsParent = false;
        relatedTickets.Add(relatedTicket);
      }

      tickets = new TicketsView(ticket.Collection.LoginUser);
      tickets.LoadRelated(ticket.TicketID);
      foreach (TicketsViewItem item in tickets)
      {
        RelatedTicket relatedTicket = new RelatedTicket();
        relatedTicket.TicketID = item.TicketID;
        relatedTicket.TicketNumber = item.TicketNumber;
        relatedTicket.Name = item.Name;
        relatedTicket.Severity = item.Severity;
        relatedTicket.Status = item.Status;
        relatedTicket.Type = item.TicketTypeName;
        relatedTicket.IsParent = null;
        relatedTickets.Add(relatedTicket);
      }

      return relatedTickets.ToArray();
    }

    private TicketCustomer[] GetTicketCustomers(int ticketID)
    {
      List<TicketCustomer> customers = new List<TicketCustomer>();

      ContactsView contacts = new ContactsView(TSAuthentication.GetLoginUser());
      contacts.LoadByTicketID(ticketID);
      foreach (ContactsViewItem contact in contacts)
      {
        TicketCustomer customer = new TicketCustomer();
        customer.Company = contact.Organization;
        customer.OrganizationID = contact.OrganizationID;
        customer.Contact = contact.FirstName + " " + contact.LastName;
        customer.UserID = contact.UserID;
        customers.Add(customer);
      }

      Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
      organizations.LoadByNotContactTicketID(ticketID);
      foreach (Organization organization in organizations)
      {
        TicketCustomer customer = new TicketCustomer();
        customer.Company = organization.Name;
        customer.OrganizationID = organization.OrganizationID;
        customer.UserID = null;
        customers.Add(customer);
      }
      return customers.ToArray();
    }

    private AssetProxy[] GetTicketAssets(int ticketID)
    {
      Assets assets = new Assets(TSAuthentication.GetLoginUser());
      assets.LoadByTicketID(ticketID);
      return assets.GetAssetProxies();
    }

    [WebMethod]
    public UserInfo[] GetSubscribers(int ticketID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
      if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
      return GetSubscribers(ticket);
    }

    private UserInfo[] GetSubscribers(TicketsViewItem ticket)
    {
      UsersView users = new UsersView(ticket.Collection.LoginUser);
      users.LoadBySubscription(ticket.TicketID, ReferenceType.Tickets);
      List<UserInfo> result = new List<UserInfo>();
      foreach (UsersViewItem user in users)
      {
        result.Add(new UserInfo(user));
      }
      return result.ToArray();
    }

    private UserInfo[] GetQueuers(TicketsViewItem ticket)
    {
      UsersView users = new UsersView(ticket.Collection.LoginUser);
      users.LoadByTicketQueue(ticket.TicketID);
      List<UserInfo> result = new List<UserInfo>();
      foreach (UsersViewItem user in users)
      {
        result.Add(new UserInfo(user));
      }
      return result.ToArray();
    }

    [WebMethod]
    public void DeleteAttachment(int attachmentID)
    {
      Attachment attachment = Attachments.GetAttachment(TSAuthentication.GetLoginUser(), attachmentID);
      if (attachment == null || attachment.RefType != ReferenceType.Actions) return;
      TeamSupport.Data.Action action = Actions.GetAction(attachment.Collection.LoginUser, attachment.RefID);
      if (!CanEditAction(action)) return;
      attachment.DeleteFile();
      attachment.Delete();
      attachment.Collection.Save();
    }

    [WebMethod]
    public NewTicketSaveInfo DummyTicketSaveInfo()
    {
      return null;
    }

    [WebMethod]
    public int[] NewTicket(string data)
    {
      List<int> result = new List<int>();
      NewTicketSaveInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<NewTicketSaveInfo>(data);

      Ticket ticket = (new Tickets(TSAuthentication.GetLoginUser())).AddNewTicket();
      ticket.OrganizationID = TSAuthentication.OrganizationID;
      ticket.TicketSource = info.ChatID != null ? "Chat" : "Agent";
      ticket.Name = info.Name;
      ticket.TicketTypeID = info.TicketTypeID;
      ticket.TicketStatusID = info.TicketStatusID;
      ticket.TicketSeverityID = info.TicketSeverityID;
      ticket.UserID = info.UserID < 0 ? null : (int?) info.UserID;
      ticket.GroupID = info.GroupID < 0 ? null : (int?) info.GroupID;
      ticket.ProductID = info.ProductID < 0 ? null : (int?) info.ProductID;
      ticket.ReportedVersionID = info.ReportedID < 0 ? null : (int?) info.ReportedID;
      ticket.SolvedVersionID = info.ResolvedID < 0 ? null : (int?) info.ResolvedID;
      ticket.ProductID = info.ProductID < 0 ? null : (int?) info.ProductID;
      ticket.IsKnowledgeBase = info.IsKnowledgebase;
      ticket.IsVisibleOnPortal = info.IsVisibleOnPortal;
      ticket.ParentID = info.ParentTicketID;
      ticket.Collection.Save();

      if (info.CategoryID != null && info.CategoryID > -1) ticket.AddCommunityTicket((int)info.CategoryID);

      TeamSupport.Data.Action action = (new Actions(ticket.Collection.LoginUser)).AddNewAction();
      action.ActionTypeID = null;
      action.Name = "Description";
      action.SystemActionTypeID = SystemActionType.Description;
      action.ActionSource = ticket.TicketSource;
      action.Description = info.Description;
      action.IsVisibleOnPortal = ticket.IsVisibleOnPortal;
      action.IsKnowledgeBase = ticket.IsKnowledgeBase;
      action.TicketID = ticket.TicketID;
      action.Collection.Save();

      result.Add(ticket.TicketID);
      result.Add(action.ActionID);

      foreach (int ticketID in info.RelatedTickets)
      {
        AddRelated(ticket.TicketID, ticketID, null);
      }

      foreach (int ticketID in info.ChildTickets)
      {
        AddRelated(ticket.TicketID, ticketID, true);
      }

      foreach (int userID in info.Subscribers)
      {
        Subscriptions.AddSubscription(ticket.Collection.LoginUser, userID, ReferenceType.Tickets, ticket.TicketID);
      }

      foreach (int userID in info.Queuers)
      {
        TicketQueue.Enqueue(ticket.Collection.LoginUser, ticket.TicketID, userID);
      }

      foreach (string tag in info.Tags)
      {
        AddTag(ticket.TicketID, tag);
      }

      foreach (int assetID in info.Assets)
      {
        AddTicketAsset(ticket.TicketID, assetID);
      }

      foreach (int id in info.Customers)
      {
        AddTicketCustomer(ticket.TicketID, "o", id);
      }

      foreach (int id in info.Contacts)
      {
        AddTicketCustomer(ticket.TicketID, "u", id);
      }

      foreach (CustomFieldSaveInfo field in info.Fields)
      {
        CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, ticket.TicketID);
        if (field.Value == null)
        {
          customValue.Value = "";
        }
        else
        {
          if (customValue.FieldType == CustomFieldType.DateTime)
          {
            //customValue.Value = ((DateTime)field.Value).ToString();
            DateTime dt;
            if (DateTime.TryParse(((string)field.Value).Replace("UTC", "GMT"), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
            {
              customValue.Value = dt.ToUniversalTime().ToString();              
            }
          }
          else
          {
            customValue.Value = field.Value.ToString();
          }

        }

        customValue.Collection.Save();
      }

      if (info.ChatID != null)
      {

        Chat chat = Chats.GetChat(ticket.Collection.LoginUser, (int)info.ChatID);
        if (chat != null)
        {
          TeamSupport.Data.Action chatAction = (new Actions(ticket.Collection.LoginUser)).AddNewAction();
          chatAction.ActionTypeID = null;
          chatAction.Name = "Chat";
          chatAction.ActionSource = "Chat";
          chatAction.SystemActionTypeID = SystemActionType.Chat;
          chatAction.Description = chat.GetHtml(true, chatAction.Collection.LoginUser.OrganizationCulture);
          chatAction.IsVisibleOnPortal = ticket.IsVisibleOnPortal;
          chatAction.IsKnowledgeBase = ticket.IsKnowledgeBase;
          chatAction.TicketID = ticket.TicketID;
          chatAction.Collection.Save();
          chat.ActionID = chatAction.ActionID;
          chat.Collection.Save();
        }
      }

      Reminders reminders = new Reminders(ticket.Collection.LoginUser);
      foreach (NewTicketReminderInfo reminderInfo in info.Reminders)
      {
        Reminder reminder = reminders.AddNewReminder();
        reminder.OrganizationID = TSAuthentication.OrganizationID;
        reminder.RefID = ticket.TicketID;
        reminder.RefType = ReferenceType.Tickets;
        DateTime dt;
        if (DateTime.TryParse((reminderInfo.DueDate).Replace("UTC", "GMT"), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
        {
          reminder.DueDate = dt.ToUniversalTime();
        }
        reminder.Description = reminderInfo.Description;
        reminder.UserID = reminderInfo.UserID;
      }
      reminders.Save();

      User user = Users.GetUser(ticket.Collection.LoginUser, TSAuthentication.UserID);
      if (user.SubscribeToNewTickets) Subscriptions.AddSubscription(ticket.Collection.LoginUser, TSAuthentication.UserID, ReferenceType.Tickets, ticket.TicketID);


      return result.ToArray();
    }

    [WebMethod]
    public void AddNewTicketCustomer(string first, string last, string email, string company, int? organizationID)
    { 
    
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

  [DataContract]
  public class TicketInfo
  {
    [DataMember] public TicketsViewItemProxy Ticket { get; set; }
    [DataMember] public ActionInfo[] Actions { get; set; }
    [DataMember] public TicketCustomer[] Customers { get; set; }
    [DataMember] public RelatedTicket[] Related { get; set; }
    [DataMember] public TagProxy[] Tags { get; set; }
    [DataMember] public CustomValueProxy[] CustomValues { get; set; }
    [DataMember] public UserInfo[] Subscribers { get; set; }
    [DataMember] public UserInfo[] Queuers { get; set; }
    [DataMember] public ReminderProxy[] Reminders { get; set; }
    [DataMember] public AssetProxy[] Assets { get; set; }
  }

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class NewTicketSaveInfo
  {
    public NewTicketSaveInfo() { }
    [DataMember] public string Name { get; set; }
    [DataMember] public int TicketTypeID { get; set; }
    [DataMember] public int TicketStatusID { get; set; }
    [DataMember] public int TicketSeverityID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public int GroupID { get; set; }
    [DataMember] public int ProductID { get; set; }
    [DataMember] public int ReportedID { get; set; }
    [DataMember] public int ResolvedID { get; set; }
    [DataMember] public bool IsVisibleOnPortal { get; set; }
    [DataMember] public bool IsKnowledgebase { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int? ChatID { get; set; }
    [DataMember] public int? CategoryID { get; set; }
    [DataMember] public int? ParentTicketID { get; set; }
    [DataMember] public List<int> RelatedTickets { get; set; }
    [DataMember] public List<int> ChildTickets { get; set; }
    [DataMember] public List<int> Customers { get; set; }
    [DataMember] public List<int> Contacts { get; set; }
    [DataMember] public List<string> Tags { get; set; }
    [DataMember] public List<CustomFieldSaveInfo> Fields { get; set; }
    [DataMember] public List<int> Subscribers { get; set; }
    [DataMember] public List<int> Queuers { get; set; }
    [DataMember] public List<NewTicketReminderInfo> Reminders { get; set; }
    [DataMember] public List<int> Assets { get; set; }

  }

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class NewTicketReminderInfo
  {
    public NewTicketReminderInfo() { }
    [DataMember] public string Description { get; set; }
    [DataMember] public string DueDate { get; set; }
    [DataMember] public int UserID { get; set; }
  }

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class CustomFieldSaveInfo
  {
    public CustomFieldSaveInfo() { }
    [DataMember] public int CustomFieldID { get; set; }
    [DataMember] public object Value { get; set; }
  }

  [DataContract]
  public class ActionInfo
  {
    [DataMember]
    public UserInfo Creator { get; set; }
    [DataMember]
    public ActionProxy Action { get; set; }
    [DataMember]
    public AttachmentProxy[] Attachments { get; set; }
  }

  [DataContract]
  public class UserInfo
  {
    [DataMember] public int UserID { get; set; }
    [DataMember] public string Email { get; set; }
    [DataMember] public string FirstName { get; set; }
    [DataMember] public string MiddleName { get; set; }
    [DataMember] public string LastName { get; set; }
    [DataMember] public string Title { get; set; }
    [DataMember] public bool IsActive { get; set; }
    [DataMember] public DateTime LastLogin { get; set; }
    [DataMember] public DateTime LastActivity { get; set; }
    [DataMember] public DateTime? LastPing { get; set; }
    [DataMember] public bool IsSystemAdmin { get; set; }
    [DataMember] public bool IsPortalUser { get; set; }
    [DataMember] public bool IsChatUser { get; set; }
    [DataMember] public bool InOffice { get; set; }
    [DataMember] public string InOfficeComment { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Organization { get; set; }

    public UserInfo(UsersViewItem user)
    {
      this.OrganizationID = user.OrganizationID;
      this.Organization = user.Organization;
      this.InOfficeComment = user.InOfficeComment;
      this.InOffice = user.InOffice;
      this.IsChatUser = user.IsChatUser;
      this.IsPortalUser = user.IsPortalUser;
      this.IsSystemAdmin = user.IsSystemAdmin;
      this.IsActive = user.IsActive;
      this.Title = user.Title;
      this.LastName = user.LastName;
      this.MiddleName = user.MiddleName;
      this.FirstName = user.FirstName;
      this.Email = user.Email;
      this.UserID = user.UserID;
      this.LastLogin = DateTime.SpecifyKind(user.LastLogin, DateTimeKind.Local);
      this.LastActivity = DateTime.SpecifyKind(user.LastActivity, DateTimeKind.Local);
      this.LastPing = user.LastPing == null ? user.LastPing : DateTime.SpecifyKind((DateTime)user.LastPing, DateTimeKind.Local); 
    }
  }

  [DataContract]
  public class TicketCustomer
  {
    [DataMember]
    public string Company { get; set; }
    [DataMember]
    public string Contact { get; set; }
    [DataMember]
    public int OrganizationID { get; set; }
    [DataMember]
    public int? UserID { get; set; }
  }

  [DataContract]
  public class RelatedTicket
  {
    [DataMember]
    public int TicketNumber { get; set; }
    [DataMember]
    public int TicketID { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string Status { get; set; }
    [DataMember]
    public string Severity { get; set; }
    [DataMember]
    public string Type { get; set; }
    [DataMember]
    public bool? IsParent { get; set; }
  }



}