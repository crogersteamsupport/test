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

public partial class Frames_Search : BaseFramePage
{
  private TicketFilters _filters;

  protected override void  OnLoad(EventArgs e)
  {
 	  base.OnLoad(e);

    ajaxManager.AjaxSettings.AddAjaxSetting(btnSearch, lblResults);
    ajaxManager.AjaxSettings.AddAjaxSetting(tsMain, tsMain);
    ajaxManager.AjaxSettings.AddAjaxSetting(tsMain, lblResults);
    ajaxManager.AjaxSettings.AddAjaxSetting(btnSearch, ticketsFrame, loadSearch);
    ajaxManager.AjaxSettings.AddAjaxSetting(tsMain, ticketsFrame, loadSearch);
    

    _filters = new TicketFilters();

    textSearch.Focus();
  }


  private void Search(string text, bool matchAll)
  {
    try
    {
      if (text.Trim() == "")
      {
        return;
      }
      lblResults.Text = "";
      _filters.SearchText = text;// DataUtils.BuildSearchString(text, matchAll);

      if (tsMain.SelectedIndex == 0)
      {
        ticketsFrame.Attributes["src"] = "Tickets.aspx?AllowFilters=1&SearchText=" + _filters.SearchText;
        /*Tickets tickets = new Tickets(UserSession.LoginUser);
        int count = tickets.LoadForGridCount(UserSession.LoginUser.OrganizationID, _filters.TicketTypeID, _filters.TicketStatusID,
          _filters.TicketSeverityID, _filters.UserID, _filters.GroupID, _filters.ProductID, _filters.ReportedVersionID, _filters.ResolvedVersionID,
          _filters.CustomerID, _filters.IsPortal, _filters.IsKnowledgeBase, _filters.DateCreateBegin, _filters.DateCreateEnd,
          _filters.DateModifiedBegin, _filters.DateModifiedEnd, _filters.SearchText);

        lblResults.Text = count.ToString() + " Tickets Found";*/
      }
      else
      {
        WikiArticlesView wiki = new WikiArticlesView(UserSession.LoginUser);
        int count = wiki.LoadForGridCount(UserSession.LoginUser.OrganizationID, UserSession.LoginUser.UserID, _filters.SearchText);

      //  lblResults.Text = count.ToString() + " Articles Found";
        ticketsFrame.Attributes["src"] = "Wikis.aspx?SearchText=" + _filters.SearchText;
      }
    }
    catch (Exception ex)
    {
      lblResults.Text = "Error retrieving results.";
    }
    
    if (UserSession.LoginUser.UserID == 34)
    {
      lblResults.Text = lblResults.Text + " &nbsp&nbsp&nbsp&nbsp&nbsp Search Term: " + _filters.SearchText;
    }
  }

  protected void btnSearch_Click(object sender, EventArgs e)
  {
    Search(textSearch.Text, cbMatchAll.Checked);
    lblResults.Visible = true;
  }
  protected void tsMain_TabClick(object sender, RadTabStripEventArgs e)
  {
    Search(textSearch.Text, cbMatchAll.Checked);
    lblResults.Visible = true;
  }
}
