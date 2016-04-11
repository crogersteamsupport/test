var dateFormat;
var _dueDate = null;
var _ticketGroupID = null;
var _ticketGroupUsers = null;
var _ticketTypeID = null;
var _parentFields = [];
var _productFamilyID = null;
var _lastTicketTypeID = null;
var _ticketID = null;
var _doClose = false;
var canEdit = top.Ts.System.User.IsSystemAdmin || top.Ts.System.User.ChangeKbVisibility;

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
var screenSharingPublisher;
var videoURL;
var tokTimer;
var defaultTemplateText = "";


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
};

var tickettimer = function () {
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

Selectize.define('no_results', function (options) {
  var self = this;

  options = $
            .extend({
              message: 'No results found.', html: function (data) {
                return ('<div class="selectize-dropdown ' + data.classNames + ' dropdown-empty-message">' + '<div class="selectize-dropdown-content" style="padding: 3px 12px">' + data.message + '</div>' + '</div>');
              }
            }, options);

  self.displayEmptyResultsMessage = function () {
    this.$empty_results_container.css('top', this.$control.outerHeight());
    this.$empty_results_container.show();
  };

  self.refreshOptions = (function () {
    var original = self.refreshOptions;

    return function () {
      original.apply(self, arguments);
      this.hasOptions ? this.$empty_results_container.hide() :
          this.displayEmptyResultsMessage();
    }
  })();

  self.onKeyDown = (function () {
    var original = self.onKeyDown;

    return function (e) {
      original.apply(self, arguments);
      if (e.keyCode === 27) {
        this.$empty_results_container.hide();
      }
    }
  })();

  self.onBlur = (function () {
    var original = self.onBlur;

    return function () {
      original.apply(self, arguments);
      this.$empty_results_container.hide();
    };
  })();

  self.setup = (function () {
    var original = self.setup;
    return function () {
      original.apply(self, arguments);
      self.$empty_results_container = $(options.html($.extend({
        classNames: self.$input.attr('class')
      }, options)));
      self.$empty_results_container.insertBefore(self.$dropdown);
      self.$empty_results_container.hide();
    };
  })();
});

$(document).ready(function () {
  apiKey = "45228242";
  SetupDescriptionEditor();
  LoadTicketPageOrder();
  
  SetupActionTimers();

  $('.page-loading').hide().next().show();
});

function LoadTicketPageOrder() {
  top.Ts.Services.TicketPage.GetTicketPageOrder("NewTicketFieldsOrder", function (order) {
    jQuery.each(order, function (i, val) { if (val.Disabled == "false") AddTicketProperty(val); });
    SetupTicketProperties();
  });
};

function AddTicketProperty(item) {
  if ($("#ticket-group-" + item.CatID).length > 0) {
    var compiledTemplate = Handlebars.compile($("#ticket-group-" + item.CatID).html());
    $('#ticket-properties-area').append(compiledTemplate);
  }
};

function UpdateTicketGroups(callback) {
    var selectizeGroup = $('#ticket-group')[0].selectize;
    selectizeGroup.clear(true);
    selectizeGroup.clearOptions();
    selectizeGroup.addOption({ value: -1, text: 'Unassigned' });

    var persistedGroup = false;
    var ticketGroupID = -1;
    if ($('#ticket-group').length && $('#ticket-group').val() !== '') {
        ticketGroupID = $('#ticket-group').val()
    }

    var groups = top.Ts.Cache.getGroups();
    if (top.Ts.System.Organization.UseProductFamilies && _productFamilyID != null) {
        for (var i = 0; i < groups.length; i++) {
            if (groups[i].ProductFamilyID == null || _productFamilyID == groups[i].ProductFamilyID) {
                selectizeGroup.addOption({ value: groups[i].GroupID, text: groups[i].Name });
                if (ticketGroupID == groups[i].GroupID) {
                    selectizeGroup.addItem(groups[i].GroupID, false);
                    persistedGroup = true;
                }
            }
        }
    }
    else {
        persistedGroup = true;
        for (var i = 0; i < groups.length; i++) {
            selectizeGroup.addOption({ value: groups[i].GroupID, text: groups[i].Name });
            if (ticketGroupID == groups[i].GroupID) {
                selectizeGroup.addItem(groups[i].GroupID, false);
            }
        }
    }
    if (!persistedGroup) {
        selectizeGroup.addItem(-1);
        callback(false);
    }
    else {
        callback(true);
    }
}

function UpdateTicketTypes(persistedGroup, callback) {
    var selectizeType = $('#ticket-type')[0].selectize;
    selectizeType.clear(true);
    selectizeType.clearOptions();

    var persistedType = false;
    var firstTypeID = 0;
    var types = top.Ts.Cache.getTicketTypes();
    if (top.Ts.System.Organization.UseProductFamilies && _productFamilyID != null) {
        for (var i = 0; i < types.length; i++) {
            if (types[i].ProductFamilyID == null || _productFamilyID == types[i].ProductFamilyID) {
                selectizeType.addOption({ value: types[i].TicketTypeID, text: types[i].Name });
                //_lastTicketTypeID = types[i].TicketTypeID;
                if (firstTypeID == 0) {
                    firstTypeID = types[i].TicketTypeID;
                }

                if (_lastTicketTypeID == types[i].TicketTypeID) {
                    selectizeType.addItem(types[i].TicketTypeID, true);
                    persistedType = true;
                }
            }
        }
    }
    else {
        persistedType = true;
        for (var i = 0; i < types.length; i++) {
            selectizeType.addOption({ value: types[i].TicketTypeID, text: types[i].Name });
            if (firstTypeID == 0) {
                firstTypeID = types[i].TicketTypeID;
            }

            if (_lastTicketTypeID == types[i].TicketTypeID) {
                selectizeType.addItem(types[i].TicketTypeID, true);
            }
        }
    }

    if (!persistedType) {
        selectizeType.addItem(firstTypeID);
    }

    callback({ Group: persistedGroup, Type: persistedType })
}

function SetupTicketProperties() {
  //Assigned To
  var users = top.Ts.Cache.getUsers();

  if ($('#ticket-assigned').length) {
    $('#ticket-assigned').selectize({
      dataAttr: 'assigned',
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true,
      render: {
        option: function (item, escape) {
          var optionlabel = item.text;
          if (item.data.InOfficeMessage) optionlabel = optionlabel + ' - ' + item.data.InOfficeMessage;

          if (item.data.IsSender && item.data.IsCreator)
            return '<div data-value="' + escape(item.value) + '" data-selectable="" class="option">' + optionlabel + ' (Sender and Creator)</div>';
          else if (item.data.IsSender)
            return '<div data-value="' + escape(item.value) + '" data-selectable="" class="option">' + optionlabel + ' (Sender)</div>';
          else if (item.data.IsCreator)
            return '<div data-value="' + escape(item.value) + '" data-selectable="" class="option">' + optionlabel + ' (Creator)</div>';
          else
            return '<div data-value="' + escape(item.value) + '" data-selectable="" class="option">' + optionlabel + '</div>';
        }
      },
      onChange: function (value) {
          top.Ts.Services.Users.GetUserGroups(value, function (groups) {
              if (groups !== null && groups.length == 1) {
                  SetGroup(groups[0].GroupID);
              }
          });
      },
    });

    var selectize = $("#ticket-assigned")[0].selectize;
    selectize.addOption({ value: -1, text: 'Unassigned', data: '' });

    for (var i = 0; i < users.length; i++) {
      selectize.addOption({ value: users[i].UserID, text: users[i].Name, data: users[i] });
    }

    selectize.setValue(top.Ts.System.User.UserID);
  }

  //Group
  var groups = top.Ts.Cache.getGroups();
  AppendSelect('#ticket-group', null, 'group', -1, 'Unassigned', false);
  for (var i = 0; i < groups.length; i++) {
    AppendSelect('#ticket-group', groups[i], 'group', groups[i].GroupID, groups[i].Name);
  }
  if ($('#ticket-group').length) {
    $('#ticket-group').selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true
    });
  }

  //Type
  var types = top.Ts.Cache.getTicketTypes();
  for (var i = 0; i < types.length; i++) {
    AppendSelect('#ticket-type', types[i], 'type', types[i].TicketTypeID, types[i].Name);
  }
  
  _lastTicketTypeID = types[0].TicketTypeID;
  if ($('#ticket-type').length) {
    $('#ticket-type').selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true
    });
    //AppendTicketTypeTemplate(_lastTicketTypeID);
  }

  $('#ticket-type').change(function (e) {
    SetupStatusField();
    showCustomFields();
    _lastTicketTypeID = $(this).val();
    AppendTicketTypeTemplate(_lastTicketTypeID);
  });

  //Status
  SetupStatusField();

  //Severity
  var severities = top.Ts.Cache.getTicketSeverities();
  for (var i = 0; i < severities.length; i++) {
    AppendSelect('#ticket-severity', severities[i], 'severity', severities[i].TicketSeverityID, severities[i].Name);
  }
  if ($('#ticket-severity').length) {
    $('#ticket-severity').selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true
    });
  }

  if (top.Ts.System.Organization.SetNewActionsVisibleToCustomers == true) {
    $('#ticket-visible').prop('checked', true);
  }

  $('#ticket-properties-area').on('click', 'span.tagRemove', function (e) {
    var tag = $(this).parent();
    tag.remove();
    if ($(tag).hasClass("OrgAnchor")) ReloadProductList();
  });

  //KB
  SetupKBFields();

  //Community
  if (top.Ts.System.Organization.UseForums == false) {
    $('#ticket-Community').closest('.form-horizontal').remove();
  }
  else SetupCommunityField();

  //DueDate
  SetupDueDateField();

  //Customer Section
  if (top.Ts.System.Organization.ProductType == top.Ts.ProductType.Express) {
    $('#ticket-Customer').closest('.form-group').remove();
  }
  else SetupCustomerSection();

  //Product Section
  if (top.Ts.System.Organization.ProductType == top.Ts.ProductType.Express || top.Ts.System.Organization.ProductType === top.Ts.ProductType.HelpDesk) {
    $('#ticket-Product').closest('.form-horizontal').remove();
    $('#ticket-Resolved').closest('.form-horizontal').remove();
    $('#ticket-Versions').closest('.form-horizontal').remove();
  }
  else SetupProductSection();

  //Inventory Section
  SetupInventorySection();

  //Tags 
  SetupTagsSection();

  //Queues Section
  SetupUserQueuesSection();

  //Subscribers Section
  SetupSubscribedUsersSection();

  //Reminders
  SetupRemindersSection();

  //Associate Tickets
  SetupAssociatedTicketsSection();

  createCustomFields();

  setInitialValue();

  top.Ts.Services.Settings.SetMoxieManagerSessionVariables();
};

