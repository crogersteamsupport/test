var _ticketInfo = null;
var _ticketNumber = null;
var _ticketID = null;
var _ticketCreator = new Object();
var _ticketSender = null;

var _ticketGroupID = null;
var _ticketGroupUsers = null;
var _ticketTypeID = null;
var _parentFields = [];
var _productFamilyID = null;

var _isNewActionPrivate = true;

var _timeLine = new Object();
var _currDateSpan = null;
var _compiledActionTemplate = null;
var _actionTotal = 0;
var _workingActionNumer = 0;
var _isLoading = false;
var dateformat;

var _timerid;
var _timerElapsed = 0;
var speed = 50, counter = 0, start;

var userFullName = top.Ts.System.User.FirstName + " " + top.Ts.System.User.LastName;

var clueTipOptions = top.Ts.Utils.getClueTipOptions(null);

var _doClose = false;
var canEdit = top.Ts.System.User.IsSystemAdmin || top.Ts.System.User.ChangeKbVisibility;
var alertMessage = null;
var dateFormat;
top.Ts.System.logAction('New Ticket - Started');

var execGetCustomer = null;
var execGetTags = null;
var execGetAsset = null;
var execGetUsers = null;
var execGetRelated = null;
var execSelectTicket = null;

var getCustomers = function (request, response) {
  if (execGetCustomer) { execGetCustomer._executor.abort(); }
  execGetCustomer = top.Ts.Services.TicketPage.GetUserOrOrganizationForTicket(request, function (result) { response(result); });
}

var getTags = function (request, response) {
  if (execGetTags) { execGetTags._executor.abort(); }
  execGetTags = top.Ts.Services.Tickets.SearchTags(request.term, function (result) { response(result); });
}

var getAssets = function (request, response) {
  if (execGetAsset) { execGetAsset._executor.abort(); }
  execGetAsset = top.Ts.Services.Assets.FindAsset(request, function (result) { response(result); });
}

var getUsers = function (request, response) {
  if (execGetUsers) { execGetUsers._executor.abort(); }
  execGetUsers = top.Ts.Services.TicketPage.SearchUsers(request, function (result) { response(result); });
}

var getRelated = function (request, response) {
  if (execGetRelated) { execGetRelated._executor.abort(); }
  execGetRelated = top.Ts.Services.Tickets.SearchTickets(request, null, function (result) { response(result); });
}

var selectTicket = function (request, response) {
  if (execSelectTicket) { execSelectTicket._executor.abort(); }
  var filter = $(this.element).data('filter');
  if (filter === undefined) {
    execSelectTicket = top.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) { response(result); });
  }
  else {
    execSelectTicket = top.Ts.Services.Tickets.SearchTickets(request.term, filter, function (result) { response(result); });
  }
}

var ellipseString = function (text, max) { return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; };

//var isFormValidToClose = function (isClosed, callback) {
//    var result = true;
//    if (isClosed) {
//      $('.isRequiredToClose.isEmpty').addClass('hasCloseError');
//        if ($('.hasCloseError').length > 0) {
//            result = false;
//        }
//    }
//    callback(result);
//}

Selectize.define('sticky_placeholder', function (options) {
  var self = this;

  self.updatePlaceholder = (function () {
    var original = self.updatePlaceholder;
    return function () {
      original.apply(this, arguments);
      if (!this.settings.placeholder) return;
      var $input = this.$control_input;
      $input.attr('placeholder', this.settings.placeholder);
    };
  })();

});

$(document).ready(function () {
  _ticketNumber = top.Ts.Utils.getQueryValue("TicketNumber", window);

  //Setup Ticket Elements
  SetupTicketPage();

  $('.scrollup').click(function () {
    $('.frame-container').animate({
      scrollTop: 0
    }, 600);
    return false;
  });
});

function SetupTicketPage() {
  //Create the new action LI element
  CreateNewActionLI();

  top.Ts.Services.TicketPage.GetTicketPageOrder("TicketFieldsOrder", function (order) {
    jQuery.each(order, function (i, val) { if (val.Disabled == "false") AddTicketProperty(val); });
    SetupTicketProperties();
  });

  top.Ts.Services.Customers.GetDateFormat(true, function (format) {
    dateFormat = format.replace("yyyy", "yy");
    if (dateFormat.length < 8) {
      var dateArr = dateFormat.split('/');
      if (dateArr[0].length < 2) {
        dateArr[0] = dateArr[0] + dateArr[0];
      }
      if (dateArr[1].length < 2) {
        dateArr[1] = dateArr[1] + dateArr[1];
      }
      if (dateArr[2].length < 2) {
        dateArr[1] = dateArr[1] + dateArr[1];
      }
      dateFormat = dateArr[0] + "/" + dateArr[1] + "/" + dateArr[2];
    }
  });
};

function AddTicketProperty(item) {
  if ($("#ticket-group-" + item.CatID).length > 0) {
    var compiledTemplate = Handlebars.compile($("#ticket-group-" + item.CatID).html());
    $('#ticket-properties-area').append(compiledTemplate);
  }
};

function SetupTicketProperties() {

    //action timers
    SetupActionTimers();

    //update ticket property controls with the values needed
    LoadTicketControls();

    $('.page-loading').hide().next().show();
};

function CreateNewActionLI() {
  var _compiledNewActionTemplate = Handlebars.compile($("#new-action-template").html());
  var html = _compiledNewActionTemplate({ OrganizationID: top.Ts.System.User.OrganizationID, UserID: top.Ts.System.User.UserID });
  $("#action-timeline").append(html);

  $('#action-new-save').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    var self = $(this);
    var oldActionID = self.data('actionid');
    SaveAction(oldActionID, _isNewActionPrivate, function (result) {
      UploadAttachments(result);
      $('#action-new-editor').parent().fadeOut('normal', function () {
        tinymce.activeEditor.destroy();
      });
      top.Ts.Services.TicketPage.GetActionAttachments(result.item.RefID, function (attachments) {
        result.Attachments = attachments;
        if (oldActionID === -1) {
          _actionTotal = _actionTotal + 1;
          var actionElement = CreateActionElement(result, false);
          actionElement.find('.ticket-action-number').text(_actionTotal);
        }
        else {
          UpdateActionElement(result, false);
        }
      });
    });
  });

  $('#action-new-type').change(function (e) {
    var action = $(this).val();
    top.Ts.Services.TicketPage.GetActionTicketTemplate(action, function (result) {
      if (result != null && result != "" && result != "<br>") {
        var currenttext = tinyMCE.activeEditor.getContent();
        tinyMCE.activeEditor.setContent(currenttext + result);
      }
      elem.parent().fadeIn('normal');
    });
  });
};

