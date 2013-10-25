/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
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
  $('.first-name').text(top.Ts.System.User.FirstName);


  $('.welcome-new-user').click(function (e) {
    e.preventDefault();
    top.Ts.MainPage.openUser(top.Ts.System.User.UserID);
  });
  $('.welcome-new-customer').click(function (e) {
    e.preventDefault();
    top.Ts.MainPage.openCustomer(-1);
  });
  $('.welcome-new-ticket').click(function (e) {
    e.preventDefault();
    top.Ts.MainPage.newTicket();
  });

  //top.Ts.Services.Organizations.GetPortalOption(top.Ts.System.Organization.OrganizationID, function (portalOption) { });
  //$('.welcome-view-portal').click(function (e) { });

  $('.welcome-view-portal').attr('href', 'https://portal.teamsupport.com/?OrganizationID=' + top.Ts.System.Organization.OrganizationID);

  $('.welcome-test-chat').click(function (e) {
    e.preventDefault();

    window.open(
      top.Ts.System.AppDomain + '/Chat/ChatInit.aspx?uid=' + top.Ts.System.Organization.ChatID,
      'TSChat',
      'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=450,height=500'
      );
  });

  $('.welcome-hide').click(function (e) {
    e.preventDefault();
    top.Ts.MainPage.hideWelcome();
  });

  $('.welcome-payonline').click(function (e) {
    e.preventDefault();
    window.open('https://ticket.teamsupport.com/basicportal.aspx?OrgName=TeamSupportBilling', 'TSPayment');
  });


  var sysEmail = top.Ts.System.Organization.SystemEmailID + '@teamsupport.com';

  $('.welcome-sysemail')
    .text(sysEmail)
    .attr('href', 'mailto:' + sysEmail);

  $('.welcome-setup-email').click(function (e) {
    e.preventDefault();
    top.Ts.MainPage.openAdmin('Email');

  });

  $('.welcome-setup-tickettypes').click(function (e) {
    e.preventDefault();
    top.Ts.Settings.User.write('SelectedCustomPropertyValue', 5, function () {
      top.Ts.MainPage.openAdmin('Custom Properties');
    });

  });

  $('.welcome-setup-ticketstatuses').click(function (e) {
    e.preventDefault();
    top.Ts.Settings.User.write('SelectedCustomPropertyValue', 4, function () {
      top.Ts.MainPage.openAdmin('Custom Properties');
    });
  });

  $('.welcome-setup-custom-fields').click(function (e) {
    e.preventDefault();
    top.Ts.MainPage.openAdmin('Custom Fields');
  });

  $('.welcome-setup-workflow').click(function (e) {
    e.preventDefault();
    top.Ts.MainPage.openAdmin('Workflow');
  });

  $('.welcome-setup-portals').click(function (e) {
    e.preventDefault();
    top.Ts.MainPage.openAdmin('My Portal');
  });

  $('.welcome-help-ticket').click(function (e) {
    e.preventDefault();
    top.Ts.System.openSupport();
  });

  $('.welcome-help-chat').click(function (e) {
    e.preventDefault();
    top.Ts.System.logAction('Helcome - Page Hidden');
    window.open('https://app.teamsupport.com/Chat/ChatInit.aspx?uid=22bd89b8-5162-4509-8b0d-f209a0aa6ee9', 'TSChat', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=450,height=500');
  });

  

};


WelcomePage.prototype = {
  constructor: WelcomePage,
  refresh: function () {

  }
};
