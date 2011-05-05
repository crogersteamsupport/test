window.name = "TSMain";
var TABTYPE_MAIN = 'main';
var TABTYPE_TICKET = 'ticket';
var TABTYPE_NEWTICKET = 'newticket';
var _version = null;
var _statusChanging = false;
var _currentFrame = null;
var _newMessages = 0;
var _newRequests = 0;
var _statusTimer = null;
var _isIdle = false;
var _debugCount = 0;
var _userInfo = null;
var _menuTree = null;
var _isWindowFocused = true;
var _selectContactID = -1;
var _selectCustomerID = -1;
var x = false;

function TabData(tabType, id, url, caption, confirmMessage) {
  this.TabType = tabType;
  this.ID = id;
  this.Url = url;
  this.Caption = caption;
  this.ConfirmMessage = confirmMessage;
}

$(document).ready(function() {
  if (navigator.appName == 'Microsoft Internet Explorer') {
    document.onfocusin = function() { _isWindowFocused = true; }
    document.onfocusout = function() { _isWindowFocused = false; }
  }
  else {
    window.onfocus = function() { _isWindowFocused = true; }
    window.onblur = function() { _isWindowFocused = false; }
  }


});

function tsHead_OnClientLoad() {
  _menuTree = new MenuTree($('#divMenuTree')[0]);
  _menuTree.click(mainVMenu_OnClick);
  var mniWC = _menuTree.getItemByID('mniWC');
  if (mniWC != null) mniWC.select(function() { PageMethods.UpdateWaterCoolerID(); });

  var mainTab = GetMainHeadTab();
  var menuItem = _menuTree.getSelected();
  mainTab.set_value(new TabData(TABTYPE_MAIN, 0, '', ''));
  mainTab.set_text(menuItem.getText());
  mainTab.set_imageUrl(menuItem.getImageUrl());
  mainTab.set_selected(true);
  SetContentFrame(mainTab.get_value());

  SetNewMessageInterval(false);
  setInterval('FlashTitle()', 1250);
  setInterval('UpdateUserStatus()', 4000);

  PageMethods.GetOpenTabs(function(result) {
    if (result != null) {
      for (var i = 0; i < result.length; i++) { AddHeadTab(result[i], false, true); }
    }

    var ticketID = GetQueryParamValue('ticketid', window.location);
    if (ticketID) { AddTicketTab(ticketID, true); }

    var ticketNumber = GetQueryParamValue('ticketnumber', window.location);
    if (ticketNumber) { PageMethods.GetTicketID(ticketNumber, function(result) { AddTicketTab(result, true, ticketNumber); }); }
  });

  processQuery();

}

function processQuery()
{

  var customerID = GetQueryParamValue('customerid', window.location);
  if (customerID) {
    var contactID = GetQueryParamValue('contactid', window.location);
    if (contactID) OpenContact(contactID, customerID); else OpenCustomer(customerID);
    return;
  }
  var customerName = GetQueryParamValue('customername', window.location);
  if (customerName) {
    openCustomerByName(customerName);
    return;
  }
  
  var phoneNumber = GetQueryParamValue('phonenumber', window.location);
  if (phoneNumber) {
    PageMethods.GetIDByPhone(phoneNumber, function(result) {
      if (result[1] > -1) OpenContact(result[1], result[0]);
      else if (result[0] > -1) OpenCustomer(result[0]);
    });
    return;
  }
}

function FlashTitle() {
  if (document.title != 'Team Support') {
    document.title = 'Team Support';
  }
  else if (_newRequests > 1) {
    document.title = _newRequests + ' New Chat Requests';
  }
  else if (_newRequests == 1) {
    document.title = _newRequests + ' New Chat Request';
  }
  else if (_newMessages > 1) {
    document.title = _newMessages + ' New Messages';
  }
  else if (_newMessages == 1) {
    document.title = _newMessages + ' New Message';
  }

}

function GetUserInfo() {
  PageMethods.GetUserInfo(function(result) {
    _userInfo = result;

  });
}

