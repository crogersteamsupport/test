/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="~/Default.aspx" />

var waterCoolerPage = null;
$(document).ready(function () {
  waterCoolerPage = new WaterCoolerPage();

});

function onShow() {
  waterCoolerPage.refresh();
};


WaterCoolerPage = function () {
  // Get the top 25 WC threads and display them
  // this funciton is on the webservice in the AppCode dir.  I just returns a WC object in that file
  top.Ts.Services.WaterCooler.GetThreads(function (threads) {
    var threadContainer = $('<div>');
    


    // insert the threads into the div
    $('.wc-threads').empty().append(threadContainer);
    $('.loading-section').hide().next().show();
  });


  // delete link event
  $('.wc-threads').delegate('.wc-delete-link', 'click', function (e) {
    var parent = $(this).closest('.wc-message');
    var message = parent.data('message');
    top.Ts.Services.WaterCooler.DeleteMessage(message.MessageID, function (result) {
      if (result == true) parent.remove();
    });

  });


  // set up the refresh button so we can just click that to see our dev changes
  $('#btnRefresh').click(function (e) { e.preventDefault(); window.location = window.location; }).toggle(window.location.hostname.indexOf('127.0.0.1') > -1);

  // change the style of some stuff
  $('button').button();
  $('a').addClass('ui-state-default ts-link');


 };


WaterCoolerPage.prototype = {
  constructor: WaterCoolerPage,
  refresh: function () {

  }
};
