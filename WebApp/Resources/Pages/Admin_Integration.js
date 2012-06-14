/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var adminInt = null;
$(document).ready(function () {
  adminInt = new AdminInt();
  adminInt.refresh();
});

function onShow() {
  adminInt.refresh();
};


AdminInt = function () {
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
    }
  });

  $('#int-api-enable').click(function (e) {
    e.preventDefault();
    top.Ts.Services.Organizations.SetApiEnabled(true, loadApiInfo);
  });

  $('#int-api-disable').click(function (e) {
    e.preventDefault();
    top.Ts.Services.Organizations.SetApiEnabled(false, loadApiInfo);
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
      loadMaps($(this).next().show());
    }

  });

  function loadMaps(element) {
    var data = element.data('link');
    element.find('.mappings').toggle(data != undefined);
    if (data == undefined) return;

    top.Ts.Services.Organizations.GetCrmLinkFields(data.CRMLinkID, function (fields) {
      loadMapFields(element, fields);
    });

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

      $('.int-map-type').combobox({ selected: function (e, ui) {
        loadFields(element.find('.int-map-tsfield'), element.find('.int-map-type').val());
      }
      });

      loadFields(element.find('.int-map-tsfield'), element.find('.int-map-type').val());
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

    if (fields.length > 0) {
      for (var i = 0; i < fields.length; i++) {
        if (fields[i].CRMObjectName === 'Account') appendMappedField(account, fields[i]);
        else appendMappedField(contact, fields[i]);
      }
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

    parent.find('.int-map-crmfield').val('');
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
      parent.find('.int-message').append('<div>Your tokens / API keys do not match.</div>').show();
      flag = true;
    }

    if (parent.find('.int-crm-token2').val() != parent.find('.int-crm-token2-confirm').val()) {
      parent.find('.int-message').append('<div>Your CRM Authentication tokens do not match.</div>').show();
      flag = true;
    }

    if (flag) return;
    parent.find('.int-action').hide();

    var linkID = parent.data('link') == undefined ? -1 : parent.data('link').CRMLinkID;

    var crmType = '';
    var type = parent.parents('.int-type');
    if (type.hasClass('batchbook')) crmType = 'Batchbook';
    else if (type.hasClass('highrise')) crmType = 'Highrise';
    else if (type.hasClass('mailchimp')) crmType = 'MailChimp';
    else if (type.hasClass('salesforce')) crmType = 'Salesforce';
    else if (type.hasClass('zohocrm')) crmType = 'ZohoCrm';
    else if (type.hasClass('zohoreports')) crmType = 'ZohoReports';

    var crmToken = crmToken = parent.find('.int-crm-token').val();

    if (typeof parent.find('.int-crm-token2').val() != "undefined" && parent.find('.int-crm-token2').val().replace(/^\s+|\s+$/g, "") != '') {
      crmToken = crmToken + ', ' + parent.find('.int-crm-token2').val().replace(/^\s+|\s+$/g, "");
    }

    top.Ts.Services.Organizations.SaveCrmLink(
          linkID,
          parent.find('.int-crm-active').attr('checked'),
          crmType,
          (parent.find('.int-crm-password').length > 0 ? parent.find('.int-crm-password').val() : ''),
          crmToken,
          (parent.find('.int-crm-tag').length > 0 ? parent.find('.int-crm-tag').val() : ''),
          (parent.find('.int-crm-user').length > 0 ? parent.find('.int-crm-user').val() : ''),
          (parent.find('.int-crm-email').length > 0 ? parent.find('.int-crm-email').attr('checked') : false),
          (parent.find('.int-crm-portal').length > 0 ? parent.find('.int-crm-portal').attr('checked') : false),
          function (result) {
            parent.data('link', result).find('.int-message').removeClass('ui-state-error').html('Your information was saved.').show().delay(1000).fadeOut('slow');
            loadMaps(parent);
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
  $('.int-content-center input').unbind('change').unbind('keydown');

  function loadPanel(element) {
    element = $(element);
    var item = element.data('link');
    if (item == null) return;
    element.addClass('crmlinkid_' + item.CRMLinkID);
    element.find('.int-crm-user').val(item.Username);
    element.find('.int-crm-password').val(item.Password);
    element.find('.int-crm-password-confirm').val(item.Password);
    element.find('.int-crm-token').val(item.SecurityToken);
    element.find('.int-crm-token-confirm').val(item.SecurityToken);
    element.find('.int-crm-token2').val(item.SecurityToken2);
    element.find('.int-crm-token2-confirm').val(item.SecurityToken2);
    element.find('.int-crm-tag').val(item.TypeFieldMatch);
    if (item.Active) {
      element.find('.int-crm-active').attr('checked', 'checked');
    }
    else {
      element.find('.int-crm-active').attr('checked', '');
    }
    if (item.SendWelcomeEmail) {
      element.find('.int-crm-email').attr('checked', 'checked');
    }
    else {
      element.find('.int-crm-email').attr('checked', '');
    }
    if (item.AllowPortalAccess) {
      element.find('.int-crm-portal').attr('checked', 'checked');
    }
    else {
      element.find('.int-crm-portal').attr('checked', '');
    }
  }
};

function loadCrmFields(crmLinkID) { 

}


AdminInt.prototype = {
  constructor: AdminInt,
  refresh: function () {

  }
};
