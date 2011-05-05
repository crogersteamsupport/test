<%@ Page Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true"
  CodeFile="Action.aspx.cs" Inherits="Dialogs_Action" Title="Action" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <style type="text/css">
    .reToolbar.Office2007 .InsertTicketLink
    {
    
      background-image: url(../images/icons/add.png);
    }
    
.reButton_text
{
    text-indent:0px !important;
}    
  </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div id="pnlProperties" runat="server" style="background-color: #BFDBFF; border: Solid 1px #A0BEE5;">
    <table cellpadding="0" cellspacing="5" border="0">
      <tr>
        <td class="labelColTDx">
          Action Title:
        </td>
        <td class="inputColTDx">
          <telerik:RadTextBox ID="textName" runat="server" Width="197px"></telerik:RadTextBox>
        </td>
        <td class="labelColTDx" id="tdActionTypeLabel" runat="server">
          Action Type:
        </td>
        <td class="inputColTDx" id="tdActionTypeInput" runat="server">
          <telerik:RadComboBox ID="cmbActionType" runat="server" Width="200px" AutoPostBack="True">
          </telerik:RadComboBox>
        </td>
      </tr>
      <tr>
        <td class="labelColTDx">
          Knowledge Base:
        </td>
        <td class="inputColTDx">
          <asp:CheckBox ID="cbKnowledge" runat="server" CssClass="" Text="" />
        </td>
        <td class="labelColTDx" id="td1" runat="server">
          Visible on Portal:
        </td>
        <td class="inputColTDx" id="td2" runat="server">
          <asp:CheckBox ID="cbPortal" runat="server" CssClass="" Text="" />
        </td>
      </tr>
      <tr id="trTime" runat="server">
        <td class="labelColTDx">
          Date Started:
        </td>
        <td class="inputColTDx">
          <telerik:RadDateTimePicker ID="dtpStarted" runat="server" Width="200px"></telerik:RadDateTimePicker>
        </td>
        <td class="labelColTDx">
          Time Spent (minutes):
        </td>
        <td class="inputColTDx">
          <telerik:RadNumericTextBox ID="numSpent" runat="server" Width="197px" DataType="System.Int32">
          </telerik:RadNumericTextBox>
        </td>
      </tr>
    </table>
  </div>
  <telerik:RadTabStrip ID="tsMain" runat="server" SelectedIndex="0" MultiPageID="mpMain"
    Width="100%">
    <Tabs>
      <telerik:RadTab runat="server" Text="Description"></telerik:RadTab>
      <telerik:RadTab runat="server" Text="Attachments" Selected="True"></telerik:RadTab>
    </Tabs>
  </telerik:RadTabStrip>
  <telerik:RadMultiPage ID="mpMain" runat="server" SelectedIndex="0" Height="300px"
    BackColor="#DBE6F4">
    <telerik:RadPageView ID="pvDescription" runat="server">
      <telerik:RadEditor ID="editorDescription" runat="server" EditModes="Design" ToolsFile="~/Editor/StandardTools.xml" OnClientLoad="EditorLoad"
        BorderWidth="0px" Width="785px" Height="290px" StripFormattingOnPaste="MSWord">
        <Content>
</Content>
      </telerik:RadEditor>
    </telerik:RadPageView>
    <telerik:RadPageView ID="pvAttachments" runat="server">
      <div style="padding: 7px 0 10px 10px;">
        <asp:Repeater ID="rptAttachments" runat="server">
          <ItemTemplate>
            <div class="repeaterItem">
              <img src="../images/icons/Trash.png" alt="Trash" onclick="DeleteActionAttachment(<%# DataBinder.Eval(Container.DataItem, "AttachmentID")%>);  return false;" />
              <span>
                <%# DataBinder.Eval(Container.DataItem, "FileName")%></span>
            </div>
          </ItemTemplate>
        </asp:Repeater>
        <br />
        <telerik:RadUpload ID="uploadMain" runat="server" Width="100%" ControlObjectsVisibility="RemoveButtons, AddButton">
        </telerik:RadUpload>
      </div>
    </telerik:RadPageView>
  </telerik:RadMultiPage>
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

    <script type="text/javascript" language="javascript">
      function EditorLoad(sender, args) {
        sender.get_contentArea().style.overflow = "auto";
      }
    
      function DeleteActionAttachment(id) {

        if (!confirm('Are you sure you would like to delete this attachment?')) return;
        top.privateServices.DeleteAttachment(id, AttachmentDeleted);

      }

      function AttachmentDeleted(result) {
        setTimeout('SendAjaxRequest()', 500);
      }

      function SendAjaxRequest() {
        var ajaxManager = $find("<%=RadAjaxManager.GetCurrent(Page).ClientID  %>");
        ajaxManager.ajaxRequest();
        return false;
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
      
    </script>

  </telerik:RadScriptBlock>
</asp:Content>