function UpdateUserStatus() {
  if (_userInfo == null) { GetUserInfo(); return; }
  $.ajax({
    type: "POST",
    url: "Default.aspx/UpdateUserStatus",
    data: "{'userInfo':" + JSON.stringify(_userInfo) + "}",
    contentType: "application/json; charset=utf-8",
    dataType: "json"
  });

  //PageMethods.UpdateUserStatus(_userInfo);
}

function GetNewMessages() {
  if (_userInfo == null) {
    GetUserInfo();
    return;
  }
  var wcItem = _menuTree.getItemByID('mniWC');
  PageMethods.GetNewMessages(_userInfo, wcItem == null ? false : wcItem.isSelected(), function(status) {
    if (!status) window.location = window.location;

    if (_version) {
      if (_version != status.Version) {
        window.location = window.location;
      }


      if (status.NewWCMessage) {
        var item = _menuTree.getItemByID('mniWC');
        if (item != null && !item.isSelected()) { item.setStateHighlight(); }
      }

      if (status.Messages && status.Messages.length > 0) {
        for (var i = 0; i < status.Messages.length; i++) {
          $.jGrowl(status.Messages[i], { life: 5000 });
        }
      }

      if (status.Requests && status.Requests.length > 0) {
        window.focus();
        alert(status.Requests[0]);
        
        PageMethods.GetNewMessages(_userInfo, false, function(status2) { if (status2.ChatRequestCount < 1) { alert('All chat requests are already answered.'); } });
        window.focus();

        for (var i = 0; i < status.Requests.length; i++) {
          $.jGrowl(status.Requests[i], { life: 5000 });
        }
      }

      _newRequests = status.ChatRequestCount;
      _newMessages = status.NewMessageCount;


      if (_newRequests + _newMessages > 0) {
        var item = _menuTree.getItemByID('mniChat');
        if (item != null && !item.isSelected()) item.setStateHighlight();
      }
      else {
        var item = _menuTree.getItemByID('mniChat');
        if (item != null && !item.isSelected() && item.isHighlighted()) item.setStateDefault();
      }

      if (status.IsIdle != _isIdle) {
        SetNewMessageInterval(status.IsIdle);
        return;
      }

    }
    else {
      _version = status.Version;
    }
  });
}


function SetNewMessageInterval(isIdle) {
  clearInterval(_statusTimer);
  _isIdle = isIdle;
  if (isIdle) {
    _statusTimer = setInterval('GetNewMessages()', 30000);
    $('#spanActivity').text('Idle');
  }
  else {
    _statusTimer = setInterval('GetNewMessages()', 4000);
    $('#spanActivity').text('');
  }
}



function paneInfo_OnClientResized(sender, args) {
  if (privateServices) privateServices.SetUserSetting('PaneInfoWidth', sender.get_width());
}
function paneInfo_OnClientCollapsed(sender, args) {
  if (privateServices) privateServices.SetUserSetting('PaneInfoCollapsed', 'True');
}
function paneInfo_OnClientExpanded(sender, args) {
  if (privateServices) privateServices.SetUserSetting('PaneInfoCollapsed', 'False');
}

function cmbTicket_OnClick() {
  var combo = $find(get_cmbTicketClientID());
  combo.set_text('');
  combo.set_value('');
  combo.clearItems();
  combo.clearSelection();
}

function cmbTicket_OnClientSelectedIndexChanged(sender, args) {
  var value = sender.get_value();
  sender.set_text('');
  sender.set_value('');
  sender.clearSelection();
  sender.clearItems();
  sender._applyEmptyMessage();
  OpenTicketWindow(value)

}
function cmbTicket_OnClientItemsRequesting(sender, args) {
  sender.set_closeDropDownOnBlur(true);
  var context = args.get_context();
  context["FilterString"] = args.get_text();
}

