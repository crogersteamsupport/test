/// <reference path="ts/ts.js" />
/// <reference path="ts/parent.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var welcomePage = null;
$(document).ready(function () {
  welcomePage = new WelcomePage();
  welcomePage.refresh();
});

function onShow() {
  welcomePage.refresh();
};


WelcomePage = function () {
  $('#btnRefresh')
  .click(function (e) {
    e.preventDefault();
    window.location = window.location;
  })
  .toggle(window.location.hostname.indexOf('127.0.0.1') > -1);

  $('button').button();

  $('a').addClass('ui-state-default ts-link');

  $('.loading-section').hide().next().show();
  $('.first-name').text(parent.Ts.System.User.FirstName);


  $('.welcome-new-user').click(function (e) {
    e.preventDefault();
      //parent.Ts.MainPage.openNewContact(parent.Ts.System.User.UserID);
    parent.Ts.MainPage.openUser(parent.Ts.System.User.UserID);
    

  });
  $('.welcome-new-customer').click(function (e) {
      e.preventDefault();
      parent.Ts.MainPage.newCustomer(null, parent.Ts.System.User.OrganizationID);
    //parent.Ts.MainPage.openNewCustomer(-1);
  });
  $('.welcome-new-ticket').click(function (e) {
    e.preventDefault();
    parent.Ts.MainPage.newTicket();
  });

  //parent.Ts.Services.Organizations.GetPortalOption(parent.Ts.System.Organization.OrganizationID, function (portalOption) { });
  //$('.welcome-view-portal').click(function (e) { });

  $('.welcome-view-portal').attr('href', parent.Ts.System.PoratlDomain + '?OrganizationID=' + parent.Ts.System.Organization.OrganizationID);

  $('.welcome-test-chat').click(function (e) {
    e.preventDefault();

    window.open(
      parent.Ts.System.AppDomain + '/Chat/ChatInit.aspx?uid=' + parent.Ts.System.Organization.ChatID,
      'TSChat',
      'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=450,height=500'
      );
  });

  $('.welcome-hide').click(function (e) {
    e.preventDefault();
    parent.Ts.MainPage.hideWelcome();
  });

  $('.welcome-payonline').click(function (e) {
    e.preventDefault();
    window.open('http://www.teamsupport.com/billing/', 'TSPayment');
  });


  var sysEmail = parent.Ts.System.Organization.SystemEmailID + '@' + parent.Ts.System.Domain;

  $('.welcome-sysemail')
    .text(sysEmail)
    .attr('href', 'mailto:' + sysEmail);

  $('.welcome-setup-email').click(function (e) {
    e.preventDefault();
    parent.Ts.MainPage.openAdmin('Email');

  });

  $('.welcome-setup-tickettypes').click(function (e) {
    e.preventDefault();
    parent.Ts.Settings.User.write('SelectedCustomPropertyValue', 5, function () {
      parent.Ts.MainPage.openAdmin('Custom Properties');
    });

  });

  $('.welcome-setup-ticketstatuses').click(function (e) {
    e.preventDefault();
    parent.Ts.Settings.User.write('SelectedCustomPropertyValue', 4, function () {
      parent.Ts.MainPage.openAdmin('Custom Properties');
    });
  });

  $('.welcome-setup-custom-fields').click(function (e) {
    e.preventDefault();
    parent.Ts.MainPage.openAdmin('Custom Fields');
  });

  $('.welcome-setup-workflow').click(function (e) {
    e.preventDefault();
    parent.Ts.MainPage.openAdmin('Workflow');
  });

  $('.welcome-setup-portals').click(function (e) {
    e.preventDefault();
    parent.Ts.MainPage.openAdmin('My Portal');
  });

  $('.welcome-help-ticket').click(function (e) {
    e.preventDefault();
    parent.Ts.System.openSupport();
  });

  $('.welcome-help-chat').click(function (e) {
    e.preventDefault();
    parent.Ts.System.logAction('Helcome - Page Hidden');
    window.open(parent.Ts.System.AppDomain + '/Chat/ChatInit.aspx?uid=22bd89b8-5162-4509-8b0d-f209a0aa6ee9', 'TSChat', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=450,height=500');
  });

  

};


WelcomePage.prototype = {
  constructor: WelcomePage,
  refresh: function () {

  }
};
