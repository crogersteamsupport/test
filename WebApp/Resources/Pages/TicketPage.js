var _ticketInfo = null;
var _ticketNumber = null;
var _ticketID = null;
var _ticketCreator = new Object();
var _ticketSender = null;
var _ticketCurrStatus = null;

var _ticketGroupID = null;
var _ticketGroupUsers = null;
var _ticketTypeID = null;
var _parentFields = [];
var _productFamilyID = null;

var _isNewActionPrivate = true;
var _newAction = null;
var _oldActionID = null;

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

var execGetCustomer = null;
var execGetTags = null;
var execGetAsset = null;
var execGetUsers = null;
var execGetRelated = null;
var execSelectTicket = null;
var execGetCompany = null;

var session;
var token;
var recordingID;
var apiKey;
var sessionId;
var tokurl;
var publisher;

var getTicketCustomers = function (request, response) {
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

var getCompany = function (request, response) {
  if (execGetCompany) { execGetCompany._executor.abort(); }
  execGetCompany = top.Ts.Services.Organizations.WCSearchOrganization(request, function (result) { response(result); });
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

var isFormValidToClose = function (isClosed, callback) {
  var result = true;
  if (isClosed) {
    $('.isRequiredToClose.isEmpty').addClass('hasCloseError');
    if ($('.hasCloseError').length > 0) {
      result = false;
    }
  }
  callback(result);
}

var isFormValid = function (callback) {
  top.Ts.Settings.Organization.read('RequireNewTicketCustomer', false, function (requireNewTicketCustomer) {
    var result = true;

    if ($('.hasError').length > 0) {
      result = false;
    }

    //If custom required check if the ticket is a KB if not then see if we have at least one customer
    if (requireNewTicketCustomer == "True" && $('#ticket-isKB').is(":checked") == false) {
      if ($('#ticket-Customer > div.tag-item').length < 1) {
        $('#ticket-Customer').closest('.form-group').addClass('hasError');
        result = false;
      }
      else {
        $('#ticket-Customer').closest('.form-group').removeClass('hasError');
      }
    }

    if (callback) callback(result);
  });
}

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

  apiKey = "45228242";

  //Setup Ticket Elements
  SetupTicketPage();

  //Create Handlebar Helpers
  CreateHandleBarHelpers();

  //Create Dom Events
  CreateTicketToolbarDomEvents();
  CreateTimeLineDelegates();

  //Setup WC Area
  SetupWCArea();
});

var loadTicket = function (ticketNumber, refresh) {
  top.Ts.Services.Tickets.GetTicketInfo(_ticketNumber, function (info) {
    _ticketInfo = info;
    _ticketID = info.Ticket.TicketID;
    top.Ts.Services.Tickets.GetTicketLastSender(_ticketID, function (result) {
      if (result !== null) {
        _ticketSender = new Object();
        _ticketSender.UserID = result.UserID;
        _ticketSender.Name = result.FirstName + ' ' + result.LastName;
      }
    });
    _ticketCreator = new Object();
    _ticketCreator.UserID = info.Ticket.CreatorID;
    _ticketCreator.Name = info.Ticket.CreatorName;
    _productFamilyID = info.Ticket.ProductFamilyID;
    _ticketTypeID = _ticketInfo.Ticket.TicketTypeID;

    $('#ticket-title-label').text($.trim(_ticketInfo.Ticket.Name) === '' ? '[Untitled Ticket]' : $.trim(_ticketInfo.Ticket.Name));
    $('#ticket-number').text('Ticket #' + _ticketInfo.Ticket.TicketNumber);
    top.Ts.Services.Customers.LoadTicketAlerts(_ticketID, function (note) {
      LoadTicketNotes(note);
    });


    $('#ticket-status-label').toggleClass('ticket-closed', _ticketInfo.Ticket.IsClosed);
    $('#ticket-visible').prop("checked", _ticketInfo.Ticket.IsVisibleOnPortal);
    $('#ticket-isKB').prop("checked", _ticketInfo.Ticket.IsKnowledgeBase);
    $('#ticket-KB-Category-RO').text(_ticketInfo.Ticket.KnowledgeBaseCategoryName);
    SetKBCategory(_ticketInfo.Ticket.KnowledgeBaseCategoryID);
    SetCommunityCategory(_ticketInfo.Ticket.ForumCategory);
    SetDueDate(_ticketInfo.Ticket.DueDate);

    //TODO:  Need to set product 

    SetAssignedUser(_ticketInfo.Ticket.UserID);
    SetGroup(_ticketInfo.Ticket.GroupID);
    SetType(_ticketInfo.Ticket.TicketTypeID);
    SetStatus(_ticketInfo.Ticket.TicketStatusID);
    SetSeverity(_ticketInfo.Ticket.TicketSeverityID);
    CreateNewAction(_ticketInfo.Actions)

    setSLAInfo();
    AddCustomers(_ticketInfo.Customers);
    AddAssociatedTickets(_ticketInfo.Related);
    AddTags(_ticketInfo.Tags);
    AppenCustomValues(_ticketInfo.CustomValues);
    AddSubscribers(_ticketInfo.Subscribers);
    AddQueues(_ticketInfo.Queuers);
    AddReminders(_ticketInfo.Reminders);
    AddInventory(_ticketInfo.Assets);
    LoadTicketHistory();

    if (typeof refresh === "undefined") {
      window.top.ticketSocket.server.getTicketViewing(_ticketNumber);
    }

  });
};

function CreateNewAction(actions) {
  var firstAction = $(".ticket-action[data-iswc='false']").first();

  var firstActionID = firstAction.data('id');
  if (firstActionID !== actions[0].Action.ActionID)
  {
    top.Ts.Services.TicketPage.ConvertActionItem(actions[0].Action.ActionID, function (actionInfo) {
      var actionElement = CreateActionElement(actionInfo, false);
      _actionTotal++;
      actionElement.find('.ticket-action-number').text(_actionTotal);
    });
  }
}

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
  top.Ts.Services.TicketPage.GetTicketInfo(_ticketNumber, function (info) {
    if (info == null) {
      var url = window.location.href;
      if (url.indexOf('.') > -1) {
        url = url.substring(0, url.lastIndexOf('/') + 1);
      }
      window.location = url + 'NoTicketAccess.html';
      return;
    }
    _ticketInfo = info;
    _ticketID = info.Ticket.TicketID;
    top.Ts.Services.Tickets.GetTicketLastSender(_ticketID, function (result) {
      if (result !== null) {
        _ticketSender = new Object();
        _ticketSender.UserID = result.UserID;
        _ticketSender.Name = result.FirstName + ' ' + result.LastName;
      }
    });
    _ticketCreator = new Object();
    _ticketCreator.UserID = info.Ticket.CreatorID;
    _ticketCreator.Name = info.Ticket.CreatorName;
    _productFamilyID = info.Ticket.ProductFamilyID;

    top.Ts.System.logAction('View Ticket');

    if (info == null) alert('no ticket');

    if (top.Ts.System.User.IsSystemAdmin || top.Ts.System.User.UserID === _ticketInfo.UserID) {
      $('.ticket-menu-actions').append('<li><a id="Ticket-Delete">Delete</a></li>');
      $('#Ticket-Delete').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (confirm('Are you sure you would like to delete this ticket?')) {
          top.Ts.System.logAction('Ticket - Deleted');
          top.Ts.Services.Tickets.DeleteTicket(_ticketID, function () {
            top.Ts.MainPage.closeTicketTab(_ticketNumber);
            window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "delete", userFullName);
          }, function () {
            alert('There was an error deleting this ticket.');
          });
        }
      });
    };

    //set the url for the copy paste button
    var ticketURLLink = new ZeroClipboard(document.getElementById("Ticket-URL"));
    ticketURLLink.on("aftercopy", function (event) {
      alert("Copied URL to clipboard: " + event.data["text/plain"]);
    });
    var ticketUrl = top.Ts.System.AppDomain + "/?TicketNumber=" + _ticketNumber;
    $("#Ticket-URL").attr("data-clipboard-text", ticketUrl);

    //set the ticket title 
    $('#ticket-title-label').text($.trim(_ticketInfo.Ticket.Name) === '' ? '[Untitled Ticket]' : $.trim(_ticketInfo.Ticket.Name));
    $('#ticket-number').text('Ticket #' + _ticketInfo.Ticket.TicketNumber);
    $('.ticket-source').css('backgroundImage', "url('../" + top.Ts.Utils.getTicketSourceIcon(_ticketInfo.Ticket.TicketSource) + "')").attr('title', 'Ticket Source: ' + (_ticketInfo.Ticket.TicketSource == null ? 'Agent' : _ticketInfo.Ticket.TicketSource));
    //get total number of actions so we can use it to number each action
    GetActionCount();
    //create timeline now that we have a ticketID
    FetchTimeLineItems(0);

    //action timers
    SetupActionTimers();

    //Setup ToolTips
    SetupToolTips();
    //update ticket property controls with the values needed
    LoadTicketControls();
    //Get Ticket Notes for Customers associated with ticket
    top.Ts.Services.Customers.LoadTicketAlerts(_ticketID, function (note) {
      LoadTicketNotes(note);
    });

    $('.page-loading').hide().next().show();

    isFormValid();

    if (typeof refresh === "undefined") {
      window.top.ticketSocket.server.getTicketViewing(_ticketNumber);
    }

  });
};

function SetupToolTips() {
  $('#Ticket-Subscribe').attr('data-original-title', (_ticketInfo.Ticket.IsSubscribed) ? 'UnSubscribe to Ticket' : 'Subscribe to Ticket');
  $('#Ticket-Queue').attr('data-original-title', (_ticketInfo.Ticket.IsEnqueued) ? 'Remove from your Ticket Queue' : 'Add to your Ticket Queue');
  $('#Ticket-Flag').attr('data-original-title', (_ticketInfo.Ticket.IsFlagged) ? 'UnFlag Ticket' : 'Flag Ticket');
  $('.btn-group [data-toggle="tooltip"]').tooltip({ placement: 'bottom', container: '.ticket-toolbar-row', animation: false });
};