function AddHeadTab(tabData, isSelected, bypassSave) {
  var tabs = $find(get_tsHeadClientID()).get_tabs();

  if (isSelected) {
    for (var i = 0; i < tabs.get_count(); i++) {
      var t = tabs.getTab(i).get_value();
      if (t.TabType == tabData.TabType && t.ID == tabData.ID) {
        tabs.getTab(i).set_selected(true);
        return;
      }
    }
  }

  var tab = new Telerik.Web.UI.RadTab();
  tab.set_text(tabData.Caption);
  tab.set_value(tabData);
  tabs.add(tab);
  switch (tabData.TabType) {
    case TABTYPE_NEWTICKET:
      tab.set_imageUrl('images/headtabs/ticket_new.png');
      tab.get_textElement().style.color = 'red';
      break;
    case TABTYPE_TICKET: tab.set_imageUrl('images/headtabs/ticket.png'); break;
  }

  AttachCloseImage(tab);

  if (isSelected) tab.set_selected(true);
  if (!bypassSave) PageMethods.AddOpenTab(tabData);
}


function AddTicketTab(ticketID, selected, ticketNumber) {
  var tabData = new TabData(TABTYPE_TICKET, ticketID, 'frames/ticket.aspx?Embedded=1&TicketID=' + ticketID, '');
  if (ticketNumber) {
    tabData.Caption = 'Ticket: ' + ticketNumber;
    AddHeadTab(tabData, selected);
  }
  else {
    PageMethods.GetTicketNumber(ticketID,
          function(result) {
            if (result) {
              tabData.Caption = 'Ticket: ' + result;
              AddHeadTab(tabData, selected);
            }
          });
  }
}

function AddNewTicketTab(chatID) {
  var url = !chatID || chatID < 0 ? 'frames/newticket.aspx' : 'frames/newticket.aspx?ChatID=' + chatID;
  var tabData = new TabData(TABTYPE_NEWTICKET, 0, url, 'New Ticket', 'Are you sure you would like to cancel this new ticket?');
  AddHeadTab(tabData, true);
}

function AttachCloseImage(tab) {
  if (tab.get_value() == '0') return;
  var closeImage = document.createElement("img");
  closeImage.src = 'images/head_tab_x.png';
  closeImage.alt = "";
  closeImage.setAttribute('className', 'headTabCloseImg');
  closeImage.AssociatedTab = tab;
  closeImage.onclick = function(e) {
    if (!e) e = event;
    if (!e.target) e = e.srcElement;

    DeleteHeadTab(tab);

    e.cancelBubble = true;
    if (e.stopPropagation) {
      e.stopPropagation();
    }

    return false;
  }
  tab.get_innerWrapElement().appendChild(closeImage);
}

function DeleteTicketTab(ticketID) {
  var tab = GetHeadTab(TABTYPE_TICKET, ticketID);
  if (tab) return DeleteHeadTab(tab);
  return false;
}

function SetTicketCloseConfirmation(confirm, ticketID) {
  var tab = GetHeadTab(TABTYPE_TICKET, ticketID);
  if (tab) {
    var tabData = tab.get_value();
    tabData.ConfirmMessage = confirm ? 'Changes to your ticket have not been saved.  Would you like to close this tab?' : '';
    tab.set_value(tabData);
  }
}

function DeleteNewTicketTab(overrideConfirm) {
  return DeleteTab(TABTYPE_NEWTICKET, 0, overrideConfirm);
}

function DeleteTab(tabType, id, overrideConfirm) {
  var tab = GetHeadTab(tabType, id);
  if (tab) {
    if (overrideConfirm) {
      var tabData = tab.get_value();
      tabData.ConfirmMessage = '';
      tab.set_value(tabData);
    }
    return DeleteHeadTab(tab);
  }
  return false;
}

function GetHeadTab(type, id) {
  var tabs = $find(get_tsHeadClientID()).get_tabs();

  for (var i = 0; i < tabs.get_count(); i++) {
    var tabData = tabs.getTab(i).get_value();
    if (tabData.ID == id && tabData.TabType == type) {
      return tabs.getTab(i);
    }
  }
  return null;
}