function SetupActionEditor(elem, action) {
  top.Ts.MainPage.highlightTicketTab(_ticketNumber, true);
  initEditor(elem, true, function (ed) {
    $("#action-new-type").val($("#action-new-type option:first").val());


    $('#action-new-editor').val('');
    if (action) {
      $('#action-new-type').val(action.ActionTypeID);
      if (action.TimeSpent) {
        $('#action-new-hours').val(Math.floor(action.TimeSpent / 60));
        $('#action-new-minutes').val(Math.floor(action.TimeSpent % 60));
      }
      tinyMCE.activeEditor.setContent(action.Message);
      //elem.parent().fadeIn('normal');
    }
    else {
      var actionTypeID = $('#action-new-type').val();
      $('#action-new-hours').val(0);
      $('#action-new-minutes').val(0);
      top.Ts.Services.TicketPage.GetActionTicketTemplate(actionTypeID, function (result) {
        if (result != null && result != "" && result != "<br>") {
          var currenttext = tinyMCE.activeEditor.getContent();
          tinyMCE.activeEditor.setContent(currenttext + result);
        }
      });

      top.Ts.Services.Tickets.GetTicketTypeTemplateText(_ticketTypeID, function (result) {
        if (result != null && result != "" && result != "<br>") {
          var currenttext = tinyMCE.activeEditor.getContent();
          tinyMCE.activeEditor.setContent(currenttext + result);
        }
      });
    }
    elem.parent().fadeIn('normal');

    $('.frame-container').animate({
      scrollTop: 0
    }, 600);
  });

  var element = $('.action-new-area');
  $('.file-upload').fileupload({
    namespace: 'new_action',
    dropZone: element,
    add: function (e, data) {
      for (var i = 0; i < data.files.length; i++) {
        var item = $('<li>')
        .appendTo(element.find('.upload-queue'));

        data.context = item;
        item.data('data', data);

        var bg = $('<div>')
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

        //<span class="tagRemove" aria-hidden="true">×</span>

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
      callback(null);
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

    }
  });

  $('#action-new-saveoptions').empty();
  if (action) {
    $('#action-new-save').text('Update').data('actionid', action.RefID);
    for (var i = 0; i < statuses.length; i++) {
      $('#action-new-saveoptions').append('<li><a class="action-create-option" data-actionid=' + action.RefID + ' data-statusid=' + statuses[i].TicketStatusID + ' href="#">Create and Set Status to ' + statuses[i].Name + '</a></li>');
    }
  }
  else {
    $('#action-new-save').text('Save').data('actionid', -1);
    for (var i = 0; i < statuses.length; i++) {
      $('#action-new-saveoptions').append('<li><a class="action-create-option" data-actionid=-1 data-statusid=' + statuses[i].TicketStatusID + ' href="#">Save and Set Status to ' + statuses[i].Name + '</a></li>');
    }
  }
};

function SetupActionTimers() {
  $('#action-new-date-started').datetimepicker({ useCurrent: true, format: 'MM/DD/YYYY hh:mm A', defaultDate: new Date() });

  $('.spinner .btn:first-of-type').click(function () {
    var spinner = $(this).parent().prev();
    spinner.val(parseInt(spinner.val(), 10) + 1);
  });

  $('.spinner .btn:last-of-type').click(function () {
    var spinner = $(this).parent().prev();
    spinner.val(parseInt(spinner.val(), 10) - 1);
  });
  $('#action-new-timer').click(function (e) {
    var hasStarted = $(this).data('hasstarted');

    if (!hasStarted) {
      start = new Date().getTime();
      tickettimer();
      $(this).find(':first-child').css('color', 'green');
    }
    else {
      $(this).find(':first-child').css('color', 'red');
      clearTimeout(_timerid);
    }
    $(this).data('hasstarted', !hasStarted);
  });
}

function SetupActionTypeSelect() {
  var selectType = $('#action-new-type');
  selectType.empty();
  var types = top.Ts.Cache.getActionTypes();
  for (var i = 0; i < types.length; i++) {
    $('<option>').attr('value', types[i].ActionTypeID).text(types[i].Name).data('data', types[i]).appendTo(selectType);
  }
};

function SaveAction(oldActionID, isPrivate, callback) {
  var action = new top.TeamSupport.Data.ActionProxy();

  action.ActionID = oldActionID;
  action.TicketID = _ticketID;

  var actionType = $('#action-new-type option:selected').data('data');
  action.ActionTypeID = actionType.ActionTypeID

  action.SystemActionTypeID = 0;

  var timeSpent = parseInt($('#action-new-hours').val()) * 60 + parseInt($('#action-new-minutes').val());

  if (timeSpent < 1 && actionType.IsTimed == true && top.Ts.System.Organization.TimedActionsRequired == true) {
    $('#action-save-alert').text('Please enter the time you worked on this action.').show();
    return false;
  }

  action.TimeSpent = timeSpent || 0;
  action.DateStarted = top.Ts.Utils.getMsDate($('#action-new-date-started').val());
  action.IsKnowledgeBase = false;
  action.IsVisibleOnPortal = !isPrivate;

  action.Description = tinymce.activeEditor.getContent();

  if (action.IsVisibleOnPortal == true) confirmVisibleToCustomers();
  top.Ts.Services.TicketPage.UpdateAction(action, function (result) {
    top.Ts.MainPage.highlightTicketTab(_ticketNumber, false);
    callback(result)
  }, function (error) {
    callback(null);
  });
}

var confirmVisibleToCustomers = function () {
  var visible = $('#ticket-visible').is(":checked");
  if (!visible) {
    if (confirm('This ticket is not visible to customers.\n\nWould you like to make it visible to customers now?') == true) {
      $('#ticket-visible').click();
    }
  }
}

function UploadAttachments(newAction) {
  if ($('.upload-queue li').length > 0 && newAction !== null) {
    $('.upload-queue li').each(function (i, o) {
      var data = $(o).data('data');
      data.url = '../../../Upload/Actions/' + newAction.item.RefID;
      data.jqXHR = data.submit();
      $(o).data('data', data);
    });
  }
  $('.upload-queue').empty();
}

function tickettimer() {
  var real = (counter * speed),
  ideal = (new Date().getTime() - start);

  counter++;

  var diff = (ideal - real);

  if (_timerElapsed != Math.floor(ideal / 60000)) {
    var oldVal = parseInt($('#action-new-minutes').val()) || 0;
    $('#action-new-minutes').val(oldVal + 1);
    _timerElapsed = Math.floor(ideal / 60000);
  }
  _timerid = setTimeout(tickettimer, (speed - diff));
};

function SetupAssignedField() {
  //TODO:  Need ability to recreate selectize field
  top.Ts.Services.TicketPage.GetTicketUsers(_ticketID, function (users) {
    for (var i = 0; i < users.length; i++) {
      AppendSelect('#ticket-assigned', users[i], 'assigned', users[i].ID, users[i].Name, users[i].IsSelected);
    }

    $('#ticket-assigned').selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      }
    });
  });
}

function LoadTicketControls() {
  SetupAssignedField();

  top.Ts.Services.TicketPage.GetTicketGroups(_ticketID, function (groups) {
    for (var i = 0; i < groups.length; i++) {
      AppendSelect('#ticket-group', groups[i], 'group', groups[i].ID, groups[i].Name, groups[i].IsSelected);
    }
    $('#ticket-group').selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      }
    });
  });

  var types = top.Ts.Cache.getTicketTypes();
  for (var i = 0; i < types.length; i++) {
    AppendSelect('#ticket-type', types[i], 'type', types[i].TicketTypeID, types[i].Name, false);
  }

  //SetupStatusField(_ticketInfo.Ticket.TicketStatusID);

  var severities = top.Ts.Cache.getTicketSeverities();
  for (var i = 0; i < severities.length; i++) {
    AppendSelect('#ticket-severity', severities[i], 'severity', severities[i].TicketSeverityID, severities[i].Name, false);
  }

  if (top.Ts.System.User.ChangeKbVisibility || top.Ts.System.User.IsSystemAdmin) {

    var categories = top.Ts.Cache.getKnowledgeBaseCategories();
    for (var i = 0; i < categories.length; i++) {
      var cat = categories[i].Category;
      AppendSelect('#ticket-KB-Category', cat, 'category', cat.CategoryID, cat.CategoryName, false);;

      for (var j = 0; j < categories[i].Subcategories.length; j++) {
        var subcat = categories[i].Subcategories[j];
        AppendSelect('#ticket-KB-Category', subcat, 'subcategory', subcat.CategoryID, cat.CategoryName + ' -> ' + subcat.CategoryName, false);
      }
    }

    $('#ticket-KB-Category').selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      }
    });
  }
  else {
    $('#ticket-KBInfo').remove();
    $('#ticket-isKB-RO').text('Visible');
    $('#ticket-KBVisible-RO').show();
    $('#ticket-KBCat-RO').show();
  }

  $('#ticket-DaysOpened').text('0');

  SetupDueDateField(undefined);

  if (top.Ts.System.Organization.UseForums == true) {
    if (top.Ts.System.User.CanChangeCommunityVisibility) {
      var forumCategories = top.Ts.Cache.getForumCategories();
      for (var i = 0; i < forumCategories.length; i++) {
        var cat = forumCategories[i].Category;
        AppendSelect('#ticket-Community', cat, 'community', cat.CategoryID, cat.CategoryName, false);

        for (var j = 0; j < forumCategories[i].Subcategories.length; j++) {
          var subcat = forumCategories[i].Subcategories[j];
          AppendSelect('#ticket-Community', subcat, 'subcategory', subcat.CategoryID, cat.CategoryName + ' -> ' + subcat.CategoryName, false);
        }
      }

      $('#ticket-Community').selectize({
        onDropdownClose: function ($dropdown) {
          $($dropdown).prev().find('input').blur();
        }
      });
    }
    else {
      $('#ticket-CommunityInfo-RO').show();
      $('#ticket-Community-RO').text('Unassigned');
      $('#ticket-Community-RO').show();
      $('#ticket-Community').closest('.form-horizontal').remove();
    }
  }
  else {
    $('#ticket-Community').closest('.form-horizontal').remove();
  }

  $('.ticket-select').selectize({
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });

  setSLAInfo();

  //SetupTicketPropertyEvents();
  SetupCustomerSection();
  SetupTagsSection();
  SetupProductSection();

  if (top.Ts.System.Organization.IsInventoryEnabled === true) {
    SetupInventorySection();
  }
  else {
    $('#ticket-group-assets').hide();
  }

  SetupUserQueuesSection();
  SetupSubscribedUsersSection();
  SetupAssociatedTicketsSection();
  SetupRemindersSection();
  SetupCustomFieldsSection();
};

function AppendSelect(parent, data, type, id, name, isSelected) {
  var option = $('<option>').val(id).text(name).appendTo(parent).data(type, data);
  if (isSelected) {
    option.attr('selected', 'selected');
  }
};

//function SetupTicketPropertyEvents() {
//  $('#ticket-title-label').click(function (e) {
//    e.preventDefault();
//    e.stopPropagation();
//    var self = $(this);
//    self.hide();
//    var parent = self.parent();
//    var input = $('#ticket-title-input');

//    var titleInputContainer = $('#ticket-title-input-panel').show();
//    $('#ticket-title-input').val(_ticketInfo.Ticket.Name).focus().select();