function CreateNewActionLI() {
  var _compiledNewActionTemplate = Handlebars.compile($("#new-action-template").html());
  var html = _compiledNewActionTemplate({ OrganizationID: top.Ts.System.User.OrganizationID, UserID: top.Ts.System.User.UserID });
  $("#action-timeline").append(html);

  $('#action-add-public').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    var editor = $('#action-new-editor');
    SetupActionEditor(editor);
    SetupActionTypeSelect();
    FlipNewActionBadge(false);
    _isNewActionPrivate = false;
    $('#action-save-alert').text('').hide();
  });

  $('#action-add-private').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    var editor = $('#action-new-editor');
    SetupActionEditor(editor);
    SetupActionTypeSelect();
    FlipNewActionBadge(true);
    _isNewActionPrivate = true;
    $('#action-save-alert').text('').hide();
  });

  $('#action-add-wc').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#inputDescription').val("");
    $('#associationQueue').find('.upload-queue').empty();
    $('#associationQueue').find('.ticket-queue').empty();
    $('#associationQueue').find('.group-queue').empty();
    $('#associationQueue').find('.customer-queue').empty();
    $('#associationQueue').find('.user-queue').empty();
    $('#associationQueue').find('.product-queue').empty();
    $('.watercooler-new-area').show();
  });

  $('#action-new-cancel').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#action-new-editor').parent().fadeOut('normal', function () {
      tinymce.activeEditor.destroy();
    });
    top.Ts.MainPage.highlightTicketTab(_ticketNumber, false);
    $('#recorder').remove();
  });

  $('#action-timeline').on('click', '#newcommentcancel', function (e) {
    $('.watercooler-new-area').fadeOut('normal');
  });

  $('#action-new-save').click(function (e) {
    if ($("#recorder").length == 0) {
      e.preventDefault();
      e.stopPropagation();
      var self = $(this);
      self.prop('disabled', true);
      _oldActionID = self.data('actionid');

      isFormValid(function (isValid) {
        if (isValid) {
          SaveAction(_oldActionID, _isNewActionPrivate, function (result) {
            $('#action-new-editor').parent().fadeOut('normal', function () {
              tinymce.activeEditor.destroy();
            });
            if ($('.upload-queue li').length > 0) {
              UploadAttachments(result);
            }
            else
            {
              _newAction = null;
              if (_oldActionID === -1) {
                _actionTotal = _actionTotal + 1;
                var actionElement = CreateActionElement(result, false);
                actionElement.find('.ticket-action-number').text(_actionTotal);
              }
              else {
                UpdateActionElement(result, false);
              }
            }
          });
        }
        else {
          self.prop('disabled', false);
          alert("Please fill in the required fields before submitting this action.");
          return;
        }
      });
    }
  });

  $('#action-timeline').on('click', '.action-create-option', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var self = $(this);
    var _oldActionID = self.data('actionid');
    SaveAction(_oldActionID, _isNewActionPrivate, function (result) {
      UploadAttachments(result);
      $('#action-new-editor').val('').parent().fadeOut('normal');
      tinymce.activeEditor.destroy();

      var statusID = self.data("statusid");
      top.Ts.Services.Tickets.SetTicketStatus(_ticketID, statusID, function () {
        SetupStatusField(statusID);
        top.Ts.System.logAction('Ticket - Status Changed');
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changestatus", userFullName);
      });

      top.Ts.Services.TicketPage.GetActionAttachments(result.item.RefID, function (attachments) {
        result.Attachments = attachments;
        CreateActionElement(result, false);
      });
    });
  });

  $('#action-timeline').on('click', '.remove-attachment', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var self = $(this);
    var attachmentid = self.data('attachmentid');
    var filename = self.data('name');

    if (!confirm('Are you sure you would like to delete "' + filename + '."')) return;
    top.Ts.Services.Tickets.DeleteAttachment(
      attachmentid,
      function () {
        self.prev().remove();
        self.remove();
      }, function () {
        alert('There was a problem deleting "' + attachment.FileName + '."');
      });
  });
  //remove-attachment

  $('#action-new-type').change(function (e) {
    var actionID = $(this).val();
    var action = $(this).find(':selected').data('data');
    HideActionTimer(!action.IsTimed);
    top.Ts.Services.TicketPage.GetActionTicketTemplate(actionID, function (result) {
      if (result != null && result != "" && result != "<br>") {
        var currenttext = tinyMCE.activeEditor.getContent();
        tinyMCE.activeEditor.setContent(currenttext + result);
      }
      elem.parent().fadeIn('normal');
    });
  });
};

function SetupActionEditor(elem, action) {
  $('button.wc-textarea-send').prop('disabled', false);
  $('#action-new-save').prop('disabled', false);
  $('#newcomment').prop('disabled', false);
  $('.watercooler-new-area').hide();
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
    }
    elem.parent().fadeIn('normal');

    $('.frame-container').animate({
      scrollTop: 0
    }, 600);
  });

  var element = $('.action-new-area');
  $('#action-file-upload').fileupload({
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
      top.Ts.Services.TicketPage.GetActionAttachments(_newAction.item.RefID, function (attachments) {debugger
        _newAction.Attachments = attachments;
        if (_oldActionID === -1) {
          _actionTotal = _actionTotal + 1;
          var actionElement = CreateActionElement(_newAction, false);
          actionElement.find('.ticket-action-number').text(_actionTotal);
        }
        else {
          UpdateActionElement(_newAction, false);
        }
        _newAction = null;
      });
    }
  });

  element.find('#rcdtok').click(function (e) {
      top.Ts.Services.Tickets.StartArchiving(sessionId, function (resultID) {
          element.find('#rcdtok').hide();
          element.find('#stoptok').show();
          element.find('#inserttok').hide();
          element.find('#deletetok').hide();
          recordingID = resultID;
          element.find('#statusText').text("Currently Recording ...");
      });
  });

  element.find('#stoptok').hide();

  element.find('#stoptok').click(function (e) {
      element.find('#statusText').text("Processing...");
      top.Ts.Services.Tickets.StopArchiving(recordingID, function (resultID) {
          element.find('#rcdtok').show();
          element.find('#stoptok').hide();
          element.find('#inserttok').show();
          element.find('#canceltok').show();
          tokurl = "https://s3.amazonaws.com/teamsupportvideos/45228242/" + resultID + "/archive.mp4";
          element.find('#statusText').text("Recording Stopped");
      });
  });

  element.find('#inserttok').hide();

  element.find('#inserttok').click(function (e) {
      tinyMCE.activeEditor.execCommand('mceInsertContent', false, '<br/><br/><video width="400" height="400" controls><source src="' + tokurl + '" type="video/mp4"><a href="' + tokurl + '">Please click here to view the video.</a></video>');
      session.unpublish(publisher);
      element.find('#rcdtok').show();
      element.find('#stoptok').hide();
      element.find('#inserttok').hide();
      element.find('#recordVideoContainer').hide();
      element.find('#statusText').text("");
  });

  element.find('#deletetok').hide();

  element.find('#canceltok').click(function (e) {
      if (recordingID) {
          element.find('#statusText').text("Cancelling Recording ...");
          top.Ts.Services.Tickets.DeleteArchive(recordingID, function (resultID) {
              element.find('#rcdtok').show();
              element.find('#stoptok').hide();
              element.find('#inserttok').hide();
              session.unpublish(publisher);
              element.find('#recordVideoContainer').hide();
              element.find('#statusText').text("");
          });
      }
      else {
          session.unpublish(publisher);
          element.find('#recordVideoContainer').hide();
      }
      element.find('#statusText').text("");
  });
  element.find('#recordVideoContainer').hide();

  var statuses = top.Ts.Cache.getNextStatuses(_ticketInfo.Ticket.TicketStatusID);
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
  HideActionTimer(!types[0].IsTimed)
};

function HideActionTimer(ShouldHide) {
  if (ShouldHide) {
    $('#action-new-minutes').closest('.form-group').hide();
    $('#action-new-hours').closest('.form-group').hide();
    $('#action-new-date-started').closest('.form-group').hide();
  }
  else {
    $('#action-new-minutes').closest('.form-group').show();
    $('#action-new-hours').closest('.form-group').show();
    $('#action-new-date-started').closest('.form-group').show();
  }
}

function FlipNewActionBadge(isPrivate) {
  if (isPrivate) {
    $('#private-badge').show();
    $('#public-badge').hide();
  }
  else {
    $('#private-badge').hide();
    $('#public-badge').show();
  }
  _isNewActionPrivate = isPrivate;
}

function SaveAction(_oldActionID, isPrivate, callback) {

  var action = new top.TeamSupport.Data.ActionProxy();

  action.ActionID = _oldActionID;
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
    _newAction = result;
    top.Ts.MainPage.highlightTicketTab(_ticketNumber, false);
    result.item.MessageType = actionType.Name;
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
}

function LoadTicketNotes(note) {
  if (note) {
    $('#modalAlertMessage').html(note.Description);
    $('#alertID').val(note.RefID);
    $('#alertType').val(note.RefType);

    var buttons = {
      "Close": function () {
        $(this).dialog("close");
      },
      "Snooze": function () {
        top.Ts.Services.Customers.SnoozeAlert($('#alertID').val(), $('#alertType').val());
        $(this).dialog("close");
      }
    }

    if (!top.Ts.System.Organization.HideDismissNonAdmins || top.Ts.System.User.IsSystemAdmin) {
      buttons["Dismiss"] = function () {
        top.Ts.Services.Customers.DismissAlert($('#alertID').val(), $('#alertType').val());
        $(this).dialog("close");
      }
    }

    $("#dialog").dialog({
      resizable: false,
      width: 'auto',
      height: 'auto',
      modal: true,
      create: function () {
        $(this).css('maxWidth', '800px');
      },
      buttons: buttons
    });

  }
};

function GetActionCount() {
  top.Ts.Services.TicketPage.GetActionCount(_ticketID, function (total) {
    _actionTotal = total;
    _workingActionNumer = total;
  });
};

