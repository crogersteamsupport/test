<%@ Page Title="Portal Options" Language="C#" MasterPageFile="~/Dialogs/Dialog.master"
  AutoEventWireup="true" CodeFile="PortalOptions.aspx.cs" Inherits="Dialogs_PortalOptions"
  ValidateRequest="false" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <style type="text/css">
    div
    {
      padding-bottom: 3px;
    }
  </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div class="dialogContentWrapperDiv" style="position:relative;">
    <div class="dialogContentDiv" >
      <div>
        <table width="100%" cellpadding="5px" cellspacing="0" border="0">
          <tr>
            <td>
              Default Portal Group:
            </td>
            <td>
              <telerik:RadComboBox ID="cmbGroups" runat="server" Width="225px"></telerik:RadComboBox>
            </td>
            <td>
              Portal Name:
            </td>
            <td>
              <telerik:RadTextBox ID="textPortalName" runat="server" Width="225px"></telerik:RadTextBox>
            </td>
          </tr>

          <tr>
            <td>
              Use Company in Basic Portal:
            </td>
            <td>
              <asp:CheckBox ID="cbUseCompanyInBasic" runat="server" />
            </td>
            <td>
              Company Required in Basic Portal:
            </td>
            <td>
              <asp:CheckBox ID="cbCompanyRequiredInBasic" runat="server" />
            </td>
          </tr>
          <tr>
            <td>
              Hide User Assigned To:
            </td>
            <td>
              <asp:CheckBox ID="cbHideUserAssignedTo" runat="server" />
            </td>
            <td>
              Hide Group Assigned To:
            </td>
            <td>
              <asp:CheckBox ID="cbHideGroupAssignedTo" runat="server" />
            </td>
          </tr>
          <tr>
            <td>
              Display Product Pulldown in Portals:
            </td>
            <td>
              <asp:CheckBox ID="cbProduct" runat="server" />
            </td>
            <td>
              Display Product Version Pulldown in Portals:
            </td>
            <td>
              <asp:CheckBox ID="cbVersion" runat="server" />
            </td>
          </tr>
          <tr>
            <td>
              Allow Access to Public Knowledgebase:
            </td>
            <td>
              <asp:CheckBox ID="cbKb" runat="server" />
            </td>
            <td>
              Allow Access to Public Articles:
            </td>
            <td>
              <asp:CheckBox ID="cbArticles" runat="server" />
            </td>
          </tr>
          <tr>
            <td>
              Display Group Pulldown in Portals:
            </td>
            <td>
              <asp:CheckBox ID="cbGroup" runat="server" />
            </td>
            <td>
              Use Recaptcha:
            </td>
            <td>
              <asp:CheckBox ID="cbRecaptcha" runat="server" />
            </td>
          </tr>
          <tr>
            <td>
              Theme:
            </td>
            <td>
              <telerik:RadComboBox ID="cmbTheme" runat="server" Width="225px"></telerik:RadComboBox>
            </td>
            <td>
            </td>
            <td>
            </td>
          </tr>
          <tr>
            <td>
              Advanced Portal Width:
            </td>
            <td>
              <telerik:RadNumericTextBox ID="textAdvWidth" runat="server" Width="225px"></telerik:RadNumericTextBox>
            </td>
            <td>
              Basic Portal Width:
            </td>
            <td>
              <telerik:RadNumericTextBox ID="textWidth" runat="server" Width="225px"></telerik:RadNumericTextBox>
            </td>
          </tr>
          <tr>
            <td>
              Basic Portal Header Message:
            </td>
            <td>
              <telerik:RadTextBox ID="textBasicPortalDirections" runat="server" Width="225px"></telerik:RadTextBox>
            </td>
            <td>
              Ticket Deflection Enabled:
            </td>
            <td>
              <asp:CheckBox ID="cbDeflection" runat="server" />
            </td>
          </tr>
        </table>
      </div>
      <br />
      <div>
        External Portal Link: &nbsp&nbsp
        <span id="spanExternalLinkHelp" runat="server" style="text-decoration: underline;
          cursor: pointer;">What is this?</span><telerik:RadToolTip ID="tipExternalLink" runat="server"
            Animation="Fade" ShowEvent="OnClick" Text="This allows you to define an external link to embed the TeamSupport Portal."
            TargetControlID="spanExternalLinkHelp"></telerik:RadToolTip>
      </div>
      
      <div>
        <telerik:RadTextBox ID="textExternalLink" runat="server" Width="100%"></telerik:RadTextBox></div>
      <br />
      
      <div>
        Portal Header HTML/CSS:</div>
      <div>
        <telerik:RadTextBox ID="textHeader" runat="server" Width="100%" Height="100px" TextMode="MultiLine">
        </telerik:RadTextBox></div>
      <br />
      <div>
        Portal Footer HTML:</div>
      <div>
        <telerik:RadTextBox ID="textFooter" runat="server" Width="100%" Height="100px" TextMode="MultiLine">
        </telerik:RadTextBox></div>
    </div>
  </div>


</asp:Content>