//    $('#ticket-title-save').click(function (e) {
//      e.preventDefault();
//      e.stopPropagation();
//      titleInputContainer.hide();
//      top.Ts.Services.Tickets.SetTicketName(_ticketID, input.val(), function (result) {
//        _ticketInfo.Ticket.Name = result;
//        top.Ts.System.logAction('Ticket - Renamed');
//        self.text($.trim(_ticketInfo.Ticket.Name) === '' ? _ticketInfo.Ticket.TicketNumber + ': ' + '[Untitled Ticket]' : _ticketInfo.Ticket.TicketNumber + ': ' + $.trim(_ticketInfo.Ticket.Name)).show();


//        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeticketname", userFullName);
//      },
//      function (error) {
//        alert('There was an error saving the ticket name.');
//      });
//    });
//  });

//  $('#ticket-assigned').change(function (e) {
//    var self = $(this);
//    var value = self.val();
//    top.Ts.Services.Tickets.SetTicketUser(_ticketID, value, function (userInfo) {
//      var selectize = self[0].selectize;

//      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeassigned", userFullName);
//    },
//      function (error) {
//        alert('There was an error setting the user.');
//      });
//  });

//  $('#ticket-group').change(function (e) {
//    var self = $(this);
//    var GroupID = self.val();
//    top.Ts.Services.Tickets.SetTicketGroup(_ticketID, GroupID, function (result) {
//      if (result !== null) {
//        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changegroup", userFullName);
//        $('#ticket-group').focusout();
//        if (GroupID != null) {
//          top.Ts.Services.Users.GetGroupUsers(GroupID, function (result) {
//            _ticketGroupUsers = result;
//          });
//        }
//        else {
//          _ticketGroupUsers = null;
//        }
//      }

//      if (top.Ts.System.Organization.UpdateTicketChildrenGroupWithParent) {
//        top.Ts.Services.Tickets.SetTicketChildrenGroup(_ticketID, GroupID);
//      }
//    },
//    function (error) {
//      alert('There was an error setting the group.');
//    });
//  });

//  $('#ticket-type').change(function (e) {
//    var self = $(this);
//    var value = self.val();
//    top.Ts.Services.TicketPage.SetTicketType(_ticketID, value, function (result) {
//      if (result !== null) {
//        _ticketTypeID = value;
//        SetupStatusField(result[0].TicketStatusID);

//        $('#ticket-status-label').toggleClass('ticket-closed', result[0].IsClosed);

//        AppenCustomValues(result[1]);

//        _ticketInfo.Ticket = result[2];
//        setSLAInfo();

//        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changetype", userFullName);
//      }
//    },
//    function (error) {
//      alert('There was an error setting your ticket type.');
//    });
//  });

//  $('#ticket-severity').change(function (e) {
//    var self = $(this);
//    var value = self.val();
//    top.Ts.Services.Tickets.SetTicketSeverity(_ticketID, value, function (result) {
//      if (result !== null) {
//        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeseverity", userFullName);
//      }
//    },
//    function (error) {
//      alert('There was an error setting your ticket severity.');
//    });
//  });

//  $('#ticket-visible').change(function (e) {
//    var self = $(this);

//    if (top.Ts.System.User.ChangeTicketVisibility || top.Ts.System.User.IsSystemAdmin) {
//      var value = self.is(":checked");
//      top.Ts.System.logAction('Ticket - Visibility Changed');
//      top.Ts.Services.Tickets.SetIsVisibleOnPortal(_ticketID, value, function (result) {
//        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeisportal", userFullName);
//      },
//      function (error) {
//        alert('There was an error saving the ticket portal visible\'s status.');
//      });
//    }
//  });

//  $('#ticket-isKB').change(function (e) {
//    var self = $(this);

//    if (top.Ts.System.User.ChangeTicketVisibility || top.Ts.System.User.IsSystemAdmin) {
//      var value = self.is(":checked");
//      top.Ts.System.logAction('Ticket - Visibility Changed');
//      top.Ts.Services.Tickets.SetIsVisibleOnPortal(_ticketID, value, function (result) {
//        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeisportal", userFullName);
//      },
//      function (error) {
//        alert('There was an error saving the ticket portal visible\'s status.');
//      });
//    }

//    if (top.Ts.System.User.ChangeKbVisibility || top.Ts.System.User.IsSystemAdmin) {
//      var value = self.is(":checked");
//      top.Ts.System.logAction('Ticket - KB Status Changed');
//      top.Ts.Services.Tickets.SetIsKB(_ticketID, value,
//      function (result) {
//        if (result === true) {
//          $('#ticket-group-KBCat').show();
//        }
//        else {
//          $('#ticket-group-KBCat').hide();
//        }
//        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeiskb", userFullName);
//      },
//      function (error) {
//        alert('There was an error saving the ticket knowlegdgebase\'s status.');
//      });
//    }
//  });

//  $('#ticket-KB-Category').change(function (e) {
//    var self = $(this);
//    var value = self.val();
//    top.Ts.System.logAction('Ticket - KnowledgeBase Community Changed');
//    top.Ts.Services.Tickets.SetTicketKnowledgeBaseCategory(_ticketID, value, function (result) {
//      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changekbcat", userFullName);
//    },
//    function (error) {
//      alert('There was an error setting your ticket knowledgebase category.');
//    });
//  });

//  $('#ticket-Community').change(function (e) {
//    var self = $(this);
//    var value = self.val();
//    var oldCatName = _ticketInfo.Ticket.CategoryName;
//    var newCatName = self.text();
//    top.Ts.System.logAction('Ticket - Community Changed');
//    top.Ts.Services.Tickets.SetTicketCommunity(_ticketID, value, oldCatName == null ? 'Unassigned' : oldCatName, newCatName == null ? 'Unassigned' : newCatName, function (result) {
//      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecommunity", userFullName);
//    },
//    function (error) {
//      alert('There was an error setting your ticket community.');
//    });
//  });
//}

function SetupCustomerSection() {
  AddCustomers(_ticketInfo.Customers);

  $('#ticket-Customers-Input').selectize({
    valueField: 'id',
    labelField: 'label',
    searchField: 'label',
    load: function (query, callback) {
      getCustomers(query, callback)
    },
    initData: true,
    preload: true,
    onLoad: function () {
      if (this.settings.initData === true) {
        this.settings.initData = false;
      }
    },
    create: function (input, callback) {
      $('#NewCustomerModal').modal('show');
      callback(null);
    },
    onItemAdd: function (value, $item) {
      if (this.settings.initData === false) {
        var customerData = $item.data();

        top.Ts.Services.Tickets.AddTicketCustomer(_ticketID, customerData.type, value, function (customers) {
          AddCustomers(customers);

          if (customerData.type == "u") {
            top.Ts.Services.Customers.LoadAlert(value, top.Ts.ReferenceTypes.Users, function (note) {
              LoadTicketNotes(note);
            });
          }
          else {
            top.Ts.Services.Customers.LoadAlert(value, top.Ts.ReferenceTypes.Organizations, function (note) {
              LoadTicketNotes(note);
            });
          }

          window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addcustomer", userFullName);
        }, function () {
          $(this).parent().remove();
          alert('There was an error adding the customer.');
        });
        this.removeItem(value, true);
        top.Ts.System.logAction('Ticket - Customer Added');
      }
    },
    plugins: {
      'sticky_placeholder': {}
    },
    render: {
      item: function (item, escape) {
        return '<div data-value="' + item.value + '" data-type="' + item.data + '" data-selectable="" class="option">' + item.label + '</div>';
      },
      option: function (item, escape) {
        return '<div data-value="' + escape(item.value) + '" data-type="' + escape(item.data) + '" data-selectable="" class="option">' + item.label + '</div>';
      }
    },
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });

  $('#Customer-Create').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('Ticket - New Customer Added');
    var email = $('#customer-email-input').val();
    var firstName = $('#customer-fname-input').val();
    var lastName = $('#customer-lname-input').val();
    var phone = $('#customer-phone-input').val();;
    var companyName = $('#customer-company-input').val();
    top.Ts.Services.Users.CreateNewContact(email, firstName, lastName, companyName, phone, false, function (result) {
      if (result.indexOf("u") == 0 || result.indexOf("o") == 0) {
        top.Ts.Services.Tickets.AddTicketCustomer(_ticketID, result.charAt(0), result.substring(1), function (result) {
          AddCustomers(result);
          $('.ticket-new-customer-email').val('');
          $('.ticket-new-customer-first').val('');
          $('.ticket-new-customer-last').val('');
          $('.ticket-new-customer-company').val('');
          $('.ticket-new-customer-phone').val('');
          $('#NewCustomerModal').modal('hide');
        });
      }
      else if (result.indexOf("The company you have specified is invalid") !== -1) {
        if (top.Ts.System.User.CanCreateCompany || top.Ts.System.User.IsSystemAdmin) {
          if (confirm('Unknown company, would you like to create it?')) {
            top.Ts.Services.Users.CreateNewContact(email, firstName, lastName, companyName, phone, true, function (result) {
              top.Ts.Services.Tickets.AddTicketCustomer(_ticketID, result.charAt(0), result.substring(1), function (result) {
                AddCustomers(result);
                $('.ticket-new-customer-email').val('');
                $('.ticket-new-customer-first').val('');
                $('.ticket-new-customer-last').val('');
                $('.ticket-new-customer-company').val('');
                $('.ticket-new-customer-phone').val('');
                $('#NewCustomerModal').modal('hide');
              });
            });
          }
        }
        else {
          alert("We're sorry, but you do not have the rights to create a new company.");
          $('.ticket-new-customer-email').val('');
          $('.ticket-new-customer-first').val('');
          $('.ticket-new-customer-last').val('');
          $('.ticket-new-customer-company').val('');
          $('.ticket-new-customer-phone').val('');
          $('#NewCustomerModal').modal('hide');
        }
      }
      else {
        alert(result);
      }
    });
  });

  $('#ticket-Customer').on('click', 'span.tagRemove', function (e) {
    var self = $(this);
    var data = self.parent().data().tag;

    if (data.UserID) {
      top.Ts.Services.Tickets.RemoveTicketContact(_ticketID, data.UserID, function (customers) {
        AddCustomers(customers);
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removecontact", userFullName);
      }, function () {
        alert('There was a problem removing the contact from the ticket.');
      });
      top.Ts.System.logAction('Ticket - Contact Removed');
    }
    else {
      top.Ts.Services.Tickets.RemoveTicketCompany(_ticketID, data.OrganizationID, function (customers) {
        AddCustomers(customers);
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removecompany", userFullName);
      }, function () {
        alert('There was a problem removing the company from the ticket.');
      });
      top.Ts.System.logAction('Ticket - Customer Removed');
    }
  });
};

