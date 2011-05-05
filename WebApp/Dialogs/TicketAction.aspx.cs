using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;
using System.Text;
using System.ServiceModel;
using System.Web.Services;
using System.IO;
using System.Runtime.Serialization;

public partial class Dialogs_TicketAction : System.Web.UI.Page
{
  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);
    if (!IsPostBack)
    {
      LoadActionTypes();
    }

    
  }

  protected void Page_Load(object sender, EventArgs e)
  {

    dtpStarted.Culture = UserSession.LoginUser.CultureInfo;
    if (Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID).TimedActionsRequired) fieldRequireTime.Value = "1";
  }

  private void LoadActionTypes()
  {
    ActionTypes actionTypes = new ActionTypes(UserSession.LoginUser);
    actionTypes.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
    cmbActionType.Items.Clear();

    foreach (ActionType actionType in actionTypes)
    {
      cmbActionType.Items.Add(new RadComboBoxItem(actionType.Name, actionType.ActionTypeID.ToString()));
    }
    cmbActionType.Items.Add(new RadComboBoxItem("Resolution", "-1"));
    if (cmbActionType.Items.Count > 0) cmbActionType.SelectedIndex = 0;
  }


  [WebMethod(true)]
  public static ActionProxy GetAction(int actionID)
  {
    if (actionID < 0) return null;
    TeamSupport.Data.Action action = Actions.GetAction(UserSession.LoginUser, actionID);
    Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, action.TicketID);
    if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
    ActionProxy proxy = action.GetProxy();
    proxy.Description = Clean(proxy.Description);
    return proxy;
  }

  [WebMethod(true)]
  public static string Clean(string txt)
  {
    //return HtmlUtility.RemoveInvalidHtmlTags(txt);
    return HtmlCleaner.CleanHtml(txt);
  }

  [WebMethod(true)]
  public static ActionTypeProxy GetActionType(int? actionTypeID)
  {
    if (actionTypeID == null) return null;
    ActionType actionType = ActionTypes.GetActionType(UserSession.LoginUser, (int)actionTypeID);
    return actionType.GetProxy();
  }
  protected void btnSave_Click(object sender, EventArgs e)
  {
    int actionID = int.Parse(fieldActionID.Value);
    int ticketID = int.Parse(fieldTicketID.Value);
    Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, ticketID);
    if (ticket == null)
    {
      string error = string.Format("Invalid ticket ID: {0}.  Please close the action dialog and try again.", ticketID.ToString());
      RadAjaxManager1.Alert(error);
      ExceptionLogs.AddLog(UserSession.LoginUser, error);
      return;
    }

    if (ticket.OrganizationID != UserSession.LoginUser.OrganizationID) return;

    if (!ticket.IsVisibleOnPortal && cbPortal.Checked)
    {
      RadAjaxManager1.Alert("This ticket has not been marked 'Visible to Customers' so no updates will be sent.  If you would like customers to receive notifications, please check the 'Visible to Customers' checkbox in the Ticket Properties section.");
    }

    if (uploadMain.UploadedFiles.Count > 0)
    {
      int used = Organizations.GetStorageUsed(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
      int allowed = Organizations.GetTotalStorageAllowed(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);

      if (used > allowed)
      {
        if (UserSession.CurrentUser.IsFinanceAdmin)
        {
          RadAjaxManager1.Alert("You have exceeded your allocated storage capacity.  If you would like to add additional storage, please contact our sales team at 800.596.2820 x806, or send an email to sales@teamsupport.com");
        }
        else
        {
          Users users = new Users(UserSession.LoginUser);
          users.LoadFinanceAdmins(UserSession.LoginUser.OrganizationID);
          if (users.IsEmpty)
          {
            RadAjaxManager1.Alert("Please ask your billing administrator to purchase additional storage to add additional attachments.");
          }
          else
          {
            RadAjaxManager1.Alert("Please ask your billing administrator (" + users[0].FirstLastName + ") to purchase additional storage to add additional attachments.");
          }
        }


        return;
      }
    }



    TeamSupport.Data.Action action;

    if (actionID < 0)
    {
      action = (new Actions(UserSession.LoginUser)).AddNewAction();

      int typeID = int.Parse(cmbActionType.SelectedValue);

      if (typeID < 0)
      {
        action.ActionTypeID = null;
        action.SystemActionTypeID = SystemActionType.Resolution;
        action.Name = "Resolution";
      }
      else
      {
        action.ActionTypeID = typeID;
        action.SystemActionTypeID = SystemActionType.Custom;
        action.Name = textName.Text;
      }

      action.Description = editorDescription.Content;
      action.IsVisibleOnPortal = cbPortal.Checked;
      action.IsKnowledgeBase = cbKnowledge.Checked;
      action.TicketID = ticketID;

      action.TimeSpent = numSpent.Value == null ? null : (int?)numSpent.Value;
      action.DateStarted = DataUtils.DateToUtc(UserSession.LoginUser, dtpStarted.SelectedDate);

      if (action.ActionTypeID == null && action.SystemActionTypeID == 0)
      {
        RadAjaxManager1.Alert("You have selected an invalid Action Type.");
        return;
      }

      action.Collection.Save();
    }
    else
    {
       action = Actions.GetAction(UserSession.LoginUser, actionID);
       if (action == null || action.TicketID != ticketID)
       {
         RadAjaxManager1.Alert("There was an error updating the action.");
         return;
       }

       int typeID = int.Parse(cmbActionType.SelectedValue);

       if (action.SystemActionTypeID != SystemActionType.Description)
       {
         if (typeID < 0)
         {
           action.ActionTypeID = null;
           action.SystemActionTypeID = SystemActionType.Resolution;
           action.Name = "Resolution";
         }
         else
         {
           action.ActionTypeID = typeID;
           action.SystemActionTypeID = SystemActionType.Custom;
           action.Name = textName.Text;
         }
       }

       action.IsVisibleOnPortal = cbPortal.Checked;
       action.IsKnowledgeBase = cbKnowledge.Checked;
       action.Description = editorDescription.Content;
       action.TimeSpent = numSpent.Value == null ? null : (int?)numSpent.Value;
       action.DateStarted = DataUtils.DateToUtc(UserSession.LoginUser, dtpStarted.SelectedDate);

       if (action.ActionTypeID == null && action.SystemActionTypeID == 0)
       {
         RadAjaxManager1.Alert("You have selected an invalid Action Type.");
         return;
       }

       action.Collection.Save();

    }
    

    try
    {
      SaveAttachments(action.ActionID);
    }
    catch (Exception ex)
    {

      RadAjaxManager1.Alert("There was an error uploading your attachments.  " + ex.Message);
      return;
    }

    


    litScript.Text = "<script>CloseWindow();</script>";
  }


  private void SaveAttachments(int actionID)
  {
    Attachments attachments = new Attachments(UserSession.LoginUser);

    foreach (UploadedFile file in uploadMain.UploadedFiles)
    {
      string directory = TSUtils.GetAttachmentPath("Actions", actionID);

      Attachment attachment = attachments.AddNewAttachment();
      attachment.RefType = ReferenceType.Actions;
      attachment.RefID = actionID;
      attachment.OrganizationID = UserSession.LoginUser.OrganizationID;
      attachment.FileName = file.GetName();
      attachment.Path = Path.Combine(directory, attachment.FileName);
      attachment.FileType = string.IsNullOrEmpty(file.ContentType) ? "application/octet-stream" : file.ContentType;
      attachment.FileSize = file.ContentLength;

      Directory.CreateDirectory(directory);
      file.SaveAs(attachment.Path, true);
      attachments.Save();
    }

  }

  [WebMethod(true)]
  public static TicketAndActions GetTicketActions(int ticketID)
  {
    TicketAndActions result = new TicketAndActions();
    result.Ticket = TicketsView.GetTicketsViewItem(UserSession.LoginUser, ticketID).GetProxy();
    if (result.Ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
    ActionsView actions = new ActionsView(UserSession.LoginUser);
    actions.LoadKBByTicketID(ticketID);
    result.Actions = actions.GetActionsViewItemProxies();
    return result;
  }

  [DataContract]
  public class TicketAndActions
  {
    [DataMember] 
    public TicketsViewItemProxy Ticket { get; set; }
    [DataMember]
    public ActionsViewItemProxy[] Actions { get; set; }

  }

}