function SaveTicket() {
  if ($("#recorder").length == 0) {
    isFormValid(function (isValid) {
      if (isValid == true) {
        var info = new Object();
        info.Name = $('#ticket-title-input').val();
        info.TicketTypeID = ($('#ticket-type').length) ? $('#ticket-type').val() : '-1';//$('#ticket-type').val();
        info.TicketStatusID = ($('#ticket-status').length) ? $('#ticket-status').val() : '-1';//$('#ticket-status').val();

        if ($('#ticket-status').length) {
          info.TicketStatusID = $('#ticket-status').val();
        }
        else {
          var statuses = top.Ts.Cache.getTicketStatuses();
          info.TicketStatusID = statuses[0].TicketStatusID;
        }
        info.TicketSeverityID = ($('#ticket-severity').length) ? $('#ticket-severity').val() : '-1';//$('#ticket-severity').val(); 
        info.UserID = ($('#ticket-assigned').length && $('#ticket-assigned').val() !== '') ? $('#ticket-assigned').val() : '-1';//($('#ticket-assigned').val() == '') ? '-1' : $('#ticket-assigned').val();
        info.GroupID = ($('#ticket-group').length && $('#ticket-group').val() !== '') ? $('#ticket-group').val() : '-1';//($('#ticket-group').val() == '') ? '-1' : $('#ticket-group').val();
        var dueDate = $('.ticket-action-form-dueDate').datetimepicker('getDate');
        info.DueDate = _dueDate;

        info.CategoryID = ($('#ticket-Category').length) ? $('#ticket-Category').val() : null;//$('#ticket-Category').val();
        info.ProductID = ($('#ticket-Product').length && $('#ticket-Product').val() !== '') ? $('#ticket-Product').val() : '-1';//($('#ticket-Product').val() == '') ? '-1' : $('#ticket-Product').val();
        info.ReportedID = ($('#ticket-Versions').length && $('#ticket-Versions').val() !== '') ? $('#ticket-Versions').val() : '-1';//($('#ticket-Versions').val() == '') ? '-1' : $('#ticket-Versions').val();
        info.ResolvedID = ($('#ticket-Resolved').length && $('#ticket-Resolved').val() !== '') ? $('#ticket-Resolved').val() : '-1';//($('#ticket-Resolved').val() == '') ? '-1' : $('#ticket-Resolved').val();
        info.IsVisibleOnPortal = ($('#ticket-visible').length) ? $('#ticket-visible').prop('checked') : false;//$('#ticket-visible').prop('checked')
        info.IsKnowledgebase = ($('#ticket-isKB').length) ? $('#ticket-isKB').prop('checked') : false;//$('#ticket-isKB').prop('checked');
        info.KnowledgeBaseCategoryID = ($('#ticket-KB-Category').length) ? $('#ticket-KB-Category').val() : '-1'; //($('#ticket-KB-Category').val() == '') ? '-1' : $('#ticket-KB-Category').val();
        info.Description = tinyMCE.activeEditor.getContent();
        info.DateStarted = top.Ts.Utils.getMsDate(moment($('#action-new-date-started').val(), dateFormat + ' hh:mm A').format('MM/DD/YYYY hh:mm A')); 

        var timeSpent = parseInt($('#action-new-hours').val()) * 60 + parseInt($('#action-new-minutes').val());
        info.TimeSpent = timeSpent;

        // Custom Values
        info.Fields = new Array();
        $('.custom-field:visible').each(function () {
          var data = $(this).data('field');
          var field = new Object();
          field.CustomFieldID = data.CustomFieldID;
          switch (data.FieldType) {
            case top.Ts.CustomFieldType.Boolean:
              field.Value = $(this).find('input').prop('checked');
              break;
            case top.Ts.CustomFieldType.PickList:
              field.Value = $(this).find('select').val();
              break;
            case top.Ts.CustomFieldType.Time:
              var text = $(this).find('a').text();
              var value = top.Ts.Utils.getMsDate("1/1/1900 " + text);
              field.Value = text == null ? null : value.toUTCString();
              break;
            case top.Ts.CustomFieldType.Date:
            case top.Ts.CustomFieldType.DateTime:
              var text = $(this).find('a').text();
              var value = top.Ts.Utils.getMsDate(text);
              field.Value = text == null ? null : value.toUTCString();
              break;
            default:
              field.Value = $(this).find('input').val();
          }
          info.Fields[info.Fields.length] = field;
        });

        // Associated Tickets
        info.ChildTickets = new Array();
        info.RelatedTickets = new Array();
        $('#ticket-AssociatedTickets > div.tag-item').each(function () {
          var data = $(this).data('tag');
          var IsParent = $(this).data('IsParent');
          if (IsParent === null)
          {
            info.RelatedTickets[info.RelatedTickets.length] = data.TicketID
          }
          else if (IsParent)
          {
            info.ParentTicketID = data.TicketID;
          }
          else {
            info.ChildTickets[info.ChildTickets.length] = data.TicketID
          }
        });

        //Tags
        info.Tags = new Array();
        $('#ticket-tags > div.tag-item').each(function () {
          var data = $(this).data('tag');
          info.Tags[info.Tags.length] = data.value;

        });

        //Subscribers
        info.Subscribers = new Array();
        $('#ticket-SubscribedUsers > div.tag-item').each(function () {
          info.Subscribers[info.Subscribers.length] = $(this).attr('id');
        });

        //Reminders
        info.Reminders = new Array();
        $('#ticket-Reminders > div.tag-item').each(function () {
          info.Reminders[info.Reminders.length] = $(this).attr('id');
        });

        //Queues
        info.Queuers = new Array();
        $('#ticket-UserQueue > div.tag-item').each(function () {
          info.Queuers[info.Queuers.length] = $(this).attr('id');
        });

        //Inventory
        info.Assets = new Array();
        $('#ticket-Inventory > div.tag-item').each(function () {
          info.Assets[info.Assets.length] = $(this).attr('id');
        });

        //Customers
        info.Customers = new Array();
        info.Contacts = new Array();

        $('#ticket-Customer > div.tag-item').each(function () {
          var data = $(this).data('tag');
          if (data.UserID)
          {
            info.Contacts[info.Contacts.length] = data.UserID;
          }
          else
          {
            info.Customers[info.Customers.length] = data.OrganizationID;
          }
        });

        //var chatID = top.Ts.Utils.getQueryValue('chatid', window)
        //if (chatID && chatID != null) {
        //  top.Ts.Services.Tickets.GetChatCustomer(chatID, function (result) {
        //    AddCustomers(result);
        //  });
        //}

        var chatID = top.Ts.Utils.getQueryValue('chatid', window)
        if (chatID && chatID != null) {
          info.ChatID = chatID;
        }
        
        top.Ts.Services.Tickets.NewTicket(top.JSON.stringify(info), function (result) {
          if (result == null) {
            alert('There was an error saving your ticket.  Please try again.');
            $('.new-ticket-save-buttons').removeClass('saving');
            return;
          }
          _ticketID = result[0];
          
          top.Ts.System.logAction('Ticket Created');
          if ($('.upload-queue li').length > 0) {
            $('.upload-queue li').each(function (i, o) {
              var data = $(o).data('data');
              data.url = '../../../Upload/Actions/' + result[1];
              data.jqXHR = data.submit();
              $(o).data('data', data);
            });
          }
          else {
          	top.Ts.Services.TicketPage.CheckContactEmails(_ticketID, function (isInvalid) {
          		if (!isInvalid)
          			alert("At least one of the contacts associated with this ticket does not have an email address defined or is inactive, and will not receive any emails about this ticket.");

          		if (_doClose != true) top.Ts.MainPage.openTicketByID(result[0]);
          		top.Ts.MainPage.closeNewTicketTab();
          	});

          }
        });
      }
      else {
        $('#ticket-create').prop('disabled', false);
        $('#ticket-createandclose').prop('disabled', false);
      }
    });
  }
};