function AddCustomers(customers) {
  var customerDiv = $("#ticket-Customer");
  customerDiv.empty();
  $("#ticket-Customers-Input").val('');
  for (var i = 0; i < customers.length; i++) {
    var label = "";

    if (customers[i].Contact !== null && customers[i].Company !== null) {
      label = customers[i].Contact + '<br/>' + customers[i].Company;
    }
    else if (customers[i].Contact !== null) {
      label = customers[i].Contact;
    }
    else if (customers[i].Company !== null) {
      label = customers[i].Company;
    }

    var cssClasses = "tag-item";

    if (customers[i].Flag) {
      cssClasses = cssClasses + " tag-error"
    }

    if (customers[i].UserID !== null) {
      cssClasses = cssClasses + ' UserAnchor';
      var newelement = PrependTag(customerDiv, customers[i].UserID, label, customers[i], cssClasses);
      newelement.data('userid', customers[i].UserID).data('placement', 'left').data('ticketid', _ticketID);
    }
    else {
      cssClasses = cssClasses + ' OrgAnchor';
      var newelement = PrependTag(customerDiv, customers[i].OrganizationID, label, customers[i], cssClasses);
      newelement.data('orgid', customers[i].OrganizationID).data('placement', 'left').data('ticketid', _ticketID);
    }
  };
}

function SetupTagsSection() {
  //AddTags(_ticketInfo.Tags);

  $("#ticket-tag-Input").autocomplete({
    minLength: 2,
    source: getTags,
    response: function (event, ui) {
      var inputValue = $(this).val();

      var filtered = $(ui.content).filter(function () {
        return this.value == inputValue;
      });

      if (filtered.length === 0) {
        ui.content.push({
          label: inputValue,
          value: inputValue,
          id: 0
        });
      }
    },
    select: function (event, ui) {
      $(this)
  .data('item', ui.item)

      top.Ts.Services.Tickets.AddTag(_ticketID, ui.item.value, function (tags) {
        if (tags !== null) {
          AddTags(tags);
          //window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addtag", userFullName);
        }

      }, function () {
        alert('There was an error adding the tag.');
      });
      top.Ts.System.logAction('Ticket - Added');
    }
  })
  .data("autocomplete")._renderItem = function (ul, item) {
    return $("<li>")
        .append("<a>" + item.label + "</a>")
        .appendTo(ul);
  };

  $('#ticket-tags').on('click', 'span.tagRemove', function (e) {
    var tag = $(this).parent()[0];
    if (tag) {
      top.Ts.Services.Tickets.RemoveTag(_ticketID, tag.id, function (tags) {
        tag.remove();
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removetag", userFullName);
      }, function () {
        alert('There was a problem removing the tag from the ticket.');
      });
    }
    else {
      alert('There was a problem removing the reminder from the ticket.');
    }

  });

};

function AddTags(tags) {
  var tagDiv = $("#ticket-tags");
  tagDiv.empty();
  $("#ticket-tag-Input").val('');

  for (var i = 0; i < tags.length; i++) {
    var label = tags[i].Value
    PrependTag(tagDiv, tags[i].TagID, label, tags[i]);
  };
}

function PrependTag(parent, id, value, data, cssclass) {
  if (cssclass === undefined) cssclass = 'tag-item';
  var _compiledTagTemplate = Handlebars.compile($("#ticket-tag").html());
  var tagHTML = _compiledTagTemplate({ id: id, value: value, data: data, css: cssclass });
  return $(tagHTML).prependTo(parent);
}

function SetupProductSection() {
  top.Ts.Settings.Organization.read('ShowOnlyCustomerProducts', false, function (showOnlyCustomers) {
    if (showOnlyCustomers == "True") {
      top.Ts.Services.TicketPage.GetTicketCustomerProducts(_ticketID, function (CustomerProducts) {
        LoadProductList(CustomerProducts);
      });
    }
    else {
      var products = top.Ts.Cache.getProducts();
      LoadProductList(products);
    }

    //var product = top.Ts.Cache.getProduct(_ticketInfo.Ticket.ProductID);
    //SetupProductVersionsControl(product);
    //SetProductVersionAndResolved(_ticketInfo.Ticket.ReportedVersionID, _ticketInfo.Ticket.SolvedVersionID);

    top.Ts.Services.Organizations.IsProductRequired(function (result) {
      if (result && _ticketInfo.Ticket.ProductID == null)
        $('#ticket-Product').closest('.form-group').addClass('hasError');
      else
        $('#ticket-Product').closest('.form-group').removeClass('hasError');
    });

    $('#ticket-Product').change(function (e) {
      var self = $(this);
      top.Ts.Services.Tickets.SetProduct(_ticketID, self.val(), function (result) {
        if (result !== null) {
          var name = result.label;
          _productFamilyID = result.data;
          var product = top.Ts.Cache.getProduct(self.val());
          SetupProductVersionsControl(product);
          SetProductVersionAndResolved(null, null);
        }

        top.Ts.Services.Tickets.GetParentValues(_ticketID, function (fields) {
          AppenCustomValues(fields);
        });

        top.Ts.Services.Organizations.IsProductRequired(function (IsRequired) {
          if (IsRequired && (name == null || name == ''))
            $('#ticket-Product').closest('.form-group').addClass('hasError');
          else
            $('#ticket-Product').closest('.form-group').removeClass('hasError');
        });
      },
      function (error) {
        alert('There was an error setting the product.');
      });
    });

    $('#ticket-Versions').change(function (e) {
      top.Ts.System.logAction('Ticket - Reported Version Changed');
      top.Ts.Services.Tickets.SetReportedVersion(_ticketID, $(this).val(), function (result) {
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changereported", userFullName);
      },
      function (error) {
        alert('There was an error setting the reported version.');
      });
    });

    $('#ticket-Resolved').change(function (e) {
      top.Ts.System.logAction('Ticket - Resolved Version Changed');
      top.Ts.Services.Tickets.SetSolvedVersion(_ticketID, $(this).val(), function (result) {
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeresolved", userFullName);
      },
      function (error) {
        alert('There was an error setting the reported version.');
      });
    });

  })
};

function LoadProductList(products) {
  if (products == null) products = top.Ts.Cache.getProducts();

  for (var i = 0; i < products.length; i++) {
    AppendSelect('#ticket-Product', products[i], 'product', products[i].ProductID, products[i].Name, (products[i].ProductID === _ticketInfo.Ticket.ProductID));
  }

  var $productselect = $('#ticket-Product').selectize({
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });

  if (_ticketInfo.Ticket.ProductID == null) {
    var $productselectInput = $productselect[0].selectize;
    $productselectInput.clear();
  }
}

function SetupProductVersionsControl(product) {
  var $select = $("#ticket-Versions").selectize({
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });
  var versionInput = $select[0].selectize;

  if (versionInput) {
    versionInput.destroy();
  }

  var $select = $("#ticket-Resolved").selectize({
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });
  var resolvedInput = $select[0].selectize;

  if (resolvedInput) {
    resolvedInput.destroy();
  }

  if (product !== null && product.Versions.length > 0) {
    var versions = product.Versions;
    for (var i = 0; i < versions.length; i++) {
      AppendSelect('#ticket-Versions', versions[i], 'version', versions[i].ProductVersionID, versions[i].VersionNumber, false);
      AppendSelect('#ticket-Resolved', versions[i], 'resolved', versions[i].ProductVersionID, versions[i].VersionNumber, false);
    }
    $('#ticket-Versions').selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      }
    });
    $('#ticket-Resolved').selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      }
    });
  }
}

