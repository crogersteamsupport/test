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
using System.Globalization;
using System.Security.Cryptography;

namespace TSWebServices
{
  [ScriptService]
  [WebService(Namespace = "http://teamsupport.com/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class TSSystem : System.Web.Services.WebService
  {

    public TSSystem()
    {

      //Uncomment the following line if using designed components 
      //InitializeComponent(); 
    }

    [WebMethod]
    public UserProxy GetCurrentUser()
    {
      return TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).GetProxy();
    }

    [WebMethod]
    public string GetAppDomain()
    {
      return Settings.SystemDB.ReadString("AppDomain", "https://app.teamsupport.com");
    }


    [WebMethod]
    public OrganizationProxy GetCurrentOrganization()
    {
      return TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).GetProxy();
    }

    [WebMethod]
    public Culture GetCulture()
    {
      Culture result = new Culture();
      CultureInfo culture = TSAuthentication.GetLoginUser().CultureInfo;
      result.DateTimePattern.LongDate = culture.DateTimeFormat.LongDatePattern;
      result.DateTimePattern.LongDateLongTime = culture.DateTimeFormat.FullDateTimePattern;
      result.DateTimePattern.LongDateShortTime = culture.DateTimeFormat.LongDatePattern + " " + culture.DateTimeFormat.ShortTimePattern;
      result.DateTimePattern.LongTime = culture.DateTimeFormat.LongTimePattern;
      result.DateTimePattern.ShortDate = culture.DateTimeFormat.ShortDatePattern;
      result.DateTimePattern.ShortDateLongTime = culture.DateTimeFormat.ShortDatePattern + " " + culture.DateTimeFormat.LongTimePattern;
      result.DateTimePattern.ShortDateShortTime = culture.DateTimeFormat.ShortDatePattern + " " + culture.DateTimeFormat.ShortTimePattern;
      result.DateTimePattern.ShortTime = culture.DateTimeFormat.ShortTimePattern;
      return result;
    }
  
    [WebMethod]
    public ChatUserSettingProxy GetCurrentUserChatSettings()
    {
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      ChatUserSetting setting = ChatUserSettings.GetChatUserSetting(loginUser, TSAuthentication.UserID);
      if (setting == null)
      {
        setting = (new ChatUserSettings(loginUser)).AddNewChatUserSetting();
        setting.UserID = TSAuthentication.UserID;
      }
      return setting.GetProxy();
    }

    [WebMethod(true)]
    public void SignOut()
    {
      HttpContext.Current.Response.Cookies["rememberme"].Value = null;
      HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName].Value = null;
      HttpContext.Current.Session.Clear();
      HttpContext.Current.Session.Abandon();
      FormsAuthentication.SignOut();
    }