function LoadTicketControls() {
  if (_ticketInfo.Ticket.IsFlagged) {
    $('#Ticket-Flag').children().addClass('color-red');
  }

  if (_ticketInfo.Ticket.IsEnqueued) {
    $('#Ticket-Queue').children().addClass('color-green');
  }

  if (_ticketInfo.Ticket.IsSubscribed) {
    $('#Ticket-Subscribe').children().addClass('color-green');
  }

  if ($('#ticket-assigned').length) {
    top.Ts.Services.TicketPage.GetTicketUsers(_ticketID, function (users) {
      for (var i = 0; i < users.length; i++) {
        AppendSelect('#ticket-assigned', users[i], 'group', users[i].ID, users[i].Name, users[i].IsSelected);
      }

      $('#ticket-assigned').selectize({
        onDropdownClose: function ($dropdown) {
          $($dropdown).prev().find('input').blur();
        },
        onChange: function (value) {
          top.Ts.Services.Tickets.SetTicketUser(_ticketID, value, function (userInfo) {
            window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeassigned", userFullName);
          },
          function (error) {
            alert('There was an error setting the assigned user.');
          });
        },
        closeAfterSelect: true
      });
    });
  }

  if ($('#ticket-group').length) {
    top.Ts.Services.TicketPage.GetTicketGroups(_ticketID, function (groups) {
      for (var i = 0; i < groups.length; i++) {
        AppendSelect('#ticket-group', groups[i], 'group', groups[i].ID, groups[i].Name, groups[i].IsSelected);
      }
      $('#ticket-group').selectize({
        onDropdownClose: function ($dropdown) {
          $($dropdown).prev().find('input').blur();
        },
        closeAfterSelect: true
      });
    });
  }


  _ticketTypeID = _ticketInfo.Ticket.TicketTypeID;
  var types = top.Ts.Cache.getTicketTypes();
  for (var i = 0; i < types.length; i++) {
    AppendSelect('#ticket-type', types[i], 'type', types[i].TicketTypeID, types[i].Name, (_ticketInfo.Ticket.TicketTypeID === types[i].TicketTypeID));
  }

  SetupStatusField(_ticketInfo.Ticket.TicketStatusID);

  $('#ticket-status-label').toggleClass('ticket-closed', _ticketInfo.Ticket.IsClosed);

  var severities = top.Ts.Cache.getTicketSeverities();
  for (var i = 0; i < severities.length; i++) {
    AppendSelect('#ticket-severity', severities[i], 'severity', severities[i].TicketSeverityID, severities[i].Name, (_ticketInfo.Ticket.TicketSeverityID === severities[i].TicketSeverityID));
  }

  $('#ticket-visible').prop("checked", _ticketInfo.Ticket.IsVisibleOnPortal)

  $('#ticket-isKB').prop("checked", _ticketInfo.Ticket.IsKnowledgeBase)

  if (top.Ts.System.User.ChangeKbVisibility || top.Ts.System.User.IsSystemAdmin) {
    if (_ticketInfo.Ticket.IsKnowledgeBase) {
      $('#ticket-isKB').prop("checked", true);
      $('#ticket-group-KBCat').show();
    }
    else {
      $('#ticket-isKB').prop("checked", false);
      $('#ticket-group-KBCat').hide();
    }

    var categories = top.Ts.Cache.getKnowledgeBaseCategories();
    for (var i = 0; i < categories.length; i++) {
      var cat = categories[i].Category;
      AppendSelect('#ticket-KB-Category', cat, 'category', cat.CategoryID, cat.CategoryName, (_ticketInfo.Ticket.KnowledgeBaseCategoryID === cat.CategoryID));

      for (var j = 0; j < categories[i].Subcategories.length; j++) {
        var subcat = categories[i].Subcategories[j];
        AppendSelect('#ticket-KB-Category', subcat, 'subcategory', subcat.CategoryID, cat.CategoryName + ' -> ' + subcat.CategoryName, (_ticketInfo.Ticket.KnowledgeBaseCategoryID === subcat.CategoryID));
      }
    }
    if ($('#ticket-KB-Category').length) {
      $('#ticket-KB-Category').selectize({
        onDropdownClose: function ($dropdown) {
          $($dropdown).prev().find('input').blur();
        },
        closeAfterSelect: true
      });
    }
  }
  else {
    $('#ticket-KBInfo').remove();
    $('#ticket-isKB-RO').text('Visible');
    $('#ticket-KB-Category-RO').text(_ticketInfo.Ticket.KnowledgeBaseCategoryName);
    $('#ticket-KBVisible-RO').show();
    $('#ticket-KBCat-RO').show();
  }

  $('#ticket-TimeSpent').text(top.Ts.Utils.getTimeSpentText(_ticketInfo.Ticket.HoursSpent));

  if (_ticketInfo.Ticket.IsClosed == true) {
    $('#ticket-DaysOpened').text(_ticketInfo.Ticket.DaysClosed).parent().prev().html('Days Closed');
  }
  else {
    $('#ticket-DaysOpened').text(_ticketInfo.Ticket.DaysOpened).parent().prev().html('Days Opened');
  }

  var dueDate = _ticketInfo.Ticket.DueDate;
  SetupDueDateField(dueDate);

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

      $('#ticket-Community').val(_ticketInfo.Ticket.ForumCategory);

      if ($('#ticket-Community').length) {
        $('#ticket-Community').selectize({
          onDropdownClose: function ($dropdown) {
            $($dropdown).prev().find('input').blur();
          },
          closeAfterSelect: true
        });
      }
    }
    else {
      $('#ticket-CommunityInfo-RO').show();
      $('#ticket-Community-RO').text((_ticketInfo.Ticket.CategoryName == null ? 'Unassigned' : _ticketInfo.Ticket.CategoryDisplayString));
      $('#ticket-Community-RO').show();
      $('#ticket-Community').closest('.form-horizontal').remove();
    }
  }
  else {
    $('#ticket-Community').closest('.form-horizontal').remove();
    //$('#ticket-Community-RO').remove();
  }

  $('.ticket-select').selectize({
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    },
    closeAfterSelect: true
  });

  setSLAInfo();

  SetupTicketPropertyEvents();
  if (top.Ts.System.Organization.ProductType == top.Ts.ProductType.Express) {
    $('#ticket-Customer').closest('.form-group').remove();
  }
  else SetupCustomerSection();

  SetupTagsSection();
  SetupProductSection();

  if (top.Ts.System.Organization.ProductType == top.Ts.ProductType.Express || top.Ts.System.Organization.ProductType === top.Ts.ProductType.HelpDesk) {
    $('#ticket-Product').closest('.form-horizontal').remove();
    $('#ticket-Resolved').closest('.form-horizontal').remove();
    $('#ticket-Versions').closest('.form-horizontal').remove();
  }
  else SetupProductSection();

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

function SetupTicketPropertyEvents() {
  $('#ticket-title-label').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    var self = $(this);
    self.hide();
    var parent = self.parent();
    var input = $('#ticket-title-input');

    var titleInputContainer = $('#ticket-title-input-panel').show();
    $('#ticket-title-input').val(_ticketInfo.Ticket.Name).focus().select();

    $('#ticket-title-save').click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      titleInputContainer.hide();
      top.Ts.Services.Tickets.SetTicketName(_ticketID, input.val(), function (result) {
        _ticketInfo.Ticket.Name = result;
        top.Ts.System.logAction('Ticket - Renamed');
        self.text($.trim(_ticketInfo.Ticket.Name) === '' ? '[Untitled Ticket]' : $.trim(_ticketInfo.Ticket.Name)).show();

        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeticketname", userFullName);
      },
      function (error) {
        alert('There was an error saving the ticket name.');
      });
    });

    $('#ticket-title-cancel').click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      titleInputContainer.hide();
      self.show();
    });
  });

  $('#ticket-group').change(function (e) {
    var self = $(this);
    var GroupID = self.val();
    top.Ts.Services.Tickets.SetTicketGroup(_ticketID, GroupID, function (result) {
      if (result !== null) {
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changegroup", userFullName);
        $('#ticket-group').focusout();
        if (GroupID != null) {
          top.Ts.Services.Users.GetGroupUsers(GroupID, function (result) {
            _ticketGroupUsers = result;
          });
        }
        else {
          _ticketGroupUsers = null;
        }
      }

      if (top.Ts.System.Organization.UpdateTicketChildrenGroupWithParent) {
        top.Ts.Services.Tickets.SetTicketChildrenGroup(_ticketID, GroupID);
      }
    },
    function (error) {
      alert('There was an error setting the group.');
    });
  });

  $('#ticket-type').change(function (e) {
    var self = $(this);
    var value = self.val();
    top.Ts.Services.TicketPage.SetTicketType(_ticketID, value, function (result) {
      if (result !== null) {
        _ticketTypeID = value;
        SetupStatusField(result[0].TicketStatusID);

        $('#ticket-status-label').toggleClass('ticket-closed', result[0].IsClosed);

        AppenCustomValues(result[1]);

        _ticketInfo.Ticket = result[2];
        setSLAInfo();

        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changetype", userFullName);
      }
    },
    function (error) {
      alert('There was an error setting your ticket type.');
    });
  });

  $('#ticket-severity').change(function (e) {
    var self = $(this);
    var value = self.val();
    top.Ts.Services.Tickets.SetTicketSeverity(_ticketID, value, function (result) {
      if (result !== null) {
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeseverity", userFullName);
      }
    },
    function (error) {
      alert('There was an error setting your ticket severity.');
    });
  });

  $('#ticket-visible').change(function (e) {
    var self = $(this);

    if (top.Ts.System.User.ChangeTicketVisibility || top.Ts.System.User.IsSystemAdmin) {
      var value = self.is(":checked");
      top.Ts.System.logAction('Ticket - Visibility Changed');
      top.Ts.Services.Tickets.SetIsVisibleOnPortal(_ticketID, value, function (result) {
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeisportal", userFullName);
      },
      function (error) {
        alert('There was an error saving the ticket portal visible\'s status.');
      });
    }
  });

  $('#ticket-isKB').change(function (e) {
    var self = $(this);

    if (top.Ts.System.User.ChangeKbVisibility || top.Ts.System.User.IsSystemAdmin) {
      var value = self.is(":checked");
      top.Ts.System.logAction('Ticket - KB Status Changed');
      top.Ts.Services.Tickets.SetIsKB(_ticketID, value,
      function (result) {
        if (result === true) {
          $('#ticket-group-KBCat').show();
        }
        else {
          $('#ticket-group-KBCat').hide();
        }
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeiskb", userFullName);
      },
      function (error) {
        alert('There was an error saving the ticket knowlegdgebase\'s status.');
      });
    }
  });

  $('#ticket-KB-Category').change(function (e) {
    var self = $(this);
    var value = self.val();
    top.Ts.System.logAction('Ticket - KnowledgeBase Community Changed');
    top.Ts.Services.Tickets.SetTicketKnowledgeBaseCategory(_ticketID, value, function (result) {
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changekbcat", userFullName);
    },
    function (error) {
      alert('There was an error setting your ticket knowledgebase category.');
    });
  });

  $('#ticket-Community').change(function (e) {
    var self = $(this);
    var value = self.val();
    var oldCatName = _ticketInfo.Ticket.CategoryName;
    var newCatName = self.text();
    top.Ts.System.logAction('Ticket - Community Changed');
    top.Ts.Services.Tickets.SetTicketCommunity(_ticketID, value, oldCatName == null ? 'Unassigned' : oldCatName, newCatName == null ? 'Unassigned' : newCatName, function (result) {
      window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecommunity", userFullName);
    },
    function (error) {
      alert('There was an error setting your ticket community.');
    });
  });
}