function SetProductVersionAndResolved(versionId, resolvedId) {
  var $select = $("#ticket-Versions").selectize({
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });
  var versionInput = $select[0].selectize;

  if (versionId !== null) {
    versionInput.setValue(versionId);
  }
  else {
    versionInput.clear();
  }

  var $select = $("#ticket-Resolved").selectize({
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });
  var resolvedInput = $select[0].selectize;

  if (resolvedId !== null) {
    resolvedInput.setValue(resolvedId);
  }
  else {
    resolvedInput.clear();
  }
};

function SetupInventorySection() {
  AddInventory(_ticketInfo.Assets);

  $('#ticket-Inventory-Input').selectize({
    valueField: 'id',
    labelField: 'label',
    searchField: 'label',
    load: function (query, callback) {
      getAssets(query, callback)
    },
    onItemAdd: function (value, $item) {
      top.Ts.Services.Tickets.AddTicketAsset(_ticketID, value, function (assets) {
        AddInventory(assets);
        top.Ts.Services.Tickets.GetTicketCustomers(_ticketID, function (customers) {
          AddCustomers(customers);
        });
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addasset", userFullName);
      }, function () {
        alert('There was an error adding the asset.');
      });
      top.Ts.System.logAction('Ticket - Asset Added');
      this.removeItem(value, true);
    },
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    },

    plugins: {
      'sticky_placeholder': {}
    }
  });

  $('#ticket-Inventory').on('click', 'span.tagRemove', function (e) {
    var self = $(this);
    var data = self.parent().data().tag;
    top.Ts.Services.Tickets.RemoveTicketAsset(_ticketID, data.AssetID, function (assets) {
      AddInventory(assets);
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removeasset", userFullName);
    }, function () {
      alert('There was a problem removing the asset from the ticket.');
    });
  });
};

function AddInventory(Inventory) {
  var InventoryDiv = $("#ticket-Inventory");
  InventoryDiv.empty();
  $("#ticket-Inventory-Input").val('');

  for (var i = 0; i < Inventory.length; i++) {
    var newelement = PrependTag(InventoryDiv, Inventory[i].AssetID, Inventory[i].Name, Inventory[i], "tag-item AssetAnchor");
    newelement.data('assetid', Inventory[i].AssetID).data('placement', 'left');
  };
}

function SetupUserQueuesSection() {
  AddQueues(_ticketInfo.Queuers);

  $('#ticket-UserQueue-Input').selectize({
    valueField: 'id',
    labelField: 'label',
    searchField: 'label',
    load: function (query, callback) {
      getUsers(query, callback)
    },
    onItemAdd: function (value, $item) {
      top.Ts.Services.Tickets.SetQueue(_ticketID, true, value, function (queues) {
        AddQueues(queues);
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addqueue", userFullName);
      }, function () {
        alert('There was an error adding the queue.');
      });
      top.Ts.System.logAction('Ticket - Enqueued');
      top.Ts.System.logAction('Queued');
      this.removeItem(value, true);
    },
    plugins: {
      'sticky_placeholder': {}
    },
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });

  $('#ticket-UserQueue').on('click', 'span.tagRemove', function (e) {
    var self = $(this);
    var data = self.parent().data().tag;

    top.Ts.Services.Tickets.SetQueue(_ticketID, false, data.UserID, function (queues) {
      AddQueues(queues);
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removequeue", userFullName);
    }, function () {
      alert('There was a problem removing the queue from the ticket.');
    });
    top.Ts.System.logAction('Ticket - Dequeued');
  });
}

function AddQueues(queues) {
  var UserQueueDiv = $("#ticket-UserQueue");
  UserQueueDiv.empty();
  $("#ticket-UserQueue-Input").val('');

  for (var i = 0; i < queues.length; i++) {
    var newelement = PrependTag(UserQueueDiv, queues[i].UserID, queues[i].FirstName + " " + queues[i].LastName, queues[i], "tag-item UserAnchor");
    newelement.data('userid', queues[i].UserID).data('placement', 'left').data('ticketid', _ticketID);
  };
}

function SetupSubscribedUsersSection() {
  AddSubscribers(_ticketInfo.Subscribers);

  $('#ticket-SubscribedUsers-Input').selectize({
    valueField: 'id',
    labelField: 'label',
    searchField: 'label',
    load: function (query, callback) {
      getUsers(query, callback)
    },
    onItemAdd: function (value, $item) {
      top.Ts.Services.Tickets.SetSubscribed(_ticketID, true, value, function (subscribers) {
        AddSubscribers(subscribers);
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addsubscriber", userFullName);
      }, function () {
        alert('There was an error adding the subscriber.');
      });
      top.Ts.System.logAction('Ticket - User Subscribed');
      this.removeItem(value, true);
    },
    plugins: {
      'sticky_placeholder': {}
    },
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });

  $('#ticket-SubscribedUsers').on('click', 'span.tagRemove', function (e) {
    var self = $(this);
    var data = self.parent().data().tag;
    top.Ts.Services.Tickets.SetSubscribed(_ticketID, false, data.UserID, function (subscribers) {
      AddSubscribers(subscribers);
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removesubscriber", userFullName);
    }, function () {
      alert('There was a problem removing the subscriber from the ticket.');
    });
    top.Ts.System.logAction('Ticket - Subscriber Removed');
  });
};

function AddSubscribers(Subscribers) {
  var SubscribersDiv = $("#ticket-SubscribedUsers");
  SubscribersDiv.empty();
  $("#ticket-SubscribedUsers-Input").val('');

  for (var i = 0; i < Subscribers.length; i++) {
    var newelement = PrependTag(SubscribersDiv, Subscribers[i].UserID, Subscribers[i].FirstName + " " + Subscribers[i].LastName, Subscribers[i], "tag-item UserAnchor");
    newelement.data('userid', Subscribers[i].UserID).data('placement', 'left').data('ticketid', _ticketID);
  };
}

function SetupAssociatedTicketsSection() {
  AddAssociatedTickets(_ticketInfo.Related);

  $('#ticket-AssociatedTickets-Input').selectize({
    valueField: 'data',
    labelField: 'label',
    searchField: 'label',
    load: function (query, callback) {
      getRelated(query, callback)
    },
    onItemAdd: function (value, $item) {
      $('#AssociateTicketModal').data('ticketid', value).modal('show');
      this.removeItem(value, true);
    },
    plugins: {
      'sticky_placeholder': {}
    },
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });

  $('#ticket-AssociatedTickets').on('click', 'span.tagRemove', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var self = $(this);
    var data = self.parent().data().tag;
    top.Ts.Services.Tickets.RemoveRelated(_ticketID, data.TicketID, function (result) {
      if (result !== null && result === true) self.parent().remove();
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removerelationship", userFullName);
    }, function () {
      alert('There was an error removing the associated ticket.');
    });

    top.Ts.System.logAction('Ticket - Association Removed');
  });

  $('#ticket-AssociatedTickets').on('click', 'div.tag-item', function (e) {
    var self = $(this);
    var data = self.data().tag;
    top.Ts.MainPage.openTicket(data.TicketNumber, true);
  });

  $('.ticket-association').click(function (e) {
    var IsParent = $(this).data('isparent');
    var TicketID2 = $(this).closest('#AssociateTicketModal').data('ticketid');
    $('#associate-error').hide();
    $('#associate-success').hide();
    if (_ticketID == TicketID2) {
      $('#associate-error').text('You picked the ticket you are viewing. Please try again.').show();
      return;
    }
    top.Ts.Services.Tickets.AddRelated(_ticketID, TicketID2, IsParent, function (tickets) {
      AddAssociatedTickets(tickets);
      $('#associate-success').show();
      setTimeout(function () { $('#AssociateTicketModal').modal('hide'); }, 2000);
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addrelationship", userFullName);
    }, function (error) {
      $('#associate-error').text(error.get_message()).show();
    });
  });
};

function AddAssociatedTickets(Tickets) {
  if (Tickets !== null) {
    var AssociatedTicketsDiv = $("#ticket-AssociatedTickets");
    AssociatedTicketsDiv.empty();
    $("#ticket-AssociatedTickets-Input").val('');
    for (var i = 0; i < Tickets.length; i++) {
      var related = Tickets[i];
      var caption = 'Related';
      if (related.IsParent !== null) {
        caption = (related.IsParent === true ? 'Parent' : 'Child');
      }
      var label = caption + "<br />" + ellipseString(related.TicketNumber + ': ' + related.Name, 30);

      var newelement = PrependTag(AssociatedTicketsDiv, related.TicketID, related.IsClosed ? '<s>' + label + '</s>' : label, related, 'tag-item TicketAnchor');
      newelement.data('ticketid', related.TicketID).data('placement', 'left');
    };
  }
}