function ChangeTabTextColor(tabType, id, color) {
  var tab = GetHeadTab(tabType, id);
  if (!tab) return;
  tab.get_textElement().style.color = color;

}

function GetFrameID(tabData) {
  return 'contentFrame_' + tabData.TabType + tabData.ID;
}

function GetMainFrameID(menuItemID) {
  return 'contentFrame_' + TABTYPE_MAIN + menuItemID;
}

function DeleteHeadTab(tab) {

  var tabData = tab.get_value();

  if (tabData.ConfirmMessage && tabData.ConfirmMessage != '') {
    if (!confirm(tabData.ConfirmMessage)) return false;
  }


  DeleteContentFrame(GetFrameID(tabData));
  PageMethods.RemoveOpenTab(tabData);

  var tabToSelect = tab.get_nextTab();
  if (!tabToSelect) tabToSelect = tab.get_previousTab();
  $find(get_tsHeadClientID()).get_tabs().remove(tab);
  if (tabToSelect) tabToSelect.set_selected(true);
  return true;
}

function DeleteContentFrame(frameID) {
  try {
    var frameDiv = $get('frameDiv');
    var frame = $get(frameID);
    if (frame) frameDiv.removeChild(frame);
  }
  catch (err) {

  }

}

function tsHead_OnClientTabSelected(sender, args) {
  SetContentFrame(args.get_tab().get_value());
}



function mainVMenu_OnClick(menuItem) {
  var mainHeadTab = GetMainHeadTab();
  mainHeadTab.set_text(menuItem.getText());
  mainHeadTab.set_imageUrl(menuItem.getImageUrl());
  mainHeadTab.set_selected(true);
  SetContentFrame(mainHeadTab.get_value());
  if (privateServices) privateServices.SetUserSetting('LeftTabID', menuItem.element.id);
}

function GetMainHeadTab() {
  return $find(get_tsHeadClientID()).get_tabs().getTab(0);
}

function SetContentFrame(tabData) {
  var frameDiv = $get('frameDiv');
  $('.contentFrame').hide();
  try { if (_currentFrame != null && _currentFrame.contentWindow.onHide) _currentFrame.contentWindow.onHide(); } catch (err) { }
  if (_currentFrame != null) {
    var src = _currentFrame.getAttribute('src').toLowerCase();
    if (src.indexOf('watercooler') > -1) {
      frameDiv.removeChild(_currentFrame);

    }
  }

  var framePaneInfo = $get(get_framePaneInfoClientID());
  var infoPage = '';
  if (tabData.TabType == TABTYPE_MAIN) {
    var leftSelected = _menuTree.getSelected();
    if (leftSelected == null) return;
    var leftData = leftSelected.getData();
    tabData.ID = leftSelected.element.id;
    tabData.Url = leftData.ContentUrl;
    infoPage = leftData.PaneInfoUrl;
  }
  else if (tabData.TabType == TABTYPE_NEWTICKET) {
    infoPage = 'PaneInfo/NewTicket.aspx';
  }
  else if (tabData.TabType == TABTYPE_TICKET) {
    infoPage = 'PaneInfo/Ticket.aspx';
  }

  framePaneInfo.setAttribute('src', infoPage);

  var frameName = GetFrameID(tabData);
  var contentFrame = $get(frameName);
  $('.debugspan').text(tabData.Url);
  if (contentFrame == null) {
    contentFrame = document.createElement('iframe');
    contentFrame.setAttribute('id', frameName);
    contentFrame.setAttribute('name', frameName);
    contentFrame.setAttribute('className', 'contentFrame');
    contentFrame.setAttribute('class', 'contentFrame');
    contentFrame.setAttribute('scrolling', 'no');
    contentFrame.setAttribute('src', tabData.Url);
    contentFrame.setAttribute('frameBorder', '0');
    contentFrame.setAttribute('width', '100%');
    contentFrame.setAttribute('height', '100%');
    frameDiv.appendChild(contentFrame);
  }
  $(contentFrame).show();
  _currentFrame = contentFrame;

  if (contentFrame.contentWindow.refreshData) contentFrame.contentWindow.refreshData();
  try { if (contentFrame.contentWindow.onShow) contentFrame.contentWindow.onShow(); } catch (err) { }
  try { privateServices.UpdateUserActivityTime(); } catch (err) { }
  SetNewMessageInterval(false);
}