function SetupCustomerSection() {
  AddCustomers(_ticketInfo.Customers);
  if ($('#ticket-Customers-Input').length) {
    $('#ticket-Customers-Input').selectize({
      valueField: 'id',
      labelField: 'label',
      searchField: 'label',
      load: function (query, callback) {
        getTicketCustomers(query, callback)
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
        $('#ticket-Customers-Input').closest('.form-group').removeClass('hasError');
      },
      onItemAdd: function (value, $item) {
        if (this.settings.initData === false) {
          $('#ticket-Customers-Input').closest('.form-group').removeClass('hasError');
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
        },
        option_create: function (data, escape) {
          return '<div class="create">Create <strong>' + escape(data.input) + '</strong></div>';
        }
      },
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true
    });
  }

  $('#customer-company-input').selectize({
    valueField: 'label',
    labelField: 'label',
    searchField: 'label',
    load: function (query, callback) {
      getCompany(query, callback)
    },
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    },
    closeAfterSelect: true,
    plugins: {
      'sticky_placeholder': {}
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
    var cssClasses = "tag-item";

    if (customers[i].Flag) {
      cssClasses = cssClasses + " tag-error"
    }
    if (customers[i].Contact !== null && customers[i].Company !== null) {
      label = '<span class="UserAnchor" data-userid="' + customers[i].UserID + '" data-placement="left">' + customers[i].Contact + '</span><br/><span class="OrgAnchor" data-orgid="' + customers[i].OrganizationID + '" data-placement="left">' + customers[i].Company + '</span>';
      var newelement = PrependTag(customerDiv, customers[i].UserID, label, customers[i], cssClasses);
    }
    else if (customers[i].Contact !== null) {
      label = customers[i].Contact;
      cssClasses = cssClasses + ' UserAnchor';
      var newelement = PrependTag(customerDiv, customers[i].UserID, label, customers[i], cssClasses);
      newelement.data('userid', customers[i].UserID).data('placement', 'left').data('ticketid', _ticketID);
    }
    else if (customers[i].Company !== null) {
      label = customers[i].Company;
      cssClasses = cssClasses + ' OrgAnchor';
      var newelement = PrependTag(customerDiv, customers[i].OrganizationID, label, customers[i], cssClasses);
      newelement.data('orgid', customers[i].OrganizationID).data('placement', 'left').data('ticketid', _ticketID);
    }
  };
}

function SetupTagsSection() {
  AddTags(_ticketInfo.Tags);
  if ($('#ticket-tag-Input').length) {
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
  }
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
  return $(tagHTML).prependTo(parent).data('tag', data);
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

    var product = top.Ts.Cache.getProduct(_ticketInfo.Ticket.ProductID);
    SetupProductVersionsControl(product);
    SetProductVersionAndResolved(_ticketInfo.Ticket.ReportedVersionID, _ticketInfo.Ticket.SolvedVersionID);

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
        $('#ticket-Versions').closest('.form-group').removeClass('hasError');
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changereported", userFullName);
      },
      function (error) {
        alert('There was an error setting the reported version.');
      });
    });

    $('#ticket-Resolved').change(function (e) {
      top.Ts.System.logAction('Ticket - Resolved Version Changed');
      top.Ts.Services.Tickets.SetSolvedVersion(_ticketID, $(this).val(), function (result) {
        $('#ticket-Versions').closest('.form-group').removeClass('hasError');
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changeresolved", userFullName);
      },
      function (error) {
        alert('There was an error setting the reported version.');
      });
    });

  })
};

function LoadProductList(products) {
  if ($('#ticket-Product').length) {
    if (products == null) products = top.Ts.Cache.getProducts();

    for (var i = 0; i < products.length; i++) {
      AppendSelect('#ticket-Product', products[i], 'product', products[i].ProductID, products[i].Name, (products[i].ProductID === _ticketInfo.Ticket.ProductID));
    }

    var $productselect = $('#ticket-Product').selectize({
      render: {
        item: function (item, escape) {
          return '<div data-ticketid="' + _ticketID + '" data-productid="' + escape(item.value) + '" data-value="' + escape(item.value) + '" data-type="' + escape(item.data) + '" data-selectable="" data-placement="left" class="option ProductAnchor">' + escape(item.text) + '</div>';
        }
      },
      allowEmptyOption: true,
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true
    });

    if (_ticketInfo.Ticket.ProductID == null) {
      var $productselectInput = $productselect[0].selectize;
      $productselectInput.clear();
    }
  }
}

function SetupProductVersionsControl(product) {
  if ($('#ticket-Versions').length) {
    var $select = $("#ticket-Versions").selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true
    });
    var versionInput = $select[0].selectize;

    if (versionInput) {
      versionInput.destroy();
    }
  }
  if ($('#ticket-Resolved').length) {
    var $select = $("#ticket-Resolved").selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true
    });
    var resolvedInput = $select[0].selectize;

    if (resolvedInput) {
      resolvedInput.destroy();
    }
  }

  if (product !== null && product.Versions.length > 0) {
    var versions = product.Versions;
    for (var i = 0; i < versions.length; i++) {
      AppendSelect('#ticket-Versions', versions[i], 'version', versions[i].ProductVersionID, versions[i].VersionNumber, false);
      AppendSelect('#ticket-Resolved', versions[i], 'resolved', versions[i].ProductVersionID, versions[i].VersionNumber, false);
    }
    if ($('#ticket-Resolved').length) {
      $('#ticket-Versions').selectize({
        onDropdownClose: function ($dropdown) {
          $($dropdown).prev().find('input').blur();
        },
        closeAfterSelect: true
      });
    }

    if ($('#ticket-Resolved').length) {
      $('#ticket-Resolved').selectize({
        onDropdownClose: function ($dropdown) {
          $($dropdown).prev().find('input').blur();
        },
        closeAfterSelect: true
      });
    }
  }
}

function SetProductVersionAndResolved(versionId, resolvedId) {
  if ($('#ticket-Versions').length) {
    var $select = $("#ticket-Versions").selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true
    });
    var versionInput = $select[0].selectize;

    if (versionId !== null) {
      versionInput.setValue(versionId);
    }
    else {
      versionInput.clear();
    }
  }

  if ($('#ticket-Resolved').length) {
    var $select = $("#ticket-Resolved").selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true
    });
    var resolvedInput = $select[0].selectize;

    if (resolvedId !== null) {
      resolvedInput.setValue(resolvedId);
    }
    else {
      resolvedInput.clear();
    }
  }

  top.Ts.Services.Organizations.IsProductVersionRequired(function (IsProductVersionRequired) {
    if (IsProductVersionRequired && (versionId == null && resolvedId == null))
      $('#ticket-Versions').closest('.form-group').addClass('hasError');
    else
      $('#ticket-Versions').closest('.form-group').removeClass('hasError');
  });
};

function SetupInventorySection() {
  AddInventory(_ticketInfo.Assets);
  if ($('#ticket-Inventory-Input').length) {
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
      closeAfterSelect: true,
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
  }
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
  if ($('#ticket-UserQueue-Input').length) {
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
      },
      closeAfterSelect: true
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
  if ($('#ticket-SubscribedUsers-Input').length) {
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
      },
      closeAfterSelect: true
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
  }
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
  if ($('#ticket-AssociatedTickets-Input').length) {
    $('#ticket-AssociatedTickets-Input').selectize({
      valueField: 'data',
      labelField: 'label',
      searchField: 'label',
      loadThrottle: null,
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
      },
      closeAfterSelect: true
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
        $('#AssociateTicketModal').modal('hide');
        AddAssociatedTickets(tickets);
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addrelationship", userFullName);
      }, function (error) {
        $('#associate-error').text(error.get_message()).show();
      });
    });
  }
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
      newelement.data('ticketid', related.TicketID).data('placement', 'left').data('IsParent', related.IsParent);
    };
  }
}

