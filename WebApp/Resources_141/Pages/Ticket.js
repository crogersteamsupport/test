/// <reference path="ts/ts.js" />
/// <reference path="ts/ts.services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="~/Default.aspx" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.ui.ticketgrid.js" />



$(document).ready(function () {
  var self = this;
  $('.ts-loading').show().next().hide();

  var editorOptions = {
    theme: "advanced",
    skin: "o2k7",
    plugins: "autoresize",
    theme_advanced_buttons1: "bold,italic,underline,strikethrough,bullist,numlist,fontselect,fontsizeselect,forecolor,backcolor,|,link,unlink,|,code",
    theme_advanced_buttons2: "",
    theme_advanced_buttons3: "",
    theme_advanced_buttons4: "",
    theme_advanced_toolbar_location: "top",
    theme_advanced_toolbar_align: "left",
    theme_advanced_statusbar_location: "none",
    theme_advanced_resizing: true,
    convert_urls: true,
    relative_urls: false,
    content_css: "../Css/jquery-ui-latest.custom.css",
    body_class: "ui-widget",
    template_external_list_url: "tinymce/jscripts/template_list.js",
    external_link_list_url: "tinymce/jscripts/link_list.js",
    external_image_list_url: "tinymce/jscripts/image_list.js",
    media_external_list_url: "tinymce/jscripts/media_list.js",
    setup: function (ed) { /*ed.onChange.add(function (ed, l) { isModified(true); });*/ }
    //,oninit: onEditorInit
  };

  $("#editorNewAction").tinymce(editorOptions);



  loadTicket(top.Ts.Utils.getQueryValue('TicketNumber', window));
  function loadTicket(ticketNumber) {
    top.Ts.Services.Tickets.GetTicketInfo(ticketNumber, function (info) {

      $('.ts-loading').hide().next().show();
      $('.ticket-layout').layout({
        resizeNestedLayout: true,
        defaults: {
          spacing_open: 0,
          closable: false
        },
        center: {
          paneSelector: ".ticket-panel-content"
        },
        north: {
          paneSelector: ".ticket-panel-toolbar",
          size: 31
        }
      });

      if (info == null) alert('no ticket');

      $('#ticketNumber').text(info.Ticket.TicketNumber);
      $('#ticketName').text(info.Ticket.Name).click(function (e) { e.preventDefault(); });
      $('#creator').text(info.Ticket.CreatorName);
      $('#dateCreated').text(info.Ticket.DateCreated.localeFormat(top.Sys.CultureInfo.CurrentCulture.dateTimeFormat.FullDateTimePattern));
      $('#isTicketPortal').text((info.Ticket.IsVisibleOnPortal == true ? 'Yes' : 'No'));
      $('#isTicketKB').text((info.Ticket.IsKnowledgebase == true ? 'Yes' : 'No'));
      $('#ticketType').text(info.Ticket.TicketTypeName);
      $('#ticketStatus').text(info.Ticket.Status);
      $('#ticketSeverity').text(info.Ticket.Severity);
      $('#userName').text(info.Ticket.UserName == null ? 'Unassigned' : info.Ticket.UserName);
      $('#ticketGroup').text(info.Ticket.GroupName == null ? 'Unassigned' : info.Ticket.GroupName);
      $('#product').text(info.Ticket.ProductName == null ? 'Unassigned' : info.Ticket.ProductName);
      $('#reported').text(info.Ticket.ReportedVersion == null ? 'Unassigned' : info.Ticket.ReportedVersion);
      $('#resolved').text(info.Ticket.SolvedVersion == null ? 'Unassigned' : info.Ticket.SolvedVersion);

      // last modi date, name, time spent

      var details = $('#divDetails').empty();
      if (info.Ticket.IsClosed == true) {
        $('<div>').text('Days Closed: ' + info.Ticket.DaysClosed).appendTo(details);
      }
      else {
        $('<div>').text('Days Opened: ' + info.Ticket.DaysOpened).appendTo(details);
      }
      if (info.Ticket.ModifierName) $('<div>').text('Last Modified By: ' + info.Ticket.ModifierName).appendTo(details);
      if (info.Ticket.DateModified) $('<div>').text('Last Modified On: ' + info.Ticket.DateModified.localeFormat(top.Ts.Utils.getDateTimePattern())).appendTo(details);
      $('<div>').text('Total Time Spent: ' + top.Ts.Utils.getTimeSpentText(info.Ticket.HoursSpent)).appendTo(details);

      function ellipseString(text, max) { return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; };

      function getRemovableItem(title, description) {
        var item = $('<div>').addClass('ticket-removable-item ui-corner-all ts-color-bg-accent');

        $('<span>').addClass('ui-icon ui-icon-close').appendTo(item);
        if (title) $('<div>').addClass('ticket-removable-item-title').text(ellipseString(title, 30)).appendTo(item);
        if (description) $('<div>').addClass('ticket-removable-item-description').text(ellipseString(description, 30)).appendTo(item);
        return item;
      }

      $('#divCustomers').empty();

      for (var i = 0; i < info.Customers.length; i++) {
        var customer = info.Customers[i];
        if (customer.UserID) {
          $('#divCustomers').append(getRemovableItem(customer.Contact, customer.Company));
        }
        else {
          $('#divCustomers').append(getRemovableItem(customer.Company));
        }
      }

      $('#divTags').empty();
      for (var i = 0; i < info.Tags.length; i++) {
        $('#divTags').append(getRemovableItem(info.Tags[i].Value));
      }

      $('#divRelated').empty();
      for (var i = 0; i < info.Related.length; i++) {
        var related = info.Related[i];
        var item = getRemovableItem('', '');
        $('<a href="#">')
          .text(ellipseString(related.TicketNumber + ': ' + related.Name, 30))
          .data('number', related.TicketNumber)
          .click(function (e) {
            e.preventDefault();
            top.Ts.MainPage.openTicket($(this).data('number'), true);
          })
          .appendTo(item);

        $('#divRelated').append(item);
      }

      $('#divActions').empty();
      for (var i = 0; i < info.Actions.length; i++) {
        var actionDiv = $('<div>')
          .addClass('ticket-action')
          .data('action', info.Actions[i]);

        var title = $('<h1>').appendTo(actionDiv);
        $('<span>').addClass('ts-icon ts-icon-open').appendTo(title);
        $('<span>').addClass('ticket-action-title').text(info.Actions[i].Name).appendTo(title);
        var edit = $('<span>').addClass('ticket-action-edit').text(info.Actions[i].Name).appendTo(title);

        $('<div>').addClass('ui-helper-clearfix').appendTo(actionDiv);
        $('<div>').addClass('ui-widget-content ts-separator').appendTo(actionDiv);


        $('<div>').html(info.Actions[i].Description).appendTo(actionDiv);
        $('#divActions').append(actionDiv);
      }


      $('.ticket-header a').addClass('ui-state-default ts-link');
      $('.ticket-rail a').addClass('ui-state-default ts-link');

    });
  }


  $('button').button();
  $('#btnRefresh').click(function (e) { e.preventDefault(); window.location = window.location; });

});

