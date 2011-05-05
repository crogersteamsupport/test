<%@ Page Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="TicketPreview.aspx.cs" Inherits="Frames_TicketPreview" Title="Untitled Page" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../UserControls/Actions.ascx" TagName="Actions" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <style type="text/css">
    body, html, form { overflow: hidden; background-color: #ffffff; }
    
    .ticketDetailsHeaderDiv { height: 30px; line-height: 30px; vertical-align: middle; width: 100%; font-size: 14px; font-weight: bold; color: #003399; white-space: nowrap; }
    .ticketDetailsInfoDiv strong { color: #4E6BA3; font-weight: normal; font-size: 12px; padding-right: 5px; }
    .ticketDetailsInfoDiv { font-size: 12px; }
    
    .ticketDetailsInfoDiv span { font-size: 12px; }
    
    .ticketPropertiesDiv { width: 100%; height: 105px; border-bottom: Solid 1px #003399; padding-bottom: 5px; padding-left: 5px; }
    #spanTagList { padding: 5px 10px; }
    .tag-span { padding-right: 15px; }
    .tag-span sup { padding-left: 3px;} 
    .tag-span span { font-weight:normal; } 

  </style>

  <script type="text/javascript" language="javascript">
    function pageLoad() {
      loadTags();
    }

    function GetTicketID() {
      var id = $get("<%=fieldTicketID.ClientID %>").value;
      if (id < 0) window.location = window.location;
      else return id;
    }

    function OnClientItemsRequesting(sender, eventArgs) {
      var context = eventArgs.get_context();
      context["FilterString"] = eventArgs.get_text();
    }

    function OnClientDropDownClosed(sender, args) {
      sender.clearItems();
    }

    function OnClientFocus(sender, args) {
      sender.set_value('');
      sender.set_text('');
      sender.clearItems();
      sender.clearSelection();
    }

    function cmbTag_OnClientKeyPressing(sender, args) {
      if (args.get_domEvent().keyCode == 13) {
        AddTag();
      }
      else if (args.get_domEvent().keyCode == 27) {
        combo.set_value('');
        combo.set_text('');
        combo.clearSelection();
        combo.clearItems();
        combo._applyEmptyMessage();
      }
    }

    function cmbTag_OnClick() {
      var combo = $find("<%=cmbTag.ClientID %>");
      combo.set_text('');
      combo.set_value('');
      combo.clearItems();
      combo.clearSelection();
    }

    function AddTag() {
      var combo = $find("<%=cmbTag.ClientID %>");
      var text = combo.get_text();
      if (!text || text == combo.get_emptyMessage()) return;
      top.privateServices.AddTicketTagByValue(GetTicketID(), text, loadTags);
      combo.get_inputDomElement().focus();
      combo.set_text('');
      loadTags();
    }

    function deleteTag(tagID) {
      if (!confirm('Are you sure you would like to delete this tag?')) return;
      PageMethods.DeleteTicketTag(tagID, GetTicketID(), loadTags);
    }

    function loadTags() {
      PageMethods.GetTags(GetTicketID(), function (result) {
        if (result.length < 1) {
          $('#spanTagList').html('<br/>There are no tags associated.');
        }
        else {
          var html = '';
          for (var i = 0; i < result.length; i++) {
            var tag = result[i];
            html = html +
             '<span class="tag-span"><span>' + tag.Value + '</span>' +
            //'<span class="tag-span"><a class="ts-link" title="Open" href="#" onclick="return false;">' + tag.Value + '</a>' +
             '<sup>(<a class="removeLink" title="Remove tag." href="#" onclick="deleteTag(' + tag.TagID + ')">x</a>)</sup></span>';
          }
          $('#spanTagList').html(html);
        }
      });
    }

  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div style="width: 100%; height: 100%; overflow: auto;">
    <div id="ticketPropertiesDiv" runat="server" class="ticketPropertiesDiv">
      <div class="ticketDetailsHeaderDiv">
        <asp:Label ID="lblTicketCaption" runat="server" Text="[No Ticket Selected]"></asp:Label>
      </div>
      <div class="ticketDetailsInfoDiv">
        <table border="0" cellpadding="0" cellspacing="0" width="100%">
          <tr>
            <td><strong>Ticket Type:</strong><asp:Label ID="lblTicketType" runat="server" Text=""></asp:Label>
            </td>
            <td><strong>Assigned To:</strong><asp:Label ID="lblUser" runat="server" Text=""></asp:Label>
            </td>
            <td><strong>Group:</strong><asp:Label ID="lblGroup" runat="server" Text=""></asp:Label>
            </td>
            <td><span runat="server" id="spanReported"><strong>Version Reported:</strong><asp:Label
              ID="lblReproted" runat="server" Text=""></asp:Label>
            </span></td>
          </tr>
          <tr>
            <td><strong>Status:</strong><asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
            </td>
            <td><strong>Severity:</strong><asp:Label ID="lblSeverity" runat="server" Text=""></asp:Label>
            </td>
            <td><span runat="server" id="spanProduct"><strong>Product:</strong><asp:Label ID="lblProduct"
              runat="server" Text=""></asp:Label>
            </span></td>
            <td><span runat="server" id="spanResolved"><strong>Version Resolved:</strong><asp:Label
              ID="lblSolved" runat="server" Text=""></asp:Label>
            </span></td>
          </tr>
        </table>
        <div>
          <span runat="server" id="spanCustomers"><strong>Customers:</strong><asp:Label ID="lblCustomers"
            runat="server" Text=""></asp:Label>
          </span>
        </div>
        <div runat="server" id="divTags">
          <table border="0" cellpadding="0" cellspacing="0">
            <tr>
              <td> Tags:
              </td>
              <td style="vertical-align:text-top;">
                <span id="spanTagList">
                </span>
              </td>
              <td>
                <telerik:RadComboBox ID="cmbTag" runat="server" Width="200px" AllowCustomText="True"
                  MarkFirstMatch="True" ShowToggleImage="false" EnableLoadOnDemand="True" LoadingMessage="Loading..."
                  OnClientItemsRequesting="OnClientItemsRequesting" OnClientDropDownClosed="OnClientDropDownClosed"
                  EmptyMessage="Enter a Tag" OnClientKeyPressing="cmbTag_OnClientKeyPressing" ShowDropDownOnTextboxClick="False"
                  onclick="cmbTag_OnClick();" DropDownWidth="400px" OnClientFocus="OnClientFocus">
                  <WebServiceSettings Path="~/Services/PrivateServices.asmx" Method="GetTicketTags" />
                  <CollapseAnimation Duration="200" Type="OutQuint" />
                </telerik:RadComboBox>
              </td>
              <td>
                <img alt="Clear" src="../images/icons/add.png" style="padding: 3px 5px 0 2px; cursor: pointer;"
                  onclick="AddTag();" />
              </td>
            </tr>
          </table>
        </div>
      </div>
    </div>
    <asp:HiddenField ID="fieldTicketID" runat="server" />

    <uc1:Actions ID="ucActions" runat="server" />
  </div>
</asp:Content>
