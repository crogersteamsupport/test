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
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;

public partial class Frames_AdminWorkflow : BaseFramePage
{

  public int SelectedTicketTypeIndex
  {
    get { return Settings.Session.ReadInt("AdminWorkflowTicketTypeIndex", 0); }
    set { Settings.Session.WriteInt("AdminWorkflowTicketTypeIndex", value); }
  }

  public int SelectedTicketStatusIndex
  {
    get { return Settings.Session.ReadInt("AdminWorkflowStatusIndex", 0); }
    set { Settings.Session.WriteInt("AdminWorkflowStatusIndex", value); }
  }

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);

    LoadTicketTypes();
    if (cmbTicketTypes.Items.Count > 0)
    {
      cmbTicketTypes.SelectedIndex = SelectedTicketTypeIndex;
      LoadStatuses(int.Parse(cmbTicketTypes.SelectedValue));
      if (cmbStatuses.Items.Count > 0)
      {
        cmbStatuses.SelectedIndex = SelectedTicketStatusIndex;
        LoadNextStatuses(int.Parse(cmbStatuses.SelectedValue));
      }
    }
    else
    {
      cmbStatuses.Items.Clear();
    }

  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    bool isAdmin = UserSession.CurrentUser.IsSystemAdmin;
    gridNext.Columns[0].Visible = isAdmin;
    gridNext.Columns[1].Visible = isAdmin;
    gridNext.Columns[2].Visible = isAdmin;
    lnkAddStatus.Visible = isAdmin;

    if (SelectedTicketTypeIndex != cmbTicketTypes.SelectedIndex)
    {
      SelectedTicketTypeIndex = cmbTicketTypes.SelectedIndex;
      LoadStatuses(GetSelectedTypeID());
      LoadNextStatuses(GetSelectedStatusID());
      SelectedTicketStatusIndex = cmbStatuses.SelectedIndex;
    }
    else if (SelectedTicketStatusIndex != cmbStatuses.SelectedIndex)
    {
      SelectedTicketStatusIndex = cmbStatuses.SelectedIndex;
      LoadNextStatuses(GetSelectedStatusID());
    }
  }

  private int GetSelectedTypeID()
  {
    if (cmbTicketTypes.SelectedIndex < 0) return -1;
    return int.Parse(cmbTicketTypes.SelectedValue);
  }

  private int GetSelectedStatusID()
  { 
    if (cmbStatuses.SelectedIndex < 0) return -1;
    return int.Parse(cmbStatuses.SelectedValue);
  }

  private void LoadTicketTypes()
  {
    cmbTicketTypes.Items.Clear();
    TicketTypes types = new TicketTypes(UserSession.LoginUser);
    types.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, UserSession.CurrentUser.ProductType);
    foreach (TicketType type in types)
    {
      cmbTicketTypes.Items.Add(new RadComboBoxItem(type.Name, type.TicketTypeID.ToString()));
    }
    if (cmbTicketTypes.Items.Count > 0) cmbTicketTypes.SelectedIndex = 0;
  }

  private void LoadStatuses(int ticketTypeID)
  {
    cmbStatuses.Items.Clear();
    
    TicketStatuses statuses = new TicketStatuses(UserSession.LoginUser);
    statuses.LoadByTicketTypeID(ticketTypeID);

    foreach (TicketStatus status in statuses)
    {
      cmbStatuses.Items.Add(new RadComboBoxItem(status.Name, status.TicketStatusID.ToString()));
    }
    if (cmbStatuses.Items.Count > 0) cmbStatuses.SelectedIndex = 0;
  }

  private void LoadAvailableStatuses(int statusID)
  {
    TicketStatuses statuses = new TicketStatuses(UserSession.LoginUser);
    statuses.LoadNotNextStatuses(statusID);

    cmbNewStatus.Items.Clear();
    cmbNewStatus.Items.Add(new RadComboBoxItem("[Select a status]", "-1"));
    foreach (TicketStatus status in statuses)
    {
      cmbNewStatus.Items.Add(new RadComboBoxItem(status.Name, status.TicketStatusID.ToString()));
    }

  }

  private void LoadNextStatuses(int ticketStatusID)
  {
    TicketNextStatuses nextStatuses = new TicketNextStatuses(UserSession.LoginUser);
    nextStatuses.LoadNextStatuses(ticketStatusID);
    gridNext.DataSource = nextStatuses.Table;
    gridNext.DataBind();
    nextStatuses.ValidatePositions(ticketStatusID);
    LoadAvailableStatuses(ticketStatusID);
  }

  protected void gridNext_ItemCommand(object source, GridCommandEventArgs e)
  {
    if (e.CommandName == RadGrid.DeleteCommandName)
    {
      TicketNextStatuses statuses = new TicketNextStatuses(UserSession.LoginUser);
      statuses.DeleteFromDB((int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["TicketNextStatusID"]);
    }
    else if (e.CommandName == "MoveUp")
    {
      TicketNextStatuses statuses = new TicketNextStatuses(UserSession.LoginUser);
      statuses.MovePositionUp((int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["TicketNextStatusID"]);
    }
    else if (e.CommandName == "MoveDown")
    {
      TicketNextStatuses statuses = new TicketNextStatuses(UserSession.LoginUser);
      statuses.MovePositionDown((int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["TicketNextStatusID"]);
    }

    LoadNextStatuses(GetSelectedStatusID());
    
  }

  protected void cmbNewStatus_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
  {
    if (cmbNewStatus.SelectedIndex < 1) return;
    TicketNextStatuses ticketNextStatuses = new TicketNextStatuses(UserSession.LoginUser);
    TicketNextStatus ticketNextStatus = ticketNextStatuses.AddNewTicketNextStatus();
    ticketNextStatus.CurrentStatusID = GetSelectedStatusID();
    ticketNextStatus.NextStatusID = int.Parse(cmbNewStatus.SelectedValue);
    ticketNextStatus.Position = ticketNextStatuses.GetMaxPosition(ticketNextStatus.CurrentStatusID) + 1;
    ticketNextStatuses.Save();
    LoadNextStatuses(GetSelectedStatusID());
  }
}
