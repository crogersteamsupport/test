<%@ Page Title="User" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true"
  CodeFile="User.aspx.cs" Inherits="Dialogs_User" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
      <table id="tblCustomControls" runat="server" width="775px" cellpadding="0" cellspacing="5"
        border="0">
        <tr>
          <td class="labelColTD">
            First Name:
          </td>
          <td class="inputColTD">
            <telerik:RadTextBox ID="textFirstName" runat="server" Width="200px"></telerik:RadTextBox>
          </td>
          <td class="labelColTD">
            Middle Name:
          </td>
          <td class="inputColTD">
            <telerik:RadTextBox ID="textMiddleName" runat="server" Width="200px"></telerik:RadTextBox>
          </td>
        </tr>
        <tr>
          <td class="labelColTD">
            Last Name:
          </td>
          <td class="inputColTD">
            <telerik:RadTextBox ID="textLastName" runat="server" Width="200px"></telerik:RadTextBox>
          </td>
          <td class="labelColTD">
            Title
          </td>
          <td class="inputColTD">
            <telerik:RadTextBox ID="textTitle" runat="server" Width="200px"></telerik:RadTextBox>
          </td>
        </tr>
        <tr>
          <td class="labelColTD">
            Email:
          </td>
          <td class="inputColTD">
            <telerik:RadTextBox ID="textEmail" runat="server" Width="200px"></telerik:RadTextBox>
          </td>
          <td id="tdActive" runat="server" class="labelColTD">
            Active:
          </td>
          <td class="inputColTD">
            <asp:CheckBox ID="cbActive" runat="server" Text="" CssClass="" />
          </td>
        </tr>
        <tr id="trAdmin" runat="server">
          <td class="labelColTD">
            System Admin:
          </td>
          <td class="inputColTD">
            <asp:CheckBox ID="cbIsSystemAdmin" runat="server" Text="" CssClass="" />
          </td>
          <td class="labelColTD" style="display:none;">
            Financial Admin:
          </td>
          <td class="inputColTD" style="display:none;">
            <asp:CheckBox ID="cbIsFinanceAdmin" runat="server" Text="" CssClass="" />
          </td>
        </tr>
        <tr>
          <td class="labelColTD" id="tdChatLabel" runat="server">
            Chat User:
          </td>
          <td class="inputColTD">
            <asp:CheckBox ID="cbChat" runat="server" Text=""/>
          </td>
        </tr>
        <tr>
          <td class="labelColTD">
            Email Ticket Notifications:
          </td>
          <td class="inputColTD">
            <asp:CheckBox ID="cbEmailNotify" runat="server" Text=""/>
          </td>
          <td class="labelColTD">
            Receive Assigned Group Notifications:
          </td>
          <td class="inputColTD">
            <asp:CheckBox ID="cbReceiveGroup" runat="server" Text=""/>
          </td>
        </tr>
        <tr>
          <td class="labelColTD" id="td1" runat="server">
            Automatically Subscribe to New Tickets:
          </td>
          <td class="inputColTD">
            <asp:CheckBox ID="cbSubscribe" runat="server" Text=""/>
          </td>
          <td class="labelColTD">
           Do not subscribe to tickets when cc'd on email:
          </td>
          <td class="inputColTD">
            <asp:CheckBox ID="cbNoAutoSubscribe" runat="server" Text=""/>
          </td>
        </tr>
        <tr>
          <td class="labelColTD">
            Time Zone:
          </td>
          <td class="inputColTD">
            <telerik:RadComboBox ID="cmbTimeZones" runat="server" Width="200px"></telerik:RadComboBox>
          </td>
          <td class="labelColTD">
            Date Format:
          </td>
          <td class="inputColTD">
            <telerik:RadComboBox ID="cmbDateFormat" runat="server" Width="200px"></telerik:RadComboBox>
          </td>
        </tr>
      </table>
      <br />
      <div id="pnlChange" runat="server" style="margin-bottom: 5px;">
        <span class="button buttonBlue">
          <a class="buttonLink buttonLinkPadding" href="../ChangePassword.aspx" target="ChangePassword">
            <span class="buttonSpan">
              <span class="buttonTextSpan">Change Password</span></span></a></span>
      </div>
      <asp:Button ID="btnReset" runat="server" Text="Reset and Email Password" OnClick="btnReset_Click" />
      <div style="padding: 0px 0 0 8px;">
        <asp:CheckBox ID="cbEmail" runat="server" Text="Email new user a password?" CssClass=""
          Checked="true" />
      </div>
    </div>
  </div>
</asp:Content>
