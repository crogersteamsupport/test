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
using Telerik.Web.UI;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Text;
using System.Web.Services;
using System.Globalization;

public partial class Frames_Tickets : BaseFramePage
{
  private TicketFilters _ticketFilters;

  protected override void OnInit(EventArgs e)
  {
    CachePage = true;
    base.OnInit(e);
    if (!IsPostBack)
    {
      if ((Request["AllowFilters"] != null && Request["AllowFilters"] == "1"))
      {
        gridTickets.AllowFilteringByColumn = true;
      }
      gridTickets.PageSize = Settings.UserDB.ReadInt("TicketsPageSize", 10);
      gridTickets.MasterTableView.SortExpressions.Clear();
      GridSortExpression expression = new GridSortExpression();
      expression.SortOrder = (GridSortOrder)Settings.UserDB.ReadInt("TicketGridSortOrder" + Request.Url, (int)GridSortOrder.Descending);
      expression.FieldName = Settings.UserDB.ReadString("TicketGridSortField" + Request.Url, "TicketNumber");
      gridTickets.MasterTableView.SortExpressions.AddSortExpression(expression);
      paneGrid.Height = new Unit(Settings.UserDB.ReadInt("TicketGridHeight", 250), UnitType.Pixel);
    
    }
  }

  protected override void InitializeCulture()
  {
    base.InitializeCulture();
    Page.Culture = UserSession.LoginUser.CultureInfo.Name;
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    try
    {
      _ticketFilters = GetTicketFilters();
    }
    catch 
    {
      Response.Write("Invalid parameters.");
      Response.End();
    }

    ajaxManager.AjaxSettings.AddAjaxSetting(gridTickets, gridTickets);

    if (UserSession.CurrentUser.IsTSUser || (Request["IsToolBarVisible"] != null && Request["IsToolBarVisible"] == "0"))
    {
      paneToolBar.Height = new Unit(1, UnitType.Pixel);
      tbMain.Visible = false;
    }


    if (UserSession.CurrentUser.ProductType == ProductType.Express || UserSession.CurrentUser.ProductType == ProductType.BugTracking)
    {
      tbMain.Items[3].Visible = false;
    }

    if (UserSession.CurrentUser.ProductType == ProductType.Express || UserSession.CurrentUser.ProductType == ProductType.HelpDesk)
    {
      gridTickets.Columns[6].Visible = false;
      gridTickets.Columns[7].Visible = false;
      gridTickets.Columns[8].Visible = false;
    }

    if (UserSession.CurrentUser.IsSystemAdmin)
    {
      tbMain.Items[1].Visible = true;
    }
    
    if (!IsPostBack)
    {
      gridTickets.CurrentPageIndex = Settings.UserDB.ReadInt("CurrentPageIndex" + Request.Url, 0);
      gridTickets.Rebind();
      ticketPreviewFrame.Attributes["src"] = "TicketPreview.aspx?TicketID=" + GetGridTicketID();
    }

  }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender(e);
  }
  
  protected void gridTickets_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
  {
    int organizationID;
    if (UserSession.CurrentUser.IsTSUser)
    {
      organizationID = _ticketFilters.CustomerID;
      _ticketFilters.CustomerID = TicketFilters.Values.All;
    }
    else
    {
      organizationID = UserSession.LoginUser.OrganizationID;
    }


    TicketsView tickets = new TicketsView(UserSession.LoginUser);

    //gridTickets.VirtualItemCount = tickets.LoadForGridCount(organizationID, _ticketFilters.TicketTypeID, _ticketFilters.TicketStatusID, _ticketFilters.TicketSeverityID, _ticketFilters.UserID, _ticketFilters.GroupID, _ticketFilters.ProductID, _ticketFilters.ReportedVersionID, _ticketFilters.ResolvedVersionID, _ticketFilters.CustomerID, _ticketFilters.IsPortal, _ticketFilters.IsKnowledgeBase, _ticketFilters.DateCreateBegin, _ticketFilters.DateCreateEnd, _ticketFilters.DateModifiedBegin, _ticketFilters.DateModifiedEnd, _ticketFilters.SearchText);
    //if (gridTickets.VirtualItemCount < gridTickets.PageSize * gridTickets.CurrentPageIndex) gridTickets.CurrentPageIndex = 0;
    int pageSize = 10000;
    if (_ticketFilters.Tags.Trim() != "")
    {
      tickets.LoadForTags(_ticketFilters.Tags);
    
    }
    else if (_ticketFilters.SearchText.Trim() != "")
    {
      TicketLoadFilter filter = new TicketLoadFilter();
      filter.SearchText = _ticketFilters.SearchText;
      tickets.LoadByRange(0, 10000, filter);
    }
    else
    {
      if (gridTickets.MasterTableView.SortExpressions.Count > 0)
      {
        GridSortExpression expression = gridTickets.MasterTableView.SortExpressions[0];
        tickets.LoadForGrid(0, pageSize, organizationID, _ticketFilters.TicketTypeID, _ticketFilters.TicketStatusID, _ticketFilters.TicketSeverityID, _ticketFilters.UserID, _ticketFilters.GroupID, _ticketFilters.ProductID, _ticketFilters.ReportedVersionID, _ticketFilters.ResolvedVersionID, _ticketFilters.CustomerID, _ticketFilters.IsPortal, _ticketFilters.IsKnowledgeBase, _ticketFilters.DateCreateBegin, _ticketFilters.DateCreateEnd, _ticketFilters.DateModifiedBegin, _ticketFilters.DateModifiedEnd, _ticketFilters.SearchText, expression.FieldName, expression.SortOrder == GridSortOrder.Ascending);
        
      }
      else
      {
        tickets.LoadForGrid(0, pageSize, organizationID, _ticketFilters.TicketTypeID, _ticketFilters.TicketStatusID, _ticketFilters.TicketSeverityID, _ticketFilters.UserID, _ticketFilters.GroupID, _ticketFilters.ProductID, _ticketFilters.ReportedVersionID, _ticketFilters.ResolvedVersionID, _ticketFilters.CustomerID, _ticketFilters.IsPortal, _ticketFilters.IsKnowledgeBase, _ticketFilters.DateCreateBegin, _ticketFilters.DateCreateEnd, _ticketFilters.DateModifiedBegin, _ticketFilters.DateModifiedEnd, _ticketFilters.SearchText, "TicketNumber", false);
      }
    }

    tickets.Table.CaseSensitive = false;

    gridTickets.DataSource = tickets.Table;

    
  }
  
  protected void gridTickets_PageSizeChanged(object source, GridPageSizeChangedEventArgs e)
  {
    if (IsPostBack) Settings.UserDB.WriteInt("TicketsPageSize", e.NewPageSize);

  }

  protected void gridTickets_DataBound(object sender, EventArgs e)
  {
    foreach (RadToolBarItem item in tbMain.Items)
    {
      if (item.Index > 0 && item is RadToolBarButton)
      {
        item.Enabled = gridTickets.Items.Count > 0;
      }

    }

    if (!SetGridTicketID(Settings.UserDB.ReadInt("SelectedTicketID" + Request.Url)) && gridTickets.Items.Count > 0)
    {
      gridTickets.Items[0].Selected = true;
      SetToolbarText();
    }
  }

  private void SetToolbarText()
  {
    int id = GetGridTicketID();

    if (Tickets.IsUserSubscribed(UserSession.LoginUser, UserSession.LoginUser.UserID, id))
      tbMain.Items[5].Text = "Unsubscribe";
    else
      tbMain.Items[5].Text = "Subscribe";
    
      
  }

  private int GetGridTicketID()
  {
    if (gridTickets.SelectedItems.Count < 1) return -1;
    GridItem item = gridTickets.SelectedItems[0];
    return (int)item.OwnerTableView.DataKeyValues[item.ItemIndex]["TicketID"]; 
  }

  private bool SetGridTicketID(int ticketID)
  {
    GridDataItem item = gridTickets.MasterTableView.FindItemByKeyValue("TicketID", ticketID);
    if (item != null)
    {
      item.Selected = true;
      SetToolbarText();
      return true;
    }
    else
    {
      return false;
    }
  }

  protected void gridTickets_PageIndexChanged(object source, GridPageChangedEventArgs e)
  {
    Settings.UserDB.WriteInt("CurrentPageIndex" + Request.Url, e.NewPageIndex);
  }

  private TicketFilters GetTicketFilters()
  { 
    TicketFilters result = new TicketFilters();
    result.DateCreateBegin = Request["DateCreateBegin"] != null ? (DateTime?)DateTime.Parse(Request["DateCreateBegin"]) : null;
    result.DateCreateEnd = Request["DateCreateEnd"] != null ? (DateTime?)DateTime.Parse(Request["DateCreateEnd"]) : null;
    result.DateModifiedBegin = Request["DateModifiedBegin"] != null ? (DateTime?)DateTime.Parse(Request["DateModifiedBegin"]) : null;
    result.DateModifiedEnd = Request["DateModifiedEnd"] != null ? (DateTime?)DateTime.Parse(Request["DateModifiedEnd"]) : null;

    result.CustomerID = Request["CustomerID"] != null ? int.Parse(Request["CustomerID"]) : TicketFilters.Values.All;
    result.GroupID = Request["GroupID"] != null ? int.Parse(Request["GroupID"]) : TicketFilters.Values.All;
    result.ProductID = Request["ProductID"] != null ? int.Parse(Request["ProductID"]) : TicketFilters.Values.All;
    result.ReportedVersionID = Request["ReportedVersionID"] != null ? int.Parse(Request["ReportedVersionID"]) : TicketFilters.Values.All;
    result.ResolvedVersionID = Request["ResolvedVersionID"] != null ? int.Parse(Request["ResolvedVersionID"]) : TicketFilters.Values.All;
    result.TicketSeverityID = Request["TicketSeverityID"] != null ? int.Parse(Request["TicketSeverityID"]) : TicketFilters.Values.All;
    result.TicketStatusID = Request["TicketStatusID"] != null ? int.Parse(Request["TicketStatusID"]) : TicketFilters.Values.All;
    result.TicketTypeID = Request["TicketTypeID"] != null ? int.Parse(Request["TicketTypeID"]) : TicketFilters.Values.All;
    result.UserID = Request["UserID"] != null ? int.Parse(Request["UserID"]) : TicketFilters.Values.All;

    result.IsKnowledgeBase = Request["IsKnowledgeBase"] != null ? (bool?)(Request["IsKnowledgeBase"] == "1") : null;
    result.IsPortal = Request["IsPortal"] != null ? (bool?)(Request["IsPortal"] == "1") : null;

    result.SearchText = Request["SearchText"] != null ? Request["SearchText"] : "";
    result.Tags = Request["Tags"] != null ? Request["Tags"] : "";

    return result;
  }

  protected void gridTickets_SortCommand(object source, GridSortCommandEventArgs e)
  {
    Settings.UserDB.WriteString("TicketGridSortField" + Request.Url, e.SortExpression);
    Settings.UserDB.WriteInt("TicketGridSortOrder" + Request.Url, (int)e.NewSortOrder);
  }

  protected void gridTickets_ItemCreated(object sender, GridItemEventArgs e)
  {

  }
  protected void gridTickets_ItemCommand(object source, GridCommandEventArgs e)
  {
    
  }
  protected void gridTickets_ItemDataBound(object sender, GridItemEventArgs e)
  {
    if (e.Item is GridDataItem)
    {
      GridDataItem item = (GridDataItem)e.Item;
      
      string key = item.GetDataKeyValue("TicketID").ToString();
      ImageButton button = (ImageButton)item["ButtonOpen"].Controls[0];
      button.OnClientClick = "OpenTicket('" + key + "'); return false;";

      try
      {

        if (item["SlaViolationTime"].Text.Trim() != "" &&
            item["SlaViolationTime"].Text.IndexOf("&nbsp") < 0 &&
            int.Parse(item["SlaViolationTime"].Text) < 1)
        {
          item["ButtonOpen"].CssClass = "slaViolation rgRow";
        }
        else if (item["SlaWarningTime"].Text.Trim() != "" &&
                 item["SlaWarningTime"].Text.IndexOf("&nbsp") < 0 &&
                 int.Parse(item["SlaWarningTime"].Text) < 1)
        {
          item["ButtonOpen"].CssClass = "slaWarning rgRow";
        }

        foreach (GridColumn column in ((GridItem)(item)).OwnerTableView.Columns)
        {
          
          if (column.DataType == typeof(DateTime))
          {
            string s = item[column].Text;
            if (s != "")
            {

              try
              {
                DateTime dt = DateTime.SpecifyKind(DateTime.Parse(s, new CultureInfo("en-US")), DateTimeKind.Utc);
                item[column].Text = DataUtils.DateToLocal(UserSession.LoginUser, dt).ToString("g", UserSession.LoginUser.CultureInfo);
              }
              catch (Exception)
              {
              }
            }
          }
        }
      }
      catch (Exception) { }
    } 



  }

  [WebMethod(true)]
  public static void Enqueue(int ticketID)
  {
    TicketQueue.Enqueue(UserSession.LoginUser, ticketID, UserSession.LoginUser.UserID);
  }

}
