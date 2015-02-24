using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TeamSupport.Data
{
  public enum DataFormat { XML, XHTML, JSON }

  public enum TicketTemplateType { TicketType, PickList }

  public enum ReferenceType
  {  None = -1,
     Actions = 0, 
     ActionTypes = 1, 
     Addresses = 2, 
     Attachments = 3, 
     CustomFields = 4, 
     CustomValues = 5,
     Groups = 6, 
     GroupUsers = 7, 
     OrganizationProducts = 8, 
     Organizations = 9, 
     OrganizationTickets = 10,
     PhoneNumbers = 11, 
     PhoneTypes = 12, 
     Products = 13, 
     ProductVersions = 14, 
     ProductVersionStatuses = 15,
     TechDocs = 16, 
     Tickets = 17, 
     TicketSeverities = 18, 
     TicketStatuses = 19, 
     Subscriptions = 20, 
     TicketTypes = 21, 
     Users = 22, 
     ActionLogs = 23, 
     BillingInfo = 24, 
     ExceptionLogs = 25, 
     Invoices = 26, 
     SystemSettings = 27, 
     TicketNextStatuses = 28, 
     UserSettings = 29, 
     TicketQueue = 30,
     CreditCards = 31,
     Contacts = 32,
     Chat = 33,
     Assets = 34,
     EmailPost = 35,
     ForumCategories = 36,
     UserPhoto = 37,
     WaterCooler = 38,
     Wikis = 39,
     Notes = 40,
     Tags = 41,
     KnowledgeBaseCategories = 42,
     Reports = 43,
     ProductFamilies = 44
  };

  public enum SlaViolationType
  {
    InitialResponse, 
    LastAction, 
    TimeClosed
  }

  public enum SystemUser
  { 
    Unknown = -1,
    Importer = -2,
    API = -3,
    CRM = -4
  }

  public enum TicketRightType
  { 
    All = 0,
    Assigned = 1,
    Groups = 2,
    Customers = 3
  }

  public enum ProductFamiliesRightType
  {
      AllFamilies = 0,
      SomeFamilies = 1
  }

  public enum ChatRequestType
  { 
    External = 0,
    Transfer = 1,
    Invitation = 2
  }

  public enum ChatParticipantType
  { 
    User = 0,
    External = 1
  }

  public enum EmailPostType
  {
    TicketUpdateRequest = 0, //PARAMS: TicketID, UserID
    TicketModified = 1, //PARAMS: 1:TicketID, 2:OldUserID, 3:OldGroupID, 4:OldTicketStatusID, 5:OldTicketSeverityID, 6:ModifiedActionsArray, 7:IsNew=1, 8:TicketUsersArray
    WelcomeNewSignup = 10, //PARAMS: UserID, Password
    WelcomeTSUser = 3, //PARAMS: UserID
    WelcomePortalUser = 4, //PARAMS: UserID
    ResetTSPassword = 5, //PARAMS: UserID, Password
    ResetPortalPassword = 6, //PARAMS: UserID, Password
    ChangedTSPassword = 7, //PARAMS: UserID
    ChangedPortalPassword = 8, //PARAMS: UserID
    InternalSignupNotification = 9, //PARAMS: UserID
    TicketSendEmail = 11, //PARAMS: UserID, TicketID, Addresses
  }
  
  public enum ProductType
  {
    Express = 0,
    HelpDesk = 1,
    Enterprise = 2,
    BugTracking= 3
  }

  public enum SystemActionType
  { 
    Custom = 0,
    Description = 1,
    Resolution = 2,
    Email = 3,
    UpdateRequest = 4,
    Chat = 5,
    Reminder = 6
  }

  public enum CreditCardType
  {
    Unknown,
    Invalid,
    Visa,
    MasterCard,
    Discover,
    AmericanExpress,
    BetaTest
  }

  public enum CustomFieldType
  {
    Text = 0,
    DateTime = 1,
    Boolean = 2,
    Number = 3,
    PickList = 4,
    Date = 5,
    Time = 6
  }

  public enum ActionLogType
  {
    Insert,
    Update,
    Delete
  }

  public enum IntegrationType
  {
      SalesForce,
      Highrise,
      Batchbook,
      FreshBooks,
      MailChimp,
      ZohoCRM,
      ZohoReports,
      Jira,
      Oracle
  }

  public enum ReportType
  {
    Table = 0,
    Chart = 1,
    External = 2,
    Custom = 3,
    Summary = 4,
    TicketView = 5,
  }

  public enum ReportTypeOld { 
      Standard = 0,
      Custom,
      Favorite,
      Graphical
  }

  public enum ReferenceProcess
  {
    PushGridPointSalesOrders = 1
  }

  public class Enums
	{
    public static Dictionary<string, string> PortalThemeNames = new Dictionary<string, string>
    {
      {"Default","Gray"},
      {"Web20","Blue"},
      {"Hay","Green"},
      {"Sunset","Orange"},
      {"Beta","Beta"}
    };

    public static Dictionary<SystemActionType, string> SystemActionNames = new Dictionary<SystemActionType, string>
    {
      {SystemActionType.Custom, null},
      {SystemActionType.Description,"Description"},
      {SystemActionType.Resolution,"Resolution"},
      {SystemActionType.Email,"Email"},
      {SystemActionType.UpdateRequest,"Update Request"},
      {SystemActionType.Chat,"Chat"},
      {SystemActionType.Reminder, "Reminder"}
    };

    public static string GetDescription(Enum input)
    {
      FieldInfo fi = input.GetType().GetField(input.ToString());
      DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
      string result;
      if (attributes.Length > 0)
      {
        result = attributes[0].Description;
      }
      else
      {
        result = input.ToString();
      }
      return result;
    }
  }

  public enum WaterCoolerAttachmentType
  {
      Ticket = 0,
      Product,
      Company,
      User,
      Group
  }

  public enum FontFamily
  {
    Unassigned      = 0,
    [Description("Andale Mono")]
    AndaleMono      = 1,
    Arial           = 2,
    [Description("Arial Black")]
    ArialBlack      = 3,
    [Description("Book Antiqua")]
    BookAntiqua     = 4,
    [Description("Comic Sans MS")]
    ComicSansMS     = 5,
    [Description("Courier New ")]
    CourierNew      = 6,
    Georgia         = 7,
    Helvetica       = 8,
    Impact          = 9,
    Symbol          = 10,
    Tahoma          = 11,
    Terminal        = 12,
    [Description("Times New Roman")]
    TimesNewRoman   = 13,
    [Description("Trebuchet MS")]
    TrebuchetMS     = 14,
    Verdana         = 15,
    Webdings        = 16,
    Wingdings       = 17
  }

  public enum FontSize
  {
    Unassigned  = 0,
    [Description("8pt")]
    One8pt      = 1,
    [Description("10pt")]
    Two10pt     = 2,
    [Description("12pt")]
    Three12pt   = 3,
    [Description("14pt")]
    Four14pt    = 4,
    [Description("18pt")]
    Five18pt    = 5,
    [Description("24pt")]
    Six24pt     = 6,
    [Description("36pt")]
    Seven36pt   = 7
  }
}