function SetupRemindersSection() {
  AddReminders(_ticketInfo.Reminders);
  if ($('#ticket-reminder-who').length) {
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
      },
      closeAfterSelect: true
    });

    $('#ticket-reminder-save').click(function (e) {
      var selectizeControl = $reminderSelect[0].selectize;
      var date = top.Ts.Utils.getMsDate($('#ticket-reminder-date').val());
      var userid = selectizeControl.getValue();
      if (userid == "") {
        $('#ticket-reminder-who').parent().addClass('has-error').removeClass('has-success');
      }
      else {
        $('#ticket-reminder-who').closest('.form-group').addClass('has-success').removeClass('has-error');
      }
      var title = $('#ticket-reminder-title').val();
      if (title == "") {
        $('#ticket-reminder-title').parent().addClass('has-error').removeClass('has-success');
      }
      else {
        $('#ticket-reminder-title').parent().addClass('has-success').removeClass('has-error');
      }

      top.Ts.Services.System.EditReminder(null, top.Ts.ReferenceTypes.Tickets, _ticketID, title, date, userid, function (result) {
        var label = ellipseString(result.Description, 30) + '<br>' + result.DueDate.localeFormat(top.Ts.Utils.getDateTimePattern())
        PrependTag($("#ticket-reminder-span"), result.ReminderID, label, result);
        $('#RemindersModal').modal('hide');
        $('#ticket-reminder-title').val('');
        $('#ticket-reminder-date').val('');
        selectizeControl.clear();
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
}

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
        case top.Ts.CustomFieldType.PickList: AddCustomFieldSelect(field, parentContainer, true); break;
        default:
      }
    }
  }
  appendCategorizedCustomValues(fields);
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
            container.append($('<hr>')).append(header);
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
      //$('<label>').addClass('col-sm-4 control-label select-label').text(field.Name).appendTo(div);

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
                  .addClass('form-control ticket-simple-input muted-placeholder')
                  .attr("placeholder", "Enter Value")
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
  input.prop("checked", value);

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
                  .addClass('form-control ticket-simple-input muted-placeholder')
                  .attr("placeholder", "Enter Value")
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
  var select = $('<select>').addClass('hidden-select muted-placeholder').attr("placeholder", "Select Value").appendTo(selectContainer);
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
        var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children form-horizontal').insertAfter(formcontainer);
        appendMatchingParentValueFields(childrenContainer, result);
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
      }, function () {
        alert("There was a problem saving your ticket property.");
      });
    },
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    },
    closeAfterSelect: true
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
                      //.text((duedate === null ? '' : duedate.localeFormat(top.Ts.Utils.getDateTimePattern())))
                      .addClass('control-label ticket-anchor ticket-nullable-link ticket-duedate-anchor')
                      .appendTo(dateContainer);
  if (duedate !== null) {
    dateLink.text(duedate.localeFormat(top.Ts.Utils.getDateTimePattern()));
  }

  dateLink.click(function (e) {
    e.preventDefault();
    dateLink.hide();
    var input = $('<input type="text">')
                    .addClass('form-control')
                    .val(duedate === null ? '' : duedate.localeFormat(top.Ts.Utils.getDateTimePattern()))
                    .datetimepicker({
                      showClear: true,
                      sideBySide: true,
                      autoclose: true
                    })
                    .appendTo(dateContainer)
                    .focus();

    input.change(function (e) {
      var value = top.Ts.Utils.getMsDate(input.val());
      top.Ts.Services.Tickets.SetDueDate(_ticketID, value, function (result) {
        var date = result === null ? null : top.Ts.Utils.getMsDate(result);
        input.blur().remove();
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
  _ticketCurrStatus = StatusId;
  if ($('#ticket-status').length) {
    $("#ticket-status").selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true,
      onChange: function (value) {
        var status = top.Ts.Cache.getTicketStatus(value);
        isFormValidToClose(status.IsClosed, function (isValid) {
          if (isValid == true) {
            top.Ts.Services.Tickets.SetTicketStatus(_ticketID, value, function (result) {
              if (result !== null) {
                _ticketCurrStatus = result.TicketStatusID;
                SetStatus(value);
                top.Ts.System.logAction('Ticket - Status Changed');
                $('#ticket-status-label').toggleClass('ticket-closed', result.IsClosed);
                window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changestatus", userFullName);
              }
            },
            function (error) {
              SetStatus(_ticketCurrStatus);
              alert('There was an error setting your ticket status.');
            });
          }
          else {
            SetStatus(_ticketCurrStatus);
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

var LoadTicketHistory = function () {
  top.Ts.Services.Tickets.GetTicketHistory(_ticketID, function (logs) {
    var historyTable = $('#ticket-history-table > tbody');
    historyTable.empty().addClass('ts-loading');
    for (var i = 0; i < logs.length; i++) {
      var row = $('<tr>').appendTo(historyTable);
      var col1 = $('<td>').text(logs[i].CreatorName).appendTo(row);
      var col2 = $('<td>').text(logs[i].DateCreated.localeFormat(top.Ts.Utils.getDateTimePattern())).appendTo(row);
      var col3 = $('<td>').html(logs[i].Description).appendTo(row);
    }
    historyTable.removeClass('ts-loading');
  }, function () {
    alert('There was a problem retrieving the history for the ticket.');
  });
}

function openTicketWindow(ticketID) {
  top.Ts.MainPage.openTicket(ticketID, true);
}

function FetchTimeLineItems(start) {
  _isLoading = true;
  $('.results-loading').show();
  top.Ts.Services.TicketPage.GetTimeLineItems(_ticketID, start, function (TimeLineItems) {
    _timeLine = TimeLineItems;

    if (TimeLineItems.length < 1) {
      $('.results-loading').hide();
      $('.results-done').show();
    }
    else {
      //compile action template
      _compiledActionTemplate = Handlebars.compile($("#action-template").html());

      //create first timeline date marker if needed
      if (_currDateSpan == null) {
        _currDateSpan = _timeLine[0].item.DateCreated;
        var dateSpan = '<span class="label bgcolor-bluegray daybadge">' + _currDateSpan.localeFormat(top.Ts.Utils.getDatePattern()) + '</span>';
        $("#action-timeline").append(dateSpan);
      };

      for (i = 0; i < _timeLine.length; i++) {
        var timeLineItem = _timeLine[i];
        CreateActionElement(timeLineItem, !timeLineItem.item.IsPinned);
      }
      _isLoading = false;
      $('.results-loading').hide();
    };
  });

};

function CreateActionElement(val, ShouldAppend) {
  if (_currDateSpan.toDateString() !== val.item.DateCreated.toDateString()) {
    var dateSpan = '<span class="label bgcolor-bluegray daybadge">' + val.item.DateCreated.localeFormat(top.Ts.Utils.getDatePattern()) + '</span>';
    $("#action-timeline").append(dateSpan);
    _currDateSpan = val.item.DateCreated;
  }
  var html = _compiledActionTemplate(val);
  var actionElement = $(html);
  actionElement.find('a').attr('target', '_blank');
  if (ShouldAppend) {
    $("#action-timeline").append(actionElement);
  }
  else {
    var actionElement
    $('.action-placeholder').after(actionElement);
  }
  return actionElement;
};

function UpdateActionElement(val) {
  if (_currDateSpan.toDateString() !== val.item.DateCreated.toDateString()) {
    var dateSpan = '<span class="label bgcolor-bluegray daybadge">' + val.item.DateCreated.localeFormat(top.Ts.Utils.getDatePattern()) + '</span>';
    $("#action-timeline").append(dateSpan);
    _currDateSpan = val.item.DateCreated;
  }
  var html = _compiledActionTemplate(val);
  var li = $("#action-timeline li[data-id=" + val.item.RefID + "]");
  var actionNumber = li.find('.ticket-action-number').text();
  li.replaceWith(html);
  $("#action-timeline li[data-id=" + val.item.RefID + "]").find('.ticket-action-number').text(actionNumber);
};

function CreateHandleBarHelpers() {
  Handlebars.registerHelper('UserImageTag', function () {
    if (this.item.CreatorID !== -1) return '<img class="user-avatar pull-left" src="../../../dc/' + this.item.OrganizationID + '/useravatar/' + this.item.CreatorID + '/48" />';
    return '';
  });

  Handlebars.registerHelper('FormatDateTime', function (Date) {
    return Date.localeFormat(top.Ts.Utils.getDateTimePattern())
  });

  Handlebars.registerHelper('TimeLineLabel', function () {
    if (this.item.IsVisibleOnPortal) {
      return '<div class="bgcolor-green"><span class="bgcolor-green">&nbsp;</span><a href="#" class="action-option-visible">Public</a></div>';
    }
    else if (!this.item.IsWC) {
      return '<div class="bgcolor-orange"><span class="bgcolor-orange">&nbsp;</span><a href="#" class="action-option-visible">Private</a></div>';
    }
    else if (this.item.IsWC) {
      return '<div class="bgcolor-blue"><span class="bgcolor-blue">&nbsp;</span><label>WC</label></div>';
    }

    return '';
  });

  Handlebars.registerHelper('ActionData', function () {
    return JSON.stringify(this.item);
  });

  Handlebars.registerHelper('TagData', function () {
    return JSON.stringify(this.data);
  });

  Handlebars.registerHelper('ActionNumber', function () {
    _workingActionNumer = _workingActionNumer - 1;
    return _workingActionNumer + 1;
  });

  Handlebars.registerHelper('CanPin', function (options) {
    if (top.Ts.System.User.UserCanPinAction || top.Ts.System.User.IsSystemAdmin) { return options.fn(this); }
  });

  Handlebars.registerHelper('CanEdit', function (options) {
    var action = this.item;
    var canEdit = top.Ts.System.User.IsSystemAdmin || top.Ts.System.User.UserID === action.CreatorID;
    var restrictedFromEditingAnyActions = !top.Ts.System.User.IsSystemAdmin && top.Ts.System.User.RestrictUserFromEditingAnyActions;

    if (!(!top.Ts.System.User.AllowUserToEditAnyAction && (!canEdit || restrictedFromEditingAnyActions))) { return options.fn(this); }
  });

  Handlebars.registerHelper('CanKB', function (options) {
    if (top.Ts.System.User.ChangeKbVisibility || top.Ts.System.User.IsSystemAdmin) { return options.fn(this); }
  });

  Handlebars.registerHelper('CanMakeVisible', function (options) {
    if (top.Ts.System.User.ChangeTicketVisibility || top.Ts.System.User.IsSystemAdmin) { return options.fn(this); }
  });

  Handlebars.registerHelper('TimeSpent', function () {
    var hours = Math.floor(this.item.TimeSpent / 60);
    var mins = Math.floor(this.item.TimeSpent % 60);
    var timeSpentString = "";
    if (hours > 0) timeSpentString = hours + ((hours > 1) ? " hours " : " hour ");
    if (mins > 0) timeSpentString += mins + ((mins > 1) ? " minutes " : " minute ");
    if (timeSpentString == "") return ""
    else return " - " + timeSpentString

    return timeSpentString;
  });


  Handlebars.registerHelper('WCLikes', function () {
    if (this.Likes > 0) {
      return "+" + this.Likes;
    }
  });
};

function CreateTimeLineDelegates() {
  $("#action-timeline").on("mouseenter", ".action-options", function (event) {
    $(this).find(".action-options-icon").hide();
    $(this).find(".action-option-items").fadeIn();
  }).on("mouseleave", ".action-options", function (event) {
    $(this).find(".action-option-items").hide();
    $(this).find(".action-options-icon").fadeIn();
  });

  $('#action-timeline').on('click', 'a.action-option-pin', function (e) {
    e.preventDefault();
    e.stopPropagation();

    var self = $(this);
    var parentLI = self.closest('li');
    var titleElement = $('.action-placeholder');
    var Action = parentLI.data().action;

    parentLI.find(".action-option-items").hide();
    parentLI.find(".action-options-icon").fadeIn();
    if (top.Ts.System.User.IsSystemAdmin || top.Ts.System.User.UserCanPinAction) {
      $('a.ticket-action-pinned').addClass('hidden');
      top.Ts.System.logAction('Ticket - Action Pin Icon Clicked');
      top.Ts.Services.Tickets.SetActionPinned(_ticketID, Action.RefID, !Action.IsPinned,
      function (result) {
        parentLI.data().action.IsPinned = result;
        parentLI.find('a.ticket-action-pinned').toggleClass('hidden');
        var pinnedAction = $('.pinned');
        var actionID = parseInt(pinnedAction.find('.ticket-action-number').text()) + 1;

        titleElement.after(parentLI.clone().addClass('pinned'));
        parentLI.insertAfter(titleElement);
        parentLI.remove();

        var InLineElement = $("label.ticket-action-number:contains('" + actionID + "')").closest('li');
        if (InLineElement.length > 0) {
          InLineElement.after(pinnedAction.clone().removeClass('pinned'));
        }
        else {
          titleElement.next().after(pinnedAction.clone().removeClass('pinned'));
        }
        pinnedAction.remove();

      }, function () {
        alert('There was an error editing this action.');
      });
    }

  });

  $('#action-timeline').on('click', 'a.ticket-action-pinned', function (e) {
    e.preventDefault();
    e.stopPropagation();

    var self = $(this);
    var parentLI = self.closest('li');
    var action = parentLI.data().action;
    var titleElement = $('.action-placeholder');

    if (top.Ts.System.User.IsSystemAdmin || top.Ts.System.User.UserCanPinAction) {
      top.Ts.System.logAction('Ticket - Action Pin Icon Clicked');
      top.Ts.Services.Tickets.SetActionPinned(_ticketID, action.RefID, false,
      function (result) {
        parentLI.data().action.IsPinned = result;
        parentLI.find('a.ticket-action-pinned').toggleClass('hidden');

        var actionID = parseInt(parentLI.find('.ticket-action-number').text()) + 1;

        var InLineElement = $("label.ticket-action-number:contains('" + actionID + "')").closest('li');
        if (InLineElement.length > 0) {
          InLineElement.after(parentLI.clone().removeClass('pinned'));
        }
        else {
          titleElement.next().after(parentLI.clone().removeClass('pinned'));
        }
        $('a.ticket-action-pinned').addClass('hidden');
        parentLI.remove();
      }, function () {
        alert('There was an error editing this action.');
      });
    }
  });

  $('#action-timeline').on('click', 'a.action-option-kb', function (e) {
    e.preventDefault();
    e.stopPropagation();

    var self = $(this);
    var action = self.closest('li').data().action;

    if (top.Ts.System.User.ChangeKbVisibility || top.Ts.System.User.IsSystemAdmin) {
      top.Ts.System.logAction('Ticket - Action KB Icon Clicked');
      top.Ts.Services.Tickets.SetActionKb(action.RefID, !action.IsKnowledgeBase,
    function (result) {
      var parentLI = self.closest('li');
      parentLI.data().action.IsKnowledgeBase = result;
      parentLI.find('a.ticket-action-kb').toggleClass('hidden');
    }, function () {
      alert('There was an error editing this action.');
    });
    }
  });

  $('#action-timeline').on('click', 'a.ticket-action-kb', function (e) {
    e.preventDefault();
    e.stopPropagation();

    var self = $(this);
    var action = self.closest('li').data().action;

    if (top.Ts.System.User.ChangeKbVisibility || top.Ts.System.User.IsSystemAdmin) {
      top.Ts.System.logAction('Ticket - Action KB Icon Clicked');
      top.Ts.Services.Tickets.SetActionKb(action.RefID, !action.IsKnowledgeBase,
    function (result) {
      var parentLI = self.closest('li');
      parentLI.data().action.IsKnowledgeBase = result;
      parentLI.find('a.ticket-action-kb').toggleClass('hidden');
    }, function () {
      alert('There was an error editing this action.');
    });
    }
  });

  $('#action-timeline').on('click', 'a.action-option-visible', function (e) {
    e.preventDefault();
    e.stopPropagation();

    var self = $(this);
    var action = self.closest('li').data().action;

    if (top.Ts.System.User.ChangeTicketVisibility || top.Ts.System.User.IsSystemAdmin) {
      top.Ts.System.logAction('Ticket - Action Visible Icon Clicked');
      top.Ts.Services.Tickets.SetActionPortal(action.RefID, !action.IsVisibleOnPortal,
      function (result) {
        var parentLI = self.closest('li');
        parentLI.data().action.IsVisibleOnPortal = result;
        var badgeDiv = parentLI.find('div.ticket-badge');
        badgeDiv.empty();

        if (result) {
          badgeDiv.html('<div class="bgcolor-green"><span class="bgcolor-green">&nbsp;</span><a href="#" class="action-option-visible">Public</a></div>');
        }
        else {
          badgeDiv.html('<div class="bgcolor-orange"><span class="bgcolor-orange">&nbsp;</span><a href="#" class="action-option-visible">Private</a></div>');
        }

        top.Ts.Services.Tickets.GetAction(action.RefID, function (action) {
          parentLI.find('div.timeline-body').html(action.Description);
        });

      }, function () {
        alert('There was an error editing this action.');
      });
    }
  });

  $('#action-timeline').on('click', 'a.action-option-edit', function (e) {
    e.preventDefault();
    e.stopPropagation();

    var self = $(this);
    var action = self.closest('li').data().action;

    var editor = $('#action-new-editor');
    SetupActionEditor(editor, action);
    SetupActionTypeSelect();



    //FlipNewActionBadge(false);
    //_isNewActionPrivate = false;
    $('#action-save-alert').text('').hide();

  });

  $('#action-timeline').on('click', 'a.action-option-delete', function (e) {
    e.preventDefault();
    e.stopPropagation();

    var self = $(this);
    var action = self.closest('li').data().action;

    if (confirm('Are you sure you would like to delete this action?')) {
      top.Ts.System.logAction('Ticket - Action Deleted');
      top.Ts.Services.Tickets.DeleteAction(action.RefID, function () {
        self.closest('li').remove();
        window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "deleteaction", userFullName);
      },
      function () { alert('There was an error deleting this action.'); });
    }

  });

  $('.frame-container').bind('scroll', function () {
    if ($(this).scrollTop() > 100) {
      $('.scrollup').fadeIn();
    } else {
      $('.scrollup').fadeOut();
    }

    if (_isLoading == true) return;
    if ($('.results-done').is(':visible')) return;

    if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
      var count = $('#action-timeline > li').length;
      FetchTimeLineItems($('#action-timeline > li').length - 1);
    }
  });

  $('.scrollup').click(function () {
    $('.frame-container').animate({
      scrollTop: 0
    }, 600);
    return false;
  });

  $('.ticket-filter-public').click(function (e) {
    e.preventDefault();
    e.stopPropagation();

    var isVisible = $(this).data('visible');

    if (isVisible) {
      $('li > div.timeline-panel > div.ticket-badge > div.bgcolor-green').closest('li').hide();
      $('.filter-public').addClass('bgcolor-darkgray');
      $('.filter-public').removeClass('bgcolor-green');
    }
    else {
      $('li > div.timeline-panel > div.ticket-badge > div.bgcolor-green').closest('li').show();
      $('.filter-public').removeClass('bgcolor-darkgray');
      $('.filter-public').addClass('bgcolor-green');
    }

    $(this).data('visible', !isVisible)
  });

  $('.ticket-filter-private').click(function (e) {
    e.preventDefault();
    e.stopPropagation();

    var isVisible = $(this).data('visible');

    if (isVisible) {
      $('li > div.timeline-panel > div.ticket-badge > div.bgcolor-orange').closest('li').hide();
      $('.filter-private').addClass('bgcolor-darkgray');
      $('.filter-private').removeClass('bgcolor-orange');
    }
    else {
      $('li > div.timeline-panel > div.ticket-badge > div.bgcolor-orange').closest('li').show();
      $('.filter-private').removeClass('bgcolor-darkgray');
      $('.filter-private').addClass('bgcolor-orange');
    }

    $(this).data('visible', !isVisible)
  });

  $('.ticket-filter-wc').click(function (e) {
    e.preventDefault();
    e.stopPropagation();

    var isVisible = $(this).data('visible');

    if (isVisible) {
      $('li > div.timeline-panel > div.ticket-badge > div.bgcolor-blue').closest('li').hide();
      $('.filter-wc').addClass('bgcolor-darkgray');
      $('.filter-wc').removeClass('bgcolor-blue');
    }
    else {
      $('li > div.timeline-panel > div.ticket-badge > div.bgcolor-blue').closest('li').show();
      $('.filter-wc').removeClass('bgcolor-darkgray');
      $('.filter-wc').addClass('bgcolor-blue');
    }

    $(this).data('visible', !isVisible)
  });

  $('#action-timeline').on('click', '.wc-option-replyarea', function (e) {
    $(this).hide();
    $(this).parent().find('.wc-textarea').slideToggle("fast").find('textarea').focus();
  });

  $('#action-timeline').on('click', 'button.wc-textarea-send', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var self = $(this);
    var action = self.closest('li').data().action;
    var replyText = self.closest('.wc-textarea').find('textarea').val();
    if (replyText.length > 0) {
      $(this).prop('disabled', true);
      var commentinfo = new Object();
      commentinfo.Description = replyText;
      commentinfo.Attachments = new Array();
      commentinfo.ParentTicketID = action.RefID;

      commentinfo.PageType = -1;
      commentinfo.PageID = -1;

      commentinfo.Tickets = new Array();
      //$('#commentatt:first').find('.ticket-queue').find('.ticket-removable-item').each(function () {
      //    commentinfo.Tickets[commentinfo.Tickets.length] = $(this).data('Ticket');
      //});

      commentinfo.Groups = new Array();
      //$('#commentatt:first').find('.group-queue').find('.ticket-removable-item').each(function () {
      //    commentinfo.Groups[commentinfo.Groups.length] = $(this).data('Group');
      //});

      commentinfo.Products = new Array();
      //$('#commentatt:first').find('.product-queue').find('.ticket-removable-item').each(function () {
      //    commentinfo.Products[commentinfo.Products.length] = $(this).data('Product');
      //});

      commentinfo.Company = new Array();
      //$('#commentatt:first').find('.customer-queue').find('.ticket-removable-item').each(function () {
      //    commentinfo.Company[commentinfo.Company.length] = $(this).data('Company');
      //});

      commentinfo.User = new Array();
      //$('#commentatt:first').find('.user-queue').find('.ticket-removable-item').each(function () {
      //    commentinfo.User[commentinfo.User.length] = $(this).data('User');
      //});

      if (commentinfo.Tickets.length > 0) top.Ts.System.logAction('Water Cooler - Ticket Inserted');
      if (commentinfo.Groups.length > 0) top.Ts.System.logAction('Water Cooler - Group Inserted');
      if (commentinfo.Products.length > 0) top.Ts.System.logAction('Water Cooler - Product Inserted');
      if (commentinfo.Company.length > 0) top.Ts.System.logAction('Water Cooler - Company Inserted');
      if (commentinfo.User.length > 0) top.Ts.System.logAction('Water Cooler - User Inserted');

      //var attcontainer = $(this).parent().parent().find('#commentatt').find('.upload-queue div.ticket-removable-item');
      //TODO:  Getting strange error
      top.Ts.Services.WaterCooler.NewComment(top.JSON.stringify(commentinfo), function (Message) {
        var _compiledWCReplyTemplate = Handlebars.compile($("#wc-new-reply-template").html());
        var html = _compiledWCReplyTemplate(Message);
        self.closest('li').find('.timeline-wc-responses').append(html);
        self.parent().hide();
        self.parent().parent().find('.wc-option-replyarea').show();
        self.closest('.wc-textarea').find('textarea').val('');
        window.top.chatHubClient.server.newThread(Message.MessageID, top.Ts.System.User.OrganizationID);
      });
    }

  });

  $(document).click(function (e) {
    if ($(e.target).is('.wc-textarea *, .wc-option-replyarea, .wc-option-replyarea *')) return;
    $('.wc-textarea').hide();
    $('.wc-option-replyarea').show();
  });

  $('#action-timeline').on('click', 'a.wc-reply-like-link', function (e) {
    e.preventDefault();
    e.stopPropagation();

    var self = $(this);
    var hasLiked = self.data('liked');
    if (!hasLiked) {
      var messageID = self.closest('div.timeline-wc-reply').data().messageid;
      top.Ts.System.logAction('Water Cooler - Message Liked');
      top.Ts.Services.WaterCooler.AddCommentLike(messageID, function (likes) {
        var countSpan = self.find('.wc-reply-like-total');
        countSpan.html("+" + likes.length);
        self.data('liked', true);
        countSpan.next().hide();
        //TODO:  need to update signalr
        //window.top.chatHubClient.server.addLike(likes, messageID, thread.Message.MessageParent, top.Ts.System.User.OrganizationID);
      });
    };
  });

  $('.new-action-option-visible').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    if (this.text == 'Private') {
      FlipNewActionBadge(false);
    }
    else {
      FlipNewActionBadge(true);
    }

  })
};

function CreateTicketToolbarDomEvents() {
  $('#Ticket-Owner').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    SetAssignedUser(top.Ts.System.User.UserID);
    top.Ts.System.logAction('Ticket - Take Ownership');
  });

  $('#Ticket-GetUpdate').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('Ticket - Request Update');
    top.Ts.Services.TicketPage.RequestUpdate(_ticketID, function (actionInfo) {
      CreateActionElement(actionInfo, false);
      alert('An update has been requested for this ticket.');
    }, function () {
      alert('There was an error requesting an update for this ticket.');
    });
  });

  $('#Ticket-Read').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    var self = $(this);
    var isRead = self.find('i').hasClass('color-blue');
    top.Ts.Services.Tickets.SetTicketRead(_ticketID, isRead, function () {
      if (!isRead) {
        self.find('i').addClass('color-blue');
        self.attr('data-original-title', 'Mark Ticket as Read').tooltip('fixTitle');
        top.Ts.System.logAction('Ticket Grid - Mark UnRead');
      }
      else {
        self.find('i').removeClass('color-blue');
        self.attr('data-original-title', 'Mark Ticket as UnRead').tooltip('fixTitle');
        top.Ts.System.logAction('Ticket Grid - Mark Read');
      }
    });
  });

  $("#Ticket-Merge-search").autocomplete({
    minLength: 2,
    source: selectTicket,
    select: function (event, ui) {
      if (ui.item.data == _ticketID) {
        alert("Sorry, but you can not merge this ticket into itself.");
        return;
      }

      $(this).data('ticketid', ui.item.data).removeClass('ui-autocomplete-loading');
      $(this).data('ticketnumber', ui.item.id);

      try {
        top.Ts.Services.Tickets.GetTicketInfo(ui.item.id, function (info) {
          var descriptionString = info.Actions[0].Action.Description;

          if (ellipseString(info.Actions[0].Action.Description, 30).indexOf("<img src") !== -1)
            descriptionString = "This ticket starts off with an embedded/linked image. We have disabled this for the preview.";
          else if (ellipseString(info.Actions[0].Action.Description, 30).indexOf(".viewscreencast.com") !== -1)
            descriptionString = "This ticket starts off with an embedded recorde video.  We have disabled this for the preview.";
          else
            descriptionString = ellipseString(info.Actions[0].Action.Description, 30);

          var ticketPreviewName = "<div><strong>Ticket Name:</strong> " + info.Ticket.Name + "</div>";
          var ticketPreviewAssigned = "<div><strong>Ticket Assigned To:</strong> " + info.Ticket.UserName + "</div>";
          var ticketPreviewDesc = "<div><strong>Ticket Desciption Sample:</strong> " + descriptionString + "</div>";

          $('#ticketmerge-preview-details').after(ticketPreviewName + ticketPreviewAssigned + ticketPreviewDesc);
          $('#dialog-ticketmerge-preview').show();
          $('#dialog-ticketmerge-warning').show();
          $(".dialog-ticketmerge").dialog("widget").find(".ui-dialog-buttonpane").find(":button:contains('OK')").prop("disabled", false).removeClass("ui-state-disabled");
        })
      }
      catch (e) {
        alert("Sorry, there was a problem loading the information for that ticket.");
      }
    },
    position: { my: "right top", at: "right bottom", collision: "fit flip" }
  });

  $('#ticket-merge-complete').click(function (e) {
    e.preventDefault();
    if ($('#Ticket-Merge-search').val() == "") {
      alert("Please select a valid ticket to merge");
      return;
    }

    if ($('#dialog-ticketmerge-confirm').prop("checked")) {
      var winningID = $('#Ticket-Merge-search').data('ticketid');
      var winningTicketNumber = $('#Ticket-Merge-search').data('ticketnumber');
      var JSTop = top;
      //var window = window;
      top.Ts.Services.Tickets.MergeTickets(winningID, _ticketID, function (result) {
        if (result != "")
          alert(result);
        else {
          $('#MergeModal').modal('hide');
          JSTop.Ts.MainPage.closeTicketTab(_ticketNumber);
          JSTop.Ts.MainPage.openTicket(winningTicketNumber, true);
          //window.location = window.location;
          window.top.ticketSocket.server.ticketUpdate(_ticketNumber + "," + winningTicketNumber, "merge", userFullName);
        }
      });
      //top.Ts.Services.Tickets.MergeTickets(winningID, _ticketID, MergeSuccessEvent(_ticketNumber, winningTicketNumber),
      //  function () {
      //  $('#merge-error').show();
      //});
    }
    else {
      alert("You did not agree to the conditions of the merge. Please go back and check the box if you would like to merge.")
    }
  });

  $('#Ticket-Refresh').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('Ticket - Refreshed');
    top.Ts.MainPage.highlightTicketTab(_ticketNumber, false);
    window.location = window.location;
  });

  $('#Ticket-Subscribe').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    var self = $(this);
    var isSubscribed = _ticketInfo.Ticket.IsSubscribed;
    top.Ts.System.logAction('Ticket - Subscribed');
    top.Ts.Services.Tickets.SetSubscribed(_ticketID, !isSubscribed, null, function (subscribers) {
      _ticketInfo.Ticket.IsSubscribed = !isSubscribed;
      self.children().toggleClass('color-green');
      self.attr('data-original-title', (_ticketInfo.Ticket.IsSubscribed) ? 'UnSubscribe to Ticket' : 'Subscribe to Ticket').tooltip('fixTitle');
      AddSubscribers(subscribers);
    }, function () {
      alert('There was an error subscribing this ticket.');
    });
  });

  $('#Ticket-Queue').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    var self = $(this);
    var isQueued = _ticketInfo.Ticket.IsEnqueued;
    top.Ts.System.logAction('Ticket - Enqueued');
    top.Ts.System.logAction('Queued');
    top.Ts.Services.Tickets.SetQueue(_ticketID, !isQueued, null, function (queues) {
      _ticketInfo.Ticket.IsEnqueued = !isQueued;
      self.children().toggleClass('color-green');
      self.attr('data-original-title', (_ticketInfo.Ticket.IsEnqueued) ? 'Remove from your Ticket Queue' : 'Add to your Ticket Queue').tooltip('fixTitle');
      AddQueues(queues);
    }, function () {
      alert('There was an error queueing this ticket.');
    });
  });

  $('#Ticket-Flag').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    var self = $(this);
    var isFlagged = _ticketInfo.Ticket.IsFlagged;
    _ticketInfo.Ticket.IsFlagged = !isFlagged;
    self.children().toggleClass('color-red');
    self.attr('data-original-title', (_ticketInfo.Ticket.IsFlagged) ? 'UnFlag Ticket' : 'Flag Ticket').tooltip('fixTitle');
    top.Ts.System.logAction('Ticket - Flagged');
    top.Ts.Services.Tickets.SetTicketFlag(_ticketID, !isFlagged, null, function () {
    }, function () {
      alert('There was an error unflagging to this ticket.');
    });
  });

  $('#Ticket-Print').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('Ticket - Printed');
    window.open('../../../TicketPrint.aspx?ticketid=' + _ticketID, 'TSPrint' + _ticketID);
  });

  $('#Ticket-Email').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('Ticket - Emailed');
    top.Ts.Services.Tickets.EmailTicket(_ticketID, $("#ticket-email-input").val(), $("#ticket-intro-input").val(), function () {
      $('#email-success').show();
      $('#EmailModal').modal('hide');
    }, function () {
      $('#email-error').show();
    });
  });

  $('#Ticket-History').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('Ticket - Shown History');
    LoadTicketHistory();
    $('#TicketHistoryModal').modal('show')
  });
};

