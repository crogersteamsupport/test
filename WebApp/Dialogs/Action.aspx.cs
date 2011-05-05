using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using TeamSupport.WebUtils;
using TeamSupport.Data;
using Telerik.Web.UI;
using System.IO;

public partial class Dialogs_Action : BaseDialogPage
{
  private int _ticketID = -1;
  private int _actionID = -1;

  protected override void  OnLoad(EventArgs e)
  {
 	  base.OnLoad(e);

    editorDescription.EnsureToolsFileLoaded();
    editorDescription.Modules.Clear();

    _manager.AjaxSettings.Clear();
    _manager.AjaxRequest += new RadAjaxControl.AjaxRequestDelegate(_manager_AjaxRequest);

    try
    {
      _ticketID = int.Parse(Request["TicketID"]);
      _actionID = Request["ActionID"] != null ? int.Parse(Request["ActionID"]) : -1;

    }
    catch 
    {
      Response.Write("Invalid TicketID or ActionID.");
      Response.End();
    }

    Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, _ticketID);
    if (ticket != null && ticket.OrganizationID != UserSession.LoginUser.OrganizationID)
    {
      Response.Write("Invalid Request");
      Response.End();
      return;
    }

    TeamSupport.Data.Action action = Actions.GetAction(UserSession.LoginUser, _actionID);
    if (action != null)
    {
      ticket = Tickets.GetTicket(UserSession.LoginUser, action.TicketID);
      if (ticket != null && ticket.OrganizationID != UserSession.LoginUser.OrganizationID)
      {
        Response.Write("Invalid Request");
        Response.End();
        return;
      }
    }


    if (!Page.IsPostBack)
    {
      dtpStarted.Culture = UserSession.LoginUser.CultureInfo;
      LoadCombos();
      if (_actionID > -1) LoadAction();
    }

    textName.Enabled = int.Parse(cmbActionType.SelectedValue) > -1 && textName.Enabled;

    trTime.Visible = false;
    ActionType actionType = (ActionType)ActionTypes.GetActionType(UserSession.LoginUser, int.Parse(cmbActionType.SelectedValue));
    if (actionType != null)
    {
      trTime.Visible = actionType.IsTimed;
    }

    _manager.AjaxSettings.AddAjaxSetting(cmbActionType, pnlProperties);
    _manager.AjaxSettings.AddAjaxSetting(_manager, rptAttachments);
  }

  void _manager_AjaxRequest(object sender, AjaxRequestEventArgs e)
  {
    LoadAttachments();
  }
  /*
  public override void AjaxRequested(AjaxRequest request)
  {
    base.AjaxRequested(request);
    if (request.ActionObject == "Attachment" && request.ActionName == "Delete")
    {
      Attachments.DeleteAttachmentAndFile(UserSession.LoginUser, int.Parse(request.Arguments[0]));
      LoadAttachments();
    }
  }*/

  private void LoadAction()
  {

    Actions actions = new Actions(UserSession.LoginUser);
    actions.LoadByActionID(_actionID);
    if (actions.IsEmpty) return;

    textName.Text = actions[0].Name;
    editorDescription.Content = actions[0].Description;
    cmbActionType.SelectedValue = actions[0].ActionTypeID.ToString();
    dtpStarted.SelectedDate = actions[0].DateStarted;
    numSpent.Value = actions[0].TimeSpent;
    cbKnowledge.Checked = actions[0].IsKnowledgeBase;
    cbPortal.Checked = actions[0].IsVisibleOnPortal;

    if (actions[0].SystemActionTypeID == SystemActionType.Description)
    {
      tdActionTypeInput.Visible = false;
      tdActionTypeLabel.Visible = false;
      textName.Enabled = false;
      cmbActionType.SelectedIndex = -1;
    }
    LoadAttachments();
  }

  private void LoadAttachments()
  {
    Attachments attachments = new Attachments(UserSession.LoginUser);
    attachments.LoadByActionID(_actionID);
    rptAttachments.DataSource = attachments.Table;
    rptAttachments.DataBind();
  }

  public override bool Save()
  {

    if (uploadMain.UploadedFiles.Count > 0)
    {
      int used = Organizations.GetStorageUsed(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
      int allowed = Organizations.GetTotalStorageAllowed(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);

      if (used > allowed)
      {
        if (UserSession.CurrentUser.IsFinanceAdmin)
        {
          _manager.Alert("You have exceeded your allocated storage capacity.  If you would like to add additional storage, please contact our sales team at 800.596.2820 x806, or send an email to sales@teamsupport.com");
        }
        else
        {
          Users users = new Users(UserSession.LoginUser);
          users.LoadFinanceAdmins(UserSession.LoginUser.OrganizationID);
          if (users.IsEmpty)
          {
            _manager.Alert("Please ask your billing administrator to purchase additional storage to add additional attachments.");
          }
          else
          {
            _manager.Alert("Please ask your billing administrator (" + users[0].FirstLastName + ") to purchase additional storage to add additional attachments.");
          }
        }


        return false;
      }
    }

    Actions actions = new Actions(UserSession.LoginUser);
    TeamSupport.Data.Action action;

    if (_actionID > -1)
    {
      actions.LoadByActionID(_actionID);
      if (actions.IsEmpty)
      {
        _manager.Alert("There was an error updating the action.");
        return false;
      }

      action = actions[0];
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
      action.DateStarted = dtpStarted.SelectedDate;

      if (action.ActionTypeID == null && action.SystemActionTypeID == 0)
      {
        _manager.Alert("You have selected an invalid Action Type.");
        return false;
      }

      actions.Save();

    }
    else
    {
      action = actions.AddNewAction();
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
      action.TicketID = _ticketID;

      action.TimeSpent = numSpent.Value == null ? null : (int?)numSpent.Value;
      action.DateStarted = dtpStarted.SelectedDate;

      if (action.ActionTypeID == null && action.SystemActionTypeID == 0)
      {
        _manager.Alert("You have selected an invalid Action Type.");
        return false;
      }

      actions.Save();
    }

    try
    {
      SaveAttachments(action.ActionID);
    }
    catch (Exception ex)
    {

      _manager.Alert("There was an error uploading your attachments.  " + ex.Message);
      return false;
    }

    return true;
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
      attachment.FileType = file.ContentType;
      attachment.FileSize = file.ContentLength;

      Directory.CreateDirectory(directory);
      file.SaveAs(attachment.Path, true);
      attachments.Save();
    }

  }

  private void LoadCombos()
  {
    ActionTypes actionTypes = new ActionTypes(UserSession.LoginUser);
    actionTypes.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
    cmbActionType.Items.Clear();

    foreach (ActionType actionType in actionTypes)
    {
      cmbActionType.Items.Add(new RadComboBoxItem(actionType.Name, actionType.ActionTypeID.ToString()));
    }


    Actions actions = new Actions(UserSession.LoginUser);
    actions.LoadByTicketID(_ticketID);
    bool flag = false;
    foreach (TeamSupport.Data.Action action in actions)
    {
      if (action.SystemActionTypeID == SystemActionType.Resolution)
      {
        flag = true;
        break;
      }
    }

    if (!flag) cmbActionType.Items.Add(new RadComboBoxItem("Resolution", "-1"));

    if (cmbActionType.Items.Count > 0) cmbActionType.SelectedIndex = 0;
  }

}
