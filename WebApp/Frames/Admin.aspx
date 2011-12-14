<%@ Page Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="Admin.aspx.cs" Inherits="Frames_Admin" Title="Untitled Page" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <telerik:RadSplitter VisibleDuringInit="false" ID="RadSplitter1" runat="server" Height="100%" Width="100%" Orientation="Horizontal" BorderSize="0" BackColor="#DBE6F4">
    <telerik:RadPane ID="RadPane1" runat="server" Height="40px" BackColor="#BFDBFF" Scrolling="None">
      <div style="padding-top: 14px;">
       <telerik:RadTabStrip ID="tsMain" runat="server" SelectedIndex="1"  
          OnClientTabSelected="TabSelected" PerTabScrolling="True" 
          ScrollChildren="True">
         <Tabs>
           <telerik:RadTab runat="server" Text="My Company" 
             Value="AdminCompany.aspx"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="My Portal" Value="../vcr/140/Pages/Admin_Portal.html"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="Old Portal" Value="AdminPortal.aspx"></telerik:RadTab>
           <telerik:RadTab runat="server" Selected="True" Text="Custom Fields" Value="../vcr/140/Pages/Admin_CustomFields.html"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="Custom Properties" 
             Value="AdminCustomProperties.aspx"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="SLA" Value="AdminSla.aspx"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="Workflow" Value="AdminWorkflow.aspx"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="Email" Value="AdminEmails.aspx"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="Integration" Value="../vcr/140/Pages/Admin_Integration.html" Visible="true"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="Ticket Templates" Value="AdminTicketTemplates.aspx"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="Ticket Automation" Value="../vcr/140/Pages/Admin_Automation.html" Visible="true"></telerik:RadTab>
           <telerik:RadTab runat="server" Text="Chat" Value="AdminChat.aspx" Visible="false"></telerik:RadTab>
         </Tabs>
        </telerik:RadTabStrip>
       </div>

    </telerik:RadPane>
    <telerik:RadPane ID="RadPane2" runat="server" Height="100%" Scrolling="None">
    <iframe id="frmAdmin" runat="server" class="contentFrame" scrolling="no" src="" frameborder="0" height="100%" width="100%"></iframe>
    </telerik:RadPane>
  </telerik:RadSplitter>
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
    <script type="text/javascript" language="javascript">

      var _lastRefreshTime = new Date();

      function refreshData() {
        if (_lastRefreshTime != null) {
          var now = new Date();
          var diff = (now - _lastRefreshTime) / 1000;
          if (diff < 300) return;
        }

        _lastRefreshTime = new Date();

        window.location = window.location;

      }    
      function TabSelected(sender, args) {
        showLoadingPanel("<%=frmAdmin.ClientID %>");
        var tab = args.get_tab();
        var frame = $get("<%=frmAdmin.ClientID %>");
        frame.setAttribute('src', tab.get_value());
        top.privateServices.SetUserSetting('SelectedAdminTabIndex' , tab.get_index());
        hideLoadingPanel("<%=frmAdmin.ClientID %>");
      }  
    
            
  
    
    
    </script>
  
  </telerik:RadScriptBlock>

</asp:Content>