function SetupRemindersSection() {
  AddReminders(_ticketInfo.Reminders);

  $('#ticket-reminder-date').datetimepicker({ useCurrent: true, format: 'MM/DD/YYYY hh:mm A', defaultDate: new Date() });

  var $reminderSelect = $('#ticket-reminder-who').selectize({
    valueField: 'id',
    labelField: 'label',
    searchField: 'label',
    load: function (query, callback) {
      top.Ts.Services.TicketPage.SearchUsers(query, function (result) {
        callback(result);
      });

    },
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });

  $('#ticket-reminder-save').click(function (e) {
    var selectizeControl = $reminderSelect[0].selectize;
    var date = top.Ts.Utils.getMsDate($('#ticket-reminder-date').val());
    var userid = selectizeControl.getValue();
    if (userid == "") {
      $('#ticket-reminder-who').parent().addClass('has-error').removeClass('has-success');
    }
    else {
      $('#ticket-reminder-who').closest('form-group').addClass('has-success').removeClass('has-error');
    }
    var title = $('#ticket-reminder-title').val();
    if (title == "") {
      $('#ticket-reminder-title').parent().addClass('has-error').removeClass('has-success');
    }
    else {
      $('#ticket-reminder-title').parent().addClass('has-success').removeClass('has-error');
    }

    top.Ts.Services.System.EditReminder(null, top.Ts.ReferenceTypes.Tickets, _ticketID, title, date, userid, function (result) {
      $('#reminder-success').show();
      var label = ellipseString(result.Description, 30) + '<br>' + result.DueDate.localeFormat(top.Ts.Utils.getDateTimePattern())
      PrependTag($("#ticket-reminder-span"), result.ReminderID, label, result);
      setTimeout(function () { $('#RemindersModal').modal('hide'); }, 2000);
    },
    function () {
      $('#reminder-error').show();
    });
  });

  $('#ticket-reminder-span').on('click', 'span.tagRemove', function (e) {
    var reminder = $(this).parent()[0];
    if (reminder) {
      top.Ts.Services.System.DismissReminder(reminder.id, function () {
        reminder.remove();
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "removereminder", userFullName);
      }, function () {
        alert('There was a problem removing the reminder from the ticket.');
      });
    }
    else {
      alert('There was a problem removing the reminder from the ticket.');
    }

  });
}
//TODO:  not working for some reaosn
function AddReminders(reminders) {
  var remindersDiv = $("#ticket-reminder-span");
  remindersDiv.empty();

  for (var i = 0; i < reminders.length; i++) {
    var label = ellipseString(reminders[i].Description, 30) + '<br>' + reminders[i].DueDate.localeFormat(top.Ts.Utils.getDateTimePattern())
    PrependTag(remindersDiv, reminders[i].ReminderID, label, reminders[i]);
  };
}

function SetupCustomFieldsSection() {
  AppenCustomValues(_ticketInfo.CustomValues);
}

function AppenCustomValues(fields) {
  var parentContainer = $('#ticket-group-custom-fields');
  if (fields === null || fields.length < 1) { parentContainer.empty().hide(); return; }
  parentContainer.empty()
  parentContainer.show();
  _parentFields = [];

  for (var i = 0; i < fields.length; i++) {
    var field = fields[i];

    if (field.CustomFieldCategoryID == -1) {
      switch (field.FieldType) {
        case top.Ts.CustomFieldType.Text: AddCustomFieldEdit(field, parentContainer); break;
        case top.Ts.CustomFieldType.Date: AddCustomFieldDate(field, parentContainer); break;
        case top.Ts.CustomFieldType.Time: AddCustomFieldTime(field, parentContainer); break;
        case top.Ts.CustomFieldType.DateTime: AddCustomFieldDateTime(field, parentContainer); break;
        case top.Ts.CustomFieldType.Boolean: AddCustomFieldBool(field, parentContainer); break;
        case top.Ts.CustomFieldType.Number: AddCustomFieldNumber(field, parentContainer); break;
        case top.Ts.CustomFieldType.PickList: AddCustomFieldSelect(field, parentContainer, false); break;
        default:
      }
    }
  }
  appendCategorizedCustomValues(fields);
}

var AssignUser = function (UserID) {
  var selectize = $('#ticket-assigned')[0].selectize;
  selectize.setValue(UserID);
}

var appendCategorizedCustomValues = function (fields) {
  top.Ts.Services.CustomFields.GetCategories(top.Ts.ReferenceTypes.Tickets, _ticketTypeID, function (categories) {
    var container = $('#ticket-group-custom-fields');
    for (var j = 0; j < categories.length; j++) {
      var isFirstFieldAdded = true;
      for (var i = 0; i < fields.length; i++) {
        var item = null;

        var field = fields[i];

        if (field.CustomFieldCategoryID == categories[j].CustomFieldCategoryID) {
          if (isFirstFieldAdded) {
            isFirstFieldAdded = false;
            var header = $('<label>').text(categories[j].Category).addClass('customFieldCategoryHeader');
            container.append(header);
          }

          switch (field.FieldType) {
            case top.Ts.CustomFieldType.Text: AddCustomFieldEdit(field, container); break;
            case top.Ts.CustomFieldType.Date: AddCustomFieldDate(field, container); break;
            case top.Ts.CustomFieldType.Time: AddCustomFieldTime(field, container); break;
            case top.Ts.CustomFieldType.DateTime: AddCustomFieldDateTime(field, container); break;
            case top.Ts.CustomFieldType.Boolean: AddCustomFieldBool(field, container); break;
            case top.Ts.CustomFieldType.Number: AddCustomFieldNumber(field, container); break;
            case top.Ts.CustomFieldType.PickList: AddCustomFieldSelect(field, container, false); break;
            default:
          }
        }
      }
    }
    appendConditionalFields();
  });
}

var appendConditionalFields = function () {
  for (var i = 0; i < _parentFields.length; i++) {
    var field = _parentFields[i].data('field');
    $('.' + field.CustomFieldID + 'children').remove();
    var parentContainer = $('#ticket-group-custom-fields');
    var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children form-horizontal').appendTo(parentContainer);
    appendMatchingParentValueFields(childrenContainer, field);
  }
}

var appendMatchingParentValueFields = function (container, parentField) {
  top.Ts.Services.Tickets.GetMatchingParentValueFields(_ticketID, parentField.CustomFieldID, parentField.Value, function (fields) {
    for (var i = 0; i < fields.length; i++) {
      var field = fields[i];
      var div = $('<div>').addClass('form-group form-group-sm').data('field', field);
      $('<label>').addClass('col-sm-4 control-label select-label').text(field.Name).appendTo(div);

      container.append(div);

      switch (field.FieldType) {
        case top.Ts.CustomFieldType.Text: AddCustomFieldEdit(field, div); break;
        case top.Ts.CustomFieldType.Date: AddCustomFieldDate(field, div); break;
        case top.Ts.CustomFieldType.Time: AddCustomFieldTime(field, div); break;
        case top.Ts.CustomFieldType.DateTime: AddCustomFieldDateTime(field, div); break;
        case top.Ts.CustomFieldType.Boolean: AddCustomFieldBool(field, div); break;
        case top.Ts.CustomFieldType.Number: AddCustomFieldNumber(field, div); break;
        case top.Ts.CustomFieldType.PickList: AddCustomFieldSelect(field, div, true); break;
        default:
      }
    }
  });
}

var AddCustomFieldEdit = function (field, parentContainer) {
  var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
  var groupContainer = $('<div>').addClass('form-group form-group-sm')
                          .data('field', field)
                          .appendTo(formcontainer)
                          .append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
  var inputContainer = $('<div>').addClass('col-sm-8 ticket-input-container').appendTo(groupContainer);
  var inputGroupContainer = $('<div>').addClass('input-group').appendTo(inputContainer);
  var input = $('<input type="text">')
                  .addClass('form-control ticket-simple-input')
                  .val(field.Value)
                  .appendTo(inputGroupContainer)
                  .after(getUrls(field.Value));


  if (field.Mask) {
    input.mask(field.Mask);
    input.attr("placeholder", field.Mask);
  }

  input.change(function (e) {
    var value = input.val();

    if (field.IsRequired && (value === null || $.trim(value) === '')) {
      groupContainer.addClass('hasError');
    }
    else {
      groupContainer.removeClass('hasError');
    }
    if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
      groupContainer.addClass('hasCloseError');
      alert("This field can not be cleared in a closed ticket");
      return;
    }
    else {
      groupContainer.removeClass('hasCloseError');
    }
    if (value === null || $.trim(value) === '') {
      groupContainer.addClass('isEmpty');
    }
    else {
      groupContainer.removeClass('isEmpty');
    }
    top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
      groupContainer.data('field', result);
      groupContainer.find('.external-link').remove();
      input.after(getUrls(result.Value));
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
    }, function () {
      alert("There was a problem saving your ticket property.");
    });
  });

  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasError');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasCloseError');
  }
  if (field.IsRequiredToClose) {
    groupContainer.addClass('isRequiredToClose');
  }
  if (field.Value === null || $.trim(field.Value) === '') {
    groupContainer.addClass('isEmpty');
  }
}

