using System;
using System.Collections.Generic;
using System.Linq;
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
     Assets = 34
  };

  public enum SystemUser
  { 
    Unknown = -1,
    Importer = -2,
    API = -3
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
    TicketSendEmail = 11 //PARAMS: UserID, TicketID, Addresses
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
    PingUpdate = 4,
    Chat = 5
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
    PickList = 4
  }

  public enum ActionLogType
  {
    Insert,
    Update,
    Delete
  }

  public class Enums
	{
    public static Dictionary<string, string> PortalThemeNames = new Dictionary<string, string>
    {
      {"Default","Gray"},
      {"Web20","Blue"},
      {"Hay","Green"},
      {"Sunset","Orange"}
    };

    public static Dictionary<SystemActionType, string> SystemActionNames = new Dictionary<SystemActionType, string>
    {
      {SystemActionType.Custom, null},
      {SystemActionType.Description,"Description"},
      {SystemActionType.Resolution,"Resolution"},
      {SystemActionType.Email,"Email"},
      {SystemActionType.PingUpdate,"Ping Update"},
      {SystemActionType.Chat,"Chat"}
    };

  }



}
