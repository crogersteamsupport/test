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
  this.WndScreenR = null;
};

Ts.Pages.Main.prototype =
{
  constructor: Ts.Pages.Main,

  loadUserStatus: function () {
    if (Ts.System.ChatUserSettings.IsAvailable) {
      $('.main-status-chat').removeClass('ui-state-disabled');
      $('.menu-chatstatus .ts-icon').addClass('ts-icon-chat-small');
      $('.menu-chatstatus-text').text('Chat: Online');
    }
    else {
      $('.menu-chatstatus-text').text('Chat: Offline');
      $('.menu-chatstatus .ts-icon').addClass('ts-icon-nochat-small');
      $('.main-status-chat').addClass('ui-state-disabled');
    }

    if (Ts.System.User.InOffice) {
      $('.main-status-online').switchClass('ts-icon-offline', 'ts-icon-online', 0);
      $('.menu-officestatus .ts-icon').addClass('ts-icon-online-small');
    }
    else {
      $('.menu-officestatus .ts-icon').addClass('ts-icon-offline-small');
      $('.main-status-online').switchClass('ts-icon-online', 'ts-icon-offline', 0);
    }

    var status = Ts.System.User.InOfficeComment === '' ? 'What is your status?' : Ts.System.User.InOfficeComment;
    $('.main-header-status-text').text(status).siblings('input').val(status);
    $('.menu-status-text').val(Ts.System.User.InOfficeComment).data('o', Ts.System.User.InOfficeComment);
    $('.main-status-left').html('Logged in as: ' + Ts.System.User.FirstName + ' ' + Ts.System.User.LastName + ' - ' + Ts.System.Organization.Name);
  },

  getCalcStyle: function () {
    var colorHeader = $('.main-footer').css('background-color');
    var colorBorder = $('.main-footer').css('border-top-color');
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

    $('.menu-signout').click(function (e) {
      e.preventDefault();
      top.Ts.System.signOut();
    });

    $('.menu-help-support').click(function (e) {
      e.preventDefault();
      top.Ts.System.openSupport();
    });

    $('.menu-help-chat').click(function (e) {
      e.preventDefault();
      window.open('https://app.teamsupport.com/Chat/ChatInit.aspx?uid=22bd89b8-5162-4509-8b0d-f209a0aa6ee9', 'TSChat', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=450,height=500');
    });

    $('.menu-help-switch').click(function (e) {
      e.preventDefault();
      Ts.Services.Users.SetIsClassicView(!(Ts.System.User.IsClassicView == true), function () {
        window.location = '.';
      });
    });


    $('.main-header-menu li')
      .hover(function () { $(this).addClass('hover'); }, function () { $('.main-header-menu li').removeClass('hover'); })
      .click(function (e) {
        e.stopPropagation();
        if (menuBlur != null) clearTimeout(menuBlur);
      });

    $('.menu-popup').click(function (e) {
      e.stopPropagation();
    });

    $('.menu-chatstatus').click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      Ts.Services.Users.ToggleUserChatStatus(function (setting) {
        Ts.System.ChatUserSettings = setting;
        if (Ts.System.ChatUserSettings.IsAvailable) {
          $('.menu-chatstatus .ts-icon').addClass('ts-icon-chat-small').removeClass('ts-icon-nochat-small');
          $('.menu-chatstatus-text').text('Chat: Online');
        }
        else {
          $('.menu-chatstatus .ts-icon').addClass('ts-icon-nochat-small').removeClass('ts-icon-chat-small');
          $('.menu-chatstatus-text').text('Chat: Offline');
        }
      });
    });

    $('.menu-officestatus').click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      hidePopupMenus();
      $(this).addClass('selected ui-widget-content ui-corner-top').removeClass('hover');
      $('.menu-popup-officestatus').css('left', $(this).offset().left).show().find('.menu-input').focus();
    });

    $('.menu-office-online').click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      Ts.Services.Users.UpdateUserStatus(true, function () {
        $('.menu-officestatus .ts-icon').addClass('ts-icon-online-small').removeClass('ts-icon-offline-small');
      });
      hidePopupMenus();
    });

    $('.menu-office-offline').click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      Ts.Services.Users.UpdateUserStatus(false, function () {
        $('.menu-officestatus .ts-icon').addClass('ts-icon-offline-small').removeClass('ts-icon-online-small');
      });
      hidePopupMenus();
    });

    $('.menu-input-hidden').css('opacity', '0');

    $('.menu-popup-officestatus li').hover(function () { $(this).addClass('ui-state-hover'); }, function () { $('.menu-popup-officestatus li').removeClass('ui-state-hover'); });

    $('.menu-status-text').keypress(function () {
      $('.menu-office-status-action').show();
    });

    $('.menu-office-save').click(function (e) {
      e.preventDefault();
      $('.menu-office-status-action').hide();
      $('.menu-status-text').data('o', $('.menu-status-text').val())
      Ts.Services.Users.UpdateUserStatusComment($('.menu-status-text').val());
    });

    $('.menu-office-cancel').click(function (e) {
      e.preventDefault();
      $('.menu-status-text').val($('.menu-status-text').data('o'));
      $('.menu-office-status-action').hide();
    });


    $('.menu-help').click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      hidePopupMenus();
      $(this).addClass('selected ui-widget-content ui-corner-top').removeClass('hover');
      $('.menu-popup-help').show().css('left', $(this).offset().left + $(this).outerWidth() - $('.menu-popup-help').outerWidth()).find('.menu-input').focus();
    });

    var menuBlur = null;
    $('.menu-input').blur(function () {
      try { // fix for dumb ie
        if ($('.main-header-menu:focus').length > 0) return;
      } catch (e) {

      }
      menuBlur = setTimeout(hidePopupMenus, 500);
    });

    $(document).click(function (e) { hidePopupMenus(); });

    function hidePopupMenus() {
      try { // fix for dumb ie
        if ($('.main-header-menu:focus').length > 0) return;
      } catch (e) {

      }
      $('.menu-popup').hide();
      $('.main-header-menu li').removeClass('selected ui-widget-content ui-corner-top');
    }


    $('.old-header-links a').addClass('ui-state-default ui-corner-all').hover(function (e) { $(this).addClass('ui-state-hover'); }, function (e) { $(this).removeClass('ui-state-hover'); });
    $('.main-header-links li').addClass('ui-state-default ui-corner-all').hover(function (e) { $(this).addClass('ui-state-hover'); }, function (e) { $(this).removeClass('ui-state-hover'); });

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

    function setupReminderDialog() {
      $(".dialog-reminder").dialog({
        height: 250,
        width: 300,
        autoOpen: false,
        modal: true,
        buttons: {
          OK: function () { $(this).dialog("close"); },
          Cancel: function () { $(this).dialog("close"); }
        }
      });

      $('.dialog-reminder .reminder-date').datetimepicker();
      $('.dialog-reminder .reminder-user').combobox();

    }

    setupReminderDialog();

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
        size: (Ts.System.User.IsClassicView == true ? 60 : 80),
        spacing_open: 1
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
        size: 150,
        closable: false

      }
    });

    if (Ts.System.User.IsClassicView == true) {
      $('.classic-view').show();
      $('.new-view').hide();
      $('.main-header').addClass('classic-view-header ui-widget-header').removeClass('new-view-header');
      $('.main-tabs').removeClass('main-tabs');
      $('.classic-tabs').addClass('main-tabs');
      $('.menu-popup').addClass('ui-corner-all').removeClass('ui-corner-bottom new-view-menu');
      $('.menu-help-switch').text('Switch to the new view.');
      if (Ts.System.User.Email.toLowerCase().indexOf('kjones') > -1 ||
           Ts.System.User.Email.toLowerCase().indexOf('eharrington') - 1 ||
           Ts.System.User.Email.toLowerCase().indexOf('jharada') - 1 ||
           true) {
        $('.old-header-links').hide().next().show();
      }



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

    }

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
      var defaultMenuItem = 'mniWelcome';
      if ($('.menutree-item-welcome-mniWelcome').length < 1) defaultMenuItem = 'mniDashboard';

      Ts.Services.Settings.ReadUserSetting('main-menu-selected', defaultMenuItem, function (selectedID) {
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

        if (window.location.hostname.indexOf('127.0.0.1x') > -1) {
          clearInterval(timer);
        }

        if (result.OpenUnreadTicketCount && result.OpenUnreadTicketCount != null && result.OpenUnreadTicketCount > 0) {
          $('.menutree-item-mytickets-mniMyTickets a').text('My Tickets (' + result.OpenUnreadTicketCount + ')').css('font-weight', 'bold');
        }
        else {
          $('.menutree-item-mytickets-mniMyTickets a').text('My Tickets').css('font-weight', 'normal');
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
      var caption = item.getCaption();
      if (caption.indexOf('My Tickets') == 0) caption = 'My Tickets';
      mainTab.setCaption(caption);
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
            if (result[i].TabType === Ts.Ui.Tabs.Tab.Type.Ticket && (result[i].Caption != null || result[i].Caption != '')) {
              self.MainTabs.add(false, result[i].TabType, result[i].ID, result[i].Caption, true, true, false, result[i].Title);
            }
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

          if (item.getType() == 'wc2') {
            self.MainMenu.find('mniWC2', 'wc2').setIsHighlighted(false);
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
              .attr('src', 'vcr/142/Pages/Ticket.html?TicketNumber=' + tab.getId());
          }
          else {
            div.show();
          }
          $('.main-info-content').load('vcr/142/PaneInfo/ticket.html');

          break;
        case Ts.Ui.Tabs.Tab.Type.NewTicket:
          div = $('.main-tab-content .main-ticket-new');
          if (div.length < 1) {
            var query = '';
            if (tab.getData()) query = tab.getData();
            div = $('<div>')
              .addClass('main-tab-content-item main-tab-newticket main-ticket-new')
              .appendTo('.main-tab-content');

            $('<iframe>')
              .attr('frameborder', 0)
              .attr('scrolling', 'no')
              .appendTo(div)
              .attr('src', 'vcr/142/Pages/NewTicket.html' + query);
            //.attr('src', 'frames/newticket.aspx' + query);
          }
          else {
            div.show();
          }
          $('.main-info-content').load('vcr/142/PaneInfo/newticket.html');
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
    /*
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
    */


    Ts.Services.Settings.ReadUserSetting('main-info-state', true, function (isOpen) {

      if (isOpen == "True") self.MainLayout.open('east');

    });

    this.WndScreenR = $('<iframe>', { id: 'wndScreenR', name: 'wndScreenR', src: 'ScreenR.html', width: '0', height: '0', frameborder: '0', scrolling: 'no' }).appendTo('body')[0];


  }, // end init

  recordScreen: function (params, onComplete, onCancel) {
    if (!params) {
      params = new Object();
      params.userName = top.Ts.System.User.FirstName + ' ' + top.Ts.System.User.LastName;
      params.userEmail = top.Ts.System.User.Email;
      params.hideAllFields = true;
      params.maxTimeLimit = 300;
    }

    params.id = "b67bdeab7c084032bc4f37e5308eae1e";

    var recorder = this.WndScreenR.contentWindow.Screenr.Recorder({ id: "b67bdeab7c084032bc4f37e5308eae1e", hideAllFields: true, maxTimeLimit: 300 }); // this.WndScreenR.contentWindow.Screenr.Recorder(params);
    if (onComplete) recorder.setOnComplete(onComplete);
    if (onCancel) recorder.setOnCancel(onCancel);


    recorder.record();


  },

  openTicketByID: function (ticketID, doSelect) {
    var self = this;
    Ts.Services.Tickets.GetTicketNumber(ticketID, function (number) {
      self.openTicket(number, doSelect);
    });
  },
  openTicket: function (ticketNumber, doSelect) {
    //isSelected, tabType, id, caption, isClosable, isSortable, isHighlighted, icon, imageUrl
    var self = this;
    doSelect = doSelect == null ? true : doSelect;
    Ts.Services.Tickets.GetTicketName(ticketNumber, function (name) {
      self.MainTabs.add(doSelect, Ts.Ui.Tabs.Tab.Type.Ticket, ticketNumber, 'Ticket: ' + ticketNumber, true, true, false, null, null, null, name);
    });
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
  openAsset: function (assetID) {
    //alert('Needs implementation');
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
  openAdmin: function (tabText) {
    var self = this;
    top.Ts.Settings.Organization.write('SelectedAdminTabText', tabText, function () {
      self.MainMenu.find('mniAdmin', 'admin').select();
    });


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
  hideWelcome: function () {
    this.MainMenu.find('mniDashboard', 'dashboard').select();
    $('.menutree-item-welcome-mniWelcome').remove();

    top.Ts.Services.Users.HideWelcomePage(function () { });
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
  },

  editReminder: function (reminder, doSave, callback) {
    $('.dialog-reminder').dialog('option', 'width', 300);
    $('.dialog-reminder').dialog('option', 'height', 300);

    var select = $('.dialog-reminder .reminder-user');
    select.empty();
    var users = top.Ts.Cache.getUsers();
    if (users != null) {
      for (var i = 0; i < users.length; i++) {
        $('<option>').attr('value', users[i].UserID).text(users[i].Name).data('o', users[i]).appendTo(select);
      }
    }

    var buttons = $('.dialog-reminder').dialog('option', 'buttons');

    buttons.OK = function () {
      if ($.trim($('.dialog-reminder .reminder-description').val()) == '') {
        $('.dialog-reminder .reminder-description').closest('.label-block').addClass('ui-state-error ui-corner-all');
        return;
      }

      reminder.Description = $('.dialog-reminder .reminder-description').val();
      reminder.DueDate = top.Ts.Utils.getMsDate($('.dialog-reminder .reminder-date').datetimepicker('getDate'));
      reminder.UserID = $('.dialog-reminder .reminder-user').val();
      var dialog = $(this);
      if (doSave == false) {
        dialog.dialog("close");
        if (callback) callback(reminder);
      }
      else {
        //(int? reminderID, ReferenceType refType, int refID, string description, DateTime dueDate, int userID)
        top.Ts.Services.System.EditReminder(
        reminder.ReminderID,
        reminder.RefType,
        reminder.RefID,
        reminder.Description,
        reminder.DueDate,
        reminder.UserID,
        function (result) {
          dialog.dialog("close");
          if (callback) callback(result);
        },
        function () {
          alert('There was an error saving your reminder.  Please try again.');
        }
        );
      }
    }

    $('.dialog-reminder').dialog('option', 'buttons', buttons).dialog('open');

    if (!reminder.RefID) { reminder.RefID = null; }
    if (!reminder.RefType) { reminder.RefType = null; }

    if (reminder.ReminderID) {
      $('.dialog-reminder').find('.ts-loading').show().next().hide();
      top.Ts.Services.System.GetReminder(reminder.ReminderID, function (result) {
        reminder = result;
        $('.dialog-reminder .reminder-description').val(reminder.Description);
        $('.dialog-reminder .reminder-date').datetimepicker('setDate', reminder.DueDate),
        $('.dialog-reminder .reminder-user').combobox('setValue', reminder.UserID);
        $('.dialog-reminder').find('.ts-loading').hide().next().show();
      });
    }
    else {
      reminder.ReminderID = null;
      $('.dialog-reminder').find('.ts-loading').hide().next().show();
      $('.dialog-reminder .reminder-description').val((params.Description ? reminder.Description : ''));
      $('.dialog-reminder .reminder-date').datetimepicker('setDate', (reminder.DueDate ? reminder.DueDate : new Date()));
      select.combobox('setValue', (reminder.UserID ? reminder.UserID : top.Ts.System.User.UserID));
    }
    //$('.reminder-description').val((params.Description ? params.Description : ''));
    //$('.reminder-description').val((params.Description ? params.Description : ''));
  }




};