var AddCustomFieldDate = function (field, parentContainer) {
  var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);
  var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
  var groupContainer = $('<div>').addClass('form-group form-group-sm').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
  var dateContainer = $('<div>').addClass('col-sm-8 ticket-input-container').attr('style', 'padding-top: 3px;').appendTo(groupContainer);
  var dateLink = $('<a>')
                      .attr('href', '#')
                      .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDatePattern())))
                      .addClass('control-label')
                      .attr('style', 'padding-left: 5px;')
                      .appendTo(dateContainer);

  dateLink.click(function (e) {
    e.preventDefault();
    $(this).hide();
    var input = $('<input type="text">')
                    .addClass('form-control')
                    .val(date === null ? '' : date.localeFormat(top.Ts.Utils.getDatePattern()))
                    .datetimepicker({ pickTime: false })
                    .appendTo(dateContainer)
                    .focus();

    input.focusout(function (e) {
      var value = top.Ts.Utils.getMsDate(input.val());
      this.remove();
      dateLink.text((value === null ? 'Unassigned' : value.localeFormat(top.Ts.Utils.getDatePattern()))).show();

      if (field.IsRequired && (value === null || $.trim(value) === '')) {
        groupContainer.addClass('hasError');
      }
      else {
        groupContainer.removeClass('hasError');
      }
      if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
        groupContainer.addClass('hasCloseErrory');
        alert("This field can not be cleared in a closed ticket");
        return;
      }
      else {
        groupContainer.removeClass('hasCloseErrory');
      }
      if (value === null || $.trim(value) === '') {
        groupContainer.addClass('isEmpty');
      }
      else {
        groupContainer.removeClass('isEmpty');
      }

      top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
      }, function () {
        alert("There was a problem saving your ticket property.");
      });
    })
  });

  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasError');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasCloseError');
  }
  if (field.IsRequiredToClose) {
    groupContainer.addClass('isRequiredToClose');
  }
  if (field.Value === null || $.trim(field.Value) === '') {
    groupContainer.addClass('isEmpty');
  }
}

var AddCustomFieldDateTime = function (field, parentContainer) {
  var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);
  var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
  var groupContainer = $('<div>').addClass('form-group form-group-sm').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
  var dateContainer = $('<div>').addClass('col-sm-8 ticket-input-container').attr('style', 'padding-top: 3px;').appendTo(groupContainer);
  var dateLink = $('<a>')
                      .attr('href', '#')
                      .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern())))
                      .addClass('control-label')
                      .attr('style', 'padding-left: 5px;')
                      .appendTo(dateContainer);

  dateLink.click(function (e) {
    e.preventDefault();
    $(this).hide();
    var input = $('<input type="text">')
                    .addClass('form-control')
                    .val(date === null ? '' : date.localeFormat(top.Ts.Utils.getDateTimePattern()))
                    .datetimepicker()
                    .appendTo(dateContainer)
                    .focus();

    input.focusout(function (e) {
      var value = top.Ts.Utils.getMsDate(input.val());
      this.remove();
      dateLink.text((value === null ? 'Unassigned' : value.localeFormat(top.Ts.Utils.getDateTimePattern()))).show();

      if (field.IsRequired && (value === null || $.trim(value) === '')) {
        groupContainer.addClass('hasError');
      }
      else {
        groupContainer.removeClass('hasError');
      }
      if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
        groupContainer.addClass('hasCloseErrory');
        alert("This field can not be cleared in a closed ticket");
        return;
      }
      else {
        groupContainer.removeClass('hasCloseErrory');
      }
      if (value === null || $.trim(value) === '') {
        groupContainer.addClass('isEmpty');
      }
      else {
        groupContainer.removeClass('isEmpty');
      }

      top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
      }, function () {
        alert("There was a problem saving your ticket property.");
      });
    })
  });

  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasError');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasCloseError');
  }
  if (field.IsRequiredToClose) {
    groupContainer.addClass('isRequiredToClose');
  }
  if (field.Value === null || $.trim(field.Value) === '') {
    groupContainer.addClass('isEmpty');
  }
}

var AddCustomFieldTime = function (field, parentContainer) {
  var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);
  var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
  var groupContainer = $('<div>').addClass('form-group form-group-sm').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
  var dateContainer = $('<div>').addClass('col-sm-8 ticket-input-container').attr('style', 'padding-top: 3px;').appendTo(groupContainer);
  var dateLink = $('<a>')
                      .attr('href', '#')
                      .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getTimePattern())))
                      .addClass('control-label')
                      .attr('style', 'padding-left: 5px;')
                      .appendTo(dateContainer);

  dateLink.click(function (e) {
    e.preventDefault();
    $(this).hide();
    var input = $('<input type="text">')
                    .addClass('form-control')
                    .val(date === null ? '' : date.localeFormat(top.Ts.Utils.getTimePattern()))
                    .datetimepicker({ pickDate: false })
                    .appendTo(dateContainer)
                    .focus();

    input.focusout(function (e) {
      var value = top.Ts.Utils.getMsDate("1/1/1900 " + input.val());
      this.remove();
      dateLink.text((value === null ? 'Unassigned' : value.localeFormat(top.Ts.Utils.getTimePattern()))).show();

      if (field.IsRequired && (value === null || $.trim(value) === '')) {
        groupContainer.addClass('hasError');
      }
      else {
        groupContainer.removeClass('hasError');
      }
      if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
        groupContainer.addClass('hasCloseErrory');
        alert("This field can not be cleared in a closed ticket");
        return;
      }
      else {
        groupContainer.removeClass('hasCloseErrory');
      }
      if (value === null || $.trim(value) === '') {
        groupContainer.addClass('isEmpty');
      }
      else {
        groupContainer.removeClass('isEmpty');
      }

      top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
      }, function () {
        alert("There was a problem saving your ticket property.");
      });
    })
  });

  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasError');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasCloseError');
  }
  if (field.IsRequiredToClose) {
    groupContainer.addClass('isRequiredToClose');
  }
  if (field.Value === null || $.trim(field.Value) === '') {
    groupContainer.addClass('isEmpty');
  }
}

var AddCustomFieldBool = function (field, parentContainer) {
  var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
  var groupContainer = $('<div>')
                          .addClass('form-group form-group-sm')
                          .data('field', field)
                          .appendTo(formcontainer)
                          .append($('<label>').addClass('col-sm-4 control-label').text(field.Name));
  var inputContainer = $('<div>').addClass('col-sm-8 ticket-input-container').appendTo(groupContainer);
  var inputDiv = $('<div>').addClass('checkbox ticket-checkbox').appendTo(inputContainer);
  var input = $('<input type="checkbox">').appendTo(inputDiv);
  var value = (field.Value === null || $.trim(field.Value) === '' || field.Value === 'False' ? false : true);
  input.attr("checked", value);

  input.change(function (e) {
    var isChecked = input.is(':checked')
    top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, isChecked, function (result) {
      groupContainer.data('field', result);
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
    }, function () {
      alert("There was a problem saving your ticket property.");
    });
  });
}

var AddCustomFieldNumber = function (field, parentContainer) {
  var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
  var groupContainer = $('<div>').addClass('form-group form-group-sm').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
  var inputContainer = $('<div>').addClass('col-sm-8 ticket-input-container').appendTo(groupContainer);
  var input = $('<input type="text">')
                  .addClass('form-control ticket-simple-input')
                  .val(field.Value)
                  .appendTo(inputContainer)
                  .numeric();

  input.change(function (e) {
    var value = input.val();

    if (field.IsRequired && (value === null || $.trim(value) === '')) {
      groupContainer.addClass('hasError');
    }
    else {
      groupContainer.removeClass('hasError');
    }
    if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
      groupContainer.addClass('hasCloseError');
      alert("This field can not be cleared in a closed ticket");
      return;
    }
    else {
      groupContainer.removeClass('hasCloseError');
    }
    if (value === null || $.trim(value) === '') {
      groupContainer.addClass('isEmpty');;
    }
    else {
      groupContainer.removeClass('isEmpty');
    }
    top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
      groupContainer.data('field', result);
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
    }, function () {
      alert("There was a problem saving your ticket property.");
    });
  });

  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasError');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasCloseError');
  }
  if (field.IsRequiredToClose) {
    groupContainer.addClass('isRequiredToClose');
  }
  if (field.Value === null || $.trim(field.Value) === '') {
    groupContainer.addClass('isEmpty');
  }
}

var AddCustomFieldSelect = function (field, parentContainer, loadConditionalFields) {
  var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
  var groupContainer = $('<div>').addClass('form-group form-group-sm').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
  var selectContainer = $('<div>').addClass('col-sm-8 ticket-input-container').appendTo(groupContainer);
  var select = $('<select>').addClass('hidden-select').appendTo(selectContainer);
  var options = field.ListValues.split('|');

  if (field.Value == "") {
    $('<option>').text("unassigned").val("").appendTo(select);
    if (field.IsRequired) groupContainer.addClass('hasError');

  }
  for (var i = 0; i < options.length; i++) {
    var optionValue = options[i];
    var option = $('<option>').text(optionValue).val(optionValue).appendTo(select);
    if (field.Value === options[i]) option.attr('selected', 'selected');
  }
  select.selectize({
    allowEmptyOption: true,
    onItemAdd: function (value, $item) {
      if (field.IsRequired && field.IsFirstIndexSelect == true && value == "") {
        groupContainer.addClass('hasError');
      }
      else {
        groupContainer.removeClass('hasError');
      }

      if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && field.IsFirstIndexSelect == true && value == "") {
        groupContainer.addClass('hasCloseError');
        alert("This field can not be cleared in a closed ticket");
        return;
      }
      else {
        groupContainer.removeClass('hasCloseError');
      }

      if (field.IsFirstIndexSelect == true && value == "") {
        groupContainer.addClass('isEmpty');
      }
      else {
        groupContainer.removeClass('isEmpty');
      }

      top.Ts.System.logAction('Ticket - Custom Value Set');
      top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
        $('.' + field.CustomFieldID + 'children').remove();
        var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children form-horizontal').appendTo(parentContainer);
        appendMatchingParentValueFields(childrenContainer, result);
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
      }, function () {
        alert("There was a problem saving your ticket property.");
      });
    },
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });

  var items = field.ListValues.split('|');
  if (field.IsRequired && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
    groupContainer.addClass('hasError');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
    groupContainer.addClass('hasCloseError');
  }
  if (field.IsRequiredToClose) {
    groupContainer.addClass('isRequiredToClose');
  }
  if ((field.IsFirstIndexSelect == true && items[0] == field.Value) || field.Value == null || $.trim(field.Value) === '') {
    groupContainer.addClass('isEmpty');
  }

  if (loadConditionalFields) {
    $('.' + field.CustomFieldID + 'children').remove();
    var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children form-horizontal').appendTo(parentContainer);
    appendMatchingParentValueFields(childrenContainer, field);
  }
  else {
    _parentFields.push(groupContainer);
  }
}

