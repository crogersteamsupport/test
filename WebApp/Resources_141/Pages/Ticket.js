/// <reference path="ts/ts.js" />
/// <reference path="ts/ts.services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="~/Default.aspx" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.ui.ticketgrid.js" />

$(document).ready(function () {
  var self = this;
  $('button').button();
  $('#btnRefresh').click(function (e) { e.preventDefault(); window.location = window.location; });
  //$('.ts-loading').hide().next().show();
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
});