function SetupWCArea() {
  //search functions for the associations
  var execGetCustomer = null;
  function getCustomers(request, response) {
    if (execGetCustomer) { execGetCustomer._executor.abort(); }
    execGetCustomer = top.Ts.Services.Organizations.WCSearchOrganization(request.term, function (result) {
      response(result);
    });
  }

  var execGetUsers = null;
  function getUsers(request, response) {
    if (execGetUsers) { execGetUsers._executor.abort(); }
    execGetUsers = top.Ts.Services.Users.SearchUsers(request.term, function (result) { response(result); });
  }

  var execGetTicket = null;
  function getTicketsByTerm(request, response) {
    if (execGetTicket) { execGetTicket._executor.abort(); }
    //execGetTicket = Ts.Services.Tickets.GetTicketsByTerm(request.term, function (result) { response(result); });
    execGetTicket = top.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) {
      $('.main-quick-ticket').removeClass('ui-autocomplete-loading');
      response(result);
    });

  }

  var execGetGroups = null;
  function getGroupsByTerm(request, response) {
    if (execGetGroups) { execGetGroups._executor.abort(); }
    execGetTicket = top.Ts.Services.WaterCooler.GetGroupsByTerm(request.term, function (result) { response(result); });
  }

  var execGetProducts = null;
  function getProductByTerm(request, response) {
    if (execGetProducts) { execGetProducts._executor.abort(); }
    execGetProducts = top.Ts.Services.WaterCooler.GetProductsByTerm(request.term, function (result) { response(result); });
  }

  $('#associationSearch').attr("placeholder", "Search Tickets").val("");
  $('#associationSearch').autocomplete({
    minLength: 2, source: getTicketsByTerm, delay: 300,
    select: function (event, ui) {
      if (ui.item) {
        var isDupe;
        $('#associationQueue').find('.ticket-queue').find('.ticket-removable-item').each(function () {
          if (ui.item.id == $(this).data('Ticket')) {
            isDupe = true;
          }
        });
        if (!isDupe) {
          var bg = $('<div>')
          .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
          .appendTo($('#associationQueue').find('.ticket-queue')).data('Ticket', ui.item.id);


          $('<span>')
          .text(ellipseString(ui.item.value, 20))
          .addClass('filename')
          .appendTo(bg);

          $('<span>')
          .addClass('ui-icon ui-icon-close')
          .click(function (e) {
            e.preventDefault();
            $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
          })
          .appendTo(bg);
          this.value = "";
          return false;
        }
      }
    }
  });
  //handle the event association
  $('.addticket').click(function ()
  { itemAssociation("0"); });
  $('.adduser').click(function ()
  { itemAssociation("1"); });
  $('.addcustomer').click(function ()
  { itemAssociation("2"); });
  $('.addgroup').click(function ()
  { itemAssociation("3"); });
  $('.addproduct').click(function ()
  { itemAssociation("4"); });

  function itemAssociation(associationType) {
    var searchbox = $('#associationSearch');
    switch (associationType) {
      case "0":
        $('#searchGroup').show();
        $(".arrow-up").css('left', '7px');
        $('#associationSearch').attr("placeholder", "Search Tickets").val("");
        $('#associationSearch').autocomplete({
          minLength: 2, source: getTicketsByTerm, delay: 300,
          select: function (event, ui) {
            if (ui.item) {
              var isDupe;
              $('#associationQueue').find('.ticket-queue').find('.ticket-removable-item').each(function () {
                if (ui.item.id == $(this).data('Ticket')) {
                  isDupe = true;
                }
              });
              if (!isDupe) {
                var bg = $('<div>')
                .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                .appendTo($('#associationQueue').find('.ticket-queue')).data('Ticket', ui.item.id);


                $('<span>')
                .text(ellipseString(ui.item.value, 20))
                .addClass('filename')
                .appendTo(bg);

                $('<span>')
                .addClass('ui-icon ui-icon-close')
                .click(function (e) {
                  e.preventDefault();
                  $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                })
                .appendTo(bg);
                this.value = "";
                return false;
              }
            }
          }
        });
        break;
      case "1":
        $('#searchGroup').show();
        $(".arrow-up").css('left', '30px');
        $('#associationSearch').attr("placeholder", "Search Users").val("");
        searchbox.autocomplete({
          minLength: 3, source: getUsers, delay: 300,
          select: function (event, ui) {
            if (ui.item) {
              var isDupe;
              $('#associationQueue').find('.user-queue').find('.ticket-removable-item').each(function () {
                if (ui.item.id == $(this).data('User')) {
                  isDupe = true;
                }
              });
              if (!isDupe) {
                var bg = $('<div>')
            .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
            .appendTo($('#associationQueue').find('.user-queue')).data('User', ui.item.id);


                $('<span>')
            .text(ellipseString(ui.item.value, 20))
            .addClass('filename')
            .appendTo(bg);

                $('<span>')
            .addClass('ui-icon ui-icon-close')
            .click(function (e) {
              e.preventDefault();
              $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
            })
            .appendTo(bg);
                this.value = "";
                return false;
              }
            }
          }
        });
        break;
      case "2":
        $('#searchGroup').show();
        $(".arrow-up").css('left', '53px');
        $('#associationSearch').attr("placeholder", "Search Companies").val("");
        $('#associationSearch').autocomplete({
          minLength: 3,
          source: getCustomers,
          select: function (event, ui) {
            if (ui.item) {
              var isDupe;
              $('#associationQueue').find('.customer-queue').find('.ticket-removable-item').each(function () {
                if (ui.item.id == $(this).data('Company')) {
                  isDupe = true;
                }
              });
              if (!isDupe) {
                var bg = $('<div>')
                .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                .appendTo($('#associationQueue').find('.customer-queue')).data('Company', ui.item.id);


                $('<span>')
                .text(ellipseString(ui.item.value, 20))
                .addClass('filename')
                .appendTo(bg);

                $('<span>')
                .addClass('ui-icon ui-icon-close')
                .click(function (e) {
                  e.preventDefault();
                  $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                })
                .appendTo(bg);
                this.value = "";
                return false;
              }
            }
          }
        });
        break;
      case "3":
        $('#searchGroup').show();
        $(".arrow-up").css('left', '78px');
        $('#associationSearch').attr("placeholder", "Search Groups").val("");
        $('#associationSearch').autocomplete({
          minLength: 2,
          source: getGroupsByTerm,
          select: function (event, ui) {
            if (ui.item) {
              var isDupe;
              $('#associationQueue').find('.group-queue').find('.ticket-removable-item').each(function () {
                if (ui.item.id == $(this).data('Group')) {
                  isDupe = true;
                }
              });
              if (!isDupe) {
                var bg = $('<div>')
                .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                .appendTo($('#associationQueue').find('.group-queue')).data('Group', ui.item.id);


                $('<span>')
                .text(ellipseString(ui.item.value, 20))
                .addClass('filename')
                .appendTo(bg);

                $('<span>')
                .addClass('ui-icon ui-icon-close')
                .click(function (e) {
                  e.preventDefault();
                  $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                })
                .appendTo(bg);
                this.value = "";
                return false;
              }
            }
          }
        });
        break;
      case "4":
        $('#searchGroup').show();
        $(".arrow-up").css('left', '104px');
        $('#associationSearch').attr("placeholder", "Search Products").val("");
        $('#associationSearch').autocomplete({
          minLength: 3,
          source: getProductByTerm,
          select: function (event, ui) {
            if (ui.item) {
              var isDupe;
              $('#associationQueue').find('.product-queue').find('.ticket-removable-item').each(function () {
                if (ui.item.id == $(this).data('Product')) {
                  isDupe = true;
                }
              });
              if (!isDupe) {
                var bg = $('<div>')
                .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                .appendTo($('#associationQueue').find('.product-queue')).data('Product', ui.item.id);


                $('<span>')
                .text(ellipseString(ui.item.value, 20))
                .addClass('filename')
                .appendTo(bg);

                $('<span>')
                .addClass('ui-icon ui-icon-close')
                .click(function (e) {
                  e.preventDefault();
                  $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                })
                .appendTo(bg);
                this.value = "";
                return false;
              }
            }
          }
        });
        break;
    }
  }

  $('#newcomment').click(function (e) {
    e.preventDefault();
    e.stopPropagation();

    if ($('#inputDescription').val().length > 0) {
      $(this).prop('disabled', true);
      var commentinfo = new Object();
      commentinfo.Description = $('#inputDescription').val();
      commentinfo.Attachments = new Array();
      commentinfo.ParentTicketID = -1;

      commentinfo.PageType = 0;
      commentinfo.PageID = _ticketNumber;

      commentinfo.Tickets = new Array();
      $('#associationQueue:first').find('.ticket-queue').find('.ticket-removable-item').each(function () {
        commentinfo.Tickets[commentinfo.Tickets.length] = $(this).data('Ticket');
      });

      commentinfo.Groups = new Array();
      $('#associationQueue:first').find('.group-queue').find('.ticket-removable-item').each(function () {
        commentinfo.Groups[commentinfo.Groups.length] = $(this).data('Group');
      });

      commentinfo.Products = new Array();
      $('#associationQueue:first').find('.product-queue').find('.ticket-removable-item').each(function () {
        commentinfo.Products[commentinfo.Products.length] = $(this).data('Product');
      });

      commentinfo.Company = new Array();
      $('#associationQueue:first').find('.customer-queue').find('.ticket-removable-item').each(function () {
        commentinfo.Company[commentinfo.Company.length] = $(this).data('Company');
      });

      commentinfo.User = new Array();
      $('#associationQueue:first').find('.user-queue').find('.ticket-removable-item').each(function () {
        commentinfo.User[commentinfo.User.length] = $(this).data('User');
      });

      if (commentinfo.Tickets.length > 0) top.Ts.System.logAction('Water Cooler - Ticket Inserted');
      if (commentinfo.Groups.length > 0) top.Ts.System.logAction('Water Cooler - Group Inserted');
      if (commentinfo.Products.length > 0) top.Ts.System.logAction('Water Cooler - Product Inserted');
      if (commentinfo.Company.length > 0) top.Ts.System.logAction('Water Cooler - Company Inserted');
      if (commentinfo.User.length > 0) top.Ts.System.logAction('Water Cooler - User Inserted');

      top.Ts.Services.TicketPage.NewWCPost(top.JSON.stringify(commentinfo), function (message) {
        if ($('.wc-attachments li').length > 0) {
          $('.wc-attachments li').each(function (i, o) {
            var data = $(o).data('data');
            data.url = '../../../Upload/WaterCooler/' + message.item.RefID;
            data.jqXHR = data.submit();
            $(o).data('data', data);
          });
        }
        window.top.chatHubClient.server.newThread(message.item.RefID, top.Ts.System.User.OrganizationID);
        $('.wc-attachments').empty();
        CreateActionElement(message, false);
        $('.watercooler-new-area').fadeOut('normal');
      });
    }
  });

  var wcelement = $('.watercooler-new-area');
  $('#wc-file-upload').fileupload({
    namespace: 'new_action',
    dropZone: wcelement,
    add: function (e, data) {
      for (var i = 0; i < data.files.length; i++) {
        var item = $('<li>')
        .appendTo(wcelement.find('.wc-attachments'));

        data.context = item;
        item.data('data', data);

        var bg = $('<div class="ui-corner-all ts-color-bg-accent ticket-removable-item ulfn">')
        .appendTo(item);

        $('<span>')
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
      wcelement.find('.progress').progressbar().show();
      wcelement.find('.wc-attachments .ui-icon-close').hide();
      wcelement.find('.wc-attachments .ui-icon-cancel').show();
    },
    stop: function (e, data) {

    }
  });
}

