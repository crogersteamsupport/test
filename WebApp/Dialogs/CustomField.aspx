<%@ Page Title="Custom Field" Language="C#" MasterPageFile="~/Dialogs/Dialog.master"
  AutoEventWireup="true" CodeFile="CustomField.aspx.cs" Inherits="Dialogs_CustomField" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <style type="text/css">
    .inputDiv
    {
      padding-bottom: 10px;
      padding-top: 2px;
    }
  </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
      <div id="divMain" runat="server">
        <div>
          Name:</div>
        <div class="inputDiv">
          <telerik:RadTextBox ID="textName" runat="server" MaxLength="50" Width="350px"></telerik:RadTextBox></div>
        <div>
          API Field Name:</div>
        <div class="inputDiv">
          <telerik:RadTextBox ID="textApiFieldName" runat="server" MaxLength="50" Width="350px"></telerik:RadTextBox></div>
        <div>
          Property Type:</div>
        <div class="inputDiv">
          <telerik:RadComboBox ID="comboFieldType" runat="server" AutoPostBack="true" Width="350px">
          </telerik:RadComboBox>
        </div>
        <div id="pnlPickList" runat="server">
          <div>
            Pick List Values:</div>
          <div class="inputDiv">
            <asp:TextBox ID="textList" runat="server" Height="50px" Width="350px" Wrap="True"
              TextMode="MultiLine" MaxLength="8000" Style="overflow: auto;" Rows="400" Columns="30"></asp:TextBox>
          </div>
          <div><asp:CheckBox ID="cbFirstSelect" runat="server" Text="The first value is not a valid selection for a required field." /></div>
        </div>
          <div><asp:CheckBox ID="cbIsRequired" runat="server" Text="A value is required" /></div>
          <div><asp:CheckBox ID="cbIsRequiredToClose" runat="server" Text="A value is required prior to closing ticket" /></div>
        <br />
        <div>
          Description:</div>
        <div class="inputDiv">
          <telerik:RadTextBox ID="textDescription" runat="server" MaxLength="250" Width="350px">
          </telerik:RadTextBox></div>
          <div>
            <asp:CheckBox ID="cbIsVisibleOnPortal" runat="server" Text="Visible on Portal" />
          
          </div>
        <div ID="maskDiv" runat="server" style="margin-top: 15px;">
          <div>
            Mask: -leave blank for no mask (What's an edit mask? <a href="https://sites.google.com/site/teamsupportdocumentation/administration/custom-fields/#inputMask" target="_blank">Click here</a>)
          </div>
          <div class="inputDiv">
            <telerik:RadTextBox ID="textMask" runat="server" MaxLength="250" Width="350px">
            </telerik:RadTextBox>
          </div>
        </div>
      </div>
    </div>
  </div>
</asp:Content>