    [WebMethod]
    public TsMenuItem[] GetMainMenuItems()
    {
      List<TsMenuItem> items = new List<TsMenuItem>();
      LoginUser loginUser = TSAuthentication.GetLoginUser();

      Organization org = TSAuthentication.GetOrganization(loginUser);
      User user = TSAuthentication.GetUser(loginUser);
      string data = @"{{""ContentUrl"":""{0}"",""PaneInfoUrl"":""{1}""}}";

      if (org.ParentID != null)
      {

        items.Add(new TsMenuItem("dashboard", "mniDashboard", "Dashboard", "vcr/140/images/nav/16/Dashboard.png", string.Format(data, "Frames/Dashboard.aspx", "vcr/140/PaneInfo/Dashboard.html")));

        items.Add(new TsMenuItem("mytickets", "mniMyTickets", "My Tickets", "vcr/140/images/nav/16/mytickets.png", string.Format(data, "vcr/140/Pages/TicketTabs.html?UserID=" + TSAuthentication.UserID, "vcr/140/PaneInfo/MyTickets.html")));
        items.Add(new TsMenuItem("tickettags", "mniTicketTags", "Ticket Tags", "vcr/140/images/nav/16/tag.png", string.Format(data, "Frames/TicketTags.aspx", "vcr/140/PaneInfo/TicketTags.html")));

        TsMenuItem ticketItem = new TsMenuItem("tickets", "mniTickets", "All Tickets", "vcr/140/images/nav/16/tickets.png", string.Format(data, "vcr/140/Pages/TicketTabs.html", "vcr/140/PaneInfo/Tickets.html"));
        items.Add(ticketItem);

        TicketTypes ticketTypes = new TicketTypes(loginUser);
        ticketTypes.LoadByOrganizationID(TSAuthentication.OrganizationID, org.ProductType);
        foreach (TicketType ticketType in ticketTypes)
        {
          ticketItem.AddItem(new TsMenuItem("tickettype", "mniTicketType_" + ticketType.TicketTypeID.ToString(), ticketType.Name, ticketType.IconUrl, string.Format(data, "vcr/140/Pages/TicketTabs.html?TicketTypeID=" + ticketType.TicketTypeID.ToString(), "vcr/140/PaneInfo/Tickets.html")));
        }

        items.Add(new TsMenuItem("kb", "mniKB", "Knowledge Base", "vcr/140/images/nav/16/knowledge.png", string.Format(data, "Frames/KnowledgeBase.aspx", "vcr/140/PaneInfo/Knowledge.html")));

        if (org.UseForums == true && TSAuthentication.UserID == 34)
        {
          items.Add(new TsMenuItem("forum", "mniForum", "Community", "vcr/140/images/nav/16/forum.png", string.Format(data, "vcr/140/Pages/Tickets.html", "vcr/140/PaneInfo/Forum.html")));
        }

        if (org.ProductType != ProductType.Express)
        {
          int? articleID = org.DefaultWikiArticleID;
          string wikiLink = articleID == null ? "Wiki/ViewPage.aspx" : "Wiki/ViewPage.aspx?ArticleID=" + articleID;
          items.Add(new TsMenuItem("wiki", "mniWiki", "Wiki", "vcr/140/images/nav/16/wiki.png", string.Format(data, wikiLink, "vcr/140/PaneInfo/Wiki.html")));
        }

        items.Add(new TsMenuItem("search", "mniSearch", "Search", "vcr/140/images/nav/16/search.png", string.Format(data, "Frames/Search.aspx", "vcr/140/PaneInfo/Search.html")));

        if (user.IsChatUser && org.ChatSeats > 0)
        {
          items.Add(new TsMenuItem("chat", "mniChat", "Chat", "vcr/140/images/nav/16/chat.png", string.Format(data, "Frames/Chat.aspx", "vcr/140/PaneInfo/Chat.html")));
        }

        if (org.ProductType != ProductType.Express)
          items.Add(new TsMenuItem("wc", "mniWC", "Water Cooler", "vcr/140/images/nav/16/watercooler.png", string.Format(data, "WaterCooler/WaterCooler.aspx", "vcr/140/PaneInfo/WaterCooler.html")));
        items.Add(new TsMenuItem("users", "mniUsers", "Users", "vcr/140/images/nav/16/users.png", string.Format(data, "Frames/Users.aspx", "vcr/140/PaneInfo/Users.html")));
        items.Add(new TsMenuItem("groups", "mniGroups", "Groups", "vcr/140/images/nav/16/groups.png", string.Format(data, "Frames/Groups.aspx", "vcr/140/PaneInfo/Groups.html")));
        if (org.ProductType == ProductType.Enterprise || org.ProductType == ProductType.HelpDesk)
          items.Add(new TsMenuItem("customers", "mniCustomers", "Customers", "vcr/140/images/nav/16/customers.png", string.Format(data, "Frames/Organizations.aspx", "vcr/140/PaneInfo/Customers.html")));
        if (org.ProductType == ProductType.Enterprise || org.ProductType == ProductType.BugTracking)
          items.Add(new TsMenuItem("products", "mniProducts", "Products", "vcr/140/images/nav/16/products.png", string.Format(data, "Frames/Products.aspx", "vcr/140/PaneInfo/Products.html")));
        if (org.IsInventoryEnabled)
          items.Add(new TsMenuItem("inventory", "mniInventory", "Inventory", "vcr/140/images/nav/16/inventory.png", string.Format(data, "Inventory/Inventory.aspx", "vcr/140/PaneInfo/Inventory.html")));

        if (user.IsSystemAdmin || !org.AdminOnlyReports)
          items.Add(new TsMenuItem("reports", "mniReports", "Reports", "vcr/140/images/nav/16/reports.png", string.Format(data, "Frames/Reports.aspx", "vcr/140/PaneInfo/Reports.html")));
        if (user.IsSystemAdmin)
          items.Add(new TsMenuItem("admin", "mniAdmin", "Admin", "vcr/140/images/nav/16/admin.png", string.Format(data, "Frames/Admin.aspx", "vcr/140/PaneInfo/Admin.html")));
        if (TSAuthentication.OrganizationID == 1078 || TSAuthentication.UserID == 84)
        {
          TsMenuItem utils = new TsMenuItem("utils", "mniUtils", "Utilities", "vcr/140/images/nav/16/iis.png", string.Format(data, "vcr/140/Pages/Utils.html", "vcr/140/PaneInfo/Admin.html"));
          items.Add(utils);
          utils.AddItem(new TsMenuItem("utils", "utils-tickets", "Tickets", "vcr/140/images/nav/16/Tickets.png", string.Format(data, "vcr/140/Pages/Utils_Tickets.html", "vcr/140/PaneInfo/Admin.html")));
          utils.AddItem(new TsMenuItem("utils", "utils-organizations", "Organizations", "vcr/140/images/nav/16/Customers.png", string.Format(data, "vcr/140/Pages/Utils_Organizations.html", "vcr/140/PaneInfo/Admin.html")));
          utils.AddItem(new TsMenuItem("utils", "utils-exceptions", "Exceptions", "vcr/140/images/nav/16/close_2.png", string.Format(data, "vcr/140/Pages/Utils_Exceptions.html", "vcr/140/PaneInfo/Admin.html")));
          utils.AddItem(new TsMenuItem("utils", "utils-services", "Services", "vcr/140/images/nav/16/close_2.png", string.Format(data, "vcr/140/Pages/Utils_Services.html", "vcr/140/PaneInfo/Admin.html")));
          utils.AddItem(new TsMenuItem("utils", "utils-sanitizer", "Sanitizer", "vcr/140/images/nav/16/close_2.png", string.Format(data, "vcr/140/Pages/Utils_Sanitizer.html", "vcr/140/PaneInfo/Admin.html")));
        }
      }
      else
      {
        items.Add(new TsMenuItem("tsusers", "mniUsers", "System Users", "vcr/140/images/nav/16/users.png", string.Format(data, "Frames/Users.aspx", "vcr/140/PaneInfo/Users.html")));
        items.Add(new TsMenuItem("tscustomers", "mniCustomers", "System Customers", "vcr/140/images/nav/16/customers.png", string.Format(data, "Frames/Organizations.aspx", "vcr/140/PaneInfo/Organizations.html")));
      }
      return items.ToArray();
    }

