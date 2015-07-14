/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />
var slaLevels;
var actionTypes;
var ticketTypes;
var adminInt = null;
$(document).ready(function () {
  adminInt = new AdminInt();
  adminInt.refresh();
});

function onShow() {
  adminInt.refresh();
};


AdminInt = function () {
  top.Ts.Services.Organizations.GetSlaLevels(function (result) {
    slaLevels = result;
  });

  actionTypes = top.Ts.Cache.getActionTypes();
  ticketTypes = top.Ts.Cache.getTicketTypes();

  $('#btnRefresh')
  .click(function (e) {
    e.preventDefault();
    window.location = window.location;
  })
  .toggle(window.location.hostname.indexOf('127.0.0.1') > -1);

  $('button').button();

  $('a').addClass('ui-state-default ts-link');
  $('.int-support').click(function (e) {
    e.preventDefault();
    top.Ts.System.openSupport();
  });

  top.Ts.Services.Organizations.GetCrmLinks(function (result) {
    for (var i = 0; i < result.length; i++) {
      loadPanel($('.' + result[i].CRMType.toLowerCase() + ' .int-panel').data('link', result[i]));
    }
  });

  $('#int-api-new').click(function (e) {
    e.preventDefault();
    if (confirm('Are you sure you would like to generate a new API token?' + '\n' + 'You will need to replace the token in all the applications that use the TeamSupport API.')) {
      top.Ts.Services.Organizations.GenerateNewApiToken(loadApiInfo);
      top.Ts.System.logAction('Admin Integration - API Token Generated');

    }
  });

  $('#int-api-enable').click(function (e) {
    e.preventDefault();
    top.Ts.Services.Organizations.SetApiEnabled(true, loadApiInfo);
    top.Ts.System.logAction('Admin Integration - API Toggled');
  });

  $('#int-api-disable').click(function (e) {
    e.preventDefault();
    top.Ts.Services.Organizations.SetApiEnabled(false, loadApiInfo);
    top.Ts.System.logAction('Admin Integration - API Toggled');
  });

  $('.int-api-link-users').click(function (e) {
    e.preventDefault();
    var info = $('.ts-api').data('info');
    if (info.IsEnabled === false) {
      if (confirm('You must have your API enabled before you can use it.' + '\n' + 'Would you like to enabled it now?')) {
        top.Ts.Services.Organizations.SetApiEnabled(true, function (result) {
          loadApiInfo(result);
          info = $('.ts-api').data('info');
          window.open($('.int-api-link-users').attr('href'), "TSAPI");
          top.Ts.System.logAction('Admin Integration - API Toggled');

        });
      }
    }
    else {
      window.open($('.int-api-link-users').attr('href'), "TSAPI");
    }
  });

  function loadApiInfo(info) {
    if (info.IsActive === true) {
      $('.ts-api').data('info', info);
      $('#int-api-active').show();
      $('#int-api-inactive').hide();
      $('#int-api-disable').toggle(info.IsEnabled);
      $('#int-api-enable').toggle(!info.IsEnabled);
      var list = $('.int-api-info').empty();
      $('<dt>').text('TeamSupport API token:').appendTo(list);
      $('<dd>').text(info.Token).appendTo(list);
      $('<dt>').text('Current daily requests:').appendTo(list);
      var current = info.RequestCount;
      if (info.RequestCount >= info.RequestMax) {
        current = '<strong class="ui-state-error">' + info.RequestCount + ' (You have reached your maximum amount of requests for the last 24 hours.)</strong>';
      }
      $('<dd>').html(current).appendTo(list);
      $('<dt>').text('Maximum daily requests:').appendTo(list);
      $('<dd>').text(info.RequestMax).appendTo(list);
      var link = 'https://' + top.Ts.System.Organization.OrganizationID + ':' + info.Token + '@app.teamsupport.com/api/xml/users';
      $('.int-api-link-users').attr('href', link).text(link);
    }
    else {
      $('#int-api-active').hide();
      $('#int-api-inactive').show();
    }
  }

  top.Ts.Services.Organizations.GetApiInfo(loadApiInfo);

  $('.int-list li').addClass('ui-widget-content ui-corner-all');

  $('.int-panel').hide();

  $('.int-list li h1.collapsable').click(function (e) {
    if ($(this).next().is(':visible')) {
      $(this).find('.ui-icon').addClass('ui-icon-triangle-1-e').removeClass('ui-icon-triangle-1-s');
      $(this).next().hide();
    }
    else {
      $(this).find('.ui-icon').addClass('ui-icon-triangle-1-s').removeClass('ui-icon-triangle-1-e');
      loadMaps($(this).next());
      loadSlaLevels($(this).next());
      loadActionTypes($(this).next());
      loadTicketTypes($(this).next());
      $(this).next().show();
    }

  });

  function loadMaps(element) {
    var data = element.data('link');
    element.find('.mappings').toggle(data != undefined);
    if (data == undefined) return;

    top.Ts.Services.Organizations.GetCrmLinkFields(data.CRMLinkID, function (fields) {
      loadMapFields(element, fields);
    });

    switch (data.CRMType) {
      case 'Salesforce':
    if ($('.int-map-type option').length < 1) {
      $('<option>')
        .text('Account')
        .attr('value', top.Ts.ReferenceTypes.Organizations)
        .attr('selected', 'selected')
        .appendTo('.int-map-type');

      $('<option>')
        .text('Contact')
        .attr('value', top.Ts.ReferenceTypes.Contacts)
        .appendTo('.int-map-type');

      $('<option>')
        .text('Ticket')
        .attr('value', top.Ts.ReferenceTypes.Tickets)
        .appendTo('.int-map-type');

      $('.int-map-type').combobox({ selected: function (e, ui) {
        loadFields(element.find('.int-map-tsfield'), element.find('.int-map-type').val());
      }
      });

      loadFields(element.find('.int-map-tsfield'), element.find('.int-map-type').val());
        }
        break;
      case 'Jira':
        if ($('.int-jira-map-tsfield option').size() == 0) {
          loadFields(element.find('.int-jira-map-tsfield'), 17);
        }
        break;
      case 'ZohoCrm':
        if ($('.int-zoho-map-type option').length < 1) {
          $('<option>')
            .text('Account')
            .attr('value', top.Ts.ReferenceTypes.Organizations)
            .attr('selected', 'selected')
            .appendTo('.int-zoho-map-type');

          $('<option>')
            .text('Contact')
            .attr('value', top.Ts.ReferenceTypes.Contacts)
            .appendTo('.int-zoho-map-type');

//          $('<option>')
//            .text('Product')
//            .attr('value', top.Ts.ReferenceTypes.Products)
//            .appendTo('.int-zoho-map-type');

          $('.int-zoho-map-type').combobox({ selected: function (e, ui) {
            loadFields(element.find('.int-zoho-map-tsfield'), element.find('.int-zoho-map-type').val());
          }
          });

        loadFields(element.find('.int-zoho-map-tsfield'), element.find('.int-zoho-map-type').val());
        }
        break;
    }
  }

  function loadFields(select, refType) {
    select.empty();
    top.Ts.Services.CustomFields.GetAllFields(refType, null, false, function (fields) {
      for (var i = 0; i < fields.length; i++) {
        $('<option>')
          .text(fields[i].Name)
          .data('field', fields[i])
          .appendTo(select);
      }

      select.combobox();
      select.combobox('setValue', fields[0].Name);
    });
  }

  function loadMapFields(element, fields) {
    var account = element.find('.map-account').empty();
    var contact = element.find('.map-contact').empty();
    var ticket = element.find('.map-ticket').empty();

    if (fields.length > 0) {
      for (var i = 0; i < fields.length; i++) {
        switch (fields[i].CRMObjectName) {
          case 'Account':
            appendMappedField(account, fields[i]);
            break;
          case 'Contact':
            appendMappedField(contact, fields[i]);
            break;
          case 'Ticket':
            appendMappedField(ticket, fields[i]);
            break;
        }
      }
    }
  }

  function loadSlaLevels(panel) {
    var slaLevelsList = panel.find('.int-defaultSla');
    if (slaLevelsList == null) return;

    panel = $(panel);
    var item = panel.data('link');

    if (slaLevels.length > 0) {
      if (slaLevels.length > 1) {
        slaLevelsList.attr('disabled', false);
      }
      else {
        slaLevelsList.attr('disabled', 'disabled');
      }
      slaLevelsList.empty();
      for (var i = 0; i < slaLevels.length; i++) {
        var selected = '">';
        if (item != null && item.DefaultSlaLevelID != null && item.DefaultSlaLevelID == slaLevels[i].SlaLevelID) {
          selected = '" selected="selected">';
        }
        slaLevelsList.append('<option value="' + slaLevels[i].SlaLevelID + selected + slaLevels[i].Name + '</option>');
      }
    }
    else {
      slaLevelsList.attr('disabled', 'disabled');
    }
  }

  function loadActionTypes(panel) {
    var actionTypesList = panel.find('.int-actionTypeToPush');
    if (actionTypesList == null) return;

    panel = $(panel);
    var item = panel.data('link');

    if (actionTypes.length > 0) {
      actionTypesList.empty();
      var nullSelected = '>';
      if (item != null && item.ActionTypeIDToPush == null) {
        nullSelected = ' selected="selected">';
      }
      actionTypesList.append('<option value=""' + nullSelected + 'All Types</option>');

      for (var i = 0; i < actionTypes.length; i++) {
        var selected = '">';
        if (item != null && item.ActionTypeIDToPush != null && item.ActionTypeIDToPush == actionTypes[i].ActionTypeID) {
          selected = '" selected="selected">';
        }
        actionTypesList.append('<option value="' + actionTypes[i].ActionTypeID + selected + actionTypes[i].Name + '</option>');
      }
    }
    else {
      actionTypesList.attr('disabled', 'disabled');
    }
  }

  function loadTicketTypes(panel) {
    var ticketTypesList = panel.find('.int-ticketTypeToPush');
    if (ticketTypesList == null) return;

    panel = $(panel);
    var item = panel.data('link');

    if (ticketTypesList.length > 0) {
      ticketTypesList.empty();

      for (var i = 0; i < ticketTypes.length; i++) {
        var selected = '">';

        if (item != null && item.RestrictedToTicketTypes != null) {
          var restrictedToTicketTypesArray = item.RestrictedToTicketTypes.split(',');
          var found = jQuery.inArray(ticketTypes[i].TicketTypeID.toString(), restrictedToTicketTypesArray);
          
          if (found > -1) {
            selected = '" selected="selected">';
          }
        }

        ticketTypesList.append('<option value="' + ticketTypes[i].TicketTypeID + selected + ticketTypes[i].Name + '</option>');
      }
    }
    else {
      ticketTypesList.attr('disabled', 'disabled');
    }
  }

  function appendMappedField(element, field) {
    var div = $('<div>')
      .addClass('map-fieldset')
      .data('field', field)
      .appendTo(element);

    $('<span>')
      .text(field.CRMFieldName)
      .appendTo(div);

    $('<span>')
      .text(' -> ')
      .appendTo(div);

    $('<span>')
      .text(field.TSFieldName)
      .appendTo(div);

    $('<span>')
      .addClass('ui-icon ui-icon-close')
      .click(function (e) {
        e.preventDefault();
        if (confirm('Are you sure you would like to delete his mapping?')) {
          top.Ts.Services.Organizations.DeleteCrmLinkField(div.data('field').CRMFieldID, function () {
            div.remove();
            top.Ts.System.logAction('Admin Integration - CRM Mapping Deleted');
          });
        }

      })
      .appendTo(div);
  }

  $('.map-add').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    var parent = $('.map-add').closest('.int-panel');
    var crmField = parent.find('.int-map-crmfield').val();
    if (crmField === '') { alert('Please enter a field value.'); return; }

    var tsField = parent.find('.int-map-tsfield option:selected').data('field');

    top.Ts.Services.Organizations.SaveCrmLinkField(
      parent.data('link').CRMLinkID,
      tsField.ID,
      tsField.IsCustom,
      crmField,
      parent.find('.int-map-type').val(),
      function (fields) {
        loadMapFields(parent, fields);
      }


    );
    top.Ts.System.logAction('Admin Integration - CRM Mapping Added');
    parent.find('.int-map-crmfield').val('');
  });

  $('.jira-map-add').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    var parent = $('.jira-map-add').closest('.int-panel');
    var crmField = parent.find('.int-jira-map-crmfield').val();
    if (crmField === '') { alert('Please enter a field value.'); return; }

    var tsField = parent.find('.int-jira-map-tsfield option:selected').data('field');

    top.Ts.Services.Organizations.SaveCrmLinkField(
      parent.data('link').CRMLinkID,
      tsField.ID,
      tsField.IsCustom,
      crmField,
      17,
      function (fields) {
        loadMapFields(parent, fields);
      }


    );
    top.Ts.System.logAction('Admin Integration - Jira Mapping Added');
    parent.find('.int-jira-map-crmfield').val('');
  });

  $('.zoho-map-add').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    var parent = $('.zoho-map-add').closest('.int-panel');
    var crmField = parent.find('.int-zoho-map-crmfield').val();
    if (crmField === '') { alert('Please enter a field value.'); return; }

    var tsField = parent.find('.int-zoho-map-tsfield option:selected').data('field');

    top.Ts.Services.Organizations.SaveCrmLinkField(
      parent.data('link').CRMLinkID,
      tsField.ID,
      tsField.IsCustom,
      crmField,
      parent.find('.int-zoho-map-type').val(),
      function (fields) {
        loadMapFields(parent, fields);
      }


    );
    top.Ts.System.logAction('Admin Integration - ZohoCRM Mapping Added');
    parent.find('.int-zoho-map-crmfield').val('');
  });


  $('.int-type').delegate('.int-save', 'click', function (e) {
    e.preventDefault();
    var parent = $(this).parents('.int-panel');

    parent.find('.int-message').addClass('ui-state-error').empty();

    var flag = false;
    if (parent.find('.int-crm-password-confirm').length > 0 && parent.find('.int-crm-password').val() != parent.find('.int-crm-password-confirm').val()) {
      parent.find('.int-message').append('<div>Your passwords do not match.</div>').show();
      flag = true;
    }

    if (parent.find('.int-crm-token-confirm').length > 0 && parent.find('.int-crm-token').val() != parent.find('.int-crm-token-confirm').val()) {
      parent.find('.int-message').append('<div>Your tokens do not match.</div>').show();
      flag = true;
    }

    if (flag) return;
    parent.find('.int-action').hide();

    var linkID = parent.data('link') == undefined ? -1 : parent.data('link').CRMLinkID;

    var crmType = '';
    var type = parent.parents('.int-type');
    if (type.hasClass('batchbook')) crmType = 'Batchbook';
    else if (type.hasClass('highrise')) crmType = 'Highrise';
    else if (type.hasClass('jira')) crmType = 'Jira';
    else if (type.hasClass('mailchimp')) crmType = 'MailChimp';
    else if (type.hasClass('oracle')) crmType = 'Oracle';
    else if (type.hasClass('salesforce')) crmType = 'Salesforce';
    else if (type.hasClass('zohocrm')) crmType = 'ZohoCrm';
    else if (type.hasClass('zohoreports')) crmType = 'ZohoReports';
    else if (type.hasClass('hubspot')) crmType = 'HubSpot';

    var crmToken = crmToken = parent.find('.int-crm-token').val();

    if (typeof crmToken == "undefined") {
      crmToken = null
    }

    var defaultSlaLevelId = parent.find('.int-defaultSla').val();

    if (typeof defaultSlaLevelId == "undefined") {
      defaultSlaLevelId = null;
    }

    var pullCasesAsTickets = parent.find('.int-crm-pull-cases-as-tickets').prop('checked');
    var pushTicketsAsCases = parent.find('.int-crm-push-tickets-as-cases').prop('checked');
    var pushTicketsAsComments = parent.find('.int-crm-push-tickets-as-account-comments').prop('checked');

    if (typeof pullCasesAsTickets == 'undefined') {
      pullCasesAsTickets = false;
    }
    if (typeof pushTicketsAsCases == 'undefined') {
      pushTicketsAsCases = false;
    }
    if (typeof pushTicketsAsComments == 'undefined') {
      pushTicketsAsComments = true;
    }

    var pullCustomerProducts = parent.find('.int-crm-pull-customer-products').prop('checked');

    if (typeof pullCustomerProducts == 'undefined') {
      pullCustomerProducts = false;
    }

    var actionTypeIDToPush = parent.find('.int-actionTypeToPush').val();

    if (typeof actionTypeIDToPush == 'undefined') {
      actionTypeIDToPush = null;
    }

    var hostName = parent.find('.int-crm-host').val();

    if (typeof hostName == 'undefined') {
      hostName = null;
    }

    var defaultProject = parent.find('.int-crm-project').val();

    if (typeof defaultProject == 'undefined') {
      defaultProject = null;
    }

    var matchAccountsByName = parent.find('.int-crm-match-accounts-by-name').prop('checked');
    if (typeof matchAccountsByName == 'undefined') {
      matchAccountsByName = true;
    }

    var useSandBoxServer = parent.find('.int-crm-use-sandbox-server').prop('checked');
    if (typeof useSandBoxServer == 'undefined') {
      useSandBoxServer = false;
    }

    var alwaysUseDefaultProjectKey = parent.find('.int-crm-always-use-default-project-key').prop('checked');
    if (typeof alwaysUseDefaultProjectKey == 'undefined') {
      alwaysUseDefaultProjectKey = false;
    }

    var useAllTicketTypes = parent.find('.int-crm-ticket-types').prop('checked');
    if (typeof useAllTicketTypes == 'undefined') {
      useAllTicketTypes = true;
    }

    var restrictedToTicketTypes = null;
    if (useAllTicketTypes) {
      restrictedToTicketTypes = null;
    }
    else {
      var restrictedTicketTypesList = parent.find('#restrictedTicketTypesList');
      if (typeof restrictedTicketTypesList == 'undefined') {
        restrictedToTicketTypes = null;
      }
      else {
        restrictedToTicketTypes = '';

        for (var x = 0; x < restrictedTicketTypesList[0].length; x++) {
          if (restrictedTicketTypesList[0][x].selected) {
            if (restrictedToTicketTypes != '') {
              restrictedToTicketTypes = restrictedToTicketTypes + ','
            }

            restrictedToTicketTypes = restrictedToTicketTypes + restrictedTicketTypesList[0][x].value;
          }
        }

        if (restrictedToTicketTypes === null || restrictedToTicketTypes == '') {
          parent.find('.int-crm-ticket-types').prop('checked', true);
          parent.find('#restrictedTicketTypesList').hide();
          restrictedToTicketTypes = null;
        }
      }
    }

    var updateTicketStatus = parent.find('.int-crm-update-type').prop('checked');
    if (typeof updateTicketStatus == 'undefined') {
      updateTicketStatus = true;
    }

    top.Ts.Services.Organizations.SaveCrmLink(
          linkID,
          parent.find('.int-crm-active').prop('checked'),
          crmType,
          (parent.find('.int-crm-password').length > 0 ? parent.find('.int-crm-password').val() : ''),
          crmToken,
          (parent.find('.int-crm-tag').length > 0 ? parent.find('.int-crm-tag').val() : ''),
          (parent.find('.int-crm-user').length > 0 ? parent.find('.int-crm-user').val() : ''),
          (parent.find('.int-crm-email').length > 0 ? parent.find('.int-crm-email').prop('checked') : false),
          (parent.find('.int-crm-portal').length > 0 ? parent.find('.int-crm-portal').prop('checked') : false),
          defaultSlaLevelId,
          pullCasesAsTickets,
          pushTicketsAsCases,
          pushTicketsAsComments,
          pullCustomerProducts,
          actionTypeIDToPush,
          hostName,
          defaultProject,
          (parent.find('.int-crm-update-status').length > 0 ? parent.find('.int-crm-update-status').prop('checked') : null),
          updateTicketStatus,
          matchAccountsByName,
          useSandBoxServer,
          alwaysUseDefaultProjectKey,
          restrictedToTicketTypes,
          function (result) {
            parent.data('link', result).find('.int-message').removeClass('ui-state-error').html('Your information was saved.').show().delay(1000).fadeOut('slow');
            loadMaps(parent);
            top.Ts.System.logAction('Admin Integration - Settings Saved');
          },
          function () {
            parent.find('.int-message').addClass('ui-state-error').html('<div>There was an error saving this information, please try again.<div>').show();
            parent.find('.int-action').show().find('.int-save').addClass('ui-state-active');
          }
        );


  });


  $('.int-type').delegate('.int-cancel', 'click', function (e) {
    e.preventDefault();
    var panel = $(this).parents('.int-panel');
    panel.find('.int-action').hide();
    panel.find('.int-message').hide();
    loadPanel(panel);
  });


  var onChange = function (e) {
    var footer = $(this).parents('.int-panel').find('.int-footer');

    if (footer.length < 1) {
      footer = $('<div>').addClass('int-footer');
      $('<div>').addClass('int-message ui-helper-hidden ui-corner-all ui-state-active').appendTo(footer);
      var actions = $('<div>').addClass('int-action').appendTo(footer);

      $('<button>')
      .text('Save')
      .addClass('int-save ui-state-active')
      .appendTo(actions)
      .button();

      $('<button>')
      .text('Cancel')
      .addClass('int-cancel')
      .appendTo(actions)
      .button();
      footer.appendTo($(this).parents('.int-panel'));
    }
    else {
      footer.find('.int-action').show();
    }
  }

  $('.int-panel input').change(onChange).keydown(onChange);
  $('.int-defaultSla').change(onChange);
  $('.int-actionTypeToPush').change(onChange);
  $('.int-content-center input').unbind('change').unbind('keydown');
  $('.int-ticketTypeToPush').change(onChange);
  
  function loadPanel(element) {
    element = $(element);
    var item = element.data('link');
    if (item == null) return;
    element.addClass('crmlinkid_' + item.CRMLinkID);
    element.find('.int-crm-host').val(item.HostName);
    element.find('.int-crm-user').val(item.Username);
    element.find('.int-crm-password').val(item.Password);
    element.find('.int-crm-password-confirm').val(item.Password);
    element.find('.int-crm-token').val(item.SecurityToken1);
    element.find('.int-crm-token-confirm').val(item.SecurityToken1);
    element.find('.int-crm-tag').val(item.TypeFieldMatch);
    element.find('.int-crm-project').val(item.DefaultProject);
    if (item.Active) {
      element.find('.int-crm-active').prop('checked', true);
    }
    else {
      element.find('.int-crm-active').prop('checked', false);
    }
    if (item.PullCasesAsTickets) {
      element.find('.int-crm-pull-cases-as-tickets').prop('checked', true);
    }
    else {
      element.find('.int-crm-pull-cases-as-tickets').prop('checked', false);
    }
    if (item.PushTicketsAsCases) {
      element.find('.int-crm-push-tickets-as-cases').prop('checked', true);
    }
    else {
      element.find('.int-crm-push-tickets-as-cases').prop('checked', false);
    }
    if (item.SendBackTicketData) {
      element.find('.int-crm-push-tickets-as-account-comments').prop('checked', true);
    }
    else {
      element.find('.int-crm-push-tickets-as-account-comments').prop('checked', false);
    }
    if (item.PullCustomerProducts) {
      element.find('.int-crm-pull-customer-products').prop('checked', true);
    }
    else {
      element.find('.int-crm-pull-customer-products').prop('checked', false);
    }
    if (item.SendWelcomeEmail) {
      element.find('.int-crm-email').prop('checked', true);
    }
    else {
      element.find('.int-crm-email').prop('checked', false);
    }
    if (item.AllowPortalAccess) {
      element.find('.int-crm-portal').prop('checked', true);
    }
    else {
      element.find('.int-crm-portal').prop('checked', false);
    }
    if (item.UpdateStatus) {
      element.find('.int-crm-update-status').prop('checked', true);
    }
    else {
      element.find('.int-crm-update-status').prop('checked', false);
    }

    element.find('.int-crm-update-type').prop('checked', item.UpdateTicketType);

    if (item.MatchAccountsByName) {
      element.find('.int-crm-match-accounts-by-name').prop('checked', true);
    }
    else {
      element.find('.int-crm-match-accounts-by-name').prop('checked', false);
    }
    if (item.UseSandBoxServer) {
      element.find('.int-crm-use-sandbox-server').prop('checked', true);
    }
    else {
      element.find('.int-crm-use-sandbox-server').prop('checked', false);
    }
    if (item.AlwaysUseDefaultProjectKey) {
      element.find('.int-crm-always-use-default-project-key').prop('checked', true);
    }
    else {
      element.find('.int-crm-always-use-default-project-key').prop('checked', false);
    }

    if (item.RestrictedToTicketTypes) {
      element.find('.int-crm-ticket-types').prop('checked', false);
      $('#restrictedTicketTypesList').show();
    }
    else {
      element.find('.int-crm-ticket-types').prop('checked', true);
      $('#restrictedTicketTypesList').hide();
    }
  }

  $('.int-crm-ticket-types').click(function (e) {
    if ($(this).prop('checked')) {
      $('#restrictedTicketTypesList').hide();
    }
    else {
      $('#restrictedTicketTypesList').show();
    }
  });

  $('.int-crm-push-tickets-as-cases').click(function (e) {
    if ($('.int-crm-push-tickets-as-cases').prop('checked')) {
      $('.int-crm-push-tickets-as-account-comments').prop('checked', false);
    }
    else {
      //$('.int-crm-push-tickets-as-account-comments').prop('checked', true);
    }
  });

  $('.int-crm-push-tickets-as-account-comments').click(function (e) {
    if ($('.int-crm-push-tickets-as-account-comments').prop('checked')) {
      $('.int-crm-push-tickets-as-cases').prop('checked', false);
    }
    else {
      //$('.int-crm-push-tickets-as-cases').prop('checked', true);
    }
  });

};

function loadCrmFields(crmLinkID) { 

}


AdminInt.prototype = {
  constructor: AdminInt,
  refresh: function () {

  }
};