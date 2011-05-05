<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="NewTicket.aspx.cs" Inherits="Frames_NewTicket" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <link href="../css_5/jquery.ketchup.css" rel="stylesheet" type="text/css" />

  <script src="../js_5/jquery.ketchup.js" type="text/javascript"></script>

  <script src="../js_5/jquery.ketchup.validations.basic.js" type="text/javascript"></script>

  <script src="../js_5/jquery.ketchup.messages.js" type="text/javascript"></script>

  <style type="text/css">
    .reToolbar.Office2007 .InsertTicketLink { background-image: url(../images/icons/add.png) !important; }
    .reToolbar.Office2007 .InsertKBTicket { background-image: url(../images/icons/knowledge.png); }
    .reButton_text { text-indent: 0px !important; }
    body, a, span, div, td { font-size: 12px; }
    body { background-color: #fff !important; }
  </style>

  <script type="text/javascript" language="javascript">
    _doConfirmClose = true;
    window.onbeforeunload = confirmExit;
    function confirmExit() {
      if (_doConfirmClose) return "You have a new ticket that has not been saved!";
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline">
    <AjaxSettings>
      <telerik:AjaxSetting AjaxControlID="cmbTicketType">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="cmbStatus" />
          <telerik:AjaxUpdatedControl ControlID="tblCustomControls" />
          <telerik:AjaxUpdatedControl ControlID="editorDescription" />
        </UpdatedControls>
      </telerik:AjaxSetting>
      <telerik:AjaxSetting AjaxControlID="cmbProduct">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="cmbVersion" />
        </UpdatedControls>
      </telerik:AjaxSetting>
      <telerik:AjaxSetting AjaxControlID="cmbCustomer">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="cmbProduct" />
          <telerik:AjaxUpdatedControl ControlID="cmbVersion" />
        </UpdatedControls>
      </telerik:AjaxSetting>
      <telerik:AjaxSetting AjaxControlID="editorDescription">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="editorDescription" />
        </UpdatedControls>
      </telerik:AjaxSetting>
    </AjaxSettings>
  </telerik:RadAjaxManager>
  <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

    <script type="text/javascript" language="javascript">

      function pageLoad() {
        $('.validateCombo1 input.rcbInput').each(function () {
          //var s = 'validate(required, notequal(' + $(this).val() + '))';
          var s = 'validate(required, notequal(' + $(this).val() + '))';
          //var s = 'validate(required, min(5))';
          $(this).addClass(s);
        });

        $('.validateCombo input.rcbInput').each(function () {
          $(this).addClass('validate(required)');
        });
        $('.validateDateTime input.riTextBox').each(function () {
          $(this).addClass('validate(required)');
        });
      }


      function SaveNewTicket() {

        var textName = $find("<%=textName.ClientID %>");
        if (textName == null) return;
        var name = textName.get_value().trim();
        if (name == '') {
          alert('Please enter a ticket name.');
          textName.focus();
          return false;
        }

        if ($('.validateCustomer').length > 0 && $find("<%=cmbCustomer.ClientID %>").get_value().trim() == '') {
          alert('Please select a contact or customer.'); 
          return false;
        }

        if ($.validateForm('form') == false) { return false; }
        
        _doConfirmClose = false;
        return true;

      }

      function CancelNewTicket() {
        if (confirm('Are you sure you would like to cancel this new ticket?')) {
          _doConfirmClose = false;
          top.Ts.MainPage.closeNewTicketTab();
        }
        return false;
      }

      function customPickListChanged(sender, args) {
        PageMethods.GetPickListTemplateText(sender.get_text(), function(result) {
        if (result != 'void') $find("<%=editorDescription.ClientID %>").set_html($find("<%=editorDescription.ClientID %>").get_html() + '<br/><br/>' + result);
        });
      }



      function editorDescription_OnClientLoad(sender, args) {
        sender.get_contentArea().style.overflow = "auto";
        return;
        window.setTimeout(function() {
          var editorIframe = $get("<%=editorDescription.ClientID %>" + "Wrapper").getElementsByTagName("iframe")[0];
          editorIframe.style.height = "200px";
          sender.setSize(750, 200);
        }, 100);
      }


      if (Telerik && Telerik.Web.UI.Editor) {
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

      }

      function OnClientItemsRequesting(sender, eventArgs) {
        var context = eventArgs.get_context();
        context["FilterString"] = eventArgs.get_text();
      }

      function OnClientDropDownClosed(sender, args) {
        sender.clearItems();
      }

      function SaveAndClose(ticketID) {
        _doConfirmClose = false;
        top.Ts.MainPage.closeNewTicketTab();
      }

      function SaveAndView(ticketID) {
        _doConfirmClose = false;
        top.Ts.MainPage.openTicketByID(ticketID, true);
        top.Ts.MainPage.closeNewTicketTab();
      }


    </script>

  </telerik:RadScriptBlock>
  <div style="margin: 0 auto; width: 800px; text-align: center; width: 100%; overflow: auto;
    height: 100%;">
    <div style="text-align: left; margin: 0 auto; border-bottom: solid 0px #ABC1DE; padding-bottom: 5px;
      width: 800px;">
      <div id="divSaveCancel1" runat="server" style="text-align: center; margin: 10px auto 0 auto;
        border: solid 1px #EEB420; padding: 10px 10px 10px 0; background-color: #FFFBF1;"
        class="ui-corner-all">
        <asp:Button ID="bntSave1" runat="server" Text="Save" OnClick="btnSaveView_Click"
          OnClientClick="if (!SaveNewTicket()) return false;" />
        &nbsp
        <asp:Button ID="Button1" runat="server" Text="Save & Close Tab" OnClientClick="if (!SaveNewTicket()) return false;"
          OnClick="btnSave_Click" />
        &nbsp
        <asp:Button ID="Button2" runat="server" Text="Cancel" OnClientClick="return CancelNewTicket();" />
      </div>
      <div class="groupDiv groupLightBlue" style="margin: 10px 0 0 0px; width: 800px;">
        <div class="groupHeaderDiv">
          <span class="groupHeaderSpan"></span><span class="groupCaptionSpan">Ticket Properties</span>
        </div>
        <div class="groupBodyWrapperDiv">
          <div class="groupBodyDiv" style="overflow:hidden;">
            <div id="pnlProperties" runat="server">
              <table width="775px" cellpadding="0" cellspacing="5" border="0">
                <tr>
                  <td class="labelColTD">
                    Ticket Name:
                  </td>
                  <td colspan="3">
                    <div style="padding-left: 12px;">
                      <telerik:RadTextBox ID="textName" runat="server" Width="580px" MaxLength="255" CssClass="validateX(required)">
                      </telerik:RadTextBox></div>
                  </td>
                </tr>
              </table>
              <table id="tblControls" runat="server" width="775px" cellpadding="0" cellspacing="5"
                border="0">
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
                    Assigned Group:
                  </td>
                  <td class="inputColTD">
                    <telerik:RadComboBox ID="cmbGroup" runat="server" Width="200px">
                    </telerik:RadComboBox>
                  </td>
                </tr>
                <tr>
                  <td class="labelColTD">
                    Status:
                  </td>
                  <td class="inputColTD">
                    <telerik:RadComboBox ID="cmbStatus" runat="server" Width="200px">
                    </telerik:RadComboBox>
                  </td>
                  <td class="labelColTD">
                    <span runat="server" id="spanProduct">Product: </span>
                  </td>
                  <td class="inputColTD">
                    <telerik:RadComboBox ID="cmbProduct" runat="server" Width="200px" AutoPostBack="True"
                      OnSelectedIndexChanged="cmbProduct_SelectedIndexChanged">
                      <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                    </telerik:RadComboBox>
                  </td>
                </tr>
                <tr>
                  <td class="labelColTD">
                    Severity:
                  </td>
                  <td class="inputColTD">
                    <telerik:RadComboBox ID="cmbSeverity" runat="server" Width="200px">
                    </telerik:RadComboBox>
                  </td>
                  <td class="labelColTD">
                    <span runat="server" id="spanVersion">Product Version:</span>
                  </td>
                  <td class="inputColTD">
                    <telerik:RadComboBox ID="cmbVersion" runat="server" Width="200px">
                    </telerik:RadComboBox>
                  </td>
                </tr>
                <tr>
                  <td class="labelColTD">
                    Assigned To:
                  </td>
                  <td class="inputColTD">
                    <telerik:RadComboBox ID="cmbUser" runat="server" Width="200px">
                    </telerik:RadComboBox>
                  </td>
                  <td class="labelColTD">
                    <span runat="server" id="spanCustomer">Customer / Contact:</span>
                  </td>
                  <td class="inputColTD">
                    <telerik:RadComboBox ID="cmbCustomer" runat="server" Width="200px" AllowCustomText="True"
                      MarkFirstMatch="False" ShowToggleImage="false" EnableLoadOnDemand="True" LoadingMessage="Loading..."
                      OnClientItemsRequesting="OnClientItemsRequesting" OnClientDropDownClosed="OnClientDropDownClosed"
                      EmptyMessage="" ShowDropDownOnTextboxClick="False" AutoPostBack="true" OnSelectedIndexChanged="cmbCustomer_SelectedIndexChanged">
                      <WebServiceSettings Path="NewTicket.aspx" Method="GetUserOrOrganization" />
                      <CollapseAnimation Duration="200" Type="OutQuint" />
                    </telerik:RadComboBox>
                  </td>
                </tr>
                <tr>
                  <td class="labelColTD">
                    <span id="spanPortal" runat="server">Visible to Customers:</span>
                  </td>
                  <td class="inputColTD">
                    <asp:CheckBox ID="cbPortal" runat="server" CssClass="" Text="" />
                  </td>
                  <td class="labelColTD">
                    Knowledge Base:
                  </td>
                  <td class="inputColTD">
                    <asp:CheckBox ID="cbKnowledgeBase" runat="server" Text="" />
                  </td>
                </tr>
              </table>
              <table id="tblCustomControls" runat="server" width="775px" cellpadding="0" cellspacing="5"
                border="0">
              </table>
            </div>
          </div>
        </div>
      </div>
      <div class="groupDiv groupLightBlue" style="margin: 10px 0 0 0px; width: 800px;">
        <div class="groupHeaderDiv">
          <span class="groupHeaderSpan"></span><span class="groupCaptionSpan">Description</span>
        </div>
        <div class="groupBodyWrapperDiv">
          <div class="groupBodyDiv" style="overflow:hidden;">
            <div style="padding: 10px 0 0 9px;">
              <telerik:RadEditor ID="editorDescription" runat="server" EditModes="All" EnableResize="false"
                Height="400px" OnClientLoad="editorDescription_OnClientLoad" StripFormattingOptions="All" ToolsFile="~/Editor/StandardTools.xml"
                Width="780px" StripFormattingOnPaste="MSWord" >
                <Content>
            
                </Content>
                <Modules>
                  <telerik:EditorModule Name="fakeModule" />
                </Modules>
              </telerik:RadEditor>
            </div>
            <div style="padding: 20px 10px 10px 5px;">
              <h3 style="border-bottom: solid 1px #ABC1DE;">
                Attachments</h3>
              <telerik:RadUpload ID="uploadMain" runat="server" Width="100%" ControlObjectsVisibility="RemoveButtons, AddButton">
              </telerik:RadUpload>
            </div>
          </div>
        </div>
      </div>
      <div id="divSaveCancel2" runat="server" style="text-align: center; margin: 0 auto 0 auto;
        border: solid 1px #EEB420; padding: 10px 10px 10px 0; margin: 10px 0; background-color: #FFFBF1;"
        class="ui-corner-all">
        <asp:Button ID="Button3" runat="server" Text="Save" OnClick="btnSaveView_Click" OnClientClick="if (!SaveNewTicket()) return false;" />
        &nbsp
        <asp:Button ID="btnSave2" runat="server" Text="Save & Close Tab" OnClick="btnSave_Click"
          OnClientClick="if (!SaveNewTicket()) return false;" />
        &nbsp
        <asp:Button ID="Button4" runat="server" Text="Cancel" OnClientClick="return CancelNewTicket();" />
      </div>
    </div>
  </div>
</asp:Content>