    [WebMethod]
    public TsTabDataItem[] GetMainTabs()
    {
      List<TsTabDataItem> list = Settings.UserDB.ReadJson<List<TsTabDataItem>>("main-tabs");
      if (list == null) return null;
      return list.ToArray();
    }

    [WebMethod]
    public ServiceProxy[] GetServices()
    {
      if (TSAuthentication.OrganizationID != 1078) return null;
      Services services = new Services(TSAuthentication.GetLoginUser());
      services.LoadAll();
      return services.GetServiceProxies();
    }

    [WebMethod]
    public void SetMainTabs(TsTabDataItem[] tabs)
    {
      List<TsTabDataItem> list = new List<TsTabDataItem>(tabs);
      Settings.UserDB.WriteJson<List<TsTabDataItem>>("main-tabs", list);
    }

    [WebMethod]
    public void UpdateLastWaterCoolerID()
    {
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      User user = Users.GetUser(loginUser, TSAuthentication.UserID);
      user.LastWaterCoolerID = WaterCooler.GetLastMessageID(loginUser);
      user.Collection.Save();
    }

    [WebMethod]
    public TsMainPageUpdate GetMainPageUpdate(int lastChatMessageID, int lastChatRequestID, int lastWCMessageID, string sessionID)
    {
      if (sessionID == null) sessionID = "<NULL>";
      
      int userID = TSAuthentication.UserID;
      TsMainPageUpdate update = new TsMainPageUpdate();
      try
      {
        update.IsDebug = TSAuthentication.OrganizationID == 1078 || TSAuthentication.IsBackdoor;
        User user = Users.GetUser(LoginUser.Anonymous, userID);
        LoginUser loginUser = new LoginUser(userID, user.OrganizationID, null);


        user.UpdatePing();

        if (TSAuthentication.Ticket.Expired && !TSAuthentication.IsBackdoor)
        {
          user.SessionID = null;
          user.Collection.Save();
        }

        bool isSessionValid = (sessionID == TSAuthentication.SessionID) || TSAuthentication.IsBackdoor;
        if (!isSessionValid && !TSAuthentication.Ticket.Expired)
        {
          ExceptionLogs.AddLog(loginUser, "Session Expired in GetMainPageUpdate (SessionID = " + sessionID + ")");
          FormsAuthentication.SignOut();
          update.IsExpired = true;
          return update;
        }

        List<GrowlMessage> wcGrowl = new List<GrowlMessage>();
        List<GrowlMessage> chatMessageGrowl = new List<GrowlMessage>();
        List<GrowlMessage> chatRequestGrowl = new List<GrowlMessage>();

        update.RefreshID = int.Parse(SystemSettings.ReadString(loginUser, "RefreshID", "-1"));
        update.IsExpired = !isSessionValid || TSAuthentication.Ticket.Expired;
        update.ExpireTime = TSAuthentication.Ticket.Expiration.ToShortTimeString();
        update.Version = System.Web.Configuration.WebConfigurationManager.AppSettings["Version"];
        update.IsIdle = user.DateToUtc(user.LastActivity).AddMinutes(20) < DateTime.UtcNow;
        update.LastChatRequestID = lastChatRequestID;
        update.LastChatMessageID = lastChatMessageID;
        update.LastWCMessageID = lastWCMessageID;
        update.WCMessageCount = 0;
        update.ChatMessageCount = 0;
        update.ChatRequestCount = 0;



        int wcID = WaterCooler.GetLastMessageID(loginUser);
        update.LastWCMessageID = wcID;
        WaterCooler waterCooler = new WaterCooler(loginUser);

        if (lastWCMessageID < 0)
        {
          lastWCMessageID = user.LastWaterCoolerID;
        }
        else
        {
          waterCooler.LoadUnreadMessages(user, lastWCMessageID);
          foreach (WaterCoolerItem wcItem in waterCooler)
          {
            wcGrowl.Add(new GrowlMessage(wcItem.Message, "Water Cooler", "ui-state-active"));
          }
        }


        update.WCMessageCount = WaterCooler.LoadUnreadMessageCount(loginUser, user, user.LastWaterCoolerID);


        if (user.IsChatUser && ChatUserSettings.GetSetting(loginUser, user.UserID).IsAvailable)
        {
          ChatMessages chatMessages = new ChatMessages(loginUser);
          update.ChatMessageCount = chatMessages.GetUnreadMessageCount(loginUser.UserID, ChatParticipantType.User);
          update.ChatRequestCount = user.IsChatUser ? ChatRequests.GetWaitingRequestCount(loginUser, loginUser.UserID, loginUser.OrganizationID) : 0;

          if (lastChatMessageID < 0) update.LastChatMessageID = ChatMessages.GetLastMessageID(loginUser);
          chatMessages.LoadUnpreviewedMessages(loginUser.UserID, update.LastChatMessageID);

          foreach (ChatMessage chatMessage in chatMessages)
          {
            chatMessageGrowl.Add(new GrowlMessage(chatMessage.Message, chatMessage.PosterName, "ui-state-active"));
            update.LastChatMessageID = chatMessage.ChatMessageID;
          }

          if (ChatUserSettings.GetSetting(loginUser, user.UserID).IsAvailable)
          {
            ChatRequests requests = new ChatRequests(loginUser);
            requests.LoadNewWaitingRequests(loginUser.UserID, loginUser.OrganizationID, lastChatRequestID);

            foreach (ChatRequest chatRequest in requests)
            {
              chatRequestGrowl.Add(new GrowlMessage(string.Format("{0} {1} is requesting a chat!", chatRequest.Row["FirstName"].ToString(), chatRequest.Row["LastName"].ToString()), "Chat Request", "ui-state-error"));
              update.LastChatRequestID = chatRequest.ChatRequestID;
            }
          }
        }

        update.NewWCMessages = wcGrowl.ToArray();
        update.NewChatMessages = chatMessageGrowl.ToArray();
        update.NewChatRequests = chatRequestGrowl.ToArray();
      }
      catch (Exception ex)
      {
        /*FormsAuthentication.SignOut();
        update.RefreshID = -1;
        update.IsExpired = true;*/
        ex.Data["SessionID"] = sessionID;
        ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Main Page Upate");
      }
      return update;

    }