var MergeSuccessEvent = function (_ticketNumber, winningTicketNumber) {
  $('#merge-success').show();
  $('#MergeModal').modal('hide');
  top.Ts.MainPage.closeTicketTab(_ticketNumber);
  top.Ts.MainPage.openTicket(winningTicketNumber, true);
  window.location = window.location;
  window.top.ticketSocket.server.ticketUpdate(_ticketNumber + "," + winningTicketNumber, "merge", userFullName);
};

var addUserViewing = function (userID) {
  $('#ticket-now-viewing').show();
  $('#ticket-viewing-users').empty();
  if ($('.ticket-viewer:data(ChatID=' + userID + ')').length < 1) {
    top.Ts.Services.Users.GetUser(userID, function (user) {
      var fullName = user.FirstName + " " + user.LastName;
      var viewuser = $('<a>')
              .data('ChatID', user.UserID)
              .data('Name', fullName)
              .addClass('ticket-viewer')
              .click(function () {
                window.parent.openChat($(this).data('Name'), $(this).data('ChatID'));
                top.Ts.System.logAction('Now Viewing - Chat Opened');
              })
              .html('<img class="user-avatar ticket-viewer-avatar" src="../../../dc/' + user.OrganizationID + '/useravatar/' + user.UserID + '/48">' + fullName + '</a>')
              .appendTo($('#ticket-viewing-users'));
    });
  }
}