function isFormValid(callback) {
  $('#newticket-create-errors').empty();
  top.Ts.Services.Organizations.IsProductRequired(function (isProductRequired) {
    top.Ts.Services.Organizations.IsProductVersionRequired(function (isProductVersionRequired) {
      top.Ts.Settings.Organization.read('RequireNewTicketCustomer', false, function (requireNewTicketCustomer) {
        var result = true;
        var product = $('#ticket-Product');
        var reportversion = $('#ticket-Versions');
        var productID = product.val();
        var reportversionID = reportversion.val();
        var status = top.Ts.Cache.getTicketStatus($('#ticket-status').val());
        
        //Check if we need a product
        product.closest('.form-group').removeClass('hasError');
        if (isProductRequired && (productID == -1 || productID == ""))
        {
          product.closest('.form-group').addClass('hasError');
          InsertCreateError("Product is a required field.");
          result = false;
        }

        //See if we need a version
        reportversion.closest('.form-group').removeClass('hasError');
        if (isProductVersionRequired && (reportversionID == -1 || reportversionID == "")) {
          reportversion.closest('.form-group').addClass('hasError');
          InsertCreateError("Product version is a required field.");
          result = false;
        }

        //Ensure there is a name
        $('#ticket-title-input').closest('.form-group').removeClass('has-error hasError');
        if ($.trim($('#ticket-title-input').val()) == '') {
          $('#ticket-title-input').closest('.form-group').addClass('has-error hasError');
          InsertCreateError("Please enter a title above.");
          result = false;
        }
        //Check required custom fields
        var cfHasError = false;
        $('.custom-field:visible').each(function () {
          $(this).removeClass('hasError');
          var field = $(this).data('field');
          if (field.IsRequired) {
            switch (field.FieldType) {
              case top.Ts.CustomFieldType.Text:
              case top.Ts.CustomFieldType.Number:
                if ($.trim($(this).find('input').val()) == '') {
                  $(this).addClass('hasError');
                  cfHasError = true;
                  result = false;
                }
                break;
              case top.Ts.CustomFieldType.Date:
              case top.Ts.CustomFieldType.Time:
              case top.Ts.CustomFieldType.DateTime:
                var date = $.trim($(this).find('a').text());
                if (date == null || date == '' || date == 'Unassigned') {
                  $(this).addClass('hasError');
                  cfHasError = true;
                  result = false;
                }
                break;
              case top.Ts.CustomFieldType.PickList:
                if ($(this).hasClass('isEmpty')) {
                  $(this).addClass('hasError');
                  cfHasError = true;
                  result = false;
                }
                break;
              default:
            }
          }

          if (status.IsClosed) {
            if (field.IsRequiredToClose) {
              switch (field.FieldType) {
                case top.Ts.CustomFieldType.Text:
                case top.Ts.CustomFieldType.Number:
                  if ($.trim($(this).find('input').val()) == '') {
                    $(this).addClass('hasError');
                    cfHasError = true;
                    result = false;
                  }
                  break;
                case top.Ts.CustomFieldType.Date:
                case top.Ts.CustomFieldType.Time:
                case top.Ts.CustomFieldType.DateTime:
                  var date = $.trim($(this).find('a').text());
                  if (date == null || date == '' || date == 'Unassigned') {
                    $(this).addClass('hasError');
                    cfHasError = true;
                    result = false;
                  }
                  break;
                case top.Ts.CustomFieldType.PickList:
                  if ($(this).hasClass('isEmpty')) {
                    $(this).addClass('hasError');
                    cfHasError = true;
                    result = false;
                  }
                  break;
                default:
              }
            }
          }
        });

        if ($('#ticket-Customer > .tag-error').length > 0) {
        	InsertCreateError("An inactive contact can not be added to the ticket");
        	result = false;
        }

        if (cfHasError) { InsertCreateError("Please fill in the red required custom fields."); }
        
        //If custom required check if the ticket is a KB if not then see if we have at least one customer
        if (requireNewTicketCustomer == "True" && $('#ticket-isKB').is(":checked") == false)
        { 
          if($('#ticket-Customer > div.tag-item').length < 1)
          {
            $('#ticket-Customer').closest('.form-group').addClass('hasError');
            InsertCreateError("A customer is required to create a ticket.")
            result = false;
          }
          else
          {
            $('#ticket-Customer').closest('.form-group').removeClass('hasError');
          }
        }

        callback(result);
      });
    });
  });
};

function InsertCreateError(message) {
  var alert = $('<div>').addClass('alert alert-danger').text(message).appendTo($('#newticket-create-errors'));
}