    [WebMethod(false)]
    public void UpdateLastActivity()
    {
      TSAuthentication.SlideExpiration();

      User user = Users.GetUser(LoginUser.Anonymous, TSAuthentication.UserID);
      user.LastActivity = DateTime.UtcNow;
      user.Collection.Save();
    }

    [WebMethod]
    public AutocompleteItem[] GetLookupValues(int reportTableFieldID, string term)
    {
      List<AutocompleteItem> result = new List<AutocompleteItem>();
      Dictionary<int, string> values = LookupField.GetValues(TSAuthentication.GetLoginUser(), reportTableFieldID, term, 10);
      result.Add(new AutocompleteItem("Unassigned", "-1"));
      if (values != null)
      {
        foreach (KeyValuePair<int, string> pair in values)
        {
          result.Add(new AutocompleteItem(pair.Value, pair.Key.ToString()));
        }
      }
      return result.ToArray();
    }

    [WebMethod]
    public AutocompleteItem[] GetLookupDisplayNames(int reportTableFieldID, string term)
    {
      List<AutocompleteItem> result = new List<AutocompleteItem>();
      Dictionary<int, string> values = LookupField.GetValues(TSAuthentication.GetLoginUser(), reportTableFieldID, term, 10);
      result.Add(new AutocompleteItem("Unassigned", "-1"));
      if (values != null)
      {
        foreach (KeyValuePair<int, string> pair in values)
        {
          bool found = false;
          foreach (AutocompleteItem item in result)
          {
            if (item.label.ToLower().Trim() == pair.Value.ToLower().Trim())
            {
              found = true;
              break;
            }
          }

          if (!found) result.Add(new AutocompleteItem(pair.Value, pair.Key.ToString()));
        }
      }
      return result.ToArray();
    }

