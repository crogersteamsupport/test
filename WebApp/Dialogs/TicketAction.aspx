<%@ Page Language="C#" Title="Ticket Action" AutoEventWireup="true" CodeFile="TicketAction.aspx.cs"
  Inherits="Dialogs_TicketAction" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
  <title></title>

    <link href="../css_5/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <script src="../js_5/jquery-1.4.2.min.js" type="text/javascript"></script>
  <script src="../js_5/jquery-ui-latest.custom.min.js" type="text/javascript"></script>

  <style type="text/css">
    .reToolbar.Office2007 .InsertTicketLink { background-image: url(../images/icons/add.png); }
    .reToolbar.Office2007 .InsertKBTicket { background-image: url(../images/icons/knowledge.png); }
    .reButton_text { text-indent: 0px !important; }
    body.bodyWindow { background: #BFDBFF; font-family: "Lucida Grande" ,Helvetica,Arial,Verdana,sans-serif; font-size: 12px; font-weight: bold; color: #15428B; }
    body { overflow: hidden; }
    .tdLabel { width: 150px; }
  </style>
</head>
<body class="bodyWindow">
  <form id="form1" runat="server">
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

    <script type="text/javascript" language="javascript">
      var _action = null;
      var _actionType = null;



      function EditorLoad(sender, args) {
        sender.get_contentArea().style.overflow = "auto";

        window.setTimeout(function() {
          var editorIframe = $get("editorDescriptionWrapper").getElementsByTagName("iframe")[0];
          editorIframe.style.height = "140px";
          sender.setSize(765, 140);
        }, 100);
      }

      function DeleteActionAttachment(id) {

        if (!confirm('Are you sure you would like to delete this attachment?')) return;
        top.privateServices.DeleteAttachment(id, LoadAttachments);

      }

      function save() {
        if ($('#fieldRequireTime').val() == 1 && _actionType.IsTimed && $find("<%=numSpent.ClientID%>").get_value() == '') {
          alert('Please enter the time you spent on this action.');
          return false;
        }
      }

      function LoadAttachments() {

      }

      function OnActionTypeChanged(sender, args) {
        LoadActionType(args.get_item().get_value());

      }

      function LoadActionType(actionTypeID) {
        if (!actionTypeID || actionTypeID < 0) {
          $('#trTime').hide();
        }
        else {
          PageMethods.GetActionType(actionTypeID, LoadActionTypeResult);
        }


      }

      function LoadActionTypeResult(actionType) {
        _actionType = actionType;
        if (!actionType || !actionType.IsTimed)
          $('#trTime').hide();
        else
          $('#trTime').show();

      }

      function ClearData() {
        $('#divAttachments').html('');
        $find("<%=editorDescription.ClientID%>").set_html('');
        $find("<%=textName.ClientID%>").set_value('');
        $find("<%=numSpent.ClientID%>").set_value('');
        $find("<%=dtpStarted.ClientID%>").set_selectedDate(new Date());
        $find("<%=cmbActionType.ClientID%>").get_items().getItem(0).select();
        $get("<%=cbKnowledge.ClientID%>").checked = false;
        $get("<%=cbPortal.ClientID%>").checked = false;
        //LoadActionType($find("<%=cmbActionType.ClientID%>").get_items().getItem(0).get_value());
        $('#trTitle').show();
      }

      function cleanHtml() {
        var txt = $find("<%=editorDescription.ClientID%>").get_html();
        PageMethods.Clean(txt, function(result) {
          $find("<%=editorDescription.ClientID%>").set_html(result);
        });


      }


      function ShowLoading() {
        $find("<%= RadAjaxLoadingPanel1.ClientID %>").show('divMain');
      }

      function HideLoading() {
        $find("<%= RadAjaxLoadingPanel1.ClientID %>").hide('divMain');
      }

      function LoadAction(actionID, ticketID) {
        $get('fieldActionID').value = actionID;
        $get('fieldTicketID').value = ticketID;
        ShowLoading();
        if (actionID > -1) {
          PageMethods.GetAction(actionID, LoadActionResult);
        } else {
          ClearData();
          HideLoading();
        }
      }

      function LoadActionResult(action) {
        if (action.SystemActionTypeID == 1) {
          $('#trTitle').hide();
          $('#trTime').hide();
        }
        $find("<%=editorDescription.ClientID%>").set_html(action.Description);
        $find("<%=textName.ClientID%>").set_value(action.Name);
        $find("<%=numSpent.ClientID%>").set_value(action.TimeSpent);
        $find("<%=dtpStarted.ClientID%>").set_selectedDate(action.DateStarted);
        $get("<%=cbKnowledge.ClientID%>").checked = action.IsKnowledgeBase;
        $get("<%=cbPortal.ClientID%>").checked = action.IsVisibleOnPortal;

        if (action.SystemActionTypeID == 0) {
          var combo = $find("<%=cmbActionType.ClientID%>")
          var item = combo.findItemByValue(action.ActionTypeID);
          if (item) item.select();
          else combo.get_items().getItem(0).select();

        }
        HideLoading();
      }

      function GetRadWindow() {
        if (window.radWindow) {
          return window.radWindow;
        }
        if (window.frameElement && window.frameElement.radWindow) {
          return window.frameElement.radWindow;
        }
        return null;
      }

      function CloseWindow() {
        GetRadWindow().Close();
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


      Telerik.Web.UI.Editor.CommandList["InsertKBTicket"] = function (commandName, editor, args) {



        var insertKB = function (sender, args) {
          PageMethods.GetTicketActions(args, function (result) {
            html = '<div>';
            html = html + String.format('<h2>{0}</h2>', result.Ticket.Name);

            for (var i = 0; i < result.Actions.length; i++) {
              var action = result.Actions[i];
              html = html + String.format('<div>{0}</div></br>', action.Description);
            }

            html = html + '</div>';
            editor.pasteHtml(html);
          });
          //editor.pasteHtml(String.format("<a href={0} target='{1}' class='{2}'>{3}</a> ", args.href, args.target, args.className, args.name))
        }


        editor.showExternalDialog(
            '../Editor/SelectKBTicket.aspx',
            null,
            290,
            200,
            insertKB,
            null,
            'Insert Knowledgebase',
            true,
            Telerik.Web.UI.WindowBehaviors.Close + Telerik.Web.UI.WindowBehaviors.Move,
            false,
            false);
      };
      
    </script>

  </telerik:RadScriptBlock>
  <telerik:RadScriptManager ID="RadScriptManager1" runat="server" EnablePageMethods="true">
  </telerik:RadScriptManager>
  <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
  </telerik:RadAjaxManager>
  <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server">
  </telerik:RadAjaxLoadingPanel>
  <asp:Literal ID="litScript" runat="server" Text=""></asp:Literal>
  <div id="divMain">
    <table cellpadding="0" cellspacing="5" border="0" width="780px">
      <tr id="trTitle">
        <td class="tdLabel">
          Action Title:
        </td>
        <td>
          <telerik:RadTextBox ID="textName" runat="server" Width="197px">
          </telerik:RadTextBox>
        </td>
        <td class="tdLabel" id="tdActionTypeLabel" runat="server">
          Action Type:
        </td>
        <td id="tdActionTypeInput" runat="server">
          <telerik:RadComboBox ID="cmbActionType" runat="server" Width="200px" OnClientSelectedIndexChanged="OnActionTypeChanged">
          </telerik:RadComboBox>
        </td>
      </tr>
      <tr>
        <td class="tdLabel">
          Knowledge Base:
        </td>
        <td>
          <asp:CheckBox ID="cbKnowledge" runat="server" CssClass="" Text="" />
        </td>
        <td class="tdLabel">
          Visible to Customers:
        </td>
        <td>
          <asp:CheckBox ID="cbPortal" runat="server" CssClass="" Text="" />
        </td>
      </tr>
      <tr id="trTime">
        <td class="tdLabel">
          Date Started:
        </td>
        <td>
          <telerik:RadDateTimePicker ID="dtpStarted" runat="server" Width="200px">
          </telerik:RadDateTimePicker>
        </td>
        <td class="tdLabel">
          Time Spent (minutes):
        </td>
        <td>
          <telerik:RadNumericTextBox ID="numSpent" runat="server" Width="197px" DataType="System.Int32">
          </telerik:RadNumericTextBox>
        </td>
      </tr>
    </table>
    <telerik:RadTabStrip ID="tsMain" runat="server" SelectedIndex="0" MultiPageID="mpMain"
      Width="768px">
      <Tabs>
        <telerik:RadTab runat="server" Text="Description">
        </telerik:RadTab>
        <telerik:RadTab runat="server" Text="Attachments" Selected="True">
        </telerik:RadTab>
      </Tabs>
    </telerik:RadTabStrip>
    <div style="border-bottom: solid 1px #8DB2E3; border-left: solid 1px #8DB2E3; border-right: solid 1px #8DB2E3;">
      <telerik:RadMultiPage ID="mpMain" runat="server" SelectedIndex="0" Height="200px"
        BackColor="#DBE6F4">
        <telerik:RadPageView ID="pvDescription" runat="server">
          <telerik:RadEditor ID="editorDescription" runat="server" EditModes="All" ToolsFile="~/Editor/StandardTools.xml"
            StripFormattingOnPaste="MSWord" OnClientLoad="EditorLoad" BorderWidth="0px" Width="765px" 
            Height="140px" EnableResize="false">
            <Content>
            </Content>
            <Modules>
              <telerik:EditorModule Name="fakeModule" />
            </Modules>
          </telerik:RadEditor>
        </telerik:RadPageView>
        <telerik:RadPageView ID="pvAttachments" runat="server">
          <div style="padding: 7px 0 10px 10px;">
            <div id="divAttachments">
            </div>
            <br />
            <telerik:RadUpload ID="uploadMain" runat="server" Width="100%" ControlObjectsVisibility="RemoveButtons, AddButton">
            </telerik:RadUpload>
          </div>
        </telerik:RadPageView>
      </telerik:RadMultiPage>
    </div>
    <div style="text-align: right; padding-top: 20px;">
      <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" OnClientClick="return save();" />
      <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="return CloseWindow();" />
    </div>
  </div>
  <asp:HiddenField ID="fieldActionID" runat="server" Value="-187" />
  <asp:HiddenField ID="fieldTicketID" runat="server" Value="-1" />
  <asp:HiddenField ID="fieldRequireTime" runat="server" Value="0" />
  </form>
</body>
</html>