var SetupDueDateField = function (duedate) {
  var dateContainer = $('#ticket-duedate-container');
  var dateLink = $('<a>')
                      .attr('href', '#')
                      .text((duedate === null ? '' : duedate.localeFormat(top.Ts.Utils.getDateTimePattern())))
                      .addClass('control-label ticket-anchor ticket-nullable-link ticket-duedate-anchor')
                      .appendTo(dateContainer);

  dateLink.click(function (e) {
    e.preventDefault();
    $(this).hide();
    var input = $('<input type="text">')
                    .addClass('form-control')
                    .val(duedate === null ? '' : duedate.localeFormat(top.Ts.Utils.getDateTimePattern()))
                    .datetimepicker({
                      showClear: true,
                      sideBySide: true
                    })
                    .appendTo(dateContainer)
                    .focus();

    input.focusout(function (e) {
      var value = top.Ts.Utils.getMsDate(input.val());

      top.Ts.Services.Tickets.SetDueDate(_ticketID, value, function (result) {
        var date = result === null ? null : top.Ts.Utils.getMsDate(result);
        input.remove();
        dateLink.text((value === null ? 'Unassigned' : value.localeFormat(top.Ts.Utils.getDateTimePattern()))).show();
        _dueDate = top.Ts.Utils.getMsDate(value); //result;

        if (date != null && date < Date.now()) {
          $('#ticket-DueDate').addClass('nonrequired-field-error-font');
          $('#ticket-DueDate').parent().prev().addClass('nonrequired-field-error-font');
        }
        else {
          $('#ticket-DueDate').removeClass('nonrequired-field-error-font');
          $('#ticket-DueDate').parent().prev().removeClass('nonrequired-field-error-font');
        }
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeduedate", userFullName);
      }, function () {
        alert("There was a problem saving your ticket property.");
      });
    })
  });
};

var SetupStatusField = function (StatusId) {
  var statuses = top.Ts.Cache.getNextStatuses(StatusId);
  $("#ticket-status").selectize({
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    },
    onChange: function (value) {
      var status = top.Ts.Cache.getTicketStatus(value);
      isFormValidToClose(status.IsClosed, function (isValid) {
        if (isValid == true) {
          top.Ts.Services.Tickets.SetTicketStatus(_ticketID, value, function (result) {
            if (result !== null) {
              top.Ts.System.logAction('Ticket - Status Changed');
              $('#ticket-status-label').toggleClass('ticket-closed', result.IsClosed);
              window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changestatus", userFullName);
            }
          },
          function (error) {
            alert('There was an error setting your ticket status.');
          });
        }
        else {
          alert("Please fill in the required fields before closing the ticket.");
          return;
        }
      });
    },
    render: {
      item: function (item, escape) {
        if (item.data.IsClosed) {
          return '<div data-value="' + escape(item.value) + '" data-item="' + escape(item.data) + '" data-selectable="" class="option"><s>' + escape(item.text) + '</s></div>';
        }
        else {
          return '<div data-value="' + escape(item.value) + '" data-item="' + escape(item.data) + '" data-selectable="" class="option">' + escape(item.text) + '</div>';
        }
      }
    },
  });
  var selectize = $("#ticket-status")[0].selectize;
  selectize.clear(true);
  selectize.clearOptions();

  for (var i = 0; i < statuses.length; i++) {
    selectize.addOption({ value: statuses[i].TicketStatusID, text: statuses[i].Name, data: statuses[i] });
  }

  selectize.addItem(StatusId, true);
}

var getUrls = function (input) {
  var source = (input || '').toString();
  var parentDiv = $('<div>').addClass('input-group-addon external-link')
  var url;
  var matchArray;
  var result = '';

  // Regular expression to find FTP, HTTP(S) and email URLs. Updated to include urls without http
  var regexToken = /(((ftp|https?|www):?\/?\/?)[\-\w@:%_\+.~#?,&\/\/=]+)|((mailto:)?[_.\w-]+@([\w][\w\-]+\.)+[a-zA-Z]{2,3})/g;

  // Iterate through any URLs in the text.
  while ((matchArray = regexToken.exec(source)) !== null) {
    url = matchArray[0];
    if (url.length > 2 && url.substring(0, 3) == 'www') {
      url = 'http://' + url;
    }
    result = result + '<a target="_blank" class="valueLink" href="' + url + '" title="' + matchArray[0] + '"><i class="fa fa-external-link fa-lg custom-field-link"></i></a>';
  }

  if (result !== '') {
    return parentDiv.append(result);
  }

  return result;
}

function openTicketWindow(ticketID) {
  top.Ts.MainPage.openTicket(ticketID, true);
}


var setSLAInfo = function () {
  $('#ticket-SLAStatus').find('i').removeClass('color-green color-red color-yellow');
  if (_ticketInfo.Ticket.SlaViolationTime === null) {
    $('#ticket-SLAStatus').find('i').addClass('color-green');
    $('#ticket-SLANote').text('');
  }
  else {
    $('#ticket-SLAStatus')
      .find('i')
      .addClass((_ticketInfo.Ticket.SlaViolationTime < 1 ? 'color-red' : (_ticketInfo.Ticket.SlaWarningTime < 1 ? 'color-yellow' : 'color-green')));
    if (_ticketInfo.Ticket.SlaViolationDate !== undefined) {
      $('#ticket-SLANote').text(_ticketInfo.Ticket.SlaViolationDate.localeFormat(top.Ts.Utils.getDateTimePattern()));
    }
    else {
      $('#ticket-SLANote').text('');
    }
  }
  $('#ticket-SLAStatus').data('placement', 'left').data('ticketid', _ticketID);
}

var SetKBCategory = function (KnowledgeBaseCategoryID) {
  var selectField = $('#ticket-KB-Category');
  if (selectField.length > 0) {
    var selectize = $('#ticket-KB-Category')[0].selectize;
    selectize.setValue(KnowledgeBaseCategoryID);
  }
  else {
    $('#ticket-KB-Category-RO').text(_ticketInfo.Ticket.KnowledgeBaseCategoryName);
  }
}

var SetCommunityCategory = function (ForumCategory) {
  var selectField = $('#ticket-Community');
  if (selectField.length > 0) {
    var selectize = $('#ticket-Community')[0].selectize;
    selectize.setValue(ForumCategory);
  }
  else {
    $('#ticket-Community-RO').text((_ticketInfo.Ticket.CategoryName == null ? 'Unassigned' : _ticketInfo.Ticket.CategoryDisplayString));
  }
}

var SetDueDate = function (duedate) {
  $('.ticket-duedate-anchor').text((duedate === null ? '' : duedate.localeFormat(top.Ts.Utils.getDateTimePattern())));
};

var SetGroup = function (GroupID) {
  var selectField = $('#ticket-group');
  if (selectField.length > 0) {
    var selectize = $('#ticket-group')[0].selectize;
    selectize.setValue(GroupID);
  }
}

var SetStatus = function (StatusID) {
  var selectField = $('#ticket-status');
  if (selectField.length > 0) {
    var statuses = top.Ts.Cache.getNextStatuses(StatusID);
    var selectize = selectField[0].selectize;
    selectize.clear(true);
    selectize.clearOptions();

    for (var i = 0; i < statuses.length; i++) {
      selectize.addOption({ value: statuses[i].TicketStatusID, text: statuses[i].Name, data: statuses[i] });
    }

    selectize.addItem(StatusID, true);

  }
};

var SetType = function (TypeID) {
  var selectField = $('#ticket-type');
  if (selectField.length > 0) {
    var selectize = $('#ticket-type')[0].selectize;
    selectize.setValue(TypeID);
  }
};

var SetSeverity = function (SeverityID) {
  var selectField = $('#ticket-severity');
  if (selectField.length > 0) {
    var selectize = $('#ticket-severity')[0].selectize;
    selectize.setValue(SeverityID);
  }
};