function lnkQuickNew_OnClick() {
  AddNewTicketTab();
  return false;
}


function textMyStatus_OnKeyPress(sender, args) {
  if (args.get_keyCode() == 13) {
    imgSaveStatus_OnClick();
  }
  return false;
}

function textMyStatus_OnFocus() {
  $('#divSaveStatusButtons').show();
}

function imgSaveStatus_OnClick() {
  var text = $find(get_textMyStatusClientID());
  privateServices.SetUserStatusText(text.get_value());
  $('#divSaveStatusButtons').hide();
  text.blur();
}

function imgCancelStatus_OnClick() {
  $('#divSaveStatusButtons').hide();
  privateServices.GetUserStatusText(
        function(result) {
          $find(get_textMyStatusClientID()).set_value(result);
        });
}

function ToggleChat() {
  if (_statusChanging) return;
  _statusChanging = true;
  privateServices.ToggleUserChat(
          function(result) {
            _statusChanging = false;
            img = document.getElementById('imgChat');
            if (result) { img.setAttribute('src', 'images/icons/chat.png'); }
            else { img.setAttribute('src', 'images/icons/chat_d.png'); }
          }
        );
}

function ToggleAvailability() {
  if (_statusChanging) return;
  _statusChanging = true;
  privateServices.ToggleUserAvailability(
          function(result) {
            _statusChanging = false;
            img = document.getElementById('imgAvailability');
            if (result) { img.setAttribute('src', 'images/icons/Online.png'); }
            else { img.setAttribute('src', 'images/icons/Unavailable.png'); }
          }
        );
}

function OpenTicketWindow(ticketID) {
  AddTicketTab(ticketID, true);
}


function OpenProduct(productID, versionID) {
  if (!versionID) versionID = -1;
  PageMethods.SetProductPage(productID, versionID,
          function(result) {

            var mainTab = GetMainHeadTab();
            mainTab.set_selected(true);
            var tabData = mainTab.get_value();
            _menuTree.setSelected('mniProducts');
            var contentFrame = $get(GetFrameID(tabData));
            if (contentFrame.contentWindow.refreshData) {
              contentFrame.contentWindow.refreshData(true)
            }

          }
        );
}

function OpenUser(userID) {
  PageMethods.SetUserPage(userID,
          function(result) {
            if (result != null) {
              OpenContact(result.UserID, result.OrganizationID);
            }
            else {
              var mainTab = GetMainHeadTab();
              mainTab.set_selected(true);
              var tabData = mainTab.get_value();
              _menuTree.setSelected('mniUsers');
              var contentFrame = $get(GetFrameID(tabData));
              if (contentFrame.contentWindow.refreshData) {
                contentFrame.contentWindow.refreshData(true)
              }
            }

          }
        );
}

function OpenContact(contactID, customerID) {
  if (!customerID) {
    PageMethods.GetContactOrganizationID(contactID, function(result) { OpenContact(contactID, result); });
    return;
  }
  _selectContactID = contactID;
  _selectCustomerID = customerID;
  var mainTab = GetMainHeadTab();
  var tabData = mainTab.get_value();
  _menuTree.setSelected("mniCustomers");
  mainTab.set_selected(true);
  var contentFrame = $get(GetFrameID(tabData));

  if (contentFrame && contentFrame.contentWindow.selectContact) {
    contentFrame.contentWindow.selectContact(contactID, customerID);
  }
  
}

