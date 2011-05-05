<%@ Page Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true" CodeFile="NewTicket.aspx.cs" Inherits="Dialogs_NewTicket" Title="New Ticket" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div style="background-color: #BFDBFF; border: Solid 1px #A0BEE5;">
    <div style="padding: 10px 20px 10px 10px;">
      <div>
        Ticket Name</div>
      <div>
        <telerik:RadTextBox ID="textName" runat="server" Width="760px" MaxLength="255"></telerik:RadTextBox></div>
    </div>
    <telerik:RadTabStrip ID="tsMain" runat="server" MultiPageID="mpMain" SelectedIndex="0">
      <Tabs>
        <telerik:RadTab runat="server" Text="Properties"></telerik:RadTab>
        <telerik:RadTab runat="server" Text="Description"></telerik:RadTab>
        <telerik:RadTab runat="server" Text="Attachments"></telerik:RadTab>
      </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage ID="mpMain" runat="server" Height="320px" SelectedIndex="0"
      BackColor="#DBE6F4">
      <telerik:RadPageView ID="pvProperties" runat="server">
        <div id="pnlProperties" runat="server" style="height: 320px; overflow-y: auto; overflow-x: hidden;">
          <table id="tblControls" runat="server" width="775px" cellpadding="0" cellspacing="5" border="0">
            <tr>
              <td class="labelColTD">
                Ticket Type:
              </td>
              <td class="inputColTD">
                <telerik:RadComboBox ID="cmbTicketType" runat="server" Width="200px" AutoPostBack="True"
                  OnSelectedIndexChanged="cmbTicketType_SelectedIndexChanged">
                  <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                </telerik:RadComboBox>
              </td>
              <td class="labelColTD">
                Assigned To:
              </td>
              <td class="inputColTD">
                <telerik:RadComboBox ID="cmbUser" runat="server" Width="200px"></telerik:RadComboBox>
              </td>
            </tr>
            <tr>
              <td class="labelColTD">
                Status:
              </td>
              <td class="inputColTD">
                <telerik:RadComboBox ID="cmbStatus" runat="server" Width="200px"></telerik:RadComboBox>
              </td>
              <td class="labelColTD">
                Assigned Group:
              </td>
              <td class="inputColTD">
                <telerik:RadComboBox ID="cmbGroup" runat="server" Width="200px"></telerik:RadComboBox>
              </td>
            </tr>
            <tr>
              <td class="labelColTD">
                Severity:
              </td>
              <td class="inputColTD">
                <telerik:RadComboBox ID="cmbSeverity" runat="server" Width="200px"></telerik:RadComboBox>
              </td>
              <td class="labelColTD">
                <span runat="server" id="spanCustomer">Customer / Contact:</span>
              </td>
              <td class="inputColTD">
              <telerik:RadComboBox ID="cmbCustomer" runat="server" Width="200px" AllowCustomText="True"
                            MarkFirstMatch="True" ShowToggleImage="false" EnableLoadOnDemand="True" LoadingMessage="Loading..."
                            OnClientItemsRequesting="OnClientItemsRequesting" OnClientDropDownClosed="OnClientDropDownClosed"
                            EmptyMessage="" ShowDropDownOnTextboxClick="False">
                            <WebServiceSettings Path="~/Services/PrivateServices.asmx" Method="GetUserOrOrganization" />
                            <CollapseAnimation Duration="200" Type="OutQuint" />
                          </telerik:RadComboBox>
              </td>
            </tr>
            <tr>
              <td class="labelColTD">
                <span runat="server" id="spanProduct">Product: </span>
              </td>
              <td class="inputColTD">
                <telerik:RadComboBox ID="cmbProduct" runat="server" Width="200px" AutoPostBack="True"
                  OnSelectedIndexChanged="cmbProduct_SelectedIndexChanged">
                  <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                </telerik:RadComboBox>
              </td>
              <td class="labelColTD">
                <span runat="server" id="spanVersion">Product Version:</span>
              </td>
              <td class="inputColTD">
                <telerik:RadComboBox ID="cmbVersion" runat="server" Width="200px"></telerik:RadComboBox>
              </td>
            </tr>
            <tr>
              <td class="labelColTD">
                Knowledge Base:
              </td>
              <td class="inputColTD">
                <asp:CheckBox ID="cbKnowledgeBase" runat="server"  Text="" />
              </td>
              <td class="labelColTD">
                <span id="spanPortal" runat="server">Visible on Portal:</span>
              </td>
              <td class="inputColTD">
                <asp:CheckBox ID="cbPortal" runat="server" CssClass="" Text="" />
              </td>
            </tr>
          </table>
          <table id="tblCustomControls" runat="server" width="775px" cellpadding="0" cellspacing="5" border="0">
          </table>
        </div>
      </telerik:RadPageView>
      <telerik:RadPageView ID="pvDescription" runat="server">
        <div style="height: 320px; overflow-y: hidden; overflow-x: hidden;">
      <telerik:RadEditor ID="editorDescription" runat="server" EditModes="Design" ToolsFile="~/Editor/StandardTools.xml" OnClientLoad="EditorLoad" StripFormattingOnPaste="MSWord"
        BorderWidth="0px" Height="320px" Width="785px">
        <Content></Content>
      </telerik:RadEditor>
        </div>
      </telerik:RadPageView>
      <telerik:RadPageView ID="pvAttachments" runat="server">
        <div style="height: 300px; overflow-y: auto; overflow-x: hidden; padding: 20px 0 0 20px;">
          <telerik:RadUpload ID="uploadMain" runat="server" Width="100%" Skin="Vista" ControlObjectsVisibility="RemoveButtons, AddButton">
          </telerik:RadUpload>
        </div>
      </telerik:RadPageView>
    </telerik:RadMultiPage>
  </div>
  
  
    <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
    <script type="text/javascript" language="javascript">

      function EditorLoad(sender, args) {
        sender.get_contentArea().style.overflow = "auto";
      }
    
      Telerik.Web.UI.Editor.CommandList["InsertTicketLink"] = function(commandName, editor, args) {
        var elem = editor.getSelectedElement(); //returns the selected element.

        if (elem.tagName == "A") {
          editor.selectElement(elem);
          argument = elem;
        }
        else {
          var content = editor.getSelectionHtml();
          var link = editor.get_document().createElement("A");
          link.innerHTML = content;
          argument = link;
        }

        var myCallbackFunction = function(sender, args) {
          editor.pasteHtml(String.format("<a href={0} target='{1}' class='{2}'>{3}</a> ", args.href, args.target, args.className, args.name))
        }

        editor.showExternalDialog(
            '../Editor/SelectTicket.aspx',
            argument,
            290,
            200,
            myCallbackFunction,
            null,
            'Insert Ticket Link',
            true,
            Telerik.Web.UI.WindowBehaviors.Close + Telerik.Web.UI.WindowBehaviors.Move,
            false,
            false);
      };

      function OnClientItemsRequesting(sender, eventArgs) {
        var context = eventArgs.get_context();
        context["FilterString"] = eventArgs.get_text();
      }

      function OnClientDropDownClosed(sender, args) {
        sender.clearItems();
      }
      
    </script>

  </telerik:RadScriptBlock>
  </asp:Content>