function SetupDescriptionEditor() {
  initEditor($('#ticket-description'), true, function (ed) {
    AppendTicketTypeTemplate(_lastTicketTypeID);

    SetupUploadQueue();

    $('#ticket-create').click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      $(this).prop('disabled', true);
      _doClose = false;
      SaveTicket();
    });

    $('#ticket-createandclose').click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      $(this).prop('disabled', true);
      _doClose = true;
      SaveTicket();
    });

    $('#ticket-cancel').click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      if (confirm('Are you sure you would like to cancel this ticket?')) {
        clearTimeout(_timerid);
        _timerElapsed = 0;
        counter = 0;
        top.Ts.MainPage.closeNewTicketTab();
      }
      top.Ts.System.logAction('New Ticket - Canceled');
      $('#recorder').remove();
    });

    $('#rcdtokScreen').click(function (e) {
    	top.Ts.Services.Tickets.StartArchiving(sessionId, function (resultID) {
    		$('#rcdtokScreen').hide();
    		$('#stoptokScreen').show();
    		$('#deletetokScreen').hide();
    		$('#muteTokScreen').show();
    		recordingID = resultID;
    		$('#tokScreenCountdown').show();
    		setTimeout(function () {
    			update();
    		}, 1000);
    		//countdown("tokScreenCountdown", 5, 0, element);
    		//recordScreenTimer = setTimeout(function () { StopRecording(element); }, 300000);
    		$('#statusTextScreen').text("Currently Recording Screen...");
    	});
    });

    $('#muteTokScreen').hide();
    $('#muteTokScreen').click(function (e) {
    	publisher.publishAudio(false);
    	$('#unmuteTokScreen').show();
    	$('#muteTokScreen').hide();
    });

    $('#unmuteTokScreen').hide();
    $('#unmuteTokScreen').click(function (e) {
    	publisher.publishAudio(true);
    	$('#muteTokScreen').show();
    	$('#unmuteTokScreen').hide();
    });

    $('#stoptokScreen').hide();
    $('#stoptokScreen').click(function (e) {
    	$('#statusTextScreen').text("Processing...");
    	clearTimeout(tokTimer);
    	$("#tokScreenCountdown").html("0:00");
    	top.Ts.Services.Tickets.StopArchiving(recordingID, function (result) {
    		$('#tokScreenCountdown').hide();
    		$('#rcdtokScreen').show();
    		$('#stoptokScreen').hide();
    		$('#canceltokScreen').show();
    		$('#unmuteTokScreen').hide();
    		$('#muteTokScreen').hide();
    		tokurl = result;
    		videoURL = '<video controls poster="' + top.Ts.System.AppDomain + '/dc/1078/images/static/videoview1.jpg"><source src="' + tokurl + '" type="video/mp4"><a href="' + tokurl + '">Please click here to view the video.</a></video>';
    		if (top.Ts.System.User.OrganizationID !== 13679) {
    			tinyMCE.activeEditor.execCommand('mceInsertContent', false, '<br/><br/>' + videoURL);
    		}
    		else {
    			$('#action-new-editor').summernote('insertNode', videoURL);
    		}
    		$('#statusTextScreen').text("Recording Stopped");
    		session.unpublish(screenSharingPublisher);
    		session.unpublish(publisher);

    	});
    });

    $('#canceltokScreen').click(function (e) {
    	$('#statusTextScreen').text("");
    	clearTimeout(tokTimer);
    	$("#tokScreenCountdown").html("0:00");
    	if (recordingID != null) {
    		top.Ts.Services.Tickets.StopArchiving(recordingID, function (result) {
    			$('#tokScreenCountdown').hide();
    			$('#rcdtokScreen').show();
    			$('#stoptokScreen').hide();
    			$('#canceltokScreen').show();
    			$('#unmuteTokScreen').hide();
    			$('#muteTokScreen').hide();
    			recordingID = null;
    		});
    	}
    	session.unpublish(screenSharingPublisher);
    	session.unpublish(publisher);
    	$('#recordScreenContainer').hide();
    });

    $('#rcdtok').click(function (e) {
        top.Ts.System.logAction('Ticket - Video Recording Start Clicked');
        top.Ts.Services.Tickets.StartArchiving(sessionId, function (resultID) {
            $('#rcdtok').hide();
            $('#stoptok').show();
            $('#inserttok').hide();
            $('#deletetok').hide();
            recordingID = resultID;
            $('#statusText').text("Currently Recording ...");
        });
    });

    $('#stoptok').hide();

    $('#stoptok').click(function (e) {
        top.Ts.System.logAction('Ticket - Video Recording Stop Clicked');
        $('#statusText').text("Processing...");
        top.Ts.Services.Tickets.StopArchiving(recordingID, function (resultID) {
            $('#rcdtok').show();
            $('#stoptok').hide();
            $('#inserttok').show();
            $('#canceltok').show();
            tokurl = resultID;
            $('#statusText').text("Recording Stopped");
        });
    });

    $('#inserttok').hide();

    $('#inserttok').click(function (e) {
    	top.Ts.System.logAction('Ticket - Video Recording Insert Clicked');
    	tinyMCE.activeEditor.execCommand('mceInsertContent', false, '<br/><br/><video width="400" height="400" controls poster="' + top.Ts.System.AppDomain + '/dc/1078/images/static/videoview1.jpg"><source src="' + tokurl + '" type="video/mp4"><a href="' + tokurl + '">Please click here to view the video.</a></video>');
    		session.unpublish(publisher);
    		$('#rcdtok').show();
    		$('#stoptok').hide();
    		$('#inserttok').hide();
    		$('#recordVideoContainer').hide();
    		$('#statusText').text("");
    });

    $('#deletetok').hide();

    $('#canceltok').click(function (e) {
        top.Ts.System.logAction('Ticket - Video Recording Cancel Clicked');
        if (recordingID) {
            $('#statusText').text("Cancelling Recording ...");
            top.Ts.Services.Tickets.DeleteArchive(recordingID, function (resultID) {
                $('#rcdtok').show();
                $('#stoptok').hide();
                $('#inserttok').hide();
                session.unpublish(publisher);
                $('#recordVideoContainer').hide();
                $('#statusText').text("");
            });
        }
        else {
            session.unpublish(publisher);
            $('#recordVideoContainer').hide();
        }
        $('#statusText').text("");
    });
    $('#recordVideoContainer').hide();

  },
  function (ed) {
    $('#ticket-title-input').focus();
  });
};

function StopRecording() {
	$('#statusTextScreen').text("Processing...");
	clearTimeout(tokTimer);
	$("#tokScreenCountdown").html("0:00");
	top.Ts.Services.Tickets.StopArchiving(recordingID, function (result) {
		$('#statusText').text("");
		$('#tokScreenCountdown').hide();
		$('#rcdtokScreen').show();
		$('#stoptokScreen').hide();
		$('#canceltokScreen').show();
		$('#unmuteTokScreen').hide();
		$('#muteTokScreen').hide();
		tokurl = result;
		videoURL = '<video controls poster="' + top.Ts.System.AppDomain + '/dc/1078/images/static/videoview1.jpg"><source src="' + tokurl + '" type="video/mp4"><a href="' + tokurl + '">Please click here to view the video.</a></video>';

		if (top.Ts.System.User.OrganizationID !== 13679) {
			tinyMCE.activeEditor.execCommand('mceInsertContent', false, '<br/><br/>' + videoURL);
		}
		else {
			$('#action-new-editor').summernote('insertNode', videoURL);
		}

		$('#statusTextScreen').text("");
		session.unpublish(screenSharingPublisher);
		session.unpublish(publisher);

	});
}

function update() {
	var myTime = $("#tokScreenCountdown").html();
	var ss = myTime.split(":");
	var dt = new Date();
	dt.setHours(0);
	dt.setMinutes(ss[0]);
	dt.setSeconds(ss[1]);

	var dt2 = new Date(dt.valueOf() + 1000);
	var temp = dt2.toTimeString().split(" ");
	var ts = temp[0].split(":");

	if (temp[0] == "05") {
		StopRecording();
		return;
	}

	$("#tokScreenCountdown").html(ts[1] + ":" + ts[2]);
	tokTimer = setTimeout(function () {
		update();
	}, 1000);
}

function SetupUploadQueue() {
  var element = $('.upload-area');
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
      $('.progress').progressbar('value', 100);

      if (_doClose != true) top.Ts.MainPage.openTicketByID(_ticketID);
      top.Ts.MainPage.closeNewTicketTab();
    }
  });
}

function AppendSelect(parent, data, type, id, name, isSelected) {
  var option = $('<option>').val(id).text(name).appendTo(parent).data(type, data);
  if (isSelected) {
    option.attr('selected', 'selected');
  }
};

function SetupStatusField() {
  if ($('#ticket-status').length) {
    $("#ticket-status").selectize({
      closeAfterSelect: true,
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

    var statuses = top.Ts.Cache.getTicketStatuses();
    var ticketTypeID = $('#ticket-type').val();

    var flag = true;
    for (var i = 0; i < statuses.length; i++) {
      if (statuses[i].TicketTypeID == ticketTypeID) {
        selectize.addOption({ value: statuses[i].TicketStatusID, text: statuses[i].Name, data: statuses[i] });
        if (flag) {
          selectize.addItem(statuses[i].TicketStatusID);
          flag = false;
        }
      }
    }
  }
}

function SetupKBFields() {
  if (canEdit) {
    var categories = top.Ts.Cache.getKnowledgeBaseCategories();
    for (var i = 0; i < categories.length; i++) {
      var cat = categories[i].Category;
      AppendSelect('#ticket-KB-Category', cat, 'category', cat.CategoryID, cat.CategoryName);

      for (var j = 0; j < categories[i].Subcategories.length; j++) {
        var subcat = categories[i].Subcategories[j];
        AppendSelect('#ticket-KB-Category', subcat, 'subcategory', subcat.CategoryID, cat.CategoryName + ' -> ' + subcat.CategoryName);
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
    $('#ticket-isKB').closest('.form-horizontal').remove();
    $('#ticket-group-KBCat').parent().remove();
  }
}

function SetupCommunityField() {
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
      if ($('#ticket-Community').length) {
        $('#ticket-Community').selectize({
          onDropdownClose: function ($dropdown) {
            $($dropdown).prev().find('input').blur();
          },
          allowEmptyOption: true,
          closeAfterSelect: true
        });
      }
    }
    else {
      $('#ticket-Community').closest('.form-horizontal').remove();
    }
  }
  else {
    $('#ticket-Community').closest('.form-horizontal').remove();
  }
};

var SetupDueDateField = function (duedate) {
  var dateContainer = $('#ticket-duedate-container');
  var dateLink = $('<a>')
                      .attr('href', '#')
                      //.text((duedate === null ? '' : duedate.localeFormat(top.Ts.Utils.getDateTimePattern())))
                      .addClass('control-label ticket-anchor ticket-nullable-link ticket-duedate-anchor')
                      .appendTo(dateContainer);
  if (duedate !== undefined ) {
    dateLink.text(duedate.localeFormat(top.Ts.Utils.getDateTimePattern()));
  }

  dateLink.click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    var header = $(this).hide();
    var container = $('<div>')
          .addClass('row')
          .insertAfter(header);

    var container1 = $('<div style="padding-right:0px;">')
        .addClass('col-xs-10')
      .appendTo(container);

    var theinput = $('<input type="text">')
      .addClass('form-control')
      .val(duedate === undefined ? '' : duedate.localeFormat(top.Ts.Utils.getDateTimePattern()))
      .datetimepicker({ pickTime: true })
      .appendTo(container1)
      .focus();

    $('<i>')
      .addClass('col-xs-1 fa fa-times')
      .click(function (e) {
        $(this).closest('div').remove();
        header.show();
      })
      .insertAfter(container1);


    $('<i>')
      .addClass('col-xs-1 fa fa-check')
      .click(function (e) {
        var currDate = $(this).prev().find('input').val();
        var value = '';
        if (currDate !== '') {
          value = top.Ts.Utils.getMsDate(currDate);
        }
        _dueDate = value;
        duedate = value;
              theinput.blur().remove();
              dateLink.text((value === '' ? '' : value.localeFormat(top.Ts.Utils.getDateTimePattern()))).show();

        $(this).closest('div').remove();
        header.show();
      })
      .insertAfter(container1);
  });
}