function OpenCustomer(id) {
  _selectContactID = -1;
  _selectCustomerID = id;

  var mainTab = GetMainHeadTab();
  mainTab.set_selected(true);
  var tabData = mainTab.get_value();
  _menuTree.setSelected("mniCustomers");
  var contentFrame = $get(GetFrameID(tabData));
  if (contentFrame && contentFrame.contentWindow.selectCustomer) {
    contentFrame.contentWindow.selectCustomer(id);
  }
}

function openTag(id) {
  var item = _menuTree.getItemByID('mniTicketTags');
  var data = item.getData();
  var url = data.ContentUrl;
  DeleteContentFrame(GetMainFrameID('mniTicketTags'));
  data.ContentUrl = 'Frames/TicketTags.aspx?TagID=' + id;
  item.setData(data);
  _menuTree.setSelected('mniTicketTags');
  data.ContentUrl = url;
  item.setData(data);
}



function openCustomerByName(name) {
  PageMethods.GetCustomerID(name, function(id) {
    OpenCustomer(id);
  });
}

function GetAndWaitFrame(id) {
  var frame = null;
  var start = (new Date()).getTime();
  while (frame == null) {
    if ((new Date()).getTime() - start > 10000) return null;
    frame = $get(id);
  }

  /*while (frame.conentWindow == null) {
  if ((new Date()).getTime() - start > 10000) return null;
  }*/
  return frame;
}


function sleep(timeMS) {
  var waiting = true;
  var start = (new Date()).getTime();
  while (waiting) {
    if ((new Date()).getTime() - start > timeMS) waiting = false;
  }
}


function HideInfoPane() {
  $find(get_paneInfoClientID()).collapse();


}

function OpenWikiArticle(id) {
  var item = _menuTree.getItemByID('mniWiki');
  var data = item.getData();
  var url = data.ContentUrl;
  DeleteContentFrame(GetMainFrameID('mniWiki'));
  data.ContentUrl = 'Wiki/ViewPage.aspx?ArticleID=' + id;
  item.setData(data);
  _menuTree.setSelected('mniWiki');
  data.ContentUrl = url;
  item.setData(data);
}

function OpenEmail(emailID) {
  PageMethods.GetEmailLink(emailID, function(result) {
    if (result != null) {
      alert(result);
      window.open(result);
    }

  });

}

function ResendEmail(emailID) {
  PageMethods.ResendEmail(emailID, function(result) {
    alert("Email resent");
  });

}
function getInternetExplorerVersion() {
  // Returns the version of Internet Explorer or a -1 (indicating the use of another browser).
  var rv = -1;
  if (navigator.appName == 'Microsoft Internet Explorer') {
    var ua = navigator.userAgent;
    var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
    if (re.exec(ua) != null)
      rv = parseFloat(RegExp.$1);
  }
  return rv;
}

function ShowUserDialog(filter, isModal, callback, title) {
  var wnd = $find(get_wndSelectUserDialogClientID());
  _userDialogFilter = filter;
  ShowDialog(wnd, isModal, callback, title);
}

function ShowTicketDialog(isModal, callback, title) {
  var wnd = $find(get_wndSelectTicketDialogClientID());
  ShowDialog(wnd, isModal, callback, title);
}

function ShowDialog(wnd, isModal, callback, title) {
  if (title && title != '') wnd.set_title(title);
  wnd.set_modal(isModal);
  if (callback) {
    var fn = function(sender, args) { sender.remove_close(fn); callback(sender.argument); }
    wnd.add_close(fn);
  }
  wnd.show();
}

function OpenSupport() {
  if (_userInfo == null) {
    PageMethods.GetUserInfo(function(result) {
      _userInfo = result;
      OpenSupport();
    });
  }
  else {
    window.open("http://www.teamsupport.com/customer_portal_login.php?OrganizationID=1078&UserName=" + _userInfo.Email + "&Password=57EE1F58-5C8B-4B47-B629-BE7C702A2022", "TSPortal");
  }
}