    [WebMethod]
    public int GetCheckSum(ReferenceType refType)
    {
      return CheckSums.GetCheckSum(TSAuthentication.GetLoginUser(), refType);
    }

    [WebMethod]
    public ReminderProxy EditReminder(int? reminderID, ReferenceType refType, int refID, string description, DateTime dueDate, int userID)
    {
      Reminder reminder;
      if (reminderID == null)
      { 
        reminder = (new Reminders(TSAuthentication.GetLoginUser())).AddNewReminder();
        reminder.OrganizationID = TSAuthentication.OrganizationID;
      }
      else
      {
        reminder = Reminders.GetReminder(TSAuthentication.GetLoginUser(), (int)reminderID);
        if (reminder.OrganizationID != TSAuthentication.OrganizationID) return null;
      }

      User user = Users.GetUser(reminder.Collection.LoginUser, userID);
      if (user.OrganizationID != TSAuthentication.OrganizationID) return null;

      reminder.Description = description;
      reminder.RefType = refType;
      reminder.RefID = refID;
      reminder.DueDate = dueDate;
      reminder.UserID = userID;
      reminder.HasEmailSent = false;
      reminder.Collection.Save();
      return reminder.GetProxy();
    }

    [WebMethod]
    public void DismissReminder(int reminderID)
    {
      Reminder reminder = Reminders.GetReminder(TSAuthentication.GetLoginUser(), (int)reminderID);
      if (reminder.OrganizationID != TSAuthentication.OrganizationID) return;

      if (!TSAuthentication.IsSystemAdmin && reminder.UserID != TSAuthentication.UserID) return;
      reminder.IsDismissed = true;
      reminder.Collection.Save();
    }


