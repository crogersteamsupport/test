<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Actions.ascx.cs" Inherits="UserControls_Actions" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<style type="text/css">
  body
  {
    background-color: #ffffff;
  }
  .actionCaptionDiv
  {
    font-size: 14px;
    font-weight: bold;
  }
  .actionBodyDiv
  {
    padding: 10px 40px 0 15px;
  }
  .actionAttachmentsDiv
  {
    padding-left: 15px;
  }
  .actionTimeDiv
  {
    padding-left: 15px;
  }
  .actionPropertiesDiv
  {
    padding-left: 15px;
  }
  .actionFooterDiv
  {
    font-style: italic;
    border-bottom: dotted 0px #97aad1;
    margin-right: 20px;
    padding-bottom: 7px;
    padding-left: 15px;
  }
  .hiddenButton
  {
    display: none;
  }
</style>
<div id="actionDetailsDiv" style="background-color: #ffffff;" runat="server">
  <asp:Repeater ID="rptActions" runat="server">
    <ItemTemplate>
      <br />
      <div class="groupBarDiv groupBarBlue" style="width: 98%; margin: 0 auto;">
        <div class="groupBarHeaderDiv">
          <span class="groupBarHeaderSpan"></span>
          <span class="groupBarCaptionSpan">
            <%# DataBinder.Eval(Container.DataItem, "Name")%></span>
          <span class="groupBarButtonSpanWrapper <%# DataBinder.Eval(Container.DataItem, "CanEdit")%>" style="display:none;">
            <span class="groupBarButtonsSpan">
              <a class="groupBarButtonLink" href="#" onclick="EditAction(<%# DataBinder.Eval(Container.DataItem, "TicketID")%>, <%# DataBinder.Eval(Container.DataItem, "ActionID")%>)">
                <span class="groupBarButtonSpan">
                  <img alt="" src="../images/icons/edit.png" class="groupBarButtonImage" />
                  <span class="groupBarButtonTextSpan">Edit</span>
                </span>
              </a>
            </span>
            <span class="groupBarButtonsSpan <%# DataBinder.Eval(Container.DataItem, "CanDelete")%>">
              <a class="groupBarButtonLink" href="#" onclick="DeleteAction(<%# DataBinder.Eval(Container.DataItem, "ActionID")%>)">
                <span class="groupBarButtonSpan">
                  <img alt="" src="../images/icons/trash.png" class="groupBarButtonImage" />
                  <span class="groupBarButtonTextSpan">Delete</span>
                </span>
              </a>
            </span>

          </span>
        </div>
      </div>
      <br />
      <div class="actionBodyDiv">
        <html>
        <body>
          <div>
            <%# DataBinder.Eval(Container.DataItem, "Description")%></div>
      </div>
      </body></html>
      <br />
      <div class="actionAttachmentsDiv">
        <strong>Attachments:</strong> &nbsp
        <%# DataBinder.Eval(Container.DataItem, "Attachments")%></div>
      <br />
      <%# DataBinder.Eval(Container.DataItem, "Time")%>
      <%# DataBinder.Eval(Container.DataItem, "Properties")%>
      <div class="actionFooterDiv">
        -&nbsp<span><%# DataBinder.Eval(Container.DataItem, "UserName")%></span>
        &nbsp&nbsp&nbsp<span><%# DataBinder.Eval(Container.DataItem, "Date")%></span>
      </div>
    </ItemTemplate>
  </asp:Repeater>
  <br />
</div>

  <telerik:RadWindowManager ID="windowManager" runat="server" Behaviors="Close, Move">
    <Windows>
      <telerik:RadWindow ID="wndAction" runat="server" NavigateUrl="~/Dialogs/TicketAction.aspx"
        Width="800px" Height="500px" Animation="None" KeepInScreenBounds="False" VisibleStatusbar="False"
        VisibleTitlebar="True" OnClientShow="wndAction_OnClientShow" OnClientPageLoad="wndAction_OnClientPageLoad"
        IconUrl="~/images/icons/action.png" VisibleOnPageLoad="false" ShowContentDuringLoad="False" OnClientClose="ActionWindowClosed"
        Modal="True">
      </telerik:RadWindow>
    </Windows>
  </telerik:RadWindowManager>
<telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

  <script type="text/javascript" language="javascript">

    var _selectedActionControlActionID = -1;
    var _selectedActionControlTicketID = -1;
    function wndAction_OnClientShow(sender, args) {
      sender.get_contentFrame().contentWindow.LoadAction(_selectedActionControlActionID, _selectedActionControlTicketID);
    }

    function wndAction_OnClientPageLoad(sender, args) {
      sender.get_contentFrame().contentWindow.LoadAction(_selectedActionControlActionID, _selectedActionControlTicketID);
    }

    function EditAction(ticketID, actionID) {
      _selectedActionControlActionID = actionID;
      _selectedActionControlTicketID = ticketID;
      var wnd = GetRadWindowManager().getWindowByName("wndAction");
      wnd.show();

    }

    function DeleteAction(actionID) {
      if (!confirm("Are you sure you would like to delete this action?")) return;
      top.privateServices.DeleteAction(actionID, function(result) { SendAjaxRequest(); });
    }

    function ActionWindowClosed(sender, args) {
      SendAjaxRequest();
    }
  </script>

</telerik:RadScriptBlock>
