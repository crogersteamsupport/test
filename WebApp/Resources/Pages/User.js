/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="~/Default.aspx" />

var userPage = null;
$(document).ready(function () {
  userPage = new UserPage();
  userPage.refresh();
});

function onShow() {
  userPage.refresh();
};


UserPage = function () {
  $('#btnRefresh')
  .click(function (e) {
    e.preventDefault();
    window.location = window.location;
  })
  .toggle(window.location.hostname.indexOf('127.0.0.1') > -1);

  $('button').button();
  $('a').addClass('ui-state-default ts-link');
  var userID = top.Ts.Utils.getQueryValue("userid", window);
  top.Ts.Services.Users.GetUser(userID, function (user) {
    $('.user-displayname').text(user.FirstName + ' ' + user.LastName);

  });
  $('.loading-section').hide().next().show();
};


UserPage.prototype = {
  constructor: UserPage,
  refresh: function () {

  }
};