function AddTags(tag) {
  var tagDiv = $("#ticket-tags");
  $("#ticket-tag-Input").val('');
  PrependTag(tagDiv, tag.id, tag.value, tag);
}

function SetupTagsSection() {
  if ($("#ticket-tag-Input").length) {
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
        $(this).data('item', ui.item)
        AddTags(ui.item);
        this.removeItem(ui.item.value, true);
        top.Ts.System.logAction('Ticket - Added');
      }
    })
    .data("autocomplete")._renderItem = function (ul, item) {
      return $("<li>")
          .append("<a>" + item.label + "</a>")
          .appendTo(ul);
    };
  }
};

function PrependTag(parent, id, value, data, cssclass) {
  if (cssclass === undefined) cssclass = 'tag-item';
  var _compiledTagTemplate = Handlebars.compile($("#ticket-tag").html());
  var tagHTML = _compiledTagTemplate({ id: id, value: value, data: data, css: cssclass });
  return $(tagHTML).prependTo(parent).data('tag', data);
}

function SetupCustomerSection() {
  if ($('#ticket-Customers-Input').length) {
    $('#ticket-Customers-Input').selectize({
      valueField: 'id',
      labelField: 'label',
      searchField: 'label',
      delimiter: null,
      load: function (query, callback) {
        this.clearOptions();        // clear the data
        this.renderCache = {};      // clear the html template cache
        getCustomers(query, callback)
      },
      score: function (search) {
        return function (option) {
          return 1;
        }
      },
      create: function (input, callback) {
        $('#NewCustomerModal').modal('show');
        callback(null);
      },
      plugins: {
        'sticky_placeholder': {},
        'no_results': {}
      },
      onItemAdd: function (value, $item) {
        $('#ticket-Customer').closest('.form-group').removeClass('hasError');
        AddCustomers($item.data());

        this.removeItem(value, true);
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

    $('#customer-company-input').selectize({
      valueField: 'label',
      labelField: 'label',
      searchField: 'label',
      load: function (query, callback) {
        this.clearOptions();        // clear the data
        this.renderCache = {};      // clear the html template cache
        getCompany(query, callback)
      },
      score: function (search) {
        return function (option) {
          return 1;
        }
      },
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
		create: true,
      closeAfterSelect: true,
      plugins: {
        'sticky_placeholder': {},
        'no_results': {}
      }
    });
  }
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
        var customerData = new Object();
        customerData.type = result.substring(0, 1);
        customerData.value = result.substring(1);
        AddCustomers(customerData)
      }
      else if (result.indexOf("The company you have specified is invalid") !== -1) {
        if (top.Ts.System.User.CanCreateCompany || top.Ts.System.User.IsSystemAdmin) {
          if (confirm('Unknown company, would you like to create it?')) {
            top.Ts.Services.Users.CreateNewContact(email, firstName, lastName, companyName, phone, true, function (result) {
              var customerData = new Object();
              customerData.type = result.substring(0, 1);
              customerData.value = result.substring(1);
              AddCustomers(customerData);
                $('.ticket-new-customer-email').val('');
                $('.ticket-new-customer-first').val('');
                $('.ticket-new-customer-last').val('');
                $('.ticket-new-customer-company').val('');
                $('.ticket-new-customer-phone').val('');
                $('#ticket-Customer').closest('.form-group').removeClass('hasError');
                $('#NewCustomerModal').modal('hide');
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
          $('#ticket-Customer').closest('.form-group').removeClass('hasError');
          $('#NewCustomerModal').modal('hide');
        }
      }
      else {
        alert(result);
      }
    });
  });

  top.Ts.Settings.Organization.read('RequireNewTicketCustomer', false, function (requireNewTicketCustomer) {
    if (requireNewTicketCustomer == "True" && $('#ticket-isKB').is(":checked") == false) {
      if ($('#ticket-Customer > div.tag-item').length < 1) {
        $('#ticket-Customer').closest('.form-group').addClass('hasError');
      }
      else {
        $('#ticket-Customer').closest('.form-group').removeClass('hasError');
      }
    }
  });
};

function AddCustomers(customer) {
  top.Ts.Services.Tickets.GetTicketCustomer(customer.type, customer.value, function (result) {
    var customerdata = result;
    if (customer == null) return;
    top.Ts.System.logAction('New Ticket - Customer Added');
    var customerDiv = $("#ticket-Customer");
    $("#ticket-Customers-Input").val('');

    var label = "";
    var cssClasses = "tag-item";

    if (customerdata.Flag) {
      cssClasses = cssClasses + " tag-error"
    }
    if (customerdata.Contact !== null && customerdata.Company !== null) {
      label = '<span class="UserAnchor" data-userid="' + customerdata.UserID + '" data-placement="left">' + customerdata.Contact + '</span><br/><span class="OrgAnchor" data-orgid="' + customerdata.OrganizationID + '" data-placement="left">' + customerdata.Company + '</span>';
      var newelement = PrependTag(customerDiv, customerdata.UserID, label, customerdata, cssClasses);
    }
    else if (customerdata.Contact !== null) {
      label = '<span class="UserAnchor" data-userid="' + customerdata.UserID + '" data-placement="left">' + customerdata.Contact + '</span>';
      var newelement = PrependTag(customerDiv, customerdata.UserID, label, customerdata, cssClasses);
      newelement.data('userid', customerdata.UserID).data('placement', 'left').data('ticketid', _ticketID);
    }
    else if (customerdata.Company !== null) {
      label = '<span class="OrgAnchor" data-orgid="' + customerdata.OrganizationID + '" data-placement="left">' + customerdata.Company + '</span>';
      var newelement = PrependTag(customerDiv, customerdata.OrganizationID, label, customerdata, cssClasses);
      newelement.data('orgid', customerdata.OrganizationID).data('placement', 'left').data('ticketid', _ticketID);
    }

    ReloadProductList();
    SetDefaultSupportGroup(customerdata.OrganizationID);
  });
};

function SetDefaultSupportGroup(customerID) {
  top.Ts.Services.Organizations.GetOrganization(customerID, function (customer) {
    if (customer.DefaultSupportGroupID) SetGroup(customer.DefaultSupportGroupID);
    if (customer.DefaultSupportUserID) SetAssignedUser(customer.DefaultSupportUserID);
  });
}

function SetupProductSection() {
  var products = top.Ts.Cache.getProducts();
  if ($('#ticket-Product').length) {
    $('#ticket-Product').selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      plugins: {
      	'sticky_placeholder': {},
      	'no_results': {}
      },
      allowEmptyOption: true,
      closeAfterSelect: true
    });
  }

  LoadProductList(products);

  top.Ts.Services.Organizations.IsProductRequired(function (result) {
    if (result)
      $('#ticket-Product').closest('.form-group').addClass('hasError');
    else
      $('#ticket-Product').closest('.form-group').removeClass('hasError');
  });

  $('#ticket-Product').change(function (e) {
    var self = $(this);
    var product = top.Ts.Cache.getProduct(self.val());
    loadVersions(product);
    AppendProductMatchingCustomFields();
    $('#ticket-Product').closest('.form-group').removeClass('hasError');
    if (top.Ts.System.Organization.UseProductFamilies && _productFamilyID != product.ProductFamilyID) {
        _productFamilyID = product.ProductFamilyID;
        UpdateTicketGroups(function (persistedGroup) {
            UpdateTicketTypes(persistedGroup, function (persistedData) {
                if (!persistedData.Group || !persistedData.Type) {
                    var message = 'The new product belongs to a different line. Please update ';
                    if (!persistedData.Group) {
                        message += 'Group';
                        if (!persistedData.Type) {
                            message += ' and ';
                        }
                    }
                    if (!persistedData.Type) {
                        message += 'Type';
                    }

                    alert(message += '.');
                }
            });
        });
    }
  });
};