    [WebMethod]
    public ReminderProxy GetReminder(int reminderID)
    {
      Reminder reminder = Reminders.GetReminder(TSAuthentication.GetLoginUser(), (int)reminderID);
      if (reminder.OrganizationID != TSAuthentication.OrganizationID) return null;
      return reminder.GetProxy();
    }

    [WebMethod]
    public ReminderProxy[] GetItemReminders(ReferenceType refType, int refID, int? userID)
    {
      Reminders reminders = new Reminders(TSAuthentication.GetLoginUser());
      reminders.LoadByItem(refType, refID, userID);
      return reminders.GetReminderProxies();
    }

    [WebMethod]
    public ReminderProxy[] GetUserReminders(int userID)
    {
      Reminders reminders = new Reminders(TSAuthentication.GetLoginUser());
      reminders.LoadByUser(userID);
      return reminders.GetReminderProxies();
    }

    [WebMethod]
    public string[] GenerateKeys()
    {
      List<string> result = new List<string>();
      result.Add(string.Format("<strong>validationKey (SHA1): </strong>{0}", DataUtils.GenerateRandomKey(128)));
      result.Add(string.Format("<strong>decryptionKey (AES): </strong>{0}", DataUtils.GenerateRandomKey(64)));
      return result.ToArray();
    }

    [WebMethod]
    public ExceptionLogViewItemProxy GetException(int exceptionLogID)
    {
      ExceptionLogViewItem item = ExceptionLogView.GetExceptionLogViewItem(TSAuthentication.GetLoginUser(), exceptionLogID);
      return item.GetProxy();
    }

    [WebMethod]
    public ExceptionLogViewItemProxy[] GetTopExceptions()
    {
      ExceptionLogView view = new ExceptionLogView(TSAuthentication.GetLoginUser());
      view.LoadTop(100);
      return view.GetExceptionLogViewItemProxies();
    }

    [WebMethod]
    public void ClearExceptionLogs()
    {
      ExceptionLogs.DeleteAll();
    }

    [WebMethod]
    public int GetExceptionLogCount()
    {
      return ExceptionLogs.GetCount();
    }