var removeUserViewing = function (ticketNum, userID) {
  if (ticketNum != _ticketNumber) {
    var usersViewing = $('.ticket-viewer');
    for (i = 0; i < usersViewing.length; i++) {
      var ChatUserID = usersViewing.data('ChatID');
      if (ChatUserID == userID) {
        usersViewing[i].remove();
      }
    }

    if ($('.ticket-viewer').length < 1) {
      $('#ticket-now-viewing').hide();
    }
  }
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
    selectize.addItem(KnowledgeBaseCategoryID, true);
  }
  else {
    $('#ticket-KB-Category-RO').text(_ticketInfo.Ticket.KnowledgeBaseCategoryName);
  }
}

var SetCommunityCategory = function (ForumCategory) {
  var selectField = $('#ticket-Community');
  if (selectField.length > 0) {
    var selectize = $('#ticket-Community')[0].selectize;
    selectize.addItem(ForumCategory, true);
  }
  else {
    $('#ticket-Community-RO').text((_ticketInfo.Ticket.CategoryName == null ? 'Unassigned' : _ticketInfo.Ticket.CategoryDisplayString));
  }
}

var SetDueDate = function (duedate) {
  $('.ticket-duedate-anchor').text((duedate === null ? '' : duedate.localeFormat(top.Ts.Utils.getDateTimePattern())));
};

var SetAssignedUser = function (ID) {
  var selectUserField = $('#ticket-assigned');
  if (selectUserField.length > 0) {
    var selectizeUserField = $('#ticket-assigned')[0].selectize;
    selectizeUserField.addItem(ID, false);
  }
}

var SetGroup = function (GroupID) {
  var selectField = $('#ticket-group');
  if (selectField.length > 0) {
    var selectize = $('#ticket-group')[0].selectize;
    selectize.addItem(GroupID, false);
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

    selectize.addItem(StatusID, false);

  }
};

var SetType = function (TypeID) {
  var selectField = $('#ticket-type');
  if (selectField.length > 0) {
    var selectize = $('#ticket-type')[0].selectize;
    selectize.addItem(TypeID, false);
  }
};

var SetSeverity = function (SeverityID) {
  var selectField = $('#ticket-severity');
  if (selectField.length > 0) {
    var selectize = $('#ticket-severity')[0].selectize;
    selectize.addItem(SeverityID, false);
  }
};

var SetProduct = function (ProductID) {
  var selectField = $('#ticket-product');
  if (selectField.length > 0) {
    var selectize = $('#ticket-product')[0].selectize;
    selectize.addItem(ProductID, false);
  }
};