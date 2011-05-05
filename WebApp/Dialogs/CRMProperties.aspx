<%@ Page Title="CRM Integration" Language="C#" MasterPageFile="~/Dialogs/Dialog.master"
  AutoEventWireup="true" CodeFile="CRMProperties.aspx.cs" Inherits="Dialogs_CRMProperties" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <style type="text/css">
    .crmLabelDiv
    {
      padding-top: 10px;
    }
  </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
        <div>CRM System</div>
        <div><telerik:RadComboBox ID="cmbType" runat="server" AutoPostBack="True" Width="250px" OnSelectedIndexChanged="cmbType_SelectedIndexChanged"></telerik:RadComboBox></div>
          <div class="crmLabelDiv"><asp:Label ID="lblSecurityToken" runat="server" Text="sdfsdf"></asp:Label></div>
          <div><telerik:RadTextBox ID="textSecurityToken" runat="server" Width="250px"></telerik:RadTextBox></div>
          <div class="crmLabelDiv"><asp:Label ID="lblSecurityTokenConfirm" runat="server" Text="sdfsdf"></asp:Label></div>
          <div><telerik:RadTextBox ID="textSecurityTokenConfirm" runat="server" Width="250px" TextMode="Password" Visible="false"></telerik:RadTextBox></div>
              
          <div class="crmLabelDiv"><asp:Label ID="lblUserName" runat="server" Text="Labfdsel"></asp:Label></div>
          <div><telerik:RadTextBox ID="textUserName" runat="server" Width="250px"></telerik:RadTextBox></div>
          
          <div runat="server" id="divPassword">
            <div class="crmLabelDiv"><asp:Label ID="lblPassword" runat="server" Text="Labelasfd asfd sd fas dfa sddf "></asp:Label></div>
            <div><telerik:RadTextBox ID="textPassword" runat="server" Width="250px" TextMode="Password"></telerik:RadTextBox></div>
            <div class="crmLabelDiv"><asp:Label ID="lblConfirm" runat="server" Text="Labelasfd asfd sd fas dfa sddf "></asp:Label></div>
            <div><telerik:RadTextBox ID="textPasswordConfirm" runat="server" Width="250px" TextMode="Password"></telerik:RadTextBox></div>
          </div>
          
          <div class="crmLabelDiv"><asp:Label ID="lblTypeFieldMatch" runat="server" Text="asfd sdf"></asp:Label></div>
          <div><telerik:RadTextBox ID="textTypeFieldMatch" runat="server" Width="250px"></telerik:RadTextBox></div>
          
          <div class="crmLabelDiv"><asp:CheckBox ID="cbActive" runat="server" Text="CRM synchronization active" /></div>
    </div>
  </div>
</asp:Content>
