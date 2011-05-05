<%@ Page Title="Ticket" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true"
  CodeFile="Ticket2.aspx.cs" Inherits="Frames_Ticket" ValidateRequest="false" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../UserControls/Actions.ascx" TagName="Actions" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <style type="text/css">
    .reToolbar.Office2007 .InsertTicketLink { background-image: url(../images/icons/add.png) !important; }
    .reButton_text { text-indent: 0px !important; }
    .iconImg { border: 0; cursor: pointer; line-height: 30px; }
    .RightAlignToolbar .rtbUL { float: right !important; padding-right: 7px !important; }
    body, a, span, div, td { font-size: 12px; }
    body { background-color: #fff !important; }
    sup, a.removeLink { font-size: 12px; color: #93A7BF; }
    a.customerLink { font-size: 12px; white-space: nowrap; }
    td.labelColTD { padding-left: 20px !important; }
    .spanWarning { color: Red; }
    .spanViolation { color: Red; }
    .spanSla { cursor: default; vertical-align: top; padding-right: 10px; height: 16px; line-height: 16px; }
    img.infoImg { height: 18px; width: 18px; }
    .actionCaptionDiv { font-size: 14px; font-weight: bold; }
    .actionBodyDiv { padding: 10px 40px 0 15px; }
    .actionAttachmentsDiv { padding-left: 15px; }
    .actionTimeDiv { padding-left: 15px; }
    .actionPropertiesDiv { padding-left: 15px; }
    .actionFooterDiv { font-style: italic; border-bottom: dotted 0px #97aad1; margin-right: 20px; padding-bottom: 7px; padding-left: 15px; }
    .groupBarCaptionSpan { padding-left: 0px !important; }
  </style>
  <telerik:RadScriptBlock ID="RadScriptBlock2" runat="server">

    <script type="text/javascript" language="javascript">

      _doConfirmClose = false;
      window.onbeforeunload = confirmExit;
      function confirmExit() {
        if (_doConfirmClose) return "Changes to your ticket have not been saved!";
      }


      var _actionID = -1;
      var _ticket = null;
      var _loadingStatuses = false;
      var _loadingProducts = false;
      var _loadingVersions = false;
      var _loadingDetails = true;

      $(document).ready(function() {
        
      });

      function pageLoad(sender, args) {
        if (!args.get_isPartialLoad()) {
          if (!top.DeleteTab) {
            $("#" + "<%=btnSaveClose.ClientID %>").hide();
            $("#" + "<%=btnSaveClose2.ClientID %>").hide();
          }


          SetUserEmailLink();
          PageMethods.GetTicket(GetTicketID(), function(result) {
            _ticket = result;
            LoadStatusesCombo(_ticket.TicketTypeID, _ticket.TicketStatusID);
            LoadProductsCombo(_ticket.ProductID, _ticket.ReportedVersionID, _ticket.SolvedVersionID);
            LoadSlaTips();
            
            _loadingDetails = false;
          });
        }
      }


      function LoadStatusesCombo(ticketTypeID, statusID) {
        PageMethods.GetStatuses(ticketTypeID, statusID, function(statuses) {
          _loadingStatuses = true;
          try {
            LoadCombo($find("<%=cmbStatus.ClientID %>"), statuses, statusID);
          }
          finally {
            _loadingStatuses = false;
          }
        });
      }

      function LoadProductsCombo(productID, reportedID, resolvedID) {
        var combo = $find("<%=cmbProduct.ClientID %>");
        if (!combo) return;
        PageMethods.GetProducts(GetTicketID(), function(products) {
          _loadingProducts = true;
          try {
            LoadCombo(combo, products, productID);
            LoadVersionCombos(productID, reportedID, resolvedID);
          }
          finally {
            _loadingProducts = false;
          }

        });
      }


      function LoadVersionCombos(productID, reportedID, resolvedID) {
        PageMethods.GetVersions(productID, GetTicketID(), function(versions) {
          _loadingVersions = true;
          try {
            LoadCombo($find("<%=cmbReported.ClientID %>"), versions, reportedID);
            LoadCombo($find("<%=cmbResolved.ClientID %>"), versions, resolvedID);
            SetVersionLinks();
          }
          finally {
            _loadingVersions = false;
          }

        });
      }

      function LoadCombo(combo, items, selectedValue) {
        combo.trackChanges();
        combo.clearItems();

        for (var i = 0; i < items.length; i++) {
          var item = new Telerik.Web.UI.RadComboBoxItem();
          item.set_text(items[i].Text);
          var value = items[i].Value;
          item.set_value(value);
          combo.get_items().add(item);
        }
        combo.commitChanges();


        if (selectedValue) {
          var selected = combo.findItemByValue(selectedValue);
          if (selected) selected.select();
        }
        else if (!combo.get_selectedIndex() && combo.get_items().get_count() > 0) {
          combo.get_items().getItem(0).select();
        }



      }

      function SetComboValue(combo, value) {
        var item = combo.findItemByValue(value);
        if (item) { item.select(); }

        if (!combo.get_selectedIndex() && combo.get_items().get_count() > 0) { combo.get_items().getItem(0).select(); }
      }

      function SetVersionLinks() {
        SetVersionLink('lnkResolved', GetResolvedID());
        SetVersionLink('lnkReported', GetReportedID());
      }

      function SetVersionLink(link, id) {
        if (!id || id < 0) {
          $('#' + link).hide();
        }
        else {
          $('#' + link).show();
          $('#' + link).attr('href', 'javascript:top.OpenProduct(' + GetProductID() + ',' + id + ');');
        }
      }

      function SetUserEmailLink() {
        var userID = GetUserID();
        if (userID < 0) {
          $('#lnkUserEmail').hide();
        }
        else {
          PageMethods.GetTicketEmailLink(GetTicketID(), userID, function(result) {
            $('#lnkUserEmail').show();
            $('#lnkUserEmail').attr('href', result);
          });
        }
      }

      function SaveTicket(doClose) {
        HideSaveDiv();
        SaveCustomControls()
        /*
        try {
        var dt = new Date();
        top.privateServices.SaveCustomFieldDate(1, 2, dt.format('s'), function(result) { alert('success'); }, function(result) { alert(result.get_message()); });
        } catch (err) { alert(err.message); }
        return;
        */

        LoadPropertiesDiv();
        _ticket.TicketTypeID = GetTicketTypeID();
        _ticket.TicketStatusID = GetStatusID();
        _ticket.TicketSeverityID = GetSeverityID();
        _ticket.UserID = GetUserID();
        _ticket.GroupID = GetGroupID();
        _ticket.ProductID = GetProductID();
        _ticket.ReportedVersionID = GetReportedID();
        _ticket.SolvedVersionID = GetResolvedID();
        _ticket.IsVisibleOnPortal = document.getElementById("<%=cbPortal.ClientID %>").checked;
        _ticket.IsKnowledgeBase = document.getElementById("<%=cbKnowledgeBase.ClientID %>").checked;

        PageMethods.SaveTicket(_ticket, function(result) {
          if (doClose) setTimeout('CloseTab();', 200);
        });
      }

      function CloseTab() {
        if (top.DeleteTab) top.DeleteTab(top.TABTYPE_TICKET, GetTicketID(), true);
      }

      function CancelPropertyEdit() {
        HideSaveDiv();

        _loadingDetails = true;
        PageMethods.GetTicket(GetTicketID(), function(result) {
          _ticket = result;
          SetComboValue($find("<%=cmbSeverity.ClientID %>"), _ticket.TicketSeverityID);
          SetComboValue($find("<%=cmbUser.ClientID %>"), _ticket.UserID == null ? -1 : _ticket.UserID);
          SetComboValue($find("<%=cmbGroup.ClientID %>"), _ticket.GroupID);
          //SetComboValue($find("<%=cmbTicketType.ClientID %>"), _ticket.TicketTypeID);
          document.getElementById("<%=cbPortal.ClientID %>").checked = _ticket.IsVisibleOnPortal;
          document.getElementById("<%=cbKnowledgeBase.ClientID %>").checked = _ticket.IsKnowledgeBase;
          LoadStatusesCombo(_ticket.TicketTypeID, _ticket.TicketStatusID);
          LoadProductsCombo(_ticket.ProductID, _ticket.ReportedVersionID, _ticket.SolvedVersionID);
          _loadingDetails = false;
          SetUserEmailLink();
        });

      }

      function ShowSLATip(isViolation) {
        var tip = isViolation ? $find("<%= tipSlaViolation.ClientID %>") : $find("<%= tipSlaWarning.ClientID %>");
        if (!tip.isVisible()) tip.show();
      }

      function LoadSlaTips() {
        PageMethods.GetSLATips(GetTicketID(), function(result) {
          var t1 = $find("<%= tipSlaViolation.ClientID %>");
          var t2 = $find("<%= tipSlaWarning.ClientID %>");
          t1.set_text(result[0]);
          t2.set_text(result[1]);
        });

      }

      function ReloadTicket() {
        return;
        HideSaveDiv();

        _loadingDetails = true;

        PageMethods.GetTicket(GetTicketID(), function(result) {
          try {
            _ticket = result;

            LoadStatusesCombo(_ticket.TicketTypeID, _ticket.TicketStatusID);
            LoadProductsCombo(_ticket.ProductID, _ticket.ReportedVersionID, _ticket.SolvedVersionID);

            var items = $find("<%=cmbTicketType.ClientID %>").get_items();
            if (items.get_count() > 1) {
              items.getItem(0).select();
              items.getItem(1).select();
            }
            SetComboValue($find("<%=cmbTicketType.ClientID %>"), _ticket.TicketTypeID);
            SetComboValue($find("<%=cmbSeverity.ClientID %>"), _ticket.TicketSeverityID);
            SetComboValue($find("<%=cmbUser.ClientID %>"), _ticket.UserID);
            SetComboValue($find("<%=cmbGroup.ClientID %>"), _ticket.GroupID);
            document.getElementById("<%=cbPortal.ClientID %>").checked = _ticket.IsVisibleOnPortal;
            document.getElementById("<%=cbKnowledgeBase.ClientID %>").checked = _ticket.IsKnowledgeBase;
            LoadPropertiesDiv();
          }
          finally {
            _loadingDetails = false;
          }

        });
      }


      function GetTicketTypeID() { return $find("<%=cmbTicketType.ClientID %>").get_value(); }
      function GetStatusID() { return $find("<%=cmbStatus.ClientID %>").get_value(); }
      function GetSeverityID() { return $find("<%=cmbSeverity.ClientID %>").get_value(); }
      function GetUserID() { return $find("<%=cmbUser.ClientID %>").get_value(); }
      function GetGroupID() { return $find("<%=cmbGroup.ClientID %>").get_value(); }
      function GetProductID() {
        var combo = $find("<%=cmbProduct.ClientID %>");
        if (combo) return combo.get_value(); else return -1;
      }
      function GetResolvedID() {
        var combo = $find("<%=cmbResolved.ClientID %>");
        if (combo) return combo.get_value(); else return -1;
      }
      function GetReportedID() {
        var combo = $find("<%=cmbReported.ClientID %>");
        if (combo) return combo.get_value(); else return -1;
      }

      function PropertyChanged(sender, args) {
        if (_loadingDetails) return;
        $('.divSaveCancel').show();
        if (top.ChangeTabTextColor) top.ChangeTabTextColor(top.TABTYPE_TICKET, GetTicketID(), '#ff0000');
        _doConfirmClose = true;
        if (top.SetTicketCloseConfirmation) top.SetTicketCloseConfirmation(true, GetTicketID());
      }

      function LoadPropertiesDiv() {
        PageMethods.GetPropertyHtml(GetTicketID(), function(result) {
          $("#" + "<%= divProperties.ClientID %>").html(result);
        });
      }


      function HideSaveDiv() {
        $('.divSaveCancel').hide();
        if (top.ChangeTabTextColor) top.ChangeTabTextColor(top.TABTYPE_TICKET, GetTicketID(), '#0f3789');
        _doConfirmClose = false;
        if (top.SetTicketCloseConfirmation) top.SetTicketCloseConfirmation(false, GetTicketID());
      }


      function cmbTicketType_OnClientSelectedIndexChanged(sender, args) {
        if (_loadingDetails) return;
        PropertyChanged();
        LoadStatusesCombo(GetTicketTypeID(), null);
      }

      function cmbStatus_OnClientSelectedIndexChanged(sender, args) {
        if (_loadingStatuses || _loadingDetails) return;
        PropertyChanged();
        LoadStatusesCombo(GetTicketTypeID(), GetStatusID());
      }
      function cmbProduct_OnClientSelectedIndexChanged(sender, args) {
        if (_loadingDetails || _loadingProducts) return;
        PropertyChanged();
        LoadVersionCombos(GetProductID(), -1, -1);
      }
      function cmbReported_OnClientSelectedIndexChanged(sender, args) {
        if (_loadingDetails || _loadingVersions) return;
        SetVersionLinks();
        PropertyChanged();

      }
      function cmbResolved_OnClientSelectedIndexChanged(sender, args) {
        if (_loadingDetails || _loadingVersions) return;
        SetVersionLinks();
        PropertyChanged();

      }
      function cmbUser_OnClientSelectedIndexChanged(sender, args) {
        if (_loadingDetails) return;
        SetUserEmailLink();
        PropertyChanged();

      }


      function ButtonClicked(sender, args) {
        var button = args.get_item();
        var value = button.get_value();
        if (value == 'AddAction') { ShowAction(-1); }
        else if (value == 'Subscribe') { Subscribe(); }
        else if (value == 'TakeOwnership') { TakeOwnership(); }
        else if (value == 'RequestUpdate') { RequestUpdate(); }
        else if (value == 'History') { ShowHistory(); }
        else if (value == 'AddOrganization') { ShowAssociateOrganization(); }
        else if (value == 'Print') { window.open('../TicketPrint.aspx?ticketid=' + GetTicketID()); }
        else if (value == 'Refresh') { location.reload(); }
        else if (value == 'Delete') { DeleteTicket(); }
      }

      function GetRadWindow() {
        var oWindow = null;
        if (window.radWindow) oWindow = window.radWindow;
        else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
        return oWindow;
      }

      function ShowAction(actionID) {
        if (top.ChangeTabTextColor) top.ChangeTabTextColor(top.TABTYPE_TICKET, GetTicketID(), '#ff0000');
        _actionID = actionID;
        var wnd = GetRadWindowManager().getWindowByName("wndAction");
        var fn = wnd.get_contentFrame().contentWindow.LoadAction;
        if (fn) {
          fn(_actionID, GetTicketID());
        }
        else {
          setTimeout('ShowAction(' + actionID + ');', 100);
          return;
        }
        wnd.center();
        wnd.show();
      }

      function wndAction_OnClientPageLoad(sender, args) {
        sender.close();
        sender.add_close(function() { LoadActions(); });
      }

      function LoadActions() {
        if (top.ChangeTabTextColor) top.ChangeTabTextColor(top.TABTYPE_TICKET, GetTicketID(), '#0f3789');
        PageMethods.GetActionsHtml(GetTicketID(), LoadActionsResult);
        LoadSlaTips();
      }
      function LoadActionsResult(result) { $('.actions').html(result); }
      function ToggleKnowledge(actionID) { PageMethods.ToggleKnowledge(actionID, LoadActionsResult); }
      function TogglePortal(actionID) { PageMethods.TogglePortal(actionID, LoadActionsResult); }


      function DeleteAction(actionID) {
        if (confirm("Are you sure you would like to delete this action?")) {
          PageMethods.DeleteAction(actionID, LoadActionsResult);
        }
      }

      function DeleteAttachment(attachmentID) {
        if (confirm("Are you sure you would like to delete this attachment?")) {
          PageMethods.DeleteAttachment(attachmentID, LoadActionsResult);
        }
      }

      function DeleteTicket(verify) {
        if (verify == null || verify == true) if (!confirm('Are you sure you would like to delete this ticket?')) return;
        PageMethods.DeleteTicket(GetTicketID());
        top.DeleteTicketTab(GetTicketID());
      }

      function ShowHistory() {
        var wnd = top.GetHistoryDialog(17, GetTicketID())
        wnd.show();
      }



      function ShowAssociateOrganization() {
        var wnd = top.GetSelectOrganizationDialog(17, GetTicketID())
        wnd.add_close(DialogClosed);
        wnd.show();
      }

      function Subscribe() {
        var toolBar = $find("<%=tbMain.ClientID %>");
        var item = toolBar.findItemByValue("Subscribe");

        PageMethods.ToggleSubscription(GetTicketID(),
          function(result) {
            if (result) {
              item.set_text('Unsubscribe');
              alert('You have subscribed to this ticket.');
            }
            else {
              item.set_text('Subscribe');
              alert('You have unsubscribed to this ticket.');
            }
          });
      }


      function EditTicketName() {
        $('#divNameBox').show();
        $('#divNameLabel').hide();
        var textBox = $find("<%=textName.ClientID %>");
        var name = $("#" + "<%=lblTicketName.ClientID %>").html();
        textBox.set_value(name);
        textBox.focus();
      }

      function textName_OnKeyPress(sender, args) {
        if (args.get_keyCode() == 13) {
          SaveTicketName();
        }
        return false;
      }


      function SaveTicketName() {
        var name = $find("<%=textName.ClientID %>").get_value();
        $("#" + "<%=lblTicketName.ClientID %>").html(name);
        PageMethods.UpdateTicketName(GetTicketID(), name);
        $('#divNameBox').hide();
        $('#divNameLabel').show();
      }


      function TakeOwnership() {
        PageMethods.TakeOwnership(GetTicketID(),
          function(userID) {
            var combo = $find("<%=cmbUser.ClientID %>");
            var item = combo.findItemByValue(userID);
            if (item) {
              item.select();
            }
          }
          );
      }

      function RequestUpdate() {
        top.privateServices.RequestTicketUpdate(GetTicketID());
        alert('An update has been requested for the selected ticket.');

      }

      function GetTicketID() {
        var id = $get("<%=fieldTicketID.ClientID %>").value;
        if (id < 0) window.location = window.location;
        else return id;
      }


      function DialogClosed(sender, args) {
        sender.remove_close(DialogClosed);
        LoadCustomers();
      }

      function DeleteCustomer(id) {
        if (!confirm('Are you sure you would like to remove this customer association?')) return;
        top.privateServices.DeleteTicketOrganization(id, GetTicketID(), CustomerOrContactModified);
      }

      function DeleteContact(id) {
        if (!confirm('Are you sure you would like to remove this contact association?')) return;
        top.privateServices.DeleteTicketContact(id, GetTicketID(), CustomerOrContactModified);
      }

      function CustomerOrContactModified(result) {
        LoadProductsCombo(GetProductID(), GetReportedID(), GetResolvedID());
        LoadCustomers();

      }

      function OnClientItemsRequesting(sender, eventArgs) {
        var context = eventArgs.get_context();
        context["FilterString"] = eventArgs.get_text();
      }

      function OnClientDropDownClosed(sender, args) {
        sender.clearItems();
      }

      function cmbCompany_OnClientFocus(sender, args) {
        sender.set_value('');
        sender.set_text('');
        sender.clearItems();
        sender.clearSelection();
      }

      function cmbCompany_OnCustomerKeyPressing(sender, args) {
        if (args.get_domEvent().keyCode == 13) {
          AddCustomer();
        }
        else if (args.get_domEvent().keyCode == 27) {
          combo.set_value('');
          combo.set_text('');
          combo.clearSelection();
          combo.clearItems();
          combo._applyEmptyMessage();
        }
      }

      function cmbCompany_OnClick() {
        var combo = $find("<%=cmbCompany.ClientID %>");
        combo.set_text('');
        combo.set_value('');
        combo.clearItems();
        combo.clearSelection();
      }

      function AddCustomer() {
        var combo = $find("<%=cmbCompany.ClientID %>");
        var text = combo.get_text();
        var value = combo.get_value();

        if (!text || text == combo.get_emptyMessage()) return;

        if (!value || value < 1) return;
        top.privateServices.AddTicketOrganization(value, GetTicketID(), CustomerOrContactModified);
        combo.get_inputDomElement().focus();
      }

      function LoadCustomers() {
        var combo = $find("<%=cmbCompany.ClientID %>");
        if (!combo) return;
        PageMethods.GetCustomerText(GetTicketID(), SetCustomerText);
        combo.set_text('');
        combo.set_value('');
        combo.clearSelection();
        combo.clearItems();

        var cmbProducts = $find("<%=cmbProduct.ClientID %>");
        if (cmbProducts) cmbProducts.clearItems();
      }

      function SetCustomerText(result) {
        $(".customers").html(result);
      }




    </script>

  </telerik:RadScriptBlock>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline">
    <AjaxSettings>
      <telerik:AjaxSetting AjaxControlID="cmbTicketType">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="cmbTicketType" />
          <telerik:AjaxUpdatedControl ControlID="tblCustomControls" />
        </UpdatedControls>
      </telerik:AjaxSetting>
      <telerik:AjaxSetting AjaxControlID="btnCancel">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="cmbTicketType" />
          <telerik:AjaxUpdatedControl ControlID="tblCustomControls" />
        </UpdatedControls>
      </telerik:AjaxSetting>
      <telerik:AjaxSetting AjaxControlID="btnCancel2">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="cmbTicketType" />
          <telerik:AjaxUpdatedControl ControlID="tblCustomControls" />
        </UpdatedControls>
      </telerik:AjaxSetting>
    </AjaxSettings>
  </telerik:RadAjaxManager>
  <telerik:RadSplitter ID="splMain" runat="server" Orientation="Horizontal" Height="100%"
    Width="100%" VisibleDuringInit="False">
    <telerik:RadPane ID="paneToolBar" runat="server" Scrolling="None" Height="31px">
      <telerik:RadToolBar ID="tbMain" runat="server" CssClass="NoRoundedCornerEnds" OnClientButtonClicked="ButtonClicked"
        Width="100%">
        <Items>
          <telerik:RadToolBarButton runat="server" Value="AddAction" ImageUrl="~/images/icons/Action.png"
            Text="Log Action" ToolTip="Add an action to the selected ticket.">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="TakeOwnership" ImageUrl="~/images/icons/TakeOwnership.png"
            Text="Take Ownership" ToolTip="Take ownership of the selected ticket.">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="RequestUpdate" ImageUrl="~/images/icons/RequestUpdate.png"
            Text="Request Update" ToolTip="Request an update from the owner of the selected ticket.">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="SendUpdate" ImageUrl="~/images/icons/SendUpdate.png"
            Text="Send Update" ToolTip="Send an update to another user." Visible="false">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="Subscribe" ImageUrl="~/images/icons/Subscription.png"
            Text="Subscribe" ToolTip="Subscribe to get updated when the ticket changes.">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="AddOrganization" ImageUrl="~/images/icons/CustomerAdd.png"
            Text="Associate Customer" ToolTip="Associate a customer with this ticket.">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Button 1">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="History" ImageUrl="~/images/icons/History.png"
            Text="History" ToolTip="View the history of the ticket.">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="Refresh" ImageUrl="~/images/icons/Refresh.png"
            Text="Refresh" ToolTip="Refresh the ticket." Visible="true">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="Print" ImageUrl="~/images/icons/Print.png"
            Text="Print" ToolTip="Print the ticket.">
          </telerik:RadToolBarButton>
          <telerik:RadToolBarButton runat="server" Value="Delete" ImageUrl="~/images/icons/trash.png"
            Text="Delete" ToolTip="Delete the ticket." Visible="false">
          </telerik:RadToolBarButton>
        </Items>
      </telerik:RadToolBar>
    </telerik:RadPane>
    <telerik:RadPane ID="RadPane1" runat="server">
      <div style="margin: 0 auto; width: 800px; text-align: center; width: 100%;">
        <div style="text-align: left; margin: 0 auto; border-bottom: solid 0px #ABC1DE; padding-bottom: 5px;
          width: 800px;">
          <div style="font-size: 18px; font-weight: bold; margin-top: 10px;">
            Ticket Number:
            <asp:Label ID="lblTicketNumber" runat="server" Text="382389" Font-Size="18px"></asp:Label>
          </div>
          <div id="divNameLabel" style="padding-top: 3px; font-size: 16px;" onclick="EditTicketName();">
            <asp:Label ID="lblTicketName" runat="server" Text="TICKET NAME" Font-Size="16px"></asp:Label>
            <img id="imgEditName" alt="Edit" src="../images/icons/edit.png" class="iconImg" />
          </div>
          <div id="divNameBox" style="display: none;">
            <telerik:RadTextBox ID="textName" runat="server" Width="75%" Text="kevin" onblur="SaveTicketName();"
              AutoPostBack="false">
              <ClientEvents OnKeyPress="textName_OnKeyPress" />
            </telerik:RadTextBox>
            <img id="imgSaveTicketName" alt="Edit" src="../images/icons/Ok.png" class="iconImg"
              onclick="SaveTicketName();" />
          </div>
          <div class="groupDiv groupLightBlue" style="margin: 10px 0 0 0px; width: 800px;">
            <div class="groupHeaderDiv">
              <span class="groupHeaderSpan"></span><span class="groupCaptionSpan">Ticket Properties</span>
            </div>
            <div class="groupBodyWrapperDiv">
              <div class="groupBodyDiv">
                <div style="padding: 5px 0 10px 10px;">
                  <div class="divSaveCancel ui-corner-all" style="text-align: center; margin: 10px auto 10px auto;
                    border: solid 1px #EEB420; padding: 5px 0px; background-color: #FFFBF1; width: 755px;
                    display: none;">
                    <asp:Button ID="btnSave2" runat="server" Text="Save" OnClientClick="SaveTicket();  return false;" />
                    &nbsp
                    <asp:Button ID="btnSaveClose2" runat="server" Text="Save & Close Tab" OnClientClick="SaveTicket(true);  return false;" />
                    &nbsp
                    <asp:Button ID="btnCancel2" runat="server" Text="Cancel" OnClick="btnCancel_OnClick"
                      OnClientClick="CancelPropertyEdit();" />
                  </div>
                  <telerik:RadToolTip ID="tipSlaViolation" runat="server" RelativeTo="Element" TargetControlID="imgViolationInfo"
                    Text="SLA" Title="Service Level Agreement Violation Times" IsClientID="True" Animation="Resize"
                    ShowDelay="0" ManualClose="False" ShowEvent="FromCode" AutoCloseDelay="5000">
                  </telerik:RadToolTip>
                  <telerik:RadToolTip ID="tipSlaWarning" runat="server" RelativeTo="Element" TargetControlID="imgWarningInfo"
                    Text="SLA" Title="Service Level Agreement Warning Times" IsClientID="True" Animation="Resize"
                    ShowDelay="0" ManualClose="False" ShowEvent="FromCode" AutoCloseDelay="5000">
                  </telerik:RadToolTip>
                  <div id="divProperties" runat="server">
                  </div>
                  <table width="775px" cellpadding="" cellspacing="5" border="0">
                    <tr>
                      <td class="labelColTD">
                        Ticket Type:
                      </td>
                      <td class="inputColTD">
                        <telerik:RadComboBox ID="cmbTicketType" runat="server" Width="200px" AutoPostBack="true"
                          OnSelectedIndexChanged="cmbTicketType_SelectedIndexChanged" OnClientSelectedIndexChanged="cmbTicketType_OnClientSelectedIndexChanged">
                          <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                        </telerik:RadComboBox>
                      </td>
                      <td class="labelColTD">
                        Assigned Group:
                      </td>
                      <td class="inputColTD">
                        <telerik:RadComboBox ID="cmbGroup" runat="server" Width="200px" OnClientSelectedIndexChanged="PropertyChanged">
                          <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                        </telerik:RadComboBox>
                      </td>
                    </tr>
                    <tr>
                      <td class="labelColTD">
                        Status:
                      </td>
                      <td class="inputColTD">
                        <telerik:RadComboBox ID="cmbStatus" runat="server" Width="200px" OnClientSelectedIndexChanged="cmbStatus_OnClientSelectedIndexChanged">
                          <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                        </telerik:RadComboBox>
                      </td>
                      <td class="labelColTD">
                        <span runat="server" id="spanProduct">Product:</span>
                      </td>
                      <td class="inputColTD">
                        <telerik:RadComboBox ID="cmbProduct" runat="server" Width="200px" OnClientSelectedIndexChanged="cmbProduct_OnClientSelectedIndexChanged">
                          <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                        </telerik:RadComboBox>
                      </td>
                    </tr>
                    <tr>
                      <td class="labelColTD">
                        Severity:
                      </td>
                      <td class="inputColTD">
                        <telerik:RadComboBox ID="cmbSeverity" runat="server" Width="200px" OnClientSelectedIndexChanged="PropertyChanged">
                          <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                        </telerik:RadComboBox>
                      </td>
                      <td class="labelColTD">
                        <span runat="server" id="spanReported">Reported Version:</span>
                      </td>
                      <td class="inputColTD">
                        <telerik:RadComboBox ID="cmbReported" runat="server" Width="175px" OnClientSelectedIndexChanged="cmbReported_OnClientSelectedIndexChanged">
                          <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                        </telerik:RadComboBox>
                        <a id="lnkReported" href="#" style="background: transparent url(../images/icons/go.png) no-repeat;
                          padding: 1px 0 0 16px; margin-left: 3px; height: 20px; text-decoration: none; display: none;">
                          &nbsp</a>
                      </td>
                    </tr>
                    <tr>
                      <td class="labelColTD">
                        Assigned To:
                      </td>
                      <td class="inputColTD">
                        <telerik:RadComboBox ID="cmbUser" runat="server" Width="175px" OnClientSelectedIndexChanged="cmbUser_OnClientSelectedIndexChanged">
                          <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                        </telerik:RadComboBox>
                        <a id="lnkUserEmail" href="#" style="background: transparent url(../images/icons/email.png) no-repeat;
                          padding: 1px 0 0 16px; margin-left: 3px; height: 20px; text-decoration: none;">&nbsp</a>
                      </td>
                      <td class="labelColTD">
                        <span runat="server" id="spanResolved">Resolved Version:</span>
                      </td>
                      <td class="inputColTD">
                        <telerik:RadComboBox ID="cmbResolved" runat="server" Width="175px" OnClientSelectedIndexChanged="cmbResolved_OnClientSelectedIndexChanged">
                          <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                        </telerik:RadComboBox>
                        <a id="lnkResolved" href="#" style="background: transparent url(../images/icons/go.png) no-repeat;
                          padding: 1px 0 0 16px; margin-left: 3px; height: 20px; text-decoration: none; display: none;">
                          &nbsp</a>
                      </td>
                    </tr>
                    <tr>
                      <td class="labelColTD">
                        <span id="spanPortal" runat="server">Visible to Customers:</span>
                      </td>
                      <td class="inputColTD">
                        <asp:CheckBox ID="cbPortal" runat="server" CssClass="checkBoxAlign" Text="" onclick="PropertyChanged();" />
                      </td>
                      <td class="labelColTD">
                        Knowledge Base:
                      </td>
                      <td class="inputColTD">
                        <asp:CheckBox ID="cbKnowledgeBase" runat="server" CssClass="checkBoxAlign" Text=""
                          onclick="PropertyChanged();" />
                      </td>
                    </tr>
                  </table>
                  <table id="tblCustomControls" runat="server" width="775px" cellpadding="0" cellspacing="5"
                    border="0">
                  </table>
                  <div class="divSaveCancel ui-corner-all" style="text-align: center; margin: 10px auto 0 auto;
                    border: solid 1px #EEB420; padding: 5px 0px; background-color: #FFFBF1; width: 755px;
                    display: none;">
                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClientClick="SaveTicket();  return false;" />
                    &nbsp
                    <asp:Button ID="btnSaveClose" runat="server" Text="Save & Close Tab" OnClientClick="SaveTicket(true);  return false;" />
                    &nbsp
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_OnClick"
                      OnClientClick="CancelPropertyEdit();" />
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div id="divCustomers" runat="server">
            <div class="groupDiv groupLightBlue" style="margin: 10px 0 0 0px; width: 800px;">
              <div class="groupHeaderDiv">
                <span class="groupHeaderSpan"></span><span class="groupCaptionSpan">Associated Contacts
                  &amp; Customers</span>
              </div>
              <div class="groupBodyWrapperDiv">
                <div class="groupBodyDiv">
                  <div style="padding: 10px 0 10px 10px;">
                    <table border="0" cellpadding="0" cellspacing="5">
                      <tr>
                        <td valign="top" style="padding-top: 5px;">
                          <telerik:RadComboBox ID="cmbCompany" runat="server" Width="200px" AllowCustomText="True"
                            MarkFirstMatch="True" ShowToggleImage="false" EnableLoadOnDemand="True" LoadingMessage="Loading..."
                            OnClientItemsRequesting="OnClientItemsRequesting" OnClientDropDownClosed="OnClientDropDownClosed"
                            EmptyMessage="Enter a Contact or Customer" OnClientKeyPressing="cmbCompany_OnCustomerKeyPressing"
                            ShowDropDownOnTextboxClick="False" onclick="cmbCompany_OnClick();" DropDownWidth="400px"
                            OnClientFocus="cmbCompany_OnClientFocus">
                            <WebServiceSettings Path="~/Services/PrivateServices.asmx" Method="GetUserOrOrganization" />
                            <CollapseAnimation Duration="200" Type="OutQuint" />
                          </telerik:RadComboBox>
                        </td>
                        <td valign="top" style="padding-top: 5px;">
                          <img alt="Clear" src="../images/icons/add.png" style="padding: 3px 5px 0 2px; cursor: pointer;"
                            onclick="AddCustomer();" />
                        </td>
                        <td valign="middle" style="padding-left: 10px; line-height: 20px;">
                          <div id="divCustomerList" runat="server" class="customers">
                          </div>
                        </td>
                      </tr>
                    </table>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="groupDiv groupLightBlue" style="margin: 10px 0 0 0px; width: 800px;">
            <div class="groupHeaderDiv">
              <span class="groupHeaderSpan"></span><span class="groupCaptionSpan">Actions</span>
              <span class="groupButtonSpanWrapper"><span class="groupButtonsSpan">
                <asp:LinkButton ID="btnCrm" runat="server" CssClass="groupButtonLink" OnClientClick="ShowAction(-1); return false;">
                <span class="groupButtonSpan">
                <img alt="" src="../images/icons/action.png" class="groupButtonImage" />
                    <span class="groupButtonTextSpan">Log Action</span> </span></asp:LinkButton>
              </span></span>
            </div>
            <div class="groupBodyWrapperDiv">
              <div class="groupBodyDiv">
                <div id="divActions" runat="server" class="actions" style="padding: 10px 0 10px 10px;">
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </telerik:RadPane>
  </telerik:RadSplitter>
  <asp:HiddenField ID="fieldTicketID" runat="server" />
  <telerik:RadWindowManager ID="windowManager" runat="server" Behaviors="Close, Move">
    <Windows>
      <telerik:RadWindow ID="wndAction" runat="server" NavigateUrl="~/Dialogs/TicketAction.aspx"
        Width="800px" Height="515px" Animation="None" KeepInScreenBounds="False" VisibleStatusbar="False"
        VisibleTitlebar="True" OnClientPageLoad="wndAction_OnClientPageLoad" IconUrl="~/images/icons/action.png"
        VisibleOnPageLoad="True" ShowContentDuringLoad="False" Modal="False" Top="-1000px">
      </telerik:RadWindow>
    </Windows>
  </telerik:RadWindowManager>
</asp:Content>
