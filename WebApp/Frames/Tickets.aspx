<%@ Page Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="Tickets.aspx.cs" Inherits="Frames_Tickets" Title="Untitled Page" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <style type="text/css">
    .slaWarning { background: #FFFB00;	}
    .slaViolation { background: #E86868;	}
    .rgSorted { background-color:Transparent;}
  </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadAjaxManager ID="ajaxManager" runat="server" UpdatePanelsRenderMode="Inline">
  </telerik:RadAjaxManager>
  <telerik:RadSplitter VisibleDuringInit="false" ID="splMain" runat="server" Orientation="Horizontal"
    Height="100%" Width="100%" BorderSize="0" HeightOffset="0" LiveResize="False">
    <telerik:RadPane ID="paneToolBar" runat="server" Height="32px" Scrolling="None">
      <telerik:RadToolBar ID="tbMain" runat="server" CssClass="NoRoundedCornerEnds" OnClientButtonClicked="ButtonClicked">
        <CollapseAnimation Duration="200" Type="OutQuint"></CollapseAnimation>
        <Items>
          <telerik:RadToolBarButton runat="server" Value="NewTicket" ImageUrl="~/images/icons/New.png"
            Text="New Ticket"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="DeleteTicket" ImageUrl="~/images/icons/trash.png"
            Text="Delete Ticket" Visible="false"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Button 1"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="AddOrganization" ImageUrl="~/images/icons/CustomerAdd.png"
            Text="Associate Customer" ToolTip="Associate a customer with this ticket." Visible="false"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="AddAction" ImageUrl="~/images/icons/Action.png"
            Text="Log Action" ToolTip="Add an action to the selected ticket." Visible="false"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="Subscribe" ImageUrl="~/images/icons/Subscription.png"
            Text="Subscribe" ToolTip="Subscribe to get updated when the ticket changes."></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="Enqueue" ImageUrl="~/images/icons/Enqueue.png"
            Text="Enqueue" ToolTip="Add the ticket to the end of your queue."></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="TakeOwnership" ImageUrl="~/images/icons/TakeOwnership.png"
            Text="Take Ownership" ToolTip="Take ownership of the selected ticket."></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="RequestUpdate" ImageUrl="~/images/icons/RequestUpdate.png"
            Text="Request Update" ToolTip="Request an update from the owner of the selected ticket.">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Button 1"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="Export" ImageUrl="~/images/icons/Export.png"
            Text="Export" ToolTip="Export the ticket data."></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="History" ImageUrl="~/images/icons/History.png"
            Text="History" ToolTip="View the history of the ticket."></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="Refresh" ImageUrl="~/images/icons/Refresh.png"
            Text="Refresh" ToolTip="Refresh the ticket grid."></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="SendUpdate" ImageUrl="~/images/icons/SendUpdate.png"
            Text="Send Update" ToolTip="Send an update to another user." Visible="false"></telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="Help" ImageUrl="~/images/icons/Help.png"
            Text="Help" ToolTip="Help." Visible="false"></telerik:RadToolBarButton>
        </Items>
      </telerik:RadToolBar>
    </telerik:RadPane>
    <telerik:RadPane ID="paneGrid" runat="server" BackColor="#fafafa" Scrolling="None"
      Height="250px" OnClientResized="GridResized">
      <div class="stretchContentHolderDiv">
        <telerik:RadGrid ID="gridTickets" runat="server" AllowPaging="True" AllowSorting="True"
          AutoGenerateColumns="False" BorderWidth="0px" GridLines="None" Height="100%" PageSize="20"
          Width="100%" OnNeedDataSource="gridTickets_NeedDataSource"
          OnDataBound="gridTickets_DataBound" OnPageSizeChanged="gridTickets_PageSizeChanged"
          OnPageIndexChanged="gridTickets_PageIndexChanged" OnItemCreated="gridTickets_ItemCreated"
          OnSortCommand="gridTickets_SortCommand" OnItemCommand="gridTickets_ItemCommand"
          OnItemDataBound="gridTickets_ItemDataBound">
          <PagerStyle AlwaysVisible="true" Mode="NextPrevAndNumeric" />
          <ExportSettings ExportOnlyData="True" IgnorePaging="True" FileName="Tickets" Excel-Format="ExcelML">
            <Excel Format="ExcelML" />
          </ExportSettings>
          <GroupingSettings CaseSensitive="false" />
          <MasterTableView ClientDataKeyNames="TicketID" DataKeyNames="TicketID" TableLayout="Fixed"
            AllowCustomSorting="True" AutoGenerateColumns="false" CellSpacing="-1">
            <Columns>
              <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="../images/icons/open.png"
                UniqueName="ButtonOpen" CommandName="ShowTicket">
                <HeaderStyle Width="32px" />
              </telerik:GridButtonColumn>
              <telerik:GridBoundColumn DataField="TicketNumber" HeaderText="Number" UniqueName="TicketNumber">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="Name" HeaderText="Name" UniqueName="Name">
                <HeaderStyle Width="200px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="TicketTypeName" HeaderText="Type" UniqueName="TicketTypeName">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="Status" HeaderText="Status" UniqueName="Status">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="Severity" HeaderText="Severity" UniqueName="Severity">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="ProductName" HeaderText="Product" UniqueName="ProductName">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="ReportedVersion" HeaderText="Version Reported"
                UniqueName="ReportedVersion">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="SolvedVersion" HeaderText="Version Resolved"
                UniqueName="SolvedVersion">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="UserName" HeaderText="Assigned To" UniqueName="UserName">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="UserID" HeaderText="UserID" UniqueName="UserID"
                Visible="false">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="GroupName" HeaderText="Group" UniqueName="GroupName">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="DateModified" DataFormatString="{0:g}" HeaderText="Last Modified"
                UniqueName="LastModified">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="DateCreated" DataFormatString="{0:g}" HeaderText="Date Opened"
                UniqueName="DateCreated">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="DaysOpened" HeaderText="Days Opened" UniqueName="DaysOpened">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="IsClosed" HeaderText="Closed" UniqueName="IsClosed">
                <HeaderStyle Width="75px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="CloserName" HeaderText="Closed By" UniqueName="CloserName">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="DateClosed" HeaderText="Date Closed" UniqueName="DateClosed"
                Visible="false">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="SlaViolationHours" HeaderText="SLA Violation Hours" UniqueName="SlaViolationHours">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="SlaViolationTime" HeaderText="SLA Violation Time" UniqueName="SlaViolationTime" Visible="false">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              <telerik:GridBoundColumn DataField="SlaWarningTime" HeaderText="SLA Warning Time" UniqueName="SlaWarningTime" Visible="false">
                <HeaderStyle Width="125px" />
              </telerik:GridBoundColumn>
              
              
            </Columns>
          </MasterTableView>
          <ClientSettings AllowColumnsReorder="False">
            <Selecting AllowRowSelect="True" />
            <ClientEvents OnRowSelected="RowSelected" OnRowDblClick="RowDoubleClicked"/>
            <Scrolling AllowScroll="True" UseStaticHeaders="True" EnableVirtualScrollPaging="false"
              SaveScrollPosition="true" />
          </ClientSettings>
        </telerik:RadGrid>
      </div>
    </telerik:RadPane>
    <telerik:RadSplitBar ID="sbMain" runat="server" />
    <telerik:RadPane ID="paneDetails" runat="server" Scrolling="None" BackColor="#FFFFFF"
      Height="100%">
      <iframe id="ticketPreviewFrame" runat="server" scrolling="auto" src="" frameborder="0"
        width="100%" height="100%"></iframe>
    </telerik:RadPane>
  </telerik:RadSplitter>
  <telerik:RadWindowManager ID="windowManager" runat="server" Behaviors="Close, Move">
    <Windows>
      <telerik:RadWindow ID="wndAction" runat="server" NavigateUrl="~/Dialogs/TicketAction.aspx"
        Width="700px" Height="450px" Animation="None" KeepInScreenBounds="False" VisibleStatusbar="False"
        VisibleTitlebar="True" OnClientShow="wndAction_OnClientShow" OnClientPageLoad="wndAction_OnClientPageLoad"
        IconUrl="~/images/icons/action.png" VisibleOnPageLoad="false" ShowContentDuringLoad="False"
        Modal="True">
      </telerik:RadWindow>
    </Windows>
  </telerik:RadWindowManager>
  
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

    <script type="text/javascript" language="javascript">


      function onShow() { RefreshGrid(); }

      function GridResized(sender, args)
      {
        top.privateServices.SetUserSetting('TicketGridHeight', sender.get_height());
      }
    
      function RowSelected(sender, args)
      {
        var frame = $get("<%=ticketPreviewFrame.ClientID %>");
        var id = args.getDataKeyValue('TicketID');
        frame.setAttribute('src', 'TicketPreview.aspx?TicketID=' + id);
        top.privateServices.SetUserSetting('SelectedTicketID' + window.location, id);
        IsSubscribed();
      }
      
      function RowDoubleClicked(sender, args)
      {
        var id = args.getDataKeyValue('TicketID');
        //top.OpenTicketWindow(id);
        OpenTicket(id);
        //window.open('../Ticket.aspx?ticketid=' + id, 'Ticket' + id);
      }


      function OpenTicket(id) {
        top.Ts.MainPage.openTicketByID(id);
      }
      
      function ButtonClicked(sender, args)      
      {
         
        var button = args.get_item();
        var value = button.get_value();
        if (value == 'NewTicket') { top.Ts.MainPage.newTicket(); }
        else if (value == 'DeleteTicket') { DeleteTicket(); }
        else if (value == 'AddOrganization') { ShowAssociateOrganization(); }
        else if (value == 'AddAction') { ShowAction(); }
        else if (value == 'Subscribe') { Subscribe(); }
        else if (value == 'Enqueue') { PageMethods.Enqueue(GetSelectedTicketID()); }
        else if (value == 'TakeOwnership') { TakeOwnership(); }
        else if (value == 'RequestUpdate') { RequestUpdate(); }
        else if (value == 'Export') { Export(); }
        else if (value == 'History') { ShowHistory(); }
        else if (value == 'Refresh') { RefreshGrid(); }
        else if (value == 'Help') { top.ShowHelpDialog(170); }
          
      }
      
      function ShowNewTicket()
      {
        var value = top.GetQueryParamValue('TicketTypeID', window.location);
        var wnd = top.GetNewTicketDialog(value);
        wnd.add_close(DialogClosed);
        wnd.show();
      }

      function DeleteTicket() {
        if (!confirm('Are you sure you would like to delete the selected ticket?')) return;
        top.privateServices.DeleteTicket(GetSelectedTicketID(), function(result) { RefreshGrid(); });
      }
      
      
      
      function ShowAssociateOrganization()
      {
        var wnd = top.GetSelectOrganizationDialog(17, GetSelectedTicketID())
        wnd.add_close(DialogClosed);
        wnd.show();
      }
      
      function ShowAction() {
        var wnd = top.GetRadWindowManager().getWindowByName("wndAction");

        var fn = function(sender, args) { sender.remove_show(fn); loadAction(wnd); }
        wnd.add_show(fn);
        wnd.add_pageLoad(fn);
        wnd.show();
      }

      function loadAction(wnd) {
        $(wnd.get_contentFrame().contentWindow.document).ready(function() {
          wnd.get_contentFrame().contentWindow.LoadAction(-1, GetSelectedTicketID());
        });
        
        //alert(wnd.get_contentFrame().contentWindow.document == null);
        //wnd.get_contentFrame().contentWindow.LoadAction(-1, GetSelectedTicketID());
      }
      function wndAction_OnClientShow(sender, args) {
        
      }

      function wndAction_OnClientPageLoad(sender, args) {
        sender.get_contentFrame().contentWindow.LoadAction(-1, GetSelectedTicketID());
      }

      function ShowHistory()
      {
        var wnd = top.GetHistoryDialog(17, GetSelectedTicketID())
        wnd.show();
      }
      
      function Subscribe()
      {
        top.privateServices.SubscribeToTicket(GetSelectedTicketID());
        var toolBar = $find("<%=tbMain.ClientID %>");
        var item = toolBar.findItemByValue("Subscribe");
        setTimeout('IsSubscribed()', 2000);
        if (item.get_text() == 'Unsubscribe') 
          alert('You have unsubscribed to the selected ticket.');
        else
          alert('You have subscribed to the selected ticket.');

      }
      
      function IsSubscribed()
      {
        top.privateServices.IsSubscribedToTicket(GetSelectedTicketID(), IsSubscribedResult);
      }
      
      function IsSubscribedResult(result)
      {
        var toolBar = $find("<%=tbMain.ClientID %>");
        var item = toolBar.findItemByValue("Subscribe");
        if (result) 
          item.set_text('Unsubscribe');
        else
          item.set_text('Subscribe');
      }
      
      
      function TakeOwnership()
      {
        top.privateServices.TakeTicketOwnership(GetSelectedTicketID());
        setTimeout('RefreshGrid()', 2000);
        alert('You are now assigned to the selected ticket.');
        
      }

      function RequestUpdate()
      {
        top.privateServices.RequestTicketUpdate(GetSelectedTicketID());
        alert('An update has been requested for the selected ticket.');

      }

      function Export()
      {
        var manager = $find("<%=ajaxManager.ClientID %>");
        manager.set_enableAJAX(false);
        var grid = $find("<%=gridTickets.ClientID %>").get_masterTableView();
        grid.exportToExcel();
      }
      
      function GetSelectedTicketID()
      {
        var item = $find("<%=gridTickets.ClientID %>").get_masterTableView().get_selectedItems()[0];
        return item.getDataKeyValue('TicketID');
      }
      
      function DialogClosed(sender, args)
      {
        RefreshGrid();
        sender.remove_close(DialogClosed);
        
      }
      
      function RefreshGrid()
      {
        var grid = $find("<%=gridTickets.ClientID %>").get_masterTableView();
        grid.rebind();
        var frame = $get("<%=ticketPreviewFrame.ClientID %>");
        frame.setAttribute('src', 'TicketPreview.aspx?TicketID=' + GetSelectedTicketID());
        IsSubscribed();
      }
      
    </script>

  </telerik:RadScriptBlock>
</asp:Content>
