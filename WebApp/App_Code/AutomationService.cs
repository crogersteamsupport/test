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
  public class AutomationService : System.Web.Services.WebService
  {
    
    public AutomationService()
    {

      //Uncomment the following line if using designed components 
      //InitializeComponent(); 
    }
    
    [WebMethod(true)]
    public AutomationData GetData()
    {
      
      AutomationData result = new AutomationData();
      TicketAutomationPossibleActions actions = new TicketAutomationPossibleActions(UserSession.LoginUser);
      actions.LoadActive();
      result.Actions = actions.GetTicketAutomationPossibleActionProxies();

      ReportTableFields fields = new ReportTableFields(UserSession.LoginUser);
      fields.LoadByReportTableID(10);
      result.Fields = fields.GetReportTableFieldProxies();


      Users users = new Users(UserSession.LoginUser);
      users.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, true);
      List<AutocompleteItem> userItems = new List<AutocompleteItem>();
      foreach (User user in users)
      {
        userItems.Add(new AutocompleteItem(user.DisplayName, user.UserID.ToString()));
      }
      result.Users = userItems.ToArray();

      Groups groups = new Groups(UserSession.LoginUser);
      groups.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
      List<AutocompleteItem> groupItems = new List<AutocompleteItem>();
      foreach (Group group in groups)
      {
        groupItems.Add(new AutocompleteItem(group.Name, group.GroupID.ToString()));
      }
      result.Groups = groupItems.ToArray();

      TicketSeverities severities = new TicketSeverities(UserSession.LoginUser);
      severities.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
      List<AutocompleteItem> severityItems = new List<AutocompleteItem>();
      foreach (TicketSeverity severity in severities)
      {
        severityItems.Add(new AutocompleteItem(severity.Name, severity.TicketSeverityID.ToString()));
      }
      result.Severities = severityItems.ToArray();

      TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
      ticketTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);
      List<AutocompleteItem> statusItems = new List<AutocompleteItem>();

      foreach (TicketType ticketType in ticketTypes)
      {
        TicketStatuses statuses = new TicketStatuses(UserSession.LoginUser);
        statuses.LoadAllPositions(ticketType.TicketTypeID);

        foreach (TicketStatus status in statuses)
        {
          statusItems.Add(new AutocompleteItem(ticketType.Name + " - " + status.Name, status.TicketStatusID.ToString()));
        }
      }
      result.Statuses = statusItems.ToArray();
      return result;
    }

    [WebMethod(true)]
    public TicketAutomationTriggerProxy[] GetTriggers()
    {
      TicketAutomationTriggers triggers = new TicketAutomationTriggers(UserSession.LoginUser);
      triggers.LoadByOrganization(UserSession.LoginUser.OrganizationID);
      return triggers.GetTicketAutomationTriggerProxies();
    }

    [WebMethod(true)]
    public int CreateTrigger()
    {
      TicketAutomationTrigger trigger = (new TicketAutomationTriggers(UserSession.LoginUser)).AddNewTicketAutomationTrigger();
      trigger.Name = "New Trigger";
      trigger.Active = true;
      trigger.Position = 0;
      trigger.OrganizationID = UserSession.LoginUser.OrganizationID;
      trigger.UseCustomSQL = false;
      trigger.Collection.Save();
      return trigger.TriggerID;
    }

    [WebMethod(true)]
    public TriggerData GetTrigger(int triggerID)
    {
      TicketAutomationTrigger trigger = TicketAutomationTriggers.GetTicketAutomationTrigger(UserSession.LoginUser, triggerID);
      if (trigger.OrganizationID != UserSession.LoginUser.OrganizationID) return null;

      TicketAutomationActions actions = new TicketAutomationActions(UserSession.LoginUser);
      actions.LoadByTrigger(triggerID);

      TicketAutomationTriggerLogic logic = new TicketAutomationTriggerLogic(UserSession.LoginUser);
      logic.LoadByTrigger(triggerID);

      TriggerData result = new TriggerData();
      result.Trigger = trigger.GetProxy();
      result.LogicItems = logic.GetTicketAutomationTriggerLogicItemProxies();
      result.Actions = actions.GetTicketAutomationActionProxies();
      return result;
    }

    [WebMethod(true)]
    public void DeleteTrigger(int triggerID)
    {
      TicketAutomationTrigger trigger = TicketAutomationTriggers.GetTicketAutomationTrigger(UserSession.LoginUser, triggerID);
      if (trigger.OrganizationID != UserSession.LoginUser.OrganizationID || !UserSession.CurrentUser.IsSystemAdmin) return;

      trigger.Delete();
      trigger.Collection.Save();
    }

    [WebMethod(true)]
    public TicketAutomationTriggerProxy SaveTrigger(SaveTriggerData data)
    {
      TicketAutomationTrigger trigger = null;
      if (data.TriggerID > -1)
      {
        trigger = TicketAutomationTriggers.GetTicketAutomationTrigger(UserSession.LoginUser, data.TriggerID);
        if (trigger.OrganizationID != UserSession.LoginUser.OrganizationID || !UserSession.CurrentUser.IsSystemAdmin) return null;
      }
      else
      {
        trigger = (new TicketAutomationTriggers(UserSession.LoginUser)).AddNewTicketAutomationTrigger();
        trigger.OrganizationID = UserSession.LoginUser.OrganizationID;
      }

      trigger.Active = data.IsActive;
      trigger.Name = data.Name;
      trigger.Position = trigger.Collection.GetMaxPosition(UserSession.LoginUser.OrganizationID) + 1;
      trigger.Collection.Save();

      TicketAutomationTriggerLogic logic = new TicketAutomationTriggerLogic(UserSession.LoginUser);
      logic.LoadByTrigger(data.TriggerID);
      foreach (TicketAutomationTriggerLogicItem logicItem in logic)
      {
        logicItem.Delete();
      }
      logic.Save();

      TicketAutomationActions actions = new TicketAutomationActions(UserSession.LoginUser);
      actions.LoadByTrigger(data.TriggerID);
      foreach (TicketAutomationAction action in actions)
      {
        action.Delete();
      }
      actions.Save();

      actions = new TicketAutomationActions(UserSession.LoginUser);
      foreach (TicketAutomationActionProxy actionProxy in data.Actions)
      {
        TicketAutomationAction action = actions.AddNewTicketAutomationAction();
        action.TriggerID = trigger.TriggerID;
        action.ActionValue = actionProxy.ActionValue;
        action.ActionValue2 = actionProxy.ActionValue2;
        action.ActionID = actionProxy.ActionID;
      }
      actions.Save();

      logic = new TicketAutomationTriggerLogic(UserSession.LoginUser);
      foreach (TicketAutomationTriggerLogicItemProxy logicItemProxy in data.LogicItems)
      {
        TicketAutomationTriggerLogicItem logicItem = logic.AddNewTicketAutomationTriggerLogicItem();
        logicItem.TriggerID = trigger.TriggerID;
        logicItem.TableID = logicItemProxy.TableID;
        logicItem.FieldID = logicItemProxy.FieldID;
        logicItem.TestValue = logicItemProxy.TestValue;
        logicItem.Measure = logicItemProxy.Measure;
        logicItem.MatchAll = logicItemProxy.MatchAll;
      }
      logic.Save();

      return trigger.GetProxy();
    }

    [WebMethod]
    public TicketAutomationActionProxy[] GetActionProxies() { return null; }

    [WebMethod]
    public TicketAutomationTriggerLogicItemProxy[] GetLogicProxies() { return null; }
  }

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class TriggerData
  {
    [DataMember]
    public TicketAutomationTriggerProxy Trigger { get; set; }
    [DataMember]
    public TicketAutomationActionProxy[] Actions { get; set; }
    [DataMember]
    public TicketAutomationTriggerLogicItemProxy[] LogicItems { get; set; }
  }

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class SaveTriggerData
  {
    public SaveTriggerData()
    {
    }
    [DataMember]
    public int TriggerID { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public bool IsActive { get; set; }
    [DataMember]
    public List<TicketAutomationActionProxy> Actions { get; set; }
    [DataMember]
    public List<TicketAutomationTriggerLogicItemProxy> LogicItems { get; set; }
  }


  [DataContract(Namespace = "http://teamsupport.com/")]
  public class AutomationData
  {
    [DataMember]
    public TicketAutomationPossibleActionProxy[] Actions { get; set; }
    [DataMember]
    public AutocompleteItem[] Statuses { get; set; }
    [DataMember]
    public AutocompleteItem[] Severities { get; set; }
    [DataMember]
    public AutocompleteItem[] Users { get; set; }
    [DataMember]
    public AutocompleteItem[] Groups { get; set; }
    [DataMember]
    public ReportTableFieldProxy[] Fields { get; set; }
  }

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class LogicItem
  {
    [DataMember]
    public int TriggerLogicID { get; set; }
    [DataMember]
    public int TriggerID { get; set; }
    [DataMember]
    public int TableID { get; set; }
    [DataMember]
    public int FieldID { get; set; }
    [DataMember]
    public string FieldName { get; set; }
    [DataMember]
    public string Measure { get; set; }
    [DataMember]
    public string TestValue { get; set; }
  }
  

}