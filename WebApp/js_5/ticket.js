
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
      $("#btnSaveClose").hide();
      $("#btnSaveClose2").hide();
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
      LoadCombo($find("cmbStatus"), statuses, statusID);
    }
    finally {
      _loadingStatuses = false;
    }
  });
}

function LoadProductsCombo(productID, reportedID, resolvedID) {
  var combo = $find("cmbProduct");
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
      LoadCombo($find("cmbReported"), versions, reportedID);
      LoadCombo($find("cmbResolved"), versions, resolvedID);
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
  _ticket.IsVisibleOnPortal = document.getElementById("cbPortal").checked;
  _ticket.IsKnowledgeBase = document.getElementById("cbKnowledgeBase").checked;

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
    SetComboValue($find("cmbSeverity"), _ticket.TicketSeverityID);
    SetComboValue($find("cmbUser"), _ticket.UserID == null ? -1 : _ticket.UserID);
    SetComboValue($find("cmbGroup"), _ticket.GroupID);
    //SetComboValue($find("cmbTicketType"), _ticket.TicketTypeID);
    document.getElementById("cbPortal").checked = _ticket.IsVisibleOnPortal;
    document.getElementById("cbKnowledgeBase").checked = _ticket.IsKnowledgeBase;
    LoadStatusesCombo(_ticket.TicketTypeID, _ticket.TicketStatusID);
    LoadProductsCombo(_ticket.ProductID, _ticket.ReportedVersionID, _ticket.SolvedVersionID);
    _loadingDetails = false;
    SetUserEmailLink();
  });

}

function ShowSLATip(isViolation) {
  var tip = isViolation ? $find("tipSlaViolation") : $find("tipSlaWarning");
  if (!tip.isVisible()) tip.show();
}

function LoadSlaTips() {
  PageMethods.GetSLATips(GetTicketID(), function(result) {
    var t1 = $find("tipSlaViolation");
    var t2 = $find("tipSlaWarning");
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

      var items = $find("cmbTicketType").get_items();
      if (items.get_count() > 1) {
        items.getItem(0).select();
        items.getItem(1).select();
      }
      SetComboValue($find("cmbTicketType"), _ticket.TicketTypeID);
      SetComboValue($find("cmbSeverity"), _ticket.TicketSeverityID);
      SetComboValue($find("cmbUser"), _ticket.UserID);
      SetComboValue($find("cmbGroup"), _ticket.GroupID);
      document.getElementById("cbPortal").checked = _ticket.IsVisibleOnPortal;
      document.getElementById("cbKnowledgeBase").checked = _ticket.IsKnowledgeBase;
      LoadPropertiesDiv();
    }
    finally {
      _loadingDetails = false;
    }

  });
}


function GetTicketTypeID() { return $find("cmbTicketType").get_value(); }
function GetStatusID() { return $find("cmbStatus").get_value(); }
function GetSeverityID() { return $find("cmbSeverity").get_value(); }
function GetUserID() { return $find("cmbUser").get_value(); }
function GetGroupID() { return $find("cmbGroup").get_value(); }
function GetProductID() {
  var combo = $find("cmbProduct");
  if (combo) return combo.get_value(); else return -1;
}
function GetResolvedID() {
  var combo = $find("cmbResolved");
  if (combo) return combo.get_value(); else return -1;
}
function GetReportedID() {
  var combo = $find("cmbReported");
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
    $("#divProperties").html(result);
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
  var toolBar = $find("tbMain");
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
  var textBox = $find("textName");
  var name = $("#lblTicketName").html();
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
  var name = $find("textName").get_value();
  $("#lblTicketName").html(name);
  PageMethods.UpdateTicketName(GetTicketID(), name);
  $('#divNameBox').hide();
  $('#divNameLabel').show();
}


function TakeOwnership() {
  PageMethods.TakeOwnership(GetTicketID(),
          function(userID) {
            var combo = $find("cmbUser");
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
  var id = $get("fieldTicketID").value;
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
  var combo = $find("cmbCompany");
  combo.set_text('');
  combo.set_value('');
  combo.clearItems();
  combo.clearSelection();
}

function AddCustomer() {
  var combo = $find("cmbCompany");
  var text = combo.get_text();
  var value = combo.get_value();

  if (!text || text == combo.get_emptyMessage()) return;

  if (!value || value < 1) return;
  top.privateServices.AddTicketOrganization(value, GetTicketID(), CustomerOrContactModified);
  combo.get_inputDomElement().focus();
}

function LoadCustomers() {
  var combo = $find("cmbCompany");
  if (!combo) return;
  PageMethods.GetCustomerText(GetTicketID(), SetCustomerText);
  combo.set_text('');
  combo.set_value('');
  combo.clearSelection();
  combo.clearItems();

  var cmbProducts = $find("cmbProduct");
  if (cmbProducts) cmbProducts.clearItems();
}

function SetCustomerText(result) {
  $(".customers").html(result);
}