function LoadProductList(products) {
  if ($('#ticket-Product').length) {
    if (products == null) products = top.Ts.Cache.getProducts();
    var $productselect = $('#ticket-Product').selectize();
    var $productselectInput = $productselect[0].selectize;

    var currProduct = $productselectInput.getValue();
    $productselectInput.clearOptions();

    for (var i = 0; i < products.length; i++) {
      $productselectInput.addOption({ value: products[i].ProductID, text: products[i].Name, data: products[i] });
    }

    SetupProductVersionsControl(null);
    SetProductVersionAndResolved(null, null);

    if (currProduct) {
      $productselectInput.setValue(currProduct);
    }
  }
}

function ReloadProductList() {
  top.Ts.Settings.Organization.read('ShowOnlyCustomerProducts', false, function (showOnlyCustomers) {
    if (showOnlyCustomers == "True") {
      var organizationIDs = new Array();

      $('#ticket-Customer > div.tag-item').each(function () {
        var data = $(this).data('tag');
        organizationIDs[organizationIDs.length] = data.OrganizationID;
      });

      if (organizationIDs.length < 1) {
        var products = top.Ts.Cache.getProducts();
        LoadProductList(products);
      }
      else {
        top.Ts.Services.Tickets.GetCustomerProductIDs(top.JSON.stringify(organizationIDs), function (productIDs) {
          
          if (!productIDs || productIDs == null || productIDs.length < 1) {
            var products = top.Ts.Cache.getProducts();
            LoadProductList(products);
          }
          else {
            var products = new Array();
            for (var j = 0; j < productIDs.length; j++) {
              var product = top.Ts.Cache.getProduct(productIDs[j]);
              products.push(product);
            };
            LoadProductList(products);
          }
        });
      }
    }
    //else {
    //  var products = top.Ts.Cache.getProducts();
    //  LoadProductList(products);
    //}
  });
};

function loadVersions(product) {
  if ($('#ticket-Versions').length) {
    var selectizeVersion = $("#ticket-Versions")[0].selectize;
    selectizeVersion.clear(true);
    selectizeVersion.clearOptions();
  }

  if ($('#ticket-Resolved').length) {
    var selectizeResolved = $("#ticket-Resolved")[0].selectize;
    selectizeResolved.clear(true);
    selectizeResolved.clearOptions();
  }

  if (product !== null) {
    var versions = product.Versions;

    for (var i = 0; i < versions.length; i++) {
      selectizeVersion.addOption({ value: versions[i].ProductVersionID, text: versions[i].VersionNumber, data: versions[i] });
      selectizeResolved.addOption({ value: versions[i].ProductVersionID, text: versions[i].VersionNumber, data: versions[i] });
    }
  }
}

function SetupProductVersionsControl(product) {
  if ($('#ticket-Versions').length) {
    var $select = $("#ticket-Versions").selectize({
      onItemAdd: function (value, $item) {
        var reportversion = $('#ticket-Versions');
        reportversion.closest('.form-group').removeClass('hasError');
      },
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true
    });
    var versionInput = $select[0].selectize;
  }

  if ($('#ticket-Resolved').length) {
    var $select = $("#ticket-Resolved").selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true
    });
    var resolvedInput = $select[0].selectize;
  }

  if (product !== null && product.Versions.length > 0) {
    var versions = product.Versions;
    for (var i = 0; i < versions.length; i++) {
      AppendSelect('#ticket-Versions', versions[i], 'version', versions[i].ProductVersionID, versions[i].VersionNumber, false);
      AppendSelect('#ticket-Resolved', versions[i], 'resolved', versions[i].ProductVersionID, versions[i].VersionNumber, false);
    }
  }

  top.Ts.Services.Organizations.IsProductVersionRequired(function (result) {
    if (result)
      $('#ticket-Versions').closest('.form-group').addClass('hasError');
    else
      $('#ticket-Versions').closest('.form-group').removeClass('hasError');
  });
}

function SetProductVersionAndResolved(versionId, resolvedId) {
  if ($('#ticket-Versions').length) {
    var $select = $("#ticket-Versions").selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true
    });
  }
  if ($('#ticket-Resolved').length) {
    var $select = $("#ticket-Resolved").selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true
    });
  }
};

function SetupInventorySection() {
  if ($('#ticket-Inventory-Input').length) {
    $('#ticket-Inventory-Input').selectize({
      valueField: 'id',
      labelField: 'label',
      searchField: 'label',
      load: function (query, callback) {
        this.clearOptions();        // clear the data
        this.renderCache = {};      // clear the html template cache
        getAssets(query, callback)
      },
      score: function (search) {
        return function (option) {
          return 1;
        }
      },
      onItemAdd: function (value, $item) {
        AddInventory(value);
        this.removeItem(value, true);
      },
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true,
      plugins: {
        'sticky_placeholder': {},
        'no_results': {}
      }
    });
  }
};

function AddInventory(Inventory) {
  top.Ts.Services.Assets.GetAsset(Inventory, function (asset) {
    var InventoryDiv = $("#ticket-Inventory");
    $("#ticket-Inventory-Input").val('');

    var newelement = PrependTag(InventoryDiv, asset.AssetID, ellipseString(asset.Name, 30), asset, "tag-item AssetAnchor");
    newelement.data('assetid', asset.AssetID).data('placement', 'left');
  });
};

function SetupUserQueuesSection() {
  if ($('#ticket-UserQueue-Input').length) {
    $('#ticket-UserQueue-Input').selectize({
      valueField: 'id',
      labelField: 'label',
      searchField: 'label',
      load: function (query, callback) {
        this.clearOptions();        // clear the data
        this.renderCache = {};      // clear the html template cache
        getUsers(query, callback)
      },
      score: function (search) {return function (option) {return 1;}},
      onItemAdd: function (value, $item) {
        var item = new Object();
        item.name = $item.text();
        item.id = value;
        AddQueues(item);
        this.removeItem(value, true);
      },
      plugins: {
        'sticky_placeholder': {},
        'no_results': {}
      },
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true
    });
  }
}

function AddQueues(queues) {
  var UserQueueDiv = $("#ticket-UserQueue");
  $("#ticket-UserQueue-Input").val('');

  var newelement = PrependTag(UserQueueDiv, queues.id, queues.name, queues, "tag-item UserAnchor");
  newelement.data('userid', queues.id).data('placement', 'left').data('ticketid', 0);
}

function SetupSubscribedUsersSection() {
  if ($('#ticket-SubscribedUsers-Input').length) {
    $('#ticket-SubscribedUsers-Input').selectize({
      valueField: 'id',
      labelField: 'label',
      searchField: 'label',
      load: function (query, callback) {
        this.clearOptions();        // clear the data
        this.renderCache = {};      // clear the html template cache
        getUsers(query, callback)
      },
      score: function (search) { return function (option) { return 1; } },
      onItemAdd: function (value, $item) {
        var item = new Object();
        item.name = $item.text();
        item.id = value;
        AddSubscribers(item);
        this.removeItem(value, true);
      },
      plugins: {
        'sticky_placeholder': {},
        'no_results': {}
      },
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true
    });
  }
};

function AddSubscribers(Subscribers) {
  var SubscribersDiv = $("#ticket-SubscribedUsers");
  $("#ticket-SubscribedUsers-Input").val('');

  var newelement = PrependTag(SubscribersDiv, Subscribers.id, Subscribers.name, Subscribers, "tag-item UserAnchor");
  newelement.data('userid', Subscribers.id).data('placement', 'left').data('ticketid', 0);
}

function SetupRemindersSection() {
  $('#ticket-reminder-date').datetimepicker({ useCurrent: true, format: 'MM/DD/YYYY hh:mm A', defaultDate: new Date() });

  if ($('#ticket-reminder-who').length) {
    var $reminderSelect = $('#ticket-reminder-who').selectize({
      valueField: 'id',
      labelField: 'label',
      searchField: 'label',
      load: function (query, callback) {
        this.clearOptions();        // clear the data
        this.renderCache = {};      // clear the html template cache
        top.Ts.Services.TicketPage.SearchUsers(query, function (result) {
          callback(result);
        });

      },
      score: function (search) { return function (option) { return 1; } },
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true
    });
  }

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

    var label = ellipseString(title, 30) + '<br>' + date.localeFormat(top.Ts.Utils.getDateTimePattern())
    PrependTag($("#ticket-reminder-span"), userid, label, null);
    $('#RemindersModal').modal('hide')
  });
}

