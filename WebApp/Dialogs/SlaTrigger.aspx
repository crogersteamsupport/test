<%@ Page Title="SLA Trigger" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true" CodeFile="SlaTrigger.aspx.cs" Inherits="Dialogs_SlaTrigger" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <style type="text/css">
    .col1{ text-align:right; padding-right: 5px;}
  </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
      <table cellpadding="0" cellspacing="5" border="0">
        <tr>
          <td class="col1">SLA Name:</td><td><asp:Label ID="lblSla" runat="server" Text="Label" Font-Bold="true"></asp:Label></td>
        </tr>
        <tr><td class="col1">Ticket Type:</td><td><asp:Label ID="lblTicketType" runat="server" Text="Label" Font-Bold="true"></asp:Label></td></tr>
        <tr><td class="col1">Ticket Severity:</td><td colspan="3"><telerik:RadComboBox ID="cmbSeverities" runat="server"></telerik:RadComboBox></td></tr>
        <tr><td class="col1">Warning Time Before Violation:</td><td>
          <telerik:RadNumericTextBox ID="numWarning" Width="50px" runat="server" 
            DataType="System.Int32" MinValue="0" Value="0">
            <NumberFormat DecimalDigits="0" />
          </telerik:RadNumericTextBox></td><td>
            <asp:RadioButton ID="rbWarningMin" runat="server" Text="Minutes" GroupName="WarningTime" />
            <asp:RadioButton ID="rbWarningHour" runat="server" Text="Hours" GroupName="WarningTime" Checked="true"/>
            <asp:RadioButton ID="rbWarningDay" runat="server" Text="Days" GroupName="WarningTime"/></td></tr>
        <tr>
          <td class="col1">Enforce Business Hours:</td><td>
            <asp:CheckBox ID="cbBusinessHours" runat="server" /></td>
        </tr>
        <tr><td colspan="4"><div style="border-bottom: solid 1px; font-size: 16px; margin-bottom: 7px; padding-top: 6px;">Violation Times</div></td></tr>    
        
        <tr><td class="col1">Time Since Initial Response:</td><td>
          <telerik:RadNumericTextBox ID="numInitial" Width="50px" runat="server" 
            DataType="System.Int32" MinValue="0" Value="0">
            <NumberFormat DecimalDigits="0" />
          </telerik:RadNumericTextBox></td><td>
            <asp:RadioButton ID="rbInitialMin" runat="server" Text="Minutes" GroupName="IntialTime" />
            <asp:RadioButton ID="rbInitialHour" runat="server" Text="Hours" GroupName="IntialTime" Checked="true"/>
            <asp:RadioButton ID="rbInitialDay" runat="server" Text="Days" GroupName="IntialTime"/></td></tr>
        <tr><td class="col1">Time Since Last Action:</td><td>
          <telerik:RadNumericTextBox ID="numAction" Width="50px" runat="server" 
            DataType="System.Int32" MinValue="0" Value="0">
            <NumberFormat DecimalDigits="0" />
          </telerik:RadNumericTextBox></td><td>
            <asp:RadioButton ID="rbActionMin" runat="server" Text="Minutes" GroupName="LastAction" />
            <asp:RadioButton ID="rbActionHour" runat="server" Text="Hours" GroupName="LastAction" Checked="true"/>
            <asp:RadioButton ID="rbActionDay" runat="server" Text="Days" GroupName="LastAction"/></td></tr>
        <tr><td class="col1">Time Until Closed:</td><td>
          <telerik:RadNumericTextBox ID="numClosed" Width="50px" runat="server" 
            DataType="System.Int32" MinValue="0" Value="0">
            <NumberFormat DecimalDigits="0" />
          </telerik:RadNumericTextBox></td><td>
            <asp:RadioButton ID="rbClosedMin" runat="server" Text="Minutes" GroupName="TimeClosed" />
            <asp:RadioButton ID="rbClosedHour" runat="server" Text="Hours" GroupName="TimeClosed" Checked="true"/>
            <asp:RadioButton ID="rbClosedDay" runat="server" Text="Days" GroupName="TimeClosed"/></td></tr>
            
        <tr><td colspan="4"><div style="border-bottom: solid 1px; font-size: 16px; margin-bottom: 7px; padding-top: 6px;">Notification Options</div></td></tr>    
            
      </table>
      
      <table cellpadding="0" cellspacing="5" border="0" width="500px">
      <tr>
      <td><asp:CheckBox ID="cbGroupWarnings" runat="server" Text="Notify Group of Warnings" /></td>
      <td><asp:CheckBox ID="cbUserWarnings" runat="server" Text="Notify User of Warnings" /></td>
      </tr>
      <tr>
      <td><asp:CheckBox ID="cbGroupViolations" runat="server" Text="Notify Group of Violations" /></td>
      <td><asp:CheckBox ID="cbUserViolations" runat="server" Text="Notify User of Violations" /></td>
      </tr>
      </table>
      
    </div>
  </div>
</asp:Content>

