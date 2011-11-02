/// <reference path="ts/ts.js" />
/// <reference path="ts/ts.services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="~/Default.aspx" />
/// <reference path="ts/ts.pages.tickets.js" />
window.name = "TSMain";

Ts.Pages.Main = function () {
  this.MainLayout = null;
  this.MainTabs = null;
  this.MainMenu = null;
};

Ts.Pages.Main.prototype =
{
  constructor: Ts.Pages.Main,

  loadUserStatus: function () {
    if (Ts.System.ChatUserSettings.IsAvailable) { $('.main-status-chat').removeClass('ui-state-disabled'); } else { $('.main-status-chat').addClass('ui-state-disabled'); }
    if (Ts.System.User.InOffice) { $('.main-status-online').switchClass('ts-icon-offline', 'ts-icon-online', 0); } else { $('.main-status-online').switchClass('ts-icon-online', 'ts-icon-offline', 0); }

    var status = Ts.System.User.InOfficeComment === '' ? 'What is your status?' : Ts.System.User.InOfficeComment;
    $('.main-header-status-text').text(status).siblings('input').val(status);
    $('.main-status-left').html('Logged in as: ' + Ts.System.User.FirstName + ' ' + Ts.System.User.LastName + ' - ' + Ts.System.Organization.Name);
  },

  getCalcStyle: function () {
    var colorHeader = $('.main-header').css('background-color');
    var colorBorder = $('.main-header').css('border-bottom-color');
    var colorContent = $('.main-nav').css('background-color');
    colorContent = "transparent";

    return '<style type="text/css">.ui-layout-resizer { background-color: ' + colorBorder + '; border: 0px solid ' + colorHeader + '; }' +
    '.ui-layout-toggler { background-color: ' + colorHeader + ';}</style>'; // 'li.ts-menutree-item  div { border:1px solid ' + colorContent + ';}</style>';  
  },

  init: function () {
    var self = this;
    var timer = setInterval(processMessages, 3500);
    var flash = setInterval(flashTitle, 1250);
    var isIdle = false;
    var lastChatMessageID = -1;
    var lastChatRequestID = -1;
    var lastWCMessageID = -1;
    var refreshID = -1;
    var chatRequestCount = 0;
    var chatMessageCount = 0;
    var wcMessageCount = 0;
    var isDebug = false;

    $('head').append(this.getCalcStyle());
    $('.main-loading').hide();
    $('.main-container').show();
    $('button').button();

    $('.main-quick-panel a').hover(function () { $(this).addClass('ui-state-active'); }, function () { $(this).removeClass('ui-state-active'); });
    $('.main-link-newticket').click(function (e) { e.preventDefault(); self.newTicket(); });
    $.editLabel('.main-header-status-text', function (result) { Ts.Services.Users.UpdateUserStatusComment(result, function (user) { Ts.System.User = user; }); }, 'What is your status?', true);
    $('.main-status-chat').click(function () {
      Ts.Services.Users.ToggleUserChatStatus(function (setting) {
        Ts.System.ChatUserSettings = setting;
        self.loadUserStatus();
      });
    });



    $('.main-header-links a').addClass('ui-state-default ui-corner-all').hover(function (e) { $(this).addClass('ui-state-hover'); }, function (e) { $(this).removeClass('ui-state-hover'); });

    $('.main-status-online').click(function () {
      Ts.Services.Users.ToggleUserStatus(function (user) {
        Ts.System.User = user;
        self.loadUserStatus();
      });
    });

    self.loadUserStatus();

    $(".dialog-select-ticket").dialog({
      height: 150,
      width: 300,
      autoOpen: false,
      modal: true,
      buttons: {
        OK: function () { $(this).dialog("close"); },
        Cancel: function () { $(this).dialog("close"); }
      }
    });

    var execSelectTicket = null;
    function selectTicket(request, response) {
      if (execSelectTicket) { execSelectTicket._executor.abort(); }
      var filter = $(this.element).data('filter');
      if (filter === undefined) {
        execSelectTicket = top.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) { response(result); });
      }
      else {
        execSelectTicket = top.Ts.Services.Tickets.SearchTickets(request.term, filter, function (result) { response(result); });
      }
    }

    $(".dialog-select-ticket input").autocomplete({
      minLength: 2,
      source: selectTicket,
      select: function (event, ui) { $(this).data('item', ui.item).removeClass('ui-autocomplete-loading') },
      position: { my: "right top", at: "right bottom", collision: "fit flip" }
    });


    this.MainLayout = $('.main-container').layout({
      resizeNestedLayout: true,
      fxName: "slide",
      fxSpeed: "slow",
      center: {
        paneSelector: ".main-content"
      },
      north: {
        paneSelector: ".main-header",
        closable: false,
        resizable: false,
        size: 60,
        spacing_open: 0
      },
      south: {
        paneSelector: ".main-footer",
        closable: false,
        resizable: false,
        spacing_open: 0
      },
      east: {
        paneSelector: ".main-info",
        size: 200,
        minSize: 100,
        maxSize: 300,
        initClosed: true,
        spacing_open: 3,
        onopen: function () {
          Ts.Services.Settings.WriteUserSetting('main-info-state', true);
        },
        onclose: function () {
          Ts.Services.Settings.WriteUserSetting('main-info-state', false);
        }
      },
      west: {
        paneSelector: ".main-nav",
        spacing_open: 2,
        minSize: 150,
        maxSize: 300,
        size: 185,
        closable: false

      }
    });


    $('.main-nav').layout({
      resizeNestedLayout: true,
      defaults: {
        spacing_open: 0,
        closable: false,
        resizable: false
      },
      center: {
        paneSelector: ".main-menutree"
      },
      north: {
        paneSelector: ".main-quick-panel"
      }
    });

    $('.main-content').layout({
      resizeNestedLayout: true,
      defaults: {
        spacing_open: 0,
        closable: false,
        resizable: false
      },
      center: {
        paneSelector: ".main-tab-content"
      },
      north: {
        paneSelector: ".main-tabs",
        size: 31

      }
    });

    function beforeMenuItemSelect(item) {
      var mi = self.MainMenu.getSelected();
      if (mi == null) return;

      var element = $('.main-tab-content-item-' + mi.getId()).children('iframe');
      if (element.length < 1) return;
      var contentFrame = element[0];
      try { if (contentFrame.contentWindow.onHide) contentFrame.contentWindow.onHide(); } catch (err) { }

      if (mi.getType() == 'wc') {
        element.parent().remove();
      }
    }

    function processQuery() {
      Ts.Services.Settings.ReadUserSetting('main-menu-selected', 'mniDashboard', function (selectedID) {
        self.MainMenu.getByID(selectedID).select();

        var ticketID = Ts.Utils.getQueryValue('ticketid');
        if (ticketID) {
          self.openTicketByID(ticketID, true);
        }
        var ticketNumber = Ts.Utils.getQueryValue('ticketnumber');
        if (ticketNumber) {
          self.openTicket(ticketNumber, true);
        }

        var contactID = Ts.Utils.getQueryValue('contactid');
        var customerID = Ts.Utils.getQueryValue('customerid');
        if (contactID) {
          self.openContact(contactID);
        }
        else if (customerID) {
          self.openCustomer(customerID);
        }

        var customerName = Ts.Utils.getQueryValue('customername');
        if (customerName) {
          Ts.Services.Organizations.GetIDByName(customerName, function (result) {
            self.openCustomer(result);
          });
        }

        var phoneNumber = Ts.Utils.getQueryValue('phonenumber');
        if (phoneNumber) {
          Ts.Services.Organizations.GetIDByPhone(phoneNumber, function (result) {
            if (result[1] > -1) self.openContact(result[1], result[0]);
            else if (result[0] > -1) self.openCustomer(result[0]);

          });
        }

        var articleID = Ts.Utils.getQueryValue('articleid');
        if (articleID) {
          self.openWiki(articleID);
        }

        var productID = Ts.Utils.getQueryValue('productid');
        var versionID = Ts.Utils.getQueryValue('versionid');
        if (productID || versionID) {
          self.openProduct(productID, versionID);
        }

        var organizationProductID = Ts.Utils.getQueryValue('organizationproductid');
        if (organizationProductID) {
          self.openOrganizationProduct(organizationProductID);
        }

      });
    }

    function processMessages() {

      Ts.Services.System.GetMainPageUpdate(lastChatMessageID, lastChatRequestID, lastWCMessageID, Ts.System.getSessionID(), function (result) {
        if (result === "" || result.RefreshID === undefined || result.IsExpired === undefined) return;
        if (result.Version === null) return;
        if (result.IsExpired == true || (result.RefreshID != refreshID && refreshID > -1)) {
          clearInterval(timer);
          window.location = '.';
          return;
        }

        if (isDebug != result.IsDebug) {
          isDebug = result.IsDebug;
          if (isDebug) {
            $('.status-frame').show();
            $('.status-expiration').show();
            $('.status-debug').show();
          }
          else {
            $('.status-frame').hide();
            $('.status-expiration').hide();
            $('.status-debug').hide();
          }
        }

        $('.status-expiration').text('Expires @ ' + result.ExpireTime);

        refreshID = result.RefreshID;
        $('.status-version').html('Version: ' + result.Version);

        if (result.IsIdle != isIdle) {
          isIdle = result.IsIdle;
          clearInterval(timer);
          timer = setInterval(processMessages, isIdle ? 30000 : 4000);
        }

        if (window.location.hostname.indexOf('127.0.0.1') > -1) {
          clearInterval(timer);
        }


        var menuID = self.MainMenu.getSelected().getId();
        var isMain = mainTabs.find(0, Ts.Ui.Tabs.Tab.Type.Main).getIsSelected();
        if ((!isMain || menuID != 'mniChat') && (chatMessageCount < result.ChatMessageCount || chatRequestCount < result.ChatRequestCount)) {
          self.MainMenu.find('mniChat', 'chat').setIsHighlighted(true);
          for (var i = 0; i < result.NewChatMessages.length; i++) {
            $.jGrowl(result.NewChatMessages[i].Message, { life: 5000, theme: result.NewChatMessages[i].State, header: result.NewChatMessages[i].Title });
          }
        }

        for (var i = 0; i < result.NewChatRequests.length; i++) {
          $.jGrowl(result.NewChatRequests[i].Message, { life: 5000, theme: result.NewChatRequests[i].State, header: result.NewChatRequests[i].Title });
        }

        if ((!isMain || menuID != 'mniWC') && (wcMessageCount < result.WCMessageCount)) {
          self.MainMenu.find('mniWC', 'wc').setIsHighlighted(true);
          for (var i = 0; i < result.NewWCMessages.length; i++) {
            $.jGrowl(result.NewWCMessages[i].Message, { life: 5000, theme: result.NewWCMessages[i].State, header: result.NewWCMessages[i].Title });
          }
        }


        lastChatMessageID = result.LastChatMessageID;
        lastChatRequestID = result.LastChatRequestID;
        lastWCMessageID = result.LastWCMessageID;
        chatMessageCount = result.ChatMessageCount;
        chatRequestCount = result.ChatRequestCount;
        wcMessageCount = result.WCMessageCount;

        flashTitle();


        if (result.NewChatRequests.length > 0) {
          window.focus();
          alert(result.NewChatRequests[0].Message);
          Ts.Services.System.GetMainPageUpdate(lastChatMessageID, lastChatRequestID, lastWCMessageID, Ts.System.getSessionID(), function (result2) {
            if (result2.ChatRequestCount < 1) {
              alert('All chat requests are already answered.');
            }
          });

          window.focus();
        }
        /*public int WCMessageCount { get; set; }
        public int ChatMessageCount { get; set; }
        public int ChatRequestCount { get; set; }
        public GrowlMessage[] NewChatMessages { get; set; }
        public GrowlMessage[] NewChatRequests { get; set; }
        public GrowlMessage[] NewWCMessages { get; set; }
        public int LastWCMessageID { get; set; }
        public int LastChatMessageID { get; set; }
        public int LastChatRequestID { get; set; }*/
      },
      function (error) {
        if (error.get_statusCode() == 401) {
          window.location = 'SessionExpired.aspx';
        }

      });
    }

    function flashTitle() {
      if (document.title != 'Team Support') {
        document.title = 'Team Support';
      }
      else if (chatRequestCount > 1) {
        document.title = chatRequestCount + ' New Chat Requests';
      }
      else if (chatRequestCount == 1) {
        document.title = chatRequestCount + ' New Chat Request';
      }
      else if (chatMessageCount > 1) {
        document.title = chatMessageCount + ' New Chat Messages';
      }
      else if (chatMessageCount == 1) {
        document.title = chatMessageCount + ' New Chat Message';
      }
      /*else if (wcMessageCount > 1) {
      document.title = wcMessageCount + ' New Water Cooler Messages';
      }
      else if (wcMessageCount == 1) {
      document.title = wcMessageCount + ' New Water Cooler Message';
      }*/

    }

    function openMenuItem(item) {
      var mainTab = mainTabs.find(0, Ts.Ui.Tabs.Tab.Type.Main);
      mainTab.setCaption(item.getCaption());
      mainTab.setImageUrl(item.getImageUrl());
      mainTab.select();
    }



    this.MainMenu = new Ts.Ui.MenuTree($('.main-menutree')[0]);
    this.MainMenu.bind('afterSelect', openMenuItem);
    this.MainMenu.bind('beforeSelect', beforeMenuItemSelect);

    Ts.Services.System.GetMainMenuItems(function (result) {
      function updateMenuItems(items, parent) {
        for (var i = 0; i < items.length; i++) {
          var item = self.MainMenu.add(parent, items[i].ID, items[i].Type, items[i].Caption, items[i].ImageUrl, JSON.parse(items[i].Data));
          if (items.length > 0) { updateMenuItems(items[i].Items, item); }
        }
      }
      updateMenuItems(result, null);

      Ts.Services.System.GetMainTabs(function (result) {
        self.MainTabs.add(false, Ts.Ui.Tabs.Tab.Type.Main, 0, 'Dashboard', false, false, false, '', 'images/nav/16/dashboard.png');
        if (result) {
          for (var i = 0; i < result.length; i++) {
            //isSelected, tabType, id, caption, isClosable, isSortable, isHighlighted, icon, imageUrl
            if (result[i].TabType === Ts.Ui.Tabs.Tab.Type.Ticket) { self.MainTabs.add(false, result[i].TabType, result[i].ID, result[i].Caption, true, true, false); }
          }
        }

        mainTabs.bind('afterSelect', openTab);
        mainTabs.bind('afterAdd', saveTabs);
        mainTabs.bind('beforeRemove', function (tab) {
          var div = null;
          switch (tab.getTabType()) {
            case Ts.Ui.Tabs.Tab.Type.Ticket:
              div = $('.main-ticket-' + tab.getId());
              break;
            case Ts.Ui.Tabs.Tab.Type.NewTicket:
              div = $('.main-ticket-new');
              break;
            default:
          }

          if (div) {
            div.children('iframe').remove();
            div.remove();
          }

        });
        mainTabs.bind('afterRemove', function () { saveTabs(mainTabs); });
        mainTabs.bind('sort', saveTabs);


        processQuery();
      });


    });


    function openTab(tab) {

      $('.main-tab-content .ts-loading').show();
      $('.main-tab-content-item').hide();
      var div = null;
      switch (tab.getTabType()) {
        case Ts.Ui.Tabs.Tab.Type.Main:
          var item = self.MainMenu.getSelected();

          Ts.Services.Settings.WriteUserSetting('main-menu-selected', item.getId());

          if (item.getType() == 'wc') {
            self.MainMenu.find('mniWC', 'wc').setIsHighlighted(false);
            Ts.Services.System.UpdateLastWaterCoolerID();
          }

          if (item.getType() == 'chat') {
            self.MainMenu.find('mniChat', 'chat').setIsHighlighted(false);
          }
          div = $('.main-tab-content-item-' + item.getId());
          if (div.length < 1) {
            div = $('<div>')
              .addClass('main-tab-content-item-' + item.getId() + ' main-tab-content-item')
              .appendTo('.main-tab-content');

            var frame = $('<iframe>')
              .attr('frameborder', 0)
              .attr('scrolling', 'no')
              .appendTo(div)
              .attr('src', item.getData().ContentUrl);
          }
          else {
            div.show();
            var data = div.data('pageData');
            if (data) {
              data.refresh();
            }
            else {
              var contentFrame = $(div).children('iframe')[0];
              try { if (contentFrame.contentWindow.refreshData) contentFrame.contentWindow.refreshData(); } catch (err) { }
              try { if (contentFrame.contentWindow.onShow) contentFrame.contentWindow.onShow(); } catch (err) { }
            }
          }

          $('.main-info-content').load(item.getData().PaneInfoUrl);
          break;
        case Ts.Ui.Tabs.Tab.Type.Ticket:
          div = $('.main-tab-content .main-ticket-' + tab.getId());
          if (div.length < 1) {
            div = $('<div>')
              .addClass('main-tab-content-item main-tab-ticket main-ticket-' + tab.getId())
              .appendTo('.main-tab-content');

            $('<iframe>')
              .attr('frameborder', 0)
              .attr('scrolling', 'no')
              .appendTo(div)
              .attr('src', 'Resources_149/Pages/Ticket.html?TicketNumber=' + tab.getId());
          }
          else {
            div.show();
          }
          $('.main-info-content').load('Resources_149/PaneInfo/ticket.html');

          break;
        case Ts.Ui.Tabs.Tab.Type.NewTicket:
          div = $('.main-tab-content .main-ticket-new');
          if (div.length < 1) {
            var query = '';
            if (tab.getData()) query = tab.getData();
            div = $('<div>')
              .addClass('main-tab-content-item main-tab-newticket main-ticket-new')
              .appendTo('.main-tab-content');

            var showNewTicket = window.location.hostname.indexOf('127.0.0.1') > -1 || window.location.hostname.indexOf('beta') > -1 || window.location.hostname.indexOf('kevin') > -1;
            var nwLnk = showNewTicket ? 'Resources_149/Pages/NewTicket.html' : 'frames/newticket.aspx';

            $('<iframe>')
              .attr('frameborder', 0)
              .attr('scrolling', 'no')
              .appendTo(div)
              .attr('src', nwLnk + query);
            //.attr('src', 'frames/newticket.aspx' + query);
          }
          else {
            div.show();
          }
          $('.main-info-content').load('Resources_149/PaneInfo/newticket.html');
          break;
        default:

      }


      $('.main-tab-content .ts-loading').hide();
      Ts.Services.System.UpdateLastActivity();
      $('.status-frame').text(div.children('iframe').attr('src'));
    }

    function saveTabs(sender) {
      var tabs = sender.getTabs();
      var items = [];
      for (var i = 0; i < tabs.length; i++) {
        var tab = items[items.length] = new TSWebServices.TsTabDataItem();
        tab.ID = tabs[i].getId();
        tab.TabType = tabs[i].getTabType();
        tab.Caption = tabs[i].getCaption();
      }

      Ts.Services.System.SetMainTabs(items);
    }

    this.MainTabs = new Ts.Ui.Tabs($('.main-tabs')[0], '.main-tab-content');
    var mainTabs = this.MainTabs;




    $('.main-info-close').click(function (e) { e.preventDefault(); self.MainLayout.close('east'); });

    var execGetTicket = null;
    function getTicketsByTerm(request, response) {
      if (execGetTicket) { execGetTicket._executor.abort(); }
      //execGetTicket = Ts.Services.Tickets.GetTicketsByTerm(request.term, function (result) { response(result); });
      execGetTicket = Ts.Services.Tickets.SearchTickets(request.term, null, function (result) {
        $('.main-quick-ticket').removeClass('ui-autocomplete-loading');
        response(result);
      });

    }

    $('.main-quick-ticket').autocomplete({ minLength: 2, source: getTicketsByTerm, delay: 300,
      select: function (event, ui) {
        if (ui.item) { self.openTicket(ui.item.id); }
        $('.main-quick-ticket').removeClass('ui-autocomplete-loading');
      }
    });

    $('.main-quick-ticket')
    .focusin(function () { $(this).val('').removeClass('main-quick-ticket-blur'); })
    .focusout(function () { $(this).val('Search for a ticket...').addClass('main-quick-ticket-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('main-quick-ticket-blur'); })
    .val('Search for a ticket...');

    top.Ts.Services.Users.ShowIntroVideo(function (result) {
      var overrideIntro = Ts.Utils.getQueryValue('intro');
      if (result === false && !(overrideIntro != null && overrideIntro == 1)) return;
      var div = $('<div>')
        .addClass('dialog-intro')
        .append('<iframe width="420" height="315" src="https://www.youtube.com/embed/BVl7zLVzT7E?rel=0&autoplay=1" frameborder="0" allowfullscreen></iframe>')
        .appendTo('body');

      div.dialog({
        width: 'auto',
        height: 'auto',
        title: 'Introduction',
        resizable: false,
        close: function () {
          div.remove();
        }
      });
    });


    Ts.Services.Settings.ReadUserSetting('main-info-state', true, function (isOpen) {

      if (isOpen == "True") self.MainLayout.open('east');

    });

  }, // end init

  openTicketByID: function (ticketID, doSelect) {
    var self = this;
    Ts.Services.Tickets.GetTicketNumber(ticketID, function (number) {
      self.openTicket(number, doSelect);
    });
  },
  openTicket: function (ticketNumber, doSelect) {
    //isSelected, tabType, id, caption, isClosable, isSortable, isHighlighted, icon, imageUrl
    doSelect = doSelect == null ? true : doSelect;
    this.MainTabs.add(doSelect, Ts.Ui.Tabs.Tab.Type.Ticket, ticketNumber, 'Ticket: ' + ticketNumber, true, true, false);
  },
  newTicket: function (query) {
    //isSelected, tabType, id, caption, isClosable, isSortable, isHighlighted, icon, imageUrl, data
    this.MainTabs.add(true, Ts.Ui.Tabs.Tab.Type.NewTicket, 'new', 'New Ticket', true, true, true, null, null, query);
  },
  closeNewTicketTab: function () {
    var tab = this.MainTabs.find('new', Ts.Ui.Tabs.Tab.Type.NewTicket);
    if (tab) {
      this.closeTab(tab);
      tab.remove();
    }
  },
  closeTab: function (tab) {
    return tab.remove();
  },
  closeTicketTab: function (ticketNumber) {
    var tab = this.MainTabs.find(ticketNumber, Ts.Ui.Tabs.Tab.Type.Ticket);
    if (tab) { return this.closeTab(tab); }
    return false;
  },
  highlightTicketTab: function (ticketNumber, isHighlighted) {
    var tab = this.MainTabs.find(ticketNumber, Ts.Ui.Tabs.Tab.Type.Ticket);
    if (tab) {
      tab.setIsHighlighted(isHighlighted);
    }
  },
  openAttachment: function (attachmentID) {
    window.open('../Attachment.aspx?attachmentID=' + attachmentID, 'Attachment' + attachmentID);
  },
  openUser: function (userID) {
    var self = this;
    Ts.Services.Organizations.IsUserContact(userID, function (isContact) {
      if (isContact) { self.openContact(userID); return; }
      Ts.Services.Settings.WriteUserSetting('SelectedUserID', userID, function () {
        Ts.Services.Settings.WriteUserSetting('SelectedUserTabIndex', 0, function () {
          self.MainMenu.find('mniUsers', 'users').select();
          var element = $('.main-tab-content-item:visible');
          var contentFrame = $(element).children('iframe')[0];
          if (contentFrame && contentFrame.contentWindow.refreshData) {
            contentFrame.contentWindow.refreshData;
          }
        });
      });


    });
  },
  openContact: function (contactID, customerID) {
    var self = this;
    if (customerID == null) {
      Ts.Services.Organizations.GetUser(contactID, function (user) {
        if (user) self.openContact(contactID, user.OrganizationID);
      });
    }

    _selectContactID = contactID;
    _selectCustomerID = customerID;

    this.MainMenu.find('mniCustomers', 'customers').select();
    var element = $('.main-tab-content-item:visible');
    var contentFrame = $(element).children('iframe')[0];
    if (contentFrame && contentFrame.contentWindow.selectCustomer) {
      contentFrame.contentWindow.selectContact(contactID, customerID);
    }


  },
  openCustomerByName: function (name) {
    var self = this;
    Ts.Services.Organizations.GetIDByName(name, function (id) {
      self.openCustomer(id);
    });
  },
  openCustomer: function (customerID) {
    _selectContactID = -1;
    _selectCustomerID = customerID;

    this.MainMenu.find('mniCustomers', 'customers').select();
    var element = $('.main-tab-content-item:visible');
    var contentFrame = $(element).children('iframe')[0];
    if (contentFrame && contentFrame.contentWindow.selectCustomer) {
      contentFrame.contentWindow.selectCustomer(customerID);
    }
  },
  openProduct: function (productID, versionID) {
    var self = this;
    if (versionID == null) versionID = -1;
    Ts.Services.Settings.WriteUserSetting('SelectedProductTabIndex', versionID < 0 ? 0 : 1, function () {
      Ts.Services.Settings.WriteUserSetting('SelectedProductID', productID, function () {
        Ts.Services.Settings.WriteUserSetting('SelectedVersionID', versionID, function () {
          self.MainMenu.find('mniProducts', 'products').select();
          var element = $('.main-tab-content-item:visible');
          var contentFrame = $(element).children('iframe')[0];
          if (contentFrame && contentFrame.contentWindow.refreshData) {
            contentFrame.contentWindow.refreshData;
          }

        });
      });
    });

  },
  openWiki: function (articleID) {
    this.MainMenu.find('mniWiki', 'wiki').select();
    var element = $('.main-tab-content-item:visible');
    $(element).children('iframe').attr('src', 'Wiki/ViewPage.aspx?ArticleID=' + articleID);
  },
  openOrganizationProduct: function (organizationProductID) {
    var self = this;
    Ts.Services.Organizations.GetOrganizationProduct(organizationProductID, function (op) {
      self.openProduct(op.ProductID, op.ProductVersionID == null ? -1 : op.ProductVersionID);
    });
  },
  openTag: function (tagID) {
    this.MainMenu.find('mniTicketTags', 'tickettags').select();
    var element = $('.main-tab-content-item:visible');
    $(element).children('iframe').attr('src', 'Frames/TicketTags.aspx?TagID=' + tagID);
  },
  addDebugStatus: function (text) {
    $('.status-debug').text(text);
  },
  selectTicket: function (filter, callback) {
    $('.dialog-select-ticket').find('input').data('filter', filter).val('');
    var buttons = $('.dialog-select-ticket').dialog('option', 'buttons');

    buttons.OK = function () {
      $(this).dialog("close");
      callback($(this).find('input').data('item').data);
    }
    $('.dialog-select-ticket').dialog('option', 'buttons', buttons).dialog('open').find('input').focus();
  }

};

