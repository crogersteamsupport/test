/// <reference path="ts/ts.js" />
/// <reference path="ts/ts.services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="~/Default.aspx" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.ui.ticketgrid.js" />
Date = top.Date;

$(document).ready(function () {
  var _self = this;
  var _ticketID = -1;
  var _ticket = null;
  $('.page-loading').show().next().hide();

  $(".dialog-emailinput").dialog({
    height: 200,
    width: 400,
    autoOpen: false,
    modal: true,
    buttons: { OK: function () {
      $(this).dialog("close");
      top.Ts.Services.Tickets.EmailTicket(_ticketID, $(".dialog-emailinput input").val(), function () {
        alert('Your emails have been sent.');
      }, function () {
        alert('There was an error sending the ticket email');
      });
    },
      Cancel: function () { $(this).dialog("close"); }
    }
  });

  $('.ticket-rail .collapsable')
    .prepend('<span class="ui-icon ui-icon-triangle-1-s">')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      var icon = $(this).find('.ui-icon');
      if (icon.hasClass('ui-icon-triangle-1-s')) {
        icon.removeClass('ui-icon-triangle-1-s').addClass('ui-icon-triangle-1-e');
        $(this).next().hide();
      }
      else {
        icon.removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');
        $(this).next().show();
      }
    });


  $('.ticket-action-show-history').click(function (e) {
    e.preventDefault();
    $(this).hide().next().show();

    /*
    top.Ts.Services.Tickets.GetTicketHistory(_ticketID, function (logs) {
      for (var i = 0; i < logs.length; i++) {
        $('<div>').text(logs[i].CreatorName).appendTo('.ticket-action-history');
      }
    }, function () {
      alert('There was a problem retrieving the historu for the ticket.');
    });
    */




  });




  $('.ticket-widget').addClass('ui-widget-content ui-corner-all');

  function appendCustomers(customers) {
    $('#divCustomers').empty().append('<div class="ts-separator ui-widget-content">');

    for (var i = 0; i < customers.length; i++) {
      appendCustomer(customers[i]);
    }
  }

  function appendCustomer(customer) {
    var itemClass = (customer.UserID ? 'ticket-customer-contact' : 'ticket-customer-company');
    var item = $('<div>')
      .addClass('ticket-removable-item ui-corner-all ts-color-bg-accent ' + itemClass)
      .data('data', customer);

    if (customer.UserID) {
      $('<span>').addClass('ui-icon ui-icon-close').appendTo(item);
      var title = $('<div class="ticket-removable-item-title">').appendTo(item);
      $('<a href="#">')
        .click(function (e) {
          e.preventDefault();
          top.Ts.MainPage.openContact(customer.UserID);
        })
        .text(ellipseString(customer.Contact, 30))
        .appendTo(title);
      $('<span class="ts-icon ts-icon-info">')
      .attr('rel', 'Tips/User.aspx?UserID=' + customer.UserID)
      .cluetip(clueTipOptions)
      .appendTo(title);

      var desc = $('<div class="ticket-removable-item-description">').appendTo(item);
      $('<a href="#">')
        .click(function (e) {
          e.preventDefault();
          top.Ts.MainPage.openCustomer(customer.OrganizationID);
        })
        .text(ellipseString(customer.Company, 30))
        .appendTo(desc);
      $('<span class="ts-icon ts-icon-info">')
      .attr('rel', 'Tips/Customer.aspx?CustomerID=' + customer.OrganizationID)
      .cluetip(clueTipOptions)
      .appendTo(desc);
    }
    else {
      $('<span>').addClass('ui-icon ui-icon-close').appendTo(item);
      var title = $('<div class="ticket-removable-item-title">').appendTo(item);
      $('<a href="#">')
        .click(function (e) {
          e.preventDefault();
          top.Ts.MainPage.openCustomer(customer.OrganizationID);
        })
        .text(ellipseString(customer.Company, 30))
        .appendTo(title);

      $('<span class="ts-icon ts-icon-info">')
      .attr('rel', 'Tips/Customer.aspx?CustomerID=' + customer.OrganizationID)
      .cluetip(clueTipOptions)
      .appendTo(title);
    }
    $('#divCustomers').append(item);

  }

  $('#divCustomers').delegate('.ticket-customer-contact .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var item = $(this).parent();
    var data = item.data('data');
    top.Ts.Services.Tickets.RemoveTicketContact(_ticketID, data.UserID, function (customers) {
      appendCustomers(customers);
    }, function () {
      alert('There was a problem removing the contact from the ticket.');
    });
  });

  $('#divCustomers').delegate('.ticket-customer-company .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var item = $(this).parent();
    var data = item.data('data');
    top.Ts.Services.Tickets.RemoveTicketCompany(_ticketID, data.OrganizationID, function (customers) {
      appendCustomers(customers);
    }, function () {
      alert('There was a problem removing the company from the ticket.');
    });
  });

  var execGetCustomer = null;
  function getCustomers(request, response) {
    if (execGetCustomer) { execGetCustomer._executor.abort(); }
    execGetCustomer = top.Ts.Services.Organizations.GetUserOrOrganization(request.term, function (result) { response(result); });
  }


  $('.ticket-customer-add')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      $('.ticket-rail-input').remove();
      $(this).parent().find('.ui-icon').removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');
      var container = $('<div>').
       addClass('ticket-rail-input')
       .prependTo($(this).parent().next().show());
      var input = $('<input type="text">')
        .addClass('ui-corner-all ui-widget-content')
        .autocomplete({
          minLength: 2,
          source: getCustomers,
          select: function (event, ui) {
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading')
            .next().show();
          }
        })
        .appendTo(container)
        .focus()
        .width(container.width() - 48 - 12);

      $('<span>')
        .addClass('ts-icon ts-icon-save')
        .hide()
        .click(function (e) {
          var item = $(this).prev().data('item');
          top.Ts.Services.Tickets.AddTicketCustomer(_ticketID, item.data, item.id, function (customers) {
            appendCustomers(customers);
            $(this).parent().remove();
          }, function () {
            $(this).parent().remove();
            alert('There was an error adding the customer.');
          });

        })
        .appendTo(container)

      $('<span>')
        .addClass('ts-icon ts-icon-cancel')
        .click(function (e) {
          $(this).parent().remove();
        })
        .appendTo(container);

      $('<div class="ui-helper-clearfix">').appendTo(container);
    });

  var execGetTags = null;
  function getTags(request, response) {
    if (execGetTags) { execGetTags._executor.abort(); }
    execGetTags = top.Ts.Services.Tickets.SearchTags(request.term, function (result) { response(result); });
  }

  function appendTags(tags) {
    $('#divTags').empty();
    for (var i = 0; i < tags.length; i++) {
      var item = getRemovableItem(tags[i], 'ticket-tag', 'crap');
      var link = $('<a href="#">')
        .text(ellipseString(tags[i].Value, 30))
        .click(function (e) {
          e.preventDefault();
          top.Ts.MainPage.openTag(tags[i].TagID);
        });
      item.find('.ticket-removable-item-title').empty().append(link);
      $('#divTags').append(item);
    }
  }

  $('#divTags').delegate('.ticket-tag .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var item = $(this).parent();
    var data = item.data('data');
    top.Ts.Services.Tickets.RemoveTag(_ticketID, data.TagID, function (tags) {
      appendTags(tags);
    }, function () {
      alert('There was a problem removing the tag from the ticket.');
    });
  });


  $('.ticket-tag-add')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      $('.ticket-rail-input').remove();
      $(this).parent().find('.ui-icon').removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');
      var container = $('<div>').
       addClass('ticket-rail-input')
       .prependTo($(this).parent().next().next().show());
      var input = $('<input type="text">')
        .addClass('ui-corner-all ui-widget-content')
        .autocomplete({
          minLength: 2,
          source: getTags,
          select: function (event, ui) {
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading')
          }
        })
        .appendTo(container)
        .focus()
        .width(container.width() - 48 - 12);

      $('<span>')
        .addClass('ts-icon ts-icon-save')
        .click(function (e) {
          top.Ts.Services.Tickets.AddTag(_ticketID, $(this).prev().val(), function (tags) {
            appendTags(tags);
            $(this).parent().remove();
          }, function () {
            $(this).parent().remove();
            alert('There was an error adding the tag.');
          });

        })
        .appendTo(container)

      $('<span>')
        .addClass('ts-icon ts-icon-cancel')
        .click(function (e) {
          $(this).parent().remove();
        })
        .appendTo(container);

      $('<div class="ui-helper-clearfix">').appendTo(container);
    });

  var execGetRelated = null;
  function getRelated(request, response) {
    if (execGetRelated) { execGetRelated._executor.abort(); }
    execGetRelated = top.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) { response(result); });
  }

  function appendRelated(tickets) {
    $('#divRelated').empty();
    for (var i = 0; i < tickets.length; i++) {
      var related = tickets[i];

      var item = getRemovableItem(related, 'ticket-related', ' ', ' ');

      var icon = 'ts-icon-ticket-related';
      var caption = 'Related';

      if (related.IsParent !== null) {
        icon = (related.IsParent === true ? 'ts-icon-ticket-parent' : 'ts-icon-ticket-child');
        caption = (related.IsParent === true ? 'Parent' : 'Child');
      }

      $('<span>')
      .addClass('ts-icon ' + icon)
      .appendTo(item.find('.ticket-removable-item-title').empty());

      $('<span>')
      .addClass('ticket-removable-text')
      .text(caption)
      .appendTo(item.find('.ticket-removable-item-title'));

      $('<span>')
      .addClass('ts-icon ts-icon-info')
      .attr('rel', 'Tips/Ticket.aspx?TicketID=' + related.TicketID)
      .cluetip(clueTipOptions)
      .appendTo(item.find('.ticket-removable-item-title'));

      $('<a href="#">')
          .text(ellipseString(related.TicketNumber + ': ' + related.Name, 30))
          .data('number', related.TicketNumber)
          .click(function (e) {
            e.preventDefault();
            top.Ts.MainPage.openTicket($(this).data('number'), true);
          })
          .appendTo(item.find('.ticket-removable-item-description').empty());

      $('#divRelated').append(item);
    }
  }

  $('#divRelated').delegate('.ticket-related .ui-icon-close', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var item = $(this).parent();
    var data = item.data('data');
    top.Ts.Services.Tickets.RemoveRelated(_ticketID, data.TicketID, function (result) {
      if (result !== null && result === true) item.remove();
    }, function () {
      alert('There was an error removing the associated ticket.');
    });

  });


  $('.ticket-related-add')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      $('.ticket-rail-input').remove();
      $(this).parent().find('.ui-icon').removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');
      var container = $('<div>').
       addClass('ticket-rail-input')
       .prependTo($(this).parent().next().next().show());
      var input = $('<input type="text">')
        .addClass('ui-corner-all ui-widget-content related-input')
        .autocomplete({
          minLength: 2,
          source: getRelated,
          select: function (event, ui) {
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading')
          },
          position: { my: "right top", at: "right bottom", collision: "fit flip" }
        })
        .appendTo(container)
        .focus()
        .width(container.width() - 30);

      $('<span>')
        .addClass('ts-icon ts-icon-cancel')
        .click(function (e) {
          $(this).parent().remove();
        })
        .appendTo(container);

      $('<div class="ui-helper-clearfix">').appendTo(container);

      var buttons = $('<div>').addClass('buttons').appendTo(container);
      $('<strong>').text('Add as: ').appendTo(buttons);
      $('<button>').text('Related').appendTo(buttons).button().click(function (e) { addRelated(null); });
      $('<button>').text('Parent').appendTo(buttons).button().click(function (e) { addRelated(false); });
      $('<button>').text('Child').appendTo(buttons).button().click(function (e) { addRelated(true); });
      function addRelated(isParent) {
        var item = container.find('.related-input').data('item');
        top.Ts.Services.Tickets.AddRelated(_ticketID, item.data, isParent, function (tickets) {
          appendRelated(tickets);
          container.remove();
        }, function (error) {
          //container.remove();
          alert(error.get_message());
        });
      }
    });

  var clueTipOptions = {
    mouseOutClose: true,
    width: 350,
    hoverIntent: {
      sensitivity: 1,
      interval: 50,
      timeout: 0
    },
    cluetipClass: 'ui-corner-all',
    showTitle: false,
    dropShadow: true,
    dropShadowSteps: 10,
    sticky: true,
    ajaxProcess: function (data) {
      //data = data.replace(/<(style|title)[^<]+<\/(style|title)>/gm, '').replace(/<(link|meta)[^>]+>/g, '');
      return data;
    },
    onShow: function (ct, c) {
      if (tipTimer != null) clearTimeout(tipTimer);
      ct.addClass('ui-corner-all').find('.ui-cluetip-content').addClass('ui-corner-all').find('a').addClass('ts-link ui-state-default');
    },
    onActivate: function (ct, c) {
      $(this).mouseout(function () {
        alert('out');
      });
    }
  };
  $('body').delegate('.ts-icon-info', 'mouseout', function (e) {
    if (tipTimer != null) clearTimeout(tipTimer);
    tipTimer = setTimeout("$(document).trigger('hideCluetip');", 1000);
  });

  var tipTimer = null;

  $('body').delegate('.cluetip', 'mouseover', function (e) {
    if (tipTimer != null) clearTimeout(tipTimer);
  });

  function initEditor(element, init) {
    var editorOptions = {
      theme: "advanced",
      skin: "o2k7",
      plugins: "autoresize, jqueryinlinepopups",
      theme_advanced_buttons1: "insertTicket,insertKb,|,bold,italic,underline,strikethrough,bullist,numlist,fontselect,fontsizeselect,forecolor,backcolor,|,link,unlink,|,code",
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
      setup: function (ed) {
        ed.addButton('insertTicket', {
          title: 'Insert Ticket',
          image: '../images/nav/16/tickets.png',
          onclick: function () {
            top.Ts.MainPage.selectTicket(null, function (ticketID) {
              top.Ts.Services.Tickets.GetTicket(ticketID, function (ticket) {
                ed.focus();
                var html = '<a href="https://app.teamsupport.com?TicketNumber=' + ticket.TicketNumber + '" target="_blank" onclick="top.Ts.MainPage.openTicket(' + ticket.TicketNumber + '); return false;">Ticket ' + ticket.TicketNumber + '</a>';
                ed.selection.setContent(html);
                ed.execCommand('mceAutoResize');
                ed.focus();
              }, function () {
                alert('There was a problem inserting the ticket link.');
              });
            });
          }
        });
        ed.addButton('insertKb', {
          title: 'Insert Knowledgebase',
          image: '../images/nav/16/knowledge.png',
          onclick: function () {
            filter = new top.TeamSupport.Data.TicketLoadFilter();
            filter.IsKnowledgeBase = true;
            top.Ts.MainPage.selectTicket(filter, function (ticketID) {
              top.Ts.Services.Tickets.GetKBTicketAndActions(ticketID, function (result) {
                if (result === null) {
                  alert('There was an error inserting your knowledgebase ticket.');
                  return;
                }
                var ticket = result[0];
                var actions = result[1];

                var html = '<div><h2>' + ticket.Name + '</h2>';

                for (var i = 0; i < actions.length; i++) {
                  html = html + '<div>' + actions[i].Description + '</div></br>';
                }
                html = html + '</div>';

                ed.focus();
                ed.selection.setContent(html);
                ed.execCommand('mceAutoResize');
                ed.focus();

                //needs to resize or go to end

              }, function () {
                alert('There was an error inserting your knowledgebase ticket.');
              });
            });
          }
        });
      }
    , oninit: init
    };
    $(element).tinymce(editorOptions);
  }

  $('.ticket-action-add').click(function (e) {
    e.preventDefault();
    var parent = $(this).hide();
    $('.ticket-action-new').show();
    createActionForm($('<div>').appendTo('.ticket-action-new-form'), null, function (result) {
      if (result !== null) {
        var actionDiv = createActionElement().prependTo('#divActions');
        loadActionDisplay(actionDiv, result, true);
      }
      parent.show();
      $('.ticket-action-new').hide();
    });
  })
  .hover(function (e) {
    $(this).addClass('ui-state-hover');
  }, function (e) {
    $(this).removeClass('ui-state-hover');
  });

  $('#divActions').delegate('.ticket-action-edit', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var actionDiv = $(this).parents('.ticket-action');
    var action = actionDiv.data('action');
    var form = $('<div>').addClass('');
    actionDiv.append(form).find('.ticket-action-display').hide();
    createActionForm(form, action, function (actionInfo) {
      if (actionInfo !== null) { loadActionDisplay(actionDiv, actionInfo, true); }
      actionDiv.find('.ticket-action-display').show();
    });
  });


  function createActionForm(element, action, callback) {
    element = $(element).html($('#divActionForm').html()).addClass('ticket-action-form');
    var selectType = element.find('.ticket-action-form-actiontype');
    var types = top.Ts.Cache.getActionTypes();
    var newAction = null;
    for (var i = 0; i < types.length; i++) {
      $('<option>').attr('value', types[i].ActionTypeID).text(types[i].Name).appendTo(selectType);
    }
    selectType.combobox();
    element.find('.ticket-action-form-date').datetimepicker()
    element.find('.ticket-action-form-hours').spinner({ min: 0 });
    element.find('.ticket-action-form-minutes').spinner({ min: 0 });
    initEditor(element.find('.ticket-action-form-description'), function (ed) {
      if (action != null && action.Description != null) {
        ed.setContent(action.Description);
      }
      element.find('.ticket-action-form-description').tinymce().focus();
    });
    element.find('.ticket-action-form-cancel').click(function () { callback(null); element.remove(); });
    element.find('.ticket-action-form-save').click(function () {
      saveAction(element, action, function (result) {

        if (element.find('.upload-queue li').length > 0 && result !== null) {
          newAction = result;
          element.find('.upload-queue li').each(function (i, o) {
            var data = $(o).data('data');
            data.form.prop('action', '../../Upload/Actions/' + action.ActionID);
            data.jqXHR = data.submit();
            $(o).data('data', data);
          });
        }
        else {
          element.remove();
          callback(result);
        }
      });
    });
    var uploadName = 'new_action';
    if (action != null) {
      uploadName = 'action_' + action.ActionID;
      element.find('.ticket-action-form-name').val(action.Name);

      if (top.Ts.SystemActionTypes.Custom != action.SystemActionTypeID) {
        element.find('.ticket-action-form-block-actiontype').hide();
      }
      else {
        selectType.combobox('setValue', action.ActionTypeID);
      }

      if (action.DateStarted != null) {
        element.find('.ticket-action-form-date').datetimepicker('setDate', action.DateStarted);
      }

      if (action.TimeSpent != null) {
        element.find('.ticket-action-form-hours').val(Math.floor(action.TimeSpent / 60));
        element.find('.ticket-action-form-minutes').val(Math.floor(action.TimeSpent % 60));
      }

      if (action.IsKnowledgeBase === true) {
        element.find('.ticket-action-form-kb').prop('checked', true);
      }

      if (action.IsVisibleOnPortal === true) {
        element.find('.ticket-action-form-portal').prop('checked', true);
      }
    }

    element.find('.file-upload').fileupload({
      namespace: uploadName,
      dropZone: element,
      add: function (e, data) {
        for (var i = 0; i < data.files.length; i++) {
          var item = $('<li>')
          .appendTo(element.find('.upload-queue'));

          data.context = item;
          item.data('data', data);

          var bg = $('<div>')
          .addClass('ts-color-bg-accent ui-corner-all')
          .appendTo(item);

          $('<div>')
          .text(data.files[i].name + '  (' + top.Ts.Utils.getSizeString(data.files[i].size) + ')')
          .addClass('filename')
          .appendTo(bg);

          $('<div>')
          .addClass('progress')
          .hide()
          .appendTo(bg);

          $('<span>')
          .addClass('ui-icon ui-icon-close')
          .click(function (e) {
            e.preventDefault();
            $(this).closest('li').fadeOut(500, function () { $(this).remove(); });
          })
          .appendTo(bg);

          $('<span>')
          .addClass('ui-icon ui-icon-cancel')
          .hide()
          .click(function (e) {
            e.preventDefault();
            var data = $(this).closest('li').data('data');
            data.jqXHR.abort();
          })
          .appendTo(bg);
        }

      },
      send: function (e, data) {
        if (data.context && data.dataType && data.dataType.substr(0, 6) === 'iframe') {
          data.context.find('.progress').progressbar('value', 50);
        }
      },
      fail: function (e, data) {
        if (data.errorThrown === 'abort') return;
        alert('There was an error uploading "' + data.files[0].name + '".');
      },
      progress: function (e, data) {
        data.context.find('.progress').progressbar('value', parseInt(data.loaded / data.total * 100, 10));
      },
      start: function (e, data) {
        element.find('.progress').progressbar().show();
        element.find('.upload-queue .ui-icon-close').hide();
        element.find('.upload-queue .ui-icon-cancel').show();
      },
      stop: function (e, data) {
        element.find('.progress').progressbar('value', 100);

        top.Ts.Services.Tickets.GetActionInfo(newAction.Action.ActionID, function (result) {
          callback(result);
        });
        element.remove();

      }
    });
  }

  function saveAction(form, oldAction, callback) {
    element = $(form);
    var action = new top.TeamSupport.Data.ActionProxy();
    action.ActionID = oldAction === null ? -1 : oldAction.ActionID;
    action.TicketID = _ticketID;
    if (element.data('action') !== undefined) { action.ActionID = element.data('action').ActionID; }

    action.ActionTypeID = element.find('.ticket-action-form-actiontype').val();
    action.SystemActionTypeID = 0;
    action.DateStarted = top.Ts.Utils.getMsDate(element.find('.ticket-action-form-date').datetimepicker('getDate'));
    action.TimeSpent = parseInt(element.find('.ticket-action-form-hours').val()) * 60 + parseInt(element.find('.ticket-action-form-minutes').val());
    action.IsKnowledgeBase = element.find('.ticket-action-form-kb').prop('checked');
    action.IsVisibleOnPortal = element.find('.ticket-action-form-portal').prop('checked');
    action.Description = element.find('.ticket-action-form-description').html();

    top.Ts.Services.Tickets.UpdateAction(action, function (result) {
      callback(result)
    }, function (error) {
      callback(null);
    });
  }

  function createActionElement() {
    return $('<div>')
      .addClass('ticket-action ui-widget-content ui-corner-all ticket-content-section')
      .html($('#divActionDisplay').html())
      .hover(function (e) {
        e.preventDefault();
        $(this).find('.ticket-action-edit').show();
        $(this).find('.ticket-action-delete').show();
      }, function (e) {
        e.preventDefault();
        $('.ticket-action-edit').hide();
        $('.ticket-action-delete').hide();

      });
  }

  function appendActionElement(actionInfo) {
    var actionDiv = createActionElement().appendTo('#divActions');
    loadActionDisplay(actionDiv, actionInfo, true);
  }

  function loadActionDisplay(element, actionInfo, doExpand) {
    element = $(element);
    element
    .data('action', actionInfo.Action)
    .data('creator', actionInfo.Creator)
    .data('attachments', actionInfo.Attachments);

    var action = actionInfo.Action
    var attachments = actionInfo.Attachments;
    var creator = actionInfo.Creator;


    if (doExpand) {
      if (action.Description == null) {
        element.find('.ticket-action-expand-icon').removeClass('ui-icon-triangle-1-s').addClass('ui-icon-triangle-1-e');
      }
      else {
        element.find('.ticket-action-expand-icon').removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');
      }
    }
    element.find('.ticket-action-title').text(action.DisplayName);
    element.find('.ticket-action-kb').toggleClass('ts-icon-kb', action.IsKnowledgeBase === true).toggleClass('ts-icon-kbnot', action.IsKnowledgeBase === false);
    element.find('.ticket-action-portal').toggleClass('ts-icon-portal', action.IsVisibleOnPortal === true).toggleClass('ts-icon-portalnot', action.IsVisibleOnPortal === false);
    if (attachments.length < 1) {
      element.find('.ticket-action-attachment-count').hide();
      element.find('.ticket-action-attachments').hide();
    }
    var creatorIcon = 'ts-icon-customer';
    if (creator !== null && creator.OrganizationID === top.Ts.System.User.OrganizationID) {
      creatorIcon = 'ts-icon-tsuser';
    }

    $('<span>')
      .addClass('ts-icon creator-type ' + creatorIcon)
      .appendTo(element.find('.ticket-action-info').empty());

    $('<a href="#">')
    .click(function (e) {
      e.preventDefault();
      top.Ts.MainPage.openCustomer(creator.UserID);
    })
    .text((creator !== null ? (creator.FirstName + ' ' + creator.LastName) : 'Anonymous'))
    .addClass('name')
    .appendTo($('<span>').appendTo(element.find('.ticket-action-info')));

    $('<span>')
      .addClass('ts-icon ts-icon-info')
      .attr('rel', 'Tips/User.aspx?UserID=' + creator.UserID)
      .cluetip(clueTipOptions)
      .appendTo(element.find('.ticket-action-info'));

    $('<span>')
      .addClass('date')
      .text(' - ' + action.DateCreated.localeFormat(top.Ts.Utils.getDateTimePattern()))
      .appendTo(element.find('.ticket-action-info'));


    element.find('.ticket-action-work').empty();
    //if (action.DateStarted !== null) element.find('.ticket-action-work').append('Started on ' + action.DateCreated.localeFormat(top.Ts.Utils.getDateTimePattern()) + ' ');
    if (action.TimeSpent !== null && action.TimeSpent > 0) element.find('.ticket-action-work').append(top.Ts.Utils.getTimeSpentText(action.TimeSpent / 60));

    if (action.Description != null) {
      var attachmentList = element.find('.ticket-action-attachments ul').empty();



      for (var i = 0; i < attachments.length; i++) {
        var item = $('<li>')
        .appendTo(attachmentList)
        .data('attachment', attachments[i])
        .hover(function (e) {
          $(this).find('.ticket-action-attachment-delete').show();
        }, function (e) {
          $('.ticket-action-attachment-delete').hide();
        })
        $('<span>')
        .addClass('ts-icon ts-icon-attachment')
        .appendTo(item);

        $('<a target="_blank">')
        .text(attachments[i].FileName)
        .addClass('ui-state-default ts-link')
        .attr('href', '../dc/1078/attachments/' + attachments[i].AttachmentID)
        .appendTo(item);

        $('<span>')
        .text('Delete')
        .addClass('ui-icon ui-icon-close ticket-action-attachment-delete')
        .hide()
        .click(function (e) {
          var element = $(this).closest('li');
          var attachment = element.data('attachment');
          if (!confirm('Are you sure you would like to delete "' + attachment.FileName + '."')) return;
          top.Ts.Services.Tickets.DeleteAttachment(
            attachment.AttachmentID,
            function () {
              element.remove();
            }, function () {
              alert('There was a problem deleting "' + attachment.FileName + '."');
            });
        })
        .appendTo(item);
      }

      element.find('.ticket-action-description').html(action.Description);
      element.find('.ticket-action-body').show();
    }

    element.find('a').addClass('ui-state-default ts-link');

  }

  $('#divActions').delegate('.ticket-action-delete', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    if (confirm('Are you sure you would like to delete this action?')) {
      var actionDiv = $(this).parents('.ticket-action');
      var action = actionDiv.data('action');
      top.Ts.Services.Tickets.DeleteAction(action.ActionID, function () { actionDiv.remove(); }, function () { alert('There was an error deleting this action.'); });
    }
  });

  $('#divActions').delegate('.ticket-action-kb', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var icon = $(this);
    var actionDiv = $(this).parents('.ticket-action');
    var action = actionDiv.data('action');
    top.Ts.Services.Tickets.SetActionKb(action.ActionID, !$(this).hasClass('ts-icon-kb'),
    function (result) {
      if (result == true) {
        icon.removeClass('ts-icon-kbnot').addClass('ts-icon-kb');
      }
      else {
        icon.removeClass('ts-icon-kb').addClass('ts-icon-kbnot');
      }
    }, function () {
      alert('There was an error editing this action.');
    });

  });

  $('#divActions').delegate('.ticket-action-portal', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var icon = $(this);
    var actionDiv = $(this).parents('.ticket-action');
    var action = actionDiv.data('action');
    top.Ts.Services.Tickets.SetActionPortal(action.ActionID, !$(this).hasClass('ts-icon-portal'),
    function (result) {
      if (result == true) {
        icon.removeClass('ts-icon-portalnot').addClass('ts-icon-portal');
      }
      else {
        icon.removeClass('ts-icon-portal').addClass('ts-icon-portalnot');
      }
    }, function () {
      alert('There was an error editing this action.');
    });

  });

  $('#divActions').delegate('.ticket-action-expand', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var icon = $(this).find('.ticket-action-expand-icon');
    var actionDiv = $(this).parents('.ticket-action');
    var action = actionDiv.data('action');

    if (icon.hasClass('ui-icon-triangle-1-e')) {
      actionDiv.find('.ticket-action-body').show();
      icon.removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');
      if (action.Description == null) {
        top.Ts.Services.Tickets.GetActionInfo(action.ActionID, function (actionInfo) {
          loadActionDisplay(actionDiv, actionInfo, false);

        });

      }
    }
    else {
      actionDiv.find('.ticket-action-body').hide();
      icon.removeClass('ui-icon-triangle-1-s').addClass('ui-icon-triangle-1-e');
    }

  });


  $('#ticketName')
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
      e.preventDefault();
      var header = $(this).parent().hide();
      var container = $('<div>')
        .addClass('ticket-name-edit')
        .css('marginTop', '1em')
        .insertAfter(header);

      $('<input type="text">')
        .addClass('ui-widget-content ui-corner-all')
        .val($(this).text())
        .appendTo(container)
        .focus();

      $('<span>')
        .addClass('ts-icon ts-icon-save')
        .click(function (e) {
          $(this).closest('div').remove();
          header.show().find('img').show();
          top.Ts.Services.Tickets.SetTicketName(_ticketID, $(this).prev().val(), function (result) {
            header.show().find('img').hide().next().show().delay(800).fadeOut(400);
            $('#ticketName').html(result);
          },
          function (error) {
            header.show().find('img').hide();
            alert('There was an error saving the ticket name.');
          });
        })
        .appendTo(container)

      $('<span>')
        .addClass('ts-icon ts-icon-cancel')
        .click(function (e) {
          $(this).closest('div').remove();
          header.show();
        })
        .appendTo(container)
    });


  $('#isTicketPortal')
      .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
      .click(function (e) {
        e.preventDefault();
        var item = $(this);
        item.next().show();
        top.Ts.Services.Tickets.SetIsVisibleOnPortal(_ticketID, (item.text() !== 'Yes'),
          function (result) {
            item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
          },
          function (error) {
            alert('There was an error saving the ticket portal visible\'s status.');
            item.next().hide();
          });
      });
  $('#isTicketKB')
      .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
      .click(function (e) {
        e.preventDefault();
        var item = $(this);
        item.next().show();
        top.Ts.Services.Tickets.SetIsKB(_ticketID, (item.text() !== 'Yes'),
          function (result) {
            item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
          },
          function (error) {
            alert('There was an error saving the ticket knowlegdgebase\'s status.');
            item.next().hide();
          });
      });

  $('#ticketType')
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      removeComboBoxes();
      var parent = $(this).closest('.ticket-name-value').hide();
      var container = $('<div class="ticket-combobox">').insertAfter(parent);
      var select = $('<select>').appendTo(container);
      var types = top.Ts.Cache.getTicketTypes();
      for (var i = 0; i < types.length; i++) {
        var option = $('<option>').text(types[i].Name).appendTo(select).data('type', types[i]);
        if ($(this).text() === types[i].Name) {
          option.attr('selected', 'selected');
        }
      }

      select.combobox({
        selected: function (e, ui) {
          parent.show().find('img').show();
          var type = $(ui.item).data('type');
          top.Ts.Services.Tickets.SetTicketType(_ticketID, type.TicketTypeID, function (status) {
            if (status !== null) {
              $('#ticketType').text(type.Name);
              $('#ticketStatus').text(status.Name).data('ticketStatusID', status.TicketStatusID);
              parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
            }
            else {
              parent.show().find('img').hide();
            }
          },
          function (error) {
            parent.show().find('img').hide();
            alert('There was an error setting your ticket type.');
          });
          container.remove();
        },
        close: function (e, ui) {
          removeComboBoxes();
        }
      });
      select.combobox('search', '');
    });

  $('#ticketStatus')
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      removeComboBoxes();
      var id = $(this).data('ticketStatusID');
      var parent = $(this).closest('.ticket-name-value').hide();
      var container = $('<div class="ticket-combobox">').insertAfter(parent);
      var select = $('<select>').appendTo(container);
      var statuses = top.Ts.Cache.getNextStatuses(id);
      for (var i = 0; i < statuses.length; i++) {
        var option = $('<option>').text(statuses[i].Name).appendTo(select).data('status', statuses[i]);
        if ($(this).text() === statuses[i].Name) {
          option.attr('selected', 'selected');
        }
      }

      select.combobox({
        selected: function (e, ui) {
          parent.show().find('img').show();
          var status = $(ui.item).data('status');

          top.Ts.Services.Tickets.SetTicketStatus(_ticketID, status.TicketStatusID, function (result) {
            if (result !== null) {
              $('#ticketStatus').text(result.Name).data('ticketStatusID', result.TicketStatusID);
              parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
            }
            else {
              parent.show().find('img').hide();
            }
          },
          function (error) {
            parent.show().find('img').hide();
            alert('There was an error setting your ticket status.');
          });
          container.remove();
        },
        close: function (e, ui) {
          removeComboBoxes();
        }
      });
      select.combobox('search', '');
    });

  $('#ticketSeverity')
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      removeComboBoxes();
      var parent = $(this).closest('.ticket-name-value').hide();
      var container = $('<div class="ticket-combobox">').insertAfter(parent);
      var select = $('<select>').appendTo(container);
      var severities = top.Ts.Cache.getTicketSeverities();
      for (var i = 0; i < severities.length; i++) {
        var option = $('<option>').text(severities[i].Name).appendTo(select).data('severity', severities[i]);
        if ($(this).text() === severities[i].Name) {
          option.attr('selected', 'selected');
        }
      }

      select.combobox({
        selected: function (e, ui) {
          parent.show().find('img').show();
          var severity = $(ui.item).data('severity');
          top.Ts.Services.Tickets.SetTicketSeverity(_ticketID, severity.TicketSeverityID, function (result) {
            if (result !== null) {
              $('#ticketSeverity').text(result.Name);
              parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
            }
            else {
              parent.show().find('img').hide();
            }
          },
          function (error) {
            parent.show().find('img').hide();
            alert('There was an error setting your ticket severity.');
          });
          container.remove();
        },
        close: function (e, ui) {
          removeComboBoxes();
        }
      });
      select.combobox('search', '');
    });


  $('#userName')
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      removeComboBoxes();
      var parent = $(this).closest('.ticket-name-value').hide();
      var container = $('<div class="ticket-combobox">').insertAfter(parent);
      var select = $('<select>').appendTo(container);
      var users = top.Ts.Cache.getUsers();
      var unassigned = new Object();
      unassigned.UserID = null;
      unassigned.Name = "Unassigned";
      var option = $('<option>').text(unassigned.Name).appendTo(select).data('user', unassigned);
      if ($(this).text() === unassigned.Name) {
        option.attr('selected', 'selected');
      }

      for (var i = 0; i < users.length; i++) {
        var option = $('<option>').text(users[i].Name).appendTo(select).data('user', users[i]);
        if ($(this).text() === users[i].Name) {
          option.attr('selected', 'selected');
        }
      }

      select.combobox({
        selected: function (e, ui) {
          parent.show().find('img').show();
          var user = $(ui.item).data('user');
          top.Ts.Services.Tickets.SetTicketUser(_ticketID, user.UserID, function (result) {
            if (result !== null) {
              $('#userName').text(result == "" ? 'Unassigned' : result);
              parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
            }
            else {
              parent.show().find('img').hide();
            }
          },
          function (error) {
            parent.show().find('img').hide();
            alert('There was an error setting the user.');
          });
          container.remove();
        },
        close: function (e, ui) {
          removeComboBoxes();
        }
      });
      select.combobox('search', '');
    });

  $('#ticketGroup')
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      removeComboBoxes();
      var parent = $(this).closest('.ticket-name-value').hide();
      var container = $('<div class="ticket-combobox">').insertAfter(parent);
      var select = $('<select>').appendTo(container);
      var groups = top.Ts.Cache.getGroups();
      var unassigned = new Object();
      unassigned.GroupID = null;
      unassigned.Name = "Unassigned";
      var option = $('<option>').text(unassigned.Name).appendTo(select).data('group', unassigned);
      if ($(this).text() === unassigned.Name) {
        option.attr('selected', 'selected');
      }
      for (var i = 0; i < groups.length; i++) {
        var option = $('<option>').text(groups[i].Name).appendTo(select).data('group', groups[i]);
        if ($(this).text() === groups[i].Name) {
          option.attr('selected', 'selected');
        }
      }

      select.combobox({
        selected: function (e, ui) {
          parent.show().find('img').show();
          var group = $(ui.item).data('group');
          top.Ts.Services.Tickets.SetTicketGroup(_ticketID, group.GroupID, function (result) {
            if (result !== null) {
              $('#ticketGroup').text(result == "" ? 'Unassigned' : result);
              parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
            }
            else {
              parent.show().find('img').hide();
            }
          },
          function (error) {
            parent.show().find('img').hide();
            alert('There was an error setting the group.');
          });
          container.remove();
        },
        close: function (e, ui) {
          removeComboBoxes();
        }
      });
      select.combobox('search', '');
    });

  $('#product')
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      removeComboBoxes();
      var parent = $(this).closest('.ticket-name-value').hide();
      var container = $('<div class="ticket-combobox">').insertAfter(parent);
      var select = $('<select>').appendTo(container);
      var products = top.Ts.Cache.getProducts();
      var unassigned = new Object();
      unassigned.ProductID = null;
      unassigned.Name = "Unassigned";
      var option = $('<option>').text(unassigned.Name).appendTo(select).data('product', unassigned);
      if ($(this).text() === unassigned.Name) {
        option.attr('selected', 'selected');
      }
      for (var i = 0; i < products.length; i++) {
        var option = $('<option>').text(products[i].Name).appendTo(select).data('product', products[i]);
        if ($(this).text() === products[i].Name) {
          option.attr('selected', 'selected');
        }
      }

      select.combobox({
        selected: function (e, ui) {
          parent.show().find('img').show();
          var product = $(ui.item).data('product');
          top.Ts.Services.Tickets.SetProduct(_ticketID, product.ProductID, function (result) {
            if (result !== null) {
              setProduct(result.id, result.label);
              setVersion(null, null, true);
              setVersion(null, null, false);
              //$('#product').text(result[1] === '' ? 'Unassigned' : result[1]).data('productID', result[0]);
              //$('#resolved').text('Unassigned');
              //$('#reported').text('Unassigned');
              parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
            }
            else {
              parent.show().find('img').hide();
            }
          },
          function (error) {
            parent.show().find('img').hide();
            alert('There was an error setting the product.');
          });
          container.remove();
        },
        close: function (e, ui) {
          removeComboBoxes();
        }
      });
      select.combobox('search', '');
    });

  $('#reported')
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      removeComboBoxes();
      if ($('#product').data('productID') === null) return;
      var parent = $(this).closest('.ticket-name-value').hide();
      var container = $('<div class="ticket-combobox">').insertAfter(parent);
      var select = $('<select>').appendTo(container);
      var versions = top.Ts.Cache.getProduct($('#product').data('productID')).Versions;
      var unassigned = new Object();
      unassigned.ProductVersionID = null;
      unassigned.VersionNumber = "Unassigned";
      var option = $('<option>').text(unassigned.VersionNumber).appendTo(select).data('version', unassigned);
      if ($(this).text() === unassigned.VersionNumber) {
        option.attr('selected', 'selected');
      }
      for (var i = 0; i < versions.length; i++) {
        var option = $('<option>').text(versions[i].VersionNumber).appendTo(select).data('version', versions[i]);
        if ($(this).text() === versions[i].VersionNumber) {
          option.attr('selected', 'selected');
        }
      }

      select.combobox({
        selected: function (e, ui) {
          parent.show().find('img').show();
          var version = $(ui.item).data('version');
          top.Ts.Services.Tickets.SetReportedVersion(_ticketID, version.ProductVersionID, function (result) {
            if (result !== null) {
              setVersion(result.id, result.label, false);
              //$('#reported').text(result === '' ? 'Unassigned' : result);
              parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
            }
            else {
              parent.show().find('img').hide();
            }
          },
          function (error) {
            parent.show().find('img').hide();
            alert('There was an error setting the reported version.');
          });
          container.remove();
        },
        close: function (e, ui) {
          removeComboBoxes();
        }
      });
      select.combobox('search', '');
    });

  $('#resolved')
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      removeComboBoxes();
      if ($('#product').data('productID') === null) return;
      var parent = $(this).closest('.ticket-name-value').hide();
      var container = $('<div class="ticket-combobox">').insertAfter(parent);
      var select = $('<select>').appendTo(container);
      var versions = top.Ts.Cache.getProduct($('#product').data('productID')).Versions;
      var unassigned = new Object();
      unassigned.ProductVersionID = null;
      unassigned.VersionNumber = "Unassigned";
      var option = $('<option>').text(unassigned.VersionNumber).appendTo(select).data('version', unassigned);
      if ($(this).text() === unassigned.VersionNumber) {
        option.attr('selected', 'selected');
      }
      for (var i = 0; i < versions.length; i++) {
        var option = $('<option>').text(versions[i].VersionNumber).appendTo(select).data('version', versions[i]);
        if ($(this).text() === versions[i].VersionNumber) {
          option.attr('selected', 'selected');
        }
      }

      select.combobox({
        selected: function (e, ui) {
          parent.show().find('img').show();
          var version = $(ui.item).data('version');
          top.Ts.Services.Tickets.SetSolvedVersion(_ticketID, version.ProductVersionID, function (result) {
            if (result !== null) {
              setVersion(result.id, result.label, true);
              //$('#resolved').text(result === '' ? 'Unassigned' : result);
              parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
            }
            else {
              parent.show().find('img').hide();
            }
          },
          function (error) {
            parent.show().find('img').hide();
            alert('There was an error setting the resolved version.');
          });
          container.remove();
        },
        close: function (e, ui) {
          removeComboBoxes();
        }
      });
      select.combobox('search', '');
    });

  function removeComboBoxes() {
    $('.ticket-combobox').prev().show().next().remove();
  }

  function addToolbarButton(id, icon, caption, callback) {
    var html = '<a href="#" id="' + id + '" class="ts-toolbar-button ui-corner-all"><span class="ts-toolbar-icon ts-icon ' + icon + '"></span><span class="ts-toolbar-caption">' + caption + '</span></a>';
    $('.ticket-panel-toolbar').append(html).find('#' + id).click(callback).hover(function () { $(this).addClass('ui-state-hover'); }, function () { $(this).removeClass('ui-state-hover'); });
  }

  addToolbarButton('btnRefresh', 'ts-icon-refresh', 'Refresh', function (e) {
    e.preventDefault();
    e.stopPropagation();
    window.location = window.location;
  });
  addToolbarButton('btnDelete', 'ts-icon-delete', 'Delete', function (e) {
    e.preventDefault();
    e.stopPropagation();
    if (confirm('Are you sure you would like to delete this ticket?')) {
      top.Ts.Services.Tickets.DeleteTicket(_ticketID, function () {
        top.Ts.MainPage.closeTicketTab(_ticket.TicketNumber);
      }, function () {
        alert('There was an error deleting this ticket.');

      });
    }
  });
  addToolbarButton('btnOwn', 'ts-icon-takeownership', 'Take Ownership', function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.Services.Tickets.AssignUser(_ticketID, top.Ts.System.User.UserID, function (name) {
      if (name !== null) $('#userName').text((name !== "" ? name : "Unassigned"));
    }, function () {
      alert('There was an error taking ownwership of this ticket.');

    });
  });
  addToolbarButton('btnUpdate', 'ts-icon-request', 'Request Update', function (e) {
    e.preventDefault();
    e.stopPropagation();

    top.Ts.Services.Tickets.RequestUpdate(_ticketID, function () {
      alert('An update has been requested for this ticket.');
    }, function () {
      alert('There was an error requesting an update for this ticket.');
    });

  });
  addToolbarButton('btnSubscribe', 'ts-icon-subscribed', 'Subscribe', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var caption = $(this).find('.ts-toolbar-caption');
    if (caption.text() === 'Unsubscribe') {
      top.Ts.Services.Tickets.SetSubscribed(_ticketID, false, function () {
        caption.text('Subscribe');
      }, function () {
        alert('There was an error subscribing to this ticket.');
      });
    }
    else {
      top.Ts.Services.Tickets.SetSubscribed(_ticketID, true, function () {
        caption.text('Unsubscribe');
      }, function () {
        alert('There was an error unsubscribing to this ticket.');
      });
    }
  });

  addToolbarButton('btnEnqueue', 'ts-icon-enqueue', 'Enqueue', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var caption = $(this).find('.ts-toolbar-caption');
    if (caption.text() === 'Dequeue') {
      top.Ts.Services.Tickets.Dequeue(_ticketID, function () {
        caption.text('Enqueue');
      }, function () {
        alert('There was an error dequeueing this ticket.');
      });
    }
    else {
      top.Ts.Services.Tickets.Enqueue(_ticketID, function () {
        caption.text('Dequeue');
      }, function () {
        alert('There was an error enqueueing this ticket.');
      });
    }
  });

  addToolbarButton('btnFlag', 'ts-icon-flagged', 'Flag', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var caption = $(this).find('.ts-toolbar-caption');
    if (caption.text() === 'Unflag') {
      top.Ts.Services.Tickets.SetTicketFlag(_ticketID, false, function () {
        caption.text('Flag');
      }, function () {
        alert('There was an error flagging to this ticket.');
      });
    }
    else {
      top.Ts.Services.Tickets.SetTicketFlag(_ticketID, true, function () {
        caption.text('Unflag');
      }, function () {
        alert('There was an error unflagging to this ticket.');
      });
    }
  });

  addToolbarButton('btnPrint', 'ts-icon-print', 'Print', function (e) {
    e.preventDefault();
    e.stopPropagation();
    window.open('../../TicketPrint.aspx?ticketid=' + _ticketID, 'TSPrint' + _ticketID);
  });
  addToolbarButton('btnEmail', 'ts-icon-compose-email', 'Email', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $(".dialog-emailinput").dialog('open');

  });

  function ellipseString(text, max) { return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; };

  function getRemovableItem(data, itemClass, title, description) {
    var item = $('<div>')
    .addClass('ticket-removable-item ui-corner-all ts-color-bg-accent ' + itemClass)
    item.data('data', data);

    $('<span>').addClass('ui-icon ui-icon-close').appendTo(item);
    if (title) $('<div>').addClass('ticket-removable-item-title').text(ellipseString(title, 30)).appendTo(item);
    if (description) $('<div>').addClass('ticket-removable-item-description').text(ellipseString(description, 30)).appendTo(item);
    return item;
  }

  var _layout = null;

  loadTicket(top.Ts.Utils.getQueryValue('TicketNumber', window));
  function loadTicket(ticketNumber) {
    top.Ts.Services.Tickets.GetTicketInfo(ticketNumber, function (info) {
      _ticketID = info.Ticket.TicketID;
      _ticket = info.Ticket;

      $('.page-loading').hide().next().show();
      if (_layout === null) {
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
      }

      if (info == null) alert('no ticket');

      $('.ticket-sla-status').removeClass('ts-icon-sla-good ts-icon-sla-warning ts-icon-sla-bad')
      if (info.Ticket.SlaViolationTime === null) {
        $('.ticket-sla-status').addClass('ts-icon-sla-good');
        $('#ticketSla').text('None');
      }
      else {
        $('.ticket-sla-status').addClass((info.Ticket.SlaViolationTime < 1 ? 'ts-icon-sla-bad' : (info.Ticket.SlaWarningTime < 1 ? 'ts-icon-sla-warning' : 'ts-icon-sla-good')));
        $('#ticketSla').text(info.Ticket.SlaViolationDate.localeFormat(top.Ts.Utils.getDateTimePattern()));
      }

      $('.ticket-sla-info')
        .attr('rel', 'Tips/Sla.aspx?TicketID=' + _ticketID)
        .cluetip(clueTipOptions);

      $('#ticketNumber').text(info.Ticket.TicketNumber);
      $('#ticketName').html(info.Ticket.Name);
      $('#creator').text(info.Ticket.CreatorName);
      $('#dateCreated').text(info.Ticket.DateCreated.localeFormat(top.Ts.Utils.getDateTimePattern()));
      $('#isTicketPortal').text((info.Ticket.IsVisibleOnPortal == true ? 'Yes' : 'No'));
      $('#isTicketKB').text((info.Ticket.IsKnowledgebase == true ? 'Yes' : 'No'));
      $('#ticketType').html(info.Ticket.TicketTypeName);
      $('#ticketStatus').text(info.Ticket.Status).data('ticketStatusID', info.Ticket.TicketStatusID);
      $('#ticketSeverity').text(info.Ticket.Severity);
      $('#userName').text(info.Ticket.UserName == null ? 'Unassigned' : info.Ticket.UserName);
      $('#ticketGroup').text(info.Ticket.GroupName == null ? 'Unassigned' : info.Ticket.GroupName);
      setProduct(info.Ticket.ProductID, info.Ticket.ProductName);
      setVersion(info.Ticket.ReportedVersionID, info.Ticket.ReportedVersion, false);
      setVersion(info.Ticket.SolvedVersionID, info.Ticket.SolvedVersion, true);
      //$('#reported').text(info.Ticket.ReportedVersion == null ? 'Unassigned' : info.Ticket.ReportedVersion);
      //$('#resolved').text(info.Ticket.SolvedVersion == null ? 'Unassigned' : info.Ticket.SolvedVersion);
      if (info.Ticket.IsSubscribed === true) $('#btnSubscribe .ts-toolbar-caption').text('Unsubscribe');
      if (info.Ticket.IsEnqueued === true) $('#btnEnqueue .ts-toolbar-caption').text('Dequeue');
      if (info.Ticket.IsFlagged === true) $('#btnFlag .ts-toolbar-caption').text('Unflag');


      if (info.Ticket.IsClosed == true) {
        $('#daysOpened').text(info.Ticket.DaysClosed).prev().text('Days Closed:');
      }
      else {
        $('#daysOpened').text(info.Ticket.DaysOpened).prev().text('Days Opened:');
      }

      //if (info.Ticket.ModifierName) $('<div>').text('Last Modified By: ' + info.Ticket.ModifierName).appendTo(details);
      //if (info.Ticket.DateModified) $('<div>').text('Last Modified On: ' + info.Ticket.DateModified.localeFormat(top.Ts.Utils.getDateTimePattern())).appendTo(details);
      $('#timeSpent').text(top.Ts.Utils.getTimeSpentText(info.Ticket.HoursSpent));

      appendCustomers(info.Customers);
      appendTags(info.Tags);
      appendRelated(info.Related);

      $('#divActions').empty();

      for (var i = 0; i < info.Actions.length; i++) {
        appendActionElement(info.Actions[i]);
      }


      $('.ticket-content a').addClass('ui-state-default ts-link');
      $('.ticket-rail a').addClass('ui-state-default ts-link');
      $('.ts-icon-info').removeClass('ui-state-default ts-link');
    });
  }

  function setProduct(id, name) {
    $('#product').text((name == null || name == '') ? 'Unassigned' : name).data('productID', id);
    $('.ticket-product-info').hide();
    if (id !== null) {
      $('.ticket-product-info')
        .show()
        .attr('rel', 'Tips/Product.aspx?ProductID=' + id)
        .cluetip(clueTipOptions);
    }
  }

  function setVersion(id, name, isResolved) {
    if (isResolved === true) {
      $('#resolved').text(id == null ? 'Unassigned' : name);
      $('.ticket-resolved-info').hide();
      if (id !== null) {
        $('.ticket-resolved-info')
        .show()
        .attr('rel', 'Tips/Version.aspx?VersionID=' + id)
        .cluetip(clueTipOptions);
      }
    } else {
      $('.ticket-reported-info').hide();
      if (id !== null) {
        $('.ticket-reported-info')
        .show()
        .attr('rel', 'Tips/Version.aspx?VersionID=' + id)
        .cluetip(clueTipOptions);
      }
      $('#reported').text(id == null ? 'Unassigned' : name);
    }
  }


  $('button').button();





});