function SetupAssociatedTicketsSection() {
  if ($('#ticket-AssociatedTickets-Input').length) {
    $('#ticket-AssociatedTickets-Input').selectize({
      valueField: 'data',
      labelField: 'label',
      searchField: 'label',
      loadThrottle: null,
      load: function (query, callback) {
        this.clearOptions();        // clear the data
        this.renderCache = {};      // clear the html template cache
        getRelated(query, callback)
      },
      score: function (search) {
        return function (option) {
          return 1;
        }
      },
      onItemAdd: function (value, $item) {
        $('#AssociateTicketModal').data('ticketid', value).modal('show');
        this.removeItem(value, true);
      },
      plugins: {
        'sticky_placeholder': {},
        'no_results': {}
      },
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      },
      closeAfterSelect: true
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

      $("#ticket-AssociatedTickets-Input").val('');
      AddAssociatedTickets(TicketID2, IsParent);
      $('#AssociateTicketModal').modal('hide');
    });
  }
};

function AddAssociatedTickets(ticketid, IsParent) {
  top.Ts.Services.Tickets.GetTicket(ticketid, function (ticket) {
    if (ticket !== null) {
      var AssociatedTicketsDiv = $("#ticket-AssociatedTickets");
      var caption = 'Related';
      if (IsParent !== null) {
        caption = (IsParent === true ? 'Parent' : 'Child');
      }
      var label = caption + "<br />" + ellipseString(ticket.TicketNumber + ': ' + ticket.Name, 30);

      var newelement = PrependTag(AssociatedTicketsDiv, ticket.TicketID, ticket.IsClosed ? '<s>' + label + '</s>' : label, ticket, 'tag-item TicketAnchor');
      newelement.data('ticketid', ticket.TicketID).data('placement', 'left').data('IsParent', IsParent);
    }
  });
};

function createCustomFields() {
  top.Ts.Services.CustomFields.GetParentCustomFields(top.Ts.ReferenceTypes.Tickets, null, function (result) {
    var parentContainer = $('#ticket-group-custom-fields');
    if (result === null || result.length < 1) { parentContainer.empty().hide(); return; }
    parentContainer.empty()
    parentContainer.show();
    _parentFields = [];

    for (var i = 0; i < result.length; i++) {
      if (!result[i].CustomFieldCategoryID) {
        try {
          switch (result[i].FieldType) {
            case top.Ts.CustomFieldType.Text: AddCustomFieldEdit(result[i], parentContainer); break;
            case top.Ts.CustomFieldType.Date: AddCustomFieldDate(result[i], parentContainer); break;
            case top.Ts.CustomFieldType.Time: AddCustomFieldTime(result[i], parentContainer); break;
            case top.Ts.CustomFieldType.DateTime: AddCustomFieldDateTime(result[i], parentContainer); break;
            case top.Ts.CustomFieldType.Boolean: AddCustomFieldBool(result[i], parentContainer); break;
            case top.Ts.CustomFieldType.Number: AddCustomFieldNumber(result[i], parentContainer); break;
            case top.Ts.CustomFieldType.PickList: AddCustomFieldSelect(result[i], parentContainer, true); break;
            default:
          }
        } catch (err) {
          var errorString = '1001 NewTicket.js createCustomFields   FieldType: ' + result[i].FieldType + '  CustomFieldID: ' + result[i].CustomFieldID + ' ::  Exception Properties: ';
          for (var property in err) { errorString += property + ': ' + err[property] + '; '; }
          top.Ts.Services.System.LogException(err.message, errorString);
        }
      }
    }

    appendCategorizedCustomFields(result, null);
  });
};