    [WebMethod]
    public void CreateNewException()
    {
      try
      {
        throw new Exception("This is a test exception.");
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "TSSystem", "Extra Info");
      }
    }

    [WebMethod]
    public string SanitizeHtml(string text)
    {
      return HtmlUtility.Sanitize(text);
    }

    [WebMethod]
    public CustomValueProxy SaveCustomValue(int customFieldID, int refID, object value)
    {
      CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), customFieldID, refID);
      if (value == null)
      {
        customValue.Value = "";
        customValue.Collection.Save();
        return null;
      }

      if (customValue.FieldType == CustomFieldType.DateTime)
      {
        customValue.Value = ((DateTime)value).ToString();
      }
      else
      {
        customValue.Value = value.ToString();
      }

      customValue.Collection.Save();
      return customValue.GetProxy();
    }

  }

  [Serializable]
  public class TsMenuItem
  {
    List<TsMenuItem> _items;
    public TsMenuItem(string type, string id, string caption, string imageUrl, string data)
    {
      _items = new List<TsMenuItem>();
      ID = id;
      Type = type;
      Caption = caption;
      ImageUrl = imageUrl;
      Data = data;
    }
    public string ID { get; set; }
    public string Type { get; set; }
    public string Caption { get; set; }
    public string ImageUrl { get; set; }
    public string Data { get; set; }
    public TsMenuItem[] Items { get { return _items.ToArray(); } }
    public void AddItem(TsMenuItem item) { _items.Add(item); }
  }

  [DataContract]
  public class TsTabDataItem
  {
    [DataMember]
    public string ID { get; set; }
    [DataMember]
    public string TabType { get; set; }
    [DataMember]
    public string Caption { get; set; }
  }

  [DataContract]
  public class GrowlMessage
  {
    public GrowlMessage(string message, string title, string state)
    {
      Message = message;
      Title = title;
      State = state;
    }
    [DataMember]
    public string Message { get; set; }
    [DataMember]
    public string Title { get; set; }
    [DataMember]
    public string State { get; set; }
  }

  [DataContract]
  public class TsMainPageUpdate
  {
    [DataMember]
    public bool IsExpired { get; set; }
    [DataMember]
    public string ExpireTime { get; set; }
    [DataMember]
    public string Version { get; set; }
    [DataMember]
    public int RefreshID { get; set; }
    [DataMember]
    public bool IsIdle { get; set; }
    [DataMember]
    public bool IsDebug { get; set; }
    [DataMember]
    public int WCMessageCount { get; set; }
    [DataMember]
    public int ChatMessageCount { get; set; }
    [DataMember]
    public int ChatRequestCount { get; set; }
    [DataMember]
    public GrowlMessage[] NewChatMessages { get; set; }
    [DataMember]
    public GrowlMessage[] NewChatRequests { get; set; }
    [DataMember]
    public GrowlMessage[] NewWCMessages { get; set; }
    [DataMember]
    public int LastWCMessageID { get; set; }
    [DataMember]
    public int LastChatMessageID { get; set; }
    [DataMember]
    public int LastChatRequestID { get; set; }
  }


  [DataContract]
  public class AutocompleteItem
  {
    public AutocompleteItem() { }

    public AutocompleteItem(string label, string id)
    {
      this.label = label;
      this.value = label;
      this.id = id;
    }

    public AutocompleteItem(string label, string id, object data)
    {
      this.label = label;
      this.value = label;
      this.id = id;
      this.data = data;
    }

    [DataMember]
    public string label { get; set; }
    [DataMember]
    public string value { get; set; }
    [DataMember]
    public string id { get; set; }
    [DataMember]
    public object data { get; set; }
  }

  [DataContract]
  public class IDList
  {
    public IDList() { }
    [DataMember]
    public List<int> List { get; set; }
  }
  [DataContract]
  public class Culture
  {
    public Culture()
    {
      DateTimePattern = new DateTimePattern();
    }

    [DataMember]
    public DateTimePattern DateTimePattern { get; set; }

  }

  [DataContract]
  public class DateTimePattern
  {
    [DataMember] 
    public string ShortDate { get; set; }
    [DataMember] 
    public string LongDate { get; set; }
    [DataMember] 
    public string ShortTime { get; set; }
    [DataMember]
    public string LongTime { get; set; }
    [DataMember]
    public string ShortDateLongTime { get; set; }
    [DataMember]
    public string ShortDateShortTime { get; set; }
    [DataMember]
    public string LongDateShortTime { get; set; }
    [DataMember]
    public string LongDateLongTime { get; set; }
  }

}