var appendCategorizedCustomFields = function (fields, className) {
  top.Ts.Services.CustomFields.GetAllTypesCategories(top.Ts.ReferenceTypes.Tickets, function (categories) {
    var container = $('#ticket-group-custom-fields');
    for (var j = 0; j < categories.length; j++) {
      var isFirstFieldAdded = true;
      for (var i = 0; i < fields.length; i++) {
        var item = null;

        var field = fields[i];

        if (field.CustomFieldCategoryID == categories[j].CustomFieldCategoryID) {
          if (isFirstFieldAdded) {
            isFirstFieldAdded = false;
            //TODO:  Wrap header and hr together inside a span so they can both be removed easily
            //var container = $('<div>').addClass('cf-category category-' + categories[j].CustomFieldCategoryID).appendTo(parentcontainer);
            var header = $('<label id=CFCat-' + categories[j].CustomFieldCategoryID + '>').text(categories[j].Category).addClass('customFieldCategoryHeader');
            container.append($('<hr>')).append(header);
          }

          //TODO: need container for categories.
         // var container = $('.category-' + categories[j].CustomFieldCategoryID);
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
    showCustomFields();
  });
}

function showCustomFields() {
  var ticketTypeID = $('#ticket-type').val();
  $('.custom-field').hide().each(function () {
    var field = $(this).data('field');
    if (field.AuxID == ticketTypeID) {
      $(this).show();
      if (field.CustomFieldCategoryID !== null) $('#CFCat-' + field.CustomFieldCategoryID).show();
      if (field.ParentProductID !== null) $('#CFGroupProduct-' + field.ParentProductID).show();
    }
    else {
      $(this).hide();
      if (field.CustomFieldCategoryID !== null) $('#CFCat-' + field.CustomFieldCategoryID).hide().prev().hide();
      if (field.ParentProductID !== null) $('#CFGroupProduct-' + field.ParentProductID).hide();
    }
  });
};

function SetupActionTimers() {
	if ($('#action-new-date-started').data("DateTimePicker"))
		$('#action-new-date-started').data("DateTimePicker").destroy();

	top.Ts.Services.Customers.GetDateFormat(false, function (format) {
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

		$('#action-new-date-started').datetimepicker({ format: dateFormat + ' hh:mm A', defaultDate: new Date() });
	});

	

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

var AddCustomFieldEdit = function (field, parentContainer) {
  var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
  var groupContainer = $('<div>').addClass('form-group form-group-sm custom-field')
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
    //top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _ticketID, value, function (result) {
    //  groupContainer.data('field', result);
    //  groupContainer.find('.external-link').remove();
    //  input.after(getUrls(result.Value));
    //  window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "changecustom", userFullName);
    //}, function () {
    //  alert("There was a problem saving your ticket property.");
    //});
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
  var groupContainer = $('<div>').addClass('form-group form-group-sm custom-field').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
  var dateContainer = $('<div>').addClass('col-sm-8 ticket-input-container').attr('style', 'padding-top: 3px;').appendTo(groupContainer);
  var dateLink = $('<a>')
                      .attr('href', '#')
                      .text((date === null ? 'unassigned' : date.localeFormat(top.Ts.Utils.getDatePattern())))
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
      $(this).datepicker("hide");
      input.hide();
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
  var groupContainer = $('<div>').addClass('form-group form-group-sm custom-field').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
  var dateContainer = $('<div>').addClass('col-sm-8 ticket-input-container').attr('style', 'padding-top: 3px;').appendTo(groupContainer);
  var dateLink = $('<a>')
                      .attr('href', '#')
                      .text((date === null ? 'unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern())))
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
    	$(this).datepicker("hide");
    	input.hide();
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
  var groupContainer = $('<div>').addClass('form-group form-group-sm custom-field').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
  var dateContainer = $('<div>').addClass('col-sm-8 ticket-input-container').attr('style', 'padding-top: 3px;').appendTo(groupContainer);
  var dateLink = $('<a>')
                      .attr('href', '#')
                      .text((date === null ? 'unassigned' : date.localeFormat(top.Ts.Utils.getTimePattern())))
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
      $(this).datepicker("hide");
      input.hide();
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
                          .addClass('form-group form-group-sm custom-field')
                          .data('field', field)
                          .appendTo(formcontainer)
                          .append($('<label>').addClass('col-sm-4 control-label').text(field.Name));
  var inputContainer = $('<div>').addClass('col-sm-8 ticket-input-container').appendTo(groupContainer);
  var inputDiv = $('<div>').addClass('checkbox ticket-checkbox').appendTo(inputContainer);
  var input = $('<input type="checkbox">').appendTo(inputDiv);
  var value = (field.Value === null || $.trim(field.Value) === '' || field.Value === 'False' ? false : true);
  input.attr("checked", value);
};

var AddCustomFieldNumber = function (field, parentContainer) {
  var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
  var groupContainer = $('<div>').addClass('form-group form-group-sm custom-field').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
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
  var groupContainer = $('<div>').addClass('form-group form-group-sm custom-field').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
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
  
  //appendTemplateText(options[0]);

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
      $('.' + field.CustomFieldID + 'children').remove();
      var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children form-horizontal').insertAfter(formcontainer);

      appendMatchingParentValueFields(childrenContainer, field, value);
      appendTemplateText(value);

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
};

var appendTemplateText = function (value) {
  top.Ts.Services.Tickets.GetValueTemplateText(value, function (result) {
    if (result != null && result != "" && result != "<br>") {
    	var currenttext = tinyMCE.activeEditor.getContent();
    	tinyMCE.activeEditor.setContent(currenttext + result);
    }
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

var appendMatchingParentValueFields = function (container, field, value) {
  var items = field.ListValues.split('|');
  var ticketTypeID = $('#ticket-type').val();
  if (field.AuxID == ticketTypeID && items.length > 0) {
    if (value == null) value = items[0];
    var productID = $('#ticket-Product').val();
    if (productID == undefined || productID == "") productID = "-1";
    top.Ts.Services.CustomFields.GetParentValueMatchingCustomFields(field.CustomFieldID, value, productID, function (result) {
      for (var i = 0; i < result.length; i++) {
        var field = result[i];
        var div = $('<div>').addClass('form-group form-group-sm custom-field').data('field', field);
       // $('<label>').addClass('col-sm-4 control-label select-label').text(field.Name).appendTo(div);

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
  showCustomFields();
};

function AppendProductMatchingCustomFields() {
  $('.CFProductGroup').remove();
  var productID = $('#ticket-Product').val();
  if (productID == undefined || productID == "") productID = "-1";
  top.Ts.Services.CustomFields.GetProductMatchingCustomFields(top.Ts.ReferenceTypes.Tickets, _lastTicketTypeID, productID, function (result) {
    var container = $('<div>').addClass('CFProductGroup').appendTo($('#ticket-group-custom-fields'));

    for (var i = 0; i < result.length; i++) {
      if (!result[i].CustomFieldCategoryID) {
        try {
          var field = result[i];
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
        } catch (err) {
          var errorString = '1001 NewTicket.js createCustomFields   FieldType: ' + result[i].FieldType + '  CustomFieldID: ' + result[i].CustomFieldID + ' ::  Exception Properties: ';
          for (var property in err) { errorString += property + ': ' + err[property] + '; '; }
          top.Ts.Services.System.LogException(err.message, errorString);
        }
      }
    }
    appendCategorizedCustomFields(result, null);
  });
};

function AppendTicketTypeTemplate(TicketType) {
  top.Ts.Services.Tickets.GetTicketTypeTemplateText(TicketType, function (result) {
    if (result != null && result != "" && result != "<br>") {
      if (tinyMCE.activeEditor) {
      	var currenttext = tinyMCE.activeEditor.getContent();
      	tinyMCE.activeEditor.setContent(currenttext + result);
      }
    }
  });
};

function setInitialValue() {
  var menuID = top.Ts.MainPage.MainMenu.getSelected().getId().toLowerCase();
  switch (menuID) {
    case 'mniusers':
      //top.Ts.Services.Settings.ReadUserSetting('SelectedUserID', -1, function (result) {
      //  if (result > -1) $('.newticket-user').combobox('setValue', result);
      //});
      break;
    case 'mniproducts':
      top.Ts.Services.Settings.ReadUserSetting('SelectedProductID', -1, function (productID) {
        if (productID > -1) {
          var product = top.Ts.Cache.getProduct(productID);
          SetProduct(productID)
          loadVersions(product);
          AppendProductMatchingCustomFields();
          top.Ts.Services.Organizations.IsProductRequired(function (IsRequired) {
            if (IsRequired)
              $('#ticket-Product').closest('.form-group').addClass('hasError');
            else
              $('#ticket-Product').closest('.form-group').removeClass('hasError');
          });
          if (top.Ts.System.Organization.UseProductFamilies && _productFamilyID != product.ProductFamilyID) {
              _productFamilyID = product.ProductFamilyID;
              UpdateTicketGroups(function (persistedGroup) {
                  UpdateTicketTypes(persistedGroup, function (persistedData) {
                      if (!persistedData.Group || !persistedData.Type) {
                          var message = 'The new product belongs to a different line. Please update ';
                          if (!persistedData.Group) {
                              message += 'Group';
                              if (!persistedData.Type) {
                                  message += ' and ';
                              }
                          }
                          if (!persistedData.Type) {
                              message += 'Type';
                          }

                          alert(message += '.');
                      }
                  });
              });
          }

        }
      });
      break;
    case 'mnicustomers':
      top.Ts.Services.Settings.ReadUserSetting('SelectedOrganizationID', -1, function (organizationID) {
        if (organizationID > -1) {
          top.Ts.Services.Settings.ReadUserSetting('SelectedContactID', -1, function (contactID) {
            if (contactID > -1) {
              var org = new Object();
              org.value = contactID;
              org.type = "u";
              AddCustomers(org);
            }
            else if (organizationID > -1) {
              var org = new Object();
              org.value = organizationID;
              org.type = "o";
              AddCustomers(org);
            }
          })
        }
      });
      break;
    case 'mnikb':
      if (canEdit) {
        $('#ticket-isKB').prop('checked', true);
      }
      break;
    case 'mniinventory':
      top.Ts.Services.Settings.ReadUserSetting('SelectedAssetID', -1, function (assetID) {
        if (assetID > -1) {
          AddInventory(assetID);
        }
      });
      break;
    default:
      if (menuID.indexOf('tickettype') > -1) {
        var ticketTypeID = menuID.substr(14, menuID.length - 14);
        SetType(ticketTypeID);
        showCustomFields();
        _lastTicketTypeID = ticketTypeID;
      }
  }

  var chatID = top.Ts.Utils.getQueryValue('chatid', window)
  if (chatID && chatID != null) {
    top.Ts.Services.Tickets.GetChatCustomer(chatID, function (result) {
      if (result.UserID > -1) {
        var user = new Object();
        user.value = result.UserID;
        user.type = "u";
        AddCustomers(user);
      }
      else if (result.OrganizationID > -1) {
        var org = new Object();
        org.value = result.OrganizationID;
        org.type = "o";
        AddCustomers(org);
      }
    });
  }
}

var SetType = function (TypeID) {
  var selectField = $('#ticket-type');
  if (selectField.length > 0) {
    var selectize = $('#ticket-type')[0].selectize;
    selectize.addItem(TypeID, false);
  }
};

var SetProduct = function (ProductID) {
  var selectField = $('#ticket-Product');
  if (selectField.length > 0) {
    var selectize = $('#ticket-Product')[0].selectize;
    selectize.addItem(ProductID, false);
  }
};

var SetGroup = function (GroupID) {
  var selectField = $('#ticket-group');
  if (selectField.length > 0) {
    var selectize = $('#ticket-group')[0].selectize;
    selectize.addItem(GroupID, false);
  }
};

var SetAssignedUser = function (ID) {
  var selectUserField = $('#ticket-assigned');
  if (selectUserField.length > 0) {
    var selectizeUserField = $('#ticket-assigned')[0].selectize;
    selectizeUserField.addItem(ID, false);
  }
};

function CreateHandleBarHelpers() {
  Handlebars.registerHelper('UserImageTag', function () {
    if (this.item.CreatorID !== -1) return '<img class="user-avatar pull-left" src="../../../dc/' + this.item.OrganizationID + '/useravatar/' + this.item.CreatorID + '/48" />';
    return '';
  });

  Handlebars.registerHelper('FormatDateTime', function (Date) {
    return Date.localeFormat(top.Ts.Utils.getDateTimePattern())
  });

  Handlebars.registerHelper('ActionData', function () {
    return JSON.stringify(this.item);
  });

  Handlebars.registerHelper('TagData', function () {
    return JSON.stringify(this.data);
  });

  Handlebars.registerHelper('CanKB', function (options) {
    if (top.Ts.System.User.ChangeKbVisibility || top.Ts.System.User.IsSystemAdmin) { return options.fn(this); }
  });

  Handlebars.registerHelper('CanMakeVisible', function (options) {
    if (top.Ts.System.User.ChangeTicketVisibility || top.Ts.System.User.IsSystemAdmin) { return options.fn(this); }
  });